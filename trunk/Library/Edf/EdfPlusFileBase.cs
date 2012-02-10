//
// NeuroLoopGain Library
// Library containing helper classes used to implement the NeuroLoopGain analysis.
//
// Copyright 2012 Marco Roessen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NeuroLoopGainLibrary.DateTimeTypes;
using NeuroLoopGainLibrary.Errorhandling;

namespace NeuroLoopGainLibrary.Edf
{
  public delegate void OpenStreamEvent(object sender, ref Stream aStream, ref bool isNewStreamOwner);
  public delegate void EdfPrereadNotifyEvent(object sender, bool destroying);

  public abstract class EdfPlusFileBase : BaseEdfFileBase
  {
    #region Constructors & destructors

    protected EdfPlusFileBase()
    {
      _talPrereadError = new ErrorMessage();
      _prereadTALs = PrereadTALs.None;
      _tal = new EdfPlusAnnotationList(0) { OnTALGetData = ReadTALs };
    }

    protected EdfPlusFileBase(string aFileName, bool doOpenReadOnly, bool doUseMemoryStream, PrereadTALs aPrereadTALs, bool doOpen)
      : this()
    {
      FileName = aFileName;
      OpenReadOnly = doOpenReadOnly;
      PrereadTALs = aPrereadTALs;
      UseMemoryStream = doUseMemoryStream;
      if (doOpen)
        Active = true;
    }

    ~EdfPlusFileBase()
    {
      DisableTALsPrereadThread();
      PrereadTALsThreadEnd = null;
      PrereadTALsFinished = null;
      if (Active && !OpenReadOnly)
        WriteModifiedTALBlocks();
    }
    #endregion

    #region Private members
    //private event EventHandler _onPrereadTALsFinished;
    //private event EdfPrereadNotifyEvent _onPrereadTALsThreadEnd;
    //private event EventHandler _onPrereadTALsThreadStart;
    private bool _prereadingTALs;
    private readonly ErrorMessage _talPrereadError;
    #endregion

    #region Protected members

    private List<int> _annotationSignalNrs;
    private PrereadTALs _prereadTALs;
    private Thread _prereadThread;
    private readonly EdfPlusAnnotationList _tal;
    private short[] _talDataBuffer;

    protected override EdfFileInfoBase CreateFileInfo()
    {
      return new EdfPlusFileInfo(this);
    }

    protected override EdfSignalInfoBase CreateSignalInfo()
    {
      return new EdfPlusSignalInfo(this);
    }

    protected void DisableTALsPrereadThread()
    {
      DisableTALsPrereadThread(false);
    }

    protected void DisableTALsPrereadThread(bool destroying)
    {
      if (_prereadThread != null)
      {
        // TODO: do something with OnTerminate (prereadthread)
        //if (destroying)
        //  _prereadThread.OnTerminate = null;
        _prereadThread.Abort();
        _prereadThread.Join();
        OnPrereadTALsThreadEnd(destroying);
      }
    }

    protected override void DoCloseFile()
    {
      DisableTALsPrereadThread();
      base.DoCloseFile();
    }

    protected override void DoCommitChanges()
    {
      base.DoCommitChanges();
      GetAnnotationsSignalNrs();
    }

    // CreateNewFile always creates a file with ANrSignals+1 signals. This (last) extra
    // signal is the EDF+ Annotations signal. Also is the SignalInfo for this signal
    // filled with the required values and a default value of 20 bytes/datablock
    protected override void DoCreateNewFile(int nrSignals)
    {
      DoCreateNewFile(nrSignals, true);
    }

    protected override void DoCreateNewFile(int nrSignals, bool overwrite)
    {
      if (!UseMemoryStream)
      {
        string s = Path.GetExtension(FileName);
        if (string.IsNullOrEmpty(s) || (s.ToLower() != ".edf"))
          throw new EdfPlusFileNameExtException(DataFileConsts.EDFPlusFileNameExt);
      }
      base.DoCreateNewFile(nrSignals + 1, overwrite);
      for (int i = 0; i < nrSignals + 1; i++)
      {
        EdfPlusSignalInfo newSignalDef = new EdfPlusSignalInfo(this);
        SignalInfo.Add(newSignalDef);
      }
      DoOpenFile();
      FileInfo.NrSignals = nrSignals + 1;
      FileInfo.NrDataRecords = 0;
      if (_annotationSignalNrs == null)
        _annotationSignalNrs = new List<int>(1);
      else
        _annotationSignalNrs.Capacity = 1;
      AnnotationSignalNrs.Add(nrSignals);
      SignalInfo[nrSignals].SignalLabel = EdfConstants.AnnotationsSignalLabel;
      SignalInfo[nrSignals].PhysiMin = 0;
      SignalInfo[nrSignals].PhysiMax = 1;
      SignalInfo[nrSignals].DigiMin = -32768;
      SignalInfo[nrSignals].DigiMax = 32767;
      SignalInfo[nrSignals].NrSamples = 10; // default value, large enough for time keeping events
      FileInfo.SignalDataIsContinuous = true;
    }

    protected override void DoDataBufferSizeChanged()
    {
      base.DoDataBufferSizeChanged();
      _talDataBuffer = CreateDataBuffer();
    }

    protected void DoHandlePrereadTALsFinished(object sender)
    {
      _prereadThread = null;
      _prereadingTALs = false;
      OnPrereadTALsFinished();
    }

    protected override void DoOpenFile()
    {
      _tal.Clear();
      base.DoOpenFile();
      // Extended DataRecordDuration valid check
      // DataRecDuration can be '0' if either:
      //   1) Each ordinary signal only 1 sample/block and file is discontinuous
      //   2) Only time stamped event signals
      if (ValidFormat && (FileInfo.SampleRecDuration == 0))
      {
        int count = 0;
        int i = 0;
        while (ValidFormat && (i < FileInfo.NrSignals))
        {
          if (!((EdfPlusSignalInfo)SignalInfo[i]).IsAnnotationSignal)
          {
            count++;
            if (SignalInfo[i].NrSamples > 1)
              ValidFormat = false;
          }
          i++;
        }
        if (ValidFormat && (count > 0) && FileInfo.SignalDataIsContinuous)
          ValidFormat = false;
        if (!ValidFormat)
          FileInfo.FieldValid[(int)EdfFileInfoBase.Field.SampleRecDuration] = false;
      }
      if (ValidFormat)
      {
        TAL.Clear();
        TAL.BlockDuration = FileInfo.SampleRecDuration;
        TAL.SetBlocksListCapacity(FileInfo.NrDataRecords);
        GetAnnotationsSignalNrs();
        bool readOk = true;
        // Always preread first and last block (for getting file time)
        ReadTALs(this, 0, ref readOk);
        if (readOk && (FileInfo.NrDataRecords > 1))
          ReadTALs(this, FileInfo.NrDataRecords - 1, ref readOk);
        if (readOk)
        {
          if ((TAL.Block(0).DataRecOnset < 0) || (TAL.Block(0).DataRecOnset >= 1))
            HeaderError |= EdfHeaderErrorType.EdfPlusAnnotationFirstOffsetError;
          try
          {
            FileInfo.FirstDataBlockOnset = TAL.Block(0).DataRecOnset;
          }
          catch
          {
          }
          // Use the fileheader startdate/time, not the startdate/time of the first datablock!
          TAL.FileStartDateTime = FileInfo.StartDateTime.HasValue ? new HPDateTime(FileInfo.StartDateTime.Value) : new HPDateTime();
          switch (PrereadTALs)
          {
            case PrereadTALs.None:
              break;
            case PrereadTALs.OnOpenFile:
              TALPrereader(null);
              DoHandlePrereadTALsFinished(this);
              break;
            case PrereadTALs.UseThread:
              StartTALsPrereadThread();
              break;
            default:
              break;
          }
        }
        else
        {
          ValidFormat = false;
          HeaderError |= EdfHeaderErrorType.EdfPlusAnnotationSignalError;
        }
      }
    }

    protected void DoReadTALs(int blockNr, short[] buffer, ref bool readOk)
    {
      readOk = true;
      for (int i = 0; i < AnnotationSignalNrs.Count; i++)
        readOk = TAL.ReadFromBuffer(blockNr, ref buffer,
                                    SignalInfo[AnnotationSignalNrs[i]].BufferOffset,
                                    SignalInfo[AnnotationSignalNrs[i]].NrSamples * sizeof(short), i);
    }

    protected void GetAnnotationsSignalNrs()
    {
      if (_annotationSignalNrs == null)
        _annotationSignalNrs = new List<int>();
      else
        _annotationSignalNrs.Clear();
      int i = 0;
      while (i < FileInfo.NrSignals)
      {
        if (((EdfPlusSignalInfo)SignalInfo[i]).IsAnnotationSignal)
          _annotationSignalNrs.Add(i);
        i++;
      }
    }

    protected void GetAnnotationsSignalNr(int signalNr)
    {
      int result = -1;
      int i = 0;
      while ((result < 0) && (i < _annotationSignalNrs.Count))
      {
        if (_annotationSignalNrs[i] == signalNr)
          result = i;
        else
          i++;
      }
    }

    public override bool ValidFormat
    {
      get
      {
        bool result = base.ValidFormat;
        // Edf+ file has to have at least 1 annotation signal
        if ((AnnotationSignalNrs == null) || (AnnotationSignalNrs.Count == 0))
        {
          result = false;
          HeaderError |= EdfHeaderErrorType.EdfPlusMissingAnnotationSignal;
        }
        if (result && !ValidFormatCheckBusy)
        {
          // Check other things like blocksize etc.
          try
          {
            ValidFormatCheckBusy = true;
            if (DataBlockSize * sizeof(short) > EdfConstants.RecommendedMaxBlockSize)
            {
              if (StrictChecking)
                result = false;
              HeaderError |= EdfHeaderErrorType.EdfRecommendedRecordSizeError;
            }
            if (result)
            {
              if (FileHandle != null)
              {
                if ((FileInfo.NrDataRecords * DataBlockSize * sizeof(short) + FileInfo.HeaderBytes) != FileSize)
                {
                  result = false;
                  HeaderError |= EdfHeaderErrorType.EdfFileSizeMismatchError;
                }
              }
            }
          }
          finally
          {
            ValidFormatCheckBusy = false;
          }
        }
        return result;
      }
      internal set
      {
        base.ValidFormat = value;
      }
    }

    protected override void ReadSignalInfo()
    {
      base.ReadSignalInfo();
      if (_annotationSignalNrs == null)
        _annotationSignalNrs = new List<int>();
      _annotationSignalNrs.Clear();
      if (SignalInfo.Count == FileInfo.NrSignals)
        for (int i = 0; i < FileInfo.NrSignals; i++)
        {
          if (((EdfPlusSignalInfo)SignalInfo[i]).IsAnnotationSignal)
            _annotationSignalNrs.Add(i);
        }
    }

    protected int ReadTALs(object sender, int blockNr, ref bool readOk)
    {
      int result = -1;
      if (Active)
      {
        if ((blockNr >= 0) && (blockNr < FileInfo.NrDataRecords))
        {
          try
          {
            // Add a new block to TAL if necessary, set onset to NAN
            TAL.AddBlock(blockNr, double.NaN); // To avoid recursive calls by ReadDataBlock
            DoReadDataBlock(blockNr, ref _talDataBuffer);
            DoReadTALs(blockNr, _talDataBuffer, ref readOk);
            result = blockNr;
          }
          catch { }
        }
        if (result < 0)
        {
          readOk = false;
          result = FileInfo.NrDataRecords;
        }
      }
      return result;
    }

    protected void StartTALsPrereadThread()
    {
      DisableTALsPrereadThread();
      _prereadThread = new Thread(TALPrereader);
      _prereadThread.Start();
      _prereadingTALs = true;
      OnPrereadTALsThreadStart();
    }

    protected void TALPrereader(object data)
    {
      TALPrereadError.Clear();
      int currentBlockIndex = 0;
      bool finished = false;
      try
      {
        while (!finished && !TALPrereadError.Signaled)
        {
          // Get all TAL blocks one-by-one; this will read data from file if not available
          TAL.LastError = TalError.None;
          TAL.Block(currentBlockIndex); // read the data, ignore the result
          currentBlockIndex++;
          finished = (currentBlockIndex >= FileInfo.NrDataRecords);
          if ((TAL.LastError & TalError.BlockOrder) == TalError.BlockOrder)
            TALPrereadError.Add(EdfConstants.DataRecordsNotOrdered, EdfConstants.ErrorIdEdfPlusBlockOrder);
        }
      }
      catch (ThreadAbortException)
      {
        // Thread was stopped
      }
      catch (Exception ex)
      {
        TALPrereadError.Add(ex.Message, ErrorMessage.EIdException);
      }
      DoHandlePrereadTALsFinished(this);
    }

    protected void WriteTALs(object sender, int blockNr, ref bool writeOk)
    {
      if ((blockNr >= 0) && (blockNr <= FileInfo.NrDataRecords))
      {
        try
        {
          writeOk = true;
          if (blockNr < FileInfo.NrDataRecords)
            DoReadDataBlock(blockNr, ref _talDataBuffer);
          else
            _talDataBuffer = CreateDataBuffer();
          for (int i = 0; i < AnnotationSignalNrs.Count; i++)
          {
            writeOk = TAL.WriteToBuffer(
              blockNr, ref _talDataBuffer, SignalInfo[AnnotationSignalNrs[i]].BufferOffset,
              SignalInfo[AnnotationSignalNrs[i]].NrSamples * sizeof(short), i);
          }
          if (writeOk)
          {
            DoWriteDataBlock(blockNr, _talDataBuffer);
            TAL.Block(blockNr).Modified = false;
          }
        }
        catch
        {
          writeOk = false;
        }
      }
      else
        writeOk = false;
    }

    protected Thread PrereadThread { get { return _prereadThread; } }

    public event EdfPrereadNotifyEvent PrereadTALsThreadEnd;
    protected void OnPrereadTALsThreadEnd(bool destroying)
    {
      if (PrereadTALsThreadEnd != null)
        PrereadTALsThreadEnd(this, destroying);
    }

    public event EventHandler PrereadTALsThreadStart;
    protected void OnPrereadTALsThreadStart()
    {
      if (PrereadTALsThreadStart != null)
        PrereadTALsThreadStart(this, EventArgs.Empty);
    }

    public event EventHandler PrereadTALsFinished;
    protected void OnPrereadTALsFinished()
    {
      if (PrereadTALsFinished != null)
        PrereadTALsFinished(this, EventArgs.Empty);
    }
    #endregion

    #region Public members
    public int GetAnnotationSignalNr(int signalNr)
    {
      for (int i = 0; i < _annotationSignalNrs.Count; i++)
      {
        if (_annotationSignalNrs[i] == signalNr)
          return i;
      }
      return -1;
    }

    public bool WriteModifiedTALBlocks()
    {
      if (OpenReadOnly)
        throw new ArgumentException(EdfConstants.FileIsReadOnly);
      if (!Active)
        return false;
      int i = 0;
      bool writeOk = true;
      if (AnnotationSignalNrs.Count > 0)
      {
        while (writeOk && (i < _tal.BlockCount))
        {
          EdfPlusAnnotationDataBlock talBlock;
          if (TAL.BlockInMemory(i))
          {
            talBlock = TAL.Block(i);
            if (talBlock.Modified)
              WriteTALs(this, talBlock.DataRecNr, ref writeOk);
          }
          i++;
        }
      }
      return writeOk;
    }

    #endregion

    #region Public properties
    public List<int> AnnotationSignalNrs { get { return _annotationSignalNrs; } }

    public ErrorMessage TALPrereadError { get { return _talPrereadError; } }

    public bool PrereadingTALs { get { return _prereadingTALs; } }

    public PrereadTALs PrereadTALs
    {
      get { return _prereadTALs; }
      set
      {
        if (PrereadTALs == PrereadTALs.UseThread)
          DisableTALsPrereadThread();
        _prereadTALs = value;
        if (Active)
        {
          switch (PrereadTALs)
          {
            case PrereadTALs.OnOpenFile:
              TALPrereader(null);
              break;
            case PrereadTALs.UseThread:
              StartTALsPrereadThread();
              break;
            default:
              break;
          }
        }
      }
    }

    public EdfPlusAnnotationList TAL { get { return _tal; } }

    #endregion
  }
}
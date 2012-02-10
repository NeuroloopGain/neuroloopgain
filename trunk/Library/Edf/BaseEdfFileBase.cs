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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NeuroLoopGainLibrary.Errorhandling;
using NeuroLoopGainLibrary.IO;
using NeuroLoopGainLibrary.Text;

namespace NeuroLoopGainLibrary.Edf
{
  public abstract class BaseEdfFileBase
  {
    #region private fields

    private bool _active;
    private bool _creating;
    private int _dataBlockSize;
    private char _decimalSeparator = EdfConstants.DefaultDecimalSeparator;
    private readonly ErrorMessage _error;
    private readonly object _fileAccess;
    private Stream _fileHandle;
    private EdfFileInfoBase _fileInfo;
    private string _fileName;
    private bool _openReadOnly;
    private bool _ownsStream;
    private readonly List<EdfSignalInfoBase> _signalInfo = new List<EdfSignalInfoBase>();
    private bool _strictChecking = true;
    private char _thousandSeparator = EdfConstants.DefaultThousandSeparator;
    private bool _validFormat;

    #endregion private fields

    #region Private Methods

    private long GetFileSize()
    {
      lock (FileAccess)
      {
        return FileHandle.Length;
      }
    }

    private MemoryStream GetMemoryHandle()
    {
      return (MemoryStream)_fileHandle;
    }

    #endregion Private Methods

    #region protected fields

    protected const int ShortSize = sizeof(short);

    #endregion protected fields

    #region protected properties

    protected bool CheckVersionOnOpen { get; set; }

    protected int DataBlockSize
    {
      get
      {
        return _dataBlockSize;
      }
      set
      {
        SetDataBlockSize(value);
      }
    }

    protected char DecimalSeparator
    {
      get
      {
        return _decimalSeparator;
      }
      set
      {
        SetDecimalSeparator(value);
      }
    }

    protected ErrorMessage Error
    {
      get
      {
        return _error;
      }
    }

    protected object FileAccess
    {
      get
      {
        return _fileAccess;
      }
    }

    protected virtual OpenStreamEvent OnOpenStream { get; set; }

    protected bool StrictChecking
    {
      get
      {
        return _strictChecking;
      }
      set
      {
        SetStrictChecking(value);
      }
    }

    protected char ThousandSeparator
    {
      get
      {
        return _thousandSeparator;
      }
      set
      {
        SetThousandSeparator(value);
      }
    }

    protected bool ValidFormatCheckBusy { get; set; }

    #endregion protected properties

    #region protected methods

    protected virtual void CalculateDataBlockSize()
    {
      int sum = SignalInfo.Sum(aSignalInfo => aSignalInfo.NrSamples);
      DataBlockSize = sum;
    }

    protected void CalculateNrDataRecords()
    {
      if (!ValidFormat)
        return;
      CalculateDataBlockSize();
      FileInfo.NrDataRecords = (int)(FileSize - FileInfo.HeaderBytes) / DataBlockSize / sizeof(short);
    }

    protected bool CheckHeader()
    {
      if (Active || Creating)
        return FileInfo.DataValid;
      return false;
    }

    protected bool CheckSignals()
    {
      bool result = false;
      if (Active || Creating)
      {
        foreach (var aSignalInfo in SignalInfo)
        {
          result = aSignalInfo.DataValid;
          if (!result)
            break;
        }
      }
      return result;
    }

    protected short[] CreateDataBuffer()
    {
      return ValidFormat ? new short[DataBlockSize] : null;
    }

    protected abstract EdfFileInfoBase CreateFileInfo();

    protected abstract EdfSignalInfoBase CreateSignalInfo();

    protected virtual void DoCloseFile()
    {
      if (!Active && !Creating)
        return;
      if (Modified && !OpenReadOnly)
        CommitChanges();
      if (_ownsStream)
      {
        _fileHandle.Close();
        _fileHandle = null;
      }
      else
        _fileHandle = null;
      _active = false;
    }

    protected virtual void DoCommitChanges()
    {
      Debug.Assert(!OpenReadOnly, EdfConstants.FileIsReadOnly);
      if (HeaderModified())
        WriteHeaderInfo();
      if (SignalModified())
      {
        WriteSignalInfo();
        DoDataBufferSizeChanged();
      }
      if (!Creating || !ValidFormat)
        return;
      Creating = false;
      _active = true;
      _validFormat = true; // ?? TODO: Ask what use this is ??
      CalculateDataBlockSize();
      DoDataBufferSizeChanged();
    }

    protected virtual void DoCreateNewFile(int nrSignals)
    {
      DoCreateNewFile(nrSignals, true);
    }

    protected virtual void DoCreateNewFile(int nrSignals, bool overwrite)
    {
      Debug.Assert((nrSignals > 0), EdfConstants.NrSignalsShouldBeMoreThan0);
      if (Active)
        throw new DataFileIsOpenException(DataFileConsts.DataFileIsOpen);
      if (!UseMemoryStream)
      {
        if (FileName.Trim() == string.Empty)
          throw new DataFileException(EdfConstants.NoFileName);
        if (File.Exists(FileName))
        {
          if (!overwrite)
            throw new DataFileExistsException(DataFileConsts.DataFileDataExists);
          File.Delete(FileName);
        }
      }
      OpenReadOnly = false;
      FileInfo = null;
      SignalInfo.Clear();
      Creating = true;
    }

    protected virtual void DoDataBufferSizeChanged()
    {
      // Recalculate signal databuffer offsets and datablocksize
      if (!ValidFormat)
        return;
      int index = 0;
      for (int i = 0; i < FileInfo.NrSignals; i++)
      {
        SignalInfo[i].BufferOffset = index;
        index += SignalInfo[i].NrSamples;
      }
      CalculateDataBlockSize();
    }

    protected bool DoFloatSeparatorDetection()
    {
      bool result = false;
      // Check if the float-values are the problem
      if (FileInfo != null)
      {
        bool b = !FileInfo.FieldValid[(int)EdfFileInfoBase.Field.SampleRecDuration];
        for (int i = 0; i < FileInfo.NrSignals; i++)
        {
          b = b || !SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.PhysiMin];
          b = b || !SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.PhysiMax];
        }
        if (b) // There is a float problem!
        {
          char tmpDecimalSeparator = char.MinValue, tmpThousandSeparator = char.MinValue;
          b = false; // First try detection on fileheader
          if (!FileInfo.FieldValid[(int)EdfFileInfoBase.Field.SampleRecDuration])
          {
            b = TextUtils.DetectFloatSeparator(
              FileInfo.FileInfoRecord.SampleRecDuration, out tmpDecimalSeparator, out tmpThousandSeparator);
          }
          int i = 0;
          while (!b && (i < SignalInfo.Count))
          {

            if (!SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.PhysiMin])
              b = TextUtils.DetectFloatSeparator(
                SignalInfo[i].SignalInfoRecord.PhysiMin, out tmpDecimalSeparator, out tmpThousandSeparator);
            if (!b && (!SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.PhysiMax]))
              b = TextUtils.DetectFloatSeparator(
                SignalInfo[i].SignalInfoRecord.PhysiMax, out tmpDecimalSeparator, out tmpThousandSeparator);
            i++;
          }
          if (b)
          {
            DecimalSeparator = tmpDecimalSeparator;
            ThousandSeparator = tmpThousandSeparator;
          }
          else
          {
            // If no absolutely sure detected decimal and thousand-float separators then try
            // to detect an absolutely sure decimal separator
            // First try detection on fileheader
            if (!FileInfo.FieldValid[(int)EdfFileInfoBase.Field.SampleRecDuration])
            {
              tmpDecimalSeparator = TextUtils.DetectDecimalSeparator(FileInfo.FileInfoRecord.SampleRecDuration);
              b = (tmpDecimalSeparator != char.MinValue);
            }
            i = 0;
            while (!b && (i < SignalInfo.Count))
            {
              if (!SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.PhysiMin])
              {
                tmpDecimalSeparator = TextUtils.DetectDecimalSeparator(SignalInfo[i].SignalInfoRecord.PhysiMin);
                b = tmpDecimalSeparator != char.MinValue;
              }
              if (!b && !SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.PhysiMax])
              {
                tmpDecimalSeparator = TextUtils.DetectDecimalSeparator(SignalInfo[i].SignalInfoRecord.PhysiMax);
                b = tmpDecimalSeparator != char.MinValue;
              }
              i++;
            }
            // If we can't even detect an absolutely sure decimal separator then we have to
            // use the best guess....
            if (!b)
            {
              if (!FileInfo.FieldValid[(int)EdfFileInfoBase.Field.SampleRecDuration])
              {
                tmpDecimalSeparator = TextUtils.DetectDecimalSeparator(FileInfo.FileInfoRecord.SampleRecDuration, true);
                b = (tmpDecimalSeparator != char.MinValue);
              }
              i = 0;
              while (!b && (i < SignalInfo.Count))
              {
                if (!SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.PhysiMin])
                {
                  tmpDecimalSeparator = TextUtils.DetectDecimalSeparator(SignalInfo[i].SignalInfoRecord.PhysiMin, true);
                  b = (tmpDecimalSeparator != char.MinValue);
                }
                if (!b && !SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.PhysiMax])
                {
                  tmpDecimalSeparator = TextUtils.DetectDecimalSeparator(SignalInfo[i].SignalInfoRecord.PhysiMax, true);
                  b = (tmpDecimalSeparator != char.MinValue);
                }
                i++;
              }
            }
            if (b)
              DecimalSeparator = tmpDecimalSeparator;
          }
          result = b;
        }
        // else it is not a float problem
      }
      return result;
    }

    protected virtual void DoOpenFile()
    {
      if (Active)
        return;
      DoOpenStream();
      if (!Creating)
      {
        _active = true;
        if (_fileHandle.Length >= 256)
        {
          try
          {
            ReadHeaderInfo();
            if (!CheckVersionOnOpen || FileInfo.FieldValid[(int)EdfFileInfoBase.Field.Version])
              ReadSignalInfo();
            _validFormat = (CheckHeader() && CheckSignals());
          }
          catch (Exception)
          {
            _validFormat = false;
          }
        }
        else
          _validFormat = false;
        if (ValidFormat)
          HeaderError = EdfHeaderErrorType.None;
        else
        {
          HeaderError = !CheckHeader()
                          ? EdfHeaderErrorType.EdfFileInfoError
                          : EdfHeaderErrorType.EdfSignalInfoError;
        }
        if (!(ValidFormat || StrictChecking))
        {
          if (DoFloatSeparatorDetection())
            _validFormat = (CheckHeader() && CheckSignals());
        }
        if (ValidFormat)
        {
          CalculateNrDataRecords();
          DoDataBufferSizeChanged();
        }
        else
        {
          DataBlockSize = 0;
          FileInfo.NrDataRecords = 0;
        }
      }
    }

    protected virtual void DoOpenStream()
    {
      _ownsStream = true;
      if (OnOpenStream != null)
        OnOpenStream(this, ref _fileHandle, ref _ownsStream);
      if (_fileHandle != null)
        return;
      if (UseMemoryStream)
      {
        _fileHandle = new MemoryStream();
        if (File.Exists(_fileName))
        {
          FileStream fs = new FileStream(_fileName, FileMode.Open, System.IO.FileAccess.Read);
          _fileHandle.SetLength(fs.Length);
          fs.Read(((MemoryStream)_fileHandle).GetBuffer(), 0, (int)fs.Length);
          fs.Close();
        }
      }
      else
      {
        _fileHandle = OpenReadOnly
                        ? new MinimalReadBufferSizeFileStream(_fileName, FileMode.Open, System.IO.FileAccess.Read, FileShare.Read)
                        : new MinimalReadBufferSizeFileStream(_fileName, FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
      }
    }

    protected virtual bool DoReadDataBlock(int blockNr, ref short[] buffer)
    {
      if (!Active || (blockNr < 0) || (blockNr > FileInfo.NrDataRecords) || (buffer.Length != DataBlockSize))
        return false;
      try
      {
        long dataBlockOffset = FileInfo.HeaderBytes + (blockNr * DataBlockSize * sizeof(short));
        // Read data to the buffer
        lock (FileAccess)
        {
          int nrToRead = DataBlockSize * sizeof(short);
          if (FileHandle.Position != dataBlockOffset)
            FileHandle.Seek(dataBlockOffset, SeekOrigin.Begin);
          byte[] tmpBuffer = new byte[nrToRead];
          FileHandle.Read(tmpBuffer, 0, tmpBuffer.Length);
          Debug.Assert(FileHandle.Position == dataBlockOffset + nrToRead);
          int iEnd = tmpBuffer.Length / 2;
          for (int i = 0; i < iEnd; i++)
            buffer[i] = (short)(tmpBuffer[i * 2 + 1] * 256 + tmpBuffer[i * 2]);
        }
        return true;
      }
      catch
      {
        return false;
      }
    }

    protected virtual bool DoWriteDataBlock(int blockNr, short[] buffer)
    {
      if (!Active || OpenReadOnly || (blockNr < 0) || (blockNr > FileInfo.NrDataRecords) || (buffer.Length != DataBlockSize))
        return false;
      try
      {
        // Calculate the file DataBlockOffset of the data to be written
        //long dataBlockOffset = FileInfo.HeaderBytes + (blockNr * DataBlockSize * sizeof(short));
        long dataBlockOffset = FileInfo.HeaderBytes + ((long)blockNr * DataBlockSize * ShortSize);

        lock (FileAccess)
        {
          if (FileHandle.Position != dataBlockOffset)
            FileHandle.Seek(dataBlockOffset, SeekOrigin.Begin);
          BinaryWriter bw = new BinaryWriter(FileHandle);
          for (int i = 0; i < DataBlockSize; i++)
            bw.Write(buffer[i]);
          if (blockNr == FileInfo.NrDataRecords)
            FileInfo.NrDataRecords++;
        }
        if (!FileInfo.DataExists)
        {
          FileInfo.DataExists = true;
          foreach (var aSignalInfo in SignalInfo)
            aSignalInfo.DataExists = true;
        }
        return true;
      }
      catch
      {
        return false;
      }
    }

    protected BaseEdfFileBase()
    {
      CheckVersionOnOpen = false;
      _error = new ErrorMessage();
      _fileAccess = new object();
      _strictChecking = true;
    }

    ~BaseEdfFileBase()
    {
      Active = false;
      _fileInfo = null;
      _fileHandle = null;
    }

    protected bool GetModified()
    {
      return (HeaderModified() || SignalModified());
    }

    protected EdfSignalInfoBase GetSignalInfo(int index)
    {
      return SignalInfo[index];
    }

    protected bool HeaderModified()
    {
      if (Active || Creating)
        return FileInfo.Modified;
      return false;
    }

    protected void ReadHeaderInfo()
    {
      Debug.Assert(FileHandle != null, DataFileConsts.DataFileIsNotOpen);
      FileInfo = null;
      // ReSharper disable PossibleNullReferenceException
      FileInfo.StrictChecking = StrictChecking;
      // ReSharper restore PossibleNullReferenceException
      EdfFileInfoRaw tmpFileInfo = new EdfFileInfoRaw();
      if (FileSize < 255)
      {
        tmpFileInfo.Version = string.Empty;
        tmpFileInfo.Patient = string.Empty;
        tmpFileInfo.Recording = string.Empty;
        tmpFileInfo.StartDate = string.Empty;
        tmpFileInfo.StartTime = string.Empty;
        tmpFileInfo.HeaderBytes = string.Empty;
        tmpFileInfo.Reserved = string.Empty;
        tmpFileInfo.NrDataRecords = string.Empty;
        tmpFileInfo.SampleRecDuration = string.Empty;
        tmpFileInfo.NrSignals = string.Empty;
      }
      else
      {
        lock (FileAccess)
        {
          FileHandle.Seek(0, SeekOrigin.Begin);
          tmpFileInfo.Version = ReadStringField(8);
          tmpFileInfo.Patient = ReadStringField(80);
          tmpFileInfo.Recording = ReadStringField(80);
          tmpFileInfo.StartDate = ReadStringField(8);
          tmpFileInfo.StartTime = ReadStringField(8);
          tmpFileInfo.HeaderBytes = ReadStringField(8);
          tmpFileInfo.Reserved = ReadStringField(44);
          tmpFileInfo.NrDataRecords = ReadStringField(8);
          tmpFileInfo.SampleRecDuration = ReadStringField(8);
          tmpFileInfo.NrSignals = ReadStringField(4);
        }
      }
      FileInfo.FileInfoRecord = tmpFileInfo;
      if (FileInfo.FieldValid[(int)EdfFileInfoBase.Field.NrSignals])
        FileInfo.DataExists = (FileSize > (FileInfo.NrSignals + 1) * 256);
      else
        FileInfo.DataExists = false;
    }

    protected virtual void ReadSignalInfo()
    {
      Debug.Assert(FileHandle != null, DataFileConsts.DataFileIsNotOpen);
      SignalInfo.Clear();
      int dummy;
      if (FileInfo.FieldValid[(int) EdfFileInfoBase.Field.NrSignals] ||
          int.TryParse(FileInfo.EdfFileInfoRaw.NrSignals.Trim(), out dummy))
      {
        List<EdfSignalInfoRaw> tmpSignalInfo = new List<EdfSignalInfoRaw>();
        for (int i = 0; i < FileInfo.NrSignals; i++)
        {
          EdfSignalInfoBase newSignalDef = CreateSignalInfo();
          newSignalDef.StrictChecking = StrictChecking;
          SignalInfo.Add(newSignalDef);
          tmpSignalInfo.Add(new EdfSignalInfoRaw());
        }
        if (FileSize >= (FileInfo.NrSignals + 1)*256)
        {
          lock (FileAccess)
          {
            FileHandle.Seek(256, SeekOrigin.Begin);
            for (int i = 0; i < FileInfo.NrSignals; i++)
              tmpSignalInfo[i].SignalLabel = ReadStringField(16);
            for (int i = 0; i < FileInfo.NrSignals; i++)
              tmpSignalInfo[i].TransducerType = ReadStringField(80);
            for (int i = 0; i < FileInfo.NrSignals; i++)
              tmpSignalInfo[i].PhysiDim = ReadStringField(8);
            for (int i = 0; i < FileInfo.NrSignals; i++)
              tmpSignalInfo[i].PhysiMin = ReadStringField(8);
            for (int i = 0; i < FileInfo.NrSignals; i++)
              tmpSignalInfo[i].PhysiMax = ReadStringField(8);
            for (int i = 0; i < FileInfo.NrSignals; i++)
              tmpSignalInfo[i].DigiMin = ReadStringField(8);
            for (int i = 0; i < FileInfo.NrSignals; i++)
              tmpSignalInfo[i].DigiMax = ReadStringField(8);
            for (int i = 0; i < FileInfo.NrSignals; i++)
              tmpSignalInfo[i].PreFilter = ReadStringField(80);
            for (int i = 0; i < FileInfo.NrSignals; i++)
              tmpSignalInfo[i].NrSamples = ReadStringField(8);
            for (int i = 0; i < FileInfo.NrSignals; i++)
              tmpSignalInfo[i].Reserved = ReadStringField(32);
          }
          for (int i = 0; i < FileInfo.NrSignals; i++)
          {
            SignalInfo[i].SignalInfoRecord = tmpSignalInfo[i];
            SignalInfo[i].DataExists = FileInfo.DataExists;
          }
        }
        CalculateDataBlockSize();
      }
    }

    protected string ReadStringField(int length)
    {
      lock (FileAccess)
      {
        BinaryReader br = new BinaryReader(FileHandle);
        try
        {
          return new string(br.ReadChars(length));
        }
        catch (Exception)
        {
          return string.Empty;
        }

      }
    }

    protected void SetActive(bool value)
    {
      if (!Active && !Creating)
      {
        if (value)
          DoOpenFile();
      }
      else
      {
        if (!value)
          DoCloseFile();
      }
    }

    protected void SetCreating(bool value)
    {
      _creating = value;
    }

    protected void SetDataBlockSize(int value)
    {
      const int cReqDataBlockSize = 4096;
      _dataBlockSize = value;
      if (!(FileHandle is MinimalReadBufferSizeFileStream))
        return;
      MinimalReadBufferSizeFileStream stream = FileHandle as MinimalReadBufferSizeFileStream;
      if ((DataBlockSize > 0) && (DataBlockSize < cReqDataBlockSize))
        stream.MinimalReadBufferSize = (cReqDataBlockSize / DataBlockSize) * DataBlockSize;
      else
        stream.MinimalReadBufferSize = 0;
    }

    protected virtual void SetDecimalSeparator(char value)
    {
      _decimalSeparator = value;
    }

    protected void SetFileName(string value)
    {
      if (Active || Creating)
        throw new DataFileIsOpenException(DataFileConsts.DataFileIsOpen);
      if (Creating)
        throw new DataFileIsCreatingException(DataFileConsts.DataFileIsCreating);
      _fileName = value;
    }

    protected void SetOpenReadOnly(bool value)
    {
      if (Active)
        throw new DataFileIsOpenException(DataFileConsts.DataFileIsOpen);
      if (Creating)
        throw new DataFileIsCreatingException(DataFileConsts.DataFileIsCreating);
      _openReadOnly = value;
    }

    protected void SetStrictChecking(bool value)
    {
      _strictChecking = value;
      if (!Active && !Creating)
        return;
      if (FileInfo != null)
        FileInfo.StrictChecking = StrictChecking;
      foreach (var aSignalInfo in SignalInfo)
        aSignalInfo.StrictChecking = StrictChecking;
      _validFormat = (CheckHeader() && CheckSignals());
    }

    protected virtual void SetThousandSeparator(char value)
    {
      _thousandSeparator = value;
    }

    protected bool SignalModified()
    {
      if ((Active || Creating) && (SignalInfo.Count > 0))
      {
        return SignalInfo.Any(aSignalInfo => aSignalInfo.Modified);
      }
      return false;
    }

    protected void WriteHeaderInfo()
    {
      if (!FileInfo.StartTime.HasValue)
        throw new ArgumentNullException("fileinfo starttime");
      if (!FileInfo.StartDate.HasValue)
        throw new ArgumentNullException("fileinfo startdate");
      Debug.Assert(FileHandle != null, DataFileConsts.DataFileIsNotOpen);
      lock (FileAccess)
      {
        FileHandle.Seek(0, SeekOrigin.Begin);
        WriteStringField(FileInfo.FieldValid[(int)EdfFileInfoBase.Field.Version], FileInfo.Version, 8);
        WriteStringField(FileInfo.FieldValid[(int)EdfFileInfoBase.Field.Patient], FileInfo.Patient, 80);
        WriteStringField(FileInfo.FieldValid[(int)EdfFileInfoBase.Field.Recording], FileInfo.Recording, 80);
        WriteStringField(FileInfo.FieldValid[(int)EdfFileInfoBase.Field.StartDate], FileInfo.StartDate.Value.ToString("dd.MM.yy"), 8);
        WriteStringField(FileInfo.FieldValid[(int)EdfFileInfoBase.Field.StartTime],
                         string.Format("{0:00}.{1:00}.{2:00}", FileInfo.StartTime.GetValueOrDefault().Hours, FileInfo.StartTime.GetValueOrDefault().Minutes, FileInfo.StartTime.Value.Seconds), 8);
        WriteStringField(FileInfo.FieldValid[(int)EdfFileInfoBase.Field.HeaderBytes], FileInfo.HeaderBytes.ToString(), 8);
        WriteStringField(FileInfo.FieldValid[(int)EdfFileInfoBase.Field.Reserved], FileInfo.Reserved, 44);
        WriteStringField(FileInfo.FieldValid[(int)EdfFileInfoBase.Field.NrDataRecords], FileInfo.NrDataRecords.ToString(), 8);
        WriteStringField(FileInfo.FieldValid[(int)EdfFileInfoBase.Field.SampleRecDuration],
                         TextUtils.DoubleToString(FileInfo.SampleRecDuration, FormatInfo, 8), 8);
        WriteStringField(FileInfo.FieldValid[(int)EdfFileInfoBase.Field.NrSignals], FileInfo.NrSignals.ToString(), 4);
        FileInfo.Modified = false;
      }
    }

    protected void WriteSignalInfo()
    {
      Debug.Assert(FileHandle != null, DataFileConsts.DataFileIsNotOpen);
      lock (FileAccess)
      {
        FileHandle.Seek(256, SeekOrigin.Begin);
        for (int i = 0; i < FileInfo.NrSignals; i++)
          WriteStringField(SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.SignalLabel], SignalInfo[i].SignalLabel, 16);
        for (int i = 0; i < FileInfo.NrSignals; i++)
          WriteStringField(SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.Transducertype], SignalInfo[i].TransducerType, 80);
        for (int i = 0; i < FileInfo.NrSignals; i++)
          WriteStringField(SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.PhysiDim],
                           SignalInfo[i].IsLogFloat ? EdfConstants.LogFloatSignalLabel : SignalInfo[i].PhysiDim, 8);
        for (int i = 0; i < FileInfo.NrSignals; i++)
          WriteStringField(SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.PhysiMin],
                           TextUtils.DoubleToString(SignalInfo[i].PhysiMin, FormatInfo, 8), 8);
        for (int i = 0; i < FileInfo.NrSignals; i++)
          WriteStringField(SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.PhysiMax],
                           TextUtils.DoubleToString(SignalInfo[i].PhysiMax, FormatInfo, 8), 8);
        for (int i = 0; i < FileInfo.NrSignals; i++)
          WriteStringField(SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.DigiMin], SignalInfo[i].DigiMin.ToString(), 8);
        for (int i = 0; i < FileInfo.NrSignals; i++)
          WriteStringField(SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.DigiMax], SignalInfo[i].DigiMax.ToString(), 8);
        for (int i = 0; i < FileInfo.NrSignals; i++)
          WriteStringField(SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.Prefilter], SignalInfo[i].PreFilter, 80);
        for (int i = 0; i < FileInfo.NrSignals; i++)
          WriteStringField(SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.NrSamples], SignalInfo[i].NrSamples.ToString(), 8);
        for (int i = 0; i < FileInfo.NrSignals; i++)
        {
          WriteStringField(SignalInfo[i].FieldValid[(int)EdfSignalInfoBase.Field.Reserved], SignalInfo[i].Reserved, 32);
          SignalInfo[i].Modified = false;
        }
      }
    }

    protected void WriteStringField(bool valid, string s, int length)
    {
      // Make sure the string is exactly length characters long
      if (!valid || (s == null))
        s = new string(' ', length);
      else
        s = s.PadRight(length);
      s = s.Substring(0, length);

      lock (FileAccess)
      {
        FileHandle.Write(Encoding.ASCII.GetBytes(s), 0, s.Length);
      }
    }

    #endregion protected methods

    #region public properties

    public bool Active
    {
      get
      {
        return _active;
      }
      set
      {
        SetActive(value);
      }
    }

    public bool Creating
    {
      get
      {
        return _creating;
      }
      protected set
      {
        SetCreating(value);
      }
    }

    public abstract short[] DataBuffer
    {
      get;
    }

    public Stream FileHandle
    {
      get
      {
        return _fileHandle;
      }
    }

    public EdfFileInfoBase FileInfo
    {
      get { return _fileInfo ?? (_fileInfo = CreateFileInfo()); }
      protected set { _fileInfo = value; }
    }

    public string FileName
    {
      get { return _fileName; }
      set { SetFileName(value); }
    }

    public long FileSize { get { return GetFileSize(); } }

    public IFormatProvider FormatInfo
    {
      get
      {
        NumberFormatInfo ni = new NumberFormatInfo();
        if (DecimalSeparator != char.MinValue)
          ni.NumberDecimalSeparator = DecimalSeparator.ToString();
        if (ThousandSeparator != char.MinValue)
          ni.NumberGroupSeparator = ThousandSeparator.ToString();
        return ni;
      }
    }

    public EdfHeaderErrorType HeaderError { get; protected set; }

    public MemoryStream MemoryHandle { get { return GetMemoryHandle(); } }

    public bool Modified { get { return GetModified(); } }

    public bool OpenReadOnly
    {
      get { return _openReadOnly; }
      set { SetOpenReadOnly(value); }
    }

    public List<EdfSignalInfoBase> SignalInfo
    {
      get { return _signalInfo; }
    }

    public bool UseMemoryStream { get; set; }

    public virtual bool ValidFormat
    {
      get
      {
        HeaderError = EdfHeaderErrorType.None;
        if (Active)
          return _validFormat;
        if (Creating)
          return (CheckHeader() && CheckSignals());
        try
        {
          DoOpenFile();
          return _validFormat;
        }
        finally
        {
          DoCloseFile();
        }
      }
      internal set { _validFormat = value; }
    }

    #endregion public properties

    #region public methods

    public void CommitChanges()
    {
      DoCommitChanges();
    }

    public void CreateNewFile(int nrSignals)
    {
      CreateNewFile(nrSignals, true);
    }

    public void CreateNewFile(int nrSignals, bool overWrite)
    {
      DoCreateNewFile(nrSignals, overWrite);
    }

    public void DiscardChanges()
    {
      ReadHeaderInfo();
      ReadSignalInfo();
      CalculateDataBlockSize();
      DoDataBufferSizeChanged();
    }

    public virtual void ReadDataBlock(int blockNr)
    {
      if (!Active)
        throw new DataFileHeaderInvalidException(DataFileConsts.DataFileHeaderInvalid);
      if ((blockNr < 0) || (blockNr >= FileInfo.NrDataRecords))
        throw new DataFileReadDataException(DataFileConsts.DataFileReadData);
    }

    public virtual void WriteDataBlock(int blockNr)
    {
      if (!Active)
        throw new DataFileHeaderInvalidException(DataFileConsts.DataFileHeaderInvalid);
      if (_openReadOnly)
        throw new DataFileReadOnlyException(DataFileConsts.DataFileReadOnly);
      if ((blockNr < 0) || (blockNr > FileInfo.NrDataRecords))
        throw new DataFileReadDataException(DataFileConsts.DataFileReadData);
    }

    #endregion public methods
  }
}
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
using System.Diagnostics;
using System.Threading;

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfPlusFile : EdfPlusFileBase
  {
    #region Protected Fields

    protected int _currentDataBlock;
    protected short[] _dataBuffer;

    #endregion Protected Fields

    #region Protected Methods

    protected override void DoDataBufferSizeChanged()
    {
      base.DoDataBufferSizeChanged();
      _dataBuffer = CreateDataBuffer();
    }

    #endregion Protected Methods

    #region Constructors

    public EdfPlusFile(string aFileName, bool doOpenReadOnly, bool doUseMemoryStream, PrereadTALs aPrereadTALs, bool doOpen)
      : base(aFileName, doOpenReadOnly, doUseMemoryStream, aPrereadTALs, doOpen) { }

    public EdfPlusFile() { }

    #endregion Constructors

    #region Public Properties

    public int CurrentDataBlock
    {
      get { return _currentDataBlock; }
    }

    public override short[] DataBuffer
    {
      get { return _dataBuffer ?? (_dataBuffer = CreateDataBuffer()); }
    }

    public new OpenStreamEvent OnOpenStream
    {
      get { return base.OnOpenStream; }
      set { base.OnOpenStream = value; }
    }

    public new Thread PrereadThread { get { return base.PrereadThread; } }

    #endregion Public Properties

    #region Public Methods    

    public static bool IsValidFile(string filename, bool doStrictChecking = false)
    {
      try
      {
        EdfPlusFile edf = new EdfPlusFile(filename, true, false, PrereadTALs.None, false) { CheckVersionOnOpen = true };
        return edf.ValidFormat;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public override void ReadDataBlock(int blockNr)
    {
      base.ReadDataBlock(blockNr);
      Debug.Assert(DataBuffer != null, "Object not initialised");
      if (!DoReadDataBlock(blockNr, ref _dataBuffer))
        throw new DataFileReadDataException(DataFileConsts.DataFileReadData);
      if (!TAL.BlockInMemory(blockNr))
      {
        bool b = true;
        DoReadTALs(blockNr, DataBuffer, ref b);
        if (!b)
          throw new DataFileReadDataException(DataFileConsts.DataFileErrorReadingTAL);
      }
    }

    public override void WriteDataBlock(int blockNr)
    {
      base.WriteDataBlock(blockNr);
      Debug.Assert(DataBuffer != null, "Object is not initialized");
      // Update TAL list
      if (!TAL.BlockInMemory(blockNr))
      {
        if (!FileInfo.SignalDataIsContinuous)
          throw new DataFileNotContinuousException(DataFileConsts.DataFileNotContinuous);
        TAL.AddBlock(blockNr, blockNr * FileInfo.SampleRecDuration);
        if (blockNr == 0)
          FileInfo.FirstDataBlockOnset = 0;
      }
      // Write data to file
      if (!DoWriteDataBlock(blockNr, DataBuffer))
        throw new DataFileWriteDataException(DataFileConsts.DataFileWriteData);
    }

    public void WriteDataBlock(int blockNr, double onset)
    {
      base.WriteDataBlock(blockNr);
      Debug.Assert(DataBuffer == null, "Object is not initialized");
      if (blockNr == 0)
      {
        if ((onset < 0) || (onset >= 1))
          throw new DataFileInvalidBlock0OnsetException(DataFileConsts.DataFileInvalidBlock0Onset);
        FileInfo.FirstDataBlockOnset = onset;
      }
      if (FileInfo.SignalDataIsContinuous && (FileInfo.FirstDataBlockOnset + blockNr * FileInfo.SampleRecDuration != onset))
        throw new DataFileContinuousException(DataFileConsts.DataFileContinuous);
      TAL.AddBlock(blockNr, onset); // overwrites offset if block already exists
      // Write data to file
      if (!DoWriteDataBlock(blockNr, DataBuffer))
        throw new DataFileWriteDataException(DataFileConsts.DataFileWriteData);
    }

    #endregion Public Methods
    //public new EventHandler OnPrereadTALsFinished
    //{
    //  get { return base.OnPrereadTALsFinished; }
    //  set { base.OnPrereadTALsFinished = value; }
    //}
    //public new EventHandler OnPrereadTALsThreadStart
    //{
    //  get { return base.OnPrereadTALsThreadStart; }
    //  set { base.OnPrereadTALsThreadStart = value; }
    //}
  }
}

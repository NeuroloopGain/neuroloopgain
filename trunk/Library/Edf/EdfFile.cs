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

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfFile : EdfFileBase
  {
		#region public properties 

    public override short[] DataBuffer
    {
      get { return _dataBuffer ?? (_dataBuffer = CreateDataBuffer()); }
    }

		#endregion public properties 

		#region public methods 

    public static bool IsValidFile(string filename, bool doStrictChecking = false)
    {
      try
      {
        EdfFile edf = new EdfFile(filename,true,false,doStrictChecking,false) {CheckVersionOnOpen = true};
        return edf.ValidFormat;
      }
      catch (Exception)
      {
        return false;
      }
    }

		#endregion public methods 


    #region Constructors
    public EdfFile() { }

    public EdfFile(string aFileName, bool doOpenReadOnly, bool doUseMemoryStream, bool doStrictChecking, bool doOpen)
    {
      FileName = aFileName;
      OpenReadOnly = doOpenReadOnly;
      UseMemoryStream = doUseMemoryStream;
      StrictChecking = doStrictChecking;
      if (doOpen)
        Active = true;
    }
    #endregion

    #region Protected members

    protected int _currentDataBlock;
    protected short[] _dataBuffer;

    protected override void DoDataBufferSizeChanged()
    {
      base.DoDataBufferSizeChanged();
      _dataBuffer = CreateDataBuffer();
    }

    #endregion

    #region Public members
    public override void ReadDataBlock(int blockNr)
    {
      base.ReadDataBlock(blockNr);
      Debug.Assert(DataBuffer != null, "Databuffer not initialized");
      if (!DoReadDataBlock(blockNr, ref _dataBuffer))
        throw new DataFileReadDataException(DataFileConsts.DataFileReadData);
    }

    public override void WriteDataBlock(int blockNr)
    {
      base.WriteDataBlock(blockNr);
      Debug.Assert(DataBuffer != null, "Databuffer not initialized");
      if (!DoWriteDataBlock(blockNr, DataBuffer))
        throw new DataFileWriteDataException(DataFileConsts.DataFileWriteData);
    }
    #endregion

    #region Properties
    public int CurrentDataBlock { get { return _currentDataBlock; } }

    public new OpenStreamEvent OnOpenStream
    {
      get { return base.OnOpenStream; }
      set { base.OnOpenStream = value; }
    }

    public new bool StrictChecking
    {
      get { return base.StrictChecking; }
      set { base.StrictChecking = value; }
    }
    #endregion
  }
}
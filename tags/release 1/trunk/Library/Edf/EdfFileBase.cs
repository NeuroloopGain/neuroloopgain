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

namespace NeuroLoopGainLibrary.Edf
{
  public abstract class EdfFileBase : BaseEdfFileBase
  {
    #region Protected members
    protected override EdfFileInfoBase CreateFileInfo()
    {
      return new EdfFileInfo(this);
    }

    protected override EdfSignalInfoBase CreateSignalInfo()
    {
      return new EdfSignalInfo(this);
    }

    protected override void DoCreateNewFile(int nrSignals, bool overwrite)
    {
      base.DoCreateNewFile(nrSignals, overwrite);
      FileInfo.StrictChecking = StrictChecking;
      for (int i = 0; i < nrSignals; i++)
      {
        EdfSignalInfo newSignalDef = (EdfSignalInfo)CreateSignalInfo();
        newSignalDef.StrictChecking = StrictChecking;
        SignalInfo.Add(newSignalDef);
      }
      DoOpenFile();
      FileInfo.NrSignals = nrSignals;
      FileInfo.NrDataRecords = 0;
    }

    public override bool ValidFormat
    {
      get
      {
        bool result = base.ValidFormat;
        if (result && Active && !ValidFormatCheckBusy)
        {
          // Check other things like blocksize, etc
          try
          {
            ValidFormatCheckBusy = true;
            long blockSize = (long)DataBlockSize * sizeof(short);
            if (blockSize > EdfConstants.RecommendedMaxBlockSize)
            {
              HeaderError = HeaderError | EdfHeaderErrorType.EdfRecommendedRecordSizeError;
              if (StrictChecking && (blockSize > 10 * (1 * 1024 * 1024))) // blockSize > 10 MB
                result = false;
            }
            if (FileInfo.SampleRecDuration < 1)
            {
              if (((1 / FileInfo.SampleRecDuration) * DataBlockSize * sizeof(short)) < EdfConstants.RecommendedMaxBlockSize)
              {
                if (StrictChecking)
                  result = false;
                HeaderError = HeaderError | EdfHeaderErrorType.EdfSampleRecDurationError;
              }
            }
            int i = int.Parse(FileInfo.FileInfoRecord.NrDataRecords);
            if (i >= 0)
            {
              if ((i * DataBlockSize * sizeof(short) + FileInfo.HeaderBytes) != FileSize)
              {
                if (StrictChecking)
                  result = false;
                HeaderError = HeaderError | EdfHeaderErrorType.EdfFileSizeMismatchError;
              }
            }
            else
              if (!Creating)
                HeaderError = HeaderError | EdfHeaderErrorType.EdfUnkownNumberOfDataRecords;
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

    protected override void SetDecimalSeparator(char value)
    {
      base.SetDecimalSeparator(value);
      if (FileInfo != null)
      {
        FileInfo.DecimalSeparator = value;
        FileInfo.ReCheck();
      }
      foreach (EdfSignalInfo aSignalInfo in SignalInfo)
      {
        aSignalInfo.DecimalSeparator = value;
        aSignalInfo.ReCheck();
      }
    }

    protected override void SetThousandSeparator(char value)
    {
      base.SetThousandSeparator(value);
      if (FileInfo != null)
      {
        FileInfo.ThousandSeparator = value;
        FileInfo.ReCheck();
      }
      foreach (EdfSignalInfo aSignalInfo in SignalInfo)
      {
        aSignalInfo.ThousandSeparator = value;
        aSignalInfo.ReCheck();
      }
    }
    #endregion
  }
}
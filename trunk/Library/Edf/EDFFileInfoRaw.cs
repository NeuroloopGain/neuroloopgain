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
using NeuroLoopGainLibrary.Text;

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfFileInfoRaw : ICloneable
  {
		#region private fields 

    private string _headerBytes;
    private string _nrDataRecords;
    private string _nrSignals;
    private string _patient;
    private string _recording;
    private string _reserved;
    private string _sampleRecDuration;
    private string _startDate;
    private string _startTime;
    private string _version;

		#endregion private fields 

		#region public properties 

    public string HeaderBytes
    {
      get { return _headerBytes; }
      set {_headerBytes = TextUtils.SubStringLeft(value, 8); }
    }

    public string NrDataRecords
    {
      get { return _nrDataRecords; }
      set { _nrDataRecords = TextUtils.SubStringLeft(value, 8); }
    }

    public string NrSignals
    {
      get { return _nrSignals; }
      set { _nrSignals = TextUtils.SubStringLeft(value, 4); }
    }

    public string Patient
    {
      get { return _patient; }
      set { _patient = TextUtils.SubStringLeft(value, 80); }
    }

    public string Recording
    {
      get { return _recording; }
      set { _recording = TextUtils.SubStringLeft(value, 80); }
    }

    public string Reserved
    {
      get { return _reserved; }
      set { _reserved = TextUtils.SubStringLeft(value, 44); }
    }

    public string SampleRecDuration
    {
      get { return _sampleRecDuration; }
      set { _sampleRecDuration = TextUtils.SubStringLeft(value, 8); }
    }

    public string StartDate
    {
      get { return _startDate; }
      set { _startDate = TextUtils.SubStringLeft(value, 8); }
    }

    public string StartTime
    {
      get { return _startTime; }
      set { _startTime = TextUtils.SubStringLeft(value, 8); }
    }

    public string Version
    {
      get { return _version; }
      set { _version = TextUtils.SubStringLeft(value, 8); }
    }

		#endregion public properties 

		#region public methods 

    public void Clear()
    {
      Version = string.Empty;
      Patient = string.Empty;
      Recording = string.Empty;
      StartDate = string.Empty;
      StartTime = string.Empty;
      HeaderBytes = string.Empty;
      Reserved = string.Empty;
      NrDataRecords = string.Empty;
      SampleRecDuration = string.Empty;
      NrSignals = string.Empty;
    }

		#endregion public methods 


    #region ICloneable Members

    public object Clone()
    {
      return new EdfFileInfoRaw
                 {
                   HeaderBytes = HeaderBytes,
                   NrDataRecords = NrDataRecords,
                   NrSignals = NrSignals,
                   Patient = Patient,
                   Recording = Recording,
                   Reserved = Reserved,
                   SampleRecDuration = SampleRecDuration,
                   StartDate = StartDate,
                   StartTime = StartTime,
                   Version = Version
                 };
    }

    #endregion
  }
}
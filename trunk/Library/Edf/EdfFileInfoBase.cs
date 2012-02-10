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
using System.Globalization;
using MchSlc.Library.Edf;
using NeuroLoopGainLibrary.DateTimeTypes;
using NeuroLoopGainLibrary.General;
using NeuroLoopGainLibrary.Mathematics;
using NeuroLoopGainLibrary.Text;

namespace NeuroLoopGainLibrary.Edf
{
  public abstract class EdfFileInfoBase : EdfFileHeaderInfoBase
  {
		#region enums 

    public enum Field
    {
      Version = 0,
      Patient = 1,
      Recording = 2,
      StartDate = 3,
      StartTime = 4,
      HeaderBytes = 5,
      Reserved = 6,
      NrDataRecords = 7,
      SampleRecDuration = 8,
      NrSignals = 9
    }

		#endregion enums 

		#region private fields 

private readonly EdfFileInfoRaw _edfFileInfo = new EdfFileInfoRaw();
    private EdfFileInfoRaw _edfFileInfoRaw = new EdfFileInfoRaw();
    private int _headerBytes;
    private int _nrDataRecords;
    private int _nrSignals;
    private string _patient;
    private readonly EdfPatientInfoString _patientInfo = new EdfPatientInfoString();
    private string _recording;
    private readonly EdfRecordingInfoString _recordingInfo = new EdfRecordingInfoString();
    private string _reserved;
    private double _sampleRecDuration;
    private bool _signalDataIsContinuous = true;
    private DateTime? _startDate;
    private DateTime? _startDateTime;
    private TimeSpan? _startTime;
    private string _version = "0";

		#endregion private fields 

		#region protected methods 

    protected override void DoReCheck()
    {
      // DoReCheck if _fileInfoRecord is valid. Temporarily set _dataExists to false before testing
      //bool b = DataExists;
      DataExists = false;
    }

    protected EdfFileInfoBase(object owner)
      : base(owner)
    {
      // Set protected NumberDataFields value to avoid a virtual method call in constructor
      NumberDataFields = 10;

      for (int i = 0; i < NrDataFields; i++)
      {
        switch (i)
        {
          case 0:
          case 1:
          case 2:
          case 6:
            FieldValid[i] = true;
            break;
          default:
            break;
        }
      }
    }

    protected abstract void SetStartDateAsString(string value);

    protected abstract void SetStartTimeAsString(string value);

    protected virtual bool ValidFileVersion(string version)
    {
      return version.Equals("0");
    }

		#endregion protected methods 

		#region public properties 

    public virtual EdfFileInfoRaw EdfFileInfo
    {
      get { return _edfFileInfo; }
    }

    public virtual EdfFileInfoRaw EdfFileInfoRaw
    {
      get { return _edfFileInfoRaw; }
      protected set { _edfFileInfoRaw = value; }
    }

    public virtual DateTime? EndDateTime
    {
      get
      {
        return DataValid ? StartDateTime + TimeSpan.FromSeconds(NrDataRecords * SampleRecDuration) : null;
      }
    }

    public virtual EdfFileInfoRaw FileInfoRecord
    {
      get
      {
        EdfFileInfoRaw result = _edfFileInfo;
        // If field is valid then return field value, else return empty string
        result.Version = FieldValid[(int)Field.Version] ? Version : string.Empty;
        result.Patient = FieldValid[(int)Field.Patient] ? Patient : string.Empty;
        result.Recording = FieldValid[(int)Field.Recording] ? Recording : string.Empty;
        result.StartDate = FieldValid[(int)Field.StartDate] ? _startDateTime.GetValueOrDefault().ToString("dd.MM.yy") : string.Empty;
        result.StartTime = FieldValid[(int)Field.StartTime] ? _startDateTime.GetValueOrDefault().ToString("HH.mm.ss") : string.Empty;
        result.HeaderBytes = FieldValid[(int)Field.NrSignals] ? ((NrSignals + 1) * 256).ToString() : string.Empty;
        result.Reserved = FieldValid[(int)Field.Reserved] ? _reserved : string.Empty;
        result.NrDataRecords = FieldValid[(int)Field.NrDataRecords] ? _nrDataRecords.ToString() : string.Empty;
        if (FieldValid[(int)Field.SampleRecDuration])
        {
          NumberFormatInfo ni = new NumberFormatInfo();
          if (DecimalSeparator == '\0')
            DecimalSeparator = '.';
          ni.NumberDecimalSeparator = DecimalSeparator.ToString();
          ni.NumberGroupSeparator = ThousandSeparator.ToString();
          try
          {
            result.SampleRecDuration = TextUtils.DoubleToString(_sampleRecDuration, ni, 8);
          }
          catch (ConvertException)
          {
            result.SampleRecDuration = string.Empty;
          }
        }
        else
          result.SampleRecDuration = string.Empty;
        return result;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        int i;
        double d;
        EdfFileInfoRaw = (EdfFileInfoRaw)value.Clone();
        for (int j = 0; j < NrDataFields; j++)
          FieldValid[j] = false;
        Version = _edfFileInfoRaw.Version;
        _patientInfo.Parse(_edfFileInfoRaw.Patient);
        Patient = _edfFileInfoRaw.Patient;
        _recordingInfo.Parse(_edfFileInfoRaw.Recording);
        Recording = _edfFileInfoRaw.Recording;
        SetStartDateAsString(_edfFileInfoRaw.StartDate.Trim());
        SetStartTimeAsString(_edfFileInfoRaw.StartTime.Trim());
        if (int.TryParse(_edfFileInfoRaw.HeaderBytes, out i))
          HeaderBytes = i;
        Reserved = _edfFileInfoRaw.Reserved;
        if (int.TryParse(_edfFileInfoRaw.NrDataRecords, out i))
          NrDataRecords = i;
        if (double.TryParse(
          _edfFileInfoRaw.SampleRecDuration, NumberStyles.Float | NumberStyles.AllowThousands, FormatInfo, out d))
          SampleRecDuration = d;
        else
          FieldValid[(int)Field.SampleRecDuration] = false;
        int prevHeaderBytes = _headerBytes;
        if (int.TryParse(_edfFileInfoRaw.NrSignals, out i))
          NrSignals = i;
        _headerBytes = prevHeaderBytes;
        FieldValid[(int)Field.HeaderBytes] = FieldValid[(int)Field.HeaderBytes] &&
          FieldValid[(int)Field.NrSignals] && (_headerBytes == (_nrSignals + 1) * 256);
        Modified = false;
        if (!StrictChecking)
          return;
        DateTime dt;
        FieldValid[(int)Field.StartDate] &=
          DateTime.TryParseExact(_edfFileInfoRaw.StartDate, "dd.MM.yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        FieldValid[(int)Field.StartTime] &=
          DateTime.TryParseExact(_edfFileInfoRaw.StartTime, "HH.mm.ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        FieldValid[(int)Field.NrDataRecords] &= TextUtils.IsLeftJustified(_edfFileInfoRaw.NrDataRecords);
        FieldValid[(int)Field.SampleRecDuration] &=
          TextUtils.IsLeftJustified(_edfFileInfoRaw.SampleRecDuration);
        FieldValid[(int)Field.NrSignals] &= TextUtils.IsLeftJustified(_edfFileInfoRaw.NrSignals);
      }
    }

    public abstract double FirstDataBlockOnset { get; set; }

    public virtual int HeaderBytes
    {
      get
      {
        return _headerBytes;
      }
      protected set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (DataExists)
          throw new InvalidOperationException(EdfConstants.FileAlreadyContainsData);
        if (value != _headerBytes)
          Modified = true;
        _headerBytes = value;
        FieldValid[(int)Field.HeaderBytes] = (value >= 0);
      }
    }

    public virtual HPDateTime HPEndDateTime
    {
      get
      {
        return new HPDateTime(HPStartDateTime + NrDataRecords * SampleRecDuration, FirstDataBlockOnset);
      }
    }

    public virtual HPDateTime HPStartDateTime
    {
      get
      {
        return new HPDateTime(StartDateTime.GetValueOrDefault());
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        StartDateTime = value.DateTime;
      }
    }

    public virtual int NrDataRecords
    {
      get
      {
        return _nrDataRecords;
      }
      internal set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (string.IsNullOrEmpty(_edfFileInfoRaw.NrDataRecords) || (_edfFileInfoRaw.NrDataRecords.Trim() == string.Empty))
          _edfFileInfoRaw.NrDataRecords = value.ToString();
        if (value != _nrDataRecords)
        {
          Modified = true;
          _nrDataRecords = value;
        }
        FieldValid[(int)Field.NrDataRecords] = (value >= -1);
      }
    }

    public virtual int NrSignals
    {
      get
      {
        return _nrSignals;
      }
      internal set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (DataExists)
          throw new InvalidOperationException(EdfConstants.FileAlreadyContainsData);
        if (value != _nrSignals)
        {
          Modified = true;
          _nrSignals = value;
        }
        FieldValid[(int)Field.NrSignals] = (value >= 0);
        if (FieldValid[(int)Field.NrSignals])
          HeaderBytes = (value + 1) * 256;
        else
          HeaderBytes = 0;
      }
    }

    public virtual string Patient
    {
      get
      {
        return _patient;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (!value.TrimEnd(null).Equals(Patient))
        {
          Modified = true;
          _patient = value.TrimEnd(null);
          _patientInfo.Parse(Patient);
        }
        FieldValid[(int)Field.Patient] =
          (!StrictChecking || TextUtils.IsLeftJustifiedAndAsciiPrintable(value));
      }
    }

    public virtual DateTime? PatientBirthDate
    {
      get
      {
        return _patientInfo.BirthDate;
      }
      set
      {
        // Removed next check. Otherwise it is not possible to set unknown birthdate.
        //if (value.HasValue)
        {
          if (ReadOnly)
            throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
          //if (!_patientInfo.BirthDate.HasValue || (_patientInfo.BirthDate != value))
          if (_patientInfo.BirthDate != value)
          {
            Modified = true;
            _patientInfo.BirthDate = value;
            _patient = _patientInfo.ToString().TrimEnd();
          }
          FieldValid[(int)Field.Patient] = !StrictChecking || TextUtils.IsLeftJustifiedAndAsciiPrintable(_patient);
        }
      }
    }

    public virtual char PatientGender
    {
      get
      {
        return _patientInfo.Gender;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value != _patientInfo.Gender)
        {
          Modified = true;
          _patientInfo.Gender = value;
          _patient = _patientInfo.ToString().TrimEnd();
        }
        FieldValid[(int)Field.Patient] = !StrictChecking || TextUtils.IsLeftJustifiedAndAsciiPrintable(_patient);
      }
    }

    public virtual string PatientId
    {
      get
      {
        return _patientInfo.Id;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value.TrimEnd() != _patientInfo.Id)
        {
          Modified = true;
          _patientInfo.Id = value.TrimEnd();
          _patient = _patientInfo.ToString().TrimEnd();
        }
        FieldValid[(int)Field.Patient] = !StrictChecking || TextUtils.IsLeftJustifiedAndAsciiPrintable(_patient);
      }
    }

    public virtual EdfPatientInfoString PatientInfo { get { return _patientInfo; } }

    public virtual string PatientName
    {
      get
      {
        return _patientInfo.Name;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value.TrimEnd() != _patientInfo.Name)
        {
          Modified = true;
          _patientInfo.Name = value.TrimEnd();
          _patient = _patientInfo.ToString().TrimEnd();
        }
        FieldValid[(int)Field.Patient] = !StrictChecking || TextUtils.IsLeftJustifiedAndAsciiPrintable(_patient);
      }
    }

    public virtual string PatientOtherInfo
    {
      get
      {
        return _patientInfo.OtherInfo;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value.TrimEnd() != _patientInfo.OtherInfo)
        {
          Modified = true;
          _patientInfo.OtherInfo = value.TrimEnd();
          _patient = _patientInfo.ToString().TrimEnd();
        }
        FieldValid[(int)Field.Patient] = !StrictChecking || TextUtils.IsLeftJustifiedAndAsciiPrintable(_patient);
      }
    }

    public virtual string Recording
    {
      get
      {
        return _recording;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value.TrimEnd() != Recording)
        {
          Modified = true;
          _recording = value.TrimEnd();
        }
        FieldValid[(int)Field.Recording] = !StrictChecking || TextUtils.IsLeftJustifiedAndAsciiPrintable(value);
      }
    }

    public virtual string RecordingAdministrationCode
    {
      get
      {
        return _recordingInfo.AdministrationCode;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value != _recordingInfo.AdministrationCode)
        {
          Modified = true;
          _recordingInfo.AdministrationCode = value;
          _recording = _recordingInfo.ToString().TrimEnd();
        }
        FieldValid[(int)Field.Recording] = !StrictChecking || TextUtils.IsLeftJustifiedAndAsciiPrintable(_recording);
      }
    }

    public virtual string RecordingEquipment
    {
      get
      {
        return _recordingInfo.Equipment;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value != _recordingInfo.Equipment)
        {
          Modified = true;
          _recordingInfo.Equipment = value;
          _recording = _recordingInfo.ToString().TrimEnd();
        }
        FieldValid[(int)Field.Recording] = !StrictChecking || TextUtils.IsLeftJustifiedAndAsciiPrintable(_recording);
      }
    }

    public virtual EdfRecordingInfoString RecordingInfo { get { return _recordingInfo; } }

    public virtual string RecordingInvestigator
    {
      get
      {
        return _recordingInfo.Investigator;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value != _recordingInfo.Investigator)
        {
          Modified = true;
          _recordingInfo.Investigator = value;
          _recording = _recordingInfo.ToString().TrimEnd();
        }
        FieldValid[(int)Field.Recording] = !StrictChecking || TextUtils.IsLeftJustifiedAndAsciiPrintable(_recording);
      }
    }

    public virtual string RecordingOtherInfo
    {
      get
      {
        return _recordingInfo.OtherInfo;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value != _recordingInfo.OtherInfo)
        {
          Modified = true;
          _recordingInfo.OtherInfo = value;
          _recording = _recordingInfo.ToString().TrimEnd();
        }
        FieldValid[(int)Field.Recording] = !StrictChecking || TextUtils.IsLeftJustifiedAndAsciiPrintable(_recording);
      }
    }

    public virtual DateTime? RecordingStartDate
    {
      get
      {
        return _recordingInfo.StartDate;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value != _recordingInfo.StartDate)
        {
          Modified = true;
          _recordingInfo.StartDate = value;
        }
        _recording = _recordingInfo.ToString().TrimEnd();
        FieldValid[(int)Field.Recording] = !StrictChecking || TextUtils.IsLeftJustifiedAndAsciiPrintable(_recording);
        if (FieldValid[(int)Field.Recording])
          _startDateTime = _startDate.GetValueOrDefault().Date + _startTime.GetValueOrDefault();
      }
    }

    public virtual string Reserved
    {
      get { return _reserved; }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (!string.Equals(value.TrimEnd(), _reserved))
        {
          Modified = true;
          _reserved = value.TrimEnd();
          _recording = _recordingInfo.ToString().TrimEnd();
        }
        FieldValid[(int)Field.Reserved] =
            !StrictChecking || (TextUtils.IsLeftJustifiedAndAsciiPrintable(value));
      }
    }

    public virtual double SampleRecDuration
    {
      get { return _sampleRecDuration; }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (!MathEx.SameValue(value, _sampleRecDuration))
        {
          Modified = true;
          _sampleRecDuration = value;
        }
        FieldValid[(int)Field.SampleRecDuration] = (value > 0);
      }
    }

    public virtual bool SignalDataIsContinuous
    {
      get
      {
        return _signalDataIsContinuous;
      }
      internal set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        _signalDataIsContinuous = value;
      }
    }

    public virtual DateTime? StartDate
    {
      get { return _startDate; }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value != _startDate)
        {
          Modified = true;
          _startDate = value;
          FieldValid[(int)Field.StartDate] = value.GetValueOrDefault() > DateTime.MinValue;
          if (FieldValid[(int)Field.StartDate] && FieldValid[(int)Field.StartTime])
            _startDateTime = _startDate.GetValueOrDefault().Date + StartTime.GetValueOrDefault();
        }
      }
    }

    public virtual DateTime? StartDateTime
    {
      get { return _startDateTime; }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value != _startDateTime)
        {
          Modified = true;
          StartDate = value.GetValueOrDefault().Date;
          StartTime = value.GetValueOrDefault().TimeOfDay;
          // Set _startDateTime last, because StartDate and StartTime both modify the _startDateTime value
          _startDateTime = value;
        }
      }
    }

    public virtual TimeSpan? StartTime
    {
      get { return _startTime; }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value != _startTime)
        {
          Modified = true;
          _startTime = value;
        }
        FieldValid[(int)Field.StartTime] = (value.HasValue && (value.Value < TimeSpan.FromDays(1)));
        _startDateTime = FieldValid[(int)Field.StartDate] && FieldValid[(int)Field.StartTime]
                           ? (DateTime?)(_startDate.GetValueOrDefault() + _startTime.GetValueOrDefault())
                           : null;
      }
    }

    public virtual string Version
    {
      get
      {
        return _version;
      }
      protected set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (!value.TrimEnd(null).Equals(_version))
          Modified = true;
        _version = value.TrimEnd(null);
        FieldValid[(int)Field.Version] = ValidFileVersion(Version);
      }
    }

		#endregion public properties 

		#region public methods 

    /// <summary>
    /// Copies the information from sourceInfo
    /// </summary>
    /// <param name="sourceInfo">The source info.</param>
    /// <param name="allFields">if set to <c>true</c> all fields are copied; otherwise the fields NrSignal, HeaderBytes and NrDataRecords are ignored.</param>
    public void CopyFrom(EdfFileInfoRaw sourceInfo, bool allFields = false)
    {
      EdfFileInfoRaw tmpInfo = sourceInfo.Clone() as EdfFileInfoRaw;
      if (tmpInfo == null)
        throw new EdfException("Unable to clone file info.");
      if (!allFields)
      {
        tmpInfo.NrSignals = NrSignals.ToString();
        tmpInfo.HeaderBytes = HeaderBytes.ToString();
        tmpInfo.NrDataRecords = NrDataRecords.ToString();
      }
      FileInfoRecord = tmpInfo;
      Modified = true;
    }

		#endregion public methods 
  }
}
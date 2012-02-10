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
using NeuroLoopGainLibrary.DateTimeTypes;

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfPlusFileInfo : EdfFileInfoBase
  {
		#region private fields 

    private double _firstDataBlockOnset;

		#endregion private fields 

		#region protected methods 

    protected override void SetStartDateAsString(string value)
    {
      DateTime dt;
      if (DateTime.TryParseExact(value, "dd.MM.yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
      {
        value = value.Insert(6, int.Parse(value.Substring(6, 2)) > 84 ? "19" : "20");
        StartDate = DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
      }
    }

    protected override void SetStartTimeAsString(string value)
    {
      DateTime dt;
      if (DateTime.TryParseExact(value, "HH.mm.ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
        StartTime = dt.TimeOfDay;
    }

		#endregion protected methods 

		#region Constructors 

    public EdfPlusFileInfo(object owner)
      : base(owner)
    {
      if (!(owner is EdfPlusFileBase))
        throw new ArgumentException("Owner should be of class EDFPlusBase", "owner");
      FieldValid[(int)Field.SampleRecDuration] = true;
    }

		#endregion Constructors 

		#region public properties 

    public override DateTime? EndDateTime
    {
      get
      {
        return HPEndDateTime.DateTime;
      }
    }

    public override double FirstDataBlockOnset
    {
      get
      {
        return _firstDataBlockOnset;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if ((value < 0) || (value >= 1))
          throw new InvalidOperationException("Invalid FirstDataBlockOffset. SHould be >= 0 and < 1s");
        _firstDataBlockOnset = value;
        EdfPlusOwner.TAL.FileStartDateTime = HPStartDateTime;
          if (EdfPlusOwner.TAL.BlockCount == 0)
            if (!EdfPlusOwner.TAL.AddBlock(0, 0))
              throw new EdfPlusTALException(TALConsts.UnableToAddTALBlock0);
          EdfPlusOwner.TAL.Block(0).DataRecOnset = value;
      }
    }

    public override HPDateTime HPEndDateTime
    {
      get
      {
        HPDateTime result;
        EdfPlusFileBase edfPlusOwner = (EdfPlusFileBase)Owner;
        if (SampleRecDuration > 0)
        {
          if (SignalDataIsContinuous)
            result = base.HPEndDateTime;
          else
          {
            result = HPStartDateTime;
            result += edfPlusOwner.TAL.Block(NrDataRecords - 1).DataRecOnset + SampleRecDuration;
          }
          if (edfPlusOwner.TAL.LastAnnotationEnd >= result)
            result = edfPlusOwner.TAL.LastAnnotationEnd;
        }
        else
          result = edfPlusOwner.TAL.LastAnnotationEnd;
        return result;
      }
    }

    protected EdfPlusFileBase EdfPlusOwner { get { return (EdfPlusFileBase) Owner; } }

    public HPDateTime HpEndDateTimeSamples
    {
      get
      {
        if (!DataValid)
          throw new DataFileFieldValueException(DataFileConsts.DataFileFieldValue);
        if (!StartDateTime.HasValue)
          throw new ArgumentException("StartDateTime");
        if (!SignalDataIsContinuous)
          return HPStartDateTime + EdfPlusOwner.TAL.Block(NrDataRecords - 1).DataRecOnset + SampleRecDuration;
        return
          new HPDateTime(StartDateTime.Value, (NrDataRecords * SampleRecDuration) / TimeSpan.FromDays(1).TotalSeconds);
      }
    }

    public override HPDateTime HPStartDateTime
    {
      get
      {
        HPDateTime result = base.HPStartDateTime;
        if (!double.IsNaN(FirstDataBlockOnset))
          result += FirstDataBlockOnset;
        return result;
      }
      set
      {
        if (HPStartDateTime != value)
        {
          StartDateTime = new DateTime(value.DateTime.Year, value.DateTime.Month, value.DateTime.Day,
            value.DateTime.Hour, value.DateTime.Minute, value.DateTime.Second);
          FirstDataBlockOnset = value.SecondsAfterMidnight - (long)value.SecondsAfterMidnight;
        }
      }
    }

    public override string Patient
    {
      get
      {
        return base.Patient;
      }
      set
      {
        base.Patient = value;
        PatientInfo.Parse(value);
      }
    }

    public override string Recording
    {
      get
      {
        return RecordingInfo.ToString();
      }
      set
      {
        base.Recording = value;
        RecordingInfo.Parse(value);
      }
    }

    public override string Reserved
    {
      get
      {
        return base.Reserved;
      }
      set
      {
        base.Reserved = value;
        FieldValid[(int)Field.Reserved] &=
          Reserved.StartsWith(EdfConstants.IdentifierEdfPlusC, StringComparison.InvariantCultureIgnoreCase) ||
          Reserved.StartsWith(EdfConstants.IdentifierEdfPlusD, StringComparison.InvariantCultureIgnoreCase);
      }
    }

    public override double SampleRecDuration
    {
      get
      {
        return base.SampleRecDuration;
      }
      set
      {
        base.SampleRecDuration = value;
        FieldValid[(int)Field.SampleRecDuration] = (value >= 0);
        if (FieldValid[(int)Field.SampleRecDuration])
          ((EdfPlusFileBase)Owner).TAL.BlockDuration = value;
      }
    }

    public override bool SignalDataIsContinuous
    {
      get
      {
        return (Reserved.StartsWith(EdfConstants.IdentifierEdfPlusC, StringComparison.InvariantCultureIgnoreCase));
      }
      internal set
      {
        base.SignalDataIsContinuous = value;
        Reserved = value ? EdfConstants.IdentifierEdfPlusC : EdfConstants.IdentifierEdfPlusD;
      }
    }

    public override DateTime? StartDate
    {
      get
      {
        return base.StartDate;
      }
      set
      {
        base.StartDate = value;
        if (FieldValid[(int)Field.StartDate])
          RecordingStartDate = value;
      }
    }

		#endregion public properties 
  }
}
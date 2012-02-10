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
using System.Text;
using NeuroLoopGainLibrary.Text;

namespace NeuroLoopGainLibrary.Edf
{
  class EdfFileInfo : EdfFileInfoBase
  {
		#region protected methods 

    protected override void SetStartDateAsString(string value)
    {
      StringBuilder sb = new StringBuilder(value);
      sb.Replace(' ', '0');
      if ((sb.Length >= 2) && !char.IsDigit(sb[1]))
        sb.Insert(0, '0');
      if ((sb.Length >= 5) && !char.IsDigit(sb[4]))
        sb.Insert(3, '0');
      if (sb.Length > 2)
        sb[2] = '-';
      if (sb.Length > 5)
        sb[5] = '-';
      DateTime dt;
      if (!DateTime.TryParseExact(sb.ToString(), "dd-MM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
        return;
      sb.Insert(6, int.Parse(sb.ToString().Substring(6)) > 84 ? "19" : "20");
      StartDate = DateTime.ParseExact(sb.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
    }

    protected override void SetStartTimeAsString(string value)
    {
      StringBuilder sb = new StringBuilder(value);
      sb.Replace(' ', '0');
      if ((sb.Length >= 2) && !char.IsDigit(sb[1]))
        sb.Insert(0, '0');
      if ((sb.Length >= 5) && !char.IsDigit(sb[4]))
        sb.Insert(3, '0');
      if (sb.Length > 2)
        sb[2] = ':';
      if (sb.Length > 5)
        sb[5] = ':';
      TimeSpan ts;
      if (TimeSpan.TryParse(sb.ToString(), out ts))
        StartTime = ts;
    }

		#endregion protected methods 

		#region Constructors 

    public EdfFileInfo(object owner) : base(owner) { }

		#endregion Constructors 

		#region public properties 

    public override EdfFileInfoRaw FileInfoRecord
    {
      get
      {
        return base.FileInfoRecord;
      }
      set
      {
        base.FileInfoRecord = value;
        if (!StrictChecking)
          return;
        DateTime dt;
        FieldValid[(int) Field.StartDate] &=
          DateTime.TryParseExact(EdfFileInfoRaw.StartDate, "dd.MM.yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        FieldValid[(int) Field.StartTime] &=
          DateTime.TryParseExact(EdfFileInfoRaw.StartTime, "HH.mm.ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        FieldValid[(int) Field.NrDataRecords] &= TextUtils.IsLeftJustified(EdfFileInfoRaw.NrDataRecords);
        FieldValid[(int) Field.SampleRecDuration] &= TextUtils.IsLeftJustified(EdfFileInfoRaw.SampleRecDuration);
        FieldValid[(int) Field.NrSignals] &= TextUtils.IsLeftJustified(EdfFileInfoRaw.NrSignals);
      }
    }

    public override double FirstDataBlockOnset
    {
      get { return 0; } 
      set {  }
    }

		#endregion public properties 
  }
}
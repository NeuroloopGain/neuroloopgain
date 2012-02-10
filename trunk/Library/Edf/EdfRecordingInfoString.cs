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

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfRecordingInfoString : EdfFileInfoStringBase
  {


    #region Private members
    private string _recordingInfo;
    #endregion

    #region Protected members
    public override string ToString()
    {
      // E.g.: Startdate 02-MAR-2002 PSG-1234/2002 NN Telemetry03
      StringBuilder sb = new StringBuilder("Startdate ");
      sb.Append(StartDate.HasValue
                ? StartDate.Value.ToString(EdfConstants.DefaultDateFormat, CultureInfo.InvariantCulture).ToUpper()
                : "X");
      sb.Append(" ");
      sb.Append(!string.IsNullOrEmpty(AdministrationCode) ? ReplaceSpaceChar(AdministrationCode) : "X");
      sb.Append(" ");
      sb.Append(!string.IsNullOrEmpty(Investigator) ? ReplaceSpaceChar(Investigator) : "X");
      sb.Append(" ");
      sb.Append(!string.IsNullOrEmpty(Equipment) ? ReplaceSpaceChar(Equipment) : "X");
      if (!string.IsNullOrEmpty(OtherInfo))
        sb.Append(" " + OtherInfo);
      return sb.ToString();
    }

    public void Parse(string value)
    {
      _recordingInfo = value;
      StartDate = null;
      AdministrationCode = string.Empty;
      Investigator = string.Empty;
      Equipment = string.Empty;
      OtherInfo = string.Empty;
      string[] splitValues = value.Split(' ');
      if (splitValues.Length > 0)
      {
        if (string.Compare(splitValues[0],"Startdate",true) == 0)
        {
          if (splitValues.Length > 1)
          {
            DateTime dt;
            splitValues[1] = ReplaceSpaceReplaceChar(splitValues[1]);
            if (DateTime.TryParseExact(splitValues[1], EdfConstants.DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
              StartDate = dt;
          }
          if (splitValues.Length > 2)
          {
            AdministrationCode = ReplaceSpaceReplaceChar(splitValues[2]);
            if (AdministrationCode.ToUpper() == "X")
              AdministrationCode = string.Empty;
          }
          if (splitValues.Length > 3)
          {
            Investigator = ReplaceSpaceReplaceChar(splitValues[3]);
            if (Investigator.ToUpper() == "X")
              Investigator = string.Empty;
          }
          if (splitValues.Length > 4)
          {
            Equipment = ReplaceSpaceReplaceChar(splitValues[4]);
            if (Equipment.ToUpper() == "X")
              Equipment = string.Empty;
          }
          if (splitValues.Length > 5)
            OtherInfo = splitValues[5];
        }
        else
          OtherInfo = value;
      }
    }

    public override char SpaceReplaceChar
    {
      get
      {
        return base.SpaceReplaceChar;
      }
      set
      {
        base.SpaceReplaceChar = value;
        if (!string.IsNullOrEmpty(_recordingInfo))
          Parse(_recordingInfo);
      }
    }
    #endregion

    #region Properties
    public string AdministrationCode { get; set; }
    public string Equipment { get; set; }
    public string Investigator { get; set; }
    public string OtherInfo { get; set; }
    /// <summary>
    /// Startdate from the header. This is not an EDF+ datablock offset!!!
    /// </summary>
    /// <value>The start date of this recording from the header.</value>
    public DateTime? StartDate { get; set; }

    #endregion
  }
}
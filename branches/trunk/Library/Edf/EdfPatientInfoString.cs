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
  public class EdfPatientInfoString : EdfFileInfoStringBase
  {


    #region Private members

    private string _patientInfo;

    #endregion

    #region Protected members

    public override char SpaceReplaceChar
    {
      get
      {
        return base.SpaceReplaceChar;
      }
      set
      {
        base.SpaceReplaceChar = value;
        if (!string.IsNullOrEmpty(_patientInfo))
          Parse(_patientInfo);
      }
    }

    #endregion

    #region Properties
    public DateTime? BirthDate { get; set; }
    public char Gender { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string OtherInfo { get; set; }

    #endregion

    #region Public members

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(!string.IsNullOrEmpty(Id) ? ReplaceSpaceChar(Id) : "X");
      sb.Append(" ");
      sb.Append((char.ToUpper(Gender) != 'F') && (char.ToUpper(Gender) != 'M') ? "X" : char.ToUpper(Gender).ToString());
      sb.Append(" ");
      sb.Append(BirthDate.HasValue ? BirthDate.Value.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).ToUpper() : "X");
      sb.Append(" ");
      sb.Append(!string.IsNullOrEmpty(Name) ? ReplaceSpaceChar(Name) : "X");
      if (!string.IsNullOrEmpty(OtherInfo))
        sb.Append(" " + OtherInfo);
      return sb.ToString();
    }

    public void Parse(string value)
    {
      _patientInfo = value;
      Id = string.Empty;
      Name = string.Empty;
      Gender = '\0';
      BirthDate = null;
      OtherInfo = string.Empty;
      string[] splitValues = value.Split(new[] { ' ' });
      if (splitValues.Length > 0)
      {
        Id = ReplaceSpaceReplaceChar(splitValues[0]);
        if (Id.ToUpper() == "X")
          Id = string.Empty;
      }
      if (splitValues.Length > 1)
      {
        splitValues[1] = splitValues[1].ToUpper();
        if ((splitValues[1] == "F") || (splitValues[1] == "M"))
          Gender = splitValues[1][0];
      }
      if (splitValues.Length > 2)
      {
        DateTime dt;
        if (DateTime.TryParseExact(splitValues[2], "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
          BirthDate = dt;
      }
      if (splitValues.Length > 3)
      {
        if (splitValues[3].ToUpper() != "X")
          Name = ReplaceSpaceReplaceChar(splitValues[3]);
      }
      if (splitValues.Length > 4)
        OtherInfo = splitValues[4];
    }

    #endregion
  }
}
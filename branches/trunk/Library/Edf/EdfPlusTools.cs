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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NeuroLoopGainLibrary.Errorhandling;
using NeuroLoopGainLibrary.Mathematics;
using NeuroLoopGainLibrary.Text;

namespace NeuroLoopGainLibrary.Edf
{
  public static class EdfPlusTools
  {
    #region private fields

    const char MinusReplaceChar = (char)2;
    const char PlusReplaceChar = (char)1;

    #endregion private fields

    #region Event handlers

    /// <summary>
    /// Sets (resizes) the EDF+ signal number of data for selected signal number.
    /// This method only handles one signal, zero datablock duration.
    /// </summary>
    /// <param name="tal">The annotations list.</param>
    /// <param name="newBufferSize">The new size of the buffer.</param>
    private static void DoResizeBuffer_OneSignalZeroDuration(EdfPlusAnnotationList tal, int newBufferSize)
    {
      // By using the annotation sizes, the block onsets are set; this will make sure that all annotations will fit into
      int blockIdx = 0;
      int byteCount = 0;
      for (int i = 0; i < tal.Count; i++)
      {
        // Check if this annotation fits into current buffer
        byteCount += tal[i].ToString().Length;
        // if this annotation fits into buffer, check next
        if (byteCount < newBufferSize) continue;

        // This annotation does not fit into the current datablock, so move it to the next one
        blockIdx++;

        // Set the block onset to current annotation onset
        if (blockIdx >= tal.BlockCount)
          tal.AddBlock(blockIdx, tal[i].Onset);
        else
          tal.Block(blockIdx).DataRecOnset = tal[i].Onset;

        // initialise byteCount
        byteCount = tal[i].ToString().Length;
      }
      tal.RedistributeAnnotations();
    }

    /// <summary>
    /// Find and cleanup multiple instances of same sub-label (e.g. M1+Fpz-Cz+Cz+3*Fpz -> M1+4*Fpz).
    /// </summary>
    /// <param name="lblList">The key value list containing the labels and count.</param>
    /// <param name="label">The label.</param>
    private static void MakeSimpleLabel_ProcessSubLabels(List<KeyValuePair<string, int>> lblList, string label)
    {
      // Find and cleanup multiple instances of same sub-label (e.g. Fpz-Cz+Cz -> Fpz) using a name-value pair list.
      int i = IsOperator(label[0]) ? 1 : 0;
      while (i < label.Length)
      {
        int j = i;
        while ((j < label.Length) && !IsOperator(label[j]))
          j++;

        // todo: Check if the following lines are necessary. Propably not because of plus- and minusReplaceChars
        if ((j < label.Length - 2) && IsOperator(label[j + 1]))
          j++;
        if ((j < label.Length - 1) && IsOperator(label[j]))
          j--;
        // todo: Check until until here

        bool subtract;
        string aName;

        int cnt = j < label.Length ? (j - i) + 1 : (j - i);

        if (i > 0)
        {
          subtract = (label[i - 1] == '-');
          aName = label.Substring(i, cnt);
        }
        else
        {
          subtract = false;
          aName = label.Substring(i, cnt);
        }

        // Check if aName contains a '*' (e.g. 3*Fpz); if yes cnt contains the number (3), aName contains the name (Fpz).
        int k = aName.IndexOf('*');
        if (k >= 0)
        {
          if (!int.TryParse(label.Substring(i, k - 1), out cnt))
            cnt = int.MinValue;
          if (cnt > int.MinValue)
            aName = aName.Remove(0, k);
          else
            cnt = 1;
        }
        else
          cnt = 1;

        // Check if the list alreay contains this name
        k = lblList.FindIndex(p => p.Key == aName);
        // If the list contains this name, update the value.
        if (k >= 0)
        {
          int l = lblList[k].Value;
          if (subtract)
            l -= cnt;
          else
            l += cnt;

          lblList[k] = new KeyValuePair<string, int>(lblList[k].Key, l);
        }
        else
        {
          // Add this name to the list
          //  aName = aName.Replace("=", "%is%"); not necessary anymore, we are using a KeyValuePair
          lblList.Add(subtract ? new KeyValuePair<string, int>(aName, -cnt) : new KeyValuePair<string, int>(aName, cnt));
        }
        // skip the operator
        i = j + 2;
      }

      // Remove names instances with zero count
      lblList.RemoveAll(p => p.Value == 0);

      // Move positives forward
      lblList.Sort(delegate(KeyValuePair<string, int> a, KeyValuePair<string, int> b)
                     {
                       if ((a.Value >= 0) && (b.Value < 0))
                         return -1;
                       if ((a.Value < 0) && (b.Value >= 0))
                         return 1;
                       return lblList.IndexOf(a) < lblList.IndexOf(b) ? -1 : 1;
                     });
    }

    private static string MakeSimpleLabel_RemoveBrackets(string label)
    {
      //  Remove brackets
      while (label.IndexOf(')') >= 0)
      {
        int i = label.IndexOf(')');
        int j = -1;
        int k = j;

        // Find matching '('
        do
        {
          k = label.IndexOf('(', k + 1);
          if ((k >= 0) && (k < i))
            j = k;
        } while ((k >= 0) && (k < i));
        // j now contains index of matching '('

        // should we negate the contents of the part between '(' and ')' (is there a '-' character just before the '(')
        bool negate = ((j >= 1) && (label[j - 1] == '-'));
        if (negate)
          for (k = j + 1; k < i; k++)
          {
            switch (label[k])
            {
              case '+':
                TextUtils.ReplaceAt(ref label, k, '-');
                break;
              case '-':
                TextUtils.ReplaceAt(ref label, k, '+');
                break;
            }
          }

        // Remove the matching '(' and ')' characters
        label = label.Remove(i, 1);
        label = label.Remove(j, 1);

        // remove double operators
        i = 0;
        while (i < label.Length - 1)
        {
          if (IsOperator(label[i]) && IsOperator(label[i + 1]))
          {
            TextUtils.ReplaceAt(ref label, i, (label[i] != label[i + 1]) ? '-' : '+');
            label = label.Remove(i + 1, 1);
          }
          i++;
        }
      }
      return label;
    }

    /// <summary>
    /// Replace the operator characters that are part of the signal label.
    /// </summary>
    /// <param name="label">The label.</param>
    /// <param name="charToReplace">The character to replace.</param>
    /// <param name="checkChars">The check chars.</param>
    /// <param name="replacementChar">The replacement character.</param>
    private static void MakeSimpleLabel_ReplaceNonOperatorCharacters(ref string label, char charToReplace,
      char[] checkChars, char replacementChar)
    {
      int i = 0;
      while (i >= 0)
      {
        // Check if newlabel contains a '+' character.
        i = label.IndexOf(charToReplace, i);
        if (i < 0) continue;
        // Check if this charToReplace character is followed by an "+", '-' or ')' character; if yes temporarily replace it
        if ((i == label.Length - 1) || (checkChars.Contains(label[i + 1])))
          TextUtils.ReplaceAt(ref label, i, replacementChar);
        i++;
      }
    }

    #endregion Event handlers

    #region Private Methods

    /// <summary>
    /// Determines whether the specified character is an operator.
    /// </summary>
    /// <param name="c">The character.</param>
    /// <returns>
    ///   <c>true</c> if the specified c is operator; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsOperator(char c)
    {
      return (c == '+') || (c == '-');
    }

    #endregion Private Methods

    #region public fields

    public static string[] EdfPlusECGStandardDerivation = new[]
                                                            {
                                                              "I", "II", "III", "aVR", "aVL", "aVF", "V1", "V2", "V3",
                                                              "V4", "V5", "V6", "-aVR", "V2R", "V3R", "V4R", "V7", "V8",
                                                              "V9", "X", "Y", "Z"
                                                            };
    public static string[] EdFplusEEGElectrodeNames = new[]
                                                        {
                                                          "Nz", "Fp1", "Pg1", "Sp1", "N0",
                                                          "Fpz", "Fp2", "Pg2", "Sp2", "Fp0",
                                                          "AFz", "AF0",
                                                          "Fz", "P1", "C1", "CP1", "F0",
                                                          "FCz", "P2", "C2", "CP2", "FC0",
                                                          "Cz", "P3", "C3", "CP3", "C0",
                                                          "CPz", "P4", "C4", "CP4", "Cp0",
                                                          "Pz", "P5", "C5", "CP5", "P0",
                                                          "POz", "P6", "C6", "CP6", "PO0",
                                                          "Oz", "P9", "P7", "P8", "O0",
                                                          "Iz", "P10", "O1", "T3", "I0",
                                                          "O2", "T4",
                                                          "F1", "FC1", "AF3", "T5",
                                                          "F2", "FC2", "AF4", "T6", "T7",
                                                          "F3", "FC3", "AF7", "T9", "T8",
                                                          "F4", "FC4", "AF8", "T10",
                                                          "F5", "FC5", "PO3", "TP7",
                                                          "F6", "FC6", "PO4", "TP8",
                                                          "F7", "FT7", "PO7", "TP9",
                                                          "F8", "FT8", "PO8", "TP10",
                                                          "F9", "FT9", "A1", "T1",
                                                          "F10", "FT10", "A2", "T2",
                                                          "M1", "M2",
                                                          "Ref"
                                                        };
    /// <summary>
    /// The identifier used to link an annotation to a signal
    /// </summary>
    public static string LinkedToSignalIdentifier = "@@";

    #endregion public fields

    #region public methods

    /// <summary>
    /// Sets (resizes) the EDF+ signal number of data for selected signal number.
    /// </summary>
    /// <param name="edfFile">The edf file.</param>
    /// <param name="signalNr">The signal nr.</param>
    /// <param name="newNrBytes">The new number of bytes.</param>
    /// <param name="marginPercentage">The percentage to use as margin; e.g. 20 means leave 20% of the space unused.</param>
    public static void DoResizeTALBufferSize(EdfPlusFile edfFile, int signalNr, int newNrBytes, byte marginPercentage)
    {
      if (!Range.InRange(marginPercentage, 1, 99))
        throw new ArgumentOutOfRangeException("marginPercentage");

      if (edfFile.AnnotationSignalNrs.Count == 1 && MathEx.IsZero(edfFile.FileInfo.SampleRecDuration))
        DoResizeBuffer_OneSignalZeroDuration(edfFile.TAL, (int)Math.Truncate(newNrBytes * (100 - marginPercentage) / 100d));
      else
        throw new NotImplementedException();
    }

    /// <summary>
    /// Simplifies the EDF+ label.
    /// </summary>
    /// <param name="label">The label.</param>
    /// <returns>The simplified label.</returns>
    public static string EdfPlusMakeSimpleLabel(string label)
    {
      string s;
      // Check format "EEG C3-M2-(O2-M1)+M2-M1"
      if (ValidEdfPlusLabel(label))
      {
        s = label;
        EdfPlusSignalType signalType = GetAndStripEdfplusSignalType(ref s);
        return s != string.Empty
                 ? EdfPlusSignalTypeHelper.GetEdfPlusSignalTypeName(signalType) + " " + MakeSimpleLabel(s)
                 : label;
      }
      s = MakeSimpleCombinedLabel(label);
      return s != label ? s : MakeSimpleLabel(label);
    }

    /// <summary>
    /// Gets the type of signal and srips this identifier from the signal label.
    /// </summary>
    /// <param name="signalLabel">The signal label.</param>
    /// <returns>The type of signal label</returns>
    public static EdfPlusSignalType GetAndStripEdfplusSignalType(ref string signalLabel)
    {
      EdfPlusSignalType type = GetEdfPlusSignalType(signalLabel);
      if (type == EdfPlusSignalType.Unknown)
        return type;

      string signalTypeStr = EdfPlusSignalTypeHelper.GetEdfPlusSignalTypeName(type);

      signalLabel = signalLabel.Remove(0, signalTypeStr.Length);
      signalLabel = signalLabel.TrimStart();
      int idx = 0;
      while (idx >= 0)
      {
        idx = signalLabel.IndexOf(signalTypeStr, idx);
        if ((idx < 0) || (!IsOperator(signalLabel[idx - 1])))
        {
          idx = -1;
          continue;
        }
        signalLabel = signalLabel.Remove(idx, signalTypeStr.Length);
        while (((idx < signalLabel.Length - 1) && (signalLabel[idx] == ' ')))
          signalLabel = signalLabel.Remove(idx, 1);
      }
      return type;
    }

    /// <summary>
    /// Gets the base dimension from a physical dimension string (e.g. "uV" -> "V").
    /// </summary>
    /// <param name="dimension">The dimension.</param>
    /// <returns>The base dimension of the given dimension.</returns>
    public static string GetBaseDimension(string dimension)
    {
      var result = string.Empty;
      dimension = dimension.Trim();
      if (dimension.Length > 1)
      {
        List<string> matches =
          EdfPlusSignalTypeHelper.AllUniqueEdfPlusSignalTypeBaseDim.FindAll(
            item => item.Equals(dimension, StringComparison.InvariantCultureIgnoreCase));
        if (matches.Count > 0)
          result = matches[0];
        int i = 0;
        while ((result == string.Empty) && (i <= MathEx.DimensionPrefix.GetUpperBound(0)))
        {
          if ((dimension[0] == MathEx.DimensionPrefix[i]) || (MathEx.DimensionPrefixIgnoreCase[i] && ((char.ToUpper(dimension[0])) == char.ToUpper(MathEx.DimensionPrefix[i]))))
            result = dimension.Substring(1);
          i++;
        }
      }
      if (result == string.Empty)
        result = dimension;
      return result;
    }

    /// <summary>
    /// Gets the type of the edf plus signal.
    /// </summary>
    /// <param name="signalLabel">The signal label.</param>
    /// <returns></returns>
    public static EdfPlusSignalType GetEdfPlusSignalType(string signalLabel)
    {
      EdfPlusSignalType i = EdfPlusSignalType.Distance;
      while (i < EdfPlusSignalType.Unknown)
      {
        // Check if the signallabel start with the EDF+ signal type name
        if (signalLabel.ToUpper().IndexOf(EdfPlusSignalTypeHelper.GetEdfPlusSignalTypeName(i).ToUpper()) == 0)
          return i;
        i++;
      }
      return EdfPlusSignalType.Unknown;
    }

    /// <summary>
    /// Gets the encoded EDFLogFloat information from the prefilter string.
    /// </summary>
    /// <param name="preFilter">The prefilter.</param>
    /// <param name="ymin">The ymin.</param>
    /// <param name="a">A.</param>
    /// <param name="physiDim">The physical dimension.</param>
    /// <param name="cultureInfo">The culture info.</param>
    /// <param name="numberStyles">The number styles.</param>
    /// <returns><c>true</c> if successful; otherwise <c>false</c></returns>
    public static bool GetLogFloatInfo(string preFilter, out Double ymin, out double a, out string physiDim,
      CultureInfo cultureInfo, NumberStyles numberStyles)
    {
      const string logFloatMask = "sign*LN[sign*()/()]/()";
      string t = preFilter;

      int idx = t.IndexOf(logFloatMask.Substring(0, 14), StringComparison.InvariantCulture);

      if (idx >= 0)
      {
        t = t.Substring(idx, 46);
        string s = t;
        s = s.Remove(37, 8);
        s = s.Remove(25, 8);
        s = s.Remove(14, 8);

        if (s.Equals(logFloatMask, StringComparison.InvariantCulture) &&
          double.TryParse(t.Substring(25, 8), numberStyles, cultureInfo, out ymin) &&
          double.TryParse(t.Substring(37, 8), numberStyles, cultureInfo, out a))
        {
          physiDim = t.Substring(14, 8).Trim();
          return true;
        }
      }

      ymin = 0;
      a = 0;
      physiDim = string.Empty;
      return false;
    }

    /// <summary>
    /// Determines whether the specified signal label is an ECG derivation.
    /// </summary>
    /// <param name="signalLabel">The signal label.</param>
    /// <returns>
    ///   <c>true</c> if the specified signal label is an ECG derivation; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsECGDerivation(string signalLabel)
    {
      string s = signalLabel;
      if (s.StartsWith("ECG", true, CultureInfo.InvariantCulture))
        s = s.Substring(3).Trim();

      return !string.IsNullOrEmpty(s) && EdfPlusECGStandardDerivation.Any(s.Equals);
    }

    /// <summary>
    /// Determines whether the signal label is an EEG derivation.
    /// </summary>
    /// <param name="signalLabel">The signal label.</param>
    /// <param name="edfPlus">if set to <c>true</c> check the label using EDF+ defined EEG electrode names.</param>
    /// <returns>
    ///   <c>true</c> if the signal label is an EEG derivation; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsEEGDerivation(string signalLabel, bool edfPlus = false)
    {
      if (string.IsNullOrEmpty(signalLabel))
        return false;

      do
      {
        string s = TextUtils.StripStringValue(ref signalLabel, "+-()").Trim();
        if (s.StartsWith("EEG", true, CultureInfo.InvariantCulture))
          s = s.Substring(3).Trim();
        if (s != string.Empty)
          if (!IsEEGElectrodeName(s, edfPlus))
            return false;
      } while (!string.IsNullOrEmpty(signalLabel));

      return true;
    }

    /// <summary>
    /// Determines whether the electrode name is an EDF(+) EEG electrode name.
    /// </summary>
    /// <param name="electrodeName">Name of the electrode.</param>
    /// <param name="edfPlus">if set to <c>true</c> check using edf+ defined names.</param>
    /// <returns>
    ///   <c>true</c> if the electrode name is an EDF(+) EEG electrode name; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsEEGElectrodeName(string electrodeName, bool edfPlus)
    {
      if (string.IsNullOrEmpty(electrodeName))
        return false;
      if (edfPlus)
        return EdFplusEEGElectrodeNames.Contains(electrodeName);
      return TextUtils.IsAlpha(electrodeName[0]) && TextUtils.IsAlphaNumeric(electrodeName);
    }

    /// <summary>
    /// Gets the string that links an annotation text to a signal.
    /// </summary>
    /// <param name="annotation">The annotation.</param>
    /// <param name="signalLabel">The signal label.</param>
    /// <returns></returns>
    public static string LinkToSignal(string annotation, string signalLabel)
    {
      return !string.IsNullOrEmpty(signalLabel) ? string.Format("{0}{1}{2}", annotation, LinkedToSignalIdentifier, signalLabel) : annotation;
    }

    /// <summary>
    /// Makes a simple label from a combination label.
    /// </summary>
    /// <param name="label">The label.</param>
    /// <returns></returns>
    public static string MakeSimpleCombinedLabel(string label)
    {
      // if null or empty, return the label itself
      if (string.IsNullOrEmpty(label) || label.Trim().Equals(string.Empty))
        return label;

      List<string> list = new List<string>();

      // The following regular expression splits a string like "EEG Fpz-Cz-(EEG Pz-O2)+EEG Fpz-Cz-(EEG Pz-Oz)"
      // into 4 substrings ("EEG Fpz-Cz" , "-(EEG Pz-O2)", "+EEG Fpz-Cz" and "-(EEG Pz-Oz)".
      // It either matches a x(...) string or a string matching up to x(.... or end of line.

      Regex regex = new Regex(@"([+-]?\(.+?\))|(.+?((?=[+-]?[()])|$))");

      MatchCollection matches = regex.Matches(label);

      if (matches.Count > 0)
      {
        foreach (Match m in matches)
        {
          if (m.Value.StartsWith("("))
            list.Add("+" + m.Value);
          else if (m.Value.EndsWith(")"))
            list.Add(m.Value);
          else if (m.Value.StartsWith("+") || m.Value.StartsWith("-"))
            list.Add(string.Format("{0}({1})", m.Value[0], m.Value.Substring(1)));
          else
            list.Add(string.Format("+({0})", m.Value));
        }
      }

      if (list.Count == 0)
        throw new AssertionException();

      // Check if all signal parts are of the same type
      EdfPlusSignalType signalType = EdfPlusSignalType.Unknown;
      foreach (EdfPlusSignalType t in list.Select(subString => GetEdfPlusSignalType(list[0].Substring(2, list[0].Length - 3))))
      {
        if (t == EdfPlusSignalType.Unknown)
          return label;

        switch (signalType)
        {
          case EdfPlusSignalType.Unknown:
            signalType = t;
            break;
          default:
            if (t != signalType)
              return label;
            break;
        }
      }

      string s = string.Empty;
      foreach (string t in list)
      {
        string T = t.Substring(2, t.Length - 3);
        GetAndStripEdfplusSignalType(ref T);
        s = string.Format("{0}{1}({2})", s, t.Substring(0, 1), T);
      }
      s = MakeSimpleLabel(s);
      return string.Format("{0} {1}", EdfPlusSignalTypeHelper.GetEdfPlusSignalTypeName(signalType), s);
    }

    /// <summary>
    /// Simplify an EDF(+) signal lable to it's most compact form
    /// </summary>
    /// <param name="label">The label.</param>
    /// <returns>The simplified label.</returns>
    public static string MakeSimpleLabel(string label)
    {
      // Check if label is an empty string
      if (string.IsNullOrEmpty(label) || label.Trim() == string.Empty)
        return label;

      // Remove spaces from label
      string newLabel = label.Replace("  ", string.Empty);

      // Check if brackets are synchronised
      if (newLabel.Count(c => c == '(') != newLabel.Count(c => c == ')'))
        return label;

      // Replace + characters that are part of the name with replacement character
      MakeSimpleLabel_ReplaceNonOperatorCharacters(ref newLabel, '+', new[] { '+', '-', ')' }, PlusReplaceChar);

      // Replace - characters that are part of the name with replacement character
      MakeSimpleLabel_ReplaceNonOperatorCharacters(ref newLabel, '-', new[] { '+', '-', ')' }, MinusReplaceChar);

      //  Remove brackets
      newLabel = MakeSimpleLabel_RemoveBrackets(newLabel);

      List<KeyValuePair<string, int>> lblList = new List<KeyValuePair<string, int>>();

      // Find and cleanup multiple instances of same sub-label (e.g. Fpz-Cz+Cz -> Fpz) using a name-value pair list.
      MakeSimpleLabel_ProcessSubLabels(lblList, newLabel);

      // Compose new label
      StringBuilder sb = new StringBuilder();
      foreach (KeyValuePair<string, int> entry in lblList)
      {
        int j = entry.Value;
        if (j == 0) continue;

        if (Math.Abs(j) == 1)
          sb.Append(((j < 0) ? '-' : '+'));
        else
        {
          if (j > 0)
            sb.Append('+');
          sb.Append(string.Format("{0}*", j));
        }
        sb.Append(entry.Key);
      }
      // Remove '+' sign at front of label
      if (sb.Length > 0 && sb[0] == '+')
        sb.Remove(0, 1);
      // Replace replacement characters
      sb.Replace(PlusReplaceChar, '+');
      sb.Replace(MinusReplaceChar, '-');
      // If the result label is the same as label without spaces then return the original label
      return sb.ToString().Equals(newLabel) ? label : sb.ToString();
    }

    /// <summary>
    /// Removes the part that links an annotation to a signal.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns></returns>
    public static string RemoveLinkedToSignalIdentifier(string s)
    {
      int idx = s.IndexOf(LinkedToSignalIdentifier);
      return idx >= 0 ? TextUtils.SubStringLeft(s, idx) : s;
    }

    /// <summary>
    /// Checks if 2 strings have the same base dimension.
    /// e.g. SameBaseDimension("uV","mV") = true, SameBaseDimension("km","mm") = false
    /// </summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns></returns>
    public static bool SameBaseDimension(string value1, string value2)
    {
      return GetBaseDimension(value1) == GetBaseDimension(value2);
    }

    /// <summary>
    /// Check if the label is a valid EDF+ label.
    /// </summary>
    /// <param name="signalLabel">The signal label.</param>
    /// <param name="physicalDimension">The physical dimension.</param>
    /// <returns><c>true</c> if signal label is a valid EDF+ label; otherwise <c>false</c></returns>
    public static bool ValidEdfPlusLabel(string signalLabel, string physicalDimension = null)
    {
      string s = signalLabel;
      EdfPlusSignalType type = GetAndStripEdfplusSignalType(ref s);

      // If the signal type string is repeated, the result is false
      if (!string.IsNullOrEmpty(s) && s.IndexOf(EdfPlusSignalTypeHelper.GetEdfPlusSignalTypeName(type), StringComparison.InvariantCulture) >= 0)
        return false;

      switch (type)
      {
        case EdfPlusSignalType.EEG:
          if (!IsEEGDerivation(s))
            return false;
          break;
        case EdfPlusSignalType.ECG:
          if (!IsECGDerivation(s))
            return false;
          break;
        case EdfPlusSignalType.Unknown:
          return false;
      }

      int signalType = (int)type;

      if (string.IsNullOrEmpty(physicalDimension) ||
          (EdfPlusSignalTypeHelper.EdfPlusSignalTypeBaseDim[signalType, 0] == string.Empty))
        return true;

      int i = 0;
      while (i < EdfPlusSignalTypeHelper.EdfPlusSignalTypeBaseDim.GetUpperBound(1))
      {
        if (EdfPlusSignalTypeHelper.EdfPlusSignalTypeBaseDim[signalType, i] != string.Empty)
          if (SameBaseDimension(physicalDimension, EdfPlusSignalTypeHelper.EdfPlusSignalTypeBaseDim[signalType, i]))
            return true;
        i++;
      }

      return false;
    }

    #endregion public methods
  }
}
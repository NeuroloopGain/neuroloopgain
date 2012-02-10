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
using System.Text;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Globalization;
using NeuroLoopGainLibrary.General;

namespace NeuroLoopGainLibrary.Text
{
  public static class TextUtils
  {
    public const string DetectFloatSeparatorExpression = @"[^\d;\s;a-z;A-Z+;-]"; // "any character not in this class will give a match"

    /// <summary>
    /// Get the current century string.
    /// </summary>
    /// <param name="twoDigits">If set to <c>true</c> the result will be 2 digits instead of 4.</param>
    /// <returns>E.g.: 2007 or 20 if TwoDigits is true.</returns>
    public static string CurrentCenturyString(bool twoDigits)
    {
      return twoDigits ? DateTime.Now.Year.ToString().Substring(0, 2) : DateTime.Now.Year.ToString();
    }

    /// <summary>
    /// Tries to detect the decimal separator in a given string.
    /// </summary>
    /// <param name="s">The string to check.</param>
    /// <returns></returns>
    public static char DetectDecimalSeparator(string s)
    {
      return DetectDecimalSeparator(s, false, char.MinValue);
    }

    /// <summary>
    /// Tries to detect the decimal separator in a given string.
    /// </summary>
    /// <param name="s">The string to check.</param>
    /// <param name="returnBestGuess">If set to <c>true</c> the best guess wil be returned.</param>    
    /// <returns></returns>
    public static char DetectDecimalSeparator(string s, bool returnBestGuess)
    {
      return DetectDecimalSeparator(s, returnBestGuess, char.MinValue);
    }

    /// <summary>
    /// Tries to detect the decimal separator in a given string.
    /// </summary>
    /// <param name="s">The string to check.</param>
    /// <param name="returnBestGuess">If set to <c>true</c> the best guess wil be returned.</param>
    /// <param name="thousandSeparator">The thousand separator.</param>
    /// <returns></returns>
    public static char DetectDecimalSeparator(string s, bool returnBestGuess, char thousandSeparator)
    {
      s = s.Trim();
      char result = char.MinValue;
      int index = s.Length - 1;
      Regex floatSepRegEx = new Regex(DetectFloatSeparatorExpression);
      while ((index >= 0) && (result == char.MinValue))
      {
        if (floatSepRegEx.IsMatch(s[index].ToString())) // possible decimal separator found
        {
          if (s[index] != thousandSeparator)
            result = s[index];
          else
            return result;
        }
        else
          index--;
      }
      // If a possible DecimalSeperator was found then check it to be sure
      if (result != char.MinValue)
      {
        bool sure = IsValidDecimalSeparator(s, result, thousandSeparator);
        if (!sure)
        {
          if (thousandSeparator != char.MinValue)
          {
            if (result == thousandSeparator)
              result = char.MinValue;
          }
          else
            if (!returnBestGuess)
              result = char.MinValue;
        }
      }
      return result;
    }

    /// <summary>
    /// Detects the float separators in the given string.
    /// </summary>
    /// <param name="s">The string to find the separators in.</param>
    /// <param name="decimalSeparator">The decimal separator found (char.MinValue if not found).</param>
    /// <param name="thousandSeparator">The thousand separator found (char.MinValue if not found).</param>
    /// <returns>True if both a valid decimal and thousand separator were found; otherwise false.</returns>
    public static bool DetectFloatSeparator(string s, out char decimalSeparator, out char thousandSeparator)
    {
      decimalSeparator = DetectDecimalSeparator(s);
      thousandSeparator = DetectThousandSeparator(s, false, decimalSeparator);
      if ((decimalSeparator == char.MinValue) && (thousandSeparator != char.MinValue))
        decimalSeparator = DetectDecimalSeparator(s, false, thousandSeparator);
      if ((decimalSeparator == char.MinValue) && (thousandSeparator == char.MinValue))
      {
        char tmpThousandSep = DetectThousandSeparator(s, true);
        if (tmpThousandSep != char.MinValue)
        {
          decimalSeparator = DetectDecimalSeparator(s, false, tmpThousandSep);
          thousandSeparator = DetectThousandSeparator(s, false, decimalSeparator);
        }
      }
      return ((decimalSeparator != char.MinValue) && (thousandSeparator != char.MinValue));
    }

    /// <summary>
    /// Tries to detect the thousand separator in a given string.
    /// </summary>
    /// <param name="s">The string to check.</param>
    /// <param name="returnBestGuess">If set to <c>true</c> the best guess wil be returned.</param>
    /// <returns></returns>
    public static char DetectThousandSeparator(string s, bool returnBestGuess)
    {
      return DetectThousandSeparator(s, returnBestGuess, char.MinValue);
    }

    /// <summary>
    /// Tries to detect the thousand separator in a given string.
    /// </summary>
    /// <param name="s">The string to check.</param>
    /// <param name="returnBestGuess">If set to <c>true</c> the best guess wil be returned.</param>
    /// <param name="decimalSeparator">The decimal separator.</param>
    /// <returns></returns>
    public static char DetectThousandSeparator(string s, bool returnBestGuess, char decimalSeparator)
    {
      s = s.Trim();
      char result = char.MinValue;
      char guess = char.MinValue;
      int index = 0;
      Regex floatSepRegEx = new Regex(DetectFloatSeparatorExpression);
      while ((index < s.Length) && (guess == char.MinValue))
      {
        if (floatSepRegEx.IsMatch(s[index].ToString())) // possible thousand separator found
        {
          if (s[index] != decimalSeparator)
            guess = s[index];
          else
            return result;
        }
        else
          index++;
      }
      // If found a possible ThousandSeperator then check it to be sure
      if (guess != char.MinValue)
      {
        bool tripletsOk;
        if (IsValidThousandSeparator(s, guess, out tripletsOk, decimalSeparator) || (returnBestGuess && tripletsOk))
          result = guess;
      }
      return result;
    }

    /// <summary>
    /// Puts double quotes (") around the given string.
    /// </summary>
    /// <param name="s">The string to put double quotes around.</param>
    /// <returns>Double quoted string.</returns>
    public static string DoubleQuoteString(string s)
    {
      return "\"" + s + "\"";
    }

    /// <summary>
    /// Converts a double to string, the resulting string cannot contain more than
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="maxLength">Length of the max.</param>
    /// <returns>String representation of the given string.</returns>
    /// <exception cref="ConvertException">Thrown when the string represention exceeds maxLength.</exception>
    public static string DoubleToString(double value, IFormatProvider formatProvider, int maxLength)
    {
      int nrCharsRoundedValue = (Math.Round(value)).ToString().Length;
      if (nrCharsRoundedValue > maxLength)
        throw new ConvertException("String exceeds maximum length");
      
      string result = value.ToString(formatProvider);
      if (result.Length > maxLength)
      {
        int nrCharsAfterDecimal = maxLength - nrCharsRoundedValue - 1;
        string fmt = "0." + new string('#', nrCharsAfterDecimal);
        result = value.ToString(fmt, formatProvider);
      }
      return result;
    }

    /// <summary>
    /// Formats the given time span.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <param name="format">The format string to use (use the same syntax as formatting DateTimes).</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>Formatted string representing the given TimeSpan.</returns>
    public static string FormatTimeSpan(TimeSpan value, string format, IFormatProvider formatProvider)
    {
      DateTime dt = DateTime.Now.Date + value;
      return dt.ToString(format, formatProvider);
    }

    /// <summary>
    /// Gets the SHA1 hash for a given string.
    /// </summary>
    /// <param name="s">The string to compute the hash for.</param>
    /// <returns>SHA1 hash as string.</returns>
    public static string GetSHA1Hash(string s)
    {
      SHA1 sha1Hasher = SHA1.Create();
      byte[] data = sha1Hasher.ComputeHash(Encoding.Default.GetBytes(s));
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < data.Length; i++)
      {
        sb.Append(data[i].ToString("x2"));
      }
      return sb.ToString();
    }

    /// <summary>
    /// Determines whether the specified string only contains letters a-z (either lower- or uppercase).
    /// </summary>
    /// <param name="s">The string to check.</param>
    /// <returns>
    /// 	<c>true</c> if the specified string only contains letters; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAlpha(string s)
    {
      return Regex.IsMatch(s, "^([a-z]|[A-Z])*$");
    }

    /// <summary>
    /// Determines whether the specified character is a letter in the range a-z (either lower- or uppercase).
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>
    /// 	<c>true</c> if the specified character is a letter; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAlpha(char c)
    {
      return IsAlpha(c.ToString());
    }

    /// <summary>
    /// Determines whether the specified string contains only alphanumeric characters (0-9, a-z and A-Z)
    /// </summary>
    /// <param name="s">The string to check.</param>
    /// <returns>
    /// 	<c>true</c> if the string is alphanumeric; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAlphaNumeric(string s)
    {
      return Regex.IsMatch(s, "^([0-9]|[a-z]|[A-Z])*$");
    }
    
    /// <summary>
    /// Determines whether the given string consists of printable characters.
    /// </summary>
    /// <param name="s">The string to check.</param>
    /// <returns>
    /// 	<c>True</c> if the string is printable; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsASCIIPrintable(string s)
    {
      Regex regEx = new Regex("[^\x20-\x7E]");
      return !regEx.IsMatch(s);
    }

    /// <summary>
    /// Determines whether the given string is left justified.
    /// </summary>
    /// <param name="s">The string to check.</param>
    /// <returns>
    /// 	<c>True</c> if the string is left justified; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsLeftJustified(string s)
    {
      return (s.TrimEnd(null) == s.Trim());
    }

    /// <summary>
    /// Determines whether the given string is left justified and ASCII printable.
    /// </summary>
    /// <param name="s">The string to check.</param>
    /// <returns>
    /// 	<c>True</c> if the string is left justified and ASCII printable; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsLeftJustifiedAndAsciiPrintable(string s)
    {
      return IsLeftJustified(s) && IsASCIIPrintable(s);
    }

    /// <summary>
    /// Determines whether the given decimal separator is valid in the specified string.
    /// </summary>
    /// <param name="s">The string that contains the decimal separator.</param>
    /// <param name="decimalSeparator">The decimal separator.</param>
    /// <param name="thousandSeparator">The thousand separator.</param>
    /// <returns>
    /// 	<c>True</c> if the given string contains a valid decimal separator; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValidDecimalSeparator(string s, char decimalSeparator, char thousandSeparator)
    {
      // Valid DecimalSeperator if:
      // - there is only 1 occurence
      // - absolutely sure if there are <> 3 digits after or more than 3 digits before DecimalSeperator
      bool result = false;
      Regex decimalSepCountRegEx = new Regex(string.Format(@"[{0}]", decimalSeparator));
      if (decimalSepCountRegEx.Matches(s).Count == 1)
      {
        int index = s.IndexOf(decimalSeparator);
        int count = 0;
        int i = index - 1;
        while ((i >= 0) && (count < 4))
        {
          if (!char.IsDigit(s[i]))
            break;
          count++;
          i--;
        }
        result = (count > 3);
        if (!result)
        {
          count = 0;
          i = index + 1;
          while ((i < s.Length) && (count < 4))
          {
            if (!char.IsDigit(s[i]))
              break;
            count++;
            i++;
          }
          result = (count != 3);
        }
      }
      return result;
    }

    /// <summary>
    /// Determines whether the given thousand separator is valid in the specified string.
    /// </summary>
    /// <param name="s">The string that contains the thousand separator.</param>
    /// <param name="thousandSeparator">The thousand separator.</param>
    /// <param name="tripletsOk">if set to <c>true</c> [triplets ok].</param>
    /// <param name="decimalSeparator">The decimal separator.</param>
    /// <returns>
    /// 	<c>True</c> if the given string contains a valid thousand separator; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValidThousandSeparator(string s, char thousandSeparator, out bool tripletsOk, char decimalSeparator)
    {
      // Valid ThousandSeperator if:
      // - there are 1 or more occurences
      // - always 3 digits between seperators
      // - absolutely sure if more than 1 occurence (3 digits spaced) or 1 occurrence and a DecimalSeperator (3 digits spaced)
      bool result = false;
      tripletsOk = false;
      if ((s.Length > 0) && (thousandSeparator != char.MinValue))
      {
        int index = s.IndexOf(thousandSeparator);
        int count = 0;
        int i = 0;
        if ((s[0] == '+') || (s[0] == '-'))
          i++;
        while (i < index)
        {
          if (!char.IsDigit(s[i]))
            return false;
          count++;
          i++;
        }
        if (count <= 3)
        {
          i = index + 1;
          int nrSep = 1;
          count = 0;
          while ((i < s.Length) && ((char.IsDigit(s[i])) || (s[i] == thousandSeparator)))
          {
            if (char.IsDigit(s[i]))
              count++;
            else
            {
              if (count != 3)
                return false;
              count = 0;
              nrSep++;
            }
            i++;
          }
          if (i < s.Length)
          {
            if ((decimalSeparator != char.MinValue) && (s[i] == decimalSeparator))
              nrSep++;
          }
          tripletsOk = (count == 3);
          result = (tripletsOk && (nrSep > 1));
        }
      }
      return result;
    }

    /// <summary>
    /// Parses the time span from a (english) text like:
    /// - '2 weeks'
    /// - '16 seconds'
    /// - '1 year' or '3 years'
    /// Decimal separators are not yet supported. Supported units are 'seconds', 'minutes', 'days', 'weeks', 'months', 'years' (or singular forms like 'second', ..., 'year') 
    /// </summary>
    /// <param name="s">The text to parse.</param>
    /// <returns></returns>
    public static TimeSpan ParseTimeSpan(string s)
    {
      if (string.IsNullOrEmpty(s))
        throw new ArgumentException("Cannot parse timespan, given string is null or empty");
      string[] parts = s.Split(' ');
      if (parts.Length == 2)
      {
        int nr = int.Parse(parts[0]);
        double multiplier = 0;
        switch (parts[1])
        {
          case "second":
          case "seconds":
            multiplier = 1d / (3600 * 24);
            break;
          case "minute":
          case "minutes":
            break;
          case "hour":
          case "hours":
            multiplier = 1d / 24;
            break;
          case "day":
          case "days":
            multiplier = 1;
            break;
          case "week":
          case "weeks":
            multiplier = 7;
            break;
          case "month":
          case "months":
            multiplier = 30.436875d;
            break;
          case "year":
            multiplier = 365.2425d;
            break;
          default:
            throw new FormatException("Invalid unit " + parts[1]);
        }
        return TimeSpan.FromDays(nr * multiplier);
      }
      throw new FormatException("Invalid format, string not in form '[nr] [units]'");
    }

    /// <summary>
    /// Converts a PSG number to a filename.
    /// </summary>
    /// <param name="prefix">A prefix that the filename starts with.</param>
    /// <param name="psgNr">The PSG number that is to be converted.</param>
    /// <param name="postfix">The postfix to be added to the filename.</param>
    /// <returns>The complete filename.</returns>
    public static string PSGNrToFileName(string prefix, string psgNr, string postfix)
    {
      string fileName;

      if (!ValidPSGNr(psgNr))
        fileName = "";
      else
      {
        psgNr = ToYYYYPSGNr(psgNr);
        fileName = prefix;
        fileName += psgNr.Substring(0, 4);
        int year = int.Parse(psgNr.Substring(5, 4));
        if ((year < 1985) || (year > 2920))
          throw new Exception("Invalid year in PSG number + " + psgNr);
        year -= 1985;
        fileName += (char)('A' + (year % 26));
        if (postfix.Contains("."))
          postfix = postfix.Remove(postfix.LastIndexOf('.'), 1);
        if (year / 26 <= 9)
          fileName += (char)(('0') + (year / 26)) + "." + postfix;
        else
          fileName += (char)(('A') + (year / 26) - 10) + "." + postfix;
      }
      return fileName;
    }

    /// <summary>
    /// Replaces part of a string (case insensitive!!!!)
    /// </summary>
    /// <param name="original">The string containing the value(s) to replace.</param>
    /// <param name="pattern">The part to search for (case insensitive!).</param>
    /// <param name="replacement">The part to replace 'pattern' with.</param>
    /// <returns>String where all occurences of 'pattern' are replaced with 'replacement'</returns>
    public static string ReplaceEx(string original, string pattern, string replacement)
    {
      int count = 0;
      int position0 = 0;
      int position1;
      string upperString = original.ToUpper();
      string upperPattern = pattern.ToUpper();
      int inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
      char[] chars = new char[original.Length + Math.Max(0, inc)];
      while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
      {
        for (int i = position0; i < position1; ++i)
          chars[count++] = original[i];
        foreach (char t in replacement)
          chars[count++] = t;
        position0 = position1 + pattern.Length;
      }
      if (position0 == 0) return original;
      for (int i = position0; i < original.Length; ++i)
        chars[count++] = original[i];
      return new string(chars, 0, count);
    }

    /// <summary>
    /// Replaces the character at the specified position with newCharacter.
    /// </summary>
    /// <param name="s">The string to replace the character in.</param>
    /// <param name="index">The index at which to replace the character.</param>
    /// <param name="newCharacter">The new character.</param>
    public static void ReplaceAt(ref string s, int index, char newCharacter)
    {
      var chars = s.ToCharArray();
      chars[index] = newCharacter;
      s = new string(chars);
    }

    /// <summary>
    /// Splits the variables from CSV strings and removes leading and ending quote pairs
    /// </summary>
    /// <param name="csv">The CSV string.</param>
    /// <returns>Array containing the separated variables</returns>
    public static string[] SplitCSV(string csv)
    {
      return SplitCSV(csv, true);
    }

    /// <summary>
    /// Splits the variables from CSV strings
    /// </summary>
    /// <param name="csv">The CSV string.</param>
    /// <param name="removeQuotes">if set to <c>true</c> [remove quotes].</param>
    /// <returns>Array containing the separated variables</returns>
    public static string[] SplitCSV(string csv, bool removeQuotes)
    {
      Regex regex = new Regex(@"(((("".*?"")|('.*?')|([^""',]+))(?<!,)))|(?=,,)");
      MatchCollection matches = regex.Matches(csv);
      string[] s = new string[matches.Count];
      for (int i = 0; i < matches.Count; i++)
      {
        string t = matches[i].Value;
        if (Regex.IsMatch(t, @"(("".*?"")|('.*?'))"))
          s[i] = t.Substring(1, t.Length - 2);
        else
          s[i] = t;
      }
      return s;
    }

    /// <summary>
    /// Strips all occurences of '\r\n' from the end of a string.
    /// </summary>
    /// <param name="s">The string to strip.</param>
    /// <returns>Clean string.</returns>
    public static string StripNewLinesFromEnd(string s)
    {
      while (s.EndsWith("\r\n"))
        s = s.Remove(s.Length - 2, 2);
      while (s.EndsWith("\n"))
        s = s.Remove(s.Length - 1, 1);
      return s;
    }

    public static string StripStringValue(ref string s, string delimiters, bool removeDelimiters = true)
    {
      int j = -1;
      if (delimiters.Length > 0)
      {
        j = int.MaxValue - 1;
        foreach (char t in delimiters)
        {
          int delimiterIndex = s.IndexOf(t);
          if (delimiterIndex >= 0)
            j = Math.Min(j, delimiterIndex);
        }
      }
      string result = j >= 0 ? s.Substring(0, removeDelimiters ? Math.Min(j, s.Length) : Math.Min(j + 1, s.Length)) : s;
      s = s.Remove(0, Math.Min(s.Length, j + 1));
      return result.Trim();
    }

    /// <summary>
    /// Returns the left part of the given string. The maximum number of returned characters is count. If the string length is less than count characters, the whole string will be returned.
    /// </summary>
    /// <param name="s">The string to get the left part of.</param>
    /// <param name="count">The maximum number of characters to return.</param>
    /// <returns>Left substring.</returns>
    public static string SubStringLeft(string s, int count)
    {
      return s == null ? string.Empty : s.Substring(0, Math.Min(s.Length, count));
    }

    /// <summary>
    /// Converts PSG numbers in the form '2589/07' to '2589/2007'
    /// </summary>
    /// <param name="psgNr">The PSG number to process.</param>
    /// <returns>A PSG number in the form '2589/2007'</returns>
    public static string ToYYYYPSGNr(string psgNr)
    {
      if ((psgNr.Trim() != "") && (psgNr.Length != 9))
        return psgNr.Substring(0, 5) + CurrentCenturyString(true) + psgNr.Substring(5, psgNr.Length - 5);
      return psgNr;
    }

    /// <summary>
    /// Parses the time span from a (english) text like:
    /// - '2 weeks'
    /// - '16 seconds'
    /// - '1 year' or '3 years'
    /// Decimal separators are not yet supported. Supported units are 'seconds', 'minutes', 'days', 'weeks', 'months', 'years' (or singular forms like 'second', ..., 'year')
    /// </summary>
    /// <param name="s">The text to parse.</param>
    /// <param name="result">The result.</param>
    /// <returns>True if succesfully parsed, false if there was an error.</returns>
    public static bool TryParseTimeSpan(string s, out TimeSpan result)
    {
      try
      {
        result = ParseTimeSpan(s);
        return true;
      }
      catch
      {
        result = TimeSpan.MinValue;
        return false;
      }
    }

    /// <summary>
    /// Checks if the given string is a valid integer.
    /// </summary>
    /// <param name="number">The string to check.</param>
    /// <returns>True if the given number is a valid integer, false if not.</returns>
    public static bool ValidInt(string number)
    {
      int i;
      return int.TryParse(number.Trim(), out i);
    }

    /// <summary>
    /// Checks if the given string is a valid double.
    /// </summary>
    /// <param name="number">The string to check.</param>
    /// <param name="formatProvider">The format settings to use.</param>
    /// <returns>True if the given number is a valid double, false if not.</returns>
    public static bool ValidDouble(string number, IFormatProvider formatProvider)
    {
      double d;
      return double.TryParse(number, NumberStyles.None, formatProvider, out d);
    }

    /// <summary>
    /// Check if a PSG number is valid.
    /// </summary>
    /// <param name="psgNr">The PSG number to check.</param>
    /// <returns>True if valid, false if not.</returns>
    public static bool ValidPSGNr(string psgNr)
    {
      try
      {
        psgNr = ToYYYYPSGNr(psgNr).ToUpper();
        if (psgNr.Length == 9)
        {
          string s = psgNr.Substring(5, 4);     // get year number
          return ((psgNr[4] == '/') && (ValidInt(psgNr.Substring(0, 4))) &&
            (ValidInt(s)) && (int.Parse(s) > 1985));
        }
      }
      catch
      {
        return false;
      }
      return false;
    }

    /// <summary>
    /// Verifies the given string against the given SHA1 hash.
    /// </summary>
    /// <param name="s">The string to verify.</param>
    /// <param name="hash">The hash to use.</param>
    /// <returns>True if the SHA1 hash for the given string is the same as the given hash, false if not.</returns>
    public static bool VerifySHA1Hash(string s, string hash)
    {
      string hashedS = GetSHA1Hash(s);
      StringComparer sComparer = StringComparer.OrdinalIgnoreCase;
      return (sComparer.Compare(hashedS, hash) == 0);
    }

    /// <summary>
    /// Wraps the given string so that every line contains at maximum maxLength characters.
    /// Not of any use if the given string does not contain spaces.
    /// </summary>
    /// <param name="text">The text to split.</param>
    /// <param name="maxLength">Maximum length per line.</param>
    /// <returns>Wrapped string.</returns>
    public static string WrapString(string text, int maxLength)
    {
      string[] words = text.Split(' ');
      string currentLine = "";
      StringCollection lines = new StringCollection();
      foreach (string aWord in words)
      {
        if ((currentLine.Length > maxLength) || (currentLine.Length + aWord.Length > maxLength))
        {
          lines.Add(currentLine);
          currentLine = "";
        }

        if (currentLine.Length > 0)
          currentLine += " " + aWord;
        else
          currentLine += aWord;
      }
      if (currentLine.Length > 0)
        lines.Add(currentLine);

      StringBuilder sb = new StringBuilder();
      foreach (string aLine in lines)
        sb.AppendLine(aLine);
      return sb.ToString();
    }
  }
}

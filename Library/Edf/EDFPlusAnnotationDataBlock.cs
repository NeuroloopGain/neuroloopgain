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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NeuroLoopGainLibrary.DateTimeTypes;

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfPlusAnnotationDataBlock
  {


    #region Constructors
    public EdfPlusAnnotationDataBlock(EdfPlusAnnotationListBase owner, int dataRecNr, double dataRecOnset)
    {
      _owner = owner;
      _dataRecNr = dataRecNr;
      _dataRecOnset = dataRecOnset;
      _modified = false;
    }
    #endregion

    #region Private members
    private bool _dataReadFromBuffer;
    private readonly int _dataRecNr;
    private double _dataRecOnset;
    private TalError _lastError;
    private bool _modified;
    private readonly EdfPlusAnnotationListBase _owner;

    private static void AddToLastError(int annotationSignalNr, string annotationToAdd, TalError error)
    {     
    }

    private static int AddToBuffer(ref byte[] buffer, int annotationSignalNr, ref int p, ref int q, string s)
    {
      int result = 0;
      if (!string.IsNullOrEmpty(s))
      {
        result += s.Length + 1; // don't forget trailing #0
        if (p < q)
        {
          int i = 0;
          while ((i < s.Length) && (p < q))
          {
            buffer[p] = (byte)s[i];
            p++;
            i++;
          }
          if (p < q)
          {
            buffer[p] = 0;
            p++;
          }
          else
            AddToLastError(annotationSignalNr, s, TalError.BufferFull);
        }
      }
      return result;
    }
    
    private int DoWriteToBuffer(ref short[] buffer, int bufferOffset, int bufferSize, int annotationSignalNr)
    {
      Debug.Assert(annotationSignalNr >= 0, TALConsts.AnnotationSignalNrError);
      _lastError = TalError.None;
      int result = 0;
      //byte[] byteBuffer = new byte[bufferSize - bufferOffset*2];
      byte[] byteBuffer = new byte[bufferSize];
      int p = 0;
      int q = byteBuffer.Length;
      //int p = bufferOffset;
      //int q = bufferOffset + bufferSize / sizeof(short);
      double lastDuration = 0;
      double lastOnset;
      StringBuilder s = new StringBuilder();
      if (annotationSignalNr == 0)
      {
        s.AppendFormat("{0}{1}{2}{3}", (_dataRecOnset >= 0) ? "+" : "",
                       _dataRecOnset.ToString(TALConsts.ciEnglishUS), (char)20, (char)20);
        lastOnset = DataRecOnset;
      }
      else
        lastOnset = double.MinValue;
      int i = 0;
      while (i < Owner.Count)
      {
        var annotation = Owner[i];
        if ((annotation.DataRecNr == DataRecNr) && (annotation.AnnotationSignalNr == annotationSignalNr))
        {
          if (annotation.Onset == lastOnset)
          {
            if (annotation.Duration == lastDuration)
            {
              s.Append(annotation.Annotation);
              s.Append((char)20);
            }
            else
            {
              if (s.Length > 0)
                result += AddToBuffer(ref byteBuffer, annotationSignalNr, ref p, ref q, s.ToString());
              s = new StringBuilder(annotation.ToString());
              lastOnset = annotation.Onset;
              lastDuration = annotation.Duration;
            }
          }
          else
          {
            if (s.Length > 0)
              result += AddToBuffer(ref byteBuffer, annotationSignalNr, ref p, ref q, s.ToString());
            s = new StringBuilder(annotation.ToString());
            lastOnset = annotation.Onset;
            lastDuration = annotation.Duration;
          }
        }
        i++;
      }
      if (s.Length > 0)
        result += AddToBuffer(ref byteBuffer, annotationSignalNr, ref p, ref q, s.ToString());      
      while (p < q)
      {
        byteBuffer[p] = 0;
        p++;
      }
      p = bufferOffset;
      for (int j = 0; j < byteBuffer.Length; j += 2)
      {
        buffer[p] = BitConverter.ToInt16(byteBuffer, j);
        p++;
      }
      return result;
    }

    private IFormatProvider GetFormatInfo()
    {
      if (Owner != null)
        return Owner.FormatInfo;
      return TALConsts.ciEnglishUS;
    }

    private bool SplitAndAdd(string s, int annotationSignalNr, bool isFirst, int orderIndex)
    {
      Debug.Assert(annotationSignalNr >= 0, TALConsts.AnnotationSignalNrError);
      double duration = 0;
      double onset;
      if ((s == string.Empty) || ((s.IndexOf(TALConsts.c21) == -1) && s.IndexOf(TALConsts.c20) == -1) ||
          (s[s.Length - 1] != TALConsts.c20))
      {
        AddToLastError(annotationSignalNr, s, TalError.InvalidEntry);
        return false;
      }
      bool hasDuration = (s.IndexOf(TALConsts.c21) >= 0);
      string[] sValues = s.Split(new[] { TALConsts.c20, TALConsts.c21 });
      Regex illegalAllowedFloatCharsRegEx = new Regex(TALConsts.AllowedFloatCharsExpr);
      Regex illegalValidFloatCharsRegEx = new Regex(TALConsts.ValidFloatCharsExpr);
      Regex illegalFloatCharsRegEx = new Regex(TALConsts.ValidFloatExclMinPlusExpr);
      if ((sValues.Length > 0) && !illegalAllowedFloatCharsRegEx.IsMatch(sValues[0]) &&
          ((sValues[0][0] == '+') || (sValues[0][0] == '-')))
      {
        if (illegalValidFloatCharsRegEx.IsMatch(sValues[0]))
          AddToLastError(annotationSignalNr, sValues[0], TalError.InvalidOnset);
        if (!double.TryParse(sValues[0], NumberStyles.Float, FormatInfo, out onset))
        {
          AddToLastError(annotationSignalNr, sValues[0], TalError.InvalidOnset);
          return false;
        }
      }
      else
      {
        AddToLastError(annotationSignalNr, sValues[0], TalError.InvalidOnset);
        return false;
      }
      if (hasDuration)
      {
        if ((sValues.Length > 1) && !illegalAllowedFloatCharsRegEx.IsMatch(sValues[1]))
        {
          if (illegalFloatCharsRegEx.IsMatch(sValues[1]))
            AddToLastError(annotationSignalNr, sValues[1], TalError.InvalidDuration);
          if (!double.TryParse(sValues[1], NumberStyles.Float, FormatInfo, out duration))
          {
            AddToLastError(annotationSignalNr, sValues[1], TalError.InvalidDuration);
            return false;
          }
        }
        else
        {
          AddToLastError(annotationSignalNr, sValues[1], TalError.InvalidDuration);
          return false;
        }
      }
      if ((hasDuration && sValues.Length > 2) || (!hasDuration && sValues.Length > 1))
      {
        int iStart = hasDuration ? 2 : 1;
        for (int i = iStart; i < sValues.Length; i++)
        {
          int index;
          if (isFirst)
          {
            if (annotationSignalNr == 0)
            {
              if ((sValues[i] != string.Empty) || hasDuration)
              {
                AddToLastError(annotationSignalNr, string.Empty, TalError.MissingTimeEvent);
                return false;
              }
              _dataRecOnset = onset;
            }
            else
            {
              if ((sValues[i] == string.Empty) && !hasDuration)
                AddToLastError(annotationSignalNr, string.Empty, TalError.ExtraTimeEvent);
              index = Owner.Add(DataRecNr, onset, duration, sValues[i], annotationSignalNr);
              Owner[index].FileOrder = orderIndex;
            }
            isFirst = false;
          }
          else
          {
            if (sValues[i] != string.Empty)
            {
              index = Owner.Add(DataRecNr, onset, duration, sValues[i], annotationSignalNr);
              Owner[index].FileOrder = index;
            }
          }
        }
      }
      return true;
    }

    private IFormatProvider FormatInfo { get { return GetFormatInfo(); } }
    #endregion

        #region Public members
    public bool Contains(double onset)
    {
      return ((Owner.BlockDuration > 0) && (onset >= DataRecOnset) && (onset < DataRecOnset + Owner.BlockDuration));      
    }

    public bool Contains(HPDateTime onset)
    {
      return Contains(Owner.FileStartDateTime.SecDifference(onset));
    }

    public int MinimalBufferSize(int annotationSignalNr)
    {
      short[] a = null;
      return DoWriteToBuffer(ref a, 0, 0, annotationSignalNr);
    }

    public bool ReadFromBuffer(ref short[] buffer, int bufferOffset, int bufferSize, int annotationSignalNr)
    {
      Debug.Assert(annotationSignalNr >= 0, TALConsts.AnnotationSignalNrError);
      _lastError = TalError.None;
      bool result = true;
      int p = bufferOffset;
      int q = bufferOffset + bufferSize / sizeof(short);
      StringBuilder s = new StringBuilder();
      bool isFirst = true;
      int count = 0;
      while (result && (p < q))
      {
        byte[] b = BitConverter.GetBytes(buffer[p]);

        if (b[0] == char.MinValue)
        {
          if (s.Length != 0)
          {
            result = SplitAndAdd(s.ToString(), annotationSignalNr, isFirst, count);
            count++;
            isFirst = false;
            s = new StringBuilder();
          }
        }
        else
          s.Append((char)b[0]);
        if (b[1] == char.MinValue)
        {
          if (s.Length != 0)
          {
            result = SplitAndAdd(s.ToString(), annotationSignalNr, isFirst, count);
            count++;
            isFirst = false;
            s = new StringBuilder();
          }
        }
        else
          s.Append((char)b[1]);
        p++;
      }
      if (s.ToString() != string.Empty)
      {
        result = false;
        AddToLastError(annotationSignalNr, string.Empty, TalError.InvalidFormat);
      }
      _modified = false;
      _dataReadFromBuffer = true;
      return result;
    }

    public bool WriteToBuffer(ref short[] buffer, int bufferOffset, int bufferSize, int annotationSignalNr)
    {
      return (DoWriteToBuffer(ref buffer, bufferOffset, bufferSize, annotationSignalNr) <= bufferSize);
    }
    #endregion

    #region Properties
    public bool BlockModified { get { return _modified; } }

    public bool DataReadFromBuffer { get { return _dataReadFromBuffer; } }

    public int DataRecNr { get { return _dataRecNr; } }

    public double DataRecOnset
    {
      get { return _dataRecOnset; }
      set
      {
        if (_dataRecOnset != value)
        {
          _dataRecOnset = value;
          _modified = true;
        }
      }
    }

    public TalError LastError { get { return _lastError; } }

    public bool Modified
    {
      get
      {
        bool result = _modified;
        if (!result)
        {
          int i = 0;
          while (!result && (i < Owner.Count))
          {
            if (Owner[i].DataRecNr == DataRecNr)
              result = Owner[i].Modified;
            i++;
          }
        }
        return result;
      }
      set
      {
        _modified = value;
        if (value) 
          return;
        int i = 0;
        while (i < Owner.Count)
        {
          if (Owner[i].DataRecNr == DataRecNr)
            Owner[i].Modified = false;
          i++;
        }
      }
    }

    public EdfPlusAnnotationListBase Owner { get { return _owner; } }

    #endregion
  }
}
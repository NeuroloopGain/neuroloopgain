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
using System.Globalization;
using System.Text.RegularExpressions;
using NeuroLoopGainLibrary.Mathematics;

namespace NeuroLoopGainLibrary.DateTimeTypes
{
  public struct HPDateTime : IComparable
  {
    private const int HpTicksPerDateTimeTick = 100000000;
    private const decimal FemtoSecondsPerSecond = 1000m * 1000m * 1000m * 1000m * 1000m;

    #region Private members... don't touch
    private decimal _ticks;  // 1 tick is defined as 1 fs (femto = 10^-15)        
    #endregion

    #region Constructors
    public HPDateTime(decimal ticks)
    {
      _ticks = ticks;
    }

    public HPDateTime(DateTime datetime)
      : this()
    {
      DateTime = datetime;
    }

    public HPDateTime(DateTime datetime, double seconds)
      : this()
    {
      DateTime = datetime;
      _ticks += (decimal)seconds * FemtoSecondsPerSecond;
    }

    public HPDateTime(HPDateTime hpDatetime, double seconds)
      : this()
    {
      _ticks = hpDatetime.Ticks;
      _ticks += (decimal)seconds * FemtoSecondsPerSecond;
    }

    public HPDateTime(HPDateTime hpDatetime)
      : this()
    {
      _ticks = hpDatetime.Ticks;
    }
    #endregion

    #region Operators
    public static bool operator ==(HPDateTime d1, HPDateTime d2)
    {
      return d1.Ticks == d2.Ticks;
    }

    public static bool operator !=(HPDateTime d1, HPDateTime d2)
    {
      return d1.Ticks != d2.Ticks;
    }

    public static bool operator <(HPDateTime d1, HPDateTime d2)
    {
      return d1.Ticks < d2.Ticks;
    }

    public static bool operator <=(HPDateTime d1, HPDateTime d2)
    {
      return d1.Ticks <= d2.Ticks;
    }

    public static bool operator >(HPDateTime d1, HPDateTime d2)
    {
      return d1.Ticks > d2.Ticks;
    }

    public static bool operator >=(HPDateTime d1, HPDateTime d2)
    {
      return d1.Ticks >= d2.Ticks;
    }

    public static HPDateTime operator +(HPDateTime dt, TimeSpan ts)
    {
      return new HPDateTime(dt.Ticks + (decimal)ts.TotalSeconds * FemtoSecondsPerSecond);
    }

    public static HPDateTime operator -(HPDateTime d1, double seconds)
    {
      return new HPDateTime(Math.Max(d1.Ticks - (decimal)seconds * FemtoSecondsPerSecond, 0));
    }

    public static HPDateTime operator +(HPDateTime d1, double seconds)
    {
      return new HPDateTime(d1.Ticks + (decimal)seconds * FemtoSecondsPerSecond);
    }

    #endregion

    #region Public members
    public bool IsAssigned { get { return (_ticks > 0); } }

    public double SecDifference(HPDateTime hpDateTime)
    {
      // TODO (SecDifference): Round properly, instead of casting
      return (double)((_ticks - hpDateTime.Ticks) / FemtoSecondsPerSecond);
    }

    public override int GetHashCode()
    {
      return (int)Ticks;
    }

    public override bool Equals(object obj)
    {
      return obj is HPDateTime ? ((HPDateTime) obj).Ticks == Ticks : base.Equals(obj);
    }

    public override string ToString()
    {
      return ToString(CultureInfo.CurrentCulture);
    }

    public string ToString(CultureInfo cultureInfo)
    {
      return ToString("F", cultureInfo);
    }

    public string ToString(string dateTimeFormat)
    {
      return ToString(dateTimeFormat, CultureInfo.CurrentCulture);
    }

    public string ToString(string dateTimeFormat, int nrDecimals)
    {
      return ToString(dateTimeFormat, CultureInfo.CurrentCulture, nrDecimals);
    }

    public string ToString(string dateTimeFormat, CultureInfo cultureInfo)
    {
      return ToString(dateTimeFormat, cultureInfo, 15);
    }

    public string ToString(string dateTimeFormat, CultureInfo cultureInfo,
      int nrDecimals)
    {
      Regex regex = new Regex("s+");
      MatchCollection matches = regex.Matches(dateTimeFormat);
      StringBuilder sb = new StringBuilder(dateTimeFormat);
      double s = SecondsAfterMidnight - Math.Truncate(SecondsAfterMidnight / 60) * 60;

      nrDecimals = Range.EnsureRange(nrDecimals, 0, 15);

      for (int i = matches.Count - 1; i >= 0; i--)
      {
        int l = matches[i].Length;
        if (!Range.InRange(l, 3, 5))
          continue;
        sb.Remove(matches[i].Index, l);
        switch (l)
        {
          case 3:
            sb.Insert(matches[i].Index, Math.Round(s, nrDecimals).ToString(
              cultureInfo));
            break;
          case 4:
            sb.Insert(matches[i].Index, Math.Round(s, nrDecimals).ToString(
              "00."+new string('#',nrDecimals), cultureInfo));
            break;
          case 5:
            sb.Insert(matches[i].Index, Math.Round(SecondsAfterMidnight,
                                                   nrDecimals).ToString(cultureInfo));
            break;
          default:
            break;
        }
      }

      return DateTime.ToString(sb.ToString(), cultureInfo);
    }

    #endregion

    #region Properties
    public DateTime DateTime
    {
      get { return new DateTime((long)(_ticks / HpTicksPerDateTimeTick)); }
      set
      {
        _ticks = (decimal)value.Ticks * HpTicksPerDateTimeTick;
      }
    }

    public decimal Ticks { get { return _ticks; } }

    public double SecondsAfterMidnight
    {
      get
      {
        TimeSpan ts = new TimeSpan(DateTime.Date.Ticks);
        decimal ticksAfterMidnight = Ticks - (decimal)ts.TotalSeconds * FemtoSecondsPerSecond;
        return (double)(ticksAfterMidnight / FemtoSecondsPerSecond);
      }
    }
    #endregion

    #region IComparable Members

    public int CompareTo(object obj)
    {
      if (obj == null)
        return 1;
      if (!(obj is HPDateTime))
        throw new ArgumentException("Can only compare HPDateTimes");
      if (Ticks > ((HPDateTime)obj).Ticks)
        return 1;
      if (Ticks < ((HPDateTime)obj).Ticks)
        return -1;
      return 0;
    }

    #endregion

  }
}

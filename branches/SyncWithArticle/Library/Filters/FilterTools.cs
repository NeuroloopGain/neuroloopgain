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
using System.Linq;
using NeuroLoopGainLibrary.Text;

namespace NeuroLoopGainLibrary.Filters
{
  /// <summary>
  /// Class to convert a string to an initialised filter and vice versa.
  /// </summary>
  public static class FilterTools
  {
    #region public fields

    /// <summary>
    /// List with all available filters
    /// </summary>
    public static string[] FilterNames =
      new[]
        {
          "LP", "HP", "NOTCH", "ABS", "ABSSINGLESIDE", "BP", "DUE", "SE", "G", "1-G", "BP12",
          "DCATT", "1/DCATT", "DUE1987", "DUE2007", "LP05", "LP05NS", "HP05NS", "MEDIAN", "SQR",
          "SQRT", "MEAN", "PERCENTILLIAN", "DB", "1/{1-C*X}", "DESPECKLER"
        };

    #endregion public fields

    #region public methods

    [Obsolete("Use FilterNames instead.")]
    public static void GetAllFilterNames()
    {
      throw new NotImplementedException("Not implemented");
    }

    /// <summary>
    /// Gets a filter instance using it's name.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <returns>A <see cref="FilterBase"/> instance of type described by a valid filterName; otherwise, <c>null</c>.</returns>
    public static FilterBase GetFilterByName(string filterName)
    {
      filterName = filterName.Trim().ToUpper();
      int filterNameIndex = FilterNames.ToList().IndexOf(filterName);
      if (filterNameIndex < 0)
        return null;
      FilterType filterType = (FilterType)Enum.ToObject(typeof(FilterType), filterNameIndex);
      switch (filterType)
      {
        //case FilterType.LP:
        //  return new LPFilter();
        //case FilterType.HP:
        //  return new HPFilter();
        //case FilterType.NOTCH:
        //  return new NotchFilter();
        //case FilterType.RECT:
        //  return new Rectifier();
        //case FilterType.SingleSideRect:
        //  return new SingleSideRectifier();
        //case FilterType.BP:
        //  return new BPFilter();
        case FilterType.DUE:
          return new DUEFilter();
        case FilterType.SE:
          return new SEFilter();
        //case FilterType.G:
        //  return new GFilter();
        //case FilterType.InvG:
        //  return new InvGFilter();
        //case FilterType.BP12:
        //  return new BP12Filter();
        //case FilterType.DCatt:
        //  return new DCattFilter();
        //case FilterType.InvDCatt:
        //  return new INVDCattFilter();
        //case FilterType.DUE1987:
        //  return new DUE1987Filter();
        //case FilterType.DUE2007:
        //  return new DUE2007Filter();
        //case FilterType.LP05:
        //  return new LP05Filter();
        //case FilterType.LP05NS:
        //  return new LP05NSFilter();
        //case FilterType.HP05NS:
        //  return new HP05NSFilter();
        //case FilterType.Median:
        //  return new MedianFilter();
        //case FilterType.Square:
        //  return new SqrFilter();
        //case FilterType.Squareroot:
        //  return new SqrtFilter();
        //case FilterType.Mean:
        //  return new MeanFilterQuick();
        //case FilterType.Percentillian:
        //  return new PercentillianFilter();
        //case FilterType.dB:
        //  return new DBFilter();
        //case FilterType.GainDivOneMinusX:
        //  return new GainDivOneMinusX();
        //case FilterType.Despeckler:
        //  return new Despeckler();
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    /// <param name="filterType">Type of the filter.</param>
    /// <returns>A <see cref="string"/> containing the name of the filter.</returns>
    public static string GetFilterName(FilterType filterType)
    {
      int i = Convert.ToInt32(filterType);
      return FilterNames[i];
    }

    /// <summary>
    /// Gets the type of the filter.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>The <see cref="FilterType"/> corresponding to the filter.</returns>
    public static FilterType GetFilterType(FilterBase filter)
    {
      string s = filter.GetType().Name.ToUpper();
      if (s == "LPFILTER")
        return FilterType.LP;
      if (s == "HPFILTER")
        return FilterType.HP;
      if (s == "DUEFILTER")
        return FilterType.DUE;
      if (s == "SEFILTER")
        return FilterType.SE;
      if (s == "NOTCHFILTER")
        return FilterType.NOTCH;
      if (s == "GFILTER")
        return FilterType.G;
      if (s == "INVGFILTER")
        return FilterType.InvG;
      if (s == "BPFILTER")
        return FilterType.BP;
      if (s == "RECTIFIER")
        return FilterType.RECT;
      if (s == "SINGLESIDERECTIFIER")
        return FilterType.SingleSideRect;
      if (s == "BP12FILTER")
        return FilterType.BP12;
      if (s == "DCATTFILTER")
        return FilterType.DCatt;
      if (s == "INVDCATTFILTER")
        return FilterType.InvDCatt;
      if (s == "DUE1987FILTER")
        return FilterType.DUE1987;
      if (s == "DUE2007FILTER")
        return FilterType.DUE2007;
      if (s == "LP05FILTER")
        return FilterType.LP05;
      if (s == "LP05NSFILTER")
        return FilterType.LP05NS;
      if (s == "HP05NSFILTER")
        return FilterType.HP05NS;
      if (s == "MEDIANFILTER")
        return FilterType.Median;
      if (s == "SQRFILTER")
        return FilterType.Square;
      if (s == "SQRTFILTER")
        return FilterType.Squareroot;
      if (s == "MEANFILTERQUICK")
        return FilterType.Mean;
      if (s == "PERCENTILLIANFILTER")
        return FilterType.Percentillian;
      if (s == "DBFILTER")
        return FilterType.dB;
      if (s == "GAINDIVONEMINUSX")
        return FilterType.GainDivOneMinusX;
      if (s == "DESPECKLER")
        return FilterType.Despeckler;
      throw new ArgumentException("Unknown filter object");
    }

    /// <summary>
    /// Converts a filter string, returning an initialised filter.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>If the value is a valid filter, an initialised filter; otherwise <c>null</c>.</returns>
    public static FilterBase Parse(string value)
    {
      FilterBase result = null;
      string s = value;
      if (s.Contains("(") && s.EndsWith(")"))
      {
        result = GetFilterByName(TextUtils.StripStringValue(ref s, "("));
        if (result != null)
        {
          s = s.Remove(s.Length - 1, 1);
          int i = 2;
          while (s != string.Empty)
          {
            double parsedValue;
            string t = TextUtils.StripStringValue(ref s, "/");
            if (double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedValue))
            {
              if (!result.Setting[i].IsReadOnly)
                result.Setting[i].Value = parsedValue;
            }
            else
              result.StringSetting[i] = t;
            i++;
          }
        }
      }
      else
        if (!s.Contains("(") && !s.Contains(")"))
          result = GetFilterByName(s.Trim());
      return result;
    }

    [Obsolete("Use method Parse instead.")]
    public static FilterBase StrToFilter(string value)
    {
      return Parse(value);
    }

    public static void ValidSampleFrequency()
    {
      throw new NotImplementedException("Not implemented");
    }

    #endregion public methods
  }

}


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
using System.Linq;
using NeuroLoopGainLibrary.Text;

namespace NeuroLoopGainLibrary.Mathematics
{
  public static class MathEx
  {
    #region delegates and events

    #region delegates

    public delegate double NRFindRootFunction(double x);

    #endregion delegates

    #endregion delegates and events

    #region private fields

    private const double DoubleResolution = 1E-15 * FuzzFactor;
    private const int FuzzFactor = 1000;

    #endregion private fields

    #region public fields

    /// <summary>
    /// The SI prefixes; m=10^-6, k=10^3, etc
    /// </summary>
    public static char[] DimensionPrefix = new[]
                                               {
                                                 'y', 'z', 'a', 'f', 'p', 'n', 'u', 'm', 'c', 'd', ' ',
                                                 'D', 'H', 'K', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y'
                                               };
    /// <summary>
    /// Determines if the SI prefixes are case sensitive
    /// </summary>
    public static bool[] DimensionPrefixIgnoreCase = new[]
                                                       {
                                                         false, false, false, false, false, false, false, false, false,
                                                         false, false, false, true, true, false, false, false, false,
                                                         false, false, false
                                                       };
    /// <summary>
    /// The equivalent value for the SI prefixes
    /// </summary>
    public static double[] DimensionPrefixValue = new[]
                                                    {
                                                      1e-24, 1e-21, 1e-18, 1e-15, 1e-12, 1e-9, 1e-6, 1e-3, 1e-2, 1e-1,
                                                      1e0, 1e1, 1e2, 1e3, 1e6, 1e9, 1e12, 1e15, 1e18, 1e21, 1e24
                                                    };
    /// <summary>
    /// Value equal to 0.5*Sqrt(2)
    /// </summary>
    public static double Trig2 = Math.Sqrt(2) / 2;

    #endregion public fields

    #region public methods

    /// <summary>
    /// Calculates the average of the specified values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns></returns>
    public static double Average(double[] values)
    {
      return Average(values, 0, values.Length - 1);
    }

    /// <summary>
    /// Calculates the average of the specified values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="idxMin">The array lower limit.</param>
    /// <param name="idxMax">The array lower limit.</param>
    /// <returns></returns>
    public static double Average(double[] values, int idxMin, int idxMax)
    {
      if (idxMax - idxMin + 1 <= 0)
        return 0;

      double averageValue = 0;

      for (int i = idxMin; i <= idxMax; i++)
        averageValue += values[i];

      return averageValue / (idxMax - idxMin + 1);
    }

    /// <summary>
    /// Calculates the average of the specified values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="lowerLimit">The array lower limit.</param>
    /// <param name="upperLimit">The array upper limit.</param>
    /// <returns></returns>
    public static int Average(int[] values, int lowerLimit, int upperLimit)
    {
      int averageValue = 0;
      int nrValidValues = 0;
      foreach (int t in values)
      {
        if (t >= lowerLimit && t <= upperLimit)
        {
          averageValue += t;
          nrValidValues++;
        }
      }
      if (nrValidValues > 0)
        averageValue = (int)Math.Round((double)averageValue / nrValidValues);

      return averageValue;
    }

    /// <summary>
    /// Compares the two values, using the SameValue() method to check if the value are equal.
    /// </summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <param name="epsilon">The epsilon.</param>
    /// <returns></returns>
    public static int CompareEps(double value1, double value2, double epsilon = 0)
    {
      if (SameValue(value1, value2, epsilon))
        return 0;
      if (value1 < value2)
        return -1;
      return 1;
    }

    /// <summary>
    /// Calculates the CoTan of a value.
    /// </summary>
    /// <param name="a">The value.</param>
    /// <returns></returns>
    public static double CoTan(double a)
    {
      return 1d / Math.Tan(a);
    }

    /// <summary>
    /// Calculates the epsilon (used for comparing float values) for given variables.
    /// </summary>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <param name="factor">The factor.</param>
    /// <returns>The calculated epsilon multiplied by the factor.</returns>
    public static double Epsilon(double a, double b, int factor = 1)
    {
      return factor * Math.Max(Math.Min(Math.Abs(a), Math.Abs(b)) * DoubleResolution, DoubleResolution);
    }

    /// <summary>
    /// Check if the specified value is even.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static bool Even(int value)
    {
      return ((value & 1) == 0);
    }

    /// <summary>
    /// Converts a LogFloat encoded value to the actual (double) value.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <param name="y0">The y0 conversion parameter.</param>
    /// <param name="a">The a conversion parameter.</param>
    /// <returns></returns>
    public static double ExpInteger(short value, double y0, double a)
    {
      if (value > 0)
        return y0 * Math.Exp(a * value);
      if (value < 0)
        return -y0 * Math.Exp(-a * value);
      return 0;
    }

    /// <summary>
    /// Converts a LogFloat encoded value to the actual (double) value.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <param name="y0">The y0 conversion parameter.</param>
    /// <param name="a">The a conversion parameter.</param>
    /// <returns></returns>
    public static double ExpInteger(int value, double y0, double a)
    {
      if (value > 0)
        return y0 * Math.Exp(a * value);
      if (value < 0)
        return -y0 * Math.Exp(-a * value);
      return 0;
    }

    /// <summary>
    /// Converts a LogFloat encoded value to the actual (double) value.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <param name="y0">The y0 conversion parameter.</param>
    /// <param name="a">The a conversion parameter.</param>
    /// <returns></returns>
    public static double ExpInteger(double value, double y0, double a)
    {
      if (value > 0)
        return y0 * Math.Exp(a * value);
      if (value < 0)
        return -y0 * Math.Exp(-a * value);
      return 0;
    }

    /// <summary>
    /// Finds the root (F(x) = 0) of given function with x between x0 and x1.
    /// </summary>
    /// <param name="f">The function.</param>
    /// <param name="x0">The x0.</param>
    /// <param name="x1">The x1.</param>
    /// <param name="xzero">The x where F(x) = 0.</param>
    /// <param name="xerror">The error (Abs(F(x)).</param>
    /// <returns></returns>
    public static bool FindRoot(NRFindRootFunction f, double x0, double x1,
      out double xzero, ref double xerror)
    {
      // Check if there is a root between x0 and x1
      if (f(x0) * f(x1) >= 0)
      {
        xzero = x0 - 1;
        xerror = double.MaxValue;
        return false;
      }
      bool sameError = false;
      double lastError = 0;
      double x2;
      do
      {
        x2 = (x0 * f(x1) - x1 * f(x0)) / (f(x1) - f(x0));
        if (!(Math.Abs(f(x2)) <= xerror))
          if (f(x0) * f(x2) > 0)
            x0 = x1;
        x1 = x2;
        if (SameValue(lastError, Math.Abs(f(x2))))
          sameError = true;
        else
          lastError = Math.Abs(f(x2));
      } while (!sameError && (Math.Abs(f(x2)) > xerror));
      xzero = x2;
      xerror = Math.Abs(f(x2));
      return true;
    }

    /// <summary>
    /// Returns the fraction of the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static double Frac(double value)
    {
      return value - Math.Truncate(value);
    }

    /// <summary>
    /// Gets the SI dimension prefix.
    /// </summary>
    /// <param name="dimension">The dimension.</param>
    /// <returns>The SI prefix.</returns>
    public static char GetPrefix(string dimension)
    {
      dimension = dimension.Trim();
      if ((dimension.Length > 1) && ((dimension[1] == '%') || TextUtils.IsAlpha(dimension[1])))
      {
        int i = 0;
        while (i < DimensionPrefix.Length)
        {
          if ((dimension[0] == DimensionPrefix[i]) || (DimensionPrefixIgnoreCase[i] && (char.ToUpper(dimension[0]) == char.ToUpper(DimensionPrefix[i]))))
            return DimensionPrefix[i];
          i++;
        }
      }
      return '\0';
    }

    /// <summary>
    /// Returns 1 if value > 0; else 0
    /// </summary>
    /// <param name="x">The value.</param>
    /// <returns></returns>
    public static double Heav(double x)
    {
      if (!IsZero(x))
        x = x / Math.Abs(x);
      return 0.5 * (x + 1);
    }

    /// <summary>
    /// Determines whether the specified value is an integer value.
    /// </summary>
    /// <param name="a">The value.</param>
    /// <returns>
    ///   <c>true</c> if the specified a is integer; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsInteger(double a)
    {
      return SameValue(a, Math.Round(a, MidpointRounding.AwayFromZero));
    }

    /// <summary>
    /// Determines whether the specified value is zero.
    /// </summary>
    /// <param name="a">The value to be examined</param>
    /// <param name="epsilon">The error range.</param>
    /// <returns>
    ///   <c>true</c> if the specified a is zero; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsZero(double a, double epsilon = 0)
    {
      return SameValue(a, 0, epsilon);
    }

    /// <summary>
    /// Limits the number of decimals.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="nrDecimals">The number of decimals.</param>
    /// <returns>The value rounded up to nrDecimals decimals.</returns>
    public static double LimitDecimals(double value, int nrDecimals)
    {
      return Math.Round(value, nrDecimals, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Limits the nmber of significant digits.
    /// </summary>
    /// <param name="r">The value.</param>
    /// <param name="nrDigits">The number of significant digits.</param>
    /// <param name="roundingMode">The mode the value is rounded.</param>
    /// <returns></returns>
    public static double LimitSignificantDigits(double r, int nrDigits, RoundingMode roundingMode = RoundingMode.Nearest)
    {
      if (IsZero(r))
        return 0;

      int exponent = (int)Math.Truncate(Math.Log10(Math.Abs(r)));
      double mantissa = r / (Math.Pow(10, exponent));
      int nrDig;
      if (Math.Abs(mantissa) < 1d)
        nrDig = nrDigits;
      else
        nrDig = nrDigits - 1;
      switch (roundingMode)
      {
        case RoundingMode.Nearest:
          mantissa = Math.Round(mantissa * Math.Pow(10, nrDig));
          break;
        case RoundingMode.Up:
          mantissa = Math.Ceiling(mantissa * Math.Pow(10, nrDig));
          break;
        case RoundingMode.Down:
          mantissa = Math.Floor(mantissa * Math.Pow(10, nrDig));
          break;
        case RoundingMode.Truncate:
          mantissa = Math.Truncate(mantissa * Math.Pow(10, nrDig));
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      return mantissa / Math.Pow(10, nrDig - exponent);
    }

    /// <summary>
    /// Calculate the natural logarithm of the specified value.
    /// </summary>
    /// <param name="a">The value</param>
    /// <returns>The natural logarithm of the specified value</returns>
    public static double Ln(double a)
    {
      return Math.Log(a, Math.E);
    }

    /// <summary>
    /// Converts a value to the equivalent LogFloat encoded value.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <param name="y0">The y0 conversion parameter.</param>
    /// <param name="a">The a conversion parameter.</param>
    /// <returns>The LogFloat encoded value</returns>
    public static short LogFloat(double value, double y0, double a)
    {
      double r;

      if (value > y0)
      {
        r = (Math.Log(value) - Math.Log(y0)) / a;
        return (short)Math.Round(Math.Min(r, short.MaxValue));
      }

      if (value < -y0)
      {
        r = (-Math.Log(-value) + Math.Log(y0)) / a;
        return (short)Math.Round(Math.Max(r, -short.MaxValue));
      }

      return 0;
    }

    /// <summary>
    /// Get the minimal difference between two adjecent values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>The minimal difference between two adjecent values</returns>
    public static double MinDifference(double[] values)
    {
      double result = double.MaxValue;
      for (int i = 1; i < values.Length; i++)
      {
        double diff = values[i] - values[i - 1];
        if (diff < result)
          result = diff;
      }
      return result;
    }

    /// <summary>
    /// Returns the minimal and maximal value of the specified array of values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="maxValue">The maximum value.</param>
    public static void MinMax(double[] values, out double minValue, out double maxValue)
    {
      minValue = double.MaxValue;
      maxValue = double.MinValue;

      foreach (double d in values)
      {
        if (d < minValue)
          minValue = d;
        if (d > maxValue)
          maxValue = d;
      }
    }

    /// <summary>
    /// Checks if the specified value is odd.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if the value is odd; otherwise <c>false</c></returns>
    public static bool Odd(int value)
    {
      return ((value & 1) != 0);
    }

    /// <summary>
    /// Converts a prefix like 'y' into a corresponding value (in this case '1e-24')
    /// </summary>
    /// <param name="prefix">The prefix.</param>
    /// <returns>Corresponding value for the given prefix.</returns>
    public static double PrefixToValue(char prefix)
    {
      double result = 1;
      int i = 0;
      while (SameValue(result, 1) && (i < DimensionPrefix.Length))
      {
        if (prefix == DimensionPrefix[i])
          result = DimensionPrefixValue[i];
        i++;
      }
      return result;
    }

    /// <summary>
    /// Rounds the nearest value, away from zero.
    /// Default Round() uses bankers rounding, towards the nearest even integer value.
    /// </summary>
    /// <param name="value">The value to be rounded.</param>
    /// <returns>The away from zero rounded value.</returns>
    public static int RoundNearest(double value)
    {
      return (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Checks if two value have the same value.
    /// </summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="epsilon">The error range.</param>
    /// <returns><c>true</c> if the values are (almost) equal; otherwise <c>false</c></returns>
    public static bool SameValue(double value1, double value2, double epsilon = 0)
    {
      if (Math.Abs(epsilon) < DoubleResolution)
        epsilon = Epsilon(value1, value2);
      if (value1 > value2)
        return ((value1 - value2) <= epsilon);
      return ((value2 - value1) <= epsilon);
    }

    /// <summary>
    /// Returns the square value of the parameter.
    /// </summary>
    /// <param name="value">The value to be squared</param>
    /// <returns>The squared value</returns>
    public static double Sqr(double value)
    {
      return value * value;
    }

    /// <summary>
    /// Calculates the standards the deviation (N).
    /// </summary>
    /// <param name="values">The input values.</param>
    /// <returns></returns>
    public static double StandardDeviation(double[] values)
    {
      if (values.Length == 0)
        return 0;

      double averageValue = Average(values);
      double sumOfSquares = values.Aggregate<double, double>(0, (current, t) => current + Math.Pow((t - averageValue), 2));
      return Math.Sqrt(sumOfSquares / values.Length);
    }

    /// <summary>
    /// Calculates the sample standards the deviation (N-1).
    /// </summary>
    /// <param name="values">The input values.</param>
    /// <returns></returns>
    public static double StandardDeviationN1(double[] values)
    {
      if (values.Length == 0)
        return 0;

      double averageValue = Average(values);
      double sumOfSquares = values.Aggregate<double, double>(0, (current, t) => current + Math.Pow((t - averageValue), 2));
      return Math.Sqrt(sumOfSquares / (values.Length - 1));
    }

    /// <summary>
    /// Calculates the sum of the specified values.
    /// </summary>
    /// <param name="values">The input values.</param>
    /// <returns></returns>
    public static double Sum(double[] values)
    {
      return values.Sum();
    }

    #endregion public methods
  }
}

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

namespace NeuroLoopGainLibrary.Mathematics
{
  public class Range
  {
    #region Constructors

    public Range(double lower, double upper)
    {
      LowerBound = lower;
      UpperBound = upper;
    }

    #endregion Constructors

    #region Public Fields

    public readonly double LowerBound;
    public readonly double UpperBound;

    #endregion Public Fields

    #region Public Methods

    /// <summary>
    /// Checks if the specified value is between or on bounds.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if value between bounds</returns>
    public bool BetweenBounds(double value)
    {
      return BetweenBounds(value, LowerBound, UpperBound);
    }

    /// <summary>
    /// Checks if the specified value is between or on bounds.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="bound1">Bound 1.</param>
    /// <param name="bound2">Bound 2.</param>
    /// <returns><c>true</c> if value between bounds</returns>
    public static bool BetweenBounds(double value, double bound1, double bound2)
    {
      if (bound1 <= bound2)
        return ((bound1 <= value) && (value <= bound2));
      return ((bound2 <= value) && (value <= bound1));
    }

    /// <summary>
    /// Checks if the specified value is between or on bounds.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="bound1">Bound 1.</param>
    /// <param name="bound2">Bound 2.</param>
    /// <returns><c>true</c> if value between bounds</returns>
    public static bool BetweenBounds(int value, int bound1, int bound2)
    {
      if (bound1 <= bound2)
        return ((bound1 <= value) && (value <= bound2));
      return ((bound2 <= value) && (value <= bound1));
    }

    /// <summary>
    /// Ensures that the value is between the specified min and max value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="minValue">The min value.</param>
    /// <param name="maxValue">The max value.</param>
    /// <returns>The value within the specified range</returns>
    public static decimal EnsureRange(decimal value, decimal minValue, decimal maxValue)
    {
      return value < minValue ? minValue : value > maxValue ? maxValue : value;
    }

    /// <summary>
    /// Ensures that the value is between the specified min and max value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="minValue">The min value.</param>
    /// <param name="maxValue">The max value.</param>
    /// <returns>The value within the specified range</returns>
    public static int EnsureRange(int value, int minValue, int maxValue)
    {
      return value < minValue ? minValue : value > maxValue ? maxValue : value;
    }

    /// <summary>
    /// Ensures that the value is between the specified min and max value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="minValue">The min value.</param>
    /// <param name="maxValue">The max value.</param>
    /// <returns>The value within the specified range</returns>
    public static short EnsureRange(short value, short minValue, short maxValue)
    {
      return value < minValue ? minValue : value > maxValue ? maxValue : value;
    }

    /// <summary>
    /// Ensures that the value is between the specified min and max value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="minValue">The min value.</param>
    /// <param name="maxValue">The max value.</param>
    /// <returns>The value within the specified range</returns>
    public static double EnsureRange(double value, double minValue, double maxValue)
    {
      return value < minValue ? minValue : value > maxValue ? maxValue : value;
    }

    /// <summary>
    /// Ensures that the value is between the specified min and max value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="minValue">The min value.</param>
    /// <param name="maxValue">The max value.</param>
    /// <returns>The value within the specified range</returns>
    public static long EnsureRange(long value, long minValue, long maxValue)
    {
      return value < minValue ? minValue : value > maxValue ? maxValue : value;
    }

    /// <summary>
    /// Checks if the specified value is within range.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if value within range</returns>
    public bool InRange(double value)
    {
      return InRange(value, LowerBound, UpperBound);
    }

    /// <summary>
    /// Checks if the specified value is within range.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="lowerBound">The lower bound.</param>
    /// <param name="upperBound">The upper bound.</param>
    /// <returns><c>true</c> if value within range</returns>
    public static bool InRange(double value, double lowerBound, double upperBound)
    {
      return ((lowerBound <= value) && (value <= upperBound));
    }

    /// <summary>
    /// Checks if the specified value is within range.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="lowerBound">The lower bound.</param>
    /// <param name="upperBound">The upper bound.</param>
    /// <returns><c>true</c> if value within range</returns>
    public static bool InRange(int value, int lowerBound, int upperBound)
    {
      return ((lowerBound <= value) && (value <= upperBound));
    }

    /// <summary>
    /// Checks if the specified value is within range.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="lowerBound">The lower bound.</param>
    /// <param name="upperBound">The upper bound.</param>
    /// <returns><c>true</c> if value within range</returns>
    public static bool InRange(long value, long lowerBound, long upperBound)
    {
      return ((lowerBound <= value) && (value <= upperBound));
    }

    /// <summary>
    /// Checks if the given values are in the range given by range1 to range2.
    /// </summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="range1">The range lower bound.</param>
    /// <param name="range2">The range upper bound.</param>
    /// <returns><c>true</c> if both values are in range, or <c>false</c> if not</returns>
    public static bool SameRange(int value1, int value2, int range1, int range2)
    {
      return InRange(value1, range1, range2) && InRange(value2, range1, range2);
    }

    /// <summary>
    /// Returns the maximum of two values.
    /// </summary>
    /// <param name="r">The first value.</param>
    /// <param name="s">The second value.</param>
    /// <returns></returns>
    public static double Max(double r, double s)
    {
      return r > s ? r : s;
    }

    /// <summary>
    /// Returns the maximum of two values.
    /// </summary>
    /// <param name="r">The first value.</param>
    /// <param name="s">The second value.</param>
    /// <returns></returns>
    public static int Max(int r, int s)
    {
      return r > s ? r : s;
    }

    /// <summary>
    /// Returns the minimum of two values.
    /// </summary>
    /// <param name="r">The first value.</param>
    /// <param name="s">The second value.</param>
    /// <returns></returns>
    public static double Min(double r, double s)
    {
      return r < s ? r : s;
    }

    /// <summary>
    /// Returns the minimum of two values.
    /// </summary>
    /// <param name="r">The first value.</param>
    /// <param name="s">The second value.</param>
    /// <returns></returns>
    public static int Min(int r, int s)
    {
      return r < s ? r : s;
    }

    #endregion Public Methods
  }
}
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
using NeuroLoopGainLibrary.Errorhandling;

namespace NeuroLoopGainLibrary.Filters
{
  /// <summary>
  /// This class is the ancestor of all IIR filters; IIR filters use Poles and Zeros to implement the filter characteristics.
  /// </summary>
  public class IIRFilterBase : FilterBase
  {
    #region protected fields

    protected double[] FilterStateP;
    protected double[] FilterStateZ;
    protected double[] Poles;
    protected double[] Zeros;

    #endregion protected fields

    #region protected methods

    /// <summary>
    /// Check the filter setting and calculates the IIR coeffients used to filter data.
    /// </summary>
    protected virtual void CalculateIIRCoeff()
    {
      int i = CheckSettings();
      if (i != 0)
        throw new RangeException(string.Format("{0}: Filter setting {1} out of range", GetType().Name, Setting[i].Info));
      Setting.NewSettings = false;
    }

    protected override FilterStateBase DoBackupFilterState()
    {
      return new IIRFilterState(FilterStateP, FilterStateZ);
    }

    protected override void DoFilterSamples(double[] samplesIn, double[] samplesOut, int idxStart, int idxEnd, int outIdxStart = -1)
    {
      int idx, idxLast;
      double s = 0;
      if (Setting.NewSettings)
        CalculateIIRCoeff();
      // Filter from IdxStart to IdxEnd avoiding endless loop
      int step = idxEnd != idxStart ? Math.Sign(((long)idxEnd - idxStart)) : 1;
      int outIdx = outIdxStart < 0 ? idxStart : outIdxStart;
      if (Direction == FilterDirectionType.Forward)
      {
        idx = idxStart;
        idxLast = idxEnd + 1;
      }
      else
      {
        idx = idxEnd;
        idxLast = idxStart - 1;
        step = -step;
      }
      while (idx != idxLast)
      {
        FilterStateZ[0] = samplesIn[idx];
        if (UseFirstSampleToReset)
        {
          DoReset(FilterStateZ[0]);
          UseFirstSampleToReset = false;
        }
        // Compute new output sample
        double r = 0;
        // Add past output-values
        int i;
        for (i = 1; i <= Poles.GetUpperBound(0); i++)
          r = r + Poles[i] * FilterStateP[i];
        // Not anticipate = do not include current input sample in output value
        if (!Anticipate)
          s = r;
        // Add past input-values
        for (i = 0; i <= Zeros.GetUpperBound(0); i++)
          r = r + Zeros[i] * FilterStateZ[i];
        // Anticipate = include current input sample in output value
        if (Anticipate)
          s = r;
        // Do backpolation (FilterStateP[1] = Last out-sample)
        s = BackPolate * FilterStateP[1] + (1.0 - BackPolate) * s;
        // Scale result 
        samplesOut[outIdx] = s;
        // Update filter state
        for (i = Poles.GetUpperBound(0); i >= 2; i--)
          FilterStateP[i] = FilterStateP[i - 1];
        FilterStateP[1] = r;
        for (i = Zeros.GetUpperBound(0); i >= 1; i--)
          FilterStateZ[i] = FilterStateZ[i - 1];
        // Next sample
        idx += step;
        outIdx += step;
      }
    }

    protected override void DoReset()
    {
      base.DoReset();

      if (Setting.NewSettings)
        CalculateIIRCoeff();

      if (FilterStateZ != null)
        Array.Clear(FilterStateZ, 0, FilterStateZ.Length);

      if (FilterStateP != null)
        Array.Clear(FilterStateP, 0, FilterStateP.Length);
    }

    protected override void DoReset(bool useNextSample)
    {
      base.DoReset(useNextSample);

      if (!useNextSample)
        DoReset();
    }

    protected override void DoReset(double value)
    {
      if (Setting.NewSettings)
        CalculateIIRCoeff();

      double r = 0;
      for (int i = 0; i <= Zeros.GetUpperBound(0); i++)
        r = r + Zeros[i];

      double s = 1;
      for (int i = 1; i <= Poles.GetUpperBound(0); i++)
        s = s - Poles[i];

      for (int i = 0; i <= FilterStateZ.GetUpperBound(0); i++)
        FilterStateZ[i] = value;

      double y = value * r / s;
      for (int i = 0; i <= FilterStateZ.GetUpperBound(0); i++)
        FilterStateP[i] = y;
    }

    protected override void DoRestoreFilterState(FilterStateBase filterState)
    {
      FilterStateP = ((IIRFilterState)filterState).FilterStateP;
      FilterStateZ = ((IIRFilterState)filterState).FilterStateZ;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IIRFilterBase"/> class.
    /// </summary>
    protected IIRFilterBase(int nrSettings)
      : base(nrSettings)
    {
      Anticipate = true;
      Setting[0].Info = "Abstract Custom IIRFilter";
    }

    #endregion protected methods

    #region public properties

    /// <summary>
    /// Gets or sets a value indicating whether the current sample is included in the output value
    /// </summary>
    /// <value>
    ///   <c>true</c> if current input sample is included in the output value; otherwise, <c>false</c>.
    /// </value>
    public bool Anticipate { get; set; }

    /// <summary>
    /// Gets or sets the backpolate value. Backpolating can be used to interpolate the filter output value between the previous and current calculated output value.
    /// The filter output value is backpolate*PreviousOutput+(1-backpolate)*CurrentOutput.
    /// </summary>
    /// <value>
    /// The backpolate value (range 0-1).
    /// </value>
    public double BackPolate { get; set; }

    #endregion public properties
  }
}

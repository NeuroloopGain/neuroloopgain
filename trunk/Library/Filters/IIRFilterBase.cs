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
  public class IIRFilterBase : FilterBase
  {
    #region Protected Fields

    protected double[] FFilterStateP;
    protected double[] FFilterStateZ;
    protected double[] FPoles;
    protected double[] FZeros;

    #endregion Protected Fields

    #region Protected Methods

    protected virtual void CalculateIIRCoeff()
    {
      int i = CheckSettings();
      if (i != 0)
        throw new RangeException(string.Format("{0}: Filter setting {1} out of range", GetType().Name,
                                               Setting[i].Info));
      Setting.NewSettings = false;
    }

    protected override FilterStateBase DoBackupFilterState()
    {
      return new IIRFilterState(FFilterStateP, FFilterStateZ);
    }

    protected override void DoRestoreFilterState(FilterStateBase filterState)
    {
      FFilterStateP = ((IIRFilterState)filterState).FilterStateP;
      FFilterStateZ = ((IIRFilterState)filterState).FilterStateZ;
    }

    #endregion Protected Methods

    #region Constructors

    public IIRFilterBase()
    {
      Anticipate = true;
      Setting[0].Info = "Abstract Custom IIRFilter";
      Setting[1].Info = "SampleFrequency";
      Setting[2].Info = "FilterGain";
    }

    #endregion Constructors

    #region Public Properties

    public bool Anticipate { get; set; }

    public double BackPolate { get; set; }

    #endregion Public Properties

    #region Public Methods

    public override void FilterSamples(double[] samplesIn, double[] samplesOut, int idxStart, int idxEnd, int outIdxStart = -1)
    {
      int idx, idxLast;
      double s = 0;
      if (Setting.NewSettings)
        CalculateIIRCoeff();
      // Filter from IdxStart to IdxEnd avoiding endless loop
      int step = idxEnd != idxStart ? Math.Sign(((long)idxEnd - idxStart)) : 1;
      int outIdx = outIdxStart < 0 ? idxStart : outIdxStart;
      if (Direction == FilterDirectionType.ForwardOnly)
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
        FFilterStateZ[0] = samplesIn[idx];
        if (UseFirstSampleToReset)
        {
          Reset(FFilterStateZ[0]);
          UseFirstSampleToReset = false;
        }
        // Compute new output sample
        double r = 0;
        // Add past output-values
        int i;
        for (i = 1; i <= FPoles.GetUpperBound(0); i++)
          r = r + FPoles[i] * FFilterStateP[i];
        // Not anticipate = do not include current input sample in output value
        if (!Anticipate)
          s = r;
        // Add past input-values
        for (i = 0; i <= FZeros.GetUpperBound(0); i++)
          r = r + FZeros[i] * FFilterStateZ[i];
        // Anticipate = include current input sample in output value
        if (Anticipate)
          s = r;
        // Do backpolation (FilterStateP[1] = Last out-sample)
        s = BackPolate * FFilterStateP[1] + (1.0 - BackPolate) * s;
        // Scale result 
        // TODO: Check if removing extra checks was ok
        samplesOut[outIdx] = s;
        //TDynDoubleArray(SamplesOut)[OutIdx]:=Max(-MaxDouble,Min(s,MaxDouble));
        // Update filter state
        for (i = FPoles.GetUpperBound(0); i >= 2; i--)
          FFilterStateP[i] = FFilterStateP[i - 1];
        FFilterStateP[1] = r;
        for (i = FZeros.GetUpperBound(0); i >= 1; i--)
          FFilterStateZ[i] = FFilterStateZ[i - 1];
        // Next sample
        idx += step;
        outIdx += step;
      }
    }

    public override void Reset()
    {
      base.Reset();
      if (Setting.NewSettings)
        CalculateIIRCoeff();
      if (FFilterStateZ != null)
        Array.Clear(FFilterStateZ, 0, FFilterStateZ.Length);
      if (FFilterStateP != null)
        Array.Clear(FFilterStateP, 0, FFilterStateP.Length);
    }

    public override void Reset(bool useNextSample)
    {
      base.Reset(useNextSample);
      if (!useNextSample)
        Reset();
    }

    public override void Reset(double xn)
    {
      int i;
      if (Setting.NewSettings)
        CalculateIIRCoeff();
      double r = 0;
      for (i = 0; i <= FZeros.GetUpperBound(0); i++)
        r = r + FZeros[i];
      double s = 1;
      for (i = 1; i <= FPoles.GetUpperBound(0); i++)
        s = s - FPoles[i];
      double y = xn * r / s;
      for (i = 0; i <= FFilterStateZ.GetUpperBound(0); i--)
        FFilterStateZ[i] = xn;
      for (i = 0; i <= FFilterStateZ.GetUpperBound(0); i--)
        FFilterStateP[i] = y;
    }

    #endregion Public Methods
  }
}

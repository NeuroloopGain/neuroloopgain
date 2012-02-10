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

namespace NeuroLoopGainLibrary.Filters
{
  public class DUEFilter : IIRFilterBase
  {
    #region Protected Methods

    protected override void CalculateIIRCoeff()
    {
      base.CalculateIIRCoeff();
      FZeros = new double[2];           // 1 Zero
      FFilterStateZ = new double[2];
      FPoles = new double[1];           // 0 Poles, Idx 0 not used
      FFilterStateP = new double[2];    // NrPoles+1 !!!!!
      // Settings: 1=SampleFreq, 2=Gain, 3=FilterFreq
      double ts = 1.0 / Setting[1].Value;
      double fprewarp = Math.Tan(Math.PI * Setting[3].Value * ts) / (Math.PI * ts);
      double r = 1.0 / (2.0 * Math.PI * fprewarp);
      double s = ts / 2.0;
      FZeros[0] = Gain * (s + r);
      FZeros[1] = Gain * (s - r);
      FPoles[0] = 1.0;
    }

    protected override int DoCheckSettings()
    {
      int result = base.DoCheckSettings();
      if (result == 0)
      {
        if ((Setting[3].Value <= 0) || (Setting[3].Value > Setting[1].Value / 2))
          result = 3;
      }
      return result;
    }

    #endregion Protected Methods

    #region Constructors

    public DUEFilter()
    {
      Setting.SetCount(3);
      Setting[0].Info = "Inverse low-pass & integrator";
      Setting[3].Info = "-3dB frequency";
    }

    #endregion Constructors
  }
}

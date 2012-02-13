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
using NeuroLoopGainLibrary.Mathematics;

namespace NeuroLoopGainLibrary.Filters
{
  public class SEFilter : IIRFilterBase
  {
    #region Protected Methods

    protected override void CalculateIIRCoeff()
    {
      base.CalculateIIRCoeff();
      Array.Resize(ref FZeros, 3);                                              // 3 Zeros
      Array.Resize(ref FFilterStateZ, 3);
      Array.Resize(ref FPoles, 3);                              // 2 Poles, Idx 0 not used
      Array.Resize(ref FFilterStateP, 4);                               // NrPoles+1 !!!!!
      // Settings: 1=SampleFreq, 2=Gain, 3=Cut-off, 4=Center-freq 5=Bandwidth
      double ts = 1.0 / Setting[1].Value;
      double fprewarp = Math.Tan(Setting[4].Value * Math.PI * ts) / (Math.PI * ts);
      double r = MathEx.Sqr(2.0 * Math.PI * fprewarp * ts);
      // From November 1992 prewarping applied because of Arends results !
      // r:=sqr(2.0*pi*f0*Ts);                         No prewarping       
      double s = 2.0 * Math.PI * Setting[5].Value * ts * 2.0;
      double t = 4.0 + r + s;
      FPoles[0] = 1.0;
      FPoles[1] = (8.0 - 2.0 * r) / t;
      FPoles[2] = (-4.0 + s - r) / t;
      fprewarp = Math.Tan(Setting[3].Value * Math.PI * ts) / (Math.PI * ts);
      r = 2.0 / (2.0 * Math.PI * fprewarp);
      // From November 1992 prewarping applied because of Arends results !
      // r:=2.0/(2.0*pi*fc);
      s = Gain * 2.0 * Math.PI * Setting[5].Value * 2.0;
      FZeros[0] = s * (r + ts) / t;
      FZeros[1] = s * (-2.0 * r) / t;
      FZeros[2] = s * (r - ts) / t;
    }

    protected override int DoCheckSettings()
    {
      int result = base.DoCheckSettings();
      if (result == 0)
      {
        if ((Setting[3].Value <= 0) || (Setting[3].Value > Setting[1].Value / 2))
          result = 3;
        else
          if ((Setting[4].Value <= 0) || (Setting[4].Value > Setting[1].Value / 2))
            result = 4;
          else
            if ((Setting[5].Value <= 0) || (Setting[5].Value > Setting[1].Value / 2))
              result = 5;
      }
      return result;
    }

    #endregion Protected Methods

    #region Constructors

    public SEFilter()
    {
      Setting.SetCount(5);
      Setting[3].Info = "Cut-off frequency";
      Setting[4].Info = "Center frequency";
      Setting[5].Info = "Bandwidth";
    }

    #endregion Constructors
  }
}

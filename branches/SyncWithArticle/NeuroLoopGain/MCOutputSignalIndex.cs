//
// NeuroLoopGain Analysis
// Application implementing NeuroLoopGain analysis, based on the mathematical derivations and specifications in the article 
// "Analysis of a sleep-dependent neuronal feedback loop: the slow-wave microcontinuity of the EEG" 
// by B Kemp, AH Zwinderman, B Tuk, HAC Kamphuisen and JJL Oberyé in IEEE-BME 47(9), 2000: 1185-1194.
//
// Copyright 2012 Bob Kemp and Marco Roessen
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

namespace NeuroLoopGain
{
  public static class MCOutputSignalIndex
  {
    #region Private Methods

    static MCOutputSignalIndex()
    {
      SU = 0;
      SS = 1;
      SUplus = 2;
      SUminus = 3;
      SSplus = 4;
      SSminus = 5;
      SSP = 6;
      SS0 = 7;
      HFart = 8;
      LFart = 9;
      MissingSignal = 10;
      MC = 11;
      MCjump = 12;
      MCevent = 13;
    }

    #endregion Private Methods

    #region Public Properties

    public static int HFart { get; private set; }

    public static int LFart { get; private set; }

    public static int MC { get; private set; }

    public static int MCevent { get; private set; }

    public static int MCjump { get; private set; }

    public static int MissingSignal { get; private set; }

    public static int SS { get; private set; }

    public static int SS0 { get; private set; }

    public static int SSminus { get; private set; }

    public static int SSP { get; private set; }

    public static int SSplus { get; private set; }

    public static int SU { get; private set; }

    public static int SUminus { get; private set; }

    public static int SUplus { get; private set; }

    #endregion Public Properties
  }
}
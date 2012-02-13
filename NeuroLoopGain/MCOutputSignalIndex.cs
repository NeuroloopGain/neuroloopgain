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

  // todo: Marco, remove copy input signal support

  public static class MCOutputSignalIndex
  {
    #region private fields

    private static bool _copyInputSignal;

    #endregion private fields

    #region Private Methods

    static MCOutputSignalIndex()
    {
      CopyInputSignal = false;
    }

    #endregion Private Methods

    #region public properties

    public static bool CopyInputSignal
    {
      get
      {
        return _copyInputSignal;
      }
      set
      {
        _copyInputSignal = value;
        Input = value ? 0 : -1;
        SU = value ? 1 : 0;
        SS = value ? 2 : 1;
        SUplus = value ? 3 : 2;
        SUminus = value ? 4 : 3;
        SSplus = value ? 5 : 4;
        SSminus = value ? 6 : 5;
        SSP = value ? 7 : 6;
        SS0 = value ? 8 : 7;
        HFart = value ? 9 : 8;
        LFart = value ? 10 : 9;
        MissingSignal = value ? 11 : 10;
        MC = value ? 12 : 11;
        MCjump = value ? 13 : 12;
        MCevent = value ? 14 : 13;
      }
    }

    public static int HFart { get; private set; }

    public static int Input { get; private set; }

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

    #endregion public properties
  }
}
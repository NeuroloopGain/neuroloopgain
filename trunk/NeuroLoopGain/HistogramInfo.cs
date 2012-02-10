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
  /// <summary>
  /// Class to store the information that should be displayed by the histogram form
  /// </summary>
    public class HistogramInfo
    {
        #region properties
        
        public string FileName { get; set; }
        public string SignalLabel { get; set; }
        public double F0 { get; set; }
        public double FC { get; set; }
        public double B { get; set; }
        public double SmoothRate { get; set; }
        public double SmoothTime { get; set; }
        public short [] SU_SS {get; set;}
        public double[] SU_SSsmoothed { get; set; }
        public double[] SU_SSmatch { get; set; }
        public short PiBvalueLog {get; set;}
        public int UnderSampling { get; set; }
        public double PiBvaluePhysi { get; set; }
        public short SS_SUmin {get; set;}
        public short SS_SUmax { get; set; }
       
        #endregion properties
    }
}

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

namespace NeuroLoopGainLibrary.Filters
{
  /// <summary>
  /// Enumeration with available filter types.
  /// </summary>
  public enum FilterType
  {
    LP,
    HP,
    NOTCH,
    RECT,
    SingleSideRect,
    BP,
    DUE,
    SE,
    G,
    InvG,
    BP12,
    DCatt,
    InvDCatt,
    DUE1987,
    DUE2007,
    LP05,
    LP05NS,
    HP05NS,
    Median,
    Square,
    Squareroot,
    Mean,
    Percentillian,
    dB,
    GainDivOneMinusX,
    Despeckler
  }
}

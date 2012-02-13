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

namespace NeuroLoopGainLibrary.Edf
{
  /// <summary>
  /// Enumeration with known EDF+ signal types
  /// </summary>
  public enum EdfPlusSignalType
  {
    /// <summary>
    /// Length or distance
    /// </summary>
    Distance = 0,

    /// <summary>
    /// Area
    /// </summary>
    Area,

    /// <summary>
    /// Volume
    /// </summary>
    Volume,

    /// <summary>
    /// Duration
    /// </summary>
    Duration,

    /// <summary>
    /// Velocity
    /// </summary>
    Velocity,

    /// <summary>
    /// Mass
    /// </summary>
    Mass,

    /// <summary>
    /// Angle
    /// </summary>
    Angle,

    /// <summary>
    /// Percentage
    /// </summary>
    Percentage,

    /// <summary>
    /// Value (money)
    /// </summary>
    Value,

    /// <summary>
    /// Electroencephalogram
    /// </summary>
    EEG,

    /// <summary>
    /// Electrocardiogram
    /// </summary>
    ECG,

    /// <summary>
    /// Electroöculogram
    /// </summary>
    EOG,

    /// <summary>
    /// Electroretinogram
    /// </summary>
    ERG,

    /// <summary>
    /// Electromyogram
    /// </summary>
    EMG,

    /// <summary>
    /// Magneto encephalogram
    /// </summary>
    MEG,

    /// <summary>
    /// Magneto cardiogram
    /// </summary>
    MCG,

    /// <summary>
    /// Evoked Potential
    /// </summary>
    EP,

    /// <summary>
    /// Temperature
    /// </summary>
    Temp,

    /// <summary>
    /// Respiration
    /// </summary>
    Respiration,

    /// <summary>
    /// Oxygen saturation
    /// </summary>
    Oxygen,

    /// <summary>
    /// Light
    /// </summary>
    Light,

    /// <summary>
    /// Sound
    /// </summary>
    Sound,

    /// <summary>
    /// Events
    /// </summary>
    Events,

    /// <summary>
    /// Unkown
    /// </summary>
    Unknown
  }
}

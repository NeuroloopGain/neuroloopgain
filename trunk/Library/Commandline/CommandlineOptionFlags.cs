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

namespace NeuroLoopGainLibrary.Commandline
{
  /// <summary>
  /// Flags to specify additional option properties
  /// </summary>
  [Flags]
  public enum CommandlineOptionFlags
  {
    /// <summary>
    /// No additional properties
    /// </summary>
    None = 0x0,

    /// <summary>
    /// This option requires a parameter
    /// </summary>
    HasParameter = 0x1,

    /// <summary>
    /// This option is required
    /// </summary>
    Required = 0x2,

    /// <summary>
    /// This option is not shown when showing the usage
    /// </summary>
    HideInUsage = 0x4
  }
}
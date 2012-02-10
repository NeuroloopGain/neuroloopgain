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

namespace Commandline
{
  /// <summary>
  /// Enumeration extension methods
  /// </summary>
  public static class EnumExtension
  {
    #region public methods

    /// <summary>
    /// Determines whether the enumeration has all of the specified flags.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="matchTo">The flags to match.</param>
    /// <returns>
    ///   <c>true</c> if the enumeration has all specified flags; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasAll(this Enum input, Enum matchTo)
    {
      UInt32 allFlags = Convert.ToUInt32(matchTo);
      return (Convert.ToUInt32(input) & allFlags) == allFlags;
    }

    /// <summary>
    /// Determines whether the enumeration has any of the specified flags.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="matchTo">The flags to match.</param>
    /// <returns>
    ///   <c>true</c> if the enumeration has any of the specified flags; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasAny(this Enum input, Enum matchTo)
    {
      return (Convert.ToUInt32(input) & Convert.ToUInt32(matchTo)) != 0;
    }

    #endregion public methods
  }
}

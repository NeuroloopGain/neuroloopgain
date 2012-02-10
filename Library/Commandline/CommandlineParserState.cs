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

namespace NeuroLoopGainLibrary.Commandline
{
  /// <summary>
  /// Enum to identify the commandline parser state
  /// </summary>
  public enum CommandlineParserState
  {
    /// <summary>
    /// The parser is searching for a command start
    /// </summary>
    SearchCommandStart,

    /// <summary>
    /// Command start found; getting the command
    /// </summary>
    GetCommand,

    /// <summary>
    /// Command requiring a parameter found; searching for a parameter start
    /// </summary>
    SearchParameterStart,

    /// <summary>
    /// Parameter start found; getting the parameter
    /// </summary>
    GetParameter
  }
}
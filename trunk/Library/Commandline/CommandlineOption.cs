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
  public class CommandlineOption : IEquatable<CommandlineOption>
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandlineOption"/> class.
    /// </summary>
    public CommandlineOption()
    {
      RequiredPosition = int.MaxValue;
    }

    #endregion Constructors

    #region public properties

    /// <summary>
    /// Gets or sets the action to be executed when this option is encountered.
    /// </summary>
    /// <value>
    /// The action.
    /// </value>
    public Action<BaseCommandlineParser, string> Action { get; set; }

    /// <summary>
    /// Gets or sets the description of this option.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    /// <value>
    /// The flags.
    /// </value>
    public CommandlineOptionFlags Flags { get; set; }

    /// <summary>
    /// Gets or sets the long name of this option.
    /// </summary>
    /// <value>
    /// The long name.
    /// </value>
    public string LongName { get; set; }

    /// <summary>
    /// Gets or sets the (short) name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    /// <value>
    /// The name of the parameter.
    /// </value>
    public string ParameterName { get; set; }

    /// <summary>
    /// Gets or sets the required position.
    /// </summary>
    /// <value>
    /// The required position. If equal to int.MaxValue (the default value), this option has no required position.
    /// </value>
    public int RequiredPosition { get; set; }

    #endregion public properties



    #region IEquatable<CommandlineArgument> Members

    /// <summary>
    /// Checks if this instance properties equals the specified other option.
    /// </summary>
    /// <param name="other">The other option.</param>
    /// <returns></returns>
    public bool Equals(CommandlineOption other)
    {
      return (Name.Equals(other.Name) && LongName.Equals(other.LongName) && Description.Equals(other.Description) &&
              Flags == other.Flags && ParameterName.Equals(other.ParameterName) && RequiredPosition.Equals(other.RequiredPosition));
    }

    #endregion
  }
}
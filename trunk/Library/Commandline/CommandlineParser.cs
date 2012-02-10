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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commandline;

namespace NeuroLoopGainLibrary.Commandline
{
  public class CommandlineParser : BaseCommandlineParser
  {
    #region Private Methods

    /// <summary>
    /// Initializes the private variables.
    /// </summary>
    private void InitializePrivateVariables()
    {
      OptionPrefixes = new[] { '/', '-' };
      ArgumentValueSeperator = new[] { ':', '=' };
    }

    #endregion Private Methods

    #region protected properties

    /// <summary>
    /// Gets or sets the characters that seperate the argument and the value.
    /// </summary>
    /// <value>
    /// The argument value seperator.
    /// </value>
    protected char[] ArgumentValueSeperator { get; set; }

    #endregion protected properties

    #region protected methods

    protected override void AppendOptionsToUsage(StringBuilder sb, IEnumerable<CommandlineOption> arguments)
    {
      foreach (var opt in arguments)
      {
        sb.Append(GetOptionShortName(opt));

        if (opt.Flags.HasAny(CommandlineOptionFlags.HasParameter))
        {
          sb.Append(ArgumentValueSeperator[0]);
          sb.Append(!String.IsNullOrEmpty(opt.ParameterName) ? opt.ParameterName : "arg");
        }

        sb.Append(' ');
      }

      if (sb.Length > 0)
        sb.Remove(sb.Length - 1, 1);
    }

    protected override void DoParse(string[] arguments)
    {
      SortOptionsByRequiredPostion();

      // First process all options that have required positions
      foreach (CommandlineOption currentOption in Options.Where(currentCommand => currentCommand.RequiredPosition != int.MaxValue && currentCommand.RequiredPosition < arguments.Length))
      {
        if (DebugParser)
          ParserLog.AppendLine(string.Format("Dispatching {0}; value {1}", currentOption.Name,
                                  arguments[currentOption.RequiredPosition]));
        string arg = arguments[currentOption.RequiredPosition];

        if (string.IsNullOrEmpty(arg) || IsOptionStart(arg, 0))
          continue;

        // Dispatch option and mark argument handled (null)
        DispatchedOptions.Add(currentOption);
        DispatchOption(currentOption, arguments[currentOption.RequiredPosition]);
        arguments[currentOption.RequiredPosition] = null;
      }

      // Process the all other (not null) arguments
      foreach (string currentArgument in arguments.Where(currentArgument => !string.IsNullOrEmpty(currentArgument)))
      {
        // Option should start with an OptionPrefixes character
        if (!IsOptionStart(currentArgument, 0))
        {
          if (DebugParser)
            ParserLog.AppendLine(string.Format("{0}: No option start found", currentArgument));
          UnknownArguments.Add(currentArgument);
          continue;
        }

        // Split current argument in a command and a parameter string
        string parameter = string.Empty;
        string command = currentArgument.Substring(1);

        int index = command.IndexOfAny(ArgumentValueSeperator);
        if (index > 0)
        {
          parameter = command.Substring(index + 1);
          command = command.Substring(0, index);
        }

        if (DebugParser)
          ParserLog.AppendLine(string.Format("Option: {0}, Parameter: {1}", command, parameter));

        // Find the option handling the current command string
        CommandlineOption currentOption = GetArgument(command, true, StringComparison.InvariantCultureIgnoreCase) ??
                                          GetArgument(command, false, StringComparison.InvariantCultureIgnoreCase);

        if (currentOption != null)
        {
          // If option found check if the parameter is required
          if (currentOption.Flags.HasAny(CommandlineOptionFlags.HasParameter) && string.IsNullOrEmpty(parameter))
          {
            if (DebugParser)
              ParserLog.AppendLine(string.Format("{0}: Missing required parameter.", currentOption.Name));
            UnknownArguments.Add(string.Format("{0}: Missing required parameter.", command));
          }
          else
          {
            // Dispatch the option
            if (DebugParser)
              ParserLog.AppendLine(string.Format("Dispatching {0}; value {1}", currentOption.Name, parameter));
            DispatchedOptions.Add(currentOption);
            DispatchOption(currentOption, parameter);
          }
        }
        else
        {
          // Invalid option
          if (DebugParser)
            ParserLog.AppendLine(string.Format("Invalid argument: {0}", command));
          UnknownArguments.Add(command);
        }
      }
    }

    protected override string GetOptionLongName(CommandlineOption commandLineArgument)
    {
      return commandLineArgument.RequiredPosition == int.MaxValue
               ? OptionPrefixes[0] + commandLineArgument.LongName
               : commandLineArgument.LongName;
    }

    protected void SortOptionsByRequiredPostion()
    {
      Options = Options.OrderBy(f => f.RequiredPosition).ToList();

      int index = 0;
      while (index < Options.Count && Options[index].RequiredPosition != int.MaxValue)
      {
        if (Options[index].RequiredPosition != index)
          throw new ArgumentException("Required positions should be consecutive and start at index 0.");
        index++;
      }
    }

    #endregion protected methods

    #region Constructors

    public CommandlineParser(string appDescription)
      : base(appDescription)
    {
      InitializePrivateVariables();
    }

    public CommandlineParser()
    {
      InitializePrivateVariables();
    }

    #endregion Constructors

    #region public methods

    public override string GetHelp()
    {
      SortOptionsByRequiredPostion();
      return base.GetHelp();
    }

    #endregion public methods
  }
}
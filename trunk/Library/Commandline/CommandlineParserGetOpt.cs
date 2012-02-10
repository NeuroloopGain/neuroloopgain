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
using System.Text;
using Commandline;

namespace NeuroLoopGainLibrary.Commandline
{
  public class CommandlineParserGetOpt : BaseCommandlineParser
  {
    #region protected methods

    protected override void DoParse(string[] arguments)
    {
      StringBuilder sb = new StringBuilder();
      foreach (string t in arguments)
      {
        sb.Append(" ");
        if (t.Contains(" ") && !(t.StartsWith("\"") && t.EndsWith("\"")))
          sb.AppendFormat("\"{0}\"", t);
        else
          sb.Append(t);
      }

      string allArguments = sb.ToString().Trim();

      CommandlineParserState mode = CommandlineParserState.SearchCommandStart;
      int characterIndex = 0;
      int oldCharacterIndex = 0;
      string currentArgument = string.Empty;
      int paramSearchStartIndex = 0;
      bool longArg = false;
      CommandlineOption currentCommand = null;

      while (characterIndex < allArguments.Length)
      {
        switch (mode)
        {
          case CommandlineParserState.SearchCommandStart:
            if (IsOptionStart(allArguments, characterIndex))
            {
              longArg = (IsLongOption(allArguments, characterIndex));
              currentArgument = string.Empty;
              mode = CommandlineParserState.GetCommand;
            }
            else
              currentArgument += allArguments[characterIndex];
            characterIndex++;
            if (longArg)
              characterIndex++;
            break;
          case CommandlineParserState.GetCommand:
            if (char.IsWhiteSpace(allArguments[characterIndex]))
              mode = CommandlineParserState.SearchCommandStart;
            currentArgument += allArguments[characterIndex];
            currentCommand = GetArgument(currentArgument, longArg);
            if (currentCommand != null)
            {
              if (DebugParser)
                ParserLog.AppendLine(string.Format("Command: {0}", currentArgument));

              currentArgument = string.Empty;

              if (currentCommand.Flags.HasAll(CommandlineOptionFlags.HasParameter))
              {
                oldCharacterIndex = characterIndex + 1;
                mode = CommandlineParserState.SearchParameterStart;
              }
              else
              {
                DispatchedOptions.Add(currentCommand);
                DispatchOption(currentCommand, string.Empty);
                currentCommand = null;
              }
            }
            characterIndex++;
            break;
          case CommandlineParserState.SearchParameterStart:
            if (longArg)
            {
              if (char.IsWhiteSpace(allArguments[characterIndex]))
                characterIndex++;
              else
              {
                mode = CommandlineParserState.GetParameter;
                paramSearchStartIndex = characterIndex;
              }
            }
            else
            {
              if (!char.IsWhiteSpace(allArguments[characterIndex]))
              {
                currentArgument += allArguments[characterIndex];
                characterIndex++;
                if (string.IsNullOrEmpty(currentArgument))
                  mode = CommandlineParserState.SearchParameterStart;
                if (GetArgument(currentArgument, false) != null)
                  currentArgument = string.Empty;
              }
              else
              {
                while (characterIndex < allArguments.Length && char.IsWhiteSpace(allArguments[characterIndex]))
                  characterIndex++;
                if (characterIndex < allArguments.Length)
                {
                  mode = CommandlineParserState.GetParameter;
                  paramSearchStartIndex = characterIndex;
                }
                else
                  mode = CommandlineParserState.SearchCommandStart;
              }
            }
            break;
          case CommandlineParserState.GetParameter:
            if (!char.IsWhiteSpace(allArguments[characterIndex]) ||
                currentArgument.StartsWith("\"", StringComparison.CurrentCulture))
            {
              currentArgument += allArguments[characterIndex];
              characterIndex++;
              if (characterIndex < allArguments.Length ||
                  (currentArgument.StartsWith("\"", StringComparison.CurrentCulture) &&
                   !currentArgument.EndsWith("\"", StringComparison.CurrentCulture)))
                continue;
            }

            if (DebugParser)
              ParserLog.AppendLine(string.Format("Parameter: {0}", currentArgument));

            string param = currentArgument.StartsWith("\"") && currentArgument.EndsWith("\"")
                             ? currentArgument.Substring(1, currentArgument.Length - 2)
                             : currentArgument;

            DispatchedOptions.Add(currentCommand);
            DispatchOption(currentCommand, param);
            currentCommand = null;

            allArguments = allArguments.Remove(paramSearchStartIndex, currentArgument.Length).TrimEnd();
            currentArgument = string.Empty;
            characterIndex = oldCharacterIndex;
            mode = CommandlineParserState.GetCommand;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      if (currentArgument != string.Empty)
        switch (mode)
        {
          case CommandlineParserState.SearchCommandStart:
          case CommandlineParserState.GetCommand:
            if (DebugParser)
              ParserLog.AppendLine(string.Format("Unkown Command: {0}", currentArgument));
            UnknownArguments.Add(currentArgument);
            break;
          case CommandlineParserState.GetParameter:
            if (DebugParser)
              ParserLog.AppendLine(string.Format("Parameter (on exit): {0}", currentArgument));
            DispatchedOptions.Add(currentCommand);
            DispatchOption(currentCommand, currentArgument);
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
    }

    #endregion protected methods

    #region Constructors

    public CommandlineParserGetOpt(string appDescription)
      : base(appDescription)
    {
    }

    public CommandlineParserGetOpt()
    {
    }

    #endregion Constructors
  }
}
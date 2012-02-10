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
  /// <summary>
  /// This parser emulates the behavior of the POSIX getopt C runtime function for parsing command line arguments.  
  /// This mechanism is fairly easy to use as it is quite flexible in how it lets you submit arguments for parsing.
  /// e.g. all of these would be valid and equivalent command line arguments if you had options x, y, and z 
  /// where z takes an argument.
  /// 
  /// -x -y -zparameter
  /// -x -y -z parameter
  /// -xyzparameter
  /// -x -yzparameter
  /// -x -yz "parameter"
  /// -x -yz"parameter"
  /// 
  /// This parser dispatches the commands automatically using the C#'s Action convention. 
  /// </summary>
  public abstract class BaseCommandlineParser
  {
    #region Private Methods

    /// <summary>
    /// Determines which required options are missing.
    /// </summary>
    /// <param name="dispatchedCommands">The dispatched commands.</param>
    /// <returns>A list containing all missing arguments</returns>
    private IList<string> DetermineMissingRequiredOptions(ICollection<CommandlineOption> dispatchedCommands)
    {
      IList<string> missing = new List<string>();

      // Check which arguments are required
      List<CommandlineOption> required = Options.Where(a => a.Flags.HasAll(CommandlineOptionFlags.Required)).ToList();

      if (required.Count > 0)
      {
        // if we actually have some required arguments, then some might not have been dispatched, which means they're missing.
        foreach (var requiredCommand in required.Where(requiredCommand => !dispatchedCommands.Contains(requiredCommand)))
          missing.Add(string.IsNullOrEmpty(requiredCommand.LongName) ? requiredCommand.Name : requiredCommand.LongName);
      }

      return missing;
    }

    /// <summary>
    /// Gets the usage string.
    /// </summary>
    /// <param name="appName">Name of the application.</param>
    /// <returns>A full help text that can be used to show the user what commandline parameters are available.</returns>
    private string GetUsageString(string appName)
    {
      var sb = new StringBuilder();

      // Usage start
      sb.Append("Usage: ");
      sb.Append(appName);
      sb.Append(' ');

      // Append required arguments to usage string
      List<CommandlineOption> required =
        Options.Where(
          a =>
          a.Flags.HasAny(CommandlineOptionFlags.Required) &&
          !a.Flags.HasAny(CommandlineOptionFlags.HideInUsage)).ToList();
      if (required.Count > 0)
        AppendOptionsToUsage(sb, required);

      // Append optional arguments to usage string
      List<CommandlineOption> optional =
        Options.Where(a => !a.Flags.HasAny(CommandlineOptionFlags.Required | CommandlineOptionFlags.HideInUsage)).ToList();

      if (optional.Count > 0)
      {
        sb.Append(" [");
        AppendOptionsToUsage(sb, optional);
        sb.Append("]");
      }

      return sb.ToString();
    }

    private void InitializePrivateVariables()
    {
      // Initialize private variables
      ParserLog = new StringBuilder();
      UnknownArguments = new List<string>();
      MissingRequiredOptions = new List<string>();
      DispatchedOptions = new List<CommandlineOption>();
      Options = new List<CommandlineOption>();
      OptionPrefixes = new[] { '-', '/' };
    }

    /// <summary>
    /// Appends a string to the specified stringbuilder instance.
    /// </summary>
    /// <param name="sb">The stringbuilder.</param>
    /// <param name="s">The string to be added.</param>
    private static void WriteLine(StringBuilder sb, string s)
    {
      sb.Append(s);
      sb.Append(Environment.NewLine);
    }

    #endregion Private Methods

    #region protected properties

    /// <summary>
    /// Gets the list of available options.
    /// </summary>
    /// <value>
    /// The options.
    /// </value>
    protected IList<CommandlineOption> Options { get; set; }

    #endregion protected properties

    #region protected methods

    /// <summary>
    /// Appends the available options to the usage string.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="options">The available options.</param>
    protected virtual void AppendOptionsToUsage(StringBuilder sb, IEnumerable<CommandlineOption> options)
    {
      foreach (var opt in options)
      {
        sb.Append(GetOptionShortName(opt));

        if (opt.Flags.HasAny(CommandlineOptionFlags.HasParameter))
        {
          if (!String.IsNullOrEmpty(opt.ParameterName))
          {
            sb.Append(" <");
            sb.Append(opt.ParameterName);
            sb.Append(">");
          }
          else
            sb.Append(" <arg>");
        }

        sb.Append(' ');
      }

      if (sb.Length > 0)
        sb.Remove(sb.Length - 1, 1);
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "BaseCommandlineParser" /> class.
    /// </summary>
    protected BaseCommandlineParser()
    {
      InitializePrivateVariables();
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "BaseCommandlineParser" /> class.
    /// </summary>
    /// <param name = "appDescription">The description of the application.</param>
    protected BaseCommandlineParser(string appDescription)
    {
      InitializePrivateVariables();
      ApplicationDescription = appDescription;
    }

    /// <summary>
    /// Dispatches the command.
    /// </summary>
    /// <param name="option">The commmandline argument.</param>
    /// <param name="parameter">The paramater.</param>
    /// <returns>null</returns>
    protected void DispatchOption(CommandlineOption option, string parameter)
    {
      option.Action(this, parameter.Trim());
    }

    /// <summary>
    /// Method for adding a new commandline argument with specified parameters.
    /// </summary>
    /// <param name="name">The short name.</param>
    /// <param name="longName">The long name.</param>
    /// <param name="description">The description.</param>
    /// <param name="paramName">Name of the param.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="action">The action.</param>
    /// <param name="requiredPosition">Required position; -1 if not required at a specific position</param>
    protected virtual void DoAddOption(string name, string longName, string description, string paramName, CommandlineOptionFlags flags,
                            Action<BaseCommandlineParser, string> action, int requiredPosition = int.MaxValue)
    {
      // Check if the parameters are valid

      if (!ValidateOptionName(name))
        throw new ArgumentException("Invalid value.", "name");

      if (!string.IsNullOrEmpty(longName) && !ValidateOptionName(longName))
        throw new ArgumentException("Invalid value.", "longName");

      if (requiredPosition < 0)
        throw new ArgumentOutOfRangeException("requiredPosition", "Value should be >= 0");

      if (requiredPosition != int.MaxValue)
      {
        if (Options.FirstOrDefault(a => a.RequiredPosition == requiredPosition) != null)
          throw new ArgumentException("Value alreay exists.", "requiredPosition");

        if (flags.HasAny(CommandlineOptionFlags.HasParameter))
          throw new ArgumentException("Required position option cannot have a parameter.", "paramName");
      }

      // Add a new option to the available options list
      Options.Add(new CommandlineOption
                      {
                        Name = name,
                        LongName = longName,
                        Description = description,
                        ParameterName = paramName,
                        Flags = flags,
                        Action = action,
                        RequiredPosition = requiredPosition
                      });
    }

    /// <summary>
    /// Performs the actual parsing of the commandline arguments.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    protected abstract void DoParse(string[] arguments);

    /// <summary>
    /// Does the validation of the option (long) name.
    /// </summary>
    /// <param name="optionName">Name of the option.</param>
    /// <returns></returns>
    protected virtual bool DoValidateOptionName(string optionName)
    {
      return !String.IsNullOrEmpty(optionName) && !OptionPrefixes.Any(optionName.Contains) &&
             !optionName.Contains(" ");
    }

    /// <summary>
    /// Gets first available commandline option handling the specified command.
    /// </summary>
    /// <param name="option">The argument buffer.</param>
    /// <param name="useLongName">if set to <c>true</c> use the long argument name.</param>
    /// <param name="stringComparisonType">Name comparison type; default invariant culture and case sensitive.</param>
    /// <returns></returns>
    protected CommandlineOption GetArgument(string option, bool useLongName, StringComparison stringComparisonType = StringComparison.InvariantCulture)
    {
      return !useLongName ?
        Options.Where(a => a.Name.Equals(option, stringComparisonType)).FirstOrDefault() :
        Options.Where(a => a.LongName.Equals(option, stringComparisonType)).FirstOrDefault();
    }

    /// <summary>
    /// Gets the long name of the command; used to display the help message.
    /// </summary>
    /// <param name="option">The option to get the long name for.</param>
    /// <returns></returns>
    protected virtual string GetOptionLongName(CommandlineOption option)
    {
      if (option.RequiredPosition == int.MaxValue)
      {
        var sb = new StringBuilder();

        sb.Append(OptionPrefixes[0]);
        sb.Append(OptionPrefixes[0]);
        sb.Append(option.LongName);

        return sb.ToString();
      }

      return option.LongName;
    }

    /// <summary>
    /// Gets the commandline option short and long name.
    /// </summary>
    /// <param name="option">The commandline option to get the short and long names from.</param>
    /// <returns></returns>
    protected virtual string GetOptionNames(CommandlineOption option)
    {
      return !string.IsNullOrEmpty(option.LongName)
               ? GetOptionShortName(option) + ", " + GetOptionLongName(option)
               : GetOptionShortName(option);
    }

    /// <summary>
    /// Gets the short name of the option; used to display the help message.
    /// </summary>
    /// <returns>The string prepended with the first ArgumentPrefixList item.</returns>
    protected virtual string GetOptionShortName(CommandlineOption option)
    {
      return option.RequiredPosition == int.MaxValue
               ? OptionPrefixes[0] + option.Name
               : option.Name;
    }

    /// <summary>
    /// Determines whether the string at the index is the start of an long option.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="index">The index.</param>
    /// <returns>
    ///   <c>true</c> if it is a long argument start; otherwise, <c>false</c>.
    /// </returns>
    protected bool IsLongOption(string s, int index)
    {
      return ((index + 1) < s.Length) && OptionPrefixes.Contains(s[index + 1]) && s[index].Equals(s[index + 1]);
    }

    /// <summary>
    /// Determines whether the character at index of specified string is an argument start character.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="index">The index.</param>
    /// <returns>
    ///   <c>true</c> if the character is an argument start character; otherwise, <c>false</c>.
    /// </returns>
    protected bool IsOptionStart(string s, int index)
    {
      return OptionPrefixes.Contains(s[index]);
    }

    /// <summary>
    /// Validates the option name.
    /// </summary>
    /// <param name="optionName">The arg.</param>
    /// <returns></returns>
    protected bool ValidateOptionName(string optionName)
    {
      return DoValidateOptionName(optionName);
    }

    #endregion protected methods

    #region public properties

    /// <summary>
    ///   Gets or sets the application description as shown on the help screen.
    /// </summary>
    /// <value>
    ///   The description of the application.
    /// </value>
    public string ApplicationDescription { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we want to debug the parser.
    /// When se to <c>true</c> the parser will add debug message to the ParserLog
    /// </summary>
    /// <value>
    ///   <c>true</c> if debugging the parser; otherwise, <c>false</c>.
    /// </value>
    public bool DebugParser { get; set; }

    /// <summary>
    /// Gets the list with dispatched options.
    /// </summary>
    public List<CommandlineOption> DispatchedOptions { get; private set; }

    /// <summary>
    ///   Gets the list of missing required options.
    /// </summary>
    public IList<string> MissingRequiredOptions { get; private set; }

    /// <summary>
    ///   Gets or sets the argument prefix list.
    ///   The argument prefix list is used to designate the set of values that are used to denote the start of an argument.  
    ///   Long versions are always assumed to be two instances of the string.  
    ///   Thus, if the short version is specified via "-", the long version would be specified via "--".
    /// </summary>
    /// <value>
    ///   The argument prefix list.
    /// </value>
    public char[] OptionPrefixes { get; set; }

    /// <summary>
    /// Gets the parser log. This log is used when DebugParser is set to <c>true</c>>
    /// </summary>
    /// <value>
    /// The parser log.
    /// </value>
    public StringBuilder ParserLog { get; private set; }

    /// <summary>
    ///   Gets the unknown arguments list. 
    ///   This list is populated during a Parse() operation accumulating the list
    ///   of commands that were supplied that are not understood by the parser.
    /// </summary>
    public IList<string> UnknownArguments { get; private set; }

    #endregion public properties

    #region public methods

    /// <summary>
    /// Method for adding a new commandline argument with specified parameters.
    /// </summary>
    /// <param name="name">The short name.</param>
    /// <param name="longName">The long name.</param>
    /// <param name="description">The description.</param>
    /// <param name="action">The action to perform when found.</param>
    public void AddOption(string name, string longName, string description, Action<BaseCommandlineParser, string> action)
    {
      DoAddOption(name, longName, description, String.Empty, CommandlineOptionFlags.None, action);
    }

    /// <summary>
    /// Method for adding a new commandline argument with specified parameters.
    /// </summary>
    /// <param name="name">The short name.</param>
    /// <param name="longName">The long name.</param>
    /// <param name="description">The description.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="action">The action.</param>
    public void AddOption(string name, string longName, string description, CommandlineOptionFlags flags,
                            Action<BaseCommandlineParser, string> action)
    {
      DoAddOption(name, longName, description, String.Empty, flags, action);
    }

    public void AddOption(string name, string longName, string description, string paramName, CommandlineOptionFlags flags,
                            Action<BaseCommandlineParser, string> action, int requiredPosition = int.MaxValue)
    {
      DoAddOption(name, longName, description, paramName, flags, action, requiredPosition);
    }

    /// <summary>
    /// Gets the help message string.
    /// </summary>
    /// <returns>A string containing the available application commandline arguments.</returns>
    public virtual string GetHelp()
    {
      StringBuilder text = new StringBuilder();

      var appName = AppDomain.CurrentDomain.FriendlyName.ToLower();

      WriteLine(text, String.Empty);

      // Write the application header
      if (!String.IsNullOrEmpty(ApplicationDescription))
        WriteLine(text, appName + " - " + ApplicationDescription);
      else
        WriteLine(text, appName);

      // Write the usage string
      WriteLine(text, String.Empty);
      WriteLine(text, GetUsageString(appName));
      WriteLine(text, String.Empty);

      // Write out the commands
      WriteLine(text, "Available options:");
      WriteLine(text, "-------------------");

      // Get the longest command expression
      int exprLength = Options.Select(c => (GetOptionNames(c).Length)).Max();

      foreach (var command in Options)
        WriteLine(text, GetOptionNames(command).PadRight(exprLength + 5, ' ') + command.Description);

      return text.ToString();
    }

    /// <summary>
    /// Processes the commandline arguments.
    /// </summary>
    /// <param name="arguments">The arguments. If not specified (null) the Environment.GetCommandLineArgs() will be used</param>
    public void Parse(string[] arguments = null)
    {
      // Reset the tracking collections for unknown and missing required commands
      UnknownArguments.Clear();
      MissingRequiredOptions.Clear();
      DispatchedOptions.Clear();
      // Clear the parser log; .NET framework 3.5 and below does not support the Clear() method!
      ParserLog.Length = 0;

      if (arguments == null)
        arguments = Environment.GetCommandLineArgs().Skip(1).ToArray();

      // If debugging add the supplied arguments to the log
      if (DebugParser)
      {
        ParserLog.Append(string.Format("Arguments:"));
        foreach (string t in arguments)
          ParserLog.Append(" " + t);
        ParserLog.AppendLine();
      }

      // Parse the arguments
      DoParse(arguments);

      // After dispatching all available commands, check if there were any required commands that didn't get supplied.
      MissingRequiredOptions = DetermineMissingRequiredOptions(DispatchedOptions);
    }

    #endregion public methods
  }
}

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

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using NeuroLoopGainLibrary.Commandline;
using NeuroLoopGainLibrary.Edf;
using NeuroLoopGainLibrary.Logging;
using System.IO;
using NeuroLoopGainLibrary.Mathematics;
using System.Text.RegularExpressions;

namespace NeuroLoopGain
{
  public partial class MainGUI : Form
  {
    #region private fields

    private MCconfiguration _configuration;
    private EdfFile _edfFile;
    private String _fileName;
    private readonly IFormatProvider _formatProvider = CultureInfo.InvariantCulture;
    private readonly IFormatProvider _guiFormatProvider = CultureInfo.CurrentUICulture;

    #endregion private fields

    #region Event handlers

    /// <summary>
    /// Handles the Click event of the buttonAlpha control.
    /// Sets the analysis parameters for analysing the alpha frequency.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void buttonAlpha_Click(object sender, EventArgs e)
    {
      SetAnalysisParameters(10, 3.5, 1.8, 0.01666);
    }

    /// <summary>
    /// Handles the Click event of the buttonCancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void buttonCancel_Click(object sender, EventArgs e)
    {
      Close();
    }

    /// <summary>
    /// Handles the Click event of the buttonSelectEDF control.
    /// Shows an open file dialog and sets the selected file information.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void buttonSelectEDF_Click(object sender, EventArgs e)
    {
      textBox.Clear();

      OpenFileDialog openFileDialog = new OpenFileDialog
                                        {
                                          Filter = @"EDF+ files (*.edf)|*.edf|EDF/EDF+ files (*.*)|*.*",
                                          FilterIndex = 2,
                                          Title = Strings.CaptionSelectEdfFileToAnalyze
                                        };

      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;
      _fileName = openFileDialog.FileName;
      textBox.Text = "Selected file: " + _fileName;
      textBoxInputFilename.Text = _fileName;
    }

    /// <summary>
    /// Handles the Click event of the buttonSelectOutputEDF control.
    /// Shows an open file dialog and sets the selected file information.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void button1_Click(object sender, EventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog
      {
        Filter = @"EDF+ files (*.edf)|*.edf|EDF/EDF+ files (*.*)|*.*",
        FilterIndex = 2,
        Title = Strings.CaptionSelectOutputEdfFile
      };

      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;
      textBoxOutputFileName.Text = openFileDialog.FileName;
    }

    /// <summary>
    /// Handles the Click event of the buttonSlowWaves control.
    /// Sets the analysis parameters for analysing the slowwave frequency.
    /// /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void buttonSlowWaves_Click(object sender, EventArgs e)
    {
      SetAnalysisParameters(1, 1.5, 1.8, 0.01666);
    }

    /// <summary>
    /// Handles the Click event of the buttonSpindles control.
    /// Sets the analysis parameters for analysing the spindles frequency.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void buttonSpindles_Click(object sender, EventArgs e)
    {
      SetAnalysisParameters(14, 3.5, 1.8, 0.01666);
    }

    /// <summary>
    /// Handles the Click event of the buttonStart control.
    /// Check the user input and runs the analysis.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void buttonStart_Click(object sender, EventArgs e)
    {
      try
      {
      ErrorLogger.WriteLog("Analysis started...");
      textBox.AppendText(Environment.NewLine + "Analysis started...");

      NeuroLoopGainController.AppController.ApplicationError.Clear();

      bool analysisOk = DoCheckInput();

      if (analysisOk)
      {
        DoSetInput();

        analysisOk = NeuroLoopGainController.AppController.Analyze();
      }

      if (analysisOk)
      {
        textBox.AppendText(Environment.NewLine + "Analysis done!");
        ErrorLogger.WriteLog("Analysis done!");
      }
      else
      {
        textBox.AppendText(Environment.NewLine + "Analysis failed!");
        textBox.AppendText(Environment.NewLine + NeuroLoopGainController.AppController.ApplicationError.Message);
        ErrorLogger.WriteLog("Analysis failed!");
        ErrorLogger.WriteLog(NeuroLoopGainController.AppController.ApplicationError.Message);
      }

      }
      catch (Exception ex)
      {
        ErrorLogger.WriteExceptionToLog(ex);
      }
    }

    /// <summary>
    /// Handles the SelectedIndexChanged event of the signalsComboBox control.
    /// Sets the applied filters
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void signalsComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (_edfFile != null)
      {
        double signalSFreq = _edfFile.SignalInfo[signalsComboBox.SelectedIndex].NrSamples / _edfFile.FileInfo.SampleRecDuration;
        // Defining a regular expression for filtering settings detection
        Regex r = new Regex(@"LP:([0-9]+.[0-9]+)Hz", RegexOptions.IgnoreCase);
        Match m = r.Match(_edfFile.SignalInfo[signalsComboBox.SelectedIndex].PreFilter);
        if (m.Success)
          textBoxLP.Text = m.Groups[1].Value;
        else
          textBoxHP.Text = "0";
        r = new Regex(@"HP:([0-9]+.[0-9]+)Hz", RegexOptions.IgnoreCase);
        m = r.Match(_edfFile.SignalInfo[signalsComboBox.SelectedIndex].PreFilter);
        if (m.Success)
          textBoxHP.Text = m.Groups[1].Value;
        else
          textBoxLP.Text = string.Format("{0}", signalSFreq / 2);
      }
      else
      {
        textBoxHP.Text = "---";
        textBoxLP.Text = "---";
      }
    }

    #endregion Event handlers

    #region Private Methods

    /// <summary>
    /// Does the input values check. If there are errors, the offending textbox will be focussed.
    /// </summary>
    /// <returns><c>true</c> if all analysis parameters are ok</returns>
    private bool DoCheckInput()
    {
      double doubleValue;
      double lpcutoff;
      double hpcutoff;
      double f0, fc, bandwith;
      double fcomputeMin = -1;
      double fcomputeMax = -1;

      // check filename and signal index
      if (signalsComboBox.Items.Count < 1)
      {
        textBox.Text = "You have to select a valid EDF inputfile first";
        textBoxInputFilename.Focus();
        NeuroLoopGainController.AppController.ApplicationError.Add("You have to select a valid EDF inputfile first", NeuroLoopGainController.DefaultErrorMessageId);
        return false;
      }

      if (signalsComboBox.SelectedIndex < 0)
      {
        signalsComboBox.Focus();
        NeuroLoopGainController.AppController.ApplicationError.Add("No input signal selected", NeuroLoopGainController.DefaultErrorMessageId);
        return false;
      }

      // check lowest LP and highest HP filter values
      if (!IsValidFloat(textBoxLP, out lpcutoff))
      {
        textBoxLP.Focus();
        NeuroLoopGainController.AppController.ApplicationError.Add("Invalid LP filter value", NeuroLoopGainController.DefaultErrorMessageId);
        return false;
      }

      if (!IsValidFloat(textBoxHP, out hpcutoff))
      {
        textBoxHP.Focus();
        NeuroLoopGainController.AppController.ApplicationError.Add("Invalid HP filter value", NeuroLoopGainController.DefaultErrorMessageId);
        return false;
      }

      if ((lpcutoff <= 0) || (lpcutoff <= hpcutoff))
      {
        textBoxLP.Focus();
        NeuroLoopGainController.AppController.ApplicationError.Add("Low Pass filter cutoff must be 0 <= HP < LP", NeuroLoopGainController.DefaultErrorMessageId);
        return false;
      }

      double signalSFreq = _edfFile.SignalInfo[signalsComboBox.SelectedIndex].NrSamples / _edfFile.FileInfo.SampleRecDuration;
      if (hpcutoff >= signalSFreq / 2)
      {
        textBoxHP.Focus();
        NeuroLoopGainController.AppController.ApplicationError.Add(string.Format("High Pass filter cutoff must be less than Nyquist frequency {0}", signalSFreq / 2), NeuroLoopGainController.DefaultErrorMessageId);
        return false;
      }

      // check F0, B0, Fc and smoother rate values
      if (!IsValidFloat(textBoxF0, out f0))
      {
        textBoxF0.Focus();
        NeuroLoopGainController.AppController.ApplicationError.Add("Invalid F0 parameter", NeuroLoopGainController.DefaultErrorMessageId);
      }

      if (!IsValidFloat(textBoxFc, out fc))
      {
        textBoxFc.Focus();
        NeuroLoopGainController.AppController.ApplicationError.Add("Invalid Fc parameter", NeuroLoopGainController.DefaultErrorMessageId);
      }

      if (!IsValidFloat(textBoxB, out bandwith))
      {
        textBoxB.Focus();
        NeuroLoopGainController.AppController.ApplicationError.Add("Invalid bandwidth parameter", NeuroLoopGainController.DefaultErrorMessageId);
      }

      if (!IsValidFloat(textBoxSmootherrate, out doubleValue))
      {
        textBoxSmootherrate.Focus();
        NeuroLoopGainController.AppController.ApplicationError.Add("Invalid smooth rate parameter", NeuroLoopGainController.DefaultErrorMessageId);
      }

      // Check filter values versus SafetyFactor
      if (!NeuroLoopGainController.AppController.ApplicationError.Signaled)
      {
        // Calculate min and max values for Fcompute
        fcomputeMin = _configuration.SafetyFactor * Math.Max(f0, fc); // Safety factor (default) of 3 actually is an approximation of 1.5 * 2. (1.5 due to the use of non-ideal filters with some bandwidth)
        fcomputeMax = Math.Min(2 * 0.75 * lpcutoff, signalSFreq); // Related with white noise spectra assumption. 0.75 is a safety factor due to the use of non-ideal filters
        if (!(fcomputeMin <= fcomputeMax))
          NeuroLoopGainController.AppController.ApplicationError.Add("No valid filters set-up. " +
             "F0 and/or Fc too large, or LP too small", NeuroLoopGainController.DefaultErrorMessageId);
        // Check HP filter
        if (!(hpcutoff < _configuration.SafetyFactor * f0))
          NeuroLoopGainController.AppController.ApplicationError.Add(string.Format("HP filter should be < {0} Hz", _configuration.SafetyFactor * f0), NeuroLoopGainController.DefaultErrorMessageId);
        // Check B < 2*F0
        if (!(bandwith < 2 * f0))
          NeuroLoopGainController.AppController.ApplicationError.Add("Bandwidth should be < 2*F0", NeuroLoopGainController.DefaultErrorMessageId);

        //check smooth time
        if (!IsValidFloat(textBoxAnalysisPeriod, out doubleValue))
        {
          textBoxAnalysisPeriod.Focus();
          NeuroLoopGainController.AppController.ApplicationError.Add("Value for analysis time is not valid", NeuroLoopGainController.DefaultErrorMessageId);
        }
        if (((1 / doubleValue) > signalSFreq) || ((1 / doubleValue) < 1))
        {
          /* 
           * 1. Sampling rate of the output cannot be higher than sampling rate of input
           * 2. In the current version sampling rate of the outupt should be equal of higher than l Hz
           */
          textBoxAnalysisPeriod.Focus();
          NeuroLoopGainController.AppController.ApplicationError.Add("Analysis time should be between " + string.Format("{0}", 1 / signalSFreq) + " and 1 s", NeuroLoopGainController.DefaultErrorMessageId);
        }
        else
        {
          double inputDoubleValue = doubleValue;
          //Effective output sampling rate should be an integer number (ej. 2 Hz is ok, 2.1 Hz will be rounded to 2 Hz)
          doubleValue = MathEx.RoundNearest(1 / doubleValue);
          Range.EnsureRange(doubleValue, 1, signalSFreq);
          if (!MathEx.SameValue(inputDoubleValue, (1 / doubleValue)))
          {
            textBox.AppendText(Environment.NewLine + "Warning, input SmoothTime will be approximated to " + string.Format("{0}", 1 / doubleValue) + " because the application cannot handle user's innitial value " + string.Format("{0}", doubleValue));
            ErrorLogger.WriteLog("Warning, input SmoothTime will be approximated to " + string.Format("{0}", 1 / doubleValue) + " because the application cannot handle user's innitial value " + string.Format("{0}", doubleValue));
          }
        }
      }

      // Calculate Fcompute/IIR_UnderSampler
      if (!NeuroLoopGainController.AppController.ApplicationError.Signaled)
      {
        // Calculate low and high value for IIRunderSampler
        int iirUnderSamplerLo = (int)Math.Truncate(0.99 * signalSFreq / fcomputeMax) + 1;
        int iirUnderSamplerHi = (int)Math.Truncate(signalSFreq / fcomputeMin);
        if (!(iirUnderSamplerLo <= iirUnderSamplerHi))
          NeuroLoopGainController.AppController.ApplicationError.Add("Unable to find a valid computation frequency. " +
             "F0 and/or Fc too large, or LP too small", NeuroLoopGainController.DefaultErrorMessageId);

        // IIRUnderSampler == 0 => we want to compute it automatically
        if (_configuration.IIRUnderSampler == 0)
        {
          // Calculate FcomputeDesired
          /* Theoretically the next value for FcomputeDesired is ok,
             but due to aliasing of alpha and sigma frequencies for
             small values of FcomputeDesired it is better to choose
             a value around 50 to 60 Hz.

            FcomputeDesired:=4.1*Max(F0,Fc);
          */
          const double fcomputeDesired = 56;
          // Find IIUnderSampler where Fcompute is closest to FcomputeDesired
          double fcompute = -1;
          for (int i = iirUnderSamplerLo; i <= iirUnderSamplerHi; i++)
          {
            double d = signalSFreq / i;
            if ((!MathEx.SameValue(fcompute, -1)) &&
                (Math.Abs(fcomputeDesired - d) >= Math.Abs(fcomputeDesired - fcompute))) continue;
            _configuration.IIRUnderSampler = i;
            fcompute = d;
          }
        }
      }

      if (!IsValidFileName(textBoxOutputFileName.Text))
      {
        textBoxOutputFileName.Focus();
        NeuroLoopGainController.AppController.ApplicationError.Add("Output path or filename is not valid", NeuroLoopGainController.DefaultErrorMessageId);
      }

      return !NeuroLoopGainController.AppController.ApplicationError.Signaled;
    }

    /// <summary>
    /// Sets the controller analysis values from the textboxes.
    /// </summary>
    private void DoSetInput()
    {
      NeuroLoopGainController.AppController.AppConf.F0 = GetFloatValue(textBoxF0);
      NeuroLoopGainController.AppController.AppConf.FC = GetFloatValue(textBoxFc);
      NeuroLoopGainController.AppController.AppConf.BandWidth = GetFloatValue(textBoxB);
      NeuroLoopGainController.AppController.AppConf.SmoothRate = GetFloatValue(textBoxSmootherrate);
      NeuroLoopGainController.AppController.AppConf.SmoothTime = GetFloatValue(textBoxAnalysisPeriod);

      NeuroLoopGainController.AppController.InputEDFFileName = textBoxInputFilename.Text;
      NeuroLoopGainController.AppController.AppConf.OutputFileName = textBoxOutputFileName.Text;
      NeuroLoopGainController.AppController.InputSignalSelected = signalsComboBox.SelectedIndex;
    }

    /// <summary>
    /// Handles the change of the input file name. Opens the file and sets the available signal labels.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void EdfInputFileNameChange(object sender, EventArgs e)
    {
      signalsComboBox.Items.Clear();
      signalsComboBox.ResetText();

      signalsComboBox.Enabled = false;
      buttonStart.Enabled = false;
      if (File.Exists(textBoxInputFilename.Text))
      {
        EdfFile edfFile = new EdfFile(textBoxInputFilename.Text, true, false, false, false);

        if (edfFile.ValidFormat)
        {
          _edfFile = edfFile;

          textBox.AppendText(Environment.NewLine + "Number of signals: " + _edfFile.FileInfo.NrSignals.ToString());

          for (int k = 0; k < _edfFile.FileInfo.NrSignals; k++)
          {
            signalsComboBox.Items.Add(string.Format("{0} - {1} ({2}Hz)", k + 1, _edfFile.SignalInfo[k].SignalLabel, _edfFile.SignalInfo[k].NrSamples / _edfFile.FileInfo.SampleRecDuration));
          }
          if (signalsComboBox.Items.Count > 0)
            signalsComboBox.SelectedIndex = 0;
          signalsComboBox.Enabled = true;
          buttonStart.Enabled = true;
        }
      }
    }

    /// <summary>
    /// Gets the float value from the textbox.
    /// </summary>
    /// <param name="textbox">The textbox.</param>
    /// <returns>The float value</returns>
    private double GetFloatValue(Control textbox)
    {
      double value;
      if (!IsValidFloat(textbox, out value))
        throw new NeuroLoopGainGuiException(string.Format("Textbox {0}'s text is not a valid double.", textbox.Name));
      return value;
    }

    /// <summary>
    /// Determines whether the specified textbox contains a valid float value.
    /// </summary>
    /// <param name="textbox">The textbox.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    ///   <c>true</c> if the specified textbox contains a valid float value; otherwise, <c>false</c>.
    /// </returns>
    private bool IsValidFloat(Control textbox, out double value)
    {
      return double.TryParse(textbox.Text, NumberStyles.Float, FormatProvider, out value) ||
             double.TryParse(textbox.Text, NumberStyles.Float, GuiFormatProvider, out value);
    }

    /// <summary>
    /// Determines whether the specified string corresponds to a valid file path including filename
    /// </summary>
    /// <param name="filepath">The string containing the file path to check.</param>
    /// <returns>
    ///   <c>true</c> if the specified string contains a valid filepath; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsValidFileName(string filepath)
    {
      // If no filepath supplied, result is false
      if (string.IsNullOrEmpty(filepath))
        return false;

      // Check for invalid path characters
      string invalidChars = new String(Path.GetInvalidPathChars());
      Regex containsABadCharacter = new Regex("[" + Regex.Escape(invalidChars) + "]");

      if (containsABadCharacter.IsMatch(filepath))
        return false;

      // Check for invalid filename characters
      invalidChars = new String(Path.GetInvalidFileNameChars());
      containsABadCharacter = new Regex("[" + Regex.Escape(invalidChars) + "]");

      string filename = Path.GetFileName(filepath);
      if (filename == null || containsABadCharacter.IsMatch(filename))
        return false;

      // Check for drive
      string drive = Path.GetPathRoot(filepath);
      return drive != null && Directory.GetLogicalDrives().Contains(drive.ToUpperInvariant());
    }

    /// <summary>
    /// Sets the analysis parameters to the textboxes.
    /// </summary>
    /// <param name="f0">The f0.</param>
    /// <param name="b">The b.</param>
    /// <param name="fc">The fc.</param>
    /// <param name="smootherrate">The smootherrate.</param>
    private void SetAnalysisParameters(double f0, double b, double fc, double smootherrate)
    {
      textBoxF0.Text = f0.ToString(FormatProvider);
      textBoxB.Text = b.ToString(FormatProvider);
      textBoxFc.Text = fc.ToString(FormatProvider);
      textBoxSmootherrate.Text = smootherrate.ToString(FormatProvider);
    }

    #endregion Private Methods

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MainGUI"/> class.
    /// Initialises the input values with default values.
    /// </summary>
    public MainGUI()
    {
      InitializeComponent();
    }

    #endregion Constructors

    #region public properties

    /// <summary>
    /// Gets the format provider, default US-Eng, to check for valid textbox values.
    /// </summary>
    public IFormatProvider FormatProvider { get { return _formatProvider; } }

    /// <summary>
    /// Gets the GUI format provider, default to current gui language, to check for valid textbox values.
    /// </summary>
    public IFormatProvider GuiFormatProvider { get { return _guiFormatProvider; } }

    #endregion public properties

    private void MainGUI_Shown(object sender, EventArgs e)
    {
      Shown -= MainGUI_Shown;
      HandleCommandLineParameters();
    }

    protected string EdfFilename { get; set; }

    protected string ConfigurationFilename { get; set; }

    private bool BatchMode { get; set; }

    private static void TryParseToDouble(string s, out double value)
    {
      if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
        double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out value);
    }

    /// <summary>
    /// Handles the commandline parameters (if any).
    /// </summary>
    private void HandleCommandLineParameters()
    {
      string configFilename = null;
      string inputFilename = null;
      int inputSignal = -1;
      string outputFilename = null;
      bool showHelp = false;
      string usage = string.Empty;
      double lp = double.NaN;
      double hp = double.NaN;

      if (Environment.GetCommandLineArgs().Length > 0)
      {
        //// First check the getopt() way....
        //CommandlineParserGetOpt commandLineParserGetOpt = new CommandlineParserGetOpt();
        //commandLineParserGetOpt.AddOption("c", "configfilename", "Configuration file containing analysis parameters",
        //                       CommandlineOptionFlags.HasParameter | CommandlineOptionFlags.Required,
        //                       (p, v) => { configFilename = v; });
        //commandLineParserGetOpt.AddOption("h", "help", "Show usage help",
        //                       CommandlineOptionFlags.HideInUsage,
        //                       (p, v) => { showHelp = true; });

        //commandLineParserGetOpt.Parse();

        //if (showHelp)
        //  usage = commandLineParserGetOpt.GetHelp();

        //if (!showHelp && commandLineParserGetOpt.MissingRequiredOptions.Count > 0)
        //{
        //  CommandlineParser commandlineParser = new CommandlineParser();
        //  commandlineParser.AddOption("EdfInputFilename", string.Empty, "EDF(+) input filename", string.Empty,
        //                              CommandlineOptionFlags.Required, (p, v) =>
        //                                                                 { inputFilename = v; }, 0);
        //  commandlineParser.AddOption("SignalIndex", string.Empty, "Index (0 - N) of signal to be analysed",
        //                              string.Empty, CommandlineOptionFlags.Required,
        //                              (p, v) => int.TryParse(v, out inputSignal), 1);
        //  commandlineParser.AddOption("OutputFilename", string.Empty, "Analysis output filename", string.Empty,
        //                              CommandlineOptionFlags.Required, (p, v) =>
        //                                                                 { outputFilename = v; }, 2);
        //  commandlineParser.AddOption("ConfigurationFilename", string.Empty, "Configuration filename", string.Empty,
        //                              CommandlineOptionFlags.None, (p, v) =>
        //                              { configFilename = v; }, 3);
        //  commandlineParser.AddOption("Batch", string.Empty, "Batchmode, no user interaction",
        //                              (p, v) => { BatchMode = true; });
        //  commandlineParser.AddOption("LP", string.Empty, "Low Pass filter value (Hz)",
        //                              CommandlineOptionFlags.HasParameter, (p, v) => TryParseToDouble(v, out lp));
        //  commandlineParser.AddOption("HP", string.Empty, "High Pass filter value (Hz)",
        //                              CommandlineOptionFlags.HasParameter, (p, v) => TryParseToDouble(v, out hp));
        //  commandlineParser.AddOption("?", string.Empty, "Show usage help",
        //                         CommandlineOptionFlags.HideInUsage,
        //                         (p, v) => { showHelp = true; });

        //  commandlineParser.Parse();

        //  if (showHelp)
        //    usage = commandlineParser.GetHelp();
        //}

        CommandlineParser commandlineParser = new CommandlineParser();
        commandlineParser.AddOption("Input", string.Empty, "EDF(+) input filename", "EdfInputFilename",
                                    CommandlineOptionFlags.HasParameter, (p, v) => { inputFilename = v; });
        commandlineParser.AddOption("SignalIndex", string.Empty, "Index (0 - N-1" + ") of signal to be analysed",
                                    "SignalIndex", CommandlineOptionFlags.HasParameter,
                                    (p, v) => int.TryParse(v, out inputSignal));
        commandlineParser.AddOption("Output", string.Empty, "Analysis output filename", "OutputFilename",
                                    CommandlineOptionFlags.HasParameter, (p, v) => { outputFilename = v; });
        commandlineParser.AddOption("ConfigFile", string.Empty, "Configuration filename", "ConfigurationFilename",
                                    CommandlineOptionFlags.HasParameter, (p, v) =>
                                    { configFilename = v; });
        commandlineParser.AddOption("LP", string.Empty, "Low Pass filter value (Hz)","LowestLowPassFrequency",
                                    CommandlineOptionFlags.HasParameter, (p, v) => TryParseToDouble(v, out lp));
        commandlineParser.AddOption("HP", string.Empty, "High Pass filter value (Hz)", "HighestHighPassFrequency",
                                    CommandlineOptionFlags.HasParameter, (p, v) => TryParseToDouble(v, out hp));
        commandlineParser.AddOption("Batch", string.Empty, "Batchmode, no user interaction",
                                    (p, v) => { BatchMode = true; });
        commandlineParser.AddOption("?", string.Empty, "Show usage help",
                               CommandlineOptionFlags.HideInUsage,
                               (p, v) => { showHelp = true; });

        commandlineParser.Parse();

        if (showHelp)
        {
          MessageBox.Show(commandlineParser.GetHelp(), Strings.UsageInformation);
          Application.Exit();
        }
      }

      // Read configuration settings from file; use default settings if no configuration file was set
      if (string.IsNullOrEmpty(configFilename))
        ErrorLogger.WriteLog("Configuration file not provided, using default configuration parameters");
      else
        ErrorLogger.WriteLog("Using configuration parameters from file " + configFilename);

      _configuration = new MCconfiguration(configFilename);

      if (!string.IsNullOrEmpty(outputFilename))
        _configuration.OutputFileName = outputFilename;
      if (File.Exists(inputFilename))
        textBoxInputFilename.Text = inputFilename;
      if (inputSignal >= 0)
        signalsComboBox.SelectedIndex = inputSignal;

      if (!double.IsNaN(lp))
        textBoxLP.Text = lp.ToString();
      if (!double.IsNaN(hp))
        textBoxHP.Text = hp.ToString();

      NeuroLoopGainController.AppController.AppConf = _configuration;

      // Default values for textBoxes
      textBoxF0.Text = _configuration.F0.ToString(FormatProvider);
      textBoxB.Text = _configuration.BandWidth.ToString(FormatProvider);
      textBoxFc.Text = _configuration.FC.ToString(FormatProvider);
      textBoxSmootherrate.Text = _configuration.SmoothRate.ToString(FormatProvider);
      textBoxAnalysisPeriod.Text = _configuration.SmoothTime.ToString(FormatProvider);
      textBoxOutputFileName.Text = _configuration.OutputFileName;

      if (!BatchMode)
        return;

      // Batchmode requested; perform analysis without user input and close application after processing
      buttonStart_Click(this, new EventArgs());
      Application.Exit();
    }
  }
}

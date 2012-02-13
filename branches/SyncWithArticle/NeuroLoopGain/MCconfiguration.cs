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
using System.Xml.Linq;
using System.Xml.XPath;
using NeuroLoopGainLibrary.Logging;

namespace NeuroLoopGain
{
  /// <summary>
  /// MCconfiguration class is a placeholder for the (adjustable) NeuroLoopGain analysis parameters.
  /// On creation it assigns default values. You can also load the analysis parameters from file.
  /// </summary>
  public class MCconfiguration
  {
    #region Private Methods

    /// <summary>
    /// Assigns the default parameters.
    /// </summary>
    private void AssignDefaultParameters()
    {
      //Int_Gain = def_INT_Gain;
      //IIR_Gain_du = def_IIR_Gain_du;
      //IIR_Gain_s = def_IIR_Gain_s;

      IIRBackPolate = def_IIRBackpolate;

      LogFloatA = def_LogFloatA;
      LogFloatY0 = def_LogFloatY0;

      SS_SUmin = def_SS_SUmin;
      SS_SUmax = def_SS_SUmax;
      piBPeakWidth = def_piBPeakWidth;
      piBCorrelationFunctionBufferSize = def_piBCorrelationFunctionBufferSize;

      XpiBPlus = def_XpiBPlus;
      XpiBMinus = def_XpiBMinus;
      XpiBZero = def_XpiBZero;

      ArtMaxSeconds = def_ArtMaxSecs;
      MCEventDuration = def_MCEventDur;
      MCEventReject = def_MCEventRej;
      MicGain = def_MicGain;
      MCjumpFind = def_MCjumpFind;

      F0 = def_F0;
      FC = def_FC;
      BandWidth = def_BandWidth;
      IIRUnderSampler = def_IIRUnderSampler;
      SmoothRate = def_SmoothRate;
      SmoothTime = def_SUSSsmoothingTime;
      SafetyFactor = def_SafetyFactor;
      OutputFileName = def_OutputFileName;
      OverWriteOutputFile = def_OverWriteOutputFile;
      CopyInputSignal = def_CopyInputSignal;
    }

    /// <summary>
    /// Tries to load the parameter as bool.
    /// </summary>
    /// <param name="paramName">Name of the param.</param>
    /// <param name="value">outputs the loaded value if successful.</param>
    /// <returns><c>true</c> if the method succeeded.</returns>
    private bool TryParameterAsBool(string paramName, out bool value)
    {
      XElement node = _confXml.XPathSelectElement("/MCconfiguration/Parameter[@Name='" + paramName + "']");

      bool success = false;
      value = false;

      try
      {
        if (node != null)
          if (!string.Empty.Equals(node.Value))
          {
            value = Convert.ToBoolean(node.Value);
            success = true;
          }
      }
      catch (Exception e)
      {
        ErrorLogger.WriteExceptionToLog(e);
      }

      if (!success)
        ErrorLogger.WriteErrorLog("Parameter '" + paramName + "' was not found in configuration file. Using default value");

      return success;
    }

    /// <summary>
    /// Tries to load parameter as double.
    /// </summary>
    /// <param name="paramName">Name of the param.</param>
    /// <param name="value">outputs the loaded value if successful.</param>
    /// <returns><c>true</c> if the method succeeded.</returns>
    private bool TryParameterAsDouble(string paramName, out double value)
    {
      XElement node = _confXml.XPathSelectElement("/MCconfiguration/Parameter[@Name='" + paramName + "']");

      bool success = false;
      value = -1;

      try
      {
        if (node != null)
          if (!string.Empty.Equals(node.Value))
          {
            value = Convert.ToDouble(node.Value, CultureInfo.InvariantCulture);
            success = true;
          }
      }
      catch (Exception e)
      {
        ErrorLogger.WriteExceptionToLog(e);
      }

      if (!success)
        ErrorLogger.WriteErrorLog("Parameter '" + paramName + "' was not found in configuration file. Using default value");

      return success;
    }

    /// <summary>
    /// Tries to load parameter as int.
    /// </summary>
    /// <param name="paramName">Name of the param.</param>
    /// <param name="value">outputs the loaded value if successful.</param>
    /// <returns><c>true</c> if the method succeeded.</returns>
    private bool TryParameterAsInt(string paramName, out int value)
    {
      XElement node = _confXml.XPathSelectElement("/MCconfiguration/Parameter[@Name='" + paramName + "']");

      bool success = false;
      value = -1;

      try
      {
        if (node != null)
          if (!string.Empty.Equals(node.Value))
          {
            value = Convert.ToInt32(node.Value);
            success = true;
          }
      }
      catch (Exception e)
      {
        ErrorLogger.WriteExceptionToLog(e);
      }

      if (!success)
        ErrorLogger.WriteErrorLog("Parameter '" + paramName + "' was not found in configuration file. Using default value");

      return success;
    }

    /// <summary>
    /// Tries to load parameter as short.
    /// </summary>
    /// <param name="paramName">Name of the param.</param>
    /// <param name="value">outputs the loaded value if successful.</param>
    /// <returns><c>true</c> if the method succeeded.</returns>
    private bool TryParameterAsShort(string paramName, out short value)
    {
      XElement node = _confXml.XPathSelectElement("/MCconfiguration/Parameter[@Name='" + paramName + "']");

      bool success = false;
      value = -1;

      try
      {
        if (node != null)
          if (!string.Empty.Equals(node.Value))
          {
            value = Convert.ToInt16(node.Value);
            success = true;
          }
      }
      catch (Exception e)
      {
        ErrorLogger.WriteExceptionToLog(e);
      }

      if (!success)
        ErrorLogger.WriteErrorLog("Parameter '" + paramName + "' was not found in configuration file. Using default value");

      return success;
    }

    /// <summary>
    /// Tries to load parameter as string.
    /// </summary>
    /// <param name="paramName">Name of the param.</param>
    /// <param name="value">outputs the loaded value if successful.</param>
    /// <returns><c>true</c> if the method succeeded.</returns>
    private bool TryParameterAsString(string paramName, out string value)
    {
      XElement node = _confXml.XPathSelectElement("/MCconfiguration/Parameter[@Name='" + paramName + "']");

      bool success = false;
      value = null;

      try
      {
        if (node != null)
          if (!string.Empty.Equals(node.Value))
          {
            value = node.Value;
            success = true;
          }
      }
      catch (Exception e)
      {
        ErrorLogger.WriteExceptionToLog(e);
      }
      if (!success)
        ErrorLogger.WriteErrorLog("Parameter '" + paramName + "' was not found in configuration file. Using default value");

      return success;
    }

    #endregion Private Methods



    #region constants

    // DEFAULT APPLICATION PARAMETERS

    private const double def_LogFloatY0 = 0.0001;
    private const double def_LogFloatA = 0.001;

    private const int def_XpiBPlus = 9;   // >0. HF artifact if [SS-SDU-piB]/piB >= XpiBplus : st. 9
    private const int def_XpiBMinus = -9; // <0. LF artifact if [SS-SDU-piB]/piB <= XpiBminus: st.-9
    private const int def_XpiBZero = 10;  // >0. No signal if piB/[SS-SDU] >= XpiBzero : st.10
    private const double def_IIRBackpolate = 0.5;    // 0.0 < Backpolate < 1.0 on s: standard 0.5

    //private const double def_INT_Gain = 1.0;     // Avoids bitnoise and clipping in SS and SU: st.1.0
    //private const double def_IIR_Gain_du = 1;  // Gain of IIR filter that computes du: standard 200
    //private const double def_IIR_Gain_s = 1;     // Gain of IIR filter that computes s: standard 8

    private const short def_SS_SUmin = -2000;  // min value for hystogram's x-axis
    private const short def_SS_SUmax = 30000;  // max value for hystogram's x-axis
    //private const int def_piBSecsSure = 1200;   // Surely artifact-free seconds in InFile: st.1200
    private const double def_piBPeakWidth = 0.2;    // Peak width as a fraction (0..1) of piB: st.0.2

    private const double def_MicGain = 10.0;        // Gain (DigiRange/PhysiRange) of MicroContinuity
    private const int def_ArtMaxSecs = 7;          // Maximum 'spread' (in s) of an artifact: st.7
    private const int def_MCEventDur = 1;    // 0..MCEventMaxDur: expected duration MC-event: st.1
    private const double def_MCEventRej = 2.0;   // >0.0. Reject if Event>MCEvRej*SmRate*100%: st.2.0
    private const double def_MCjumpFind = 0.5;   // Reset smoother at jumps > MCjumpFind*100%: st.0.5

    private const int def_piBCorrelationFunctionBufferSize = 6000;
    // Next function is no longer parsed as string text, but implemented directly in the code 
    /*
     private const string def_piBCorrelationFunction =
        "(2/3)*(-0.5894)*(HEAV(x+(2*Pi))-HEAV(x+(Pi/2)))+"+
        "(HEAV(x+(Pi/2))-HEAV(x))*((sin(2*x))/(2*x))+"+
        "(HEAV(x)-HEAV(x-Pi/2))*((sin(2*x))/(2*x))";
     */

    private const double def_F0 = 1;
    private const double def_FC = 1.8;
    private const double def_BandWidth = 1.5;
    private const int def_IIRUnderSampler = 0;
    private const double def_SUSSsmoothingTime = 1;
    private const double def_SmoothRate = 0.01666;
    private const double def_SafetyFactor = 3;
    private const string def_OutputFileName = "output.EDF";
    private const bool def_OverWriteOutputFile = true;
    private const bool def_CopyInputSignal = false;

    #endregion constants


    #region private attributes

    /// <summary>
    /// Holds the reference to the XML document with analysis parameters.
    /// </summary>
    private readonly XDocument _confXml;

    #endregion private attributes

    #region properties

    public int XpiBPlus { get; set; }
    public int XpiBMinus { get; set; }
    public int XpiBZero { get; set; }
    public int ArtMaxSeconds { get; set; }
    public int MCEventDuration { get; set; }
    public double MCEventReject { get; set; }
    public double MCjumpFind { get; set; }

    public double MicGain { get; set; }
    //public double Int_Gain { get; set; }
    //public double IIR_Gain_du { get; set; }
    //public double IIR_Gain_s { get; set; }
    public double IIRBackPolate { get; set; }

    public double LogFloatA { get; set; }
    public double LogFloatY0 { get; set; }

    public short SS_SUmin { get; set; }
    public short SS_SUmax { get; set; }
    public double piBPeakWidth { get; set; }
    public int piBCorrelationFunctionBufferSize { get; set; }

    public double F0 { get; set; }
    public double FC { get; set; }
    public double BandWidth { get; set; }
    public double SmoothRate { get; set; }
    public double SmoothTime { get; set; }
    public int IIRUnderSampler { get; set; }
    public double SafetyFactor { get; set; }

    public string OutputFileName { get; set; }
    public bool OverWriteOutputFile { get; set; }

    public bool ShowPiBHistogram { get; set; }
    public bool CopyInputSignal { get; set; }

    #endregion properties


    #region constructors

    /// <summary>
    /// Initialises a new instance of the <see cref="MCconfiguration"/> class.
    /// Initialises the parameters to their default values.
    /// </summary>
    public MCconfiguration()
    {
      AssignDefaultParameters();
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="MCconfiguration"/> class.
    /// </summary>
    /// <param name="configurationFile">The XML configuration file to load the analysis parameters from.</param>
    public MCconfiguration(string configurationFile)
    {
      AssignDefaultParameters();

      try
      {
        _confXml = XDocument.Load(configurationFile);
      }
      catch (Exception e)
      {
        _confXml = null;
        ErrorLogger.WriteExceptionToLog(e);
        ErrorLogger.WriteLog("Using default configuration parameters");
      }

      if (_confXml == null)
        return;

      String stringValue;
      int intValue;
      short shortValue;
      double doubleValue;
      bool boolValue;

      if (TryParameterAsInt("XpiBPlus", out intValue))
        XpiBPlus = intValue;

      if (TryParameterAsInt("XpiBMinus", out intValue))
        XpiBMinus = intValue;

      if (TryParameterAsInt("XpiBZero", out intValue))
        XpiBZero = intValue;

      if (TryParameterAsInt("ArtMaxSeconds", out intValue))
        ArtMaxSeconds = intValue;

      if (TryParameterAsInt("MCEventDuration", out intValue))
        MCEventDuration = intValue;

      if (TryParameterAsDouble("MCEventReject", out doubleValue))
        MCEventReject = doubleValue;

      if (TryParameterAsDouble("MCjumpFind", out doubleValue))
        MCjumpFind = doubleValue;

      if (TryParameterAsDouble("MicGain", out doubleValue))
        MicGain = doubleValue;

      if (TryParameterAsDouble("IIRBackPolate", out doubleValue))
        IIRBackPolate = doubleValue;

      if (TryParameterAsDouble("LogFloatA", out doubleValue))
        LogFloatA = doubleValue;

      if (TryParameterAsDouble("LogFloatY0", out doubleValue))
        LogFloatY0 = doubleValue;

      if (TryParameterAsShort("SS_SUmin", out shortValue))
        SS_SUmin = shortValue;

      if (TryParameterAsShort("SS_SUmax", out shortValue))
        SS_SUmax = shortValue;

      if (TryParameterAsDouble("piBPeakWidth", out doubleValue))
        piBPeakWidth = doubleValue;

      if (TryParameterAsInt("piBCorrelationFunctionBufferSize", out intValue))
        piBCorrelationFunctionBufferSize = intValue;

      if (TryParameterAsInt("IIRUnderSampler", out intValue))
        IIRUnderSampler = intValue;

      if (TryParameterAsDouble("F0", out doubleValue))
        F0 = doubleValue;

      if (TryParameterAsDouble("FC", out doubleValue))
        FC = doubleValue;

      if (TryParameterAsDouble("B", out doubleValue))
        BandWidth = doubleValue;

      if (TryParameterAsDouble("SmoothRate", out doubleValue))
        SmoothRate = doubleValue;

      if (TryParameterAsDouble("SafetyFactor", out doubleValue))
        SafetyFactor = doubleValue;

      if (TryParameterAsDouble("SmoothTime", out doubleValue))
        SmoothTime = doubleValue;

      if (TryParameterAsString("DefaultOutputFile", out stringValue))
        OutputFileName = stringValue;

      if (TryParameterAsBool("OverWriteOutputFile", out boolValue))
        OverWriteOutputFile = boolValue;

      if (TryParameterAsBool("ShowPiBHistogram", out boolValue))
        ShowPiBHistogram = boolValue;

      if (TryParameterAsBool("CopyInputSignal", out boolValue))
        CopyInputSignal = boolValue;
    }

    #endregion constructors
  }
}

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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NeuroLoopGainLibrary.Edf;
using NeuroLoopGainLibrary.Errorhandling;
using NeuroLoopGainLibrary.Logging;
using NeuroLoopGainLibrary.Mathematics;

namespace NeuroLoopGain
{
  class NeuroLoopGainController
  {
    #region private fields

    // singleton
    private static NeuroLoopGainController _instance;
    private readonly NeuroLoopGain _neuroLoopGain;
    private readonly string[] _signalOutputLabels = new[] { "input", "SU", "SS", "SU+", "SU-", "SS+", "SS-", "SSP", "SS0", "HF artifact", "LF artifact", "Missing signal", "MC (Gain)", "MC (Gain)jump", "MC (Gain)event" };
    /// <summary>
    /// The maximum EDF blocksize as specified by the EDF specifications.
    /// </summary>
    private const int MaxEdFblockSize = 61440;
    /// <summary>
    /// The maximum EDF block duration in seconds.
    /// </summary>
    private const double MaxBlockDuration = 60 * 30;

    #endregion private fields

    #region Private Methods

    private NeuroLoopGainController()
    {
      _neuroLoopGain = new NeuroLoopGain(this);
      ApplicationError = new ErrorMessage();
    }

    /// <summary>
    /// Prepares the EDF files.
    /// Opens the input file, creates a new ouput file and sets the header information.
    /// </summary>
    /// <returns></returns>
    private bool PrepareEDFFiles()
    {
      if (AppConf.CopyInputSignal)
      {
        _neuroLoopGain.NumOutputSignals = 15;
      }
      else
      {
        _neuroLoopGain.NumOutputSignals = 14;
      }


      double[] sFrecs = new double[_neuroLoopGain.NumOutputSignals];

      // Calculate best value for output EDF datablock duration according to analysis parameters

      try
      {
        //OPEN INPUT FILENAME
        _neuroLoopGain.InputEDFFile = new EdfFile(InputEDFFileName, true, false, false, true);
        _neuroLoopGain.InputSignalSelected = InputSignalSelected;

        int numInputSignals = _neuroLoopGain.InputEDFFile.FileInfo.NrSignals;
        _neuroLoopGain.InputBufferOffsets = new int[numInputSignals];
        for (int k = 0; k < numInputSignals; k++)
          _neuroLoopGain.InputBufferOffsets[k] = _neuroLoopGain.InputEDFFile.SignalInfo[k].BufferOffset;

        //OUTPUT EDF
        EdfFile outputEDF = new EdfFile(AppConf.OutputFileName, false, false, false, false);
        //EdfFile outputEDF = new EdfFile(AppConf.OutputFileName, false, true, false, false);

        EdfSignalInfoBase edfSignalInfo = _neuroLoopGain.InputEDFFile.SignalInfo[InputSignalSelected];

        outputEDF.CreateNewFile(_neuroLoopGain.NumOutputSignals);

        //PreCondition checked before: (1 / AppConf.SmoothTime) is between 1 and sFrecs[0] 
        double doubleValue = MathEx.RoundNearest(1 / AppConf.SmoothTime);
        Range.EnsureRange(doubleValue, 0, _neuroLoopGain.InputSampleFrequency);

        for (int k = 0; k < _neuroLoopGain.NumOutputSignals; k++)
        {
          sFrecs[k] = doubleValue;
        }
        if (AppConf.CopyInputSignal)
          sFrecs[0] = _neuroLoopGain.InputSampleFrequency;

        //TODO: check this
        List<EdfDataBlockSizeCalculatorResult> results = new List<EdfDataBlockSizeCalculatorResult>();
        double dataBlockDuration = EdfDataBlockSizeCalculator.Calculate(sFrecs, MaxEdFblockSize, results);
        if (!MathEx.SameValue(dataBlockDuration, -1))
        {
          // Look for minimun possible error
          double bestError = results[0].MaxRelativeError;
          int bestIdx = 0;
          for (int k = 1; k < results.Count; k++)
            if (results[k].MaxRelativeError < bestError)
            {
              bestError = results[k].MaxRelativeError;
              bestIdx = k;
            }
            else if (MathEx.SameValue(results[k].MaxRelativeError, bestError) && (results[k].Duration > results[bestIdx].Duration) && (results[k].Duration <= MaxBlockDuration))
            {
              // Try to get block durations as much as possible to avoid excesive disk reads
              bestIdx = k;
            }
          if (!MathEx.SameValue(bestError, 0))
          {
            //TODO: error??, not valid SUSSsmoothingTime parameter (we're gonna miss some samples)
            ApplicationError.Add("not valid SmoothTime parameter (we're gonna miss some samples)", DefaultErrorMessageId);
            ErrorLogger.WriteErrorLog("not valid SmoothTime parameter (we're gonna miss some samples). Error = " + bestError.ToString());
            outputEDF.FileInfo.SampleRecDuration = results[bestIdx].Duration;
          }
          else
          {
            outputEDF.FileInfo.SampleRecDuration = results[bestIdx].Duration; // Duration in seconds of block record
            AppConf.SmoothTime = 1.0 / sFrecs[1]; // Reset the value of SmoothTime to the one actually used by the system
          }
        }
        else
        {
          ApplicationError.Add("failed to assign a data block duration for output EDF", DefaultErrorMessageId);
          ErrorLogger.WriteErrorLog("failed to assign a data block duration for output EDF");
        }

        _neuroLoopGain.OutputBufferOffsets = new int[15];
        _neuroLoopGain.OutputBufferOffsets[0] = 0;

        for (int k = 0; k < _neuroLoopGain.NumOutputSignals; k++)
        {
          outputEDF.SignalInfo[k].PreFilter = edfSignalInfo.PreFilter;
          double nsamples = outputEDF.FileInfo.SampleRecDuration * sFrecs[k];
          if (Math.Truncate(nsamples) < nsamples)
          {
            //TODO: Error?? the number of samples in a data block should be an integer number
            ApplicationError.Add("not valid SmoothTime parameter: we are going miss some samples", DefaultErrorMessageId);
            ErrorLogger.WriteErrorLog("not valid SmoothTime parameter: we are going miss some samples");
            /* It should effectively be an integer number, but if we admit some error to happen
             * (bestError != 0), then we can work with the truncated value => we're gonna miss some
             * sample during the process
             */
            nsamples = Math.Truncate(nsamples);
          }
          outputEDF.SignalInfo[k].NrSamples = (int)nsamples;
          //if (k == 0)
          if ((k == 0) && AppConf.CopyInputSignal)
          {
            outputEDF.SignalInfo[k].DigiMax = edfSignalInfo.DigiMax;
            outputEDF.SignalInfo[k].DigiMin = edfSignalInfo.DigiMin;
            outputEDF.SignalInfo[k].PhysiMax = edfSignalInfo.PhysiMax;
            outputEDF.SignalInfo[k].PhysiMin = edfSignalInfo.PhysiMin;
            outputEDF.SignalInfo[k].PhysiDim = edfSignalInfo.PhysiDim;
          }
          else
          {
            outputEDF.SignalInfo[k].DigiMax = short.MaxValue;
            outputEDF.SignalInfo[k].DigiMin = -short.MaxValue;
            //if (k < 9)
            if (k < _neuroLoopGain.NumOutputSignals - 6)
            {
              outputEDF.SignalInfo[k].PhysiDim = "Filtered";
              outputEDF.SignalInfo[k].PhysiMax = short.MaxValue;
              outputEDF.SignalInfo[k].PhysiMin = -short.MaxValue;
            }
            else
              //if ((k >= 9) && (k < 12))
              if ((k >= _neuroLoopGain.NumOutputSignals - 6) && (k < _neuroLoopGain.NumOutputSignals - 3))
              {
                outputEDF.SignalInfo[k].PhysiMax = short.MaxValue;
                outputEDF.SignalInfo[k].PhysiMin = -short.MaxValue;
              }
              else
              {
                outputEDF.SignalInfo[k].PhysiDim = "%";
                outputEDF.SignalInfo[k].PhysiMax = short.MaxValue / AppConf.MicGain;
                outputEDF.SignalInfo[k].PhysiMin = -short.MaxValue / AppConf.MicGain;
              }
            if (k > 0)
            {
              if (AppConf.CopyInputSignal)
                _neuroLoopGain.OutputBufferOffsets[k] = _neuroLoopGain.OutputBufferOffsets[k - 1] + outputEDF.SignalInfo[k - 1].NrSamples;
              else
                _neuroLoopGain.OutputBufferOffsets[k + 1] = _neuroLoopGain.OutputBufferOffsets[k] + outputEDF.SignalInfo[k].NrSamples;
            }
          }

          outputEDF.SignalInfo[k].Reserved = edfSignalInfo.Reserved;
          //if ((k > 0) && (k < 9))
          if ((k >= _neuroLoopGain.NumOutputSignals - 14) && (k < _neuroLoopGain.NumOutputSignals - 6))
            if (AppConf.CopyInputSignal)
              outputEDF.SignalInfo[k].SignalLabel = _signalOutputLabels[k] + " " + edfSignalInfo.PhysiDim + "**2/x";
            else
              outputEDF.SignalInfo[k].SignalLabel = _signalOutputLabels[k + 1] + " " + edfSignalInfo.PhysiDim + "**2/x";
          else
            if (AppConf.CopyInputSignal)
              outputEDF.SignalInfo[k].SignalLabel = _signalOutputLabels[k];
            else
              outputEDF.SignalInfo[k].SignalLabel = _signalOutputLabels[k + 1];
          outputEDF.SignalInfo[k].TransducerType = edfSignalInfo.TransducerType;
          outputEDF.SignalInfo[k].ThousandSeparator = edfSignalInfo.ThousandSeparator;
        }

        // Info for MC analysis
        outputEDF.FileInfo.Recording = string.Format(CultureInfo.InvariantCulture, 
          "Startdate {0} NeuroLoop-gain analysis at {1:#.#}Hz of {2} in {3}",
          _neuroLoopGain.InputEDFFile.FileInfo.StartDate.HasValue ? _neuroLoopGain.InputEDFFile.FileInfo.StartDate.Value.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) : "X",
          sFrecs[1],
          _neuroLoopGain.InputEDFFile.SignalInfo[InputSignalSelected].SignalLabel, Path.GetFileName(InputEDFFileName));

        //Copy header info from the original signal
        outputEDF.FileInfo.Patient = _neuroLoopGain.InputEDFFile.FileInfo.Patient;
        outputEDF.FileInfo.StartDate = _neuroLoopGain.InputEDFFile.FileInfo.StartDate;
        outputEDF.FileInfo.StartTime = _neuroLoopGain.InputEDFFile.FileInfo.StartTime;

        outputEDF.CommitChanges();

        _neuroLoopGain.MCsignalsBlockSamples = outputEDF.SignalInfo[1].NrSamples;
        _neuroLoopGain.OutputEDFFile = outputEDF;
      }

      catch (Exception e)
      {
        ApplicationError.Add("Error opening input and/or ouput files: " + e.Message, DefaultErrorMessageId);
        ErrorLogger.WriteErrorLog("Error opening input and/or ouput files: " + e.Message);
      }

      return !ApplicationError.Signaled;
    }

    #endregion Private Methods

    #region public fields

    public static int DefaultErrorMessageId = 1;

    #endregion public fields

    #region public properties

    /// <summary>
    /// Gets or sets the application configuration, containing all analysis parameters.
    /// </summary>
    /// <value>
    /// The application configuration.
    /// </value>
    public MCconfiguration AppConf { get; set; }

    /// <summary>
    /// Gets the app controller. Manages and returns singleton instance.
    /// </summary>
    public static NeuroLoopGainController AppController
    {
      get { return _instance ?? (_instance = new NeuroLoopGainController()); }
    }

    public ErrorMessage ApplicationError { get; set; }

    /// <summary>
    /// Gets or sets the name of the input EDF file.
    /// </summary>
    /// <value>
    /// The name of the input EDF file.
    /// </value>
    public string InputEDFFileName
    {
      get
      {
        return _neuroLoopGain.InputEDFFileName;
      }
      set
      {
        _neuroLoopGain.InputEDFFileName = value;
      }
    }

    /// <summary>
    /// Gets or sets the index of the input signal selected.
    /// </summary>
    /// <value>
    /// The input signal selected.
    /// </value>
    public int InputSignalSelected { get; set; }

    #endregion public properties

    #region public methods

    /// <summary>
    /// Analyzes the input file and writes the output to the AppConf.OutputFileName file.
    /// First checks if the output file already exists and if it is ok to overwrite it.
    /// </summary>
    /// <returns><c>true</c> if the analysis succeeded.</returns>
    public bool Analyze()
    {
      if (File.Exists(AppConf.OutputFileName) && !AppConf.OverWriteOutputFile)
      {
        ApplicationError.Add("Output file already exists", DefaultErrorMessageId);
        return false;
      }
      return PrepareEDFFiles() && _neuroLoopGain.Analyze();
    }

    #endregion public methods
  }
}

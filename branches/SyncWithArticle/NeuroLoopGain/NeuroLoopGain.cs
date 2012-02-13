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
using System.Diagnostics;
using NeuroLoopGainLibrary.Filters;
using NeuroLoopGainLibrary.Errorhandling;
using NeuroLoopGainLibrary.Edf;
using NeuroLoopGainLibrary.Arrays;
using NeuroLoopGainLibrary.Mathematics;
using System.Globalization;
using System.Windows.Forms;

namespace NeuroLoopGain
{
  class NeuroLoopGain
  {
    #region private fields

    //private const double ArtifactPhysicalDimensionResolution = 0.001;
    public const int ArtifactPhysicalDimensionResolution = 1;

    private readonly NeuroLoopGainController _appController;
    private DUEFilter DUEfilter;
    private double[] _suForw;
    private double[] _suBack;
    private double[] _ssForw;
    private double[] _ssBack;
    private double IIR_fCompute;
    private double IIR_Delta;
    private int IIR_fCompDiv;
    private int IIR_fCompMod;
    private MCJump LastMCJump;
    private int MinSamplesBetweenJumps;
    private int MaxSamplesHalfJump;
    private int MCEventDurationSamples;
    private int MCEventThreshold;
    private double MCjumpThreshold;
    private double _piBxx;
    private short _logPiBxx;
    private SEFilter SEfilter;
    private double SUsmooth;
    private double SSsmooth;

    #endregion private fields

    #region private properties

    private double ArtHF { get; set; }

    private double ArtLF { get; set; }

    private double ArtZero { get; set; }

    private short LogPiBxx
    {
      get
      {
        return _logPiBxx;
      }
      set
      {
        _logPiBxx = value;
        _piBxx = MathEx.ExpInteger(_logPiBxx, AppConf.LogFloatY0, AppConf.LogFloatA);
      }
    }

    private double PiBxx
    {
      get
      {
        return _piBxx;
      }
    }

    #endregion private properties

    #region Event handlers

    /// <summary>
    /// Does the initialisation of the DUE filter.
    /// </summary>
    /// <returns><c>true</c> if successful</returns>
    private bool DoComputeDU_InitializeFilter()
    {
      try
      {
        DUEfilter = new DUEFilter
        {
          Anticipate = true,
          BackPolate = 0
        };
        DUEfilter.Setting[1].Value = IIR_fCompute;                    // Sample Frequency
        // If we are reading calibrated and not raw data, gain has to be multiplied
        //  by (2*High(SmallInt)/(FullRangePhysiMax-FullRangePhysiMin))
        double r = 1;
        DUEfilter.Setting[2].Value = r;                               // Filter gain
        //DUEfilter.Setting[2].Value = appConf.IIR_Gain_du * r;         // Filter gain
        DUEfilter.Setting[3].Value = AppConf.FC;                      // Cutoff Frequency
        DUEfilter.Reset();
      }
      catch (Exception e)
      {
        AppError.Add(e.Message, NeuroLoopGainController.DefaultErrorMessageId);
      }

      return !AppError.Signaled;
    }

    /// <summary>
    /// Does the computation of the discrete-time steps for the IIR filters
    /// and writes the used analysis frequencies to the output file signal headers.
    /// </summary>
    /// <returns><c>true</c> if successful</returns>
    private bool DoComputeDUSE_InitializeVariables()
    {
      try
      {
        /* Compute the discrete-time steps for the IIR filters. These are required for
        exact time keeping during the computations */
        IIR_fCompute = InputSampleFrequency / AppConf.IIRUnderSampler;
        IIR_fCompDiv = (int)Math.Truncate(IIR_fCompute);
        Math.DivRem((int)InputSampleFrequency, AppConf.IIRUnderSampler, out IIR_fCompMod);

        if (IIR_fCompDiv <= 0)
          AppError.Add("The underSampled frequency should be >= 1 Hz", NeuroLoopGainController.DefaultErrorMessageId);
        else
        {
          IIR_Delta = 1 / IIR_fCompute;
          // Gains (DigiRange/PhysiRange) of SS and SU 'Powers' must be equal:
          //Int_Gain_SU = appConf.Int_Gain / (short.MaxValue / Math.Sqrt(2)); // SU=rougly Sqr(0..MaxSmallInt)
          //Int_Gain_SS = Int_Gain_SU * (appConf.IIR_Gain_du / appConf.IIR_Gain_s) * IIR_Delta;

          //double inputPhysMax = InputEDFFile.SignalInfo[InputSignalSelected].PhysiMax;
          //double inputPhysMin = InputEDFFile.SignalInfo[InputSignalSelected].PhysiMin;
          //InGain = (2*short.MaxValue)/(inputPhysMax-inputPhysMin);
          //PowGain = Math.Pow(InGain, 2) * appConf.IIR_Gain_s * appConf.IIR_Gain_du * Int_Gain_SU;
        }

        // Update header information
        //for (int k = 1; k <= 14; k++)
        for (int k = MCOutputSignalIndex.SU; k <= MCOutputSignalIndex.MCevent; k++)
          OutputEDFFile.SignalInfo[k].TransducerType =
            string.Format(CultureInfo.InvariantCulture, "fCompute/fc/f0/B={0:0.###}/{1:0.###}/{2:0.###}/{3:0.###}Hz.", IIR_fCompute, AppConf.FC,
                          AppConf.F0, AppConf.BandWidth);

        //TODO: Review this
        /*Format(
          'sign*LN[sign*(%s)/(%s)]/(%s) '+
          '(Kemp:J Sleep Res 1998-supp2:132)',[AddTrailSpaces(
           Format('at%4.1fHz',[F0],FS),8),FloatToStrLen(LogFloatY0,8,FS),
           FloatToStrLen(LogFloatA,8,FS)],FS);*/

        /*OutputEDFFile.SignalInfo[1].PreFilter = string.Format(CultureInfo.InvariantCulture,
                                                                        "sign*LN[sign*({0,-8})/({1,-8})]/({2,-8}) (Kemp:J Sleep Res 1998-supp2:132)",
                                                                        "uV**2/x", AppConf.LogFloatY0, AppConf.LogFloatA);
        OutputEDFFile.SignalInfo[2].PreFilter = OutputEDFFile.SignalInfo[1].PreFilter;*/
        OutputEDFFile.SignalInfo[MCOutputSignalIndex.SU].PreFilter = string.Format(CultureInfo.InvariantCulture,
                                                                      "sign*LN[sign*({0,-8})/({1,-8})]/({2,-8}) (Kemp:J Sleep Res 1998-supp2:132)",
                                                                      "uV**2/x", AppConf.LogFloatY0, AppConf.LogFloatA);
        OutputEDFFile.SignalInfo[MCOutputSignalIndex.SS].PreFilter = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SU].PreFilter;
      }
      catch (Exception e)
      {
        AppError.Add(e.Message, NeuroLoopGainController.DefaultErrorMessageId);
      }

      return (!AppError.Signaled);
    }

    /// <summary>
    /// Does the initialisation of the SE filter.
    /// </summary>
    /// <returns><c>true</c> if successful</returns>
    private bool DoComputeSE_InitializeFilter()
    {
      try
      {
        SEfilter = new SEFilter
        {
          Anticipate = false,
          BackPolate = AppConf.IIRBackPolate
        };
        SEfilter.Setting[1].Value = IIR_fCompute;                     // Sample Frequency
        // Because we are reading calibrated and not raw data, gain has to be multiplied
        //  by 1/DigiToPhysiGain
        double r = 1;
        SEfilter.Setting[2].Value = r;                                // Filter gain
        //SEfilter.Setting[2].Value = appConf.IIR_Gain_s * r;           // Filter gain
        SEfilter.Setting[3].Value = AppConf.FC;                       // Cutoff Frequency
        SEfilter.Setting[4].Value = AppConf.F0;                       // Center frequency
        SEfilter.Setting[5].Value = AppConf.BandWidth;                // Bandwidth
        SEfilter.Reset();
      }
      catch (Exception e)
      {
        AppError.Add(e.Message, NeuroLoopGainController.DefaultErrorMessageId);
      }

      return !AppError.Signaled;
    }

    /// <summary>
    /// Checks the MicroContinuity analysis parameters.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if all parameters are ok
    /// </returns>
    private bool MCSmooth_CheckSettings()
    {
      try
      {
        if ((AppConf.XpiBPlus < 1) || (AppConf.XpiBMinus > -1) || (AppConf.XpiBZero < 1) || (AppConf.ArtMaxSeconds < 1))
          AppError.Add("Artifact thresholds or -spread incorrect", NeuroLoopGainController.DefaultErrorMessageId);
        /*
        if (appConf.MCEventDuration < 1)
            appError.Add(string.Format("MCEventSamples = {0} but should be >= 1", appConf.MCEventDuration), NeuroLoopGainController.defaultErrorMessageId);
        */
        //TODO: Review the following modifications in order to allow higher output sampling rate
        if (AppConf.MCEventDuration < 1)
          AppError.Add(string.Format("MCEventDuration = {0} but should be >= 1", AppConf.MCEventDuration), NeuroLoopGainController.DefaultErrorMessageId);
        if (PiBxx < AppConf.LogFloatY0 * 10)
          AppError.Add(string.Format("piB = {0} is smaller than LogFloat_Y0*10", PiBxx), NeuroLoopGainController.DefaultErrorMessageId);
        if (AppConf.MicGain < 1.0)
          AppError.Add(string.Format("MCgain = {0} but should be >= 1.0", AppConf.MicGain), NeuroLoopGainController.DefaultErrorMessageId);
        if ((AppConf.SmoothRate <= 0.0) || (AppConf.SmoothRate >= 1.0))
          AppError.Add(string.Format("SmoothRate = {0} but should be > 0.0 and < 1.0)", AppConf.SmoothRate), NeuroLoopGainController.DefaultErrorMessageId);
      }
      catch (Exception e)
      {
        AppError.Add(e.Message, NeuroLoopGainController.DefaultErrorMessageId);
      }

      return !AppError.Signaled;
    }

    /// <summary>
    /// Detect events and reset jumps.
    /// </summary>
    /// <param name="outBuffer">The out buffer.</param>
    /// <param name="dataBlockSample">The data block sample.</param>
    /// <param name="forwardProcessing">if set to <c>true</c> processing samples forward.</param>
    private void MCSmooth_DetectEvents_ResetJumps(IList<short> outBuffer, int dataBlockSample, bool forwardProcessing)
    {
      //TODO: Review the following modifications in order to allow higher output sampling rate
      for (int k = MCEventDurationSamples; k >= 1; k--)
      {
        _suForw[k] = _suForw[k - 1];
        _suBack[k] = _suBack[k - 1];
        _ssForw[k] = _ssForw[k - 1];
        _ssBack[k] = _ssBack[k - 1];
      }
      _suForw[0] = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SUplus] + dataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA); //SU+
      _suBack[0] = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SUminus] + dataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA); //SU-
      _ssForw[0] = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SSplus] + dataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA); //SS+
      _ssBack[0] = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SSminus] + dataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA); //SS-
      if (forwardProcessing)
      {
        // Temporary storage in file in space for MCjump and MCevent (respectively)
        /*
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCjump] + dataBlockSample] = MathEx.LogFloat((FSUback[appConf.MCEventDuration] - FSUforw[appConf.MCEventDuration]) / 2, appConf.LogFloatY0, appConf.LogFloatA);
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCevent] + dataBlockSample] = MathEx.LogFloat((FSSback[appConf.MCEventDuration] + FSSforw[appConf.MCEventDuration]) / 2, appConf.LogFloatY0, appConf.LogFloatA);
        */
        //TODO: Review the following modifications in order to allow higher output sampling rate
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCjump] + dataBlockSample] = MathEx.LogFloat((_suBack[MCEventDurationSamples] - _suForw[MCEventDurationSamples]) / 2, AppConf.LogFloatY0, AppConf.LogFloatA);
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCevent] + dataBlockSample] = MathEx.LogFloat((_ssBack[MCEventDurationSamples] + _ssForw[MCEventDurationSamples]) / 2, AppConf.LogFloatY0, AppConf.LogFloatA);
      }
      else
      {
        double r = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCjump] + dataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA);
        double s = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCevent] + dataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA);
        /*
        r += (FSUforw[appConf.MCEventDuration] - FSUback[appConf.MCEventDuration]) / 2;
        s += (FSSforw[appConf.MCEventDuration] + FSSback[appConf.MCEventDuration]) / 2;
        */
        //TODO: Review the following modifications in order to allow higher output sampling rate
        r += (_suForw[MCEventDurationSamples] - _suBack[MCEventDurationSamples]) / 2;
        s += (_ssForw[MCEventDurationSamples] + _ssBack[MCEventDurationSamples]) / 2;
        double mcEvent = Range.EnsureRange(100 * AppConf.MicGain * r / s, -short.MaxValue, short.MaxValue);
        // Clean MCJump from this temporary storage necessary for MCevent
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCjump] + dataBlockSample] = 0;
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCevent] + dataBlockSample] = (short)MathEx.RoundNearest(mcEvent);
      }
      /* BK 22.3.96: event detection as below stronger discriminate
         bigger from smaller K-complexes. However, the small ones are
         still clear. So I decided for the above procedure.
         MCjump:=0.0;
         MCevent:=(1.0*SUmin [MCEventSamples]/SSmin [MCEventSamples]) - (1.0*SUplus[MCEventSamples]/SSplus[MCEventSamples]);
         MCevent:=MCevent*100.0*MCgain;
         end of BK 22.3.96
       */
    }

    /// <summary>
    /// Smooths backward.
    /// </summary>
    /// <param name="outBuffer">The out buffer.</param>
    /// <param name="dataBlock">The data block.</param>
    /// <param name="dataBlockSample">The data block sample.</param>
    /// <param name="fileSampleNr">The file sample nr.</param>
    /// <param name="smoothReset">if set to <c>true</c> [smooth reset].</param>
    /// <param name="resetAtJumps">if set to <c>true</c> [reset at jumps].</param>
    private void MCSmooth_DoSmooth_Backward(short[] outBuffer, int dataBlock, int dataBlockSample, int fileSampleNr, ref bool smoothReset, bool resetAtJumps)
    {
      int n;

      int idataBlock = dataBlock;
      int idataBlockSample = dataBlockSample;
      int ifileSampleNr = fileSampleNr;

      if (resetAtJumps)
      {
        if (smoothReset)
        {
          LastMCJump.Processed = true;
          LastMCJump.SampleNr = fileSampleNr;
          LastMCJump.Size = MCjumpThreshold;
        }
        double mcJump = outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCjump] + dataBlockSample];
        if (Math.Abs(mcJump) >= Math.Abs(LastMCJump.Size))
        {
          LastMCJump.Processed = false;
          LastMCJump.SampleNr = fileSampleNr;
          LastMCJump.Size = mcJump;
        }
        if (!LastMCJump.Processed)
        {
          int m = LastMCJump.SampleNr;
          if (((m - fileSampleNr) >= MinSamplesBetweenJumps) || ((mcJump / LastMCJump.Size) < 0))
          {
            // Jump complete: initialize its processing
            // Save current samplerec with any earlier processed jumps
            OutputEDFFile.WriteDataBlock(dataBlock);
            // Get 'past' (at n) reset values from smoother
            n = Math.Max(0, m - MaxSamplesHalfJump);
            idataBlock = Math.DivRem(n, MCsignalsBlockSamples, out idataBlockSample);
            OutputEDFFile.ReadDataBlock(idataBlock);
            // Reset backward smoother to 'past' forward-smoothed state
            // TODO: Check if next line is still valid for the case of MCEventDuration > 1
            _suBack[0] = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SUplus] + idataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA); // SU+ 
            _ssBack[0] = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SSplus] + idataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA); // SS+
            SUsmooth = _suBack[0];
            SSsmooth = _ssBack[0];
            // Go to jump (at m) but preserve 1 sample of the jump
            ifileSampleNr = Math.Max(fileSampleNr, m - 1); // smoother will start at ifileSampleNr = LastJump - 1
            idataBlock = Math.DivRem(ifileSampleNr, MCsignalsBlockSamples, out idataBlockSample);
            OutputEDFFile.ReadDataBlock(idataBlock);
            LastMCJump.Processed = true;
            LastMCJump.SampleNr = fileSampleNr;
            LastMCJump.Size = MCjumpThreshold;
          }
        }
      }

      // Reset jump-sample counter
      n = Math.Min(fileSampleNr - ifileSampleNr, MaxSamplesHalfJump);

      while (ifileSampleNr >= fileSampleNr)
      {
        if (idataBlockSample == -1)
        {
          OutputEDFFile.WriteDataBlock(idataBlock);
          idataBlock--;
          OutputEDFFile.ReadDataBlock(idataBlock);
          idataBlockSample = MCsignalsBlockSamples - 1;
        }

        bool artifact = ((outBuffer[OutputBufferOffset[MCOutputSignalIndex.HFart] + idataBlockSample] > 0) ||
                         (outBuffer[OutputBufferOffset[MCOutputSignalIndex.LFart] + idataBlockSample] > 0) ||
                         (outBuffer[OutputBufferOffset[MCOutputSignalIndex.MissingSignal] + idataBlockSample] > 0) ||
                         (Math.Abs(outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCevent] + idataBlockSample]) > MCEventThreshold));

        if ((resetAtJumps) && (n > 0))
        {
          artifact = true;
          n--;
        }

        // SU and SS back-smoothed into SU- and SS-
        double s;
        double r;
        MCSmooth_SmoothSUSS(MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SU] + idataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA),
            MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SS] + idataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA),
            out r, out s, artifact, smoothReset);
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.SUminus] + idataBlockSample] = MathEx.LogFloat(r, AppConf.LogFloatY0, AppConf.LogFloatA);
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.SSminus] + idataBlockSample] = MathEx.LogFloat(s, AppConf.LogFloatY0, AppConf.LogFloatA);
        smoothReset = false;
        _suBack[1] = _suBack[0];
        _ssBack[1] = _ssBack[0];
        _suBack[0] = r;
        _ssBack[0] = s;
        _suForw[0] = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SUplus] + idataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA);
        _ssForw[0] = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SSplus] + idataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA);

        // Combine for -and backwards to symmetric SSP, SS0, MC and MCjump
        double ssp = _ssForw[0] + _ssBack[1];
        double mc;
        if (ssp <= 0)
          mc = 0;
        else
          mc = (_suBack[1] + _suForw[0]) / ssp;
        ssp = ssp / 2;
        double ss0 = ssp * (1 - mc);
        mc = Range.EnsureRange(AppConf.MicGain * 100 * mc, -short.MaxValue, short.MaxValue);

        // Put symmetric results to disk
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.SSP] + idataBlockSample] = MathEx.LogFloat(ssp, AppConf.LogFloatY0, AppConf.LogFloatA);
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.SS0] + idataBlockSample] = MathEx.LogFloat(ss0, AppConf.LogFloatY0, AppConf.LogFloatA);
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.MC] + idataBlockSample] = (short)MathEx.RoundNearest(mc);
        ifileSampleNr--;
        idataBlockSample--;
      }
    }

    /// <summary>
    /// Smooths forward.
    /// </summary>
    /// <param name="outBuffer">The out buffer.</param>
    /// <param name="dataBlock">The data block.</param>
    /// <param name="dataBlockSample">The data block sample.</param>
    /// <param name="fileSampleNr">The file sample nr.</param>
    /// <param name="smoothReset">if set to <c>true</c> reset the smoother.</param>
    /// <param name="resetAtJumps">if set to <c>true</c> reset at jumps.</param>
    private void MCSmooth_DoSmooth_Forward(IList<short> outBuffer, int dataBlock, int dataBlockSample, int fileSampleNr, ref bool smoothReset, bool resetAtJumps)
    {
      double mcJump;
      int n;

      int idataBlock = dataBlock;
      int idataBlockSample = dataBlockSample;
      int ifileSampleNr = fileSampleNr;

      if (resetAtJumps)
      {
        if (smoothReset)
        {
          LastMCJump.Processed = true;
          LastMCJump.SampleNr = fileSampleNr;
          LastMCJump.Size = MCjumpThreshold;
        }
        mcJump = outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCjump] + dataBlockSample];
        if (Math.Abs(mcJump) >= Math.Abs(LastMCJump.Size))
        {
          LastMCJump.Processed = false;
          LastMCJump.SampleNr = fileSampleNr;
          LastMCJump.Size = mcJump;
        }
        if (!LastMCJump.Processed)
        {
          int m = LastMCJump.SampleNr;
          if (((fileSampleNr - m) >= MinSamplesBetweenJumps) || ((mcJump / LastMCJump.Size) < 0))
          {
            // Jump complete: initialize its processing
            // Save current samplerec with any earlier processed jumps
            OutputEDFFile.WriteDataBlock(dataBlock);
            // Get 'future' (at n) resetvalues from smoother
            n = Math.Min(m + MaxSamplesHalfJump, MCsignalsFileSamples - 1);
            idataBlock = Math.DivRem(n, MCsignalsBlockSamples, out idataBlockSample);
            OutputEDFFile.ReadDataBlock(idataBlock);
            // Reset forward smoother to 'future' back-smoothed state
            SUsmooth = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SUminus] + idataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA);
            SSsmooth = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SSminus] + idataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA);
            // Go to jump (at m) but preserve 1 sample of the jump
            ifileSampleNr = Math.Min(m, fileSampleNr); // smoother will start at ifileSampleNr = LastJump + 1
            idataBlock = Math.DivRem(ifileSampleNr, MCsignalsBlockSamples, out idataBlockSample);
            OutputEDFFile.ReadDataBlock(idataBlock);
            LastMCJump.Processed = true;
            LastMCJump.SampleNr = fileSampleNr;
            LastMCJump.Size = MCjumpThreshold;
          }
        }
      }
      n = Math.Min(fileSampleNr - ifileSampleNr, MaxSamplesHalfJump); // Reset jump-sample counter
      while (ifileSampleNr <= fileSampleNr)
      {
        if (idataBlockSample == MCsignalsBlockSamples)
        {
          OutputEDFFile.WriteDataBlock(idataBlock);
          idataBlock++;
          OutputEDFFile.ReadDataBlock(idataBlock);
          idataBlockSample = 0;
        }
        bool artifact = ((outBuffer[OutputBufferOffset[MCOutputSignalIndex.HFart] + idataBlockSample] > 0) || (outBuffer[OutputBufferOffset[MCOutputSignalIndex.LFart] + idataBlockSample] > 0) || (outBuffer[OutputBufferOffset[MCOutputSignalIndex.MissingSignal] + idataBlockSample] > 0) || (Math.Abs(outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCevent] + idataBlockSample]) > MCEventThreshold));
        if ((resetAtJumps) && (n > 0))
        {
          artifact = true;
          n--;
        }
        // SU and SS forward-smoothed into SU+ and SS+
        double s;
        double r;
        MCSmooth_SmoothSUSS(MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SU] + idataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA),
            MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SS] + idataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA),
            out r, out s, artifact, smoothReset);
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.SUplus] + idataBlockSample] = MathEx.LogFloat(r, AppConf.LogFloatY0, AppConf.LogFloatA);
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.SSplus] + idataBlockSample] = MathEx.LogFloat(s, AppConf.LogFloatY0, AppConf.LogFloatA);
        smoothReset = false;
        _suForw[1] = _suForw[0];
        _ssForw[1] = _ssForw[0];
        _suBack[0] = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SUminus] + idataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA);
        _ssBack[0] = MathEx.ExpInteger(outBuffer[OutputBufferOffset[MCOutputSignalIndex.SSminus] + idataBlockSample], AppConf.LogFloatY0, AppConf.LogFloatA);
        _suForw[0] = r;
        _ssForw[0] = s;
        // This MCjump is biased but OK for detection of jumps
        double ssp = _ssForw[1] + _ssBack[0];
        if (ssp <= 0)
          mcJump = 0;
        else
          mcJump = (_suBack[0] - _suForw[1]) / ssp;
        /*
          The below computation of MCjump is more correct but the
          variance of the result strongly depends on MC. Therefore
          the threshold should adapt to this variance: a bridge too far
          r6:=1.0*SSforw[1]*SSback[0];
          if(r6 < 1.0) then
            MCjump:=0.0
          else
            MCjump:=(1.0*SUback[0]*SSforw[1]-SUforw[1]*SSback[0])/r6;
         */

        /*MCjump = MicGain * 100 * MCjump;
        if (Math.Abs(MCjump) > short.MaxValue)
            MCjump = Math.Sign(MCjump) * short.MaxValue;*/
        mcJump = Range.EnsureRange(AppConf.MicGain * 100 * mcJump, -short.MaxValue, short.MaxValue);
        outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCjump] + idataBlockSample] = (short)MathEx.RoundNearest(mcJump);
        ifileSampleNr++;
        idataBlockSample++;
      }
    }

    /// <summary>
    /// Resets all outfile samples (set to 0) at given index.
    /// </summary>
    /// <param name="outBuffer">The out buffer.</param>
    /// <param name="sampleIndex">The index of the sample to be cleared.</param>
    private void MCSmooth_ResetAll(IList<short> outBuffer, int sampleIndex)
    {
      outBuffer[OutputBufferOffset[MCOutputSignalIndex.SUplus] + sampleIndex] = 0;
      outBuffer[OutputBufferOffset[MCOutputSignalIndex.SUminus] + sampleIndex] = 0;
      outBuffer[OutputBufferOffset[MCOutputSignalIndex.SSplus] + sampleIndex] = 0;
      outBuffer[OutputBufferOffset[MCOutputSignalIndex.SSminus] + sampleIndex] = 0;
      outBuffer[OutputBufferOffset[MCOutputSignalIndex.SSP] + sampleIndex] = 0;
      outBuffer[OutputBufferOffset[MCOutputSignalIndex.SS0] + sampleIndex] = 0;

      // todo: Bob controleren HFart, LFart en Artzero ipv 0?

      outBuffer[OutputBufferOffset[MCOutputSignalIndex.HFart] + sampleIndex] = (short)MathEx.RoundNearest(ArtHF / ArtifactPhysicalDimensionResolution);
      outBuffer[OutputBufferOffset[MCOutputSignalIndex.LFart] + sampleIndex] = (short)MathEx.RoundNearest(ArtLF / ArtifactPhysicalDimensionResolution);
      outBuffer[OutputBufferOffset[MCOutputSignalIndex.MissingSignal] + sampleIndex] = (short)MathEx.RoundNearest(ArtZero / ArtifactPhysicalDimensionResolution);

      outBuffer[OutputBufferOffset[MCOutputSignalIndex.MC] + sampleIndex] = 0;
      outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCjump] + sampleIndex] = 0;
      outBuffer[OutputBufferOffset[MCOutputSignalIndex.MCevent] + sampleIndex] = 0;
    }

    /// <summary>
    /// Recursively smooths SUin and SSin
    /// </summary>
    /// <param name="SUin">The SU input value.</param>
    /// <param name="SSin">The SS input value.</param>
    /// <param name="SUout">The SU output value.</param>
    /// <param name="SSout">The SS output value.</param>
    /// <param name="artifact">if set to <c>true</c> artifact is present.</param>
    /// <param name="smootherReset">if set to <c>true</c> reset smoother.</param>
    private void MCSmooth_SmoothSUSS(double SUin, double SSin, out double SUout, out double SSout, bool artifact, bool smootherReset)
    {
      // Recursive smoother of SUin and SSin by updating internal variable by 'smoothrate'
      // Lowlimits for output SUout and SSout are 0 and piB respectively
      // SU and SS keep the internal state of smooth with double precision
      if (smootherReset)
      {
        SUsmooth = 0;
        SSsmooth = PiBxx;
      }
      if (artifact)
      {
        SUsmooth = (1 - AppConf.SmoothRate) * SUsmooth;
        SSsmooth = (1 - AppConf.SmoothRate) * SSsmooth;
        /* Be carefull with changing the following lower limit on SS. These influences
         * the zero-signal detection by ArtZero and XpiBArtZero
         */
        if (SSsmooth < PiBxx)
          SSsmooth = PiBxx;
      }
      else
      {
        double dSU;
        if (SUin > -PiBxx)
          dSU = SUin - SUsmooth;
        else
          dSU = -PiBxx - SUsmooth; // Clip SUin at -piB: mitigate artifacts
        double dss = SSin - SSsmooth;
        SUsmooth = Math.Max(0, SUsmooth + AppConf.SmoothRate * dSU);
        SSsmooth = SSsmooth + AppConf.SmoothRate * dss;
        /* Be carefull with changing the following lower limit on SS. These influences
         * the zero-signal detection d.m.v. ArtZero and XpiBArtZero
         */
        if (SSsmooth < PiBxx)
          SSsmooth = PiBxx;
      }
      SUout = SUsmooth;
      SSout = SSsmooth;
    }

    /// <summary>
    /// Update the HF, LF and missing-signal artifacts.
    /// </summary>
    /// <param name="smoothReset">if set to <c>true</c> reset the smoother.</param>
    /// <param name="ss">The SS.</param>
    /// <param name="su">The SU.</param>
    private void MCSmooth_UpdateArtifacts(bool smoothReset, double ss, double su)
    {
      if (smoothReset)
      {
        ArtHF = 0;
        ArtLF = 0;
        ArtZero = 0;
      }

      double artFactor = (ss - su - PiBxx) / PiBxx;
      artFactor = Range.EnsureRange(artFactor, -1000, 1000); // Avoid overflow of ArtHF

      if (artFactor >= AppConf.XpiBPlus) // XpiBPlus >= 1

        // todo: Bob controleren ArtHF, ArtLF en ArtZero: eerst afronden daarna *SmoothTime ?

        ArtHF += MathEx.RoundNearest(artFactor / AppConf.XpiBPlus) * AppConf.SmoothTime;

      //ArtHF += (short)MathEx.RoundNearest(artFactor / AppConf.XpiBPlus);
      else
      {
        //ArtHF--;
        ArtHF -= AppConf.SmoothTime;
      }

      //ArtHF = (short)Range.EnsureRange(ArtHF, 0, AppConf.ArtMaxSeconds);
      ArtHF = Range.EnsureRange(ArtHF, 0, AppConf.ArtMaxSeconds);

      if (artFactor <= AppConf.XpiBMinus) //XpiBMinus <= -1

        ArtLF += MathEx.RoundNearest(artFactor / AppConf.XpiBMinus) * AppConf.SmoothTime;
      //ArtLF += (short)MathEx.RoundNearest(artFactor / AppConf.XpiBMinus);
      else
      {
        //ArtLF--;
        ArtLF -= AppConf.SmoothTime;
      }

      //ArtLF = (short)Range.EnsureRange(ArtLF, 0, AppConf.ArtMaxSeconds);
      ArtLF = Range.EnsureRange(ArtLF, 0, AppConf.ArtMaxSeconds);

      if (ss <= PiBxx / AppConf.XpiBZero) //XpiBArtZero >= 1
      {
        //ArtZero++;
        ArtZero += (short)MathEx.RoundNearest((PiBxx / AppConf.XpiBZero) - ss) * AppConf.SmoothTime;
      }
      else
      {
        //ArtZero--;
        ArtZero -= AppConf.SmoothTime;
      }
      //ArtZero = (short)Range.EnsureRange(ArtZero, 0, Math.Min(1, AppConf.SmoothTime));
      ArtZero = Range.EnsureRange(ArtZero, 0, Math.Min(1, AppConf.SmoothTime));
    }

    #endregion Event handlers

    #region Private Methods

    /// <summary>
    /// Does the actual analysis.
    /// Copy input signal, perform SU and SS reduction, compute piB, detecting artifacts, 
    /// smooth ss and su, detect events, detect jumps and calculate final gains.
    /// </summary>
    /// <returns><c>true</c> if successful</returns>
    private bool DoAnalysis()
    {
      AppError.Clear();

      ProgressBarForm pbf = new ProgressBarForm
      {
        ShowInTaskbar = false,
        Progress = { Minimum = 0, Maximum = 100, Step = 1, Value = 0 },
        Message = "Reading input signal..."
      };
      pbf.Show();
      Application.DoEvents();

      bool result = true;
      try
      {
        // Copy input signal to output file if requested.
        if (AppConf.CopyInputSignal)
          result = TranslateInputSignal();

        pbf.Progress.Value = 5;
        Application.DoEvents();

        if (result)
        {
          pbf.Message = "Performing SU and SS reduction...";
          Application.DoEvents();
          result = DoSSSUReduction();
          pbf.Progress.Value = 30;
          Application.DoEvents();
        }

        if (result)
        {
          pbf.Message = "Computing PiB value...";
          Application.DoEvents();
          result = DoDetectPiB();
          pbf.BringToFront();
          pbf.Progress.Value = 50;
          Application.DoEvents();
        }

        if (result)
        {
          pbf.Message = "Detecting artifacts...";
          Application.DoEvents();
          result = DoComputeArtifactTraces();
          pbf.Progress.Value = 60;
          Application.DoEvents();
        }
        if (result)
        {
          pbf.Message = "Smoothing SU and SS...";
          Application.DoEvents();
          result = DoSmoothSSSU();
          pbf.Progress.Value = 70;
          Application.DoEvents();
        }
        if (result)
        {
          pbf.Message = "Detecting events...";
          Application.DoEvents();
          result = DoDetectMCEvents();
          pbf.Progress.Value = 80;
          Application.DoEvents();
        }
        if (result)
        {
          pbf.Message = "Re-smoothing signals and detecting jumps...";
          Application.DoEvents();
          // Re-smooth SS and SU rejecting MC events
          result = DoResmoothSSSU();
          pbf.Progress.Value = 90;
          Application.DoEvents();
        }
        if (result)
        {
          pbf.Message = "Computing final gains...";
          Application.DoEvents();
          result = DoComputeMC();
          pbf.Progress.Value = 100;
          Application.DoEvents();
        }
      }
      catch (Exception e)
      {
        AppError.Add(e.Message, NeuroLoopGainController.DefaultErrorMessageId);
      }

      pbf.Close();

      // Close EDF files
      InputEDFFile.Active = false;
      OutputEDFFile.Active = false;

      return (result && !AppError.Signaled);
    }

    /// <summary>
    /// Writes the artifact traces signal info to the output EDF file and computes the artifact traces.
    /// </summary>
    /// <returns><c>true</c> if successful</returns>
    private bool DoComputeArtifactTraces()
    {
      try
      {
        //TODO: Review this

        // Update filtering header information for artifact traces
        OutputEDFFile.SignalInfo[MCOutputSignalIndex.HFart].PreFilter = string.Format("Detected when [[SS-SU-piB] div piB]" +
        ">={0:#.###} with physical piB={1:#.##}", AppConf.XpiBPlus, PiBxx);
        OutputEDFFile.SignalInfo[MCOutputSignalIndex.HFart].PhysiDim = string.Format(CultureInfo.InvariantCulture, "at {0:#.#}Hz", OutputEDFFile.FileInfo.SampleRecDuration / OutputEDFFile.SignalInfo[MCOutputSignalIndex.HFart].NrSamples);

        OutputEDFFile.SignalInfo[MCOutputSignalIndex.LFart].PreFilter = string.Format("Detected when [[SS-SU-piB] div piB]" +
        "<={0:#.###} with physical piB={1:#.##}", AppConf.XpiBMinus, PiBxx);
        OutputEDFFile.SignalInfo[MCOutputSignalIndex.LFart].PhysiDim = string.Format(CultureInfo.InvariantCulture, "at {0:#.#}Hz", OutputEDFFile.FileInfo.SampleRecDuration / OutputEDFFile.SignalInfo[MCOutputSignalIndex.LFart].NrSamples);

        OutputEDFFile.SignalInfo[MCOutputSignalIndex.MissingSignal].PreFilter = string.Format("Detected when [piB div [SS]] >={0:#.###} " +
        "with physical piB={1:#.##}", AppConf.XpiBZero, PiBxx);
        OutputEDFFile.SignalInfo[MCOutputSignalIndex.MissingSignal].PhysiDim = string.Format(CultureInfo.InvariantCulture, "at {0:#.#}Hz", OutputEDFFile.FileInfo.SampleRecDuration / OutputEDFFile.SignalInfo[MCOutputSignalIndex.MissingSignal].NrSamples);

        OutputEDFFile.CommitChanges();

        MCSmooth(SmoothOption.GetArtifactsResetAll);
      }
      catch (Exception e)
      {
        AppError.Add(e.Message, NeuroLoopGainController.DefaultErrorMessageId);
      }

      return !AppError.Signaled;
    }

    /// <summary>
    /// Computes SSP, SS0 and MC
    /// </summary>
    /// <returns><c>true</c> if successful</returns>
    private bool DoComputeMC()
    {
      try
      {
        // Computing SSP, SS0 and MC...

        /* 
         * Calling 3 times at this procedure seems stupid at first sight,
         * but in the middle of the procedure MCJumps is recalculated, so
         * next call will use a different MCjumps signal to recompute all
         * the other..so...keep it???
         */
        MCSmooth(SmoothOption.SmoothResetAtJumps);
        MCSmooth(SmoothOption.SmoothResetAtJumps);
        MCSmooth(SmoothOption.SmoothResetAtJumps);
      }
      catch (Exception e)
      {
        AppError.Add(e.Message, NeuroLoopGainController.DefaultErrorMessageId);
      }

      return !AppError.Signaled;
    }

    /// <summary>
    /// Update header information for SSP, SS0, MC, MCJump and MCEvent.
    /// Smooths using option DetectEventsResetJumps.
    /// </summary>
    /// <returns><c>true</c> if successful</returns>
    private bool DoDetectMCEvents()
    {
      try
      {
        // Update header information for SSP, SS0, MC, MCJump and MCEvent

        //SSP
        EdfSignalInfoBase signalInfo = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SSP];
        signalInfo.TransducerType = string.Format(CultureInfo.InvariantCulture, "{0} Smootherrate={1:0.######}/s", signalInfo.TransducerType, AppConf.SmoothRate);
        signalInfo.PreFilter = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SS].PreFilter;

        //SS0
        signalInfo = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SS0];
        signalInfo.TransducerType = string.Format(CultureInfo.InvariantCulture, "{0} Smootherrate={1:0.######}/s", signalInfo.TransducerType, AppConf.SmoothRate);
        signalInfo.PreFilter = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SS].PreFilter;

        //MC (Gain)
        signalInfo = OutputEDFFile.SignalInfo[MCOutputSignalIndex.MC];
        signalInfo.TransducerType = string.Format(CultureInfo.InvariantCulture, "{0} Smootherrate={1:0.######}/s", signalInfo.TransducerType, AppConf.SmoothRate);
        signalInfo.PreFilter = string.Format("Microcontinuity (Method B.Kemp) physical piB={0:.##}", PiBxx);

        //MCJump (GainJump)
        signalInfo = OutputEDFFile.SignalInfo[MCOutputSignalIndex.MCjump];
        signalInfo.TransducerType = string.Format(CultureInfo.InvariantCulture, "{0} Smootherrate={1:0.######}/s", signalInfo.TransducerType, AppConf.SmoothRate);
        signalInfo.PreFilter = "Microcontinuity jump = 100 * [[SU-/SS-] - [SU+/SS+]]";

        //MCEvent (GainEvent)
        signalInfo = OutputEDFFile.SignalInfo[MCOutputSignalIndex.MCevent];
        signalInfo.TransducerType = string.Format(CultureInfo.InvariantCulture, "{0} Smootherrate={1:0.######}/s", signalInfo.TransducerType, AppConf.SmoothRate);
        /*
        signalInfo.PreFilter = string.Format("Microcontinuity event <= {0:##}s", appConf.MCEventDuration);
        */
        //TODO: Review the following in order to allow higher output sampling rate
        signalInfo.PreFilter = string.Format(CultureInfo.InvariantCulture, "Microcontinuity event <= {0:##}s", AppConf.MCEventDuration);

        OutputEDFFile.CommitChanges();

        MCSmooth(SmoothOption.DetectEventsResetJumps);
      }
      catch (Exception e)
      {
        AppError.Add(e.Message, NeuroLoopGainController.DefaultErrorMessageId);
      }

      return !AppError.Signaled;
    }

    /// <summary>
    /// Detects the piB value.
    /// </summary>
    /// <returns><c>true</c> if successful</returns>
    private bool DoDetectPiB()
    {
      int[] sssu = new int[AppConf.SS_SUmax - AppConf.SS_SUmin + 1];
      double[] sssuSmoothed = new double[AppConf.SS_SUmax - AppConf.SS_SUmin + 1];
      double[] sssuTemplate = new double[AppConf.piBCorrelationFunctionBufferSize];
      double[] sssuMatch = new double[AppConf.SS_SUmax - AppConf.SS_SUmin + 1];
      double x;
      double value;

      sssuSmoothed.Fill(0);
      sssuTemplate.Fill(0);
      sssuMatch.Fill(0);

      int dataBlockSamples = OutputEDFFile.SignalInfo[1].NrSamples;

      // Create SS-SU histogram
      for (int k = 0; k < OutputEDFFile.FileInfo.NrDataRecords; k++)
      {
        OutputEDFFile.ReadDataBlock(k);

        for (int k1 = 0; k1 < dataBlockSamples; k1++)
        {
          double su = MathEx.ExpInteger(OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.SU] + k1], AppConf.LogFloatY0, AppConf.LogFloatA);
          double ss = MathEx.ExpInteger(OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.SS] + k1], AppConf.LogFloatY0, AppConf.LogFloatA);

          // Do not add zeros from unrecorded end of file
          if ((Math.Abs(su) >= AppConf.LogFloatY0) || (Math.Abs(ss) >= AppConf.LogFloatY0))
          {
            short j = MathEx.LogFloat(ss - su, AppConf.LogFloatY0, AppConf.LogFloatA);
            //Watch it! SS_SUmin and SS_SUmax now refer to log-transformed values
            if (Range.InRange(j, AppConf.SS_SUmin, AppConf.SS_SUmax) && (sssu[j - AppConf.SS_SUmin] < int.MaxValue) && (j != 0))
              sssu[j - AppConf.SS_SUmin]++;
          }
        }
      }

      /*
       * 2*SS_SUsmoother applied to the logarithmic converted values in the histogram 
       * corresponds to the original values of piBPeakWidth
       */
      int sssuSmootherWidth = (int)Range.EnsureRange(Math.Truncate(Math.Log(1.0 + AppConf.piBPeakWidth) / AppConf.LogFloatA / 2), -int.MaxValue, int.MaxValue);

      // Apply the smoothing (mean filter) to the histogram
      for (int k = sssuSmootherWidth; k <= AppConf.SS_SUmax - AppConf.SS_SUmin - sssuSmootherWidth; k++)
      {
        for (int k1 = k - sssuSmootherWidth; k1 <= k + sssuSmootherWidth; k1++)
        {
          sssuSmoothed[k] += sssu[k1];
        }
        sssuSmoothed[k] = sssuSmoothed[k] / (2 * sssuSmootherWidth + 1);
      }

      // todo: Marco: "gewoon" maximum zoeken, geen moeilijke functies.

      // Construct the template to detect desired piB value peak in the smoothed histogram
      for (int k = 1; k < sssuTemplate.Length - 1; k++)
      {
        x = ((double)k / (sssuTemplate.Length - 1)) * 3 * Math.PI - 2 * Math.PI;
        value = 2.0 / 3 * (-0.5894) * (MathEx.Heav(x + 2 * Math.PI) - MathEx.Heav(x + (Math.PI / 2))) +
            (MathEx.Heav(x + (Math.PI / 2)) - MathEx.Heav(x)) * (Math.Sin(2 * x) / (2 * x)) +
            (MathEx.Heav(x) - MathEx.Heav(x - Math.PI / 2)) * (Math.Sin(2 * x) / (2 * x));
        sssuTemplate[k] = value;
      }

      // Calculate and substract template mean value and calculate resulting template's peak index
      x = MathEx.Average(sssuTemplate);
      value = 0;
      int templatePeakIdx = 0;
      for (int k = 0; k < sssuTemplate.Length - 1; k++)
      {
        sssuTemplate[k] -= x;
        if (sssuTemplate[k] > value)
        {
          templatePeakIdx = k;
          value = sssuTemplate[k];
        }
      }

      // Calculate correlation coefficients by shifting the template over the histogram
      for (int k = 0; k < (sssuSmoothed.Length - sssuTemplate.Length); k++)
      {
        value = 0;
        for (int k1 = 0; k1 < sssuTemplate.Length; k1++)
          value += sssuTemplate[k1] * sssuSmoothed[k + k1];
        sssuMatch[k + templatePeakIdx] = value;
      }

      LogPiBxx = 0;
      double peak = 0;
      // We'll take Pi*B as the x-value for the maximum correlation point
      for (int k = 0; k < sssuMatch.Length; k++)
      {
        if (sssuMatch[k] > peak)
        {
          peak = sssuMatch[k];
          //piB = (short)Range.EnsureRange(k + appConf.SS_SUmin, -short.MaxValue, short.MaxValue);
          LogPiBxx = (short)(k + AppConf.SS_SUmin);
        }
      }


      // Show histogram
      if (AppConf.ShowPiBHistogram)
      {
        HistogramInfo histogramInfo = new HistogramInfo
        {
          FileName = InputEDFFileName,
          SignalLabel = InputEDFFile.SignalInfo[InputSignalSelected].SignalLabel,
          SS_SUmax = AppConf.SS_SUmax,
          SS_SUmin = AppConf.SS_SUmin,
          SU_SS = sssu,
          SU_SSsmoothed = sssuSmoothed,
          UnderSampling = AppConf.IIRUnderSampler,
          SU_SSmatch = sssuMatch,
          F0 = AppConf.F0,
          FC = AppConf.FC,
          B = AppConf.BandWidth,
          PiBvalueLog = LogPiBxx,
          PiBvaluePhysi = PiBxx,
          SmoothRate = AppConf.SmoothRate,
          SmoothTime = AppConf.SmoothTime
        };

        FormPiBHistogram formHistogram = new FormPiBHistogram();
        formHistogram.SetHistogramInfo(histogramInfo);
        formHistogram.Show();
      }

      return !AppError.Signaled;
    }

    /// <summary>
    /// The same as DoSmoothSSSU but without re-establishing the header
    /// </summary>
    /// <returns><c>true</c> if successful</returns>
    private bool DoResmoothSSSU()
    {
      OutputEDFFile.CommitChanges(); // Commit pending changes from doDetectMCEvents

      return MCSmooth(SmoothOption.Smooth);
    }

    /// <summary>
    /// Sets the outputfile info for the SU+, SU-, SS+, SS- signals
    /// and smooths the signals using the Smooth option.
    /// </summary>
    /// <returns><c>true</c> if successful</returns>
    private bool DoSmoothSSSU()
    {
      try
      {
        // Update header information for SU+, SU-, SS+, SS- 

        //SU+
        EdfSignalInfoBase signalInfo = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SUplus];
        signalInfo.TransducerType = string.Format(CultureInfo.InvariantCulture, "{0} Smootherrate={1:0.######}/s", signalInfo.TransducerType, AppConf.SmoothRate);
        signalInfo.PreFilter = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SS].PreFilter;

        //SU-
        signalInfo = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SUminus];
        signalInfo.TransducerType = string.Format(CultureInfo.InvariantCulture, "{0} Smootherrate={1:0.######}/s", signalInfo.TransducerType, AppConf.SmoothRate);
        signalInfo.PreFilter = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SS].PreFilter;

        //SS+
        signalInfo = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SSplus];
        signalInfo.TransducerType = string.Format(CultureInfo.InvariantCulture, "{0} Smootherrate={1:0.######}/s", signalInfo.TransducerType, AppConf.SmoothRate);
        signalInfo.PreFilter = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SS].PreFilter;

        //SS-
        signalInfo = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SSminus];
        signalInfo.TransducerType = string.Format(CultureInfo.InvariantCulture, "{0} Smootherrate={1:0.######}/s", signalInfo.TransducerType, AppConf.SmoothRate);
        signalInfo.PreFilter = OutputEDFFile.SignalInfo[MCOutputSignalIndex.SS].PreFilter;

        OutputEDFFile.CommitChanges();

        MCSmooth(SmoothOption.Smooth);
      }
      catch (Exception e)
      {
        AppError.Add(e.Message, NeuroLoopGainController.DefaultErrorMessageId);
      }

      return !AppError.Signaled;
    }

    /// <summary>
    /// Does the SSSU reduction (calculates the SS and SU signals using the input signal).
    /// </summary>
    /// <returns><c>true</c> if successful</returns>
    private bool DoSSSUReduction()
    {
      if (DoComputeDUSE_InitializeVariables() &&
          DoComputeDU_InitializeFilter() &&
          DoComputeSE_InitializeFilter())
      {
        int inputSamples = InputEDFFile.SignalInfo[InputSignalSelected].NrSamples;

        double[] IIR_Input;
        double[] IIR_OutDU;
        double[] IIR_OutSE;
        if (AppConf.IIRUnderSampler == 1)
        {
          IIR_Input = new double[inputSamples];
          IIR_OutDU = new double[inputSamples];
          IIR_OutSE = new double[inputSamples];
        }
        else
        {
          IIR_Input = new double[(inputSamples / AppConf.IIRUnderSampler) + 1];
          IIR_OutDU = new double[(inputSamples / AppConf.IIRUnderSampler) + 1];
          IIR_OutSE = new double[(inputSamples / AppConf.IIRUnderSampler) + 1];
        }

        IIR_OutDU.Fill(0);
        IIR_OutSE.Fill(0);

        int outRecSample = 0;
        int underSample = 1; // underSample: 1..IIR_UnderSampler
        double SSactualSample = 0;
        double SUactualSample = 0;
        double integratedTime = 0;
        int integrateCount = 0;
        bool oneOutputSampleObtained = false;

        // Processing input block by block
        int totalRecords = InputEDFFile.FileInfo.NrDataRecords;
        int currentOutputRecord = 0;
        if (AppConf.CopyInputSignal)
        {
          OutputEDFFile.ReadDataBlock(currentOutputRecord);
        }
        for (int k = 0; k < totalRecords; k++)
        {
          // Reading input block to the buffer
          InputEDFFile.ReadDataBlock(k);

          IIR_Input.Fill(0);
          int IIR_LastSample = 0;

          // Apply the undersampling; result stored in IIR_Input, index [0...IIR_LastSample]
          for (int k1 = 0; k1 < inputSamples; k1++)
          {
            if (underSample == AppConf.IIRUnderSampler)
            {
              IIR_Input[IIR_LastSample] =
                    InputEDFFile.SignalInfo[InputSignalSelected].DigiToPhysi(InputEDFFile.DataBuffer[InputBufferOffsets[InputSignalSelected] + k1]);
              underSample = 1;
              IIR_LastSample++;
            }
            else
              underSample++;
          }
          IIR_LastSample--;

          // Filter the undersampled input samples to IIR_OutDU and IIR_OutSE
          DUEfilter.FilterSamples(IIR_Input, IIR_OutDU, 0, IIR_LastSample, 0);
          SEfilter.FilterSamples(IIR_Input, IIR_OutSE, 0, IIR_LastSample, 0);

          // SUSSsmoothingTime secs integration into EDF output file
          for (int k1 = 0; k1 <= IIR_LastSample; k1++)
          {
            // Accumulate the output sample value
            SUactualSample += IIR_OutDU[k1] * IIR_OutSE[k1];
            SSactualSample += Math.Pow(IIR_OutSE[k1], 2) * IIR_Delta;

            // Update the accumulated samples total time
            integratedTime += IIR_Delta;
            integrateCount++;

            // Check if we have acculmulated enough samples to write an output sample
            if (integratedTime >= AppConf.SmoothTime)
            {
              // Watch it!
              // Dividing by the integration interval implies that average powers are computed, not integrated energies.
              double integrationInterval = integrateCount * IIR_Delta;
              SUactualSample = SUactualSample / integrationInterval;
              SSactualSample = SSactualSample / integrationInterval;
              integrateCount = 0;
              integratedTime -= AppConf.SmoothTime;
              // Set flag to write the output samples
              oneOutputSampleObtained = true;
            }

            if (oneOutputSampleObtained)
            {
              /*
               LogFloat is used because:
                  IIR_Gain_du and IIR_Gain_s and INT_Gain (the derived INT_Gain_SU and INT_Gain_SS) are optimized for the years 1995-1997
                  slow-wave and analysis of sigma 12-bit ADC signals (from EEG devices like tape recorders within integer range (OutReal) account.
                  This range was therefore (+/-) 1 .. 32767. 16-bit signals (EDF) and other model and sampling frequencies can give up to 16 * 16 * 100 times greater powers. 
                  8-bit signals (eg BrainSpy) and other model and sampling frequencies can give 16 * 16 * 100 times smaller powers. So the range is OutReal, in general,
                  about 1E-4 til 10000*1E+5. This range can be projected in EDF integers with an accuracy of 0.1% using a log-conversion 
                  (see Kemp et al, Journal of Sleep Research 1998 and also www.medfac.leidenuniv.nl/neurology/KNF/kemp/edffloat.htm). 
                  This has the advantage that analysis results can be viewed with a simple EDF viewer.
               */

              // Add the samples to the output databuffer and update the index
              OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.SU] + outRecSample] = MathEx.LogFloat(SUactualSample, AppConf.LogFloatY0, AppConf.LogFloatA);
              OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.SS] + outRecSample] = MathEx.LogFloat(SSactualSample, AppConf.LogFloatY0, AppConf.LogFloatA);
              outRecSample++;

              // If output buffer is full, write it to disk
              if (outRecSample == MCsignalsBlockSamples)
              {
                OutputEDFFile.WriteDataBlock(currentOutputRecord);
                outRecSample = 0;
                currentOutputRecord++;
                if ((currentOutputRecord < OutputEDFFile.FileInfo.NrDataRecords) && AppConf.CopyInputSignal)
                {
                  OutputEDFFile.ReadDataBlock(currentOutputRecord);
                }
              }

              // Initialise the output smaple accumulators and reset output sample available flag
              SUactualSample = 0;
              SSactualSample = 0;
              oneOutputSampleObtained = false;
            }
          } // SUSSsmoothingTime secs integration

        }// processing input block by block

        // Complete last output block with zeros and write it to the ouput file
        if (outRecSample != 0)
        {
          for (int k = outRecSample; k < MCsignalsBlockSamples; k++)
          {
            OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.SU] + k] = 0;
            OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.SS] + k] = 0;
          }
          OutputEDFFile.WriteDataBlock(currentOutputRecord);
        }

        // Commit EDF file changes (changed number of datablocks)
        OutputEDFFile.CommitChanges();
      }

      return !AppError.Signaled;
    }

    /// <summary>
    /// Smoothes the using the given option.
    /// </summary>
    /// <param name="option">The option.</param>
    /// <returns><c>true</c> if successful</returns>
    private bool MCSmooth(SmoothOption option)
    {
      bool smoothReset = true;
      int fileSampleNr = 0;

      try
      {
        if (MCSmooth_CheckSettings())
        {
          /*
          MinSamplesBetweenJumps = MathEx.RoundNearest(1 / appConf.SmoothRate) + 1;
          MaxSamplesHalfJump = (MinSamplesBetweenJumps / 20) + 1;
          MCjumpThreshold = appConf.MCjumpFind * 100 * appConf.MicGain;
          MCEventThreshold = MathEx.RoundNearest(appConf.MCEventReject * appConf.SmoothRate * 100 * appConf.MicGain);

          FSUforw = new double[appConf.MCEventDuration+1];
          ArrayExtensions.Fill(FSUforw, 0);
          FSUback = new double[appConf.MCEventDuration + 1];
          ArrayExtensions.Fill(FSUback, 0);
          FSSforw = new double[appConf.MCEventDuration + 1];
          ArrayExtensions.Fill(FSSforw, PiBExpInt);
          FSSback = new double[appConf.MCEventDuration + 1];
          ArrayExtensions.Fill(FSSback, PiBExpInt);
          */

          //TODO: Review the following modifications in order to allow higher output sampling rate
          MCEventDurationSamples = MathEx.RoundNearest(AppConf.MCEventDuration / AppConf.SmoothTime);
          MinSamplesBetweenJumps = MathEx.RoundNearest(1 / (AppConf.SmoothRate * AppConf.SmoothTime)) + 1;
          MaxSamplesHalfJump = (MinSamplesBetweenJumps / 20) + 1; // Bob's 'nose'
          MCjumpThreshold = (AppConf.MCjumpFind / AppConf.SmoothTime) * 100 * AppConf.MicGain;
          MCEventThreshold = MathEx.RoundNearest((AppConf.MCEventReject / AppConf.SmoothTime) * AppConf.SmoothRate * 100 * AppConf.MicGain);

          _suForw = new double[MCEventDurationSamples + 1];
          _suForw.Fill(0);
          _suBack = new double[MCEventDurationSamples + 1];
          _suBack.Fill(0);
          _ssForw = new double[MCEventDurationSamples + 1];
          _ssForw.Fill(PiBxx);
          _ssBack = new double[MCEventDurationSamples + 1];
          _ssBack.Fill(PiBxx);

          MCsignalsFileSamples = MCsignalsBlockSamples * OutputEDFFile.FileInfo.NrDataRecords;

          // Forward direction through EDF file
          double su;
          double ss;
          for (int k = 0; k < OutputEDFFile.FileInfo.NrDataRecords; k++)
          {
            OutputEDFFile.ReadDataBlock(k);

            for (int k1 = 0; k1 < MCsignalsBlockSamples; k1++)
            {
              su = MathEx.ExpInteger(OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.SU] + k1], AppConf.LogFloatY0, AppConf.LogFloatA);
              ss = MathEx.ExpInteger(OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.SS] + k1], AppConf.LogFloatY0, AppConf.LogFloatA);
              switch (option)
              {
                case SmoothOption.GetArtifactsResetAll:
                  // UpdateArtifacts uses SS,SU,SmoothReset,XpiBArt,ArtSpreadSamples
                  MCSmooth_UpdateArtifacts(smoothReset, ss, su);
                  MCSmooth_ResetAll(OutputEDFFile.DataBuffer, k1);
                  break;
                case SmoothOption.DetectEventsResetJumps:
                  MCSmooth_DetectEvents_ResetJumps(OutputEDFFile.DataBuffer, k1, true);
                  break;
                case SmoothOption.Smooth:
                  MCSmooth_DoSmooth_Forward(OutputEDFFile.DataBuffer, k, k1, fileSampleNr, ref smoothReset, false);
                  break;
                case SmoothOption.SmoothResetAtJumps:
                  MCSmooth_DoSmooth_Forward(OutputEDFFile.DataBuffer, k, k1, fileSampleNr, ref smoothReset, true);
                  break;
              }
              smoothReset = false;
              fileSampleNr++;
            }

            OutputEDFFile.WriteDataBlock(k);
          }
          // Backward direction through EDF file
          smoothReset = true;
          _suForw.Fill(0);
          _suBack.Fill(0);
          _ssForw.Fill(PiBxx);
          _ssBack.Fill(PiBxx);
          for (int k = OutputEDFFile.FileInfo.NrDataRecords - 1; k >= 0; k--)
          {
            OutputEDFFile.ReadDataBlock(k);

            for (int k1 = MCsignalsBlockSamples - 1; k1 >= 0; k1--)
            {
              fileSampleNr--;
              su = MathEx.ExpInteger(OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.SU] + k1], AppConf.LogFloatY0, AppConf.LogFloatA);
              ss = MathEx.ExpInteger(OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.SS] + k1], AppConf.LogFloatY0, AppConf.LogFloatA);
              switch (option)
              {
                case SmoothOption.GetArtifactsResetAll:
                  // UpdateArtifacts uses SS,SU,SmoothReset,XpiBArt,ArtSpreadSamples
                  MCSmooth_UpdateArtifacts(smoothReset, ss, su);

                  // todo: Bob controleren nauwkeurigheid

                  //OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.HFart] + k1] += ArtHF;
                  OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.HFart] + k1] += (short)MathEx.RoundNearest(ArtHF / ArtifactPhysicalDimensionResolution);


                  //OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.LFart] + k1] += ArtLF;
                  OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.LFart] + k1] += (short)MathEx.RoundNearest(ArtLF / ArtifactPhysicalDimensionResolution);

                  //OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.MissingSignal] + k1] += ArtZero;
                  OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.MissingSignal] + k1] += (short)MathEx.RoundNearest(ArtZero / ArtifactPhysicalDimensionResolution);

                  break;
                case SmoothOption.DetectEventsResetJumps:
                  MCSmooth_DetectEvents_ResetJumps(OutputEDFFile.DataBuffer, k1, false);
                  break;
                case SmoothOption.Smooth:
                  MCSmooth_DoSmooth_Backward(OutputEDFFile.DataBuffer, k, k1, fileSampleNr, ref smoothReset, false);
                  break;
                case SmoothOption.SmoothResetAtJumps:
                  MCSmooth_DoSmooth_Backward(OutputEDFFile.DataBuffer, k, k1, fileSampleNr, ref smoothReset, true);
                  break;
              }
              smoothReset = false;
            }

            OutputEDFFile.WriteDataBlock(k);
          }

        } // if checksettings
      } // try
      catch (Exception e)
      {
        AppError.Add(e.Message, NeuroLoopGainController.DefaultErrorMessageId);
      }

      return !AppError.Signaled;
    }

    /// <summary>
    /// Copies the input analysis signal to the output file.
    /// </summary>
    /// <returns><c>true</c> if successful</returns>
    private bool TranslateInputSignal()
    {
      int inputBlockSamples = InputEDFFile.SignalInfo[InputSignalSelected].NrSamples;

      int outBlockSample = 0;

      // Processing input block by block
      for (int k = 0; k < InputEDFFile.FileInfo.NrDataRecords; k++)
      {
        // Reading input block to the buffer
        InputEDFFile.ReadDataBlock(k);

        for (int k1 = 0; k1 < inputBlockSamples; k1++)
        {
          OutputEDFFile.DataBuffer[OutputBufferOffset[MCOutputSignalIndex.Input] + outBlockSample] = InputEDFFile.DataBuffer[InputBufferOffsets[InputSignalSelected] + k1];

          // Check if output block filled
          if (outBlockSample == (OutputEDFFile.SignalInfo[0].NrSamples - 1))
          {
            OutputEDFFile.WriteDataBlock(OutputEDFFile.FileInfo.NrDataRecords);
            outBlockSample = 0;
          }
          else
            outBlockSample++;
        }
      }

      OutputEDFFile.CommitChanges();

      return !AppError.Signaled;
    }

    #endregion Private Methods

    #region Constructors

    //TODO: Consider the following modifications in order to allow higher output sampling rate
    /*
    private double ArtHF { get; set; }
    private double ArtLF { get; set; }
    private double ArtZero { get; set; }
     */

    /// <summary>
    /// Initializes a new instance of the <see cref="NeuroLoopGain"/> class.
    /// </summary>
    /// <param name="appController">The controller.</param>
    public NeuroLoopGain(NeuroLoopGainController appController)
    {
      _appController = appController;
      LastMCJump = new MCJump();
    }

    #endregion Constructors

    #region public properties

    /// <summary>
    /// Gets the application configuration.
    /// </summary>
    public MCconfiguration AppConf
    {
      get
      {
        return _appController.AppConf;
      }
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public ErrorMessage AppError
    {
      get
      {
        return _appController.ApplicationError;
      }
    }

    /// <summary>
    /// Gets or sets the array with all EDF input signal buffer offsets.
    /// </summary>
    /// <value>
    /// The EDF input signal buffer offsets.
    /// </value>
    public int[] InputBufferOffsets { get; set; }

    /// <summary>
    /// Gets or sets the input EDF file.
    /// </summary>
    /// <value>
    /// The input EDF file.
    /// </value>
    public EdfFile InputEDFFile { get; set; }

    /// <summary>
    /// Gets or sets the name of the input EDF file.
    /// </summary>
    /// <value>
    /// The name of the input EDF file.
    /// </value>
    public string InputEDFFileName { get; set; }

    /// <summary>
    /// Gets the input signal sample frequency.
    /// </summary>
    public double InputSampleFrequency
    {
      get
      {
        return InputEDFFile != null
                 ? InputEDFFile.SignalInfo[InputSignalSelected].NrSamples / InputEDFFile.FileInfo.SampleRecDuration
                 : double.NaN;
      }
    }

    /// <summary>
    /// Gets or sets the selected input signal.
    /// </summary>
    /// <value>
    /// The input signal selected.
    /// </value>
    public int InputSignalSelected { get; set; }

    /// <summary>
    /// Gets or sets the number of MC samples in each EDF output file datablock.
    /// </summary>
    /// <value>
    /// The M csignals block samples.
    /// </value>
    public int MCsignalsBlockSamples { get; set; }

    /// <summary>
    /// Gets or sets the total number of MC signal samples in the whole output file.
    /// </summary>
    /// <value>
    /// The M csignals file samples.
    /// </value>
    public int MCsignalsFileSamples { get; set; }

    /// <summary>
    /// Gets or sets the number of output signals.
    /// </summary>
    /// <value>
    /// The number of output signals.
    /// </value>
    public int NumOutputSignals { get; set; }

    /// <summary>
    /// Gets or sets the output signal buffer offset.
    /// </summary>
    /// <value>
    /// The output signal buffer offset.
    /// </value>
    public int[] OutputBufferOffset { get; set; }

    /// <summary>
    /// Gets or sets the output EDF file.
    /// </summary>
    /// <value>
    /// The output EDF file.
    /// </value>
    public EdfFile OutputEDFFile { get; set; }

    #endregion public properties

    #region public methods

    /// <summary>
    /// Runs the NeuroLoopGain analysis.
    /// </summary>
    /// <returns></returns>
    public bool Analyze()
    {
      return DoAnalysis();
    }

    #endregion public methods
  }
}

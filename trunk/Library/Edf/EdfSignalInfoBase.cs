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
using System.Globalization;
using System.Text;
using NeuroLoopGainLibrary.General;
using NeuroLoopGainLibrary.Mathematics;
using NeuroLoopGainLibrary.Text;

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfSignalInfoBase : EdfFileHeaderInfoBase
  {
		#region enums 

    public enum Field
    {
      SignalLabel = 0,
      Transducertype = 1,
      PhysiDim = 2,
      PhysiMin = 3,
      PhysiMax = 4,
      DigiMin = 5,
      DigiMax = 6,
      Prefilter = 7,
      NrSamples = 8,
      Reserved = 9
    }

		#endregion enums 

		#region private fields 

private short _digiMax;
    private short _digiMin;
    private string _logFloatPhysiDim;
    private int _nrSamples;
    private double _physiDigiRatio;
    private string _physiDim;
    private double _physiMax;
    private double _physiMin;
    private string _preFilter;
    private string _reserved;
    private EdfSignalInfoRaw _signalInfoRaw = new EdfSignalInfoRaw();
    private string _signalLabel;
    private string _transducerType;

		#endregion private fields 

		#region protected methods 

    protected void CheckCalibration()
    {
      Calibrated = (
                      FieldValid[(int)Field.PhysiMin] && FieldValid[(int)Field.PhysiMax] &&
                      FieldValid[(int)Field.DigiMin] && FieldValid[(int)Field.DigiMax] &&
                      (_digiMin != _digiMax) && (!MathEx.SameValue(_physiMin, _physiMax)));
      if (Calibrated)
        _physiDigiRatio = (_physiMax - _physiMin) / (_digiMax - _digiMin);
    }

    protected virtual void DoAssign(EdfSignalInfoBase source)
    {
      if (ReadOnly)
        throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
      StrictChecking = source.StrictChecking;
      DecimalSeparator = source.DecimalSeparator;
      ThousandSeparator = source.ThousandSeparator;
      DigiMax = source.DigiMax;
      DigiMin = source.DigiMin;
      NrSamples = source.NrSamples;
      PhysiDim = source.PhysiDim;
      PhysiMax = source.PhysiMax;
      PhysiMin = source.PhysiMin;
      PreFilter = source.PreFilter;
      Reserved = source.Reserved;
      SignalLabel = source.SignalLabel;
      TransducerType = source.TransducerType;
      for (int i = 0; i < FieldValid.Length; i++)
        FieldValid[i] = source.FieldValid[i];
    }

    protected override void DoReCheck()
    {
      DataExists = false;
    }

		#endregion protected methods 

		#region Constructors 

    public EdfSignalInfoBase(object owner)
      : base(owner)
    {
      // Set protected NumberDataFields value to avoid a virtual method call in constructor
      NumberDataFields = 10;

      for (int i = 0; i < NrDataFields; i++)
      {
        switch (i)
        {
          case 0:
          case 1:
          case 2:
          case 7:
          case 9:
            FieldValid[i] = true;
            break;
        }
      }

    }

		#endregion Constructors 

		#region public properties 

    public virtual int BufferOffset { get; set; }

    public bool Calibrated { get; private set; }

    public virtual short DigiMax
    {
      get
      {
        return _digiMax;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value != _digiMax)
          Modified = true;
        _digiMax = value;
        FieldValid[(int)Field.DigiMax] = true;
        CheckCalibration();
      }
    }

    public virtual short DigiMin
    {
      get
      {
        return _digiMin;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value != _digiMin)
          Modified = true;
        _digiMin = value;
        FieldValid[(int)Field.DigiMin] = true;
        CheckCalibration();
      }
    }

    public virtual bool IsLogFloat
    {
      get
      {
        if (!DataValid)
          return false;
        const string logFloatMask = EdfConstants.LogFloatMask;
        bool result = false;
        string t = PreFilter;
        if ((t != null) && (t.Contains(logFloatMask.Substring(0, 14)))) // check 'sign*LN[sign*('
        {
          t = t.Substring(t.IndexOf(logFloatMask.Substring(0, 14)), 46);
          StringBuilder s = new StringBuilder(t);
          s.Remove(37, 8);
          s.Remove(25, 8);
          s.Remove(14, 8);
          result =
            (_physiDim == EdfConstants.LogFloatSignalLabel) &&
            (DigiMin == -32767) && (MathEx.SameValue(PhysiMin, -32767)) &&
            (DigiMax == 32767) && (MathEx.SameValue(PhysiMax, 32767)) &&
            (s.ToString() == EdfConstants.LogFloatMask) &&
            TextUtils.IsLeftJustified(t.Substring(14, 8)) &&
            TextUtils.IsLeftJustified(t.Substring(25, 8)) &&
            TextUtils.ValidDouble(t.Substring(25, 8), FormatInfo) &&
            TextUtils.IsLeftJustified(t.Substring(37, 8)) &&
            TextUtils.ValidDouble(t.Substring(37, 8), FormatInfo);
          if (result)
          {
            _logFloatPhysiDim = t.Substring(14, 8).Trim();
            LogFloatYMin = double.Parse(t.Substring(25, 8), FormatInfo);
            LogFloatA = double.Parse(t.Substring(37, 8), FormatInfo);
          }
        }
        return result;
      }
    }

    public double LogFloatA { get; private set; }

    public double LogFloatYMin { get; private set; }

    public virtual int NrSamples
    {
      get
      {
        return _nrSamples;
      }
      set
      {
        if (value != NrSamples)
        {
          if (ReadOnly)
            throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
          if (DataExists)
            throw new InvalidOperationException(EdfConstants.FileAlreadyContainsData);
          Modified = true;
          _nrSamples = value;
        }
        FieldValid[(int)Field.NrSamples] = (_nrSamples > 0);
      }
    }

    public virtual string PhysiDim
    {
      get
      {
        return IsLogFloat ? _logFloatPhysiDim : _physiDim;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value.TrimEnd(null) != _physiDim)
          Modified = true;
        _physiDim = value.TrimEnd(null);
        FieldValid[(int)Field.PhysiDim] =
          (TextUtils.IsASCIIPrintable(value) && TextUtils.IsLeftJustified(value)) || !StrictChecking;
      }
    }

    public virtual double PhysiMax
    {
      get
      {
        return _physiMax;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (!MathEx.SameValue(value, _physiMax))
          Modified = true;
        _physiMax = value;
        FieldValid[(int)Field.PhysiMax] = true;
        CheckCalibration();
      }
    }

    public virtual double PhysiMin
    {
      get
      {
        return _physiMin;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (!MathEx.SameValue(value, _physiMin))
          Modified = true;
        _physiMin = value;
        FieldValid[(int)Field.PhysiMin] = true;
        CheckCalibration();
      }
    }

    public virtual string PreFilter
    {
      get
      {
        return _preFilter;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value.TrimEnd(null) != _preFilter)
          Modified = true;
        _preFilter = value.TrimEnd(null);
        FieldValid[(int)Field.Prefilter] =
          (TextUtils.IsASCIIPrintable(value) && TextUtils.IsLeftJustified(value)) || !StrictChecking;
      }
    }

    public virtual string Reserved
    {
      get
      {
        return _reserved;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if ((value == null && _reserved != null) || ((value != null) && (value.TrimEnd(null) != _reserved)))
          Modified = true;
        _reserved = value != null ? value.TrimEnd(null) : value;
        FieldValid[(int)Field.Reserved] =
          (TextUtils.IsASCIIPrintable(value) && TextUtils.IsLeftJustified(value)) || !StrictChecking;
      }
    }

    public virtual EdfSignalInfoRaw SignalInfoRaw { get { return _signalInfoRaw; } }

    public virtual EdfSignalInfoRaw SignalInfoRecord
    {
      get
      {
        EdfSignalInfoRaw result = new EdfSignalInfoRaw();
        try
        {
          result.SignalLabel = FieldValid[(int)Field.SignalLabel] ? _signalLabel : string.Empty;
          result.TransducerType = FieldValid[(int)Field.Transducertype] ? _transducerType : string.Empty;
          result.PhysiDim = FieldValid[(int)Field.PhysiDim] ? _physiDim : string.Empty;
          result.PhysiMin = string.Empty;
          if (FieldValid[(int)Field.PhysiMin])
          {
            try
            {
              result.PhysiMin = TextUtils.DoubleToString(_physiMin, FormatInfo, 8);
            }
            catch (ConvertException)
            {
            }
          }
          result.PhysiMax = string.Empty;
          if (FieldValid[(int)Field.PhysiMax])
          {
            try
            {
              result.PhysiMax = TextUtils.DoubleToString(_physiMax, FormatInfo, 8);
            }
            catch (ConvertException)
            {
            }
          }
          result.DigiMin = FieldValid[(int)Field.DigiMin] ? _digiMin.ToString() : string.Empty;
          result.DigiMax = FieldValid[(int)Field.DigiMax] ? _digiMax.ToString() : string.Empty;
          result.PreFilter = FieldValid[(int)Field.Prefilter] ? _preFilter : string.Empty;
          result.NrSamples = FieldValid[(int)Field.NrSamples] ? _nrSamples.ToString() : string.Empty;
          result.Reserved = FieldValid[(int)Field.Reserved] ? _reserved : string.Empty;
        }
        catch
        {
          result = null;
        }
        return result;
      }
      set
      {
        _signalInfoRaw = (EdfSignalInfoRaw)value.Clone();
        for (int i = 0; i < FieldValid.Length; i++)
          FieldValid[i] = false;
        SignalLabel = _signalInfoRaw.SignalLabel;
        TransducerType = _signalInfoRaw.TransducerType;
        PhysiDim = _signalInfoRaw.PhysiDim;
        double aDouble;
        if (double.TryParse(
          _signalInfoRaw.PhysiMin, NumberStyles.Float | NumberStyles.AllowThousands, FormatInfo, out aDouble))
          PhysiMin = aDouble;
        if (double.TryParse(
          _signalInfoRaw.PhysiMax, NumberStyles.Float | NumberStyles.AllowThousands, FormatInfo, out aDouble))
          PhysiMax = aDouble;
        short aShort;
        if (short.TryParse(_signalInfoRaw.DigiMin, out aShort))
          DigiMin = aShort;
        else if (!StrictChecking)
        {
          if (double.TryParse(
            _signalInfoRaw.DigiMin, NumberStyles.Float | NumberStyles.AllowThousands, FormatInfo, out aDouble))
          {
            if ((MathEx.SameValue(aDouble - Math.Floor(aDouble), 0)) && (aDouble >= short.MinValue) && (aDouble <= short.MaxValue))
              DigiMin = (short)aDouble;
          }
        }
        if (short.TryParse(_signalInfoRaw.DigiMax, out aShort))
          DigiMax = aShort;
        else if (!StrictChecking)
        {
          if (double.TryParse(
            _signalInfoRaw.DigiMax, NumberStyles.Float | NumberStyles.AllowThousands, FormatInfo, out aDouble))
          {
            if (MathEx.SameValue(aDouble - Math.Floor(aDouble), 0) && (aDouble >= short.MinValue) && (aDouble <= short.MaxValue))
              DigiMax = (short)aDouble;
          }
        }
        if (FieldValid[(int)Field.DigiMin] && FieldValid[(int)Field.DigiMax] &&
            (_digiMin >= _digiMax))
        {
          FieldValid[(int)Field.DigiMin] = false;
          FieldValid[(int)Field.DigiMax] = false;
        }
        PreFilter = _signalInfoRaw.PreFilter;
        int aInteger;
        if (int.TryParse(_signalInfoRaw.NrSamples, out aInteger))
          NrSamples = aInteger;
        Reserved = _signalInfoRaw.Reserved;
        if (StrictChecking)
        {
          FieldValid[(int)Field.PhysiMin] &=
            TextUtils.IsLeftJustified(_signalInfoRaw.PhysiMin) &&
            (DecimalSeparator == EdfConstants.DefaultDecimalSeparator) &&
            (ThousandSeparator == EdfConstants.DefaultThousandSeparator);
          FieldValid[(int)Field.PhysiMax] &=
            TextUtils.IsLeftJustified(_signalInfoRaw.PhysiMax) &&
            (DecimalSeparator == EdfConstants.DefaultDecimalSeparator) &&
            (ThousandSeparator == EdfConstants.DefaultThousandSeparator);
          FieldValid[(int)Field.DigiMin] &= TextUtils.IsLeftJustified(_signalInfoRaw.DigiMin);
          FieldValid[(int)Field.DigiMax] &= TextUtils.IsLeftJustified(_signalInfoRaw.DigiMax);
          FieldValid[(int)Field.NrSamples] &= TextUtils.IsLeftJustified(_signalInfoRaw.NrSamples);
        }
        CheckCalibration();
      }
    }

    public virtual string SignalLabel
    {
      get
      {
        return _signalLabel;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value.TrimEnd(null) != _signalLabel)
          Modified = true;
        _signalLabel = value.TrimEnd(null);
        FieldValid[(int)Field.SignalLabel] =
          (TextUtils.IsASCIIPrintable(value) && TextUtils.IsLeftJustified(value)) || !StrictChecking;
      }
    }

    public virtual string TransducerType
    {
      get
      {
        return _transducerType;
      }
      set
      {
        if (ReadOnly)
          throw new InvalidOperationException(EdfConstants.FileIsReadOnly);
        if (value.TrimEnd(null) != _transducerType)
          Modified = true;
        _transducerType = value.TrimEnd(null);
        FieldValid[(int)Field.Transducertype] =
          (TextUtils.IsASCIIPrintable(value) && TextUtils.IsLeftJustified(value)) || !StrictChecking;
      }
    }

		#endregion public properties 

		#region public methods 

    public void Assign(object source)
    {
      EdfSignalInfoBase edfSignalInfoBase = source as EdfSignalInfoBase;
      if (edfSignalInfoBase == null)
        throw new ArgumentException("Value is not an EdfSignalInfoBase object.", "source");
      DoAssign(edfSignalInfoBase);
    }

    public double DigiToPhysi(int value)
    {
      return (PhysiMin + (value - DigiMin) * ((PhysiMax - PhysiMin) / (DigiMax - DigiMin)));
    }

    public double DigiToPhysi(double value)
    {
      return (PhysiMin + (value - DigiMin) * ((PhysiMax - PhysiMin) / (DigiMax - DigiMin)));
    }

    public int PhysiToDigi(double value)
    {
      return (int)(DigiMin + (value - PhysiMin) * ((DigiMax - DigiMin) / (PhysiMax - PhysiMin)));
    }

		#endregion public methods 
  }
}
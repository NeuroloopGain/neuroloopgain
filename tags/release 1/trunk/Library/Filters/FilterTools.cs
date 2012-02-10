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
using System.Linq;
using NeuroLoopGainLibrary.Text;

namespace NeuroLoopGainLibrary.Filters
{
  public static class FilterTools
  {

    public static string[] FilterNames =
      new[]
        {
          "LP", "HP", "NOTCH", "ABS", "ABSSINGLESIDE", "BP", "DUE", "SE", "G", "1-G", "BP12",
          "DCATT", "1/DCATT", "DUE1987", "DUE2007", "LP05", "LP05NS", "HP05NS", "MEDIAN", "SQR",
          "SQRT", "MEAN", "PERCENTILLIAN", "DB", "GAINDIVONEMINUSX", "DESPECKLER"
        };

    public static FilterType GetFilterType(FilterBase filter)
    {
      string s = filter.GetType().Name.ToUpper();
      if (s == "LPFILTER")
        return FilterType.LP;
      if (s == "HPFILTER")
        return FilterType.HP;
      if (s == "DUEFILTER")
        return FilterType.DUE;
      if (s == "SEFILTER")
        return FilterType.SE;
      if (s == "NOTCHFILTER")
        return FilterType.NOTCH;
      if (s == "GFILTER")
        return FilterType.G;
      if (s == "INVGFILTER")
        return FilterType.InvG;
      if (s == "BPFILTER")
        return FilterType.BP;
      if (s == "RECTIFIER")
        return FilterType.RECT;
      if (s == "SINGLESIDERECTIFIER")
        return FilterType.SingleSideRECT;
      if (s == "BP12FILTER")
        return FilterType.BP12;
      if (s == "DCATTFILTER")
        return FilterType.DCatt;
      if (s == "INVDCATTFILTER")
        return FilterType.InvDCatt;
      if (s == "DUE1987FILTER")
        return FilterType.DUE1987;
      if (s == "DUE2007FILTER")
        return FilterType.DUE2007;
      if (s == "LP05FILTER")
        return FilterType.LP05;
      if (s == "LP05NSFILTER")
        return FilterType.LP05NS;
      if (s == "HP05NSFILTER")
        return FilterType.HP05NS;
      if (s == "MEDIANFILTER")
        return FilterType.Median;
      if (s == "SQRFILTER")
        return FilterType.Square;
      if (s == "SQRTFILTER")
        return FilterType.Squareroot;
      if (s == "MEANFILTERQUICK")
        return FilterType.Mean;
      if (s == "PERCENTILLIANFILTER")
        return FilterType.Percentillian;
      if (s == "DBFILTER")
        return FilterType.dB;
      if (s == "GAINDIVONEMINUSX")
        return FilterType.GainDivOneMinusX;
      if (s == "DESPECKLER")
        return FilterType.Despeckler;
      throw new ArgumentException("Unknown filter object");
    }

    public static string GetFilterName(FilterType filterType)
    {
      int i = Convert.ToInt32(filterType);
      return FilterNames[i];
    }

    public static FilterBase GetFilterByName(string filterName)
    {
      filterName = filterName.Trim().ToUpper();
      int filterNameIndex = FilterNames.ToList().IndexOf(filterName);
      if (filterNameIndex < 0)
        return null;
      FilterType filterType = (FilterType)Enum.ToObject(typeof(FilterType), filterNameIndex);
      switch (filterType)
      {
        //case FilterType.LP:
        //  return new LPFilter();
        //case FilterType.HP:
        //  return new HPFilter();
        //case FilterType.NOTCH:
        //  return new NotchFilter();
        //case FilterType.RECT:          
        //  return new Rectifier();
        //case FilterType.SingleSideRECT:
        //  return new SingleSideRectifier();
        //case FilterType.BP:
        //  return new BPFilter();
        case FilterType.DUE:
          return new DUEFilter();
        case FilterType.SE:
          return new SEFilter();
        //case FilterType.G:
        //  return new GFilter();
        //case FilterType.InvG:
        //  return new InvGFilter();
        //case FilterType.BP12:
        //  return new BP12Filter();
        //case FilterType.DCatt:
        //  return new DCattFilter();
        //case FilterType.InvDCatt:
        //  return new INVDCattFilter();
        //case FilterType.DUE1987:
        //  return new DUE1987Filter();
        //case FilterType.DUE2007:
        //  return new DUE2007Filter();
        //case FilterType.LP05:
        //  return new LP05Filter();
        //case FilterType.LP05NS:
        //  return new LP05NSFilter();
        //case FilterType.HP05NS:
        //  return new HP05NSFilter();
        //case FilterType.Median:
        //  return new MedianFilter();
        //case FilterType.Square:
        //  return new SqrFilter();
        //case FilterType.Squareroot:
        //  return new SqrtFilter();
        //case FilterType.Mean:
        //  return new MeanFilterQuick();
        //case FilterType.Percentillian:
        //  return new PercentillianFilter();
        //case FilterType.dB:
        //  return new DBFilter();
        //case FilterType.GainDivOneMinusX:
        //  return new GainDivOneMinusX();
        //case FilterType.Despeckler:
        //  return new Despeckler();
        default:
          throw new ArgumentOutOfRangeException();
      }
    }


    public static FilterBase StrToFilter(string value)
    {
      FilterBase result = null;
      string s = value;
      if (s.Contains("(") && s.EndsWith(")"))
      {
        result = GetFilterByName(TextUtils.StripStringValue(ref s, "("));
        if (result != null)
        {
          s = s.Remove(s.Length - 1, 1);          
          int i = 2;
          while (s != string.Empty)
          {
            double parsedValue = double.Parse(TextUtils.StripStringValue(ref s, "/"), CultureInfo.InvariantCulture);
            if (!result.Setting[i].IsReadOnly)
              result.Setting[i].Value = parsedValue;
            i++;
          }
        }
      }
      else
        if (!s.Contains("(") && !s.Contains(")"))
          result = GetFilterByName(s.Trim());
      return result;      
    }

    public static void ValidSampleFrequency()
    {
      throw new NotImplementedException("Not implemented");
      // const AFilter: TCustomFilter): TFilterType;
      // function FilterToStr(const AFilter: TCustomFilter): String;
      // function StrToFilter(const AString: String): TCustomFilter;
      // function GetFilterDescriptionStr(const FilterName:String): String;
      // function ValidSampleFrequency(const ASampleFreq: double; const AFilterStr:
      //     String; ADelimiters: String): bool;
      // <<<<<< Automatically translated
      // // Original code >>>>>>
      // function  GetFilterByName(FilterName:String): TCustomFilter;
      // function  GetFilterType(const AFilter: TCustomFilter): TFilterType;
      // 
      // function FilterToStr(const AFilter: TCustomFilter): String;
      // function StrToFilter(const AString: String): TCustomFilter;
      // 
      // function GetFilterDescriptionStr(const FilterName:String): String;
      // 
      // function ValidSampleFrequency(const ASampleFreq: Extended; const AFilterStr:
      //     String; ADelimiters: String): Boolean;
      // // <<<<<< Original code
    }

    public static void GetAllFilterNames()
    {
      throw new NotImplementedException("Not implemented");
      // TFilterType i;
      //   for(i = TFilterType; i >= Low; i--)
      //     Strings.Add(FilterNames[i]);
      // }
      // <<<<<< Automatically translated
      // // Original code >>>>>>
      // var
      //   i: TFilterType;
      // begin
      //   For i:=Low(TFilterType) to High(TFilterType) do
      //     Strings.Add(FilterNames[i]);
      // end;
      // // <<<<<< Original code
    }

  }

}

// (******************************************************************************
// *
// * Unit uFilter.pas
// *
// * Unit with a basic (digital) IIR-filter object and descendant filter objects.
// * These objects perform digital filtering of signals.
// *
// * Copyright 1999-2010, Marco Roessen
// *
// * Email: m.roessen@mchaaglanden.nl
// *
// * Medisch Centrum Haaglanden,
// * Centrum voor slaap- en waakonderzoek,
// * Lijnbaan 32,
// * NL-2512 VA Den Haag.
// *
// * Converted and updated from Borland Pascal 7.0 source by Marco Roessen
// *
// * Updates :
// *
// * Aug.2000, MR  Restructured the base objects TCustomFilter and
// *                TIIRCustomFilter for easier implementation of new
// *                filters
// * Aug.2001, MR  Added LP and HP filters
// *               Added function GetFilterByName
// * Nov.2001, MR  Added function GetFilterType
// *                     function FilterToStr
// *                     function StrToFilter
// * Mar.2008, MR  Added TLP05NSFilter filter
// *
// ****************************************************************************
// * Example:
// *
// *  MyFilter:= TGFilter.Create;
// *  try
// *     MyFilter.DataInType:= fdDouble;
// *     MyFilter.DataOutType:= fdDouble;
// *     MyFilter.Setting[1]:= SampleFreq;    { Sample Frequency eg 100Hz }
// *     MyFilter.Setting[2]:= G;             { Gain = attanuation }
// *     MyFilter.Setting[3]:= F0;            { F0 = Center frequency in Hz }
// *     MyFilter.Setting[4]:= B;             { B = BandWidth }
// *     MyFilter.CheckSettings;              { CheckSettings }
// *     MyFilter.Reset;
// *     SetLength(InBuffer,NrSamples);       { Nrsamples at once }
// *     SetLength(OutBuffer,NrSamples);
// * { Fill InBuffer with values }
// *
// *     MyFilter.FilterSamples(InBuffer,OutBuffer,0,NrSamples-1);
// *   finally
// *      MyFilter.Free
// *   end
// ******************************************************************************)
// { $LOG:  39274: uFilter.pas 
// {
// {   Rev 1.17    16-12-2010 11:34:14  Marco
// { Fixing Polyman bug reset filters when resizing window
// }
// {
// {   Rev 1.16    5-11-2010 12:48:50  Marco
// }
// {
// {   Rev 1.15    8-9-2010 10:58:04  Marco
// }
// {
// {   Rev 1.14    23-8-2010 14:36:34  Marco
// { Meanfilter doe return median, not mean
// }
// {
// {   Rev 1.13    10-6-2010 15:32:46  Marco
// { Adding RMS filters
// }
// {
// {   Rev 1.12    18-11-2009 15:51:20  Marco
// { Intermediate checkin 20091118
// }
// {
// {   Rev 1.11    14-10-2009 9:39:38  Marco
// { Fixing TLP05Filter bug.
// { Setting 3 (frequency) should be > 0.
// }
// {
// {   Rev 1.10    25-9-2009 9:16:48  Marco
// }
// {
// {   Rev 1.9    10-9-2009 12:51:48  Marco
// }
// {
// {   Rev 1.8    10-7-2009 16:27:58  Marco
// { Fixing bug Automatic Respiratory Analysis TLP05NSFilter Setting 4 error
// }
// {
// {   Rev 1.7    22-6-2009 11:28:54  Marco
// { Adding EDF+ compatible descriptive filter string
// }
// {
// {   Rev 1.6    8-5-2009 14:24:16  Marco
// { Adding Median Filter
// }
// {
// {   Rev 1.5    14-11-2008 16:32:30  Marco
// }
// {
// {   Rev 1.4    16-6-2008 15:56:28  Marco
// { Adding ResetUseNextSampleCode to TLP05NSFilter
// }
// {
// {   Rev 1.3    26-5-2008 11:16:32  Marco
// { Removing abstract error for TRectifier
// }
// {
// {   Rev 1.2    14-4-2008 13:47:52  Marco
// { Temporaraly adding "Use first sample to reset" option
// }
// {
// {   Rev 1.1    31-3-2008 13:03:02  Marco
// { Adding unit uConsts
// }
// {
// {   Rev 1.0    11-10-2007 16:45:50  Marco
// { Moving to new VCS structure
// }
// {
// {   Rev 1.0    11-10-2007 13:49:46  Marco
// { Added to VCS 11.Oct.2007
// }
// {
// {   Rev 1.8    13-7-2007 15:57:32  Marco
// { Checkin 13.JUL.2007
// { Stable, viewer only
// }
// {
// {   Rev 1.7    23-3-2007 14:16:30  Marco
// }
// {
// {   Rev 1.6    15-2-2007 17:12:20  Marco
// { Modifications regarding email Bob Kemp (24-01-2007)
// }
// {
// {   Rev 1.5    8-2-2007 17:19:06  Marco
// { Updating unit because of udStr changes
// }
// {
// {   Rev 1.4    22-1-2007 11:14:38  Marco
// }
// {
// {   Rev 1.3    7-8-2006 16:02:22  Marco
// }
// {
// {   Rev 1.2    19-6-2006 10:02:02  Marco
// }
// {
// {   Rev 1.1    7-6-2005 16:22:10  Marco
// { Added CompilerOptions.inc file to each unit
// { Added unit initialization/finalization code to get revision code from VCS
// { Added Version Control LOG:
// }
// unit uFilter;
// 
// {$I CompilerOptions.inc}
// {$WRITEABLECONST ON}
// 
// interface
// 
// uses
//   Windows, Classes, SysUtils, uMedian, uDynamicArray;
// 
// // All implemented filter types
// type
//   TFilterType =
//     (LP,HP,NOTCH,RECT,BP,DUE,SE,G,InvG,BP12,DCatt,InvDCatt,DUE1987,DUE2007,
//      LP05,LP05NS,Median,Square,Squareroot,Mean,Percentillian,dB);
// 
// const
// // Uppercase filternames only!
//   FilterNames: Array[Low(TFilterType)..High(TFilterType)] of String =
//     ('LP','HP','NOTCH','ABS','BP','DUE','SE','G','1-G','BP12',
//      'DCATT','1/DCATT','DUE1987','DUE2007','LP05','LP05NS','MEDIAN','SQR',
//      'SQRT','MEAN','PERCENTILLIAN','DB');
// 
// type
//   EFilter = class(Exception);
// 
//   FilterDataType      = (fdSmallInt,fdInteger,fdSingle,fdDouble);
//   FilterDirectionType = (ForwardOnly,BackwardOnly);
// 
// { Custom Filter:
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
// }
//   TCustomFilter = class(TObject)
//   private
//     FUseFirstSampleToReset: boolean;
//   protected
//     FDataInType,
//     FDataOutType :FilterDataType;
//     FDirection   :FilterDirectionType;
//     FNewSettings :Boolean;
//     FNrSettings :Integer;
//     FSetting     :Array of Double;
//     FSettingInfo :Array of String;
//     function DoCheckSettings: Integer; virtual;
//     procedure CheckLengthSavedState(Index: Integer); virtual;
//     procedure DoClearSavedState(Index: Integer); virtual;
//     procedure DoRestoreState(Index: Integer); virtual;
//     procedure DoSaveState(Index: Integer); virtual;
//     function GetAsString: String; virtual;
//     function GetEDFFilterStr: String; virtual;
//     function GetFilterDescription: String;
//     function GetGain: Double; virtual;
//     function GetSampleFrequency: Double; virtual;
//     function GetSetting(Index: Integer): Double; virtual;
//     function GetSettingDimensionInfo(Index: Integer): String; virtual;
//     function GetSettingInfo(Index:Integer): String;
//     function GetSettingIsReadOnly(Index: Integer): Boolean; virtual;
//     procedure SetDataOutType(const Value: FilterDataType); virtual;
//     procedure SetGain(const Value: Double); virtual;
//     procedure SetNrSettings(Value:Integer); virtual;
//     procedure SetSampleFrequency(const Value: Double); virtual;
//     procedure SetSetting(Index:Integer; const Value:Double); virtual;
//     property Direction:FilterDirectionType read FDirection write FDirection
//       default ForwardOnly;
//     property UseFirstSampleToReset: boolean read FUseFirstSampleToReset write
//         FUseFirstSampleToReset;
//   public
//     constructor Create; virtual;
//     destructor Destroy; override;
//     function CheckSettings: Integer;
//     procedure ClearSavedState(Index: Integer = 0);
//     procedure FilterSamples(var SamplesIn; var SamplesOut; IdxStart,IdxEnd:
//       Integer; OutIdxStart: Integer = -1); virtual; abstract;
//     procedure Reset; overload; virtual;
//     procedure Reset(UseNextSample: Boolean); overload; virtual;
//     procedure Reset(Xn: Double); overload; virtual; abstract;
//     procedure RestoreState(Index: Integer = 0);
//     procedure SaveState(Index: Integer = 0);
//     property AsString: string read GetAsString;
//     property DataInType:FilterDataType read FDataInType write FDataInType;
//     property DataOutType: FilterDataType read FDataOutType write SetDataOutType;
//     property EDFFilterStr: String read GetEDFFilterStr;
//     property FilterDescription: String read GetFilterDescription;
//     property Gain: Double read GetGain write SetGain;
//     property NrSettings:Integer read FNrSettings;
//     property SampleFrequency: Double read GetSampleFrequency write
//       SetSampleFrequency;
//     property Setting[Index:Integer]:Double read GetSetting write SetSetting;
//     property SettingDimensionInfo[Index:Integer]:String
//       read GetSettingDimensionInfo;
//     property SettingInfo[Index:Integer]:String read GetSettingInfo;
//     property SettingIsReadOnly[Index: Integer]: Boolean read
//       GetSettingIsReadOnly;
//   end;
// 
// { TCustomIIRFilter
//   Basic IIR filter object. This object does not modify the signal, but is
//    used as a prototype for descendant IIR filters.
//    Descendant filters only have to override the constructor Create,
//    procedure CalcIIRCoeff and CheckSettings to be functional.
// 
//   FilterInfo: 0    = FilterName         (e.g. 'HP', 'NOTCH')
//               1    = SampleFrequency    (Hz)
//               2    = Gain               (FilterGain)
//               3..N = SettingDescription (e.g. 'Bandwidth (Hz)')
//   FilterSettings : 0    = Not used
//                    1    = SampleFrequency (Hz)
//                    2    = FilterGain
//                    3..N = FilterSpecific settings (use FilterInfo for details)
// }
//   TCustomIIRFilter = class(TCustomFilter)
//   private
//     FAnticipate :Boolean;
//     FBackPolate :Double;
//   protected
//     FFilterStateP :TDynExtendedArray;
//     FFilterStateZ :TDynExtendedArray;
//     FPoles :TDynExtendedArray;
//     FSavedStateP :TDynExtendedArrayArray;
//     FSavedStateZ :TDynExtendedArrayArray;
//     FZeros :TDynExtendedArray;
//     procedure CalculateIIRCoeff; virtual;
//     procedure CheckLengthSavedState(Index: Integer); override;
//     procedure DoClearSavedState(Index: Integer); override;
//     procedure DoRestoreState(Index: Integer); override;
//     procedure DoSaveState(Index: Integer); override;
//     procedure SetGain(const Value: Double); override;
//   public
//     constructor Create; override;
//     destructor Destroy; override;
//     procedure FilterSamples(var SamplesIn; var SamplesOut; IdxStart,IdxEnd:
//       Integer; OutIdxStart: Integer = -1); override;
//     procedure Reset; overload; override;
//     procedure Reset(UseNextSample: Boolean); overload; override;
//     procedure Reset(Xn: Double); overload; override;
//     property Anticipate:Boolean read FAnticipate write FAnticipate default True;
//     property BackPolate:Double read FBackPolate write FBackPolate;
//     property DataInType;
//     property DataOutType;
//     property Direction;
//     property NrSettings;
//     property Setting;
//     property SettingInfo;
//   end;
// 
// { 'LP': First order low pass filter.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = FilterFrequency (Hz) (-3dB)
// }
//   TLPFilter = class(TCustomIIRFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//     function GetEDFFilterStr: String; override;
//   public
//     constructor Create; override;
//   end;
// 
// { 'HP': First order high pass filter.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = FilterFrequency (Hz) (-3dB)
// }
//   THPFilter = class(TCustomIIRFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//     function GetEDFFilterStr: String; override;
//   public
//     constructor Create; override;
//   end;
// 
// { 'DU/E': Cascade of inverse of lowpass filter L-1(f)=1+j.(f/fc) or
//           L-1(s)=1+s/wc (gives du/dt), integration (or 1/s) (gives u),
//           difference (gives du) and gain g0 (gives g0.du). Bilinear.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = FilterFrequency (Hz)
// }
//   TDUEFilter = class(TCustomIIRFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//   public
//     constructor Create; override;
//   end;
// 
//   TDUE1987Filter = class(TDUEFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//   end;
// 
//   TDUE2007Filter = class(TDUEFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//   end;
// 
// { 'S/E' : Cascade of L-1 and G. Gives s(t) from e(t). Bilinear.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = Cut-off frequency (Hz)
//                    4 = Center frequency (Hz)
//                    5 = BandWidth (Hz)
// }
//   TSEFilter = class(TCustomIIRFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//   public
//     constructor Create; override;
//   end;
// 
// { 'NOTCH' : Notch filter.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = Center frequency (Hz)
//                    4 = BandWidth (Hz)
// }
//   TNotchFilter = class(TCustomIIRFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//     function GetEDFFilterStr: String; override;
//     function GetSettingIsReadOnly(Index: Integer): Boolean; override;
//     procedure SetSetting(Index:Integer; const Value:Double); override;
//   public
//     constructor Create; override;
//   end;
// 
// { 'G'   : Second-order resonator G(f)=g0/[1+j.(f0/B).(f/f0-f0/f)] or
//           G(s)=g0.Brad.s/[s.s+Brad.s-s0.s0] with Brad=2.pi.B. Bilinear.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = Center frequency (Hz)
//                    4 = BandWidth (Hz)
// }
//   TGFilter = class(TCustomIIRFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//   public
//     constructor Create; override;
//   end;
// 
// { '1-G' : Inverse of Second-order resonator G(f)=g0/[1+j.(f0/B).(f/f0-f0/f)] or
//           G(s)=g0.Brad.s/[s.s+Brad.s-s0.s0] with Brad=2.pi.B. Bilinear.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = Center frequency (Hz)
//                    4 = BandWidth (Hz)
// }
//   TInvGFilter = class(TCustomIIRFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//   public
//     constructor Create; override;
//   end;
// 
// { Second order bandpass gain.[j.f/(j.f+fc)].[f0/(j.f+f0)] or
//                         gain.s.w0/[s.s+(w0+wc).s+w0.wc]. Bilinear.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = HighPassFrequency (Hz)
//                    4 = LowPassFrequency (Hz)
// }
//   TBPFilter = class(TCustomIIRFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//     function GetEDFFilterStr: String; override;
//   public
//     constructor Create; override;
//   end;
// 
// { Cascade of first-order highpass (at wc) and second-order lowpass (at w0),
//   gain.[s/(s+wc)].[w0/(s+w0)]^2. Bilinear.
// 
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = HighPassFrequency (Hz)
//                    4 = LowPassFrequency (Hz)
// }
//   TBP12Filter = class(TCustomIIRFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//   public
//     constructor Create; override;
//   end;
// 
//   TDCattFilter = class(TCustomIIRFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//     function GetSettingDimensionInfo(Index: Integer): String; override;
//   public
//     constructor Create; override;
//   end;
// 
//   TINVDCattFilter = class(TCustomIIRFilter)
//   protected
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//     function GetSettingDimensionInfo(Index: Integer): String; override;
//   public
//     constructor Create; override;
//   end;
// 
// { Rectifier
//   NOT a IIR filter, but modified to fit in the structure.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
// }
// 
//   TRectifier = class(TCustomFilter)
//   protected
//     function DoCheckSettings: Integer; override;
//   public
//     constructor Create; override;
//     procedure FilterSamples(var SamplesIn; var SamplesOut; IdxStart,IdxEnd:
//       Integer; OutIdxStart:Integer = -1); override;
//     procedure Reset; override;
//     procedure Reset(UseNextSample: Boolean); overload; override;
//     property DataInType;
//     property DataOutType;
//     property Direction;
//     property FilterDescription;
//     property NrSettings;
//     property Setting;
//     property SettingDimensionInfo;
//     property SettingInfo;
//   end;
// 
//   TdBFilter = class(TCustomFilter)
//   protected
//     function DoCheckSettings: Integer; override;
//     function GetSettingDimensionInfo(Index: Integer): String; override;
//   public
//     constructor Create; override;
//     procedure FilterSamples(var SamplesIn; var SamplesOut; IdxStart,IdxEnd:
//       Integer; OutIdxStart:Integer = -1); override;
//     procedure Reset; override;
//     procedure Reset(UseNextSample: Boolean); overload; override;
//     property DataInType;
//     property DataOutType;
//     property Direction;
//     property FilterDescription;
//     property NrSettings;
//     property Setting;
//     property SettingDimensionInfo;
//     property SettingInfo;
//   end;
// 
// { SqrFilter (y=x^2)
//   NOT a IIR filter, but modified to fit in the structure.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
// }
// 
//   TSqrFilter = class(TCustomFilter)
//   protected
//     function DoCheckSettings: Integer; override;
//   public
//     constructor Create; override;
//     procedure FilterSamples(var SamplesIn; var SamplesOut; IdxStart,IdxEnd:
//       Integer; OutIdxStart:Integer = -1); override;
//     procedure Reset; override;
//     procedure Reset(UseNextSample: Boolean); overload; override;
//     property DataInType;
//     property DataOutType;
//     property Direction;
//     property FilterDescription;
//     property NrSettings;
//     property Setting;
//     property SettingDimensionInfo;
//     property SettingInfo;
//   end;
// 
// { SqrtFilter (y=x^-2)
//   NOT a IIR filter, but modified to fit in the structure.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
// }
// 
//   TSqrtFilter = class(TCustomFilter)
//   protected
//     function DoCheckSettings: Integer; override;
//   public
//     constructor Create; override;
//     procedure FilterSamples(var SamplesIn; var SamplesOut; IdxStart,IdxEnd:
//       Integer; OutIdxStart:Integer = -1); override;
//     procedure Reset; override;
//     procedure Reset(UseNextSample: Boolean); overload; override;
//     property DataInType;
//     property DataOutType;
//     property Direction;
//     property FilterDescription;
//     property NrSettings;
//     property Setting;
//     property SettingDimensionInfo;
//     property SettingInfo;
//   end;
// 
// { Halfth order Low Pass filter.
//   NOT a IIR filter, but modified to fit in the structure.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = Cut-off Frequency (Hz)
// }
//   TLP05Filter = class(TCustomIIRFilter)
//   protected
//     FRate: Double;
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//   public
//     constructor Create; override;
//     procedure FilterSamples(var SamplesIn; var SamplesOut; IdxStart,IdxEnd:
//       Integer; OutIdxStart:Integer = -1); override;
//     procedure Reset(Xn: Double); overload; override;
//   end;
// 
// { Halfth order Low Pass filter non symmetric.
//   NOT a IIR filter, but modified to fit in the structure.
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = Cut-off Frequency (Hz)
// }
//   TLP05NSFilter = class(TLP05Filter)
//   protected
//     FRate2: Double;
//     procedure CalculateIIRCoeff; override;
//     function DoCheckSettings: Integer; override;
//   public
//     constructor Create; override;
//     procedure FilterSamples(var SamplesIn; var SamplesOut; IdxStart,IdxEnd:
//       Integer; OutIdxStart:Integer = -1); override;
//     procedure Reset(Xn: Double); overload; override;
//   end;
// 
// { Median filter
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = Lookback (s)
// }
//   TMedianFilter = class(TCustomFilter)
//   protected
//     FMedianList: TDoubleMedianList;
//     FSavedState: Array of TDoubleMedianList;
//     function DoCheckSettings: Integer; override;
//     procedure CheckLengthSavedState(Index: Integer); override;
//     procedure DoClearSavedState(Index: Integer); override;
//     procedure DoRestoreState(Index: Integer); override;
//     procedure DoSaveState(Index: Integer); override;
//     function GetSettingDimensionInfo(Index: Integer): String; override;
//     procedure InitializeMedianList;
//   public
//     constructor Create; override;
//     destructor Destroy; override;
//     procedure FilterSamples(var SamplesIn; var SamplesOut; IdxStart,IdxEnd:
//       Integer; OutIdxStart:Integer = -1); override;
//     procedure Reset; overload; override;
//     procedure Reset(UseNextSample: Boolean); overload; override;
//     property DataInType;
//     property DataOutType;
//     property FilterDescription;
//     property NrSettings;
//     property Setting;
//     property SettingDimensionInfo;
//     property SettingInfo;
//   end;
// 
// (*
// { Mean filter
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = Lookback (s)
// }
//   TMeanFilter = class(TCustomFilter)
//   protected
//     FMedianList: TDoubleMedianList;
//     FSavedState: TDoubleMedianList;
//     function DoCheckSettings: Integer; override;
//     procedure DoClearSavedState; override;
//     procedure DoRestoreState; override;
//     procedure DoSaveState; override;
//     function GetSettingDimensionInfo(Index: Integer): String; override;
//     procedure InitializeMedianList;
//   public
//     constructor Create; override;
//     destructor Destroy; override;
//     procedure FilterSamples(var SamplesIn; var SamplesOut; IdxStart,IdxEnd:
//       Integer; OutIdxStart:Integer = -1); override;
//     procedure Reset; overload; override;
//     procedure Reset(UseNextSample: Boolean); overload; override;
//     property DataInType;
//     property DataOutType;
//     property FilterDescription;
//     property NrSettings;
//     property Setting;
//     property SettingDimensionInfo;
//     property SettingInfo;
//   end;
// *)
// 
// { Mean filter
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = Lookback (s)
// }
//   TMeanFilterQuickState = record
//     SavedHeadIdx: Integer;
//     SavedState: TDynDoubleArray;
//     SavedTailIdx: Integer;
//   end;
// 
//   TMeanFilterQuick = class(TCustomFilter)
//   private
//     function GetMean: Double;
//     procedure IncrementIdx(var Idx: Integer);
//   protected
//     FHeadIdx: Integer;
//     FSamples: TDynDoubleArray;
//     FSavedState: Array of TMeanFilterQuickState;
// //    FSavedHeadIdx: Integer;
// //    FSavedState: TDynDoubleArray;
// //    FSavedTailIdx: Integer;
//     FTailIdx: Integer;
//     function DoCheckSettings: Integer; override;
//     procedure CheckLengthSavedState(Index: Integer); override;
//     procedure DoClearSavedState(Index: Integer); override;
//     procedure DoRestoreState(Index: Integer); override;
//     procedure DoSaveState(Index: Integer); override;
//     function GetSettingDimensionInfo(Index: Integer): String; override;
//     procedure InitializeArray;
//   public
//     constructor Create; override;
//     destructor Destroy; override;
//     procedure FilterSamples(var SamplesIn; var SamplesOut; IdxStart,IdxEnd:
//       Integer; OutIdxStart:Integer = -1); override;
//     procedure Reset; overload; override;
//     procedure Reset(UseNextSample: Boolean); overload; override;
//     property DataInType;
//     property DataOutType;
//     property FilterDescription;
//     property NrSettings;
//     property Setting;
//     property SettingDimensionInfo;
//     property SettingInfo;
//   end;
// 
// { Median filter
//   FilterSettings : 1 = SampleFrequency (Hz)
//                    2 = FilterGain
//                    3 = %
//                    4 = Lookback (s)
// }
//   TPercentillianFilter = class(TCustomFilter)
//   protected
//     FMedianList: TDoubleMedianList;
//     FSavedState: Array of TDoubleMedianList;
//     function DoCheckSettings: Integer; override;
//     procedure CheckLengthSavedState(Index: Integer); override;
//     procedure DoClearSavedState(Index: Integer); override;
//     procedure DoRestoreState(Index: Integer); override;
//     procedure DoSaveState(Index: Integer); override;
//     function GetSettingDimensionInfo(Index: Integer): String; override;
//     procedure InitializeMedianList;
//   public
//     constructor Create; override;
//     destructor Destroy; override;
//     procedure FilterSamples(var SamplesIn; var SamplesOut; IdxStart,IdxEnd:
//       Integer; OutIdxStart:Integer = -1); override;
//     procedure Reset; overload; override;
//     procedure Reset(UseNextSample: Boolean); overload; override;
//     property DataInType;
//     property DataOutType;
//     property FilterDescription;
//     property NrSettings;
//     property Setting;
//     property SettingDimensionInfo;
//     property SettingInfo;
//   end;
// 
// 
// procedure GetAllFilterNames(const Strings: TStrings);
// function  GetFilterByName(FilterName:String): TCustomFilter;
// function  GetFilterType(const AFilter: TCustomFilter): TFilterType;
// 
// function FilterToStr(const AFilter: TCustomFilter): String;
// function StrToFilter(const AString: String): TCustomFilter;
// 
// function GetFilterDescriptionStr(const FilterName:String): String;
// 
// function ValidSampleFrequency(const ASampleFreq: Extended; const AFilterStr:
//     String; ADelimiters: String): Boolean;
// 
// implementation
// 
// Uses
// {$IFDEF INCLUDE_CODESITE}
//   CsIntf,
// {$ENDIF}
//   Math, TypInfo,
//   udRoot,uMath, udStr, uLocale, uConsts, StrUtils;
// 
// const
//   UnitFileName = SD7LIB+'\uFilter.pas';
// 
// type
//   TLP05RateFrequencyConvertor = class
//   protected
//     FFs,
//     FF3dB,
//     Frate: Extended;
//     function Hf(x: Extended): Extended;
//     function Hrate(x: Extended): Extended;
//   public
//     function ArgLP05(Fs,F,rate: Extended): Extended;
//     function Find3dBFrequency(Fs, rate, MaxError: Extended): Extended;
//     function Find3dBRate(Fs, F3dB, MaxError: Extended): Extended;
//   end;
// 
// function TLP05RateFrequencyConvertor.ArgLP05(Fs,F,rate: Extended): Extended;
// var
//   Alfa: Extended;
// begin
// // MaxValue = 1, MinValue = r/(2-r)
//   Alfa:=2*Pi*F/Fs;
//   result:=rate/(Sqrt(1-2*(1-rate)*Cos(Alfa)+Sqr(1-rate)));
// end;
// 
// function TLP05RateFrequencyConvertor.Find3dBFrequency(Fs, rate,
//   MaxError:Extended): Extended;
// var
//   Xf: Extended;
// begin
//   FFs:=Fs;
//   Frate:=rate;
//   If(FindRoot_NR(Hf,0,Fs/2,Xf,MaxError)) then
//     result:=Xf
//   else
//     result:=-1;
// end;
// 
// function TLP05RateFrequencyConvertor.Find3dBRate(Fs, F3dB, MaxError:
//   Extended): Extended;
// var
//   Xf: Extended;
// begin
//   FFs:=Fs;
//   FF3dB:=F3dB;
//   If(FindRoot_NR(Hrate,0,1,Xf,MaxError)) then
//     result:=Xf
//   else
//     result:=-1;
// end;
// 
// function TLP05RateFrequencyConvertor.Hf(x:Extended):Extended;
// begin
//   result:=ArgLP05(FFs,x,Frate);
//   result:=result-(0.5*Sqrt(2));
// end;
// 
// function TLP05RateFrequencyConvertor.Hrate(x: Extended): Extended;
// begin
//   result:=ArgLP05(FFs,FF3dB,x);
//   result:=result-(0.5*Sqrt(2));
// end;
// 
// //  RateFreq_MaxError = 1e-6;
// //  RateFreq_Freq: Extended = 0;
// //  RateFreq_Rate: Extended = 0;
// //
// //function Arg_LP05(r,x: Extended): Extended;
// //var
// //  Cx,r1 :Extended;
// //begin
// //  r1:=1-r;
// //  Cx:=Cos(x);
// //
// ///  HIER division by zero
// //
// //  Arg_LP05:=Sqrt(Sqr(r)*(1-2*r1*Cx+Sqr(r1))/
// //                (Sqr(1+Sqr(r1))-4*r1*(1+Sqr(r1)-r1*Cx)*Cx));
// //end;
// //
// //function LP05_rate(Rate: Extended): Extended;
// //begin
// //  LP05_rate:=Arg_LP05(Rate,RateFreq_Freq)-0.5*Sqrt(2)
// //end;
// //
// //function LP05_3dB_rate(Freq,X0,X1: Extended): Extended;
// //var
// //  Xf, Xerror :Extended;
// //begin
// //  RateFreq_Freq:=Freq;
// //  Xerror:=RateFreq_MaxError;
// //  If(FindRoot_NR(LP05_rate,X0,X1,Xf,Xerror)) then
// //    result:=Xf
// //  else
// //    result:=X0-1
// //end;
// //
// //function LP05_freq(x:Extended):Extended;
// //begin
// //  result:=Arg_LP05(RateFreq_Rate,x)-0.5*Sqrt(2)
// //end;
// //
// //function LP05_3dB_Freq(Rate,x0,x1: Extended): Extended;
// //var
// //  Xf,Xerror :Extended;
// //begin
// //  RateFreq_Rate:=Rate;
// //  Xerror:=RateFreq_MaxError;
// //  If(FindRoot_NR(LP05_freq,x0,x1,Xf,Xerror)) then
// //    LP05_3dB_freq:=Xf
// //  else
// //    LP05_3dB_freq:=X0-1
// //end;
// 
// 
// { TCustomFilter }
// 
// constructor TCustomFilter.Create;
// begin
//   inherited;
//   FDirection:=ForwardOnly;
//   FNewSettings:=True;
//   SetNrSettings(2);
//   FSettingInfo[0]:='Abstract Custom Filter';
//   Gain:=1;
// end;
// 
// destructor TCustomFilter.Destroy;
// begin
//   FSettingInfo:=Nil;
//   FSetting:=Nil;
//   inherited;
// end;
// 
// function TCustomFilter.CheckSettings: Integer;
// begin
//   result:=DoCheckSettings;
// end;
// 
// procedure TCustomFilter.ClearSavedState(Index: Integer);
// begin
//   DoClearSavedState(Index);
// end;
// 
// function TCustomFilter.DoCheckSettings: Integer;
// begin
//   If(Setting[1] <= 0) then                // SampleFrequency should be > 0 Hz
//     result:=1
//   else
//     result:=0;
// end;
// 
// procedure TCustomFilter.DoClearSavedState(Index: Integer);
// begin
// // default no state, only check length SavedState
//   CheckLengthSavedState(Index)
// end;
// 
// procedure TCustomFilter.DoRestoreState(Index: Integer);
// begin
// // default no state, only check length SavedState
//   CheckLengthSavedState(Index)
// end;
// 
// procedure TCustomFilter.DoSaveState(Index: Integer);
// begin
// // default no state, only check length SavedState
//   CheckLengthSavedState(Index)
// end;
// 
// function TCustomFilter.GetAsString: String;
// var
//   i: Integer;
// begin
//   result:='';
//   For i:=2 to NrSettings do
//     result:=result+FloatToStrF(Setting[i],ffGeneral,18,18,FS_LCID_US_ENG)+
//       IfThen((i <> NrSettings),'/');
//   result:=Format('%s(%s)',[FilterNames[GetFilterType(Self)],result]);
// end;
// 
// function TCustomFilter.GetEDFFilterStr: String;
// begin
//   result:=AsString;
// end;
// 
// function TCustomFilter.GetFilterDescription: String;
// begin
//   result:=SettingInfo[0];
// end;
// 
// function TCustomFilter.GetGain: Double;
// begin
//   result:=Setting[2];
// end;
// 
// function TCustomFilter.GetSampleFrequency: Double;
// begin
//   result:=Setting[1];
// end;
// 
// function TCustomFilter.GetSetting(Index:Integer):Double;
// begin
//   If((Index <= 0) or (Index > NrSettings)) then
//     raise ERangeError.Create(sIndexOutOfRange);
//   result:=FSetting[Index];
// end;
// 
// function TCustomFilter.GetSettingDimensionInfo(Index: Integer): String;
// begin
//   If((Index <= 0) or (Index > NrSettings)) then
//     raise ERangeError.Create(sIndexOutOfRange);
//   Case Index of
//     2 : result:='';
//   else
//     result:='Hz';
//   end;
// end;
// 
// function TCustomFilter.GetSettingInfo(Index:Integer):String;
// begin
//   If((Index < 0) or (Index > NrSettings)) then
//     raise ERangeError.Create(sIndexOutOfRange);
//   result:=FSettingInfo[Index];
// end;
// 
// function TCustomFilter.GetSettingIsReadOnly(Index: Integer): Boolean;
// begin
//   Assert(InRange(Index,0,FNrSettings),sIndexOutOfRange);
//   result:=False;
// end;
// 
// procedure TCustomFilter.Reset;
// begin
//   UseFirstSampleToReset:=False;
// end;
// 
// procedure TCustomFilter.Reset(UseNextSample: Boolean);
// begin
//   UseFirstSampleToReset:=UseNextSample;
// end;
// 
// procedure TCustomFilter.RestoreState(Index: Integer);
// begin
//   DoRestoreState(Index);
// end;
// 
// procedure TCustomFilter.SaveState(Index: Integer);
// begin
//   DoSaveState(Index);
// end;
// 
// procedure TCustomFilter.SetDataOutType(const Value: FilterDataType);
// begin
//   FDataOutType:=Value;
// end;
// 
// procedure TCustomFilter.SetGain(const Value: Double);
// begin
//   Setting[2]:=Value;
// end;
// 
// procedure TCustomFilter.SetNrSettings(Value: Integer);
// begin
//   If(Value <> High(FSettingInfo)-1) then
//     begin
//       FNrSettings:=Value;
//       SetLength(FSettingInfo,FNrSettings+1);                 // 0 = FilterName
//       SetLength(FSetting,FNrSettings+1);                // Index 0 is not used
//     end;
// end;
// 
// procedure TCustomFilter.SetSampleFrequency(const Value: Double);
// begin
//   Setting[1]:=Value;
// end;
// 
// procedure TCustomFilter.SetSetting(Index:Integer; const Value:Double);
// begin
//   Assert(InRange(Index,0,NrSettings),sIndexOutOfRange);
//   If(not SettingIsReadOnly[Index]) then
//     begin
//       FSetting[Index]:=Value;
//       FNewSettings:=True;
//     end;
// end;
// 
// procedure TCustomFilter.CheckLengthSavedState(Index: Integer);
// begin
// // Default no action
// end;
// 
// { TCustomIIRFilter }
// 
// constructor TCustomIIRFilter.Create;
// begin
//   inherited;
//   FAnticipate:=True;
//   SetNrSettings(2);
//   FSettingInfo[0]:='Abstract Custom IIRFilter';
//   FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='FilterGain';
// end;
// 
// destructor TCustomIIRFilter.Destroy;
// begin
// // Dispose state buffers
//   FZeros:=Nil;
//   FPoles:=Nil;
//   FFilterStateZ:=Nil;
//   FFilterStateP:=Nil;
//   inherited;
// end;
// 
// procedure TCustomIIRFilter.CalculateIIRCoeff;
// var
//   i :Integer;
// begin
//   i:=CheckSettings;
//   If(i <> 0) then
//     raise ERangeError.CreateFmt('%s: Filter setting %s out of range',
//         [Self.ClassName,SettingInfo[i]]);
//   FNewSettings:=False;
// end;
// 
// procedure TCustomIIRFilter.CheckLengthSavedState(Index: Integer);
// begin
//   If(Index > High(FSavedStateP)) then
//     begin
//       SetLength(FSavedStateP,Index+1);
//       SetLength(FSavedStateZ,Index+1);
//     end;
// end;
// 
// procedure TCustomIIRFilter.DoClearSavedState(Index: Integer);
// begin
//   inherited;
//   FSavedStateP[Index]:=nil;
//   FSavedStateZ[Index]:=nil;
// end;
// 
// procedure TCustomIIRFilter.DoRestoreState(Index: Integer);
// begin
//   inherited;
//   If(Assigned(FSavedStateP[Index])) then
//     begin
//       CopyDynamicArray(FSavedStateP[Index],FFilterStateP);
//       CopyDynamicArray(FSavedStateZ[Index],FFilterStateZ);
//     end;
// end;
// 
// procedure TCustomIIRFilter.DoSaveState(Index: Integer);
// begin
//   inherited;
//   CopyDynamicArray(FFilterStateP,FSavedStateP[Index]);
//   CopyDynamicArray(FFilterStateZ,FSavedStateZ[Index]);
// end;
// 
// procedure TCustomIIRFilter.FilterSamples(var SamplesIn; var SamplesOut;
//   IdxStart, IdxEnd, OutIdxStart: Integer);
// var
//   i,
//   Idx,
//   IdxLast,
//   OutIdx,
//   Step    :Integer;
//   r,s     :Extended;
// begin
//   s:=0;
//   If(FNewSettings) then
//     CalculateIIRCoeff;
//   If(IdxEnd <> IdxStart) then
//     Step:=Sign(Int64(IdxEnd-IdxStart))       // Filter from IdxStart to IdxEnd
//   else
//     Step:=1;                                             // Avoid endless loop
//   If(OutIdxStart < 0) then
//     OutIdx:=IdxStart
//   else
//     OutIdx:=OutIdxStart;
//   If(FDirection = ForwardOnly) then
//     begin
//       Idx:=IdxStart;
//       IdxLast:=IdxEnd+1
//     end
//   else
//     begin
//       Idx:=IdxEnd;
//       IdxLast:=IdxStart-1;
//       Step:=-Step
//     end;
//   While(Idx <> Idxlast) do
//     begin
//       Case FDataInType of
//         fdSmallInt : FFilterStateZ[0]:=TDynSmallIntArray(SamplesIn)[Idx];
//         fdInteger  : FFilterStateZ[0]:=TDynIntegerArray(SamplesIn)[Idx];
//         fdSingle   : FFilterStateZ[0]:=TDynSingleArray(SamplesIn)[Idx];
//         fdDouble   : FFilterStateZ[0]:=TDynDoubleArray(SamplesIn)[Idx];
//       end;
//       If(UseFirstSampleToReset) then
//         begin
//           Reset(FFilterStateZ[0]);
//           FUseFirstSampleToReset:=False;
//         end;
// // Compute new output sample
//       r:=0;
// // Add past output-values
//       For i:=1 to High(FPoles) do
//         r:=r+FPoles[i]*FFilterStateP[i];
// // Not anticipate = do not include current input sample in output value
//       If(Not Anticipate) then
//         s:=r;
// // Add past input-values
//       For i:=0 to High(FZeros) do
//         r:=r+FZeros[i]*FFilterStateZ[i];
// // Anticipate = include current input sample in output value
//       If(Anticipate) then
//         s:=r;
// // Do backpolation (FilterStateP[1] = Last out-sample)
//       s:=Backpolate*FFilterstateP[1]+(1.0-Backpolate)*s;
// // Scale result
//       Case DataOutType of
//         fdSmallInt : begin
//                        If(Abs(Round(s)) > MaxSmallInt) then
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Sign(s)*MaxSmallInt
//                        else
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdInteger  : begin
//                        If(Abs(Round(s)) > MaxInt) then
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Sign(s)*MaxInt
//                        else
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdSingle   : begin
//                        TDynSingleArray(SamplesOut)[OutIdx]:=
//                          Max(-MaxSingle,Min(s,MaxSingle));
//                      end;
//         fdDouble   : begin
//                        TDynDoubleArray(SamplesOut)[OutIdx]:=
//                          Max(-MaxDouble,Min(s,MaxDouble));
//                      end;
//       end;
// // Update filter state
//       For i:=High(FPoles) downto 2 do
//         FFilterStateP[i]:=FFilterStateP[i-1];
//       FFilterStateP[1]:=r;
//       For i:=High(FZeros) downto 1 do
//         FFilterStateZ[i]:=FFilterStateZ[i-1];
// // Next sample
//       Inc(Idx,Step);
//       Inc(OutIdx,Step)
//     end
// end;
// 
// procedure TCustomIIRFilter.Reset;
// var
//   i :Integer;
// begin
//   inherited Reset;
//   If(FNewSettings) then
//     CalculateIIRCoeff;
//   If(FFilterStateZ <> Nil) then
//     For i:=Low(FFilterStateZ) to High(FFilterStateZ) do
//       FFilterStateZ[i]:=0;
//   If(FFilterStateP <> Nil) then
//     For i:=Low(FFilterStateP) to High(FFilterStateP) do
//       FFilterStateP[i]:=0;
// end;
// 
// procedure TCustomIIRFilter.Reset(UseNextSample: Boolean);
// begin
//   inherited Reset(UseNextSample);
//   If(not UseNextSample) then
//     Reset;
// end;
// 
// procedure TCustomIIRFilter.Reset(Xn: Double);
// var
//   i :Integer;
//   r,s: Double;
//   Y: Double;
// begin
//   If(FNewSettings) then
//     CalculateIIRCoeff;
//   r:=0;
//   For i:=0 to High(FZeros) do
//     r:=r+FZeros[i];
//   s:=1;
//   For i:=1 to High(FPoles) do
//     s:=s-FPoles[i];
//   Y:=Xn*r/s;
//   For i:=Low(FFilterStateZ) to High(FFilterStateZ) do
//     FFilterStateZ[i]:=Xn;
//   For i:=Low(FFilterStateP) to High(FFilterStateP) do
//     FFilterStateP[i]:=Y;
// end;
// 
// procedure TCustomIIRFilter.SetGain(const Value: Double);
// begin
//   inherited;
//   If(CheckSettings = 0) then
//     CalculateIIRCoeff;
// end;
// 
// { TLPFilter }
// 
// constructor TLPFilter.Create;
// begin
//   inherited;
//   SetNrSettings(3);
//   FSettingInfo[0]:='First order low-pass';
//   FSettingInfo[3]:='-3dB frequency';
// end;
// 
// procedure TLPFilter.CalculateIIRCoeff;
// var
//   r,t: Double;
// begin
//   inherited;
//   SetLength(FZeros,2);                                              // 1 Zeros
//   SetLength(FFilterStateZ,2);
//   SetLength(FPoles,2);                               // 1 Pole, Idx 0 not used
//   SetLength(FFilterStateP,3);                               // NrPoles+1 !!!!!
// // Settings: 1=SampleFreq, 2=Gain, 3=FilterFreq
//   r:=CoTan(Pi*(Setting[3]/Setting[1]));
//   t:=Gain/(r+1.0);
//   FZeros[0]:=t;
//   FZeros[1]:=t;
//   FPoles[0]:=1.0;
//   FPoles[1]:=-(1.0-r)/(1.0+r);
// end;
// 
// function TLPFilter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     begin
// // -3dB frequency < Fsample/2
//       If((Setting[3] <= 0) or (Setting[3] > Setting[1]/2)) then
//         result:=3;
//     end;
// end;
// 
// function TLPFilter.GetEDFFilterStr: String;
// begin
//   result:=Format('LP:%gHz',[Setting[3]],FS_LCID_US_ENG);
// end;
// 
// { THPFilter }
// 
// constructor THPFilter.Create;
// begin
//   inherited;
//   SetNrSettings(3);
//   FSettingInfo[0]:='First order high-pass';
//   FSettingInfo[3]:='-3dB frequency';
// end;
// 
// procedure THPFilter.CalculateIIRCoeff;
// var
//   r,t: Double;
// begin
//   inherited;
//   SetLength(FZeros,2);                                              // 1 Zeros
//   SetLength(FFilterStateZ,2);
//   SetLength(FPoles,2);                               // 1 Pole, Idx 0 not used
//   SetLength(FFilterStateP,3);                               // NrPoles+1 !!!!!
// // Settings: 1=SampleFreq, 2=Gain, 3=FilterFreq
//   r:=2.0*Pi*Setting[3]*((1/Setting[1])/2);
//   t:=Gain/(r+1.0);
//   FZeros[0]:=t;
//   FZeros[1]:=-t;
//   FPoles[0]:=1.0;
//   FPoles[1]:=-(r-1.0)/(r+1.0);
// end;
// 
// function THPFilter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     begin
// // -3dB frequency < Fsample/2
//       If((Setting[3] <= 0) or (Setting[3] > Setting[1]/2)) then
//         result:=3;
//     end;
// end;
// 
// function THPFilter.GetEDFFilterStr: String;
// begin
//   result:=Format('HP:%gHz',[Setting[3]],FS_LCID_US_ENG);
// end;
// 
// { TDUEFilter }
// 
// constructor TDUEFilter.Create;
// begin
//   inherited;
//   SetNrSettings(3);
//   FSettingInfo[0]:='Inverse low-pass & integrator';
//   FSettingInfo[3]:='-3dB frequency';
// end;
// 
// procedure TDUEFilter.CalculateIIRCoeff;
// Var
//   Fprewarp,
//   r,s,Ts    :Double;
// begin
//   inherited;
//   SetLength(FZeros,2);                                                // 1 Zero
//   SetLength(FFilterStateZ,2);
//   SetLength(FPoles,1);                              // 0 Poles, Idx 0 not used
//   SetLength(FFilterStateP,2);                               // NrPoles+1 !!!!!
// { Settings: 1=SampleFreq, 2=Gain, 3=FilterFreq }
//   Ts:=1.0/Setting[1];
//   Fprewarp:=Tan(Pi*Setting[3]*Ts)/(Pi*Ts);
//   r:=1.0/(2.0*Pi*Fprewarp);
//   s:=Ts/2.0;
//   FZeros[0]:=Gain*(s+r);
//   FZeros[1]:=Gain*(s-r);
//   FPoles[0]:=1.0;
// end;
// 
// function TDUEFilter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     begin
//       If((Setting[3] <= 0) or (Setting[3] > Setting[1]/2)) then
//         result:=3;
//     end;
// end;
// 
// { TDUE1987Filter }
// 
// procedure TDUE1987Filter.CalculateIIRCoeff;
// var
//   Fprewarp,
//   r,s,Ts    :Double;
// begin
//   inherited;
// { Settings: 1=SampleFreq, 2=Gain, 3=FilterFreq }
//   Ts:=1.0/Setting[1];
//   Fprewarp:=Tan(Pi*Setting[3]*Ts)/(Pi*Ts);
//   r:=1.0/(2.0*Pi*Fprewarp);
//   s:=Ts;
//   FZeros[0]:=Gain*(r);
//   FZeros[1]:=Gain*(s-r);
//   FPoles[0]:=1.0;
// end;
// 
// { TDUE2007Filter }
// 
// procedure TDUE2007Filter.CalculateIIRCoeff;
// var
//   Fprewarp,
//   r,s,Ts    :Double;
// begin
//   inherited;
// { Settings: 1=SampleFreq, 2=Gain, 3=FilterFreq }
//   Ts:=1.0/Setting[1];
//   Fprewarp:=Tan(Pi*Setting[3]*Ts)/(Pi*Ts);
//   r:=1.0/(2.0*Pi*Fprewarp);
//   s:=Ts;
//   FZeros[0]:=Gain*(s+r);
//   FZeros[1]:=Gain*(-r);
//   FPoles[0]:=1.0;
// end;
// 
// { TSEFilter }
// 
// constructor TSEFilter.Create;
// begin
//   inherited;
//   SetNrSettings(5);
//   FSettingInfo[0]:='Inverse of low-pass & G';
//   FSettingInfo[3]:='Cut-off frequency';
//   FSettingInfo[4]:='Center frequency';
//   FSettingInfo[5]:='Bandwidth';
// end;
// 
// procedure TSEFilter.CalculateIIRCoeff;
// var
//   Fprewarp,
//   r,s,t,Ts :Double;
// begin
//   inherited;
//   SetLength(FZeros,3);                                              // 3 Zeros
//   SetLength(FFilterStateZ,3);
//   SetLength(FPoles,3);                              // 2 Poles, Idx 0 not used
//   SetLength(FFilterStateP,4);                               // NrPoles+1 !!!!!
// // Settings: 1=SampleFreq, 2=Gain, 3=Cut-off, 4=Center-freq 5=Bandwidth
//   Ts:=1.0/Setting[1];
//   Fprewarp:=Tan(Setting[4]*Pi*Ts)/(Pi*Ts);
//   r:=Sqr(2.0*Pi*Fprewarp*Ts);
// { From November 1992 prewarping applied because of Arends results !
//   r:=sqr(2.0*pi*f0*Ts);                         No prewarping
// }
//   s:=2.0*Pi*Setting[5]*Ts*2.0;
//   t:=4.0+r+s;
//   FPoles[0]:=1.0;
//   FPoles[1]:=( 8.0-2.0*r)/t;
//   FPoles[2]:=(-4.0+s-r)/t;
//   Fprewarp:=Tan(Setting[3]*Pi*Ts)/(Pi*Ts);
//   r:=2.0/(2.0*Pi*Fprewarp);
// { From November 1992 prewarping applied because of Arends results !
//   r:=2.0/(2.0*pi*fc);
// }
//   s:=Gain*2.0*Pi*Setting[5]*2.0;
//   FZeros[0]:=s*(r+Ts)/t;
//   FZeros[1]:=s*(-2.0*r)/t;
//   FZeros[2]:=s*(r-Ts)/t;
// end;
// 
// function TSEFilter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     begin
//       If((Setting[3] <= 0) or (Setting[3] > Setting[1]/2)) then
//         result:=3
//       else
//         If((Setting[4] <= 0) or (Setting[4] > Setting[1]/2)) then
//           result:=4
//         else
//           If((Setting[5] <= 0) or (Setting[5] > Setting[1]/2)) then
//             result:=5;
//     end;
// end;
// 
// { TNotchFilter }
// 
// constructor TNotchFilter.Create;
// begin
//   inherited;
//   SetNrSettings(4);
//   FSettingInfo[0]:='Single notch';
//   FSettingInfo[3]:='Center frequency';
//   FSettingInfo[4]:='Bandwidth (-3dB)';
// end;
// 
// procedure TNotchFilter.CalculateIIRCoeff;
// var
//   a,b,x,y,r: Double;
// begin
//   inherited;
//   SetLength(FZeros,3);                                              // 3 Zeros
//   SetLength(FFilterStateZ,3);
//   SetLength(FPoles,3);                              // 2 Poles, Idx 0 not used
//   SetLength(FFilterStateP,4);                               // NrPoles+1 !!!!!
// // Settings: 1=SampleFreq, 2=Gain, 3=FilterFreq 4=Bandwidth
//   a:=2*Pi*(Setting[3]/Setting[1]);
//   b:=2*Pi*((Setting[3]-(Setting[4]/2))/Setting[1]);
//   r:=1-Sqrt(Sqr(cos(a)-cos(b))+Sqr(sin(a)-sin(b)))*Sqrt(Sqr(Exp(1))-1);
//   x:=Gain*cos(a);
//   y:=Gain*sin(a);
//   FZeros[0]:=Gain;
//   FZeros[1]:=(-2*x);
//   FZeros[2]:=(Sqr(x)+Sqr(y));
//   FPoles[0]:=1.0;
//   FPoles[1]:=2*r*x;
//   FPoles[2]:=-(Sqr(r*x)+Sqr(r*y));
// end;
// 
// function TNotchFilter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     begin
// // Gain should be 1 to avoid instabillity
//       If(not SameValue(Gain,1)) then
//         result:=2
//       else
// // Notch frequency < Fsample/2
//         If((Setting[3] <= 0) or (Setting[3] > Setting[1]/2)) then
//           result:=3
//         else
//           If((Setting[4] <= 0) or (Setting[4] > Setting[1]/4)) then
//             result:=4;
//     end;
// end;
// 
// function TNotchFilter.GetEDFFilterStr: String;
// begin
//   result:=Format('N:%gHz',[Setting[3]],FS_LCID_US_ENG);
// end;
// 
// function TNotchFilter.GetSettingIsReadOnly(Index: Integer): Boolean;
// begin
//   result:=inherited GetSettingIsReadOnly(Index);
//   result:=result or (Index = 2);
// end;
// 
// procedure TNotchFilter.SetSetting(Index: Integer; const Value: Double);
// begin
//   inherited;
//   If(Index = 2) then
//     FSetting[Index]:=1;
// end;
// 
// { TGFilter }
// 
// constructor TGFilter.Create;
// begin
//   inherited;
//   SetNrSettings(4);
//   FSettingInfo[0]:='Second order resonator';
//   FSettingInfo[3]:='Center frequency';
//   FSettingInfo[4]:='Bandwidth (-3dB)';
// end;
// 
// procedure TGFilter.CalculateIIRCoeff;
// var
//   Fprewarp,
//   Ts,
//   r,s,t    :Double;
// begin
//   inherited;
//   SetLength(FZeros,3);                                              // 3 Zeros
//   SetLength(FFilterStateZ,3);
//   SetLength(FPoles,3);                              // 2 Poles, Idx 0 not used
//   SetLength(FFilterStateP,4);                               // NrPoles+1 !!!!!
// // Settings: 1=SampleFreq, 2=Gain, 3=FilterFreq 4=Bandwidth
//   Ts:=1.0/Setting[1];
//   Fprewarp:=Tan(Setting[3]*Pi*Ts)/(Pi*Ts);
//   r:=4.0*pi*Setting[4]*Ts;
//   s:=sqr(2.0*Pi*Fprewarp*Ts);
//   t:=4.0+r+s;
//   FZeros[0]:= Gain*r/t;
//   FZeros[1]:=0;
//   FZeros[2]:=-Gain*r/t;
//   FPoles[0]:=1.0;
//   FPoles[1]:=-(-8.0+2.0*s)/t;
//   FPoles[2]:=-(4.0-r+s)/t;
// end;
// 
// function TGFilter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     begin
// // Filter frequency < Fsample/2
//       If((Setting[3] <= 0) or (Setting[3] > Setting[1]/2)) then
//         result:=3
//       else
//         If((Setting[4] <= 0) or (Setting[4] > Setting[1]/4)) then
//           result:=4;
//     end;
// end;
// 
// { TInvGFilter }
// 
// constructor TInvGFilter.Create;
// begin
//   inherited;
//   SetNrSettings(4);
//   FSettingInfo[0]:='Inverse of second order resonator';
//   FSettingInfo[3]:='Center frequency';
//   FSettingInfo[4]:='Bandwidth (-3dB)';
// end;
// 
// procedure TInvGFilter.CalculateIIRCoeff;
// var
//   Fprewarp,
//   Ts,
//   r,s,t    :Double;
// begin
//   inherited;
//   SetLength(FZeros,3);                                              // 3 Zeros
//   SetLength(FFilterStateZ,3);
//   SetLength(FPoles,3);                                              // 3 Poles
//   SetLength(FFilterStateP,4);                               // NrPoles+1 !!!!!
// // Settings: 1=SampleFreq, 2=Gain, 3=FilterFreq 4=Bandwidth
//   Ts:=1.0/Setting[1];
//   Fprewarp:=Tan(Setting[3]*Pi*Ts)/(Pi*Ts);
//   r:=4.0*pi*Setting[4]*Ts;
//   s:=sqr(2.0*Pi*Fprewarp*Ts);
//   t:=4.0+r+s;
//   FZeros[0]:=(4+r*(1-Gain)+s)/t;
//   FZeros[1]:=(-8+2*s)/t;
//   FZeros[2]:=(4-r*(1-Gain)+s)/t;
//   FPoles[0]:=1.0;
//   FPoles[1]:=-(-8.0+2.0*s)/t;
//   FPoles[2]:=-(4.0-r+s)/t;
// end;
// 
// function TInvGFilter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     begin
// // Filter frequency < Fsample/2
//       If((Setting[3] <= 0) or (Setting[3] > Setting[1]/2)) then
//         result:=3
//       else
//         If((Setting[4] <= 0) or (Setting[4] > Setting[1]/4)) then
//           result:=4;
//     end;
// end;
// 
// { TBPFilter }
// 
// constructor TBPFilter.Create;
// begin
//   inherited;
//   SetNrSettings(4);
//   FSettingInfo[0]:='Second order band-pass';
//   FSettingInfo[3]:='High-pass -3dB frequency';
//   FSettingInfo[4]:='Low-pass  -3dB frequency';
// end;
// 
// procedure TBPFilter.CalculateIIRCoeff;
// var
//   r,s,t    :Double;
// begin
//   inherited;
//   SetLength(FZeros,3);                                              { 3 Zeros }
//   SetLength(FFilterStateZ,3);
//   SetLength(FPoles,3);                                              { 3 Poles }
//   SetLength(FFilterStateP,4);                               { NrPoles+1 !!!!! }
// { Settings: 1=SampleFreq, 2=Gain, 3=HighPassFrequency 4=LowPassFrequency}
//   r:=2.0*Pi*Setting[4]*(1/Setting[1]);
//   s:=2.0*Pi*Setting[3]*(1/Setting[1]);
//   t:=4.0+2.0*(r+s)+r*s;
//   FZeros[0]:= Gain*2.0*r/t;
//   FZeros[1]:=0.0;
//   FZeros[2]:=-Gain*2.0*r/t;
//   FPoles[0]:=1.0;
//   FPoles[1]:=( 8.0-2.0*r*s)/t;
//   FPoles[2]:=(-4.0+2.0*(r+s)-r*s)/t;
// end;
// 
// function TBPFilter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     begin
// { Filter frequency < Fsample/2 }
//       If((Setting[3] <= 0) or (Setting[3] > Setting[1]/2)) then
//         result:=3
//       else
//         If((Setting[4] <= 0) or (Setting[4] > Setting[1]/2)) then
//           result:=4;
//     end;
// end;
// 
// function TBPFilter.GetEDFFilterStr: String;
// begin
//   result:=Format('HP:%gHz LP:%gHz',[Setting[3],Setting[4]],FS_LCID_US_ENG);
// end;
// 
// { TB12PFilter }
// 
// constructor TBP12Filter.Create;
// begin
//   inherited;
//   SetNrSettings(4);
//   FSettingInfo[0]:='1st order HP & 2nd order LP';
//   FSettingInfo[3]:='High-pass -3dB frequency';
//   FSettingInfo[4]:='Low-pass  -3dB frequency';
// end;
// 
// procedure TBP12Filter.CalculateIIRCoeff;
// var
//   r,s,t    :Double;
// begin
//   inherited;
//   SetLength(FZeros,4);                                              { 3 Zeros }
//   SetLength(FFilterStateZ,4);
//   SetLength(FPoles,4);                                              { 3 Poles }
//   SetLength(FFilterStateP,5);                               { NrPoles+1 !!!!! }
// { Settings: 1=SampleFreq, 2=Gain, 3=HighPassFrequency 4=LowPassFrequency}
//   r:=2.0*Pi*Setting[4]*(1/Setting[1]);
//   s:=2.0*Pi*Setting[3]*(1/Setting[1]);
//   t:=1.0+s/2.0+r+Sqr(r)/4.0+r*s/2.0+s*Sqr(r)/8.0;
//   FZeros[0]:=Gain*Sqr(r)/4.0/t;
//   FZeros[1]:= FZeros[0];
//   FZeros[2]:=-FZeros[0];
//   FZeros[3]:=-FZeros[0];
//   FPoles[0]:=1.0;
//   FPoles[1]:=(3.0+s/2.0+r-Sqr(r)/4.0-r*s/2.0-s*Sqr(r)/8.0)/t;
//   FPoles[2]:=(-3.0+s/2.0+r+Sqr(r)/4.0+r*s/2.0-3.0*s*Sqr(r)/8.0)/t;
//   FPoles[3]:=(1.0-s/2.0-r+Sqr(r)/4.0+r*s/2.0-s*Sqr(r)/8.0)/t;
// end;
// 
// function TBP12Filter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     begin
// { Filter frequency < Fsample/2 }
//       If((Setting[3] <= 0) or (Setting[3] > Setting[1]/2)) then
//         result:=3
//       else
//         If((Setting[4] <= 0) or (Setting[4] < Setting[3]) or
//                                               (Setting[4] > Setting[1]/2)) then
//           result:=4;
//     end;
// end;
// 
// constructor TDCattFilter.Create;
// begin
//   inherited;
//   SetNrSettings(4);
//   FSettingInfo[0]:='DC attenuator';
// { FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='FilterGain'; }
//   FSettingInfo[3]:='0 < DC gain < 1';
//   FSettingInfo[4]:='DC Bandwidth';
// end;
// 
// { TDCattFilter }
// 
// procedure TDCattFilter.CalculateIIRCoeff;
// var
//   DCattenuation: Extended;
//   Fe: Extended;
//   T: Extended;
//   Fe_prewarp: Extended;
//   Wr: Extended;
//   We: Extended;
// begin
//   inherited;
//   SetLength(FZeros,2);                                              { 2 Zeros }
//   SetLength(FFilterStateZ,2);
//   SetLength(FPoles,2);                                               { 1 Pole }
//   SetLength(FFilterStateP,3);                               { NrPoles+1 !!!!! }
// { Settings: 1=SampleFreq, 2=Gain, 3=DCattenuation 4=HighPassFrequency }
//   DCattenuation:=Setting[3];
//   Fe:=Setting[4];
//   T:=1/Setting[1];
//   Fe_prewarp:=Tan(Fe*Pi*T)/(Pi*T);
//   We:=2*Pi*Fe_prewarp;
//   Wr:=((1-DCattenuation)/DCattenuation)*We;
//   FZeros[0]:=Gain * (1+(T/2)*We) / (1+(T/2)*(We+Wr));
//   FZeros[1]:=Gain * (-1+(T/2)*We) / (1+(T/2)*(We+Wr));
//   FPoles[0]:=1.0;                                // This value will not be used
//   FPoles[1]:=(1-(T/2)*(We+Wr)) / (1+(T/2)*(We+Wr));
// end;
// 
// function TDCattFilter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     begin
// // 0 < DC gain < 1
//       If((Setting[3] <= 0) or (Setting[3] >= 1)) then
//         result:=3;
// // -3dB frequency < Fsample/2
//       If((Setting[4] <= 0) or (Setting[4] > Setting[1]/2)) then
//         result:=4;
//     end;
// end;
// 
// function TDCattFilter.GetSettingDimensionInfo(Index: Integer): String;
// begin
//   Case Index of
//     3 : result:='';
//   else
//     result:=inherited GetSettingDimensionInfo(Index);
//   end;
// end;
// 
// constructor TINVDCattFilter.Create;
// begin
//   inherited;
//   SetNrSettings(4);
//   FSettingInfo[0]:='DC un-attenuator';
// { FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='FilterGain'; }
//   FSettingInfo[3]:='0 < DC gain < 1';
//   FSettingInfo[4]:='DC Bandwidth';
// end;
// 
// { TINVDCattFilter }
// 
// procedure TINVDCattFilter.CalculateIIRCoeff;
// var
//   DCattenuation: Extended;
//   Fe: Extended;
//   T: Extended;
//   Fe_prewarp: Extended;
//   Wr: Extended;
//   We: Extended;
// begin
//   inherited;
//   SetLength(FZeros,2);                                              { 2 Zeros }
//   SetLength(FFilterStateZ,2);
//   SetLength(FPoles,2);                                               { 1 Pole }
//   SetLength(FFilterStateP,3);                               { NrPoles+1 !!!!! }
// { Settings: 1=SampleFreq, 2=Gain, 3=DCgain 4=HighPassFrequency }
//   DCattenuation:=Setting[3];
//   Fe:=Setting[4];
//   T:=1/Setting[1];
//   Fe_prewarp:=Tan(Fe*Pi*T)/(Pi*T);
//   We:=2*Pi*Fe_prewarp;
//   Wr:=((1-DCattenuation)/DCattenuation)*We;
//   FZeros[0]:=Gain * (1+(T/2)*(We+Wr)) / (1+(T/2)*We);
//   FZeros[1]:=Gain * (-1+(T/2)*(We+Wr)) / (1+(T/2)*We);
//   FPoles[0]:=1.0;                                // This value will not be used
//   FPoles[1]:=(1-(T/2)*We) / (1+(T/2)*We);
// end;
// 
// function TINVDCattFilter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     begin
// // 0 < DC gain < 1
//       If((Setting[3] <= 0) or (Setting[3] >= 1)) then
//         result:=3;
// // -3dB frequency < Fsample/2
//       If((Setting[4] <= 0) or (Setting[4] > Setting[1]/2)) then
//         result:=4;
//     end;
// end;
// 
// function TINVDCattFilter.GetSettingDimensionInfo(Index: Integer): String;
// begin
//   Case Index of
//     3 : result:='';
//   else
//     result:=inherited GetSettingDimensionInfo(Index);
//   end;
// end;
// 
// { TRectifier }
// 
// constructor TRectifier.Create;
// begin
//   inherited;
//   SetNrSettings(2);
//   FSettingInfo[0]:='Rectifier';
//   FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='FilterGain';
// end;
// 
// function TRectifier.DoCheckSettings: Integer;
// begin
//   If(Setting[1] <= 0) then                { SampleFrequency should be > 0 Hz }
//     result:=1
//   else
//     result:=0;
// end;
// 
// procedure TRectifier.FilterSamples(var SamplesIn; var SamplesOut;
//                           IdxStart, IdxEnd: Integer; OutIdxStart:Integer = -1);
// var
//   Idx, IdxLast, Step, OutIdx: Integer;
//   s: Extended;
// begin
//   s:=0;
//   If(IdxEnd <> IdxStart) then
//     Step:=Sign(Int64(IdxEnd-IdxStart))       { Filter from IdxStart to IdxEnd }
//   else
//     Step:=1;                                             { Avoid endless loop }
//   If(OutIdxStart < 0) then
//     OutIdx:=IdxStart
//   else
//     OutIdx:=OutIdxStart;
//   If(FDirection = ForwardOnly) then
//     begin
//       Idx:=IdxStart;
//       IdxLast:=IdxEnd+1
//     end
//   else
//     begin
//       Idx:=IdxEnd;
//       IdxLast:=IdxStart-1;
//       Step:=-Step
//     end;
//   While(Idx <> Idxlast) do
//     begin
//       Case FDataInType of
//         fdSmallInt : s:=TDynSmallIntArray(SamplesIn)[Idx];
//         fdInteger  : s:=TDynIntegerArray(SamplesIn)[Idx];
//         fdSingle   : s:=TDynSingleArray(SamplesIn)[Idx];
//         fdDouble   : s:=TDynDoubleArray(SamplesIn)[Idx];
//       end;
// { Compute new output sample }
//       s:=Gain*Abs(s);
//       Case DataOutType of
//         fdSmallInt : begin
//                        If(Abs(Round(s)) > MaxSmallInt) then
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Sign(s)*MaxSmallInt
//                        else
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdInteger  : begin
//                        If(Abs(Round(s)) > MaxInt) then
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Sign(s)*MaxInt
//                        else
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdSingle   : TDynSingleArray(SamplesOut)[OutIdx]:=s;
//         fdDouble   : TDynDoubleArray(SamplesOut)[OutIdx]:=s;
//       end;
// { Next sample }
//       Inc(Idx,Step);
//       Inc(OutIdx,Step);
//     end;
// end;
// 
// procedure TRectifier.Reset;
// begin
// // Nothing to reset. Filter is stateless
// end;
// 
// procedure TRectifier.Reset(UseNextSample: Boolean);
// begin
// // Nothing to reset. Filter is stateless
// end;
// 
// { TdBFilter }
// 
// constructor TdBFilter.Create;
// begin
//   inherited;
//   SetNrSettings(3);
//   FSettingInfo[0]:='dB Converter';
//   FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='FilterGain';
//   FSettingInfo[3]:='0dB corresponds to...';
// end;
// 
// function TdBFilter.DoCheckSettings: Integer;
// begin
//   If(Setting[1] <= 0) then                { SampleFrequency should be > 0 Hz }
//     result:=1
//   else
//     If(IsZero(Setting[3])) then
//       result:=3
//     else
//       result:=0;
// end;
// 
// procedure TdBFilter.FilterSamples(var SamplesIn, SamplesOut; IdxStart,
//   IdxEnd, OutIdxStart: Integer);
// var
//   Idx, IdxLast, Step, OutIdx: Integer;
//   s: Extended;
// begin
//   s:=0;
//   If(IdxEnd <> IdxStart) then
//     Step:=Sign(Int64(IdxEnd-IdxStart))       { Filter from IdxStart to IdxEnd }
//   else
//     Step:=1;                                             { Avoid endless loop }
//   If(OutIdxStart < 0) then
//     OutIdx:=IdxStart
//   else
//     OutIdx:=OutIdxStart;
//   If(FDirection = ForwardOnly) then
//     begin
//       Idx:=IdxStart;
//       IdxLast:=IdxEnd+1
//     end
//   else
//     begin
//       Idx:=IdxEnd;
//       IdxLast:=IdxStart-1;
//       Step:=-Step
//     end;
//   While(Idx <> Idxlast) do
//     begin
//       Case FDataInType of
//         fdSmallInt : s:=TDynSmallIntArray(SamplesIn)[Idx];
//         fdInteger  : s:=TDynIntegerArray(SamplesIn)[Idx];
//         fdSingle   : s:=TDynSingleArray(SamplesIn)[Idx];
//         fdDouble   : s:=TDynDoubleArray(SamplesIn)[Idx];
//       end;
// { Compute new output sample }
//       s:=Gain*(20*Log10(Max(5.0*10e-324,s/Setting[3])));
//       Case DataOutType of
//         fdSmallInt : begin
//                        If(Abs(Round(s)) > MaxSmallInt) then
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Sign(s)*MaxSmallInt
//                        else
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdInteger  : begin
//                        If(Abs(Round(s)) > MaxInt) then
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Sign(s)*MaxInt
//                        else
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdSingle   : TDynSingleArray(SamplesOut)[OutIdx]:=s;
//         fdDouble   : TDynDoubleArray(SamplesOut)[OutIdx]:=s;
//       end;
// { Next sample }
//       Inc(Idx,Step);
//       Inc(OutIdx,Step);
//     end;
// end;
// 
// procedure TdBFilter.Reset;
// begin
// // Nothing to reset. Filter is stateless
// end;
// 
// function TdBFilter.GetSettingDimensionInfo(Index: Integer): String;
// begin
//   Case Index of
//     3 : result:='';
//   else
//     result:=inherited GetSettingDimensionInfo(Index);
//   end;
// end;
// 
// procedure TdBFilter.Reset(UseNextSample: Boolean);
// begin
// // Nothing to reset. Filter is stateless
// end;
// 
// { TSqrFilter }
// 
// constructor TSqrFilter.Create;
// begin
//   inherited;
//   SetNrSettings(2);
//   FSettingInfo[0]:='Square (x^2)';
//   FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='FilterGain';
// end;
// 
// function TSqrFilter.DoCheckSettings: Integer;
// begin
//   If(Setting[1] <= 0) then                { SampleFrequency should be > 0 Hz }
//     result:=1
//   else
//     result:=0;
// end;
// 
// procedure TSqrFilter.FilterSamples(var SamplesIn; var SamplesOut;
//                           IdxStart, IdxEnd: Integer; OutIdxStart:Integer = -1);
// var
//   Idx, IdxLast, Step, OutIdx: Integer;
//   s: Extended;
// begin
//   s:=0;
//   If(IdxEnd <> IdxStart) then
//     Step:=Sign(Int64(IdxEnd-IdxStart))       { Filter from IdxStart to IdxEnd }
//   else
//     Step:=1;                                             { Avoid endless loop }
//   If(OutIdxStart < 0) then
//     OutIdx:=IdxStart
//   else
//     OutIdx:=OutIdxStart;
//   If(FDirection = ForwardOnly) then
//     begin
//       Idx:=IdxStart;
//       IdxLast:=IdxEnd+1
//     end
//   else
//     begin
//       Idx:=IdxEnd;
//       IdxLast:=IdxStart-1;
//       Step:=-Step
//     end;
//   While(Idx <> Idxlast) do
//     begin
//       Case FDataInType of
//         fdSmallInt : s:=TDynSmallIntArray(SamplesIn)[Idx];
//         fdInteger  : s:=TDynIntegerArray(SamplesIn)[Idx];
//         fdSingle   : s:=TDynSingleArray(SamplesIn)[Idx];
//         fdDouble   : s:=TDynDoubleArray(SamplesIn)[Idx];
//       end;
// { Compute new output sample }
//       s:=Gain*Sqr(s);
//       Case DataOutType of
//         fdSmallInt : begin
//                        If(Abs(Round(s)) > MaxSmallInt) then
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Sign(s)*MaxSmallInt
//                        else
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdInteger  : begin
//                        If(Abs(Round(s)) > MaxInt) then
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Sign(s)*MaxInt
//                        else
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdSingle   : TDynSingleArray(SamplesOut)[OutIdx]:=s;
//         fdDouble   : TDynDoubleArray(SamplesOut)[OutIdx]:=s;
//       end;
// { Next sample }
//       Inc(Idx,Step);
//       Inc(OutIdx,Step);
//     end;
// end;
// 
// procedure TSqrFilter.Reset;
// begin
// // Nothing to reset. Filter is stateless
// end;
// 
// procedure TSqrFilter.Reset(UseNextSample: Boolean);
// begin
// // Nothing to reset. Filter is stateless
// end;
// 
// { TSqrtFilter }
// 
// constructor TSqrtFilter.Create;
// begin
//   inherited;
//   SetNrSettings(2);
//   FSettingInfo[0]:='Square root (x^-2)';
//   FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='FilterGain';
// end;
// 
// function TSqrtFilter.DoCheckSettings: Integer;
// begin
//   If(Setting[1] <= 0) then                { SampleFrequency should be > 0 Hz }
//     result:=1
//   else
//     result:=0;
// end;
// 
// procedure TSqrtFilter.FilterSamples(var SamplesIn; var SamplesOut;
//   IdxStart, IdxEnd: Integer; OutIdxStart:Integer = -1);
// var
//   Idx, IdxLast, Step, OutIdx: Integer;
//   s: Extended;
// begin
//   s:=0;
//   If(IdxEnd <> IdxStart) then
//     Step:=Sign(Int64(IdxEnd-IdxStart))       { Filter from IdxStart to IdxEnd }
//   else
//     Step:=1;                                             { Avoid endless loop }
//   If(OutIdxStart < 0) then
//     OutIdx:=IdxStart
//   else
//     OutIdx:=OutIdxStart;
//   If(FDirection = ForwardOnly) then
//     begin
//       Idx:=IdxStart;
//       IdxLast:=IdxEnd+1
//     end
//   else
//     begin
//       Idx:=IdxEnd;
//       IdxLast:=IdxStart-1;
//       Step:=-Step
//     end;
//   While(Idx <> Idxlast) do
//     begin
//       Case FDataInType of
//         fdSmallInt : s:=TDynSmallIntArray(SamplesIn)[Idx];
//         fdInteger  : s:=TDynIntegerArray(SamplesIn)[Idx];
//         fdSingle   : s:=TDynSingleArray(SamplesIn)[Idx];
//         fdDouble   : s:=TDynDoubleArray(SamplesIn)[Idx];
//       end;
// { Compute new output sample }
//       s:=Gain*Sqrt(s);
//       Case DataOutType of
//         fdSmallInt : begin
//                        If(Abs(Round(s)) > MaxSmallInt) then
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Sign(s)*MaxSmallInt
//                        else
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdInteger  : begin
//                        If(Abs(Round(s)) > MaxInt) then
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Sign(s)*MaxInt
//                        else
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdSingle   : TDynSingleArray(SamplesOut)[OutIdx]:=s;
//         fdDouble   : TDynDoubleArray(SamplesOut)[OutIdx]:=s;
//       end;
// { Next sample }
//       Inc(Idx,Step);
//       Inc(OutIdx,Step);
//     end;
// end;
// 
// procedure TSqrtFilter.Reset;
// begin
// // Nothing to reset. Filter is stateless
// end;
// 
// procedure TSqrtFilter.Reset(UseNextSample: Boolean);
// begin
// // Nothing to reset. Filter is stateless
// end;
// 
// { TLP05Filter }
// 
// constructor TLP05Filter.Create;
// begin
//   inherited;
//   SetNrSettings(3);
//   FSettingInfo[0]:='Half order low-pass';
//   FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='FilterGain';
//   FSettingInfo[3]:='Rate';
// end;
// 
// procedure TLP05Filter.CalculateIIRCoeff;
// var
//   C: TLP05RateFrequencyConvertor;
// begin
//   inherited;
//   C:=TLP05RateFrequencyConvertor.Create;
//   try
//     FRate:=C.Find3dBRate(Setting[1],Setting[3],1e-6);
//     Setting[3]:=C.Find3dBFrequency(Setting[1],FRate,1e-6);
//     FNewSettings:=False;
//   finally
//     C.Free;
//   end;
//   SetLength(FFilterStateP,1);
// end;
// 
// function TLP05Filter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     If((Setting[3] <= 0) or (Setting[3] > Setting[1]/2)) then
//       result:=3;
// end;
// 
// procedure TLP05Filter.FilterSamples(var SamplesIn, SamplesOut; IdxStart,
//   IdxEnd, OutIdxStart: Integer);
// var
//   Idx,IdxLast,
//   Step,OutIdx: Integer;
//   s: Extended;
//   RY: Double;
// begin
//   s:=0;
//   If(FNewSettings) then
//     CalculateIIRCoeff;
//   If(IdxEnd <> IdxStart) then
//     Step:=Sign(Int64(IdxEnd-IdxStart))       // Filter from IdxStart to IdxEnd
//   else
//     Step:=1;                                             // Avoid endless loop
//   If(OutIdxStart < 0) then
//     OutIdx:=IdxStart
//   else
//     OutIdx:=OutIdxStart;
//   If(FDirection = ForwardOnly) then
//     begin
//       Idx:=IdxStart;
//       IdxLast:=IdxEnd+1
//     end
//   else
//     begin
//       Idx:=IdxEnd;
//       IdxLast:=IdxStart-1;
//       Step:=-Step
//     end;
//   RY:=FFilterStateP[0];
//   While(Idx <> Idxlast) do
//     begin
//       Case FDataInType of
//         fdSmallInt : s:=TDynSmallIntArray(SamplesIn)[Idx];
//         fdInteger  : s:=TDynIntegerArray(SamplesIn)[Idx];
//         fdSingle   : s:=TDynSingleArray(SamplesIn)[Idx];
//         fdDouble   : s:=TDynDoubleArray(SamplesIn)[Idx];
//       end;
//       If(UseFirstSampleToReset) then
//         begin
//           Reset(s);
//           UseFirstSampleToReset:=False;
//           RY:=FFilterStateP[0];
//         end;
// // Compute new output sample
//       RY:=RY+FRate*(Gain*s-RY);
//       s:=RY;
// // Store new output sample
//       Case DataOutType of
//         fdSmallInt : begin
//                        If(Abs(Round(s)) > MaxSmallInt) then
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Sign(s)*MaxSmallInt
//                        else
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdInteger  : begin
//                        If(Abs(Round(s)) > MaxInt) then
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Sign(s)*MaxInt
//                        else
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdSingle   : TDynSingleArray(SamplesOut)[OutIdx]:=s;
//         fdDouble   : TDynDoubleArray(SamplesOut)[OutIdx]:=s;
//       end;
// // Next sample
//       Inc(Idx,Step);
//       Inc(OutIdx,Step);
//     end;
//   FFilterStateP[0]:=RY;
// end;
// 
// procedure TLP05Filter.Reset(Xn: Double);
// begin
//   If(FNewSettings) then
//     CalculateIIRCoeff;
//   FFilterStateP[0]:=Xn;
// end;
// 
// { TLP05NSFilter }
// 
// constructor TLP05NSFilter.Create;
// begin
//   inherited;
//   SetLength(FFilterStateP,1);
//   SetNrSettings(4);
//   FSettingInfo[0]:='LP0.5NS:Half order LP not symmetric';
//   FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='FilterGain';
//   FSettingInfo[3]:='Rate up';
//   FSettingInfo[4]:='Rate down';
// end;
// 
// procedure TLP05NSFilter.CalculateIIRCoeff;
// var
//   C: TLP05RateFrequencyConvertor;
// begin
//   inherited;
//   C:=TLP05RateFrequencyConvertor.Create;
//   try
//     FRate2:=C.Find3dBRate(Setting[1],Setting[4],1e-6);
//     Setting[4]:=C.Find3dBFrequency(Setting[1],FRate2,1e-6);
//     FNewSettings:=False;
//   finally
//     C.Free;
//   end;
// end;
// 
// function TLP05NSFilter.DoCheckSettings: Integer;
// begin
//   result:=inherited DoCheckSettings;
//   If(result = 0) then
//     If((Setting[4] <= 0) or (Setting[4] >= Setting[1]/2)) then
//       result:=4;
// end;
// 
// procedure TLP05NSFilter.FilterSamples(var SamplesIn, SamplesOut; IdxStart,
//   IdxEnd, OutIdxStart: Integer);
// var
//   Idx,IdxLast,
//   Step,OutIdx: Integer;
//   s: Extended;
//   r: Double;
//   RY: Double;
//   dX: Double;
// begin
//   s:=0;
//   If(FNewSettings) then
//     CalculateIIRCoeff;
//   If(IdxEnd <> IdxStart) then
//     Step:=Sign(Int64(IdxEnd-IdxStart))       // Filter from IdxStart to IdxEnd
//   else
//     Step:=1;                                             // Avoid endless loop
//   If(OutIdxStart < 0) then
//     OutIdx:=IdxStart
//   else
//     OutIdx:=OutIdxStart;
//   If(FDirection = ForwardOnly) then
//     begin
//       Idx:=IdxStart;
//       IdxLast:=IdxEnd+1
//     end
//   else
//     begin
//       Idx:=IdxEnd;
//       IdxLast:=IdxStart-1;
//       Step:=-Step
//     end;
//   RY:=FFilterStateP[0];
//   While(Idx <> Idxlast) do
//     begin
//       Case FDataInType of
//         fdSmallInt : s:=TDynSmallIntArray(SamplesIn)[Idx];
//         fdInteger  : s:=TDynIntegerArray(SamplesIn)[Idx];
//         fdSingle   : s:=TDynSingleArray(SamplesIn)[Idx];
//         fdDouble   : s:=TDynDoubleArray(SamplesIn)[Idx];
//       end;
//       If(UseFirstSampleToReset) then
//         begin
//           Reset(s);
//           UseFirstSampleToReset:=False;
//           RY:=FFilterStateP[0];
//         end;
// // Compute new output sample
//       dX:=(Gain*s-RY);
//       If(dX >= 0) then
//         r:=FRate                         { Rising }
//       else
//         r:=FRate2;                        { Falling }
//       RY:=RY+r*dX;
//       s:=RY;
// // Store new output sample
//       Case DataOutType of
//         fdSmallInt : begin
//                        If(Abs(Round(s)) > MaxSmallInt) then
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Sign(s)*MaxSmallInt
//                        else
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdInteger  : begin
//                        If(Abs(Round(s)) > MaxInt) then
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Sign(s)*MaxInt
//                        else
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdSingle   : TDynSingleArray(SamplesOut)[OutIdx]:=s;
//         fdDouble   : TDynDoubleArray(SamplesOut)[OutIdx]:=s;
//       end;
// // Next sample
//       Inc(Idx,Step);
//       Inc(OutIdx,Step);
//     end;
//   FFilterStateP[0]:=RY;
// end;
// 
// procedure TLP05NSFilter.Reset(Xn: Double);
// var
//   i: Integer;
//   r: Double;
//   Y: Double;
// begin
//   If(FNewSettings) then
//     CalculateIIRCoeff;
//   r:=1;
//   For i:=1 to High(FPoles) do
//     r:=r-FPoles[i];
//   Y:=Xn/r;
//   For i:=Low(FFilterStateZ) to High(FFilterStateZ) do
//     FFilterStateZ[i]:=Xn;
//   For i:=Low(FFilterStateP) to High(FFilterStateP) do
//     FFilterStateP[i]:=Y;
// end;
// 
// { Other procedures / functions }
// 
// function GetFilterByName(FilterName:String): TCustomFilter;
// var
//   i: TFilterType;
// begin
//   result:=Nil;
//   FilterName:=Trim(UpperCase(FilterName));
//   i:=Low(TFilterType);
//   While((i < High(TFilterType)) and (FilterName <> FilterNames[i])) do
//     Inc(i);
//   If(FilterName = FilterNames[i]) then
//     Case i of
//       LP:
//         result:=TLPFilter.Create;
//       HP:
//         result:=THPFilter.Create;
//       DUE:
//         result:=TDUEFilter.Create;
//       SE:
//         result:=TSEFilter.Create;
//       NOTCH:
//         result:=TNotchFilter.Create;
//       G:
//         result:=TGFilter.Create;
//       INVG:
//         result:=TInvGFilter.Create;
//       BP:
//         result:=TBPFilter.Create;
//       RECT:
//         result:=TRectifier.Create;
//       BP12:
//         result:=TBP12Filter.Create;
//       DCatt:
//         result:=TDCattFilter.Create;
//       InvDCatt:
//         result:=TINVDCattFilter.Create;
//       DUE1987:
//         result:=TDUE1987Filter.Create;
//       DUE2007:
//         result:=TDUE2007Filter.Create;
//       LP05:
//         result:=TLP05Filter.Create;
//       LP05NS:
//         result:=TLP05NSFilter.Create;
//       Median:
//         result:=TMedianFilter.Create;
//       Square:
//         result:=TSqrFilter.Create;
//       Squareroot:
//                 result:=TSqrtFilter.Create;
//       Mean:
//         result:=TMeanFilterQuick.Create;
//       Percentillian:
//         result:=TPercentillianFilter.Create;
//       dB:
//         result:=TdBFilter.Create;
// //      MeanQuick:
// //        result:=TMeanFilterQuick.Create;
//     end;
// end;
// 
// procedure GetAllFilterNames(const Strings: TStrings);
// var
//   i: TFilterType;
// begin
//   For i:=Low(TFilterType) to High(TFilterType) do
//     Strings.Add(FilterNames[i]);
// end;
// 
// function GetFilterType(const AFilter: TCustomFilter): TFilterType;
// var
//   S: String;
//   i: Integer;
// begin
//   i:=-1;
//   S:=UpperCase(AFilter.ClassName);
//   If(S = 'TLPFILTER') then
//     i:=Ord(LP);
//   If(S = 'THPFILTER') then
//     i:=Ord(HP);
//   If(S = 'TDUEFILTER') then
//     i:=Ord(DUE);
//   If(S = 'TSEFILTER') then
//     i:=Ord(SE);
//   If(S = 'TNOTCHFILTER') then
//     i:=Ord(NOTCH);
//   If(S = 'TGFILTER') then
//     i:=Ord(G);
//   If(S = 'TINVGFILTER') then
//     i:=Ord(INVG);
//   If(S = 'TBPFILTER') then
//     i:=Ord(BP);
//   If(S = 'TRECTIFIER') then
//     i:=Ord(RECT);
//   If(S = 'TBP12FILTER') then
//     i:=Ord(BP12);
//   If(S = 'TDCATTFILTER') then
//     i:=Ord(DCatt);
//   If(S = 'TINVDCATTFILTER') then
//     i:=Ord(InvDCatt);
//   If(S = 'TDUE1987FILTER') then
//     i:=Ord(DUE1987);
//   If(S = 'TDUE2007FILTER') then
//     i:=Ord(DUE2007);
//   If(S = 'TLP05FILTER') then
//     i:=Ord(LP05);
//   If(S = 'TLP05NSFILTER') then
//     i:=Ord(LP05NS);
//   If(S = 'TMEDIANFILTER') then
//     i:=Ord(Median);
//   If(S = 'TSQRFILTER') then
//     i:=Ord(Square);
//   If(S = 'TSQRTFILTER') then
//     i:=Ord(Squareroot);
//   If(S = 'TMEANFILTERQUICK') then
//     i:=Ord(Mean);
//   If(S = 'TPERCENTILLIANFILTER') then
//     i:=Ord(Percentillian);
//   If(S = 'TDBFILTER') then
//     i:=Ord(dB);
//   Assert((i <> -1),'Variable is a unknown filter object');
//   result:=TFilterType(i);
// end;
// 
// function FilterToStr(const AFilter: TCustomFilter): String;
// begin
//   result:=AFilter.AsString;
// end;
// 
// function StrToFilter(const AString: String): TCustomFilter;
// var
//   S: String;
//   i: Integer;
// begin
//   result:=Nil;
//   S:=AString;
//   If(((Pos('(',S) > 0) and (Pos(')',S) = Length(S)))) then
//     begin
//       result:=GetFilterByName(StripStringValue(S,'('));
//       If(Assigned(result)) then
//         begin
//           Delete(S,Length(S),1);
//           i:=2;
//           While(S <> '') do
//             begin
//               result.Setting[i]:=StrToFloat(StripStringValue(S,'/'),
//                   FS_LCID_US_ENG);
//               Inc(i);
//             end;
//         end;
//     end
//   else
//     If(((Pos('(',S) = 0) and (Pos(')',S) = 0))) then
//       result:=GetFilterByName(Trim(S));
// end;
// 
// function GetFilterDescriptionStr(const FilterName:String): String; overload;
// var
//   AFilter: TCustomFilter;
// begin
//   AFilter:=GetFilterByName(FilterName);
//   try
//     If(Assigned(AFilter)) then
//       result:=AFilter.FilterDescription
//     else
//       result:='';
//   finally
//     AFilter.Free;
//   end
// end;
// 
// function ValidSampleFrequency(const ASampleFreq: Extended; const AFilterStr:
//   String; ADelimiters: String): Boolean;
// var
//   S: String;
//   FilterAsStr: String;
//   Filter: TCustomFilter;
// begin
//   result:=True;
//   S:=AFilterStr;
//   While(result and (S <> '')) do
//     begin
//       FilterAsStr:=StripStringValue(S,ADelimiters);
//       Filter:=StrToFilter(FilterAsStr);
//       try
//         If(not Assigned(Filter)) then
//           raise EFilter.Create('Invalid filter string');
//         Filter.SampleFrequency:=ASampleFreq;
//         result:=(Filter.CheckSettings = 0);
//       finally
//         Filter.Free;
//       end;
//     end;
// end;
// 
// procedure TMedianFilter.CheckLengthSavedState(Index: Integer);
// begin
//   If(Index > High(FSavedState)) then
//     SetLength(FSavedState,Index+1);
// end;
// 
// constructor TMedianFilter.Create;
// begin
//   inherited;
//   SetNrSettings(2);
//   FSettingInfo[0]:='Median Filter';
//   FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='Seconds to look back';
//   FMedianList:=TDoubleMedianList.Create;
// end;
// 
// destructor TMedianFilter.Destroy;
// var
//   i: Integer;
// begin
//   For i:=Low(FSavedState) to High(FSavedState) do
//     If(Assigned(FSavedState[i])) then
//       FreeAndNil(FSavedState[i]);
//   FreeAndNil(FMedianList);
//   inherited;
// end;
// 
// function TMedianFilter.DoCheckSettings: Integer;
// begin
//   If(Setting[1] <= 0) then       // SampleFrequency should be > 0 Hz
//     result:=1
//   else
// // Nr median elements 1-65535
//     If(not InRange(Setting[2]*SampleFrequency,1,65535)) then
//       result:=2
//     else
//       result:=0;
// end;
// 
// procedure TMedianFilter.DoClearSavedState(Index: Integer);
// begin
//   inherited;
//   FreeAndNil(FSavedState[Index]);
// end;
// 
// procedure TMedianFilter.DoRestoreState(Index: Integer);
// begin
//   inherited;
//   If(Assigned(FSavedState[Index])) then
//     FMedianList.Assign(FSavedState[Index]);
// end;
// 
// procedure TMedianFilter.DoSaveState(Index: Integer);
// begin
//   inherited;
//   FreeAndNil(FSavedState[Index]);
//   FSavedState[Index]:=TDoubleMedianList.Create;
//   FSavedState[Index].Assign(FMedianList);
// end;
// 
// procedure TMedianFilter.FilterSamples(var SamplesIn, SamplesOut; IdxStart,
//   IdxEnd, OutIdxStart: Integer);
// var
//   Idx, IdxLast, Step, OutIdx: Integer;
//   s: Extended;
// begin
//   If(FNewSettings) then
//     InitializeMedianList;
//   s:=0;
//   If(IdxEnd <> IdxStart) then
//     Step:=Sign(Int64(IdxEnd-IdxStart))       // Filter from IdxStart to IdxEnd
//   else
//     Step:=1;                                 // Avoid endless loop
//   If(OutIdxStart < 0) then
//     OutIdx:=IdxStart
//   else
//     OutIdx:=OutIdxStart;
//   If(FDirection = ForwardOnly) then
//     begin
//       Idx:=IdxStart;
//       IdxLast:=IdxEnd+1
//     end
//   else
//     begin
//       Idx:=IdxEnd;
//       IdxLast:=IdxStart-1;
//       Step:=-Step
//     end;
//   While(Idx <> Idxlast) do
//     begin
//       Case FDataInType of
//         fdSmallInt : s:=TDynSmallIntArray(SamplesIn)[Idx];
//         fdInteger  : s:=TDynIntegerArray(SamplesIn)[Idx];
//         fdSingle   : s:=TDynSingleArray(SamplesIn)[Idx];
//         fdDouble   : s:=TDynDoubleArray(SamplesIn)[Idx];
//       end;
// // Compute new output sample
//       FMedianList.Add(s);
//       s:=FMedianList.Median;
//       Case DataOutType of
//         fdSmallInt : begin
//                        If(Abs(Round(s)) > MaxSmallInt) then
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Sign(s)*MaxSmallInt
//                        else
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdInteger  : begin
//                        If(Abs(Round(s)) > MaxInt) then
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Sign(s)*MaxInt
//                        else
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdSingle   : TDynSingleArray(SamplesOut)[OutIdx]:=s;
//         fdDouble   : TDynDoubleArray(SamplesOut)[OutIdx]:=s;
//       end;
// // Next sample
//       Inc(Idx,Step);
//       Inc(OutIdx,Step);
//     end;
// end;
// 
// { TMedianFilter }
// 
// function TMedianFilter.GetSettingDimensionInfo(Index: Integer): String;
// begin
//   Case Index of
//     2 : result:='s';
//   else
//     result:=inherited GetSettingDimensionInfo(Index);
//   end;
// end;
// 
// procedure TMedianFilter.InitializeMedianList;
// begin
//   inherited;
//   FMedianList.MaxNrItems:=Max(1,RoundNearest(SampleFrequency*Setting[2]));
// end;
// 
// procedure TMedianFilter.Reset;
// begin
//   inherited;
//   FMedianList.Clear;
// end;
// 
// procedure TMedianFilter.Reset(UseNextSample: Boolean);
// begin
//   inherited;
//   FMedianList.Clear;
// end;
// 
// { TMeanFilterQuick }
// 
// constructor TMeanFilterQuick.Create;
// begin
//   inherited;
//   SetNrSettings(2);
//   FSettingInfo[0]:='Mean';
//   FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='Seconds to look back';
// end;
// 
// destructor TMeanFilterQuick.Destroy;
// var
//   i: Integer;
// begin
//   For i:=Low(FSavedState) to High(FSavedState) do
//     If(Assigned(FSavedState[i].SavedState)) then
//       FreeAndNil(FSavedState[i].SavedState);
//   inherited;
// end;
// 
// function TMeanFilterQuick.DoCheckSettings: Integer;
// var
//   MemoryStatus: TMemoryStatus;
//   MaxArraySize: Cardinal;
//   MemAvailable: Cardinal;
// begin
//   MemoryStatus.dwLength:=SizeOf(MemoryStatus);
//   GlobalMemoryStatus(MemoryStatus);
//   MemAvailable:=MemoryStatus.dwAvailVirtual div SizeOf(Double);
// 
//   MaxArraySize:=MemAvailable div 10;
//   MaxArraySize:=Max(MaxArraySize,Ceil(15*60*SampleFrequency)); // at least 15 minutes
//   MaxArraySize:=Min(MaxArraySize,MemAvailable div 2);
//   
//   If(Setting[1] <= 0) then       // SampleFrequency should be > 0 Hz
//     result:=1
//   else
// // Nr median elements 1-MaxArraySize
//     If(not InRange(Setting[2]*SampleFrequency,1,MaxArraySize)) then
//       result:=2
//     else
//       result:=0;
// end;
// 
// procedure TMeanFilterQuick.DoClearSavedState(Index: Integer);
// begin
//   inherited;
//   FSavedState[Index].SavedState:=nil;
// end;
// 
// procedure TMeanFilterQuick.DoRestoreState(Index: Integer);
// begin
//   inherited;
//   If(Assigned(FSavedState[Index].SavedState)) then
//     begin
//       CopyDynamicArray(FSavedState[Index].SavedState,FSamples);
//       FHeadIdx:=FSavedState[Index].SavedHeadIdx;
//       FTailIdx:=FSavedState[Index].SavedTailIdx;
//     end;
// end;
// 
// procedure TMeanFilterQuick.DoSaveState(Index: Integer);
// begin
//   inherited;
//   CopyDynamicArray(FSamples,FSavedState[Index].SavedState);
//   FSavedState[Index].SavedHeadIdx:=FHeadIdx;
//   FSavedState[Index].SavedTailIdx:=FTailIdx;
// end;
// 
// procedure TMeanFilterQuick.IncrementIdx(var Idx: Integer);
// begin
//   Inc(Idx);
//   If(Idx >= Length(FSamples)) then
//     Idx:=0;
// end;
// 
// function TMeanFilterQuick.GetMean: Double;
// var
//   i: Integer;
//   Sum: Extended;
//   Cnt: Integer;
// begin
//   Sum:=0;
//   Cnt:=0;
//   i:=FTailIdx;
//   repeat
//     Sum:=Sum+FSamples[i];
//     Inc(Cnt);
//     IncrementIdx(i);
//   until(i = FHeadIdx);
//   If(Cnt > 0) then
//     result:=Sum/Cnt
//   else
//     result:=0;
// end;
// 
// procedure TMeanFilterQuick.FilterSamples(var SamplesIn, SamplesOut;
//   IdxStart, IdxEnd, OutIdxStart: Integer);
// var
//   Idx, IdxLast, Step, OutIdx: Integer;
//   s: Extended;
// begin
//   If(FNewSettings) then
//     InitializeArray;
//   s:=0;
//   If(IdxEnd <> IdxStart) then
//     Step:=Sign(Int64(IdxEnd-IdxStart))       // Filter from IdxStart to IdxEnd
//   else
//     Step:=1;                                 // Avoid endless loop
//   If(OutIdxStart < 0) then
//     OutIdx:=IdxStart
//   else
//     OutIdx:=OutIdxStart;
//   If(FDirection = ForwardOnly) then
//     begin
//       Idx:=IdxStart;
//       IdxLast:=IdxEnd+1
//     end
//   else
//     begin
//       Idx:=IdxEnd;
//       IdxLast:=IdxStart-1;
//       Step:=-Step
//     end;
//   While(Idx <> Idxlast) do
//     begin
//       Case FDataInType of
//         fdSmallInt : s:=TDynSmallIntArray(SamplesIn)[Idx];
//         fdInteger  : s:=TDynIntegerArray(SamplesIn)[Idx];
//         fdSingle   : s:=TDynSingleArray(SamplesIn)[Idx];
//         fdDouble   : s:=TDynDoubleArray(SamplesIn)[Idx];
//       end;
// // Compute new output sample
//       FSamples[FHeadIdx]:=s;
//       IncrementIdx(FHeadIdx);
//       If(FHeadIdx = FTailIdx) then
//         IncrementIdx(FTailIdx);
//       s:=GetMean;
//       Case DataOutType of
//         fdSmallInt : begin
//                        If(Abs(Round(s)) > MaxSmallInt) then
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Sign(s)*MaxSmallInt
//                        else
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdInteger  : begin
//                        If(Abs(Round(s)) > MaxInt) then
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Sign(s)*MaxInt
//                        else
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdSingle   : TDynSingleArray(SamplesOut)[OutIdx]:=s;
//         fdDouble   : TDynDoubleArray(SamplesOut)[OutIdx]:=s;
//       end;
// // Next sample
//       Inc(Idx,Step);
//       Inc(OutIdx,Step);
//     end;
// end;
// 
// function TMeanFilterQuick.GetSettingDimensionInfo(Index: Integer): String;
// begin
//   Case Index of
//     2 : result:='s';
//   else
//     result:=inherited GetSettingDimensionInfo(Index);
//   end;
// end;
// 
// procedure TMeanFilterQuick.InitializeArray;
// begin
//   inherited;
//   SetLength(FSamples,Max(1,RoundNearest(SampleFrequency*Setting[2])));
// end;
// 
// procedure TMeanFilterQuick.Reset(UseNextSample: Boolean);
// begin
//   inherited;
//   FTailIdx:=FHeadIdx;
// end;
// 
// procedure TMeanFilterQuick.Reset;
// begin
//   inherited;
//   FTailIdx:=FHeadIdx;
// end;
// 
// (*
// { TMeanFilter }
// 
// constructor TMeanFilter.Create;
// begin
//   inherited;
//   SetNrSettings(2);
//   FSettingInfo[0]:='Mean';
//   FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='Seconds to look back';
//   FMedianList:=TDoubleMedianList.Create;
// end;
// 
// destructor TMeanFilter.Destroy;
// begin
//   FreeAndNil(FSavedState);
//   FreeAndNil(FMedianList);
//   inherited;
// end;
// 
// function TMeanFilter.DoCheckSettings: Integer;
// begin
//   If(Setting[1] <= 0) then       // SampleFrequency should be > 0 Hz
//     result:=1
//   else
// // Nr median elements 1-65535
//     If(not InRange(Setting[2]*SampleFrequency,1,65535)) then
//       result:=2
//     else
//       result:=0;
// end;
// 
// procedure TMeanFilter.DoClearSavedState;
// begin
//   FreeAndNil(FSavedState);
// end;
// 
// procedure TMeanFilter.DoRestoreState;
// begin
//   If(Assigned(FSavedState)) then
//     FMedianList.Assign(FSavedState);
// end;
// 
// procedure TMeanFilter.DoSaveState;
// begin
//   FreeAndNil(FSavedState);
//   FSavedState:=TDoubleMedianList.Create;
//   FSavedState.Assign(FMedianList);
// end;
// 
// procedure TMeanFilter.FilterSamples(var SamplesIn, SamplesOut; IdxStart,
//   IdxEnd, OutIdxStart: Integer);
// var
//   Idx, IdxLast, Step, OutIdx: Integer;
//   s: Extended;
// begin
//   If(FNewSettings) then
//     InitializeMedianList;
//   s:=0;
//   If(IdxEnd <> IdxStart) then
//     Step:=Sign(Int64(IdxEnd-IdxStart))       // Filter from IdxStart to IdxEnd
//   else
//     Step:=1;                                 // Avoid endless loop
//   If(OutIdxStart < 0) then
//     OutIdx:=IdxStart
//   else
//     OutIdx:=OutIdxStart;
//   If(FDirection = ForwardOnly) then
//     begin
//       Idx:=IdxStart;
//       IdxLast:=IdxEnd+1
//     end
//   else
//     begin
//       Idx:=IdxEnd;
//       IdxLast:=IdxStart-1;
//       Step:=-Step
//     end;
//   While(Idx <> Idxlast) do
//     begin
//       Case FDataInType of
//         fdSmallInt : s:=TDynSmallIntArray(SamplesIn)[Idx];
//         fdInteger  : s:=TDynIntegerArray(SamplesIn)[Idx];
//         fdSingle   : s:=TDynSingleArray(SamplesIn)[Idx];
//         fdDouble   : s:=TDynDoubleArray(SamplesIn)[Idx];
//       end;
// // Compute new output sample
//       FMedianList.Add(s);
//       s:=FMedianList.Mean;
//       Case DataOutType of
//         fdSmallInt : begin
//                        If(Abs(Round(s)) > MaxSmallInt) then
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Sign(s)*MaxSmallInt
//                        else
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdInteger  : begin
//                        If(Abs(Round(s)) > MaxInt) then
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Sign(s)*MaxInt
//                        else
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdSingle   : TDynSingleArray(SamplesOut)[OutIdx]:=s;
//         fdDouble   : TDynDoubleArray(SamplesOut)[OutIdx]:=s;
//       end;
// // Next sample
//       Inc(Idx,Step);
//       Inc(OutIdx,Step);
//     end;
// end;
// 
// function TMeanFilter.GetSettingDimensionInfo(Index: Integer): String;
// begin
//   Case Index of
//     2 : result:='s';
//   else
//     result:=inherited GetSettingDimensionInfo(Index);
//   end;
// end;
// 
// procedure TMeanFilter.InitializeMedianList;
// begin
//   inherited;
//   FMedianList.MaxNrItems:=Max(1,RoundNearest(SampleFrequency*Setting[2]));
// end;
// 
// procedure TMeanFilter.Reset;
// begin
//   inherited;
//   FMedianList.Clear;
// end;
// 
// procedure TMeanFilter.Reset(UseNextSample: Boolean);
// begin
//   inherited;
//   FMedianList.Clear;
// end;
// *)
// 
// procedure TMeanFilterQuick.CheckLengthSavedState(Index: Integer);
// begin
//   If(Index > High(FSavedState)) then
//     SetLength(FSavedState,Index+1);
// end;
// 
// { TPercentillianFilter }
// 
// procedure TPercentillianFilter.CheckLengthSavedState(Index: Integer);
// begin
//   If(Index > High(FSavedState)) then
//     SetLength(FSavedState,Index+1);
// end;
// 
// constructor TPercentillianFilter.Create;
// begin
//   inherited;
//   SetNrSettings(3);
//   FSettingInfo[0]:='Percentillian';
//   FSettingInfo[1]:='SampleFrequency';
//   FSettingInfo[2]:='Like median 50%, but now';
//   FSettingInfo[3]:='Seconds to look back';
//   FMedianList:=TDoubleMedianList.Create;
// end;
// 
// destructor TPercentillianFilter.Destroy;
// var
//   i: Integer;
// begin
//   For i:=Low(FSavedState) to High(FSavedState) do
//     If(Assigned(FSavedState[i])) then
//       FreeAndNil(FSavedState[i]);
//   FreeAndNil(FMedianList);
//   inherited;
// end;
// 
// function TPercentillianFilter.DoCheckSettings: Integer;
// begin
//   If(Setting[1] <= 0) then       // SampleFrequency should be > 0 Hz
//     result:=1
//   else
// // Percentillian percentage 0-100%
//     If(not InRange(Setting[2],0,100)) then
//       result:=2
//     else
// // Nr median elements 1-65535
//       If(not InRange(Setting[3]*SampleFrequency,1,65535)) then
//         result:=3
//       else
//         result:=0;
// end;
// 
// procedure TPercentillianFilter.DoClearSavedState(Index: Integer);
// begin
//   inherited;
//   FreeAndNil(FSavedState[Index]);
// end;
// 
// procedure TPercentillianFilter.DoRestoreState(Index: Integer);
// begin
//   inherited;
//   If(Assigned(FSavedState)) then
//     FMedianList.Assign(FSavedState[Index]);
// end;
// 
// procedure TPercentillianFilter.DoSaveState(Index: Integer);
// begin
//   inherited;
//   FreeAndNil(FSavedState);
//   FSavedState[Index]:=TDoubleMedianList.Create;
//   FSavedState[Index].Assign(FMedianList);
// end;
// 
// procedure TPercentillianFilter.FilterSamples(var SamplesIn, SamplesOut;
//   IdxStart, IdxEnd, OutIdxStart: Integer);
// var
//   Idx, IdxLast, Step, OutIdx: Integer;
//   s: Extended;
// begin
//   If(FNewSettings) then
//     InitializeMedianList;
//   s:=0;
//   If(IdxEnd <> IdxStart) then
//     Step:=Sign(Int64(IdxEnd-IdxStart))       // Filter from IdxStart to IdxEnd
//   else
//     Step:=1;                                 // Avoid endless loop
//   If(OutIdxStart < 0) then
//     OutIdx:=IdxStart
//   else
//     OutIdx:=OutIdxStart;
//   If(FDirection = ForwardOnly) then
//     begin
//       Idx:=IdxStart;
//       IdxLast:=IdxEnd+1
//     end
//   else
//     begin
//       Idx:=IdxEnd;
//       IdxLast:=IdxStart-1;
//       Step:=-Step
//     end;
//   While(Idx <> Idxlast) do
//     begin
//       Case FDataInType of
//         fdSmallInt : s:=TDynSmallIntArray(SamplesIn)[Idx];
//         fdInteger  : s:=TDynIntegerArray(SamplesIn)[Idx];
//         fdSingle   : s:=TDynSingleArray(SamplesIn)[Idx];
//         fdDouble   : s:=TDynDoubleArray(SamplesIn)[Idx];
//       end;
// // Compute new output sample
//       FMedianList.Add(s);
//       s:=FMedianList.Percentillian(Setting[2]);
//       Case DataOutType of
//         fdSmallInt : begin
//                        If(Abs(Round(s)) > MaxSmallInt) then
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Sign(s)*MaxSmallInt
//                        else
//                          TDynSmallIntArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdInteger  : begin
//                        If(Abs(Round(s)) > MaxInt) then
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Sign(s)*MaxInt
//                        else
//                          TDynIntegerArray(SamplesOut)[OutIdx]:=Round(s)
//                      end;
//         fdSingle   : TDynSingleArray(SamplesOut)[OutIdx]:=s;
//         fdDouble   : TDynDoubleArray(SamplesOut)[OutIdx]:=s;
//       end;
// // Next sample
//       Inc(Idx,Step);
//       Inc(OutIdx,Step);
//     end;
// end;
// 
// function TPercentillianFilter.GetSettingDimensionInfo(
//   Index: Integer): String;
// begin
//   Case Index of
//     2 : result:='%';
//     3 : result:='s';
//   else
//     result:=inherited GetSettingDimensionInfo(Index);
//   end;
// end;
// 
// procedure TPercentillianFilter.InitializeMedianList;
// begin
//   inherited;
//   FMedianList.MaxNrItems:=Max(1,RoundNearest(SampleFrequency*Setting[3]));
// end;
// 
// procedure TPercentillianFilter.Reset;
// begin
//   inherited;
//   FMedianList.Clear;
// end;
// 
// procedure TPercentillianFilter.Reset(UseNextSample: Boolean);
// begin
//   inherited;
//   FMedianList.Clear;
// end;
// 
// initialization
// // Unit Initialization/Finalization
// {$IFDEF DEBUG_CS_UNIT_INIT_FIN}
//   CodeSite.SendMsg(Format(SUnitInitialization, [UnitFileName]));
// {$ENDIF}
//   AddVCSGeneratedUnitInfo(
//         UnitFileName,
//         '$Revision:1.17$',
//         '$AUTHOR:Marco$',
//         '$RevDate:16-12-2010 11:34:14$');
// 
// finalization
// {$IFDEF DEBUG_CS_UNIT_INIT_FIN}
//   CodeSite.SendMsg(Format(SUnitFinalization, [UnitFileName]));
// {$ENDIF}
// 
// end.
// 

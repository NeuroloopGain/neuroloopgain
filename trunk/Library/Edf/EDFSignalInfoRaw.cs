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
using NeuroLoopGainLibrary.Text;

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfSignalInfoRaw : ICloneable
  {
		#region private fields 

    private string _digiMax;
    private string _digiMin;
    private string _nrSamples;
    private string _physiDim;
    private string _physiMax;
    private string _physiMin;
    private string _preFilter;
    private string _reserved;
    private string _signalLabel;
    private string _transducerType;

		#endregion private fields 

		#region public properties 

    public virtual string DigiMax
    {
      get
      {
        return _digiMax;
      }
      set
      {
        _digiMax = TextUtils.SubStringLeft(value, 8);
      }
    }

    public virtual string DigiMin
    {
      get
      {
        return _digiMin;
      }
      set
      {
        _digiMin = TextUtils.SubStringLeft(value, 8);
      }
    }

    public virtual string NrSamples
    {
      get
      {
        return _nrSamples;
      }
      set
      {
        _nrSamples = TextUtils.SubStringLeft(value, 8);
      }
    }

    public string PhysiDim
    {
      get
      {
        return _physiDim;
      }
      set
      {
        _physiDim = TextUtils.SubStringLeft(value, 8);
      }
    }

    public string PhysiMax
    {
      get
      {
        return _physiMax;
      }
      set
      {
        _physiMax = TextUtils.SubStringLeft(value, 8);
      }
    }

    public string PhysiMin
    {
      get
      {
        return _physiMin;
      }
      set
      {
        _physiMin = TextUtils.SubStringLeft(value, 8);
      }
    }

    public string PreFilter
    {
      get
      {
        return _preFilter;
      }
      set
      {
        _preFilter = TextUtils.SubStringLeft(value, 80);
      }
    }

    public string Reserved
    {
      get
      {
        return _reserved;
      }
      set
      {
        _reserved = TextUtils.SubStringLeft(value, 32);
      }
    }

    public string SignalLabel
    {
      get
      {
        return _signalLabel;
      }
      set
      {
        _signalLabel = TextUtils.SubStringLeft(value, 16);
      }
    }

    public string TransducerType
    {
      get
      {
        return _transducerType;
      }
      set
      {
        _transducerType = TextUtils.SubStringLeft(value, 80);
      }
    }

		#endregion public properties 

		#region public methods 

    public void Clear()
    {
      SignalLabel = string.Empty;
      TransducerType = string.Empty;
      PhysiDim = string.Empty;
      PhysiMax = string.Empty;
      PhysiMin = string.Empty;
      DigiMax = string.Empty;
      DigiMin = string.Empty;
      PreFilter = string.Empty;
      NrSamples = string.Empty;
      Reserved = string.Empty;
    }

		#endregion public methods 


    #region ICloneable Members

    public object Clone()
    {
      EdfSignalInfoRaw newSignalInfoRaw = new EdfSignalInfoRaw
                                            {
                                              SignalLabel = SignalLabel,
                                              TransducerType = TransducerType,
                                              PhysiDim = PhysiDim,
                                              PhysiMax = PhysiMax,
                                              PhysiMin = PhysiMin,
                                              DigiMax = DigiMax,
                                              DigiMin = DigiMin,
                                              PreFilter = PreFilter,
                                              NrSamples = NrSamples,
                                              Reserved = Reserved
                                            };
      return newSignalInfoRaw;
    }

    #endregion
  }
}
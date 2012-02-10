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

namespace NeuroLoopGainLibrary.Edf
{
  public abstract class EdfFileHeaderInfoBase
  {
		#region private fields 

    private char _decimalSeparator;
    private bool[] _fieldValid;
    private bool _strictChecking
      ;
    private char _thousandSeparator;

		#endregion private fields 

		#region protected fields 

    protected int NumberDataFields;

		#endregion protected fields 

		#region protected properties 

    protected NumberFormatInfo FormatInfo
    {
      get
      {
        NumberFormatInfo ni = new NumberFormatInfo
                                {
                                  NumberDecimalSeparator = _decimalSeparator.ToString(),
                                  NumberGroupSeparator = _thousandSeparator.ToString()
                                };
        return ni;
      }
    }

    protected object Owner { get; private set; }

    protected bool ReadOnly { get; set; }

		#endregion protected properties 

		#region protected methods 

    protected abstract void DoReCheck();

    protected EdfFileHeaderInfoBase(object owner)
    {
      Owner = owner;
      _decimalSeparator = EdfConstants.DefaultDecimalSeparator;
      _thousandSeparator = EdfConstants.DefaultThousandSeparator;
      _strictChecking = true;
    }

		#endregion protected methods 

		#region public properties 

    public bool DataExists { get; internal set; }

    public virtual bool DataValid
    {
      get
      {
        foreach (bool b in FieldValid)
          if (!b)
            return false;
        return true;
      }
    }

    public char DecimalSeparator
    {
      get { return _decimalSeparator; }
      set { _decimalSeparator = value; }
    }

    public bool[] FieldValid
    {
      get { return _fieldValid ?? (_fieldValid = new bool[NrDataFields]); }
    }

    public int NrDataFields { get { return NumberDataFields; } }

    public bool StrictChecking
    {
      get
      {
        return _strictChecking;
      }
      set
      {
        _strictChecking = value;
        DoReCheck();
      }
    }

    public char ThousandSeparator
    {
      get { return _thousandSeparator; }
      set { _thousandSeparator = value; }
    }

		#endregion public properties 

		#region public methods 

    public virtual bool IsValidField(int index)
    {
      if ((index < 0) || (index >= FieldValid.Length))
        throw new ArgumentOutOfRangeException();
      return FieldValid[index];
    }

    public void ReCheck()
    {
      DoReCheck();
    }

		#endregion public methods 

    public bool Modified { get; protected internal set; }
  }
}

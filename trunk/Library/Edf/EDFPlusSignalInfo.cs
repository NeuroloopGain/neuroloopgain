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

using NeuroLoopGainLibrary.Mathematics;

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfPlusSignalInfo : EdfSignalInfoBase
  {
    #region Constructors

    public EdfPlusSignalInfo(object owner) : base(owner) { }

    #endregion Constructors

    #region public properties

    public bool IsAnnotationSignal
    {
      get
      {
        //The 'EDF Annotations' signal only has meaningful header fields 'label' and
        //'nr of samples in each data record'. For the sake of EDF compatibility, the
        //fields 'digital minimum' and 'digital maximum' must be filled with -32768
        //and 32767, respectively. The 'Physical maximum' and 'Physical minimum' fields
        //must contain values that differ from each other. The other fields of this
        //signal are filled with spaces.
        return (DataValid && (SignalLabel == EdfConstants.AnnotationsSignalLabel) &&
          (DigiMin == -32768) && (DigiMax == 32767) && (!MathEx.SameValue(PhysiMin, PhysiMax)) && string.IsNullOrEmpty(PhysiDim) &&
          string.IsNullOrEmpty(TransducerType) && string.IsNullOrEmpty(PreFilter) && string.IsNullOrEmpty(Reserved));
      }
    }

    #endregion public properties

    #region public methods

    public override bool IsValidField(int index)
    {
      if (((index == (int)Field.DigiMin) || (index == (int)Field.DigiMax)) &&
            FieldValid[(int)Field.DigiMin] && FieldValid[(int)Field.DigiMax])
        return (DigiMin < DigiMax);
      return base.IsValidField(index);
    }

    #endregion public methods
  }

}
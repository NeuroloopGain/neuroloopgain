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

using System.Globalization;

namespace NeuroLoopGainLibrary.Filters
{
  public abstract class FilterBase
  {
    #region Protected Fields

    protected FilterDirectionType FDirection = FilterDirectionType.ForwardOnly;
    protected FilterStateBase SavedState;

    #endregion Protected Fields

    #region Protected Properties

    protected bool UseFirstSampleToReset { get; set; }

    #endregion Protected Properties

    #region Protected Methods

    protected virtual FilterStateBase DoBackupFilterState()
    {
      return null;
    }

    protected virtual int DoCheckSettings()
    {
      // SampleFrequency should be > 0 Hz
      return Setting[1].Value <= 0 ? 1 : 0;
    }

    protected virtual void DoClearSavedState()
    {
      SavedState = null;
    }

    protected virtual void DoRestoreFilterState(FilterStateBase filterState)
    {
      // Default no action
    }

    protected FilterBase(int nrSettings = 2)
    {
      UseFirstSampleToReset = false;
      FDirection = FilterDirectionType.ForwardOnly;
      Setting = new FilterSetting(nrSettings) { NewSettings = true };
      Setting[0].Info = "Abstract Custom Filter";
      Setting[2].Value = 1; // Gain          
      // // Original code >>>>>>
      //   inherited;
      //   FDirection:=ForwardOnly;
      //   FNewSettings:=True;
      //   SetNrSettings(2);
      //   FSettingInfo[0]:='Abstract Custom Filter';
      //   Gain:=1;
      // // <<<<<< Original code
    }

    #endregion Protected Methods

    #region Public Properties

    public string AsString
    {
      get
      {
        // TODO: Check if number formatting is right
        int i;
        string result = string.Empty;
        for (i = 2; i <= NrSettings; i++)
        {
          result += Setting[i].Value.ToString("G", CultureInfo.InvariantCulture);
          if (i != NrSettings)
            result += "/";          
        }
        result = string.Format("{0}({1})", FilterTools.GetFilterName(FilterTools.GetFilterType(this)), result);
        return result;
      }
    }

    public virtual FilterDirectionType Direction
    {
      get
      {
        return FDirection;
      }
      protected set
      {
        FDirection = value;
      }
    }

    public virtual string EDFFilterStr
    {
      get
      {
        return AsString;
      }
    }

    public string FilterDescription
    {
      get
      {
        return Setting[0].Info;
      }
    }

    public virtual double Gain
    {
      get
      {
        return Setting[2].Value;
      }
      protected set
      {
        Setting[2].Value = value;
      }
    }

    public virtual string InputSignalPhysicalDimension { get; set; }

    public int NrSettings
    {
      get
      {
        return Setting.Count;
      }
    }

    public virtual double SampleFrequency
    {
      get
      {
        return Setting[1].Value;
      }
      set
      {
        Setting[1].Value = value;
      }
    }

    public FilterSetting Setting
    {
      get;
      protected set;
    }

    #endregion Public Properties

    #region Public Methods

    public FilterStateBase BackupFilterState()
    {
      return DoBackupFilterState();
    }

    public int CheckSettings()
    {
      return DoCheckSettings();
    }

    public void ClearSavedState()
    {
      DoClearSavedState();
    }

    public abstract void FilterSamples(double[] samplesIn, double[] samplesOut, int idxStart, int idxEnd, int outIdxStart = -1);

    public virtual void Reset()
    {
      UseFirstSampleToReset = false;
    }

    public virtual void Reset(bool useNextSample)
    {
      UseFirstSampleToReset = useNextSample;
    }

    public abstract void Reset(double xn);

    public void RestoreFilterState(FilterStateBase filterState)
    {
      DoRestoreFilterState(filterState);
    }

    public void RestoreState()
    {
      DoRestoreFilterState(SavedState);
    }

    public void SaveState()
    {
      DoClearSavedState();
      SavedState = BackupFilterState();
    }

    public override string ToString()
    {
      return AsString;
    }

    #endregion Public Methods
  }

}

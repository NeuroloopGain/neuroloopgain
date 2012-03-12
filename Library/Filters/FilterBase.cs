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
using System.Data;
using System.Globalization;
using System.Text;
using NeuroLoopGainLibrary.Lists;

namespace NeuroLoopGainLibrary.Filters
{
  /// <summary>
  /// Class that is the ancestor of all sampled signal filters
  /// </summary>
  public abstract class FilterBase
  {
    #region private fields

    /// <summary>
    /// The direction the filter is processing input samples
    /// </summary>
    private FilterDirectionType _direction = FilterDirectionType.Forward;

    #endregion private fields

    #region protected fields

    /// <summary>
    /// Previous filter state storage 
    /// </summary>
    protected FilterStateBase SavedState;

    #endregion protected fields

    #region protected properties

    /// <summary>
    /// Gets or sets a value indicating whether the next input sample should be used to initialise the filter state.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if use next sample to initialise the filter state; otherwise, <c>false</c>.
    /// </value>
    protected bool UseFirstSampleToReset { get; set; }

    #endregion protected properties

    #region protected methods

    /// <summary>
    /// Makes a backup of the current filter state.
    /// </summary>
    /// <returns>returns a <see cref="FilterStateBase"/> containing the current filter state.</returns>
    protected virtual FilterStateBase DoBackupFilterState()
    {
      return null;
    }

    /// <summary>
    /// Checks the filter settings.
    /// </summary>
    /// <returns><c>0</c> if all settings are ok; otherwise, the index of the invalid setting.</returns>
    protected virtual int DoCheckSettings()
    {
      // SampleFrequency should be > 0 Hz
      return SampleFrequency <= 0 ? 1 : 0;
    }

    /// <summary>
    /// Clears the backup of the saved filter state.
    /// </summary>
    protected virtual void DoClearSavedState()
    {
      SavedState = null;
    }

    /// <summary>
    /// Does the filtering of the samples.
    /// </summary>
    /// <param name="samplesIn">The samples in.</param>
    /// <param name="samplesOut">The samples out.</param>
    /// <param name="idxStart">The input samples array start index.</param>
    /// <param name="idxEnd">The input samples array end index.</param>
    /// <param name="outIdxStart">The output samples array start index.</param>
    protected abstract void DoFilterSamples(double[] samplesIn, double[] samplesOut, int idxStart, int idxEnd, int outIdxStart = -1);

    /// <summary>
    /// Reset the filter state.
    /// </summary>
    protected virtual void DoReset()
    {
      UseFirstSampleToReset = false;
    }

    /// <summary>
    /// Reset the filter state using the next sample.
    /// </summary>
    /// <param name="useNextSample">if set to <c>true</c> use the next sample.</param>
    protected virtual void DoReset(bool useNextSample)
    {
      UseFirstSampleToReset = useNextSample;
    }

    /// <summary>
    /// Resets the filter using the value.
    /// </summary>
    /// <param name="value">The value.</param>
    protected abstract void DoReset(double value);

    /// <summary>
    /// Restores a previous filter state.
    /// </summary>
    /// <param name="filterState">Previous state of the filter.</param>
    protected virtual void DoRestoreFilterState(FilterStateBase filterState)
    {
      // Default no action
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterBase"/> class.
    /// </summary>
    /// <param name="nrSettings">The number of filter settings.</param>
    protected FilterBase(int nrSettings)
    {
      _stringSetting = new IndexerGetterSetter<string>();
      _stringSetting.GetCount += StringSetting_GetCount;
      _stringSetting.GetValue += StringSetting_GetValue;
      _stringSetting.SetValue += StringSetting_SetValue;

      if (nrSettings < 2)
        throw new ArgumentOutOfRangeException("nrSettings", @"Number of settings should be >= 2");

      UseFirstSampleToReset = false;
      _direction = FilterDirectionType.Forward;
      Setting = new FilterSettings(nrSettings) { NewSettings = true };
      Setting[0].Info = "Abstract Custom Filter";
      Setting[0].DimensionInfo = string.Empty;
      Setting[0].IsReadOnly = true;
      Setting[1].Info = "SampleFrequency";
      Setting[2].Value = 1;
      Setting[2].Info = "Gain";
      Setting[2].DimensionInfo = string.Empty;
    }

    protected virtual void StringSetting_SetValue(object sender, int index, string value)
    {
      throw new FilterException("String setting not supported");
    }

    protected virtual string StringSetting_GetValue(object sender, int index)
    {
      if (index <= NrSettings)
        throw new IndexOutOfRangeException();
      return string.Empty;
    }

    protected virtual int StringSetting_GetCount(object sender)
    {
      return 0;
    }

    #endregion protected methods

    #region public properties

    /// <summary>
    /// Gets or sets the filter direction.
    /// </summary>
    /// <value>
    /// The direction.
    /// </value>
    public virtual FilterDirectionType Direction
    {
      get
      {
        return _direction;
      }
      protected set
      {
        _direction = value;
      }
    }

    /// <summary>
    /// Gets the filter description.
    /// </summary>
    public string FilterDescription
    {
      get
      {
        return Setting[0].Info;
      }
    }

    /// <summary>
    /// Gets or sets the gain.
    /// </summary>
    /// <value>
    /// The gain.
    /// </value>
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

    private string _inputSignalPhysicalDimension;

    /// <summary>
    /// Gets or sets the physical dimension of the input signal.
    /// This value is used by some filters to set the filter setting information.
    /// </summary>
    /// <value>
    /// The input signal physical dimension.
    /// </value>
    public virtual string InputSignalPhysicalDimension
    {
      get { return _inputSignalPhysicalDimension; }
      set
      {
        if (!string.IsNullOrEmpty(OutputSignalPhysicalDim) &&
          OutputSignalPhysicalDim.Equals(_inputSignalPhysicalDimension))
          OutputSignalPhysicalDim = value;
        _inputSignalPhysicalDimension = value;
      }
    }

    /// <summary>
    /// Gets the number of settings.
    /// </summary>
    public int NrSettings
    {
      get
      {
        return Setting.Count;
      }
    }

    /// <summary>
    /// Gets or sets the sample frequency of the input signal.
    /// </summary>
    /// <value>
    /// The sample frequency.
    /// </value>
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

    /// <summary>
    /// Gets the filter setting indexer.
    /// </summary>
    public FilterSettings Setting
    {
      get;
      private set;
    }

    protected string OutputSignalPhysicalDim;
    public virtual string OutputSignalPhysicalDimension
    {
      get { return OutputSignalPhysicalDim; }
      set { throw new ReadOnlyException(); }
    }

    private readonly IndexerGetterSetter<string> _stringSetting;
    public virtual IndexerGetterSetter<string> StringSetting
    {
      get { return _stringSetting; }
    }

    #endregion public properties

    #region public methods

    /// <summary>
    /// Backups the state of the filter.
    /// </summary>
    /// <returns>The current filter state</returns>
    public FilterStateBase BackupFilterState()
    {
      return DoBackupFilterState();
    }

    /// <summary>
    /// Checks the settings.
    /// </summary>
    /// <returns><c>0</c> if all settings are ok; otherwise, the index of the invalid setting.</returns>
    public int CheckSettings()
    {
      return DoCheckSettings();
    }

    /// <summary>
    /// Clears the saved filter state.
    /// </summary>
    public void ClearSavedState()
    {
      DoClearSavedState();
    }

    /// <summary>
    /// Filters a single sample.
    /// </summary>
    /// <param name="sampleIn">The input sample.</param>
    /// <returns>The filter sample</returns>
    public double FilterSample(double sampleIn)
    {
      double[] buffer = new[] { sampleIn };
      FilterSamples(buffer, buffer, 0, 0);
      return buffer[0];
    }

    /// <summary>
    /// Filters the samples.
    /// </summary>
    /// <param name="samplesIn">The samples in.</param>
    /// <param name="samplesOut">The samples out.</param>
    /// <param name="idxStart">The input samples array start index.</param>
    /// <param name="idxEnd">The input samples array end index.</param>
    /// <param name="outIdxStart">The output samples array start index.</param>
    public void FilterSamples(double[] samplesIn, double[] samplesOut, int idxStart, int idxEnd, int outIdxStart = -1)
    {
      DoFilterSamples(samplesIn, samplesOut, idxStart, idxEnd, outIdxStart);
    }

    /// <summary>
    /// Reset the filter state.
    /// </summary>
    public virtual void Reset()
    {
      DoReset();
    }

    /// <summary>
    /// Reset the filter state using the next sample.
    /// </summary>
    /// <param name="useNextSample">if set to <c>true</c> [use next sample].</param>
    public virtual void Reset(bool useNextSample)
    {
      DoReset(useNextSample);
    }

    /// <summary>
    /// Reset the filter state using the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    public virtual void Reset(double value)
    {
      DoReset(value);
    }

    /// <summary>
    /// Restores the state of the filter.
    /// </summary>
    /// <param name="filterState">State of the filter.</param>
    public void RestoreFilterState(FilterStateBase filterState)
    {
      DoRestoreFilterState(filterState);
    }

    /// <summary>
    /// Restores previous saved filter state.
    /// </summary>
    public void RestoreState()
    {
      DoRestoreFilterState(SavedState);
    }

    /// <summary>
    /// Saves the current filter state.
    /// </summary>
    public void SaveState()
    {
      DoClearSavedState();
      SavedState = BackupFilterState();
    }

    /// <summary>
    /// ToString() variant used as EDF(+) file filter description.
    /// </summary>
    /// <returns></returns>
    public virtual string ToEDFString()
    {
      return ToString();
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();

      int i;
      for (i = 2; i <= NrSettings; i++)
      {
        if (sb.Length > 0)
          sb.Append('/');
        sb.Append(Setting[i].Value.ToString("G", CultureInfo.InvariantCulture));
      }

      // Append string settings
      i = NrSettings + 1;
      string s;
      do
      {
        s = StringSetting[i];
        if (!string.IsNullOrEmpty(s))
        {
          if (sb.Length > 0)
            sb.Append('/');
          sb.Append(s);
        }
        i++;
      } while (!string.IsNullOrEmpty(s));

      return string.Format("{0}({1})", FilterTools.GetFilterName(FilterTools.GetFilterType(this)), sb);
    }

    #endregion public methods
  }
}

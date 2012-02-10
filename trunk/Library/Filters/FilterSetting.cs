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

using System.Collections.Generic;

namespace NeuroLoopGainLibrary.Filters
{
  public class FilterSetting
  {
    #region Protected Fields

    protected List<SettingInfo> Values;

    #endregion Protected Fields

    #region Constructors

    public FilterSetting(int count)
    {
      Values = new List<SettingInfo>();
      for (int i = 0; i < count + 1; i++)
        Values.Add(new SettingInfo { DimensionInfo = "Hz", IsReadOnly = false });
      Values[2].DimensionInfo = string.Empty;
    }

    #endregion Constructors

    #region Public Properties

    public int Count
    {
      get { return Values.Count - 1; }
    }

    public bool NewSettings { get; set; }

    public SettingInfo this[int index]
    {
      get { return Values[index]; }
      set
      {
        if (!Values[index].IsReadOnly)
        {
          Values[index] = value;
          NewSettings = true;
        }
      }
    }

    #endregion Public Properties

    #region Public Methods

    public void Add(int count = 1)
    {
      for (int i = 0; i < count; i++)
        Values.Add(new SettingInfo());
    }

    public void SetCount(int nrSettings)
    {
      if (Count > nrSettings)
      {
        int nrToRemove = Count - nrSettings;
        while (nrToRemove > 0)
        {
          Values.RemoveAt(Values.Count - 1);
          nrToRemove--;
        }
      }
      else if (Count < nrSettings)
      {
        int nrToAdd = nrSettings - Count;
        while (nrToAdd > 0)
        {
          Values.Add(new SettingInfo { DimensionInfo = "Hz", IsReadOnly = false });
          nrToAdd--;
        }
      }
    }

    #endregion Public Methods
  }
}
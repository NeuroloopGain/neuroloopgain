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
  /// <summary>
  /// Class to hold the filter settings
  /// </summary>
  public class FilterSettings
  {
    #region private fields

    private readonly List<SettingInfo> _values;

    #endregion private fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterSettings"/> class.
    /// </summary>
    /// <param name="count">The number of settings.</param>
    public FilterSettings(int count)
    {
      _values = new List<SettingInfo>();
      Count = count;
    }

    #endregion Constructors

    #region public properties

    /// <summary>
    /// Gets the number of settings.
    /// </summary>
    public int Count
    {
      get { return _values.Count - 1; }
      private set
      {
        _values.Clear();
        // Always add 1 extra to store filter name
        for (int i = 0; i <= value; i++)
          _values.Add(new SettingInfo { DimensionInfo = "Hz", IsReadOnly = false });
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the settings have changed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the settings changed; otherwise, <c>false</c>.
    /// </value>
    public bool NewSettings { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="SettingInfo"/> at the specified index.
    /// </summary>
    public SettingInfo this[int index]
    {
      get { return _values[index]; }
      set
      {
        if (_values[index].IsReadOnly)
          return;
        _values[index] = value;
        NewSettings = true;
      }
    }

    #endregion public properties

    #region public methods

    /// <summary>
    /// Adds the specified number of (new) settings.
    /// </summary>
    /// <param name="count">The number of settings to add.</param>
    public void Add(int count = 1)
    {
      for (int i = 0; i < count; i++)
        _values.Add(new SettingInfo());
    }

    #endregion public methods
  }
}
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

using System.Data;

namespace NeuroLoopGainLibrary.Filters
{
  /// <summary>
  /// Class to hold the information about a filter setting.
  /// </summary>
  public class SettingInfo
  {
    #region private fields

    private double _value;

    #endregion private fields

    #region public properties

    /// <summary>
    /// Gets or sets the information about the dimension.
    /// </summary>
    /// <value>
    /// The dimension info.
    /// </value>
    public string DimensionInfo { get; set; }

    /// <summary>
    /// Gets or sets the information about the filter.
    /// </summary>
    /// <value>
    /// The info.
    /// </value>
    public string Info { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this setting is read only.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this setting is read only; otherwise, <c>false</c>.
    /// </value>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the value of this setting.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public double Value
    {
      get { return _value; }
      set
      {
        if (IsReadOnly)
          throw new ReadOnlyException("Setting is marked ReadOnly");
        _value = value;
      }
    }

    #endregion public properties
  }
}
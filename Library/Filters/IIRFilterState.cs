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

namespace NeuroLoopGainLibrary.Filters
{
  /// <summary>
  /// Class to store the current state of an IIR filter
  /// </summary>
  public class IIRFilterState : FilterStateBase
  {
    #region private fields

    private readonly double[] _filterStateP;
    private readonly double[] _filterStateZ;

    #endregion private fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="IIRFilterState"/> class.
    /// </summary>
    /// <param name="filterStateP">The current Poles filter state.</param>
    /// <param name="filterStateZ">The current Zeros filter state.</param>
    public IIRFilterState(double[] filterStateP, double[] filterStateZ)
    {
      if (filterStateP != null)
      {
        _filterStateP = new double[filterStateP.Length];
        Array.Copy(filterStateP, _filterStateP, filterStateP.Length);
      }

      if (filterStateZ != null)
      {
        _filterStateZ = new double[filterStateZ.Length];
        Array.Copy(filterStateZ, _filterStateZ, filterStateZ.Length);
      }
    }

    #endregion Constructors

    #region public properties

    /// <summary>
    /// Gets the backup of the Poles filter state.
    /// </summary>
    public double[] FilterStateP
    {
      get
      {
        if (_filterStateP == null)
          return null;

        double[] result = new double[_filterStateP.Length];
        Array.Copy(_filterStateP, result, _filterStateP.Length);

        return result;
      }
    }

    /// <summary>
    /// Gets the backup of the Zeros filter state.
    /// </summary>
    public double[] FilterStateZ
    {
      get
      {
        if (_filterStateZ == null)
          return null;

        double[] result = new double[_filterStateZ.Length];
        Array.Copy(_filterStateZ, result, _filterStateZ.Length);

        return result;
      }
    }

    #endregion public properties
  }
}
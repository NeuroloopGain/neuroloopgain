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
using System.Collections;
using System.Collections.Generic;

namespace NeuroLoopGainLibrary.Lists
{
  /// <summary>
  /// IndexerBase is the base class for a list containing base type items to "typecast" it to a list containing descendant items.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class IndexerBase<T>
  {
    #region private fields

    private IList _internalList;

    #endregion private fields

    #region protected properties

    protected IList InternalList
    {
      get { return _internalList; }
      set
      {
        if (value == null)
          throw new ArgumentException("InternalList");
        _internalList = value;
      }
    }

    #endregion protected properties

    #region protected methods

    protected IndexerBase()
    {
      InternalList = new List<T>();
    }

    #endregion protected methods

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexerBase&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="list">The list containing the base type items.</param>
    public IndexerBase(IList list)
    {
      if (list == null)
        throw new ArgumentNullException("list");
      InternalList = list;
    }

    #endregion Constructors

    #region public properties

    public virtual int Count
    {
      get
      {
        return InternalList.Count;
      }
    }

    public virtual T this[int index]
    {
      get
      {
        return (T)InternalList[index];
      }
      set
      {
        InternalList[index] = value;
      }
    }

    #endregion public properties
  }
}
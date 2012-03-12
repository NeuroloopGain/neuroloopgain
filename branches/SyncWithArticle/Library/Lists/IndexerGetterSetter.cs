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

using NeuroLoopGainLibrary.Errorhandling;

namespace NeuroLoopGainLibrary.Lists
{
  public class IndexerGetterSetter<T> : IndexerBase<T>
  {
    #region delegates and events

    #region delegates

    public delegate int IndexerCountGetter(object sender);
    public delegate T IndexerValueGetter(object sender, int index);
    public delegate void IndexerValueSetter(object sender, int index, T value);

    #endregion delegates
    #region events

    public event IndexerCountGetter GetCount;

    public event IndexerValueGetter GetValue;

    public event IndexerValueSetter SetValue;

    #endregion events

    #endregion delegates and events

    #region protected methods

    protected int OnGetCount()
    {
      IndexerCountGetter handler = GetCount;
      if (handler != null)
        return handler(this);
      throw new ListException("No GetCount set.");
    }

    protected T OnGetValue(int index)
    {
      IndexerValueGetter handler = GetValue;

      if (handler != null)
        return handler(this, index);
      throw new ListException("No ValueGetter set.");
    }

    protected void OnSetValue(int index, T value)
    {
      IndexerValueSetter handler = SetValue;

      if (handler != null)
        handler(this, index, value);
      else
        throw new ListException("No ValueSetter set.");
    }

    #endregion protected methods

    #region public properties

    public override int Count
    {
      get { return OnGetCount(); }
    }

    public override T this[int index]
    {
      get { return OnGetValue(index); }
      set { OnSetValue(index, value); }
    }

    #endregion public properties
  }
}
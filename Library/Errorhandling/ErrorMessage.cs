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
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace NeuroLoopGainLibrary.Errorhandling
{
  public class ErrorMessage
  {
		#region private fields 

    private readonly OrderedDictionary _messages;

		#endregion private fields 

		#region Private Methods 

    private string GetMessage()
    {
      StringBuilder sb = new StringBuilder();
      foreach (string msg in _messages.Keys)
        sb.AppendLine(msg);
      return sb.ToString();
    }

		#endregion Private Methods 

		#region protected properties 

    // keys are messages, values are ids
    protected OrderedDictionary Messages { get { return _messages; } }

		#endregion protected properties 

		#region protected methods 

    protected int[] GetIds()
    {
      List<int> ids = new List<int>();
      if (_messages.Count > 0)
      {
        ids.AddRange(_messages.Values.Cast<int>().Where(id => id > 0));
        if (ids.Count > 0)
          return ids.ToArray();
      }
      return null;
    }

		#endregion protected methods 

		#region Constructors 

    public ErrorMessage()
    {
      _messages = new OrderedDictionary();
    }

		#endregion Constructors 

		#region public fields 

    public const int EIdException = 1;
    public const int EIdFileAlreadyInList = EIdUnit + 1;
    public const int EIdInvalidFileFormat = EIdUnit + 2;
    public const int EIdInvalidPatientId = EIdUnit + 3;
    public const int EIdUnit = 1000;

		#endregion public fields 

		#region public properties 

    public string Message
    {
      get
      {
        return GetMessage();
      }
    }

    public bool Signaled { get; private set; }

		#endregion public properties 

		#region public methods 

    public void Add(string message, int id = 0)
    {
      _messages.Add(message, id);
      Signaled = true;
    }

    public void Add(string formatString, object[] args, int id = 0)
    {
      Add(string.Format(formatString, args), id);
    }

    public void Clear()
    {
      _messages.Clear();
      Signaled = false;
    }

    public bool ContainsId(int id)
    {
      return _messages.Values.Cast<int>().Any(idInlist => idInlist == id);
    }

    public bool ContainsId(int minimumId, int maximumId)
    {
      return _messages.Values.Cast<int>().Any(idInList => (idInList >= minimumId) && (idInList <= maximumId));
    }

    public void IfThen(bool condition, string errorMessage, int id = 0)
    {
      if (!condition)
        Add(errorMessage, id);
    }

    public void IfThen(bool condition, ref bool result, string errorMessage, int id = 0)
    {
      if (!condition)
      {
        result = false;
        Add(errorMessage, id);
      }
    }

		#endregion public methods 
  }
}

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

using System.IO;

namespace NeuroLoopGainLibrary.General
{
  /// <summary>
  /// Functions to get application specific information
  /// </summary>
  public static class ApplicationHelper
  {
    /// <summary>
    /// Gets the path to the current executable. 
    /// </summary>
    /// <value>The path to the current executable.</value>
    public static string ExecutablePath
    {
      get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase; }
    }

    /// <summary>
    /// Gets the path to the current executable, excluding the executables' name. 
    /// A directoryseperator is included at the end.
    /// </summary>
    /// <value>The path to the current executable.</value>
    public static string ExeDir
    {
      get { return Path.GetDirectoryName(ExecutablePath) + Path.DirectorySeparatorChar; }
    }

    /// <summary>
    /// Gets the complete path for a file in the application folder (folder where the executable is located).
    /// </summary>
    /// <param name="filename">The file to get the complete path for..</param>
    /// <returns>Full path to the file.</returns>
    public static string GetFilenameApplicationFolder(string filename)
    {
      return Path.Combine(ExeDir, filename);
    }
  }
}
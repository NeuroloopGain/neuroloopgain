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
using NLog;

namespace NeuroLoopGainLibrary.Logging
{
  /// <summary>
  /// Class for writing to a log file or log server (using NLog: http://http://nlog-project.org/)
  /// </summary>
  public static class ErrorLogger
  {
    #region private fields

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    #endregion

    #region public methods

    /// <summary>
    /// Writes an error to the log.
    /// </summary>
    /// <param name="msg">Message to write to the log.</param>
    public static void WriteErrorLog(string msg)
    {
      Logger.Error(msg);
    }

    /// <summary>
    /// Writes the exception to the log.
    /// </summary>
    /// <param name="ex">The exception to write.</param>
    public static void WriteExceptionToLog(Exception ex)
    {
      Logger.ErrorException(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), ex);
      /*WriteLog(ex.Message + "\n" + ex.StackTrace);*/
    }

    /// <summary>
    /// Writes to the log.
    /// </summary>
    /// <param name="msg">Message to write to the log.</param>
    public static void WriteLog(string msg)
    {
      Logger.Info(msg);
    }

    /// <summary>
    /// Writes a formatted message to the log.
    /// </summary>
    /// <param name="msg">Message to write to the log..</param>
    /// <param name="args">An System.Object array containing zero or more objects to format.</param>
    public static void WriteLogFormat(string msg, params object[] args)
    {
      WriteLog(string.Format(msg, args));
    }    

    #endregion
  }
}

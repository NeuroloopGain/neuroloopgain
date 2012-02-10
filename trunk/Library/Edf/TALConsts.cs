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

using System.Globalization;

namespace NeuroLoopGainLibrary.Edf
{
  /// <summary>
  /// 
  /// </summary>
  public static class TALConsts
  {
		#region public fields 

    // everything except digits, 
    // '+', '-', '.', 'e' or 'E' will give a match
    public const string AllowedFloatCharsExpr = @"[^\d;\+;\-;\.;e;E]";
    public const string AnnotationSignalNrError = "AnnotationSignalNr should be >= 0";
    public const char c20 = (char)20;
    public const char c21 = (char)21;
    public static CultureInfo ciEnglishUS = new CultureInfo("en-US");
    public const string InvalidBlockDuration = "Invalid block duration";
    public const string InvalidBlockIndex = "Invalid block index";
    public const string InvalidBlockOnset = "Invalid block onset";
    public const string InvalidFileStartDateTime = "Invalid filestart date/time";
    public const string NotAllTALBlocksAreAvailable = "Not all TAL blocks are available";
    public const string UnableToAddTALBlock0 = "Unable to add TAL block 0";
    public const string UnableToPerformRequestedOperation = "Unable to perform requested operation";
    // everything except digits, '+','-' or '.' will give a match
    public const string ValidFloatCharsExpr = @"[^\d;\+;\-;\.]";
    // everything except digits and '.' will give a match
    public const string ValidFloatExclMinPlusExpr = @"[^\d;\.]";

		#endregion public fields 
  }
}
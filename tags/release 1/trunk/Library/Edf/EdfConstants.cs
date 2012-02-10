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

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfConstants
  {
		#region public fields 

    public const string AnnotationsSignalLabel = "EDF Annotations";
    public const string DataRecordsNotOrdered = "Data records are not ordered in time";
    public const string DefaultDateFormat = "dd-MMM-yyyy";
    public const char DefaultDecimalSeparator = '.';
    public const char DefaultThousandSeparator = '\0';
    public const int ErrorIdEdfPlusBlockOrder = 1000;
    public const string FileAlreadyContainsData = "File already contains data.";
    public const string FileIsReadOnly = "File is read-only.";
    public const string IdentifierEdfPlusC = "EDF+C";
    public const string IdentifierEdfPlusD = "EDF+D";
    public const string LogFloatMask = "sign*LN[sign*()/()]/()";
    public const string LogFloatSignalLabel = "Filtered";
    public const string NoFileName = "No filename";
    public const string NrSignalsShouldBeMoreThan0 = "Number of signals should be > 0";
    public const int RecommendedMaxBlockSize = 61440;
    public const string SignalLabelAnnotations = "EDF Annotations";

		#endregion public fields 
  }

}

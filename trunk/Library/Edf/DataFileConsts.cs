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
  public static class DataFileConsts
  {
    public const string DataFileDataExists = "Data file already contains data";
    public const string DataFileFieldValue = "Invalid field value(s)";
    public const string DataFileHeaderInvalid = "Invalid or imcomplete file header";
    public const string DataFileIsCreating = "Data file is creating";
    public const string DataFileIsOpen = "Data file is already open";
    public const string DataFileIsNotOpen = "Busy creating new file";
    public const string DataFileReadData = "Error reading file data";
    public const string DataFileReadOnly = "File is opened read only";
    public const string DataFileWriteData = "Error writing file data";
    public const string DataFileErrorReadingTAL = "Error while reading Annotations";
    public const string DataFileContinuous = "Data file is continuous. Invalid datablock Offset specified.";
    public const string DataFileNotContinuous = "Data file is not continuous. Specify datablock Offset.";
    public const string DataFileInvalidBlock0Onset = "Invalid datablock 0 onset (should be 0 <= t < 1)";
    public const string EDFPlusFileNameExt = "Invalid filename extension. Should be .edf or .EDF";
  }
}
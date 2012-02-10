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
  public enum AnnotationType
  {
    InvalidAnnotation = -1,
    RecordingStarts = 0,
    RecordingEnds = 1,
    LightsOff = 2,
    LightsOn = 3,
    NapStarts = 4,
    NapEnds = 5,
    PatientReportsSleep = 6,
    PatientReportsNoSleep = 7,
    PatientReportsDream = 8,
    PatientReportsNoDream = 9,

    SleepStageWake = 1000,
    SleepStage1 = 1001,
    SleepStage2 = 1002,
    SleepStage3 = 1003,
    SleepStage4 = 1004,
    SleepStageRem = 1005,
    SleepStageMovement = 1006,
    SleepStageUnknown = 1007,
    SleepStageN1 = 1008,
    SleepStageN2 = 1009,
    SleepStageN3 = 1010,

    ApneaObstructive = 2000,
    ApneaCentral = 2001,
    ApneaMixed = 2002,
    ApneaGeneral = 2003,
    ApneaHypopnea = 2004,
    ApneaRera = 2005,

    LimbMovementGeneral = 3000,
    LimbMovementArousal = 3001,

    EEGArousal = 4000,

    // Custom annotations
    Desaturation = 10000,
    HypoFlow = 10001,
    HypoFlowRecover = 10002,
    ApnoFlow = 10003,
    ApnoFlowRecover = 10004,
    ApnoEffort = 10005,
    ApnoEffortRecover = 10006,
    AdjustThresholds = 10007,

    BodyPosProne = 10100,
    BodyPosUpsideDown = 10101,
    BodyPosLeft = 10102,
    BodyPosRight = 10103,
    BodyPosUpright = 10104,
    BodyPosSupine = 10105,

    CPAPOFF = 10200,
    CPAP4cmH2O = 10204,
    CPAP5cmH2O = 10205,
    CPAP6cmH2O = 10206,
    CPAP7cmH2O = 10207,
    CPAP8cmH2O = 10208,
    CPAP9cmH2O = 10209,
    CPAP10cmH2O = 10210,
    CPAP11cmH2O = 10211,
    CPAP12cmH2O = 10212,
    CPAP13cmH2O = 10213,
    CPAP14cmH2O = 10214,
    CPAP15cmH2O = 10215,
    CPAP16cmH2O = 10216,
  }
}

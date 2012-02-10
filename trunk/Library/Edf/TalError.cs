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

namespace NeuroLoopGainLibrary.Edf
{
  [Flags]
  public enum TalError
  {
    None = 0,
    InvalidFormat = 1,
    MissingTimeEvent = 2,   // No time-keeping event found
    ExtraTimeEvent = 4,     // Only first TAL signal has time keeping event
    BufferFull = 8,         // Buffer to small to store all data
    InvalidEntry = 16,      // Missing separator characters #20,#21
    InvalidOnset = 32,      // Invalid Onset
    InvalidDuration = 64,   // Invalid Duration
    AddToList = 128,        // Unable to add annotation to list
    BlockNotFound = 512,    // Invalid BlockNumber
    BlockIndex = 1024,      // Invalid Block Index
    BlockOrder = 2048,      // BlocksList are not ordered by ascending onset
    StartOfList = 4096,     // Pointer at first Annotation
    EndOfList = 8192        // Pointer at last Annotation
  }
}
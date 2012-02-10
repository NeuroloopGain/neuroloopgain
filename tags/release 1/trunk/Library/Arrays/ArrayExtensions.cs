﻿//
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

namespace NeuroLoopGainLibrary.Arrays
{
  // Define other methods and classes here
  public static class ArrayExtensions
  {
    public static void Fill(this double[] array, double value)
    {
      for (int i = 0; i < array.Length; i++)
        array[i] = value;
    }

    public static void Fill(this int[] array, int value)
    {
      for (int i = 0; i < array.Length; i++)
        array[i] = value;
    }

    public static void Fill(this long[] array, long value)
    {
      for (int i = 0; i < array.Length; i++)
        array[i] = value;
    }   
  }
}

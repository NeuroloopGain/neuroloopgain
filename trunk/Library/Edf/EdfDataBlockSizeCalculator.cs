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
using System.Collections.Generic;
using System.Linq;

namespace NeuroLoopGainLibrary.Edf
{
  public static class EdfDataBlockSizeCalculator
  {
    private static int GetAllIntegerNrSamplesBlockSize(IList<double> fs)
    {
      double blockSum = fs.Sum();
      int i = 0;
      bool allInts;
      do
      {
        i++;
        if (i * blockSum > int.MaxValue)
          return -1;
        allInts = true;
        int idx = 0;
        while (allInts && (idx < fs.Count))
        {
          allInts &= IsInt(fs[idx] * i);
          idx++;
        }
      } while (!allInts);

      return (int)(i * blockSum) * 2;
    }

    private static bool IsInt(double d)
    {
      return (int)d == d;
    }

    public static double Calculate(IList<double> sampleFrequencies, int maxBlockSize,
                                   List<EdfDataBlockSizeCalculatorResult> results = null)
    {
      int intBlockSizeMin = GetAllIntegerNrSamplesBlockSize(sampleFrequencies);
      long iMax = (long)Math.Truncate(((double)maxBlockSize / intBlockSizeMin) * 1000000);
      int idx = -1;
      double minError = double.MaxValue;
      for (int i = 1; (i < iMax && minError > 0); i++)
      {
        double maxError = double.NaN;
        foreach (double t in sampleFrequencies)
        {
          double n = i * t;
          double e = n / 1000000;
          if ((int)e <= 0)
          {
            maxError = double.NaN;
            break;
          }
          double error = (e - Math.Floor(e)) / (int)e;
          maxError = double.IsNaN(maxError) ? error : Math.Max(error, maxError);
        }
        if (double.IsNaN(maxError) || (maxError > minError))
          continue;
        minError = maxError;
        idx = i;
        if (minError > 0)
        {
          if (results != null)
            results.Add(new EdfDataBlockSizeCalculatorResult((double)i / 1000000, minError));
        }
        else
        {
          int datablockSize = 0;
          for (int j = 0; j < sampleFrequencies.Count; j++)
            datablockSize += (int)Math.Round(sampleFrequencies[j] * ((double)i / 1000000), MidpointRounding.AwayFromZero) * 2;
          int k = 1;
          while (k * datablockSize <= maxBlockSize)
          {
            if (results != null)
              results.Add(new EdfDataBlockSizeCalculatorResult((double)i * k / 1000000, minError));
            k++;
          }
        }
      }
      if (idx > 0)
        return (double)idx / 1000000;
      return -1;
    }
  }

  public class EdfDataBlockSizeCalculatorResult
  {
    public double Duration { get; set; }
    public double MaxRelativeError { get; set; }

    public EdfDataBlockSizeCalculatorResult(double duration, double maxRelativeError)
    {
      Duration = duration;
      MaxRelativeError = maxRelativeError;
    }
  }
}
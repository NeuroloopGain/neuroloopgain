//
// NeuroLoopGain Analysis
// Application implementing NeuroLoopGain analysis, based on the mathematical derivations and specifications in the article 
// "Analysis of a sleep-dependent neuronal feedback loop: the slow-wave microcontinuity of the EEG" 
// by B Kemp, AH Zwinderman, B Tuk, HAC Kamphuisen and JJL Oberyé in IEEE-BME 47(9), 2000: 1185-1194.
//
// Copyright 2012 Bob Kemp and Marco Roessen
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
using System.Linq;
using System.Windows.Forms;

namespace NeuroLoopGain
{
  public partial class FormPiBHistogram : Form
  {
    #region Event handlers

    /// <summary>
    /// Handles the Resize event of the Histogram control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void Histogram_Resize(object sender, EventArgs e)
    {
      int newHeight = Size.Height - (int)Math.Truncate(Size.Height * 0.2);
      int newWidth = Size.Width - (int)Math.Truncate(Size.Width * 0.2);
      chartHistogram.SetBounds(chartHistogram.Location.X, chartHistogram.Location.Y, newWidth, newHeight);
    }

    #endregion Event handlers

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="FormPiBHistogram"/> class.
    /// </summary>
    public FormPiBHistogram()
    {
      InitializeComponent();
    }

    #endregion Constructors

    #region public methods

    /// <summary>
    /// Sets the information that should be displayed by the chart.
    /// </summary>
    /// <param name="histogramInfo">The histogram info.</param>
    public void SetHistogramInfo(HistogramInfo histogramInfo)
    {
      // Fill configuration information
      labelFileName.Text = histogramInfo.FileName;
      labelSignalLabel.Text = histogramInfo.SignalLabel;
      labelF0.Text = histogramInfo.F0 + @" Hz"; 
      labelFC.Text = histogramInfo.FC + @" Hz"; 
      labelB.Text = histogramInfo.B + @" Hz";
      labelSmoothRate.Text = histogramInfo.SmoothRate.ToString();
      labelSmoothTime.Text = histogramInfo.SmoothTime + @" s";
      if (histogramInfo.UnderSampling != 1)
        labelUndersample.Text = histogramInfo.UnderSampling.ToString();

      // Filling the histogram for SS_SU
      double ymax = double.MinValue;
      double ymin = double.MaxValue;
      for (int k = 0; k < histogramInfo.SU_SS.Length; k++)
      {
        chartHistogram.Series.ElementAt(0).Points.AddXY(k + histogramInfo.SS_SUmin, histogramInfo.SU_SS.ElementAt(k));
        chartHistogram.Series.ElementAt(1).Points.AddXY(k + histogramInfo.SS_SUmin, histogramInfo.SU_SSsmoothed.ElementAt(k));
        double tmp = Math.Max(histogramInfo.SU_SS.ElementAt(k), histogramInfo.SU_SSsmoothed.ElementAt(k));
        if (ymax < tmp)
          ymax = tmp;
        tmp = Math.Min(histogramInfo.SU_SS.ElementAt(k), histogramInfo.SU_SSsmoothed.ElementAt(k));
        if (ymin > tmp)
          ymin = tmp;
      }

      // Representing correlation
      double scaleFactor = ymax / histogramInfo.SU_SSmatch.ElementAt(histogramInfo.PiBvalueLog - histogramInfo.SS_SUmin);
      for (int k = 0; k < histogramInfo.SU_SS.Length; k++)
      {
        chartHistogram.Series.ElementAt(2).Points.AddXY(k + histogramInfo.SS_SUmin, histogramInfo.SU_SSmatch.ElementAt(k) * scaleFactor);
      }

      // Representing PiB value
      chartHistogram.Series.ElementAt(3).Points.AddXY(histogramInfo.PiBvalueLog, 0);
      chartHistogram.Series.ElementAt(3).Points.AddXY(histogramInfo.PiBvalueLog, histogramInfo.SU_SS.ElementAt(histogramInfo.PiBvalueLog - histogramInfo.SS_SUmin));

      chartHistogram.Titles.Add("PiB value title");
      //chartHistogram.Titles.ElementAt(1).Text = String.Format("PiB value = {0} (Log-converted) / {1} (Digital) / {2} (Physical)", histogramInfo.PiBvalueLog, histogramInfo.PiBvalueDigi, histogramInfo.PiBvaluePhysi);
      chartHistogram.Titles.ElementAt(1).Text = String.Format("PiB value = {0} (Log-converted) / {1:#.00} (Physical)", histogramInfo.PiBvalueLog, histogramInfo.PiBvaluePhysi);

      chartHistogram.ChartAreas.ElementAt(0).AxisY.Minimum = ymin - 1;
      chartHistogram.ChartAreas.ElementAt(0).AxisY.Maximum = ymax + 1;
    }

    #endregion public methods
  }
}

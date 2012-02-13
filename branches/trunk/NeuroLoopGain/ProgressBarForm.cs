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
using System.Windows.Forms;

namespace NeuroLoopGain
{
  public partial class ProgressBarForm : Form
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressBarForm"/> class.
    /// </summary>
    public ProgressBarForm()
    {
      InitializeComponent();
    }

    #endregion Constructors

    #region public properties

    /// <summary>
    /// Gets or sets the message that is shown.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public String Message
    {
      get
      {
        return labelMessage.Text;
      }
      set
      {
        labelMessage.Text = value;
      }
    }

    /// <summary>
    /// Gets the progress bar component that shows the progress.
    /// </summary>
    public ProgressBar Progress { get; private set; }

    #endregion public properties
  }
}

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

namespace NeuroLoopGain
{
    partial class MainGUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.buttonStart = new System.Windows.Forms.Button();
      this.textBox = new System.Windows.Forms.TextBox();
      this.buttonSelectEDFInput = new System.Windows.Forms.Button();
      this.textBoxOutputFileName = new System.Windows.Forms.TextBox();
      this.textBoxB = new System.Windows.Forms.TextBox();
      this.textBoxFc = new System.Windows.Forms.TextBox();
      this.textBoxSmootherrate = new System.Windows.Forms.TextBox();
      this.textBoxF0 = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.buttonSpindles = new System.Windows.Forms.Button();
      this.buttonAlpha = new System.Windows.Forms.Button();
      this.buttonSlowWaves = new System.Windows.Forms.Button();
      this.label16 = new System.Windows.Forms.Label();
      this.label17 = new System.Windows.Forms.Label();
      this.textBoxAnalysisPeriod = new System.Windows.Forms.TextBox();
      this.label10 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.signalsComboBox = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.textBoxInputFilename = new System.Windows.Forms.TextBox();
      this.buttonClose = new System.Windows.Forms.Button();
      this.label11 = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.label13 = new System.Windows.Forms.Label();
      this.textBoxLP = new System.Windows.Forms.TextBox();
      this.textBoxHP = new System.Windows.Forms.TextBox();
      this.label14 = new System.Windows.Forms.Label();
      this.label15 = new System.Windows.Forms.Label();
      this.buttonSelectEDFoutput = new System.Windows.Forms.Button();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox4.SuspendLayout();
      this.SuspendLayout();
      // 
      // buttonStart
      // 
      this.buttonStart.Enabled = false;
      this.buttonStart.Location = new System.Drawing.Point(104, 518);
      this.buttonStart.Name = "buttonStart";
      this.buttonStart.Size = new System.Drawing.Size(79, 23);
      this.buttonStart.TabIndex = 0;
      this.buttonStart.Text = "Start Analysis";
      this.buttonStart.UseVisualStyleBackColor = true;
      this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
      // 
      // textBox
      // 
      this.textBox.Location = new System.Drawing.Point(6, 418);
      this.textBox.Multiline = true;
      this.textBox.Name = "textBox";
      this.textBox.Size = new System.Drawing.Size(381, 88);
      this.textBox.TabIndex = 1;
      // 
      // buttonSelectEDFInput
      // 
      this.buttonSelectEDFInput.Location = new System.Drawing.Point(10, 19);
      this.buttonSelectEDFInput.Name = "buttonSelectEDFInput";
      this.buttonSelectEDFInput.Size = new System.Drawing.Size(100, 25);
      this.buttonSelectEDFInput.TabIndex = 2;
      this.buttonSelectEDFInput.Text = "Select EDF file...";
      this.buttonSelectEDFInput.UseVisualStyleBackColor = true;
      this.buttonSelectEDFInput.Click += new System.EventHandler(this.buttonSelectEDF_Click);
      // 
      // textBoxOutputFileName
      // 
      this.textBoxOutputFileName.Location = new System.Drawing.Point(132, 24);
      this.textBoxOutputFileName.Name = "textBoxOutputFileName";
      this.textBoxOutputFileName.Size = new System.Drawing.Size(241, 20);
      this.textBoxOutputFileName.TabIndex = 4;
      this.textBoxOutputFileName.Text = "d:\\Temp\\output.edf";
      // 
      // textBoxB
      // 
      this.textBoxB.Location = new System.Drawing.Point(93, 46);
      this.textBoxB.Name = "textBoxB";
      this.textBoxB.Size = new System.Drawing.Size(85, 20);
      this.textBoxB.TabIndex = 7;
      // 
      // textBoxFc
      // 
      this.textBoxFc.Location = new System.Drawing.Point(93, 74);
      this.textBoxFc.Name = "textBoxFc";
      this.textBoxFc.Size = new System.Drawing.Size(85, 20);
      this.textBoxFc.TabIndex = 8;
      // 
      // textBoxSmootherrate
      // 
      this.textBoxSmootherrate.Location = new System.Drawing.Point(93, 102);
      this.textBoxSmootherrate.Name = "textBoxSmootherrate";
      this.textBoxSmootherrate.Size = new System.Drawing.Size(85, 20);
      this.textBoxSmootherrate.TabIndex = 9;
      // 
      // textBoxF0
      // 
      this.textBoxF0.Location = new System.Drawing.Point(93, 18);
      this.textBoxF0.Name = "textBoxF0";
      this.textBoxF0.Size = new System.Drawing.Size(85, 20);
      this.textBoxF0.TabIndex = 10;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(29, 20);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(19, 13);
      this.label3.TabIndex = 11;
      this.label3.Text = "F0";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(28, 78);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(19, 13);
      this.label4.TabIndex = 12;
      this.label4.Text = "Fc";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(31, 49);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(14, 13);
      this.label5.TabIndex = 13;
      this.label5.Text = "B";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(6, 105);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(64, 13);
      this.label6.TabIndex = 14;
      this.label6.Text = "Smooth rate";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.groupBox2);
      this.groupBox1.Controls.Add(this.label16);
      this.groupBox1.Controls.Add(this.label17);
      this.groupBox1.Controls.Add(this.textBoxAnalysisPeriod);
      this.groupBox1.Controls.Add(this.label10);
      this.groupBox1.Controls.Add(this.label9);
      this.groupBox1.Controls.Add(this.label8);
      this.groupBox1.Controls.Add(this.label7);
      this.groupBox1.Controls.Add(this.label6);
      this.groupBox1.Controls.Add(this.label5);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.textBoxF0);
      this.groupBox1.Controls.Add(this.textBoxSmootherrate);
      this.groupBox1.Controls.Add(this.textBoxFc);
      this.groupBox1.Controls.Add(this.textBoxB);
      this.groupBox1.Location = new System.Drawing.Point(6, 243);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(381, 167);
      this.groupBox1.TabIndex = 15;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Settings";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.buttonSpindles);
      this.groupBox2.Controls.Add(this.buttonAlpha);
      this.groupBox2.Controls.Add(this.buttonSlowWaves);
      this.groupBox2.Location = new System.Drawing.Point(230, 17);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(126, 132);
      this.groupBox2.TabIndex = 25;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Quick settings";
      // 
      // buttonSpindles
      // 
      this.buttonSpindles.Location = new System.Drawing.Point(20, 98);
      this.buttonSpindles.Name = "buttonSpindles";
      this.buttonSpindles.Size = new System.Drawing.Size(88, 25);
      this.buttonSpindles.TabIndex = 17;
      this.buttonSpindles.Text = "Spindles";
      this.buttonSpindles.UseVisualStyleBackColor = true;
      this.buttonSpindles.Click += new System.EventHandler(this.buttonSpindles_Click);
      // 
      // buttonAlpha
      // 
      this.buttonAlpha.Location = new System.Drawing.Point(20, 61);
      this.buttonAlpha.Name = "buttonAlpha";
      this.buttonAlpha.Size = new System.Drawing.Size(88, 25);
      this.buttonAlpha.TabIndex = 16;
      this.buttonAlpha.Text = "Alpha";
      this.buttonAlpha.UseVisualStyleBackColor = true;
      this.buttonAlpha.Click += new System.EventHandler(this.buttonAlpha_Click);
      // 
      // buttonSlowWaves
      // 
      this.buttonSlowWaves.Location = new System.Drawing.Point(20, 23);
      this.buttonSlowWaves.Name = "buttonSlowWaves";
      this.buttonSlowWaves.Size = new System.Drawing.Size(88, 25);
      this.buttonSlowWaves.TabIndex = 15;
      this.buttonSlowWaves.Text = "Slow Waves";
      this.buttonSlowWaves.UseVisualStyleBackColor = true;
      this.buttonSlowWaves.Click += new System.EventHandler(this.buttonSlowWaves_Click);
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Location = new System.Drawing.Point(184, 133);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(12, 13);
      this.label16.TabIndex = 24;
      this.label16.Text = "s";
      // 
      // label17
      // 
      this.label17.AutoSize = true;
      this.label17.Location = new System.Drawing.Point(6, 133);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(77, 13);
      this.label17.TabIndex = 23;
      this.label17.Text = "Analysis period";
      // 
      // textBoxAnalysisPeriod
      // 
      this.textBoxAnalysisPeriod.Location = new System.Drawing.Point(93, 130);
      this.textBoxAnalysisPeriod.Name = "textBoxAnalysisPeriod";
      this.textBoxAnalysisPeriod.Size = new System.Drawing.Size(85, 20);
      this.textBoxAnalysisPeriod.TabIndex = 22;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(184, 105);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(17, 13);
      this.label10.TabIndex = 21;
      this.label10.Text = "/s";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(183, 77);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(20, 13);
      this.label9.TabIndex = 20;
      this.label9.Text = "Hz";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(183, 49);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(20, 13);
      this.label8.TabIndex = 19;
      this.label8.Text = "Hz";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(183, 21);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(20, 13);
      this.label7.TabIndex = 18;
      this.label7.Text = "Hz";
      // 
      // signalsComboBox
      // 
      this.signalsComboBox.Enabled = false;
      this.signalsComboBox.FormattingEnabled = true;
      this.signalsComboBox.Location = new System.Drawing.Point(131, 54);
      this.signalsComboBox.Name = "signalsComboBox";
      this.signalsComboBox.Size = new System.Drawing.Size(241, 21);
      this.signalsComboBox.TabIndex = 16;
      this.signalsComboBox.SelectedIndexChanged += new System.EventHandler(this.signalsComboBox_SelectedIndexChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(14, 59);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(70, 13);
      this.label2.TabIndex = 17;
      this.label2.Text = "Select signal:";
      // 
      // textBoxInputFilename
      // 
      this.textBoxInputFilename.Location = new System.Drawing.Point(131, 22);
      this.textBoxInputFilename.Name = "textBoxInputFilename";
      this.textBoxInputFilename.Size = new System.Drawing.Size(241, 20);
      this.textBoxInputFilename.TabIndex = 18;
      this.textBoxInputFilename.Text = "---";
      this.textBoxInputFilename.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.textBoxInputFilename.TextChanged += new System.EventHandler(this.EdfInputFileNameChange);
      // 
      // buttonClose
      // 
      this.buttonClose.Location = new System.Drawing.Point(208, 518);
      this.buttonClose.Name = "buttonClose";
      this.buttonClose.Size = new System.Drawing.Size(86, 23);
      this.buttonClose.TabIndex = 19;
      this.buttonClose.Text = "Close";
      this.buttonClose.UseVisualStyleBackColor = true;
      this.buttonClose.Click += new System.EventHandler(this.buttonCancel_Click);
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(13, 92);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(182, 13);
      this.label11.TabIndex = 20;
      this.label11.Text = "Filters that were applied to this signal:";
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(33, 118);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(79, 13);
      this.label12.TabIndex = 21;
      this.label12.Text = "Lowest LP filter";
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(33, 144);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(83, 13);
      this.label13.TabIndex = 22;
      this.label13.Text = "Highest HP filter";
      // 
      // textBoxLP
      // 
      this.textBoxLP.Location = new System.Drawing.Point(145, 115);
      this.textBoxLP.Name = "textBoxLP";
      this.textBoxLP.Size = new System.Drawing.Size(68, 20);
      this.textBoxLP.TabIndex = 23;
      this.textBoxLP.Text = "---";
      this.textBoxLP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // textBoxHP
      // 
      this.textBoxHP.Location = new System.Drawing.Point(145, 141);
      this.textBoxHP.Name = "textBoxHP";
      this.textBoxHP.Size = new System.Drawing.Size(68, 20);
      this.textBoxHP.TabIndex = 24;
      this.textBoxHP.Text = "---";
      this.textBoxHP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(219, 118);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(20, 13);
      this.label14.TabIndex = 22;
      this.label14.Text = "Hz";
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Location = new System.Drawing.Point(219, 144);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(20, 13);
      this.label15.TabIndex = 25;
      this.label15.Text = "Hz";
      // 
      // buttonSelectEDFoutput
      // 
      this.buttonSelectEDFoutput.Location = new System.Drawing.Point(11, 22);
      this.buttonSelectEDFoutput.Name = "buttonSelectEDFoutput";
      this.buttonSelectEDFoutput.Size = new System.Drawing.Size(100, 25);
      this.buttonSelectEDFoutput.TabIndex = 26;
      this.buttonSelectEDFoutput.Text = "Select EDF file...";
      this.buttonSelectEDFoutput.UseVisualStyleBackColor = true;
      this.buttonSelectEDFoutput.Click += new System.EventHandler(this.button1_Click);
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.buttonSelectEDFoutput);
      this.groupBox3.Controls.Add(this.textBoxOutputFileName);
      this.groupBox3.Location = new System.Drawing.Point(6, 179);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(381, 60);
      this.groupBox3.TabIndex = 27;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Output file";
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.label15);
      this.groupBox4.Controls.Add(this.label14);
      this.groupBox4.Controls.Add(this.textBoxHP);
      this.groupBox4.Controls.Add(this.textBoxLP);
      this.groupBox4.Controls.Add(this.label13);
      this.groupBox4.Controls.Add(this.label12);
      this.groupBox4.Controls.Add(this.label11);
      this.groupBox4.Controls.Add(this.textBoxInputFilename);
      this.groupBox4.Controls.Add(this.label2);
      this.groupBox4.Controls.Add(this.signalsComboBox);
      this.groupBox4.Controls.Add(this.buttonSelectEDFInput);
      this.groupBox4.Location = new System.Drawing.Point(7, 5);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(379, 170);
      this.groupBox4.TabIndex = 28;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Input file";
      // 
      // MainGUI
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(399, 551);
      this.Controls.Add(this.groupBox4);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.buttonClose);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.textBox);
      this.Controls.Add(this.buttonStart);
      this.Name = "MainGUI";
      this.Text = "NeuroLoop-gain Analyzer";
      this.Shown += new System.EventHandler(this.MainGUI_Shown);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button buttonSelectEDFInput;
        private System.Windows.Forms.TextBox textBoxOutputFileName;
        private System.Windows.Forms.TextBox textBoxB;
        private System.Windows.Forms.TextBox textBoxFc;
        private System.Windows.Forms.TextBox textBoxSmootherrate;
        private System.Windows.Forms.TextBox textBoxF0;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonSpindles;
        private System.Windows.Forms.Button buttonAlpha;
        private System.Windows.Forms.Button buttonSlowWaves;
        private System.Windows.Forms.ComboBox signalsComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxInputFilename;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxLP;
        private System.Windows.Forms.TextBox textBoxHP;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textBoxAnalysisPeriod;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonSelectEDFoutput;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}


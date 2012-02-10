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
    partial class FormPiBHistogram
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.chartHistogram = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelFileName = new System.Windows.Forms.Label();
            this.labelSignalLabel = new System.Windows.Forms.Label();
            this.labelB = new System.Windows.Forms.Label();
            this.labelF0 = new System.Windows.Forms.Label();
            this.labelSmoothTime = new System.Windows.Forms.Label();
            this.labelFC = new System.Windows.Forms.Label();
            this.labelSmoothRate = new System.Windows.Forms.Label();
            this.labelUndersample = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartHistogram)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chartHistogram
            // 
            chartArea1.Name = "ChartArea1";
            this.chartHistogram.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartHistogram.Legends.Add(legend1);
            this.chartHistogram.Location = new System.Drawing.Point(29, 151);
            this.chartHistogram.Name = "chartHistogram";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.LegendText = "SS-SU histogram";
            series1.Name = "SS_SU";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.LegendText = "SS-SU smoothed";
            series2.Name = "SS_SUsmoothed";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.Black;
            series3.Legend = "Legend1";
            series3.LegendText = "Correlation with template";
            series3.Name = "SS_SUmatch";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Color = System.Drawing.Color.Red;
            series4.Legend = "Legend1";
            series4.Name = "PiB value";
            this.chartHistogram.Series.Add(series1);
            this.chartHistogram.Series.Add(series2);
            this.chartHistogram.Series.Add(series3);
            this.chartHistogram.Series.Add(series4);
            this.chartHistogram.Size = new System.Drawing.Size(586, 389);
            this.chartHistogram.TabIndex = 0;
            this.chartHistogram.Text = "chart1";
            title1.BackColor = System.Drawing.Color.White;
            title1.Name = "Histogram of SS-SU for detecting PiB value";
            title1.Text = "Histogram of SS-SU for detecting PiB value";
            this.chartHistogram.Titles.Add(title1);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelFileName);
            this.groupBox1.Controls.Add(this.labelSignalLabel);
            this.groupBox1.Controls.Add(this.labelB);
            this.groupBox1.Controls.Add(this.labelF0);
            this.groupBox1.Controls.Add(this.labelSmoothTime);
            this.groupBox1.Controls.Add(this.labelFC);
            this.groupBox1.Controls.Add(this.labelSmoothRate);
            this.groupBox1.Controls.Add(this.labelUndersample);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(29, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(586, 117);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configuration parameters";
            // 
            // labelFileName
            // 
            this.labelFileName.AutoSize = true;
            this.labelFileName.Location = new System.Drawing.Point(112, 22);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(16, 13);
            this.labelFileName.TabIndex = 15;
            this.labelFileName.Text = "---";
            // 
            // labelSignalLabel
            // 
            this.labelSignalLabel.AutoSize = true;
            this.labelSignalLabel.Location = new System.Drawing.Point(112, 42);
            this.labelSignalLabel.Name = "labelSignalLabel";
            this.labelSignalLabel.Size = new System.Drawing.Size(16, 13);
            this.labelSignalLabel.TabIndex = 14;
            this.labelSignalLabel.Text = "---";
            // 
            // labelB
            // 
            this.labelB.AutoSize = true;
            this.labelB.Location = new System.Drawing.Point(51, 88);
            this.labelB.Name = "labelB";
            this.labelB.Size = new System.Drawing.Size(16, 13);
            this.labelB.TabIndex = 13;
            this.labelB.Text = "---";
            // 
            // labelF0
            // 
            this.labelF0.AutoSize = true;
            this.labelF0.Location = new System.Drawing.Point(51, 65);
            this.labelF0.Name = "labelF0";
            this.labelF0.Size = new System.Drawing.Size(16, 13);
            this.labelF0.TabIndex = 12;
            this.labelF0.Text = "---";
            // 
            // labelSmoothTime
            // 
            this.labelSmoothTime.AutoSize = true;
            this.labelSmoothTime.Location = new System.Drawing.Point(188, 88);
            this.labelSmoothTime.Name = "labelSmoothTime";
            this.labelSmoothTime.Size = new System.Drawing.Size(16, 13);
            this.labelSmoothTime.TabIndex = 11;
            this.labelSmoothTime.Text = "---";
            // 
            // labelFC
            // 
            this.labelFC.AutoSize = true;
            this.labelFC.Location = new System.Drawing.Point(188, 65);
            this.labelFC.Name = "labelFC";
            this.labelFC.Size = new System.Drawing.Size(16, 13);
            this.labelFC.TabIndex = 10;
            this.labelFC.Text = "---";
            // 
            // labelSmoothRate
            // 
            this.labelSmoothRate.AutoSize = true;
            this.labelSmoothRate.Location = new System.Drawing.Point(337, 88);
            this.labelSmoothRate.Name = "labelSmoothRate";
            this.labelSmoothRate.Size = new System.Drawing.Size(16, 13);
            this.labelSmoothRate.TabIndex = 9;
            this.labelSmoothRate.Text = "---";
            // 
            // labelUndersample
            // 
            this.labelUndersample.AutoSize = true;
            this.labelUndersample.Location = new System.Drawing.Point(337, 65);
            this.labelUndersample.Name = "labelUndersample";
            this.labelUndersample.Size = new System.Drawing.Size(90, 13);
            this.labelUndersample.TabIndex = 8;
            this.labelUndersample.Text = "no undersampling";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(243, 88);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Smooth rate:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(243, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Undersample:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(112, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Analysis time:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(112, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "FC:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "B:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "F0:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Signal label:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File name:";
            // 
            // FormPiBHistogram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 564);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chartHistogram);
            this.Name = "FormPiBHistogram";
            this.Text = "FormPiBHistogram";
            this.SizeChanged += new System.EventHandler(this.Histogram_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.chartHistogram)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartHistogram;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelFileName;
        private System.Windows.Forms.Label labelSignalLabel;
        private System.Windows.Forms.Label labelB;
        private System.Windows.Forms.Label labelF0;
        private System.Windows.Forms.Label labelSmoothTime;
        private System.Windows.Forms.Label labelFC;
        private System.Windows.Forms.Label labelSmoothRate;
        private System.Windows.Forms.Label labelUndersample;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
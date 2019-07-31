namespace Sirification
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.MyListBox = new System.Windows.Forms.ListBox();
            this.Start_Button = new System.Windows.Forms.Button();
            this.MyTextBox = new System.Windows.Forms.TextBox();
            this.Stop_Button = new System.Windows.Forms.Button();
            this.MyTabControl = new System.Windows.Forms.TabControl();
            this.MyControlPanel_TabPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ClearLogButton = new System.Windows.Forms.Button();
            this.ShowLogsCheckBox = new System.Windows.Forms.CheckBox();
            this.MyExportLogsButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.MyDumpingCheckBox = new System.Windows.Forms.CheckBox();
            this.MyNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.MyPictureBox = new System.Windows.Forms.PictureBox();
            this.MyLogs_TabPage = new System.Windows.Forms.TabPage();
            this.MyLogInspector_TabPage = new System.Windows.Forms.TabPage();
            this.DatabaseTabPage = new System.Windows.Forms.TabPage();
            this.ExecuteButton = new System.Windows.Forms.Button();
            this.MyDataGridView = new System.Windows.Forms.DataGridView();
            this.SQLTextBox = new System.Windows.Forms.TextBox();
            this.MyTimer = new System.Timers.Timer();
            this.InstallCAButton = new System.Windows.Forms.Button();
            this.MyTabControl.SuspendLayout();
            this.MyControlPanel_TabPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MyNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MyPictureBox)).BeginInit();
            this.MyLogs_TabPage.SuspendLayout();
            this.MyLogInspector_TabPage.SuspendLayout();
            this.DatabaseTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MyDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MyTimer)).BeginInit();
            this.SuspendLayout();
            // 
            // MyListBox
            // 
            this.MyListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MyListBox.FormattingEnabled = true;
            this.MyListBox.HorizontalScrollbar = true;
            this.MyListBox.Location = new System.Drawing.Point(3, 3);
            this.MyListBox.Name = "MyListBox";
            this.MyListBox.Size = new System.Drawing.Size(754, 510);
            this.MyListBox.TabIndex = 0;
            this.MyListBox.SelectedIndexChanged += new System.EventHandler(this.MyListBox_SelectedIndexChanged);
            // 
            // Start_Button
            // 
            this.Start_Button.Location = new System.Drawing.Point(6, 6);
            this.Start_Button.Name = "Start_Button";
            this.Start_Button.Size = new System.Drawing.Size(147, 41);
            this.Start_Button.TabIndex = 1;
            this.Start_Button.Text = "Start";
            this.Start_Button.UseVisualStyleBackColor = true;
            this.Start_Button.Click += new System.EventHandler(this.Start_Button_Click);
            // 
            // MyTextBox
            // 
            this.MyTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MyTextBox.Location = new System.Drawing.Point(3, 3);
            this.MyTextBox.Multiline = true;
            this.MyTextBox.Name = "MyTextBox";
            this.MyTextBox.Size = new System.Drawing.Size(754, 510);
            this.MyTextBox.TabIndex = 2;
            // 
            // Stop_Button
            // 
            this.Stop_Button.Enabled = false;
            this.Stop_Button.Location = new System.Drawing.Point(159, 6);
            this.Stop_Button.Name = "Stop_Button";
            this.Stop_Button.Size = new System.Drawing.Size(138, 41);
            this.Stop_Button.TabIndex = 3;
            this.Stop_Button.Text = "Stop";
            this.Stop_Button.UseVisualStyleBackColor = true;
            this.Stop_Button.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // MyTabControl
            // 
            this.MyTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MyTabControl.Controls.Add(this.MyControlPanel_TabPage);
            this.MyTabControl.Controls.Add(this.MyLogs_TabPage);
            this.MyTabControl.Controls.Add(this.MyLogInspector_TabPage);
            this.MyTabControl.Controls.Add(this.DatabaseTabPage);
            this.MyTabControl.Location = new System.Drawing.Point(12, 12);
            this.MyTabControl.Name = "MyTabControl";
            this.MyTabControl.SelectedIndex = 0;
            this.MyTabControl.Size = new System.Drawing.Size(768, 542);
            this.MyTabControl.TabIndex = 4;
            // 
            // MyControlPanel_TabPage
            // 
            this.MyControlPanel_TabPage.Controls.Add(this.groupBox1);
            this.MyControlPanel_TabPage.Controls.Add(this.MyPictureBox);
            this.MyControlPanel_TabPage.Controls.Add(this.Start_Button);
            this.MyControlPanel_TabPage.Controls.Add(this.Stop_Button);
            this.MyControlPanel_TabPage.Location = new System.Drawing.Point(4, 22);
            this.MyControlPanel_TabPage.Name = "MyControlPanel_TabPage";
            this.MyControlPanel_TabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MyControlPanel_TabPage.Size = new System.Drawing.Size(760, 516);
            this.MyControlPanel_TabPage.TabIndex = 0;
            this.MyControlPanel_TabPage.Text = "Control Panel";
            this.MyControlPanel_TabPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.InstallCAButton);
            this.groupBox1.Controls.Add(this.ClearLogButton);
            this.groupBox1.Controls.Add(this.ShowLogsCheckBox);
            this.groupBox1.Controls.Add(this.MyExportLogsButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.MyDumpingCheckBox);
            this.groupBox1.Controls.Add(this.MyNumericUpDown);
            this.groupBox1.Location = new System.Drawing.Point(303, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(451, 106);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced Options";
            // 
            // ClearLogButton
            // 
            this.ClearLogButton.Location = new System.Drawing.Point(119, 58);
            this.ClearLogButton.Name = "ClearLogButton";
            this.ClearLogButton.Size = new System.Drawing.Size(92, 36);
            this.ClearLogButton.TabIndex = 10;
            this.ClearLogButton.Text = "Clear Logs";
            this.ClearLogButton.UseVisualStyleBackColor = true;
            this.ClearLogButton.Click += new System.EventHandler(this.ClearLogButton_Click);
            // 
            // ShowLogsCheckBox
            // 
            this.ShowLogsCheckBox.AutoSize = true;
            this.ShowLogsCheckBox.Checked = true;
            this.ShowLogsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowLogsCheckBox.Location = new System.Drawing.Point(9, 58);
            this.ShowLogsCheckBox.Name = "ShowLogsCheckBox";
            this.ShowLogsCheckBox.Size = new System.Drawing.Size(79, 17);
            this.ShowLogsCheckBox.TabIndex = 9;
            this.ShowLogsCheckBox.Text = "Show Logs";
            this.ShowLogsCheckBox.UseVisualStyleBackColor = true;
            // 
            // MyExportLogsButton
            // 
            this.MyExportLogsButton.Location = new System.Drawing.Point(119, 16);
            this.MyExportLogsButton.Name = "MyExportLogsButton";
            this.MyExportLogsButton.Size = new System.Drawing.Size(92, 36);
            this.MyExportLogsButton.TabIndex = 8;
            this.MyExportLogsButton.Text = "Export Logs";
            this.MyExportLogsButton.UseVisualStyleBackColor = true;
            this.MyExportLogsButton.Click += new System.EventHandler(this.MyExportLogsButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Logs Refresh Rate:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MyDumpingCheckBox
            // 
            this.MyDumpingCheckBox.AutoSize = true;
            this.MyDumpingCheckBox.Location = new System.Drawing.Point(9, 81);
            this.MyDumpingCheckBox.Name = "MyDumpingCheckBox";
            this.MyDumpingCheckBox.Size = new System.Drawing.Size(104, 17);
            this.MyDumpingCheckBox.TabIndex = 7;
            this.MyDumpingCheckBox.Text = "Enable Dumping";
            this.MyDumpingCheckBox.UseVisualStyleBackColor = true;
            this.MyDumpingCheckBox.CheckStateChanged += new System.EventHandler(this.MyDumpingCheckBox_CheckStateChanged);
            // 
            // MyNumericUpDown
            // 
            this.MyNumericUpDown.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.MyNumericUpDown.Location = new System.Drawing.Point(9, 32);
            this.MyNumericUpDown.Maximum = new decimal(new int[] {
            3600000,
            0,
            0,
            0});
            this.MyNumericUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.MyNumericUpDown.Name = "MyNumericUpDown";
            this.MyNumericUpDown.Size = new System.Drawing.Size(104, 20);
            this.MyNumericUpDown.TabIndex = 4;
            this.MyNumericUpDown.ThousandsSeparator = true;
            this.MyNumericUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.MyNumericUpDown.ValueChanged += new System.EventHandler(this.MyNumericUpDown_ValueChanged);
            // 
            // MyPictureBox
            // 
            this.MyPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MyPictureBox.Image = global::Sirification.Properties.Resources.Siri_Server;
            this.MyPictureBox.Location = new System.Drawing.Point(198, 128);
            this.MyPictureBox.Name = "MyPictureBox";
            this.MyPictureBox.Size = new System.Drawing.Size(354, 283);
            this.MyPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.MyPictureBox.TabIndex = 6;
            this.MyPictureBox.TabStop = false;
            // 
            // MyLogs_TabPage
            // 
            this.MyLogs_TabPage.Controls.Add(this.MyListBox);
            this.MyLogs_TabPage.Location = new System.Drawing.Point(4, 22);
            this.MyLogs_TabPage.Name = "MyLogs_TabPage";
            this.MyLogs_TabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MyLogs_TabPage.Size = new System.Drawing.Size(760, 516);
            this.MyLogs_TabPage.TabIndex = 1;
            this.MyLogs_TabPage.Text = "Server Logs";
            this.MyLogs_TabPage.UseVisualStyleBackColor = true;
            // 
            // MyLogInspector_TabPage
            // 
            this.MyLogInspector_TabPage.Controls.Add(this.MyTextBox);
            this.MyLogInspector_TabPage.Location = new System.Drawing.Point(4, 22);
            this.MyLogInspector_TabPage.Name = "MyLogInspector_TabPage";
            this.MyLogInspector_TabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MyLogInspector_TabPage.Size = new System.Drawing.Size(760, 516);
            this.MyLogInspector_TabPage.TabIndex = 2;
            this.MyLogInspector_TabPage.Text = "Log Inspector";
            this.MyLogInspector_TabPage.UseVisualStyleBackColor = true;
            // 
            // DatabaseTabPage
            // 
            this.DatabaseTabPage.Controls.Add(this.ExecuteButton);
            this.DatabaseTabPage.Controls.Add(this.MyDataGridView);
            this.DatabaseTabPage.Controls.Add(this.SQLTextBox);
            this.DatabaseTabPage.Location = new System.Drawing.Point(4, 22);
            this.DatabaseTabPage.Name = "DatabaseTabPage";
            this.DatabaseTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.DatabaseTabPage.Size = new System.Drawing.Size(760, 516);
            this.DatabaseTabPage.TabIndex = 3;
            this.DatabaseTabPage.Text = "Database";
            this.DatabaseTabPage.UseVisualStyleBackColor = true;
            // 
            // ExecuteButton
            // 
            this.ExecuteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExecuteButton.Location = new System.Drawing.Point(646, 6);
            this.ExecuteButton.Name = "ExecuteButton";
            this.ExecuteButton.Size = new System.Drawing.Size(108, 80);
            this.ExecuteButton.TabIndex = 2;
            this.ExecuteButton.Text = "Execute SQL";
            this.ExecuteButton.UseVisualStyleBackColor = true;
            this.ExecuteButton.Click += new System.EventHandler(this.ExecuteButton_Click);
            // 
            // MyDataGridView
            // 
            this.MyDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MyDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MyDataGridView.Location = new System.Drawing.Point(6, 92);
            this.MyDataGridView.Name = "MyDataGridView";
            this.MyDataGridView.Size = new System.Drawing.Size(748, 418);
            this.MyDataGridView.TabIndex = 1;
            // 
            // SQLTextBox
            // 
            this.SQLTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SQLTextBox.Location = new System.Drawing.Point(6, 6);
            this.SQLTextBox.Multiline = true;
            this.SQLTextBox.Name = "SQLTextBox";
            this.SQLTextBox.Size = new System.Drawing.Size(634, 80);
            this.SQLTextBox.TabIndex = 0;
            // 
            // MyTimer
            // 
            this.MyTimer.SynchronizingObject = this;
            this.MyTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.MyTimer_Elapsed);
            // 
            // InstallCAButton
            // 
            this.InstallCAButton.Location = new System.Drawing.Point(217, 16);
            this.InstallCAButton.Name = "InstallCAButton";
            this.InstallCAButton.Size = new System.Drawing.Size(88, 36);
            this.InstallCAButton.TabIndex = 11;
            this.InstallCAButton.Text = "Install Texnomic CA";
            this.InstallCAButton.UseVisualStyleBackColor = true;
            this.InstallCAButton.Click += new System.EventHandler(this.InstallCAButton_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.MyTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Sirification";
            this.MyTabControl.ResumeLayout(false);
            this.MyControlPanel_TabPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MyNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MyPictureBox)).EndInit();
            this.MyLogs_TabPage.ResumeLayout(false);
            this.MyLogInspector_TabPage.ResumeLayout(false);
            this.MyLogInspector_TabPage.PerformLayout();
            this.DatabaseTabPage.ResumeLayout(false);
            this.DatabaseTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MyDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MyTimer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox MyListBox;
        private System.Windows.Forms.Button Start_Button;
        private System.Windows.Forms.TextBox MyTextBox;
        private System.Windows.Forms.Button Stop_Button;
        private System.Windows.Forms.TabControl MyTabControl;
        private System.Windows.Forms.TabPage MyControlPanel_TabPage;
        private System.Windows.Forms.TabPage MyLogs_TabPage;
        private System.Windows.Forms.TabPage MyLogInspector_TabPage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown MyNumericUpDown;
        private System.Timers.Timer MyTimer;
        private System.Windows.Forms.PictureBox MyPictureBox;
        private System.Windows.Forms.CheckBox MyDumpingCheckBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button MyExportLogsButton;
        private System.Windows.Forms.CheckBox ShowLogsCheckBox;
        private System.Windows.Forms.TabPage DatabaseTabPage;
        private System.Windows.Forms.Button ExecuteButton;
        private System.Windows.Forms.DataGridView MyDataGridView;
        private System.Windows.Forms.TextBox SQLTextBox;
        private System.Windows.Forms.Button ClearLogButton;
        private System.Windows.Forms.Button InstallCAButton;
    }
}


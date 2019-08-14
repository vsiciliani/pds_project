﻿using System.Windows.Forms;

namespace SnifferProbeRequestApp   {
    public partial class frmMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.btnSalvaDevice = new System.Windows.Forms.PictureBox();
            this.separatoreSettingRigth = new System.Windows.Forms.Label();
            this.btnIdentificaDevice = new System.Windows.Forms.PictureBox();
            this.separatorSettingLeft = new System.Windows.Forms.Label();
            this.txtYPosition = new System.Windows.Forms.TextBox();
            this.txtXPosition = new System.Windows.Forms.TextBox();
            this.lblYPosition = new System.Windows.Forms.Label();
            this.lblXPosition = new System.Windows.Forms.Label();
            this.lblNoDeviceNoConf = new System.Windows.Forms.Label();
            this.lstBoxNoConfDevice = new System.Windows.Forms.ListBox();
            this.lblConfiguraDevice = new System.Windows.Forms.Label();
            this.tabDeviceConf = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnEliminaDevice = new System.Windows.Forms.PictureBox();
            this.btnSalvaModificaDevice = new System.Windows.Forms.PictureBox();
            this.lblModPosizione = new System.Windows.Forms.Label();
            this.lblPosizioneValue = new System.Windows.Forms.Label();
            this.lblIndirizzoIpValue = new System.Windows.Forms.Label();
            this.txtY = new System.Windows.Forms.TextBox();
            this.lblPosizioneDevice = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblIpDeviceConf = new System.Windows.Forms.Label();
            this.txtX = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblNoDeviceConf = new System.Windows.Forms.Label();
            this.lblElencoDeviceConf = new System.Windows.Forms.Label();
            this.lblNumDeviceNonConf = new System.Windows.Forms.Label();
            this.lblNumDeviceConf = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chartNumberDevice = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartPositionDevice = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timerUpdateChartNumberDevice = new System.Windows.Forms.Timer(this.components);
            this.lblProgetto = new System.Windows.Forms.Label();
            this.lblAnno = new System.Windows.Forms.Label();
            this.lblNomeCognome = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tabFeatures = new System.Windows.Forms.TabControl();
            this.tabConteggioDevice = new System.Windows.Forms.TabPage();
            this.lblInfoConteggioDevice = new System.Windows.Forms.Label();
            this.tabPosizioneDevice = new System.Windows.Forms.TabPage();
            this.lblInfoPosizioneDevice = new System.Windows.Forms.Label();
            this.tabStatisticaLungoPeriodo = new System.Windows.Forms.TabPage();
            this.btn7giorni = new System.Windows.Forms.Button();
            this.btn1giorno = new System.Windows.Forms.Button();
            this.btn12Ore = new System.Windows.Forms.Button();
            this.btn6Ore = new System.Windows.Forms.Button();
            this.btn1Ora = new System.Windows.Forms.Button();
            this.btnOra = new System.Windows.Forms.Button();
            this.btnCercaStatisticheLungoPeriodo = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerLimite = new System.Windows.Forms.DateTimePicker();
            this.upDownNumDevice = new System.Windows.Forms.NumericUpDown();
            this.chartStatisticaLungoPeriodo = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.toolTipApp = new System.Windows.Forms.ToolTip(this.components);
            this.btnRefresh = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblMin2device = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.imgPolito = new System.Windows.Forms.PictureBox();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSalvaDevice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnIdentificaDevice)).BeginInit();
            this.tabDeviceConf.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnEliminaDevice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSalvaModificaDevice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartNumberDevice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPositionDevice)).BeginInit();
            this.tabFeatures.SuspendLayout();
            this.tabConteggioDevice.SuspendLayout();
            this.tabPosizioneDevice.SuspendLayout();
            this.tabStatisticaLungoPeriodo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCercaStatisticheLungoPeriodo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownNumDevice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartStatisticaLungoPeriodo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRefresh)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPolito)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.BackColor = System.Drawing.SystemColors.GrayText;
            this.groupBoxSettings.Controls.Add(this.btnSalvaDevice);
            this.groupBoxSettings.Controls.Add(this.separatoreSettingRigth);
            this.groupBoxSettings.Controls.Add(this.btnIdentificaDevice);
            this.groupBoxSettings.Controls.Add(this.separatorSettingLeft);
            this.groupBoxSettings.Controls.Add(this.txtYPosition);
            this.groupBoxSettings.Controls.Add(this.txtXPosition);
            this.groupBoxSettings.Controls.Add(this.lblYPosition);
            this.groupBoxSettings.Controls.Add(this.lblXPosition);
            this.groupBoxSettings.Controls.Add(this.lblNoDeviceNoConf);
            this.groupBoxSettings.Controls.Add(this.lstBoxNoConfDevice);
            this.groupBoxSettings.Controls.Add(this.lblConfiguraDevice);
            this.groupBoxSettings.Controls.Add(this.tabDeviceConf);
            this.groupBoxSettings.Controls.Add(this.lblNoDeviceConf);
            this.groupBoxSettings.Controls.Add(this.lblElencoDeviceConf);
            this.groupBoxSettings.Controls.Add(this.lblNumDeviceNonConf);
            this.groupBoxSettings.Controls.Add(this.lblNumDeviceConf);
            this.groupBoxSettings.Font = new System.Drawing.Font("Calibri", 11F);
            this.groupBoxSettings.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 137);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(1059, 133);
            this.groupBoxSettings.TabIndex = 0;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // btnSalvaDevice
            // 
            this.btnSalvaDevice.Image = global::SnifferProbeRequestApp.Properties.Resources.plus;
            this.btnSalvaDevice.Location = new System.Drawing.Point(937, 38);
            this.btnSalvaDevice.Name = "btnSalvaDevice";
            this.btnSalvaDevice.Size = new System.Drawing.Size(25, 25);
            this.btnSalvaDevice.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnSalvaDevice.TabIndex = 9;
            this.btnSalvaDevice.TabStop = false;
            this.toolTipApp.SetToolTip(this.btnSalvaDevice, "Salva configurazione rilevatore");
            this.btnSalvaDevice.Visible = false;
            this.btnSalvaDevice.Click += new System.EventHandler(this.btnSalvaDevice_Click);
            // 
            // separatoreSettingRigth
            // 
            this.separatoreSettingRigth.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.separatoreSettingRigth.Location = new System.Drawing.Point(740, 14);
            this.separatoreSettingRigth.Name = "separatoreSettingRigth";
            this.separatoreSettingRigth.Size = new System.Drawing.Size(2, 115);
            this.separatoreSettingRigth.TabIndex = 13;
            // 
            // btnIdentificaDevice
            // 
            this.btnIdentificaDevice.Image = global::SnifferProbeRequestApp.Properties.Resources.placeholder;
            this.btnIdentificaDevice.Location = new System.Drawing.Point(901, 38);
            this.btnIdentificaDevice.Name = "btnIdentificaDevice";
            this.btnIdentificaDevice.Size = new System.Drawing.Size(25, 25);
            this.btnIdentificaDevice.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnIdentificaDevice.TabIndex = 12;
            this.btnIdentificaDevice.TabStop = false;
            this.toolTipApp.SetToolTip(this.btnIdentificaDevice, "Identifica device\r\n(il device selezionato inizierà a lampeggiare per 30 secondi)");
            this.btnIdentificaDevice.Visible = false;
            this.btnIdentificaDevice.Click += new System.EventHandler(this.btnIdentificaDevice_Click);
            // 
            // separatorSettingLeft
            // 
            this.separatorSettingLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.separatorSettingLeft.Location = new System.Drawing.Point(241, 14);
            this.separatorSettingLeft.Name = "separatorSettingLeft";
            this.separatorSettingLeft.Size = new System.Drawing.Size(2, 115);
            this.separatorSettingLeft.TabIndex = 12;
            // 
            // txtYPosition
            // 
            this.txtYPosition.Location = new System.Drawing.Point(996, 97);
            this.txtYPosition.Name = "txtYPosition";
            this.txtYPosition.Size = new System.Drawing.Size(53, 25);
            this.txtYPosition.TabIndex = 10;
            this.txtYPosition.Visible = false;
            // 
            // txtXPosition
            // 
            this.txtXPosition.Location = new System.Drawing.Point(996, 70);
            this.txtXPosition.Name = "txtXPosition";
            this.txtXPosition.Size = new System.Drawing.Size(53, 25);
            this.txtXPosition.TabIndex = 9;
            this.txtXPosition.Visible = false;
            // 
            // lblYPosition
            // 
            this.lblYPosition.AutoSize = true;
            this.lblYPosition.Location = new System.Drawing.Point(901, 100);
            this.lblYPosition.Name = "lblYPosition";
            this.lblYPosition.Size = new System.Drawing.Size(89, 18);
            this.lblYPosition.TabIndex = 7;
            this.lblYPosition.Text = "Coordinata Y:";
            this.lblYPosition.Visible = false;
            // 
            // lblXPosition
            // 
            this.lblXPosition.AutoSize = true;
            this.lblXPosition.Location = new System.Drawing.Point(901, 73);
            this.lblXPosition.Name = "lblXPosition";
            this.lblXPosition.Size = new System.Drawing.Size(91, 18);
            this.lblXPosition.TabIndex = 6;
            this.lblXPosition.Text = "Coordinata X:";
            this.lblXPosition.Visible = false;
            // 
            // lblNoDeviceNoConf
            // 
            this.lblNoDeviceNoConf.AutoSize = true;
            this.lblNoDeviceNoConf.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Italic);
            this.lblNoDeviceNoConf.Location = new System.Drawing.Point(750, 36);
            this.lblNoDeviceNoConf.Name = "lblNoDeviceNoConf";
            this.lblNoDeviceNoConf.Size = new System.Drawing.Size(284, 18);
            this.lblNoDeviceNoConf.TabIndex = 1;
            this.lblNoDeviceNoConf.Text = "Non ci sono rilevatori connessi da configurare";
            // 
            // lstBoxNoConfDevice
            // 
            this.lstBoxNoConfDevice.BackColor = System.Drawing.SystemColors.Window;
            this.lstBoxNoConfDevice.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lstBoxNoConfDevice.FormattingEnabled = true;
            this.lstBoxNoConfDevice.ItemHeight = 18;
            this.lstBoxNoConfDevice.Location = new System.Drawing.Point(750, 36);
            this.lstBoxNoConfDevice.Name = "lstBoxNoConfDevice";
            this.lstBoxNoConfDevice.Size = new System.Drawing.Size(145, 94);
            this.lstBoxNoConfDevice.TabIndex = 5;
            this.lstBoxNoConfDevice.Visible = false;
            // 
            // lblConfiguraDevice
            // 
            this.lblConfiguraDevice.AutoSize = true;
            this.lblConfiguraDevice.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);
            this.lblConfiguraDevice.Location = new System.Drawing.Point(748, 17);
            this.lblConfiguraDevice.Name = "lblConfiguraDevice";
            this.lblConfiguraDevice.Size = new System.Drawing.Size(177, 18);
            this.lblConfiguraDevice.TabIndex = 4;
            this.lblConfiguraDevice.Text = "Configura nuovo rilevatore:";
            // 
            // tabDeviceConf
            // 
            this.tabDeviceConf.Controls.Add(this.tabPage1);
            this.tabDeviceConf.Controls.Add(this.tabPage2);
            this.tabDeviceConf.Location = new System.Drawing.Point(255, 36);
            this.tabDeviceConf.Name = "tabDeviceConf";
            this.tabDeviceConf.SelectedIndex = 0;
            this.tabDeviceConf.Size = new System.Drawing.Size(475, 91);
            this.tabDeviceConf.TabIndex = 3;
            this.tabDeviceConf.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnEliminaDevice);
            this.tabPage1.Controls.Add(this.btnSalvaModificaDevice);
            this.tabPage1.Controls.Add(this.lblModPosizione);
            this.tabPage1.Controls.Add(this.lblPosizioneValue);
            this.tabPage1.Controls.Add(this.lblIndirizzoIpValue);
            this.tabPage1.Controls.Add(this.txtY);
            this.tabPage1.Controls.Add(this.lblPosizioneDevice);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.lblIpDeviceConf);
            this.tabPage1.Controls.Add(this.txtX);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 27);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(467, 60);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnEliminaDevice
            // 
            this.btnEliminaDevice.Image = global::SnifferProbeRequestApp.Properties.Resources.error;
            this.btnEliminaDevice.Location = new System.Drawing.Point(436, 23);
            this.btnEliminaDevice.Name = "btnEliminaDevice";
            this.btnEliminaDevice.Size = new System.Drawing.Size(25, 25);
            this.btnEliminaDevice.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnEliminaDevice.TabIndex = 10;
            this.btnEliminaDevice.TabStop = false;
            this.toolTipApp.SetToolTip(this.btnEliminaDevice, "Elimina rilevatore");
            // 
            // btnSalvaModificaDevice
            // 
            this.btnSalvaModificaDevice.Image = global::SnifferProbeRequestApp.Properties.Resources.success;
            this.btnSalvaModificaDevice.Location = new System.Drawing.Point(405, 23);
            this.btnSalvaModificaDevice.Name = "btnSalvaModificaDevice";
            this.btnSalvaModificaDevice.Size = new System.Drawing.Size(25, 25);
            this.btnSalvaModificaDevice.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnSalvaModificaDevice.TabIndex = 13;
            this.btnSalvaModificaDevice.TabStop = false;
            this.toolTipApp.SetToolTip(this.btnSalvaModificaDevice, "Salva nuova posizione del rilevatore");
            // 
            // lblModPosizione
            // 
            this.lblModPosizione.AutoSize = true;
            this.lblModPosizione.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblModPosizione.Location = new System.Drawing.Point(238, 4);
            this.lblModPosizione.Name = "lblModPosizione";
            this.lblModPosizione.Size = new System.Drawing.Size(128, 18);
            this.lblModPosizione.TabIndex = 11;
            this.lblModPosizione.Text = "Modifica Posizione:";
            // 
            // lblPosizioneValue
            // 
            this.lblPosizioneValue.AutoSize = true;
            this.lblPosizioneValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPosizioneValue.Location = new System.Drawing.Point(108, 27);
            this.lblPosizioneValue.Name = "lblPosizioneValue";
            this.lblPosizioneValue.Size = new System.Drawing.Size(46, 18);
            this.lblPosizioneValue.TabIndex = 10;
            this.lblPosizioneValue.Text = "label5";
            // 
            // lblIndirizzoIpValue
            // 
            this.lblIndirizzoIpValue.AutoSize = true;
            this.lblIndirizzoIpValue.Font = new System.Drawing.Font("Calibri", 11F);
            this.lblIndirizzoIpValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblIndirizzoIpValue.Location = new System.Drawing.Point(7, 27);
            this.lblIndirizzoIpValue.Name = "lblIndirizzoIpValue";
            this.lblIndirizzoIpValue.Size = new System.Drawing.Size(46, 18);
            this.lblIndirizzoIpValue.TabIndex = 9;
            this.lblIndirizzoIpValue.Text = "label5";
            // 
            // txtY
            // 
            this.txtY.Location = new System.Drawing.Point(341, 23);
            this.txtY.Name = "txtY";
            this.txtY.Size = new System.Drawing.Size(57, 25);
            this.txtY.TabIndex = 6;
            // 
            // lblPosizioneDevice
            // 
            this.lblPosizioneDevice.AutoSize = true;
            this.lblPosizioneDevice.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPosizioneDevice.Location = new System.Drawing.Point(108, 4);
            this.lblPosizioneDevice.Name = "lblPosizioneDevice";
            this.lblPosizioneDevice.Size = new System.Drawing.Size(119, 18);
            this.lblPosizioneDevice.TabIndex = 1;
            this.lblPosizioneDevice.Text = "Posizione attuale:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(321, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "Y:";
            // 
            // lblIpDeviceConf
            // 
            this.lblIpDeviceConf.AutoSize = true;
            this.lblIpDeviceConf.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblIpDeviceConf.Location = new System.Drawing.Point(7, 4);
            this.lblIpDeviceConf.Name = "lblIpDeviceConf";
            this.lblIpDeviceConf.Size = new System.Drawing.Size(80, 18);
            this.lblIpDeviceConf.TabIndex = 0;
            this.lblIpDeviceConf.Text = "Indirizzo IP:";
            // 
            // txtX
            // 
            this.txtX.Font = new System.Drawing.Font("Calibri", 11F);
            this.txtX.Location = new System.Drawing.Point(260, 23);
            this.txtX.Name = "txtX";
            this.txtX.Size = new System.Drawing.Size(57, 25);
            this.txtX.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(238, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "X:";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 27);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(467, 60);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lblNoDeviceConf
            // 
            this.lblNoDeviceConf.AutoSize = true;
            this.lblNoDeviceConf.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Italic);
            this.lblNoDeviceConf.Location = new System.Drawing.Point(252, 39);
            this.lblNoDeviceConf.Name = "lblNoDeviceConf";
            this.lblNoDeviceConf.Size = new System.Drawing.Size(190, 18);
            this.lblNoDeviceConf.TabIndex = 1;
            this.lblNoDeviceConf.Text = "Non ci sono device configurati";
            // 
            // lblElencoDeviceConf
            // 
            this.lblElencoDeviceConf.AutoSize = true;
            this.lblElencoDeviceConf.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);
            this.lblElencoDeviceConf.Location = new System.Drawing.Point(249, 17);
            this.lblElencoDeviceConf.Name = "lblElencoDeviceConf";
            this.lblElencoDeviceConf.Size = new System.Drawing.Size(182, 18);
            this.lblElencoDeviceConf.TabIndex = 2;
            this.lblElencoDeviceConf.Text = "Elenco rilevatori configurati:";
            // 
            // lblNumDeviceNonConf
            // 
            this.lblNumDeviceNonConf.AutoSize = true;
            this.lblNumDeviceNonConf.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);
            this.lblNumDeviceNonConf.Location = new System.Drawing.Point(4, 42);
            this.lblNumDeviceNonConf.Name = "lblNumDeviceNonConf";
            this.lblNumDeviceNonConf.Size = new System.Drawing.Size(232, 18);
            this.lblNumDeviceNonConf.TabIndex = 1;
            this.lblNumDeviceNonConf.Text = "Numero rilevatori non configurati:  0";
            // 
            // lblNumDeviceConf
            // 
            this.lblNumDeviceConf.AutoSize = true;
            this.lblNumDeviceConf.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);
            this.lblNumDeviceConf.Location = new System.Drawing.Point(6, 18);
            this.lblNumDeviceConf.Name = "lblNumDeviceConf";
            this.lblNumDeviceConf.Size = new System.Drawing.Size(205, 18);
            this.lblNumDeviceConf.TabIndex = 0;
            this.lblNumDeviceConf.Text = "Numero rilevatori configurati:  0";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // chartNumberDevice
            // 
            this.chartNumberDevice.BorderlineWidth = 0;
            this.chartNumberDevice.CausesValidation = false;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.MajorGrid.LineWidth = 2;
            chartArea1.Name = "ChartArea1";
            this.chartNumberDevice.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartNumberDevice.Legends.Add(legend1);
            this.chartNumberDevice.Location = new System.Drawing.Point(6, 24);
            this.chartNumberDevice.Name = "chartNumberDevice";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
            series1.IsVisibleInLegend = false;
            series1.IsXValueIndexed = true;
            series1.Legend = "Legend1";
            series1.Name = "N. dispositivi univoci";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int64;
            this.chartNumberDevice.Series.Add(series1);
            this.chartNumberDevice.Size = new System.Drawing.Size(1017, 366);
            this.chartNumberDevice.TabIndex = 0;
            this.chartNumberDevice.Text = "chartNumberDevice";
            // 
            // chartPositionDevice
            // 
            chartArea2.AxisX.LabelStyle.Enabled = false;
            chartArea2.AxisX.LineWidth = 0;
            chartArea2.AxisX.MajorGrid.Enabled = false;
            chartArea2.AxisX.MajorTickMark.LineWidth = 0;
            chartArea2.AxisY.LabelStyle.Enabled = false;
            chartArea2.AxisY.LineWidth = 0;
            chartArea2.AxisY.MajorGrid.Enabled = false;
            chartArea2.AxisY.MajorTickMark.LineWidth = 0;
            chartArea2.Name = "ChartArea1";
            this.chartPositionDevice.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartPositionDevice.Legends.Add(legend2);
            this.chartPositionDevice.Location = new System.Drawing.Point(6, 32);
            this.chartPositionDevice.Name = "chartPositionDevice";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series2.Legend = "Legend1";
            series2.MarkerSize = 12;
            series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Square;
            series2.Name = "ESP";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series3.Legend = "Legend1";
            series3.MarkerColor = System.Drawing.Color.Red;
            series3.MarkerSize = 15;
            series3.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Cross;
            series3.Name = "Device";
            this.chartPositionDevice.Series.Add(series2);
            this.chartPositionDevice.Series.Add(series3);
            this.chartPositionDevice.Size = new System.Drawing.Size(1004, 358);
            this.chartPositionDevice.TabIndex = 0;
            this.chartPositionDevice.Text = "chart1";
            // 
            // timerUpdateChartNumberDevice
            // 
            this.timerUpdateChartNumberDevice.Enabled = true;
            this.timerUpdateChartNumberDevice.Interval = 60000;
            this.timerUpdateChartNumberDevice.Tick += new System.EventHandler(this.timerUpdateChartNumberDevice_Tick);
            // 
            // lblProgetto
            // 
            this.lblProgetto.AutoSize = true;
            this.lblProgetto.Font = new System.Drawing.Font("Verdana", 11F, System.Drawing.FontStyle.Bold);
            this.lblProgetto.Location = new System.Drawing.Point(276, 55);
            this.lblProgetto.Name = "lblProgetto";
            this.lblProgetto.Size = new System.Drawing.Size(335, 18);
            this.lblProgetto.TabIndex = 3;
            this.lblProgetto.Text = "Progetto di Programmazione di Sistema";
            // 
            // lblAnno
            // 
            this.lblAnno.AutoSize = true;
            this.lblAnno.Font = new System.Drawing.Font("Verdana", 11F, System.Drawing.FontStyle.Bold);
            this.lblAnno.Location = new System.Drawing.Point(331, 83);
            this.lblAnno.Name = "lblAnno";
            this.lblAnno.Size = new System.Drawing.Size(232, 18);
            this.lblAnno.TabIndex = 4;
            this.lblAnno.Text = "Anno Accademico 2017/18";
            // 
            // lblNomeCognome
            // 
            this.lblNomeCognome.AutoSize = true;
            this.lblNomeCognome.Font = new System.Drawing.Font("Verdana", 11F, System.Drawing.FontStyle.Bold);
            this.lblNomeCognome.Location = new System.Drawing.Point(334, 111);
            this.lblNomeCognome.Name = "lblNomeCognome";
            this.lblNomeCognome.Size = new System.Drawing.Size(228, 18);
            this.lblNomeCognome.TabIndex = 5;
            this.lblNomeCognome.Text = "Vincenzo Siciliani #243178";
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(192, 21);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(524, 34);
            this.lblTitle.TabIndex = 7;
            this.lblTitle.Text = "SISTEMA RILEVAZIONE PRESENZE INDOOR";
            // 
            // tabFeatures
            // 
            this.tabFeatures.Controls.Add(this.tabConteggioDevice);
            this.tabFeatures.Controls.Add(this.tabPosizioneDevice);
            this.tabFeatures.Controls.Add(this.tabStatisticaLungoPeriodo);
            this.tabFeatures.Location = new System.Drawing.Point(9, 54);
            this.tabFeatures.Name = "tabFeatures";
            this.tabFeatures.SelectedIndex = 0;
            this.tabFeatures.Size = new System.Drawing.Size(1040, 427);
            this.tabFeatures.TabIndex = 8;
            this.tabFeatures.Visible = false;
            // 
            // tabConteggioDevice
            // 
            this.tabConteggioDevice.Controls.Add(this.lblInfoConteggioDevice);
            this.tabConteggioDevice.Controls.Add(this.chartNumberDevice);
            this.tabConteggioDevice.Location = new System.Drawing.Point(4, 27);
            this.tabConteggioDevice.Name = "tabConteggioDevice";
            this.tabConteggioDevice.Padding = new System.Windows.Forms.Padding(3);
            this.tabConteggioDevice.Size = new System.Drawing.Size(1032, 396);
            this.tabConteggioDevice.TabIndex = 0;
            this.tabConteggioDevice.Text = "Conteggio Device";
            this.tabConteggioDevice.UseVisualStyleBackColor = true;
            // 
            // lblInfoConteggioDevice
            // 
            this.lblInfoConteggioDevice.AutoSize = true;
            this.lblInfoConteggioDevice.ForeColor = System.Drawing.Color.Black;
            this.lblInfoConteggioDevice.Location = new System.Drawing.Point(6, 6);
            this.lblInfoConteggioDevice.Name = "lblInfoConteggioDevice";
            this.lblInfoConteggioDevice.Size = new System.Drawing.Size(565, 18);
            this.lblInfoConteggioDevice.TabIndex = 1;
            this.lblInfoConteggioDevice.Text = "Questo grafico mostra il numero di device connessi continuativamente per almero 5" +
    " minuti";
            // 
            // tabPosizioneDevice
            // 
            this.tabPosizioneDevice.Controls.Add(this.lblInfoPosizioneDevice);
            this.tabPosizioneDevice.Controls.Add(this.chartPositionDevice);
            this.tabPosizioneDevice.Location = new System.Drawing.Point(4, 27);
            this.tabPosizioneDevice.Name = "tabPosizioneDevice";
            this.tabPosizioneDevice.Padding = new System.Windows.Forms.Padding(3);
            this.tabPosizioneDevice.Size = new System.Drawing.Size(1032, 396);
            this.tabPosizioneDevice.TabIndex = 1;
            this.tabPosizioneDevice.Text = "Posizione Device";
            this.tabPosizioneDevice.UseVisualStyleBackColor = true;
            // 
            // lblInfoPosizioneDevice
            // 
            this.lblInfoPosizioneDevice.AutoSize = true;
            this.lblInfoPosizioneDevice.ForeColor = System.Drawing.Color.Black;
            this.lblInfoPosizioneDevice.Location = new System.Drawing.Point(6, 7);
            this.lblInfoPosizioneDevice.Name = "lblInfoPosizioneDevice";
            this.lblInfoPosizioneDevice.Size = new System.Drawing.Size(674, 18);
            this.lblInfoPosizioneDevice.TabIndex = 2;
            this.lblInfoPosizioneDevice.Text = "Questo grafico mostra la posizione dei device rilevati nell\'ultimo minuto rispett" +
    "o alla posizione dei rilevatori";
            // 
            // tabStatisticaLungoPeriodo
            // 
            this.tabStatisticaLungoPeriodo.Controls.Add(this.btn7giorni);
            this.tabStatisticaLungoPeriodo.Controls.Add(this.btn1giorno);
            this.tabStatisticaLungoPeriodo.Controls.Add(this.btn12Ore);
            this.tabStatisticaLungoPeriodo.Controls.Add(this.btn6Ore);
            this.tabStatisticaLungoPeriodo.Controls.Add(this.btn1Ora);
            this.tabStatisticaLungoPeriodo.Controls.Add(this.btnOra);
            this.tabStatisticaLungoPeriodo.Controls.Add(this.btnCercaStatisticheLungoPeriodo);
            this.tabStatisticaLungoPeriodo.Controls.Add(this.label4);
            this.tabStatisticaLungoPeriodo.Controls.Add(this.label1);
            this.tabStatisticaLungoPeriodo.Controls.Add(this.dateTimePickerLimite);
            this.tabStatisticaLungoPeriodo.Controls.Add(this.upDownNumDevice);
            this.tabStatisticaLungoPeriodo.Controls.Add(this.chartStatisticaLungoPeriodo);
            this.tabStatisticaLungoPeriodo.Location = new System.Drawing.Point(4, 27);
            this.tabStatisticaLungoPeriodo.Name = "tabStatisticaLungoPeriodo";
            this.tabStatisticaLungoPeriodo.Size = new System.Drawing.Size(1032, 396);
            this.tabStatisticaLungoPeriodo.TabIndex = 2;
            this.tabStatisticaLungoPeriodo.Text = "Statistica Lungo Periodo";
            this.tabStatisticaLungoPeriodo.UseVisualStyleBackColor = true;
            // 
            // btn7giorni
            // 
            this.btn7giorni.ForeColor = System.Drawing.Color.Black;
            this.btn7giorni.Location = new System.Drawing.Point(361, 59);
            this.btn7giorni.Name = "btn7giorni";
            this.btn7giorni.Size = new System.Drawing.Size(112, 25);
            this.btn7giorni.TabIndex = 13;
            this.btn7giorni.Text = "7 giorni";
            this.btn7giorni.UseVisualStyleBackColor = true;
            this.btn7giorni.Click += new System.EventHandler(this.Btn7giorni_Click);
            // 
            // btn1giorno
            // 
            this.btn1giorno.ForeColor = System.Drawing.Color.Black;
            this.btn1giorno.Location = new System.Drawing.Point(249, 59);
            this.btn1giorno.Name = "btn1giorno";
            this.btn1giorno.Size = new System.Drawing.Size(112, 25);
            this.btn1giorno.TabIndex = 11;
            this.btn1giorno.Text = "1 giorno";
            this.btn1giorno.UseVisualStyleBackColor = true;
            this.btn1giorno.Click += new System.EventHandler(this.Btn1giorno_Click);
            // 
            // btn12Ore
            // 
            this.btn12Ore.ForeColor = System.Drawing.Color.Black;
            this.btn12Ore.Location = new System.Drawing.Point(417, 35);
            this.btn12Ore.Name = "btn12Ore";
            this.btn12Ore.Size = new System.Drawing.Size(56, 25);
            this.btn12Ore.TabIndex = 10;
            this.btn12Ore.Text = "12 ore";
            this.btn12Ore.UseVisualStyleBackColor = true;
            this.btn12Ore.Click += new System.EventHandler(this.Btn12Ore_Click);
            // 
            // btn6Ore
            // 
            this.btn6Ore.ForeColor = System.Drawing.Color.Black;
            this.btn6Ore.Location = new System.Drawing.Point(361, 35);
            this.btn6Ore.Name = "btn6Ore";
            this.btn6Ore.Size = new System.Drawing.Size(56, 25);
            this.btn6Ore.TabIndex = 9;
            this.btn6Ore.Text = "6 ore";
            this.btn6Ore.UseVisualStyleBackColor = true;
            this.btn6Ore.Click += new System.EventHandler(this.Btn6Ore_Click);
            // 
            // btn1Ora
            // 
            this.btn1Ora.ForeColor = System.Drawing.Color.Black;
            this.btn1Ora.Location = new System.Drawing.Point(305, 35);
            this.btn1Ora.Name = "btn1Ora";
            this.btn1Ora.Size = new System.Drawing.Size(56, 25);
            this.btn1Ora.TabIndex = 8;
            this.btn1Ora.Text = "1 ora";
            this.btn1Ora.UseVisualStyleBackColor = true;
            this.btn1Ora.Click += new System.EventHandler(this.Btn1Ora_Click);
            // 
            // btnOra
            // 
            this.btnOra.ForeColor = System.Drawing.Color.Black;
            this.btnOra.Location = new System.Drawing.Point(249, 35);
            this.btnOra.Name = "btnOra";
            this.btnOra.Size = new System.Drawing.Size(56, 25);
            this.btnOra.TabIndex = 7;
            this.btnOra.Text = "Ora";
            this.btnOra.UseVisualStyleBackColor = true;
            this.btnOra.Click += new System.EventHandler(this.BtnOra_Click);
            // 
            // btnCercaStatisticheLungoPeriodo
            // 
            this.btnCercaStatisticheLungoPeriodo.Image = global::SnifferProbeRequestApp.Properties.Resources.search;
            this.btnCercaStatisticheLungoPeriodo.Location = new System.Drawing.Point(481, 9);
            this.btnCercaStatisticheLungoPeriodo.Name = "btnCercaStatisticheLungoPeriodo";
            this.btnCercaStatisticheLungoPeriodo.Size = new System.Drawing.Size(25, 25);
            this.btnCercaStatisticheLungoPeriodo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnCercaStatisticheLungoPeriodo.TabIndex = 6;
            this.btnCercaStatisticheLungoPeriodo.TabStop = false;
            this.btnCercaStatisticheLungoPeriodo.Click += new System.EventHandler(this.BtnCercaStatisticheLungoPeriodo_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(181, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 18);
            this.label4.TabIndex = 5;
            this.label4.Text = "Filtra da:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(7, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "Numero device:";
            // 
            // dateTimePickerLimite
            // 
            this.dateTimePickerLimite.Checked = false;
            this.dateTimePickerLimite.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dateTimePickerLimite.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerLimite.Location = new System.Drawing.Point(249, 8);
            this.dateTimePickerLimite.Name = "dateTimePickerLimite";
            this.dateTimePickerLimite.ShowUpDown = true;
            this.dateTimePickerLimite.Size = new System.Drawing.Size(224, 26);
            this.dateTimePickerLimite.TabIndex = 3;
            // 
            // upDownNumDevice
            // 
            this.upDownNumDevice.Location = new System.Drawing.Point(117, 8);
            this.upDownNumDevice.Name = "upDownNumDevice";
            this.upDownNumDevice.Size = new System.Drawing.Size(56, 26);
            this.upDownNumDevice.TabIndex = 2;
            this.upDownNumDevice.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // chartStatisticaLungoPeriodo
            // 
            chartArea3.Name = "ChartArea1";
            this.chartStatisticaLungoPeriodo.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chartStatisticaLungoPeriodo.Legends.Add(legend3);
            this.chartStatisticaLungoPeriodo.Location = new System.Drawing.Point(4, 77);
            this.chartStatisticaLungoPeriodo.Name = "chartStatisticaLungoPeriodo";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.RangeBar;
            series4.CustomProperties = "DrawSideBySide=False";
            series4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series4.IsVisibleInLegend = false;
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            series4.YValuesPerPoint = 2;
            series4.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            this.chartStatisticaLungoPeriodo.Series.Add(series4);
            this.chartStatisticaLungoPeriodo.Size = new System.Drawing.Size(1025, 314);
            this.chartStatisticaLungoPeriodo.TabIndex = 0;
            this.chartStatisticaLungoPeriodo.Text = "chart1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = global::SnifferProbeRequestApp.Properties.Resources.repeat;
            this.btnRefresh.Location = new System.Drawing.Point(9, 22);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(25, 26);
            this.btnRefresh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.TabStop = false;
            this.toolTipApp.SetToolTip(this.btnRefresh, "Aggiorna i grafici delle rilevazioni");
            this.btnRefresh.Visible = false;
            this.btnRefresh.Click += new System.EventHandler(this.imgRefresh_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.GrayText;
            this.groupBox2.Controls.Add(this.lblMin2device);
            this.groupBox2.Controls.Add(this.btnRefresh);
            this.groupBox2.Controls.Add(this.tabFeatures);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox2.Location = new System.Drawing.Point(12, 276);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1059, 487);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rilevazioni";
            // 
            // lblMin2device
            // 
            this.lblMin2device.AutoSize = true;
            this.lblMin2device.Location = new System.Drawing.Point(9, 22);
            this.lblMin2device.Name = "lblMin2device";
            this.lblMin2device.Size = new System.Drawing.Size(390, 18);
            this.lblMin2device.TabIndex = 9;
            this.lblMin2device.Text = "Configurare almeno due rilevatori per effettuare le rilevazioni";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SnifferProbeRequestApp.Properties.Resources.icon;
            this.pictureBox1.Location = new System.Drawing.Point(18, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(120, 119);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // imgPolito
            // 
            this.imgPolito.BackColor = System.Drawing.Color.Transparent;
            this.imgPolito.Image = global::SnifferProbeRequestApp.Properties.Resources.polito_logo;
            this.imgPolito.Location = new System.Drawing.Point(768, 12);
            this.imgPolito.Name = "imgPolito";
            this.imgPolito.Size = new System.Drawing.Size(280, 122);
            this.imgPolito.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgPolito.TabIndex = 6;
            this.imgPolito.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(1080, 769);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.imgPolito);
            this.Controls.Add(this.lblNomeCognome);
            this.Controls.Add(this.lblAnno);
            this.Controls.Add(this.lblProgetto);
            this.Controls.Add(this.groupBoxSettings);
            this.Name = "frmMain";
            this.Text = "Sistema rilevazione presenze";
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSalvaDevice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnIdentificaDevice)).EndInit();
            this.tabDeviceConf.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnEliminaDevice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSalvaModificaDevice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartNumberDevice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPositionDevice)).EndInit();
            this.tabFeatures.ResumeLayout(false);
            this.tabConteggioDevice.ResumeLayout(false);
            this.tabConteggioDevice.PerformLayout();
            this.tabPosizioneDevice.ResumeLayout(false);
            this.tabPosizioneDevice.PerformLayout();
            this.tabStatisticaLungoPeriodo.ResumeLayout(false);
            this.tabStatisticaLungoPeriodo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCercaStatisticheLungoPeriodo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownNumDevice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartStatisticaLungoPeriodo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRefresh)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPolito)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSettings;
        public System.Windows.Forms.DataVisualization.Charting.Chart chartNumberDevice;
        private System.Windows.Forms.Timer timerUpdateChartNumberDevice;
        private System.Windows.Forms.Label lblProgetto;
        private System.Windows.Forms.Label lblAnno;
        private System.Windows.Forms.Label lblNomeCognome;
        private System.Windows.Forms.PictureBox imgPolito;
        private System.Windows.Forms.PictureBox btnRefresh;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstBoxNoConfDevice;
        private System.Windows.Forms.Label lblConfiguraDevice;
        private System.Windows.Forms.Label lblElencoDeviceConf;
        private System.Windows.Forms.Label lblNumDeviceNonConf;
        private System.Windows.Forms.Label lblNumDeviceConf;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPositionDevice;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblNoDeviceConf;
        private System.Windows.Forms.Label lblNoDeviceNoConf;
        private System.Windows.Forms.TabControl tabDeviceConf;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lblPosizioneDevice;
        private System.Windows.Forms.Label lblIpDeviceConf;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtYPosition;
        private System.Windows.Forms.TextBox txtXPosition;
        private System.Windows.Forms.Label lblYPosition;
        private System.Windows.Forms.Label lblXPosition;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabControl tabFeatures;
        private System.Windows.Forms.TabPage tabConteggioDevice;
        private System.Windows.Forms.TabPage tabPosizioneDevice;
        private System.Windows.Forms.Label separatoreSettingRigth;
        private System.Windows.Forms.Label separatorSettingLeft;
        private System.Windows.Forms.PictureBox btnSalvaDevice;
        private System.Windows.Forms.PictureBox btnIdentificaDevice;
        private System.Windows.Forms.ToolTip toolTipApp;
        private System.Windows.Forms.Label lblModPosizione;
        private System.Windows.Forms.Label lblPosizioneValue;
        private System.Windows.Forms.Label lblIndirizzoIpValue;
        private System.Windows.Forms.PictureBox btnEliminaDevice;
        private System.Windows.Forms.PictureBox btnSalvaModificaDevice;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblInfoConteggioDevice;
        private System.Windows.Forms.Label lblInfoPosizioneDevice;
        private System.Windows.Forms.Label lblMin2device;
        private System.Windows.Forms.TabPage tabStatisticaLungoPeriodo;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartStatisticaLungoPeriodo;
        private System.Windows.Forms.NumericUpDown upDownNumDevice;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerLimite;
        private PictureBox btnCercaStatisticheLungoPeriodo;
        private Button btn7giorni;
        private Button btn1giorno;
        private Button btn12Ore;
        private Button btn6Ore;
        private Button btn1Ora;
        private Button btnOra;
    }
}


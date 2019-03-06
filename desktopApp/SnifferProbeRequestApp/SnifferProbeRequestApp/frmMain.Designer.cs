namespace SnifferProbeRequestApp
{
    public partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnIdentifica = new System.Windows.Forms.Button();
            this.txtYPosition = new System.Windows.Forms.TextBox();
            this.txtXPosition = new System.Windows.Forms.TextBox();
            this.btnConfigura = new System.Windows.Forms.Button();
            this.lblYPosition = new System.Windows.Forms.Label();
            this.lblXPosition = new System.Windows.Forms.Label();
            this.lstBoxNoConfDevice = new System.Windows.Forms.ListBox();
            this.lblConfiguraDevice = new System.Windows.Forms.Label();
            this.tabDeviceConf = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPosizioneDevice = new System.Windows.Forms.Label();
            this.lblIpDeviceConf = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblNoDeviceConf = new System.Windows.Forms.Label();
            this.lblElencoDeviceConf = new System.Windows.Forms.Label();
            this.lblNumDeviceNonConf = new System.Windows.Forms.Label();
            this.lblNumDeviceConf = new System.Windows.Forms.Label();
            this.groupBoxView1 = new System.Windows.Forms.GroupBox();
            this.imgRefresh = new System.Windows.Forms.PictureBox();
            this.chartNumberDevice = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBoxView2 = new System.Windows.Forms.GroupBox();
            this.chartPositionDevice = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timerUpdateChartNumberDevice = new System.Windows.Forms.Timer(this.components);
            this.lblProgetto = new System.Windows.Forms.Label();
            this.lblAnno = new System.Windows.Forms.Label();
            this.lblNomeCognome = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.toolTipRefresh = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabDeviceConf.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBoxView1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartNumberDevice)).BeginInit();
            this.groupBoxView2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPositionDevice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.groupBox1);
            this.groupBoxSettings.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.groupBoxSettings.Location = new System.Drawing.Point(13, 42);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(332, 405);
            this.groupBoxSettings.TabIndex = 0;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnIdentifica);
            this.groupBox1.Controls.Add(this.txtYPosition);
            this.groupBox1.Controls.Add(this.txtXPosition);
            this.groupBox1.Controls.Add(this.btnConfigura);
            this.groupBox1.Controls.Add(this.lblYPosition);
            this.groupBox1.Controls.Add(this.lblXPosition);
            this.groupBox1.Controls.Add(this.lstBoxNoConfDevice);
            this.groupBox1.Controls.Add(this.lblConfiguraDevice);
            this.groupBox1.Controls.Add(this.tabDeviceConf);
            this.groupBox1.Controls.Add(this.lblNoDeviceConf);
            this.groupBox1.Controls.Add(this.lblElencoDeviceConf);
            this.groupBox1.Controls.Add(this.lblNumDeviceNonConf);
            this.groupBox1.Controls.Add(this.lblNumDeviceConf);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 11F);
            this.groupBox1.Location = new System.Drawing.Point(7, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(319, 379);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Wifi Devices";
            // 
            // btnIdentifica
            // 
            this.btnIdentifica.Location = new System.Drawing.Point(161, 339);
            this.btnIdentifica.Name = "btnIdentifica";
            this.btnIdentifica.Size = new System.Drawing.Size(73, 28);
            this.btnIdentifica.TabIndex = 11;
            this.btnIdentifica.Text = "Identifica";
            this.btnIdentifica.UseVisualStyleBackColor = true;
            // 
            // txtYPosition
            // 
            this.txtYPosition.Location = new System.Drawing.Point(250, 307);
            this.txtYPosition.Name = "txtYPosition";
            this.txtYPosition.Size = new System.Drawing.Size(62, 25);
            this.txtYPosition.TabIndex = 10;
            // 
            // txtXPosition
            // 
            this.txtXPosition.Location = new System.Drawing.Point(250, 273);
            this.txtXPosition.Name = "txtXPosition";
            this.txtXPosition.Size = new System.Drawing.Size(62, 25);
            this.txtXPosition.TabIndex = 9;
            // 
            // btnConfigura
            // 
            this.btnConfigura.Location = new System.Drawing.Point(238, 339);
            this.btnConfigura.Name = "btnConfigura";
            this.btnConfigura.Size = new System.Drawing.Size(75, 28);
            this.btnConfigura.TabIndex = 8;
            this.btnConfigura.Text = "Configura";
            this.btnConfigura.UseVisualStyleBackColor = true;
            this.btnConfigura.Click += new System.EventHandler(this.btnConfigura_Click);
            // 
            // lblYPosition
            // 
            this.lblYPosition.AutoSize = true;
            this.lblYPosition.Location = new System.Drawing.Point(162, 310);
            this.lblYPosition.Name = "lblYPosition";
            this.lblYPosition.Size = new System.Drawing.Size(89, 18);
            this.lblYPosition.TabIndex = 7;
            this.lblYPosition.Text = "Coordinata Y:";
            // 
            // lblXPosition
            // 
            this.lblXPosition.AutoSize = true;
            this.lblXPosition.Location = new System.Drawing.Point(162, 276);
            this.lblXPosition.Name = "lblXPosition";
            this.lblXPosition.Size = new System.Drawing.Size(91, 18);
            this.lblXPosition.TabIndex = 6;
            this.lblXPosition.Text = "Coordinata X:";
            // 
            // lstBoxNoConfDevice
            // 
            this.lstBoxNoConfDevice.BackColor = System.Drawing.SystemColors.Window;
            this.lstBoxNoConfDevice.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lstBoxNoConfDevice.FormattingEnabled = true;
            this.lstBoxNoConfDevice.ItemHeight = 18;
            this.lstBoxNoConfDevice.Location = new System.Drawing.Point(10, 273);
            this.lstBoxNoConfDevice.Name = "lstBoxNoConfDevice";
            this.lstBoxNoConfDevice.Size = new System.Drawing.Size(145, 94);
            this.lstBoxNoConfDevice.TabIndex = 5;
            // 
            // lblConfiguraDevice
            // 
            this.lblConfiguraDevice.AutoSize = true;
            this.lblConfiguraDevice.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);
            this.lblConfiguraDevice.Location = new System.Drawing.Point(7, 251);
            this.lblConfiguraDevice.Name = "lblConfiguraDevice";
            this.lblConfiguraDevice.Size = new System.Drawing.Size(157, 18);
            this.lblConfiguraDevice.TabIndex = 4;
            this.lblConfiguraDevice.Text = "Configura nuovo device:";
            // 
            // tabDeviceConf
            // 
            this.tabDeviceConf.Controls.Add(this.tabPage1);
            this.tabDeviceConf.Controls.Add(this.tabPage2);
            this.tabDeviceConf.Location = new System.Drawing.Point(6, 94);
            this.tabDeviceConf.Name = "tabDeviceConf";
            this.tabDeviceConf.SelectedIndex = 0;
            this.tabDeviceConf.Size = new System.Drawing.Size(308, 155);
            this.tabDeviceConf.TabIndex = 3;
            this.tabDeviceConf.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.textBox2);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.lblPosizioneDevice);
            this.tabPage1.Controls.Add(this.lblIpDeviceConf);
            this.tabPage1.Location = new System.Drawing.Point(4, 27);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(300, 124);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(7, 90);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(133, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Elimina device";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(189, 62);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(73, 27);
            this.button1.TabIndex = 7;
            this.button1.Text = "Modifica";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(117, 64);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(57, 25);
            this.textBox2.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(93, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "Y:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(30, 64);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(57, 25);
            this.textBox1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "X:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "Modifica coordinate:";
            // 
            // lblPosizioneDevice
            // 
            this.lblPosizioneDevice.AutoSize = true;
            this.lblPosizioneDevice.Location = new System.Drawing.Point(7, 26);
            this.lblPosizioneDevice.Name = "lblPosizioneDevice";
            this.lblPosizioneDevice.Size = new System.Drawing.Size(72, 18);
            this.lblPosizioneDevice.TabIndex = 1;
            this.lblPosizioneDevice.Text = "Posizione:";
            // 
            // lblIpDeviceConf
            // 
            this.lblIpDeviceConf.AutoSize = true;
            this.lblIpDeviceConf.Location = new System.Drawing.Point(7, 4);
            this.lblIpDeviceConf.Name = "lblIpDeviceConf";
            this.lblIpDeviceConf.Size = new System.Drawing.Size(80, 18);
            this.lblIpDeviceConf.TabIndex = 0;
            this.lblIpDeviceConf.Text = "Indirizzo IP:";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 27);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(300, 119);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lblNoDeviceConf
            // 
            this.lblNoDeviceConf.AutoSize = true;
            this.lblNoDeviceConf.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Italic);
            this.lblNoDeviceConf.Location = new System.Drawing.Point(9, 94);
            this.lblNoDeviceConf.Name = "lblNoDeviceConf";
            this.lblNoDeviceConf.Size = new System.Drawing.Size(190, 18);
            this.lblNoDeviceConf.TabIndex = 1;
            this.lblNoDeviceConf.Text = "Non ci sono device configurati";
            // 
            // lblElencoDeviceConf
            // 
            this.lblElencoDeviceConf.AutoSize = true;
            this.lblElencoDeviceConf.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);
            this.lblElencoDeviceConf.Location = new System.Drawing.Point(6, 68);
            this.lblElencoDeviceConf.Name = "lblElencoDeviceConf";
            this.lblElencoDeviceConf.Size = new System.Drawing.Size(189, 18);
            this.lblElencoDeviceConf.TabIndex = 2;
            this.lblElencoDeviceConf.Text = "Elenco dispositivi configurati:";
            // 
            // lblNumDeviceNonConf
            // 
            this.lblNumDeviceNonConf.AutoSize = true;
            this.lblNumDeviceNonConf.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);
            this.lblNumDeviceNonConf.Location = new System.Drawing.Point(6, 46);
            this.lblNumDeviceNonConf.Name = "lblNumDeviceNonConf";
            this.lblNumDeviceNonConf.Size = new System.Drawing.Size(216, 18);
            this.lblNumDeviceNonConf.TabIndex = 1;
            this.lblNumDeviceNonConf.Text = "Numero device non configurati:  0";
            // 
            // lblNumDeviceConf
            // 
            this.lblNumDeviceConf.AutoSize = true;
            this.lblNumDeviceConf.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);
            this.lblNumDeviceConf.Location = new System.Drawing.Point(7, 25);
            this.lblNumDeviceConf.Name = "lblNumDeviceConf";
            this.lblNumDeviceConf.Size = new System.Drawing.Size(189, 18);
            this.lblNumDeviceConf.TabIndex = 0;
            this.lblNumDeviceConf.Text = "Numero device configurati:  0";
            // 
            // groupBoxView1
            // 
            this.groupBoxView1.Controls.Add(this.imgRefresh);
            this.groupBoxView1.Controls.Add(this.chartNumberDevice);
            this.groupBoxView1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxView1.Location = new System.Drawing.Point(351, 42);
            this.groupBoxView1.Name = "groupBoxView1";
            this.groupBoxView1.Size = new System.Drawing.Size(1008, 332);
            this.groupBoxView1.TabIndex = 1;
            this.groupBoxView1.TabStop = false;
            this.groupBoxView1.Text = "Conteggio Device Univoci";
            // 
            // imgRefresh
            // 
            this.imgRefresh.Image = global::SnifferProbeRequestApp.Properties.Resources.refresh;
            this.imgRefresh.Location = new System.Drawing.Point(6, 17);
            this.imgRefresh.Name = "imgRefresh";
            this.imgRefresh.Size = new System.Drawing.Size(25, 26);
            this.imgRefresh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgRefresh.TabIndex = 1;
            this.imgRefresh.TabStop = false;
            this.toolTipRefresh.SetToolTip(this.imgRefresh, "Aggiorna");
            this.imgRefresh.Click += new System.EventHandler(this.imgRefresh_Click);
            // 
            // chartNumberDevice
            // 
            this.chartNumberDevice.BorderlineWidth = 0;
            this.chartNumberDevice.CausesValidation = false;
            chartArea3.Name = "ChartArea1";
            this.chartNumberDevice.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chartNumberDevice.Legends.Add(legend3);
            this.chartNumberDevice.Location = new System.Drawing.Point(6, 49);
            this.chartNumberDevice.Name = "chartNumberDevice";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
            series3.IsVisibleInLegend = false;
            series3.IsXValueIndexed = true;
            series3.Legend = "Legend1";
            series3.Name = "N. dispositivi univoci";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series3.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int64;
            this.chartNumberDevice.Series.Add(series3);
            this.chartNumberDevice.Size = new System.Drawing.Size(996, 277);
            this.chartNumberDevice.TabIndex = 0;
            this.chartNumberDevice.Text = "chartNumberDevice";
            // 
            // groupBoxView2
            // 
            this.groupBoxView2.Controls.Add(this.chartPositionDevice);
            this.groupBoxView2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.groupBoxView2.Location = new System.Drawing.Point(351, 380);
            this.groupBoxView2.Name = "groupBoxView2";
            this.groupBoxView2.Size = new System.Drawing.Size(1008, 306);
            this.groupBoxView2.TabIndex = 2;
            this.groupBoxView2.TabStop = false;
            this.groupBoxView2.Text = "Mappa Posizione Device";
            // 
            // chartPositionDevice
            // 
            chartArea4.Name = "ChartArea1";
            this.chartPositionDevice.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.chartPositionDevice.Legends.Add(legend4);
            this.chartPositionDevice.Location = new System.Drawing.Point(7, 27);
            this.chartPositionDevice.Name = "chartPositionDevice";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartPositionDevice.Series.Add(series4);
            this.chartPositionDevice.Size = new System.Drawing.Size(995, 279);
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
            this.lblProgetto.Location = new System.Drawing.Point(10, 597);
            this.lblProgetto.Name = "lblProgetto";
            this.lblProgetto.Size = new System.Drawing.Size(335, 18);
            this.lblProgetto.TabIndex = 3;
            this.lblProgetto.Text = "Progetto di Programmazione di Sistema";
            // 
            // lblAnno
            // 
            this.lblAnno.AutoSize = true;
            this.lblAnno.Font = new System.Drawing.Font("Verdana", 11F, System.Drawing.FontStyle.Bold);
            this.lblAnno.Location = new System.Drawing.Point(10, 626);
            this.lblAnno.Name = "lblAnno";
            this.lblAnno.Size = new System.Drawing.Size(232, 18);
            this.lblAnno.TabIndex = 4;
            this.lblAnno.Text = "Anno Accademico 2017/18";
            // 
            // lblNomeCognome
            // 
            this.lblNomeCognome.AutoSize = true;
            this.lblNomeCognome.Font = new System.Drawing.Font("Verdana", 11F, System.Drawing.FontStyle.Bold);
            this.lblNomeCognome.Location = new System.Drawing.Point(10, 656);
            this.lblNomeCognome.Name = "lblNomeCognome";
            this.lblNomeCognome.Size = new System.Drawing.Size(228, 18);
            this.lblNomeCognome.TabIndex = 5;
            this.lblNomeCognome.Text = "Vincenzo Siciliani #243178";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::SnifferProbeRequestApp.Properties.Resources.polito_logo;
            this.pictureBox1.Location = new System.Drawing.Point(13, 453);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(332, 142);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(427, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(546, 25);
            this.lblTitle.TabIndex = 7;
            this.lblTitle.Text = "SISTEMA RILEVAZIONE PRESENZE REAL TIME";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1371, 693);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblNomeCognome);
            this.Controls.Add(this.lblAnno);
            this.Controls.Add(this.lblProgetto);
            this.Controls.Add(this.groupBoxView2);
            this.Controls.Add(this.groupBoxView1);
            this.Controls.Add(this.groupBoxSettings);
            this.Name = "frmMain";
            this.Text = "Sistema rilevazione presenze";
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabDeviceConf.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBoxView1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartNumberDevice)).EndInit();
            this.groupBoxView2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartPositionDevice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.GroupBox groupBoxView1;
        public System.Windows.Forms.DataVisualization.Charting.Chart chartNumberDevice;
        private System.Windows.Forms.GroupBox groupBoxView2;
        private System.Windows.Forms.Timer timerUpdateChartNumberDevice;
        private System.Windows.Forms.Label lblProgetto;
        private System.Windows.Forms.Label lblAnno;
        private System.Windows.Forms.Label lblNomeCognome;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox imgRefresh;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstBoxNoConfDevice;
        private System.Windows.Forms.Label lblConfiguraDevice;
        private System.Windows.Forms.Label lblElencoDeviceConf;
        private System.Windows.Forms.Label lblNumDeviceNonConf;
        private System.Windows.Forms.Label lblNumDeviceConf;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPositionDevice;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ToolTip toolTipRefresh;
        private System.Windows.Forms.Label lblNoDeviceConf;
        private System.Windows.Forms.TabControl tabDeviceConf;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lblPosizioneDevice;
        private System.Windows.Forms.Label lblIpDeviceConf;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtYPosition;
        private System.Windows.Forms.TextBox txtXPosition;
        private System.Windows.Forms.Button btnConfigura;
        private System.Windows.Forms.Label lblYPosition;
        private System.Windows.Forms.Label lblXPosition;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnIdentifica;
    }
}


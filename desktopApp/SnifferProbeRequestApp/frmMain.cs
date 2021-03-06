﻿using SnifferProbeRequestApp.valueClass;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SnifferProbeRequestApp
{
    public partial class frmMain : Form
    {
        DatabaseManager dbManager = null;
        public frmMain(NetworkSettings settings)
        {
            InitializeComponent();

            lblServerPort.Text = "Porta Server: " + settings.servicePort;

            dbManager = DatabaseManager.getInstance();

            //mi iscrivo agli handler
            ConfDevice.LstConfDevicesChanged += updateConfDevice;
            ConfDevice.LstConfDevicesChanged += checkTwoESP;
            NoConfDevice.LstNoConfDevicesChanged += updateNoConfDevice;
        }
       
        //EVENTI
        private void timerUpdateChartNumberDevice_Tick(object sender, EventArgs e)
        {
            updateChartNumberDevice();
            updateChartPosition();
        }

        private void imgRefreshConteggio_Click(object sender, EventArgs e) {
            updateChartNumberDevice();
        }

        private void BtnRefreshPosizioni_Click(object sender, EventArgs e) {
            updateChartPosition();
        }

        private void btnIdentificaDevice_Click(object sender, EventArgs e) {
            if (lstBoxNoConfDevice.SelectedItem == null) {
                MessageBox.Show("Selezionare un device dall'elenco", "Seleziona il device da configurare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else { //i parametri sono validi

                //leggo l'IP del device selezionato
                String ipAddress = lstBoxNoConfDevice.SelectedItem.ToString();
                ManualResetEvent deviceEvent;
                NoConfDevice.lstNoConfDevices.TryGetValue(ipAddress, out deviceEvent);
                //risveglio il thread che gestisce la connessione del dispositivo per inviare il segnale di "IDENTIFICA"
                deviceEvent.Set();
            }
        }

        private void btnSalvaDevice_Click(object sender, EventArgs e) {
            int xPosition, yPosition;
            //check sui parametri (device selezionato e valore contenuto nella cella)
            if (lstBoxNoConfDevice.SelectedItem == null) {
                MessageBox.Show("Selezionare un device dall'elenco", "Seleziona il device da configurare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (!int.TryParse(txtXPosition.Text, out xPosition)) {
                MessageBox.Show("Posizione X non valida", "Inserire un valore numerico per la posizione X ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (!int.TryParse(txtYPosition.Text, out yPosition)) {
                MessageBox.Show("Posizione Y non valida", "Inserire un valore numerico per la posizione Y", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else { //i parametri sono validi

                //leggo l'IP del device selezionato
                String ipAddress = lstBoxNoConfDevice.SelectedItem.ToString();

                //creo l'oggetto Device
                try {
                    //devo togliere il device dalla lista dei non configurati e aggiungerlo tra quelli configurati

                    //elimino il device dalla lista dei device non configurati e lancio il delegato associato
                    //semaforo per gestire la concorrenza con il thread che gestisce la connessione con il device
                    ManualResetEvent deviceEvent;
                    NoConfDevice.lstNoConfDevices.TryRemove(ipAddress, out deviceEvent);

                    Device device = new Device(ipAddress, xPosition, yPosition, 0, deviceEvent);

                    //aggiungo il device alla lista degi device configurati e lancio il delegato associato
                    ConfDevice.lstConfDevices.TryAdd(device.ipAddress, device);
                    ConfDevice.OnLstConfDevicesChanged(this, EventArgs.Empty);

                    //risveglio il thread che gestisce la connessione del dispositivo in quanto la configurazione è completata
                    deviceEvent.Set();
                    NoConfDevice.OnLstNoConfDevicesChanged(this, EventArgs.Empty);

                    //ripulisco la textbox
                    txtXPosition.Clear();
                    txtYPosition.Clear();
                } catch (Exception ex) {
                    MessageBox.Show("Errore", "Si è verificato un errore durante la configurazione del dispositivo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Error, ex.ToString());
                }
            }         
        }

        private void btnModificaCoordinate_Click(object sender, EventArgs e, Device device, string newX, string newY) {
            int xPosition, yPosition;
            if (!int.TryParse(newX, out xPosition)) {
                MessageBox.Show("Posizione X non valida", "Inserire un valore numerico per la posizione X ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }  else if (!int.TryParse(newY, out yPosition)) {
                MessageBox.Show("Posizione Y non valida", "Inserire un valore numerico per la posizione Y", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                device.x_position = xPosition;
                device.y_position = yPosition;
                ConfDevice.lstConfDevices.AddOrUpdate(device.ipAddress, device, (k, v) => v);
                ConfDevice.OnLstConfDevicesChanged(this, EventArgs.Empty);
            }
        }

        private void btnEliminaConfDevice_Click(object sender, EventArgs e, String deviceIp) {
            Device device;
            try {
                //devo togliere il device dalla lista dei configurati e aggiungerlo tra quelli non configurati
                //questo codice è duplicato in ThreadGestioneWifi (valutare se fare una funzione)
                ConfDevice.lstConfDevices.TryRemove(deviceIp, out device);
                ConfDevice.OnLstConfDevicesChanged(this, EventArgs.Empty);

                NoConfDevice.lstNoConfDevices.TryAdd(deviceIp, device.evento);
                NoConfDevice.OnLstNoConfDevicesChanged(this, EventArgs.Empty);
            } catch (Exception) {
                MessageBox.Show("Errore", "Si è verificato un errore durante la cancellazione del dispositivo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCercaStatisticheLungoPeriodo_Click(object sender, EventArgs e) {
            chartStatisticaLungoPeriodo.Series.Clear();

            string numDevice = upDownNumDevice.Value.ToString();
            DateTime filter = dateTimePickerLimite.Value.ToUniversalTime();
            string date = filter.ToString("yyyy-MM-dd HH:mm:ss");

            List<ConnectionPeriod> devicePeriod;
            try {
                devicePeriod = dbManager.longTermStatistic(numDevice, date);
            } catch (SnifferAppSqlException ex) {
                MessageBox.Show("Errore", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Dictionary<string, int> device = new Dictionary<string, int>();

            foreach (ConnectionPeriod period in devicePeriod) {

                Series series = new Series();
                series.YValuesPerPoint = 2;
                series.XValueType = ChartValueType.String;
                series.YValueType = ChartValueType.DateTime;
                series.ChartType = SeriesChartType.RangeBar;
                series.CustomProperties = "DrawSideBySide=False";
                series.Color = Color.Blue;
                series.IsVisibleInLegend = false;

                int id;

                if (device.ContainsKey(period.sourceAddress)) {
                    id = device[period.sourceAddress];
                } else {
                    id = device.Count;
                    device.Add(period.sourceAddress, id);
                }

                int idPoint = series.Points.AddXY(id, period.startTimestamp.ToLocalTime(), period.stopTimestamp.ToLocalTime());
                series.Points[idPoint].AxisLabel = period.sourceAddress;
                series.ToolTip = "Device " + period.sourceAddress + " connesso dalle: " + period.startTimestamp.ToLocalTime().ToString() + " alle " + period.stopTimestamp.ToLocalTime().ToString();
                chartStatisticaLungoPeriodo.Series.Add(series);

            }

            chartStatisticaLungoPeriodo.ChartAreas[0].AxisY.LabelStyle.Format = "dd/MM/yyyy HH:mm";
            chartStatisticaLungoPeriodo.ChartAreas[0].RecalculateAxesScale();
        }

        //inizio eventi bottoni timePicketr statistiche lungo periodo
        private void Btn30min_Click(object sender, EventArgs e) {
            dateTimePickerLimite.Value = DateTime.Now.AddMinutes(-30);
        }

        private void Btn1Ora_Click(object sender, EventArgs e) {
            dateTimePickerLimite.Value = DateTime.Now.AddHours(-1);
        }

        private void Btn6Ore_Click(object sender, EventArgs e) {
            dateTimePickerLimite.Value = DateTime.Now.AddHours(-6);
        }

        private void Btn12Ore_Click(object sender, EventArgs e) {
            dateTimePickerLimite.Value = DateTime.Now.AddHours(-12);
        }

        private void Btn1giorno_Click(object sender, EventArgs e) {
            dateTimePickerLimite.Value = DateTime.Now.AddDays(-1);
        }

        private void Btn7giorni_Click(object sender, EventArgs e) {
            dateTimePickerLimite.Value = DateTime.Now.AddDays(-7);
        }
        //fine eventi bottoni timePicketr statistiche lungo periodo

        //inizio eventi bottoni timePicketr movimento
        private void Btn30minMin_Click(object sender, EventArgs e) {
            dateTimePickerDateMin.Value = DateTime.Now.AddMinutes(-30);
        }

        private void Btn1oraMin_Click(object sender, EventArgs e) {
            dateTimePickerDateMin.Value = DateTime.Now.AddHours(-1);
        }

        private void Btn6oreMIn_Click(object sender, EventArgs e) {
            dateTimePickerDateMin.Value = DateTime.Now.AddHours(-6);
        }

        private void Btn12oreMin_Click(object sender, EventArgs e) {
            dateTimePickerDateMin.Value = DateTime.Now.AddHours(-12);
        }

        private void BtnNowMax_Click(object sender, EventArgs e) {
            dateTimePickerDateMax.Value = DateTime.Now;
        }

        private void Btn1oraMax_Click(object sender, EventArgs e) {
            dateTimePickerDateMax.Value = DateTime.Now.AddHours(-1);
        }

        private void Btn6oreMax_Click(object sender, EventArgs e) {
            dateTimePickerDateMax.Value = DateTime.Now.AddHours(-6);
        }

        private void Btn12oreMax_Click(object sender, EventArgs e) {
            dateTimePickerDateMax.Value = DateTime.Now.AddHours(-12);
        }
        //fine eventi bottoni timePicketr movimento

        //CUSTOM PROCEDURE

        public void updateChartNumberDevice() {
            CountDevice count;
            
            try {
                count = dbManager.countDevice();
            } catch (SnifferAppSqlException e) {
                MessageBox.Show("Errore", e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int po = chartNumberDevice.Series[0].Points.AddXY(count.timestamp, count.count);
            chartNumberDevice.Series[0].Points[po].ToolTip = "Timestamp: "+count.timestamp +"\nNumero device connessi: "+count.count; 
        }

        public void updateChartPosition() {
            List<DevicePosition> points;

            try {
                points = dbManager.devicesPosition();
            } catch (SnifferAppSqlException e) {
                MessageBox.Show("Errore", e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            chartPositionDevice.Series["Device"].Points.Clear();
            foreach (DevicePosition point in points) {
                int p = chartPositionDevice.Series["Device"].Points.AddXY(point.xPosition, point.yPosition);
                chartPositionDevice.Series["Device"].Points[p].ToolTip = point.sourceAddress;
            }
        }

        private void updateNoConfDevice(object sender, EventArgs e) {
            BeginInvoke((Action)(() => {
                lblNumDeviceNonConf.Text = "Numero device non configurati: " + NoConfDevice.lstNoConfDevices.Count;

                lstBoxNoConfDevice.Items.Clear();
                
                if (NoConfDevice.lstNoConfDevices.Count == 0) {
                    lblNoDeviceNoConf.Visible = true;
                    lstBoxNoConfDevice.Visible = false;
                    lblXPosition.Visible = false;
                    lblYPosition.Visible = false;
                    txtXPosition.Visible = false;
                    txtYPosition.Visible = false;
                    btnIdentificaDevice.Visible = false;
                    btnSalvaDevice.Visible = false;
                } else {

                    foreach (KeyValuePair<String, ManualResetEvent> device in NoConfDevice.lstNoConfDevices) {
                        lstBoxNoConfDevice.Items.Add(device.Key);
                    }

                    lblNoDeviceNoConf.Visible = false;
                    lstBoxNoConfDevice.Visible = true;
                    lblXPosition.Visible = true;
                    lblYPosition.Visible = true;
                    txtXPosition.Visible = true;
                    txtYPosition.Visible = true;
                    btnIdentificaDevice.Visible = true;
                    btnSalvaDevice.Visible = true;
                }
            }));          
        }

        private void checkTwoESP(object sender, EventArgs e) {
            BeginInvoke((Action)(() => {
                if (ConfDevice.lstConfDevices.Count >= 2 &&
                (ConfDevice.getMaxXPositionDevice() != ConfDevice.getMinXPositionDevice() || ConfDevice.getMaxYPositionDevice() != ConfDevice.getMinYPositionDevice())) {
                    tabFeatures.Visible = true;
                    btnRefreshConteggio.Visible = true;
                    lblMin2device.Visible = false;
                } else {
                    tabFeatures.Visible = false;
                    btnRefreshConteggio.Visible = false;
                    lblMin2device.Visible = true;
                }
            }));
        }

        private void updateConfDevice(object sender, EventArgs e) {
            
            BeginInvoke((Action)(() => {
                lblNumDeviceConf.Text = "Numero device configurati: " + ConfDevice.lstConfDevices.Count;

                if (ConfDevice.lstConfDevices.Count == 0) {
                    lblNoDeviceConf.Visible = true;
                    tabDeviceConf.Visible = false;
                    return;
                }

                tabDeviceConf.TabPages.Clear();

                //cancello tutti i punti ESP dai grafici delle posizioni e dei movimenti
                chartPositionDevice.Series["ESP"].Points.Clear();
                chartMovimentoDevice.Series["ESP"].Points.Clear();

                foreach (KeyValuePair<String, Device> device in ConfDevice.lstConfDevices) {

                    //setto le dimensioni degli assi del grafico che mostra le posizioni dei dispositivi rilevati
                    chartPositionDevice.ChartAreas[0].AxisX.Maximum = ConfDevice.getMaxXPositionDevice();
                    chartPositionDevice.ChartAreas[0].AxisX.Minimum = ConfDevice.getMinXPositionDevice();
                    
                    //aggiungo sul grafico delle posizioni il punto del device ESP
                    int point = chartPositionDevice.Series["ESP"].Points.AddXY(device.Value.x_position, device.Value.y_position);
                    chartPositionDevice.Series["ESP"].Points[point].ToolTip = device.Key;

                    //setto le dimensioni degli assi del grafico che mostra il movimento dispositivi rilevati
                    chartMovimentoDevice.ChartAreas[0].AxisX.Maximum = ConfDevice.getMaxXPositionDevice();
                    chartMovimentoDevice.ChartAreas[0].AxisX.Minimum = ConfDevice.getMinXPositionDevice();
                    
                    //aggiungo sul grafico dei movimenti il punto del device ESP
                    point = chartMovimentoDevice.Series["ESP"].Points.AddXY(device.Value.x_position, device.Value.y_position);
                    chartMovimentoDevice.Series["ESP"].Points[point].ToolTip = device.Key;

                    // 
                    // lblIpDeviceConf
                    // 
                    Label lblIpDeviceConf = new Label();
                    lblIpDeviceConf.AutoSize = true;
                    lblIpDeviceConf.Location = new Point(7, 4);
                    lblIpDeviceConf.Size = new Size(80, 18);
                    lblIpDeviceConf.Text = "Indirizzo IP: ";
                    lblIpDeviceConf.ForeColor = SystemColors.WindowText;
                    // 
                    // lblIndirizzoIpValue
                    // 
                    Label lblIndirizzoIpValue = new Label();
                    lblIndirizzoIpValue.AutoSize = true;
                    lblIndirizzoIpValue.Location = new Point(7, 27);
                    lblIndirizzoIpValue.Font = new Font("Calibri", 11F, FontStyle.Bold);
                    lblIndirizzoIpValue.Text = device.Value.ipAddress;
                    lblIndirizzoIpValue.ForeColor = SystemColors.WindowText;
                    // 
                    // lblPosizioneDevice
                    // 
                    Label lblPosizioneDeviceConf = new Label();
                    lblPosizioneDeviceConf.AutoSize = true;
                    lblPosizioneDeviceConf.Location = new Point(108, 4);
                    lblPosizioneDeviceConf.Text = "Posizione attuale:";
                    lblPosizioneDeviceConf.ForeColor = SystemColors.WindowText;
                    // 
                    // lblPosizioneValue
                    // 
                    Label lblPosizioneValue = new Label();
                    lblPosizioneValue.AutoSize = true;
                    lblPosizioneValue.Location = new Point(108, 27);
                    lblPosizioneValue.Font = new Font("Calibri", 11F, FontStyle.Bold);
                    lblPosizioneValue.Text = "(" + device.Value.x_position + "," + device.Value.y_position + ")";
                    lblPosizioneValue.ForeColor = SystemColors.WindowText;
                    // 
                    // lblModificaPosizione
                    // 
                    Label lblModificaPosizione = new Label();
                    lblModificaPosizione.AutoSize = true;
                    lblModificaPosizione.Location = new Point(238, 4);
                    lblModificaPosizione.Text = "Modifica posizione:";
                    lblModificaPosizione.ForeColor = SystemColors.WindowText;
                    // 
                    // lblModificaX
                    // 
                    Label lblModificaX = new Label();
                    lblModificaX.AutoSize = true;
                    lblModificaX.Location = new Point(238, 27);
                    lblModificaX.Size = new Size(72, 18);
                    lblModificaX.Text = "X:";
                    lblModificaX.ForeColor = SystemColors.WindowText;
                    // 
                    // txtModificaX
                    // 
                    TextBox txtModificaX = new TextBox();
                    txtModificaX.Location = new Point(262, 27);
                    txtModificaX.Size = new Size(57, 25);
                    // 
                    // lblModificaY
                    // 
                    Label lblModificaY = new Label();
                    lblModificaY.AutoSize = true;
                    lblModificaY.Location = new Point(321, 27);
                    lblModificaY.Size = new Size(72, 18);
                    lblModificaY.Text = "Y:";
                    lblModificaY.ForeColor = SystemColors.WindowText;
                    // 
                    // txtModificaY
                    // 
                    TextBox txtModificaY = new TextBox();
                    txtModificaY.Location = new Point(343, 27);
                    txtModificaY.Size = new Size(57, 25);
                    // 
                    // btnSalvaModificaDevice
                    //
                    PictureBox btnSalvaModificaDevice = new PictureBox();
                    btnSalvaModificaDevice.Image = Properties.Resources.success;
                    btnSalvaModificaDevice.Location = new Point(405, 27);
                    btnSalvaModificaDevice.Size = new Size(25, 25);
                    btnSalvaModificaDevice.SizeMode = PictureBoxSizeMode.StretchImage;
                    btnSalvaModificaDevice.TabStop = false;
                    toolTipApp.SetToolTip(btnSalvaModificaDevice, "Salva la nuova popsizione");
                    btnSalvaModificaDevice.Click += delegate {btnModificaCoordinate_Click(sender, e,
                        device.Value, txtModificaX.Text, txtModificaY.Text); 
                    };
                    
                    // 
                    // btnEliminaDevice
                    // 
                    PictureBox btnEliminaDevice = new PictureBox();
                    btnEliminaDevice.Image = Properties.Resources.error;
                    btnEliminaDevice.Location = new Point(436, 27);
                    btnEliminaDevice.Size = new Size(25, 25);
                    btnEliminaDevice.SizeMode = PictureBoxSizeMode.StretchImage;
                    btnEliminaDevice.TabStop = false;
                    toolTipApp.SetToolTip(btnEliminaDevice, "Elimina il device");
                    btnEliminaDevice.Click += delegate { btnEliminaConfDevice_Click(sender, e, device.Key); };
                    
                    
                    //aggiorno elenco devices configurati
                    TabPage tabPage = new TabPage();

                    tabPage.Controls.Add(lblIpDeviceConf);
                    tabPage.Controls.Add(lblIndirizzoIpValue);
                    tabPage.Controls.Add(lblPosizioneDeviceConf);
                    tabPage.Controls.Add(lblPosizioneValue);
                    tabPage.Controls.Add(lblModificaPosizione);
                    tabPage.Controls.Add(lblModificaX);
                    tabPage.Controls.Add(txtModificaX);
                    tabPage.Controls.Add(lblModificaY);
                    tabPage.Controls.Add(txtModificaY);
                    tabPage.Controls.Add(btnSalvaModificaDevice);
                    tabPage.Controls.Add(btnEliminaDevice);
                    tabPage.Location = new Point(4, 27);
                    tabPage.Padding = new Padding(3);
                    tabPage.Size = new Size(300, 59);
                    tabPage.Text = device.Value.ipAddress;
                    tabPage.UseVisualStyleBackColor = true;

                    tabDeviceConf.TabPages.Add(tabPage);
                }
                lblNoDeviceConf.Visible = false;
                tabDeviceConf.Visible = true;
            }));
        }

        private void btnCercaMovimentoDevice_Click(object sender, EventArgs e) {
            DateTime dateMin = dateTimePickerDateMin.Value.ToUniversalTime();
            DateTime dateMax = dateTimePickerDateMax.Value.ToUniversalTime();

            boxDevice.Items.Clear();

            foreach (string device in dbManager.connectedDeviceInPeriod(dateMin, dateMax)) {
                boxDevice.Items.Add(device);
            }

            if (boxDevice.Items.Count > 0)
                boxDevice.SelectedIndex = 0;
            else {
                //non ci sono device da mostrare
                boxDevice.Text = "";
                trackBarTempo.Enabled = false;
                chartMovimentoDevice.Series["Device"].Points.Clear();
                lblValoreMassimoTrackbar.Visible = false;
                lblValoreMinimoTrackbar.Visible = false;
                lblValoreSelezionatoTrackBar.Visible = false;
            }
        }

        private void BoxDevice_SelectedIndexChanged(object sender, EventArgs e) {
            string device = boxDevice.Text;
            DateTime dateMin = dateTimePickerDateMin.Value.ToUniversalTime();
            DateTime dateMax = dateTimePickerDateMax.Value.ToUniversalTime();

            CachedDevicePosition.cache.Clear();

            int i = 0;

            foreach (DevicePosition position in dbManager.deviceMovement(device, dateMin, dateMax)) {
                CachedDevicePosition.cache.Add(i, position);
                i++;
            }

            trackBarTempo.Enabled = true;
            lblValoreMassimoTrackbar.Visible = true;
            lblValoreMinimoTrackbar.Visible = true;
            lblValoreSelezionatoTrackBar.Visible = true;

            trackBarTempo.Minimum = 0;
            trackBarTempo.Maximum = CachedDevicePosition.cache.Count-1;
            trackBarTempo.Value = CachedDevicePosition.cache.Count-1;

            lblValoreMinimoTrackbar.Text = "Valore minimo: " + CachedDevicePosition.cache[0].time.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            lblValoreMassimoTrackbar.Text = "Valore massimo: " + CachedDevicePosition.cache[CachedDevicePosition.cache.Count - 1].time.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            lblValoreSelezionatoTrackBar.Text = "Valore selezionato: " + CachedDevicePosition.cache[CachedDevicePosition.cache.Count - 1].time.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

            //disegno il primo punto
            //ripulisco i punti del grafico del movimento
            chartMovimentoDevice.Series["Device"].Points.Clear();

            DevicePosition devicePosition = CachedDevicePosition.cache[CachedDevicePosition.cache.Count - 1];

            //aggiungo la nuova posizione
            int p = chartMovimentoDevice.Series["Device"].Points.AddXY(devicePosition.xPosition, devicePosition.yPosition);
            chartMovimentoDevice.Series["Device"].Points[p].ToolTip = devicePosition.time.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void TrackBarTempo_Scroll(object sender, EventArgs e) {
            DevicePosition position = CachedDevicePosition.cache[trackBarTempo.Value];
            lblValoreSelezionatoTrackBar.Text = "Valore selezionato: " + position.time.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

            //ripulisco i punti del grafico del movimento
            chartMovimentoDevice.Series["Device"].Points.Clear();

            //aggiungo la nuova posizione
            int p = chartMovimentoDevice.Series["Device"].Points.AddXY(position.xPosition, position.yPosition);
            chartMovimentoDevice.Series["Device"].Points[p].ToolTip = position.time.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

        }
    }
}

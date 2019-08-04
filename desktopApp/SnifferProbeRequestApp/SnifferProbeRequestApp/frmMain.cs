using SnifferProbeRequestApp.valueClass;
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
        public frmMain()
        {
            InitializeComponent();
            dbManager = DatabaseManager.getInstance();

            //mi iscrivo agli handler
            CommonData.LstConfDevicesChanged += updateConfDevice;
            CommonData.LstConfDevicesChanged += checkTwoESP;
            CommonData.LstNoConfDevicesChanged += updateNoConfDevice;
        }
       
        //EVENTI
        private void timerUpdateChartNumberDevice_Tick(object sender, EventArgs e)
        {
            updateChartNumberDevice();
        }

        private void imgRefresh_Click(object sender, EventArgs e)
        {
            updateChartNumberDevice();
        }

        private void btnIdentificaDevice_Click(object sender, EventArgs e) {
            if (lstBoxNoConfDevice.SelectedItem == null) {
                MessageBox.Show("Selezionare un device dall'elenco",
                    "Seleziona il device da configurare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else { //i parametri sono validi

                //leggo l'IP del device selezionato
                String ipAddress = lstBoxNoConfDevice.SelectedItem.ToString();
                ManualResetEvent deviceEvent = null;
                CommonData.lstNoConfDevices.TryGetValue(ipAddress, out deviceEvent);
                deviceEvent.Set();
            }
        }

        private void btnSalvaDevice_Click(object sender, EventArgs e) {
            Int32 xPosition = 0, yPosition = 0;
            //check sui parametri (device selezionato e valore contenuto nella cella)
            if (lstBoxNoConfDevice.SelectedItem == null) {
                MessageBox.Show("Selezionare un device dall'elenco",
                    "Seleziona il device da configurare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (!int.TryParse(txtXPosition.Text, out xPosition)) {
                MessageBox.Show("Posizione X non valida",
                    "Inserire un valore numerico per la posizione X ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (!int.TryParse(txtYPosition.Text, out yPosition)) {
                MessageBox.Show("Posizione Y non valida",
                    "Inserire un valore numerico per la posizione Y", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else { //i parametri sono validi

                //leggo l'IP del device selezionato
                String ipAddress = lstBoxNoConfDevice.SelectedItem.ToString();

                //creo l'oggetto Device
                try {
                    //devo togliere il device dalla lista dei non configurati e aggiungerlo tra quelli configurati

                    //elimino il device dalla lista dei device non configurati e lancio il delegato associato
                    //semaforo per gestire la concorrenza con il thread che gestisce la connessione con il device
                    ManualResetEvent deviceEvent = null;
                    CommonData.lstNoConfDevices.TryRemove(ipAddress, out deviceEvent);

                    Device device = new Device(ipAddress, 1, xPosition, yPosition, deviceEvent);

                    //aggiungo il device alla lista degi device configurati e lancio il delegato associato
                    CommonData.lstConfDevices.TryAdd(device.ipAddress, device);
                    CommonData.OnLstConfDevicesChanged(this, EventArgs.Empty);

                    deviceEvent.Set();
                    CommonData.OnLstNoConfDevicesChanged(this, EventArgs.Empty);

                    //ripulisco la textbox
                    txtXPosition.Clear();
                    txtYPosition.Clear();
                } catch (Exception ex) {
                    MessageBox.Show("Errore", "Si è verificato un errore durante la configurazione del dispositivo",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Error, ex.ToString());
                }
            }
            
        }

        private void btnModificaCoordinate_Click(object sender, EventArgs e, Device device, String newX, String newY)
        {
            Int32 xPosition = 0, yPosition = 0 ;
            if (!int.TryParse(newX, out xPosition))
            {
                MessageBox.Show("Posizione X non valida",
                    "Inserire un valore numerico per la posizione X ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!int.TryParse(newY, out yPosition))
            {
                MessageBox.Show("Posizione Y non valida",
                    "Inserire un valore numerico per la posizione Y", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                device.x_position = xPosition;
                device.y_position = yPosition;
                CommonData.lstConfDevices.AddOrUpdate(device.ipAddress, device, (k, v) => v);
                CommonData.OnLstConfDevicesChanged(this, EventArgs.Empty);
            }
        }

        private void btnEliminaConfDevice_Click(object sender, EventArgs e, String deviceIp) {
            Device device = null;
            try
            {
                //devo togliere il device dalla lista dei configurati e aggiungerlo tra quelli non configurati
                CommonData.lstConfDevices.TryRemove(deviceIp, out device);
                CommonData.OnLstConfDevicesChanged(this, EventArgs.Empty);

                CommonData.lstNoConfDevices.TryAdd(deviceIp, device.evento);
                CommonData.OnLstNoConfDevicesChanged(this, EventArgs.Empty);
            }
            catch (Exception)
            {
                MessageBox.Show("Errore", "Si è verificato un errore durante la cancellazione del dispositivo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //CUSTOM PROCEDURE
        public void updateChartNumberDevice() {
            CountDevice result = dbManager.countDevice();

            chartNumberDevice.Series[0].Points.AddXY(result.timestamp, result.count);

            List<DevicePosition> points = dbManager.devicesPosition();

            chartPositionDevice.Series["Device"].Points.Clear();
            foreach (DevicePosition point in points) {
                int p = chartPositionDevice.Series["Device"].Points.AddXY(point.xPosition, point.yPosition);
                chartPositionDevice.Series["Device"].Points[p].ToolTip = point.sourceAddress;
            }
        }

        private void updateNoConfDevice(object sender, EventArgs e) {
            BeginInvoke((Action)(() =>
            {
                lblNumDeviceNonConf.Text = "Numero device non configurati: " + CommonData.lstNoConfDevices.Count;

                lstBoxNoConfDevice.Items.Clear();
                

                if (CommonData.lstNoConfDevices.Count == 0) {
                    lblNoDeviceNoConf.Visible = true;
                    lstBoxNoConfDevice.Visible = false;
                    lblXPosition.Visible = false;
                    lblYPosition.Visible = false;
                    txtXPosition.Visible = false;
                    txtYPosition.Visible = false;
                    btnIdentificaDevice.Visible = false;
                    btnSalvaDevice.Visible = false;
                } else {

                    foreach (KeyValuePair<String, ManualResetEvent> device in CommonData.lstNoConfDevices) {
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

        private void checkTwoESP(object sender, EventArgs e){
            if (CommonData.lstConfDevices.Count >= 2){
                this.tabFeatures.Visible = true;
                this.btnRefresh.Visible = true;
                this.lblMin2device.Visible = false;
            } else {
                this.tabFeatures.Visible = true; //TODO: change
                this.btnRefresh.Visible = true; //TODO: change
                this.lblMin2device.Visible = false; //TODO: change
            }
        }

        private void updateConfDevice(object sender, EventArgs e) {
            
            lblNumDeviceConf.BeginInvoke((Action)(() => {
                lblNumDeviceConf.Text = "Numero device configurati: " + CommonData.lstConfDevices.Count;

                if (CommonData.lstConfDevices.Count == 0) {
                    lblNoDeviceConf.Visible = true;
                    tabDeviceConf.Visible = false;
                    return;
                }

                tabDeviceConf.TabPages.Clear();

                //cancello tutti i punti ESP dal grafico delle posizioni
                chartPositionDevice.Series["ESP"].Points.Clear();

                foreach (KeyValuePair<String, Device> device in CommonData.lstConfDevices) {

                    //aggiungo sul grafico delle posizioni il punto del device ESP
                    int point = chartPositionDevice.Series["ESP"].Points.AddXY(device.Value.x_position, device.Value.y_position);
                    chartPositionDevice.Series["ESP"].Points[point].ToolTip = device.Key;

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
            }));

            lblNoDeviceConf.Visible = false;
            tabDeviceConf.Visible = true;

        }

        private void BtnCercaStatisticheLungoPeriodo_Click(object sender, EventArgs e)
        {
            chartStatisticaLungoPeriodo.Series.Clear();

            String numDevice = upDownNumDevice.Value.ToString();
            DateTime filter = dateTimePickerLimite.Value.ToUniversalTime();
            String date = filter.ToString("yyyy-MM-dd HH:mm:ss");

            List<ConnectionPeriod> devicePeriod = dbManager.longTermStatistic(numDevice, date);

            Dictionary<String, Int32> device = new Dictionary<String, Int32>();

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
                series.ToolTip = "Device " + period.sourceAddress + " connesso dalle: " + period.startTimestamp.ToLocalTime().ToString() + " alle "+ period.stopTimestamp.ToLocalTime().ToString();
                chartStatisticaLungoPeriodo.Series.Add(series);

            }

            chartStatisticaLungoPeriodo.ChartAreas[0].AxisY.LabelStyle.Format = "dd/MM/yyyy HH:mm";
        }

        private void BtnOra_Click(object sender, EventArgs e)
        {
            dateTimePickerLimite.Value = DateTime.Now;
        }

        private void Btn1Ora_Click(object sender, EventArgs e)
        {
            dateTimePickerLimite.Value = DateTime.Now.AddHours(-1);
        }

        private void Btn6Ore_Click(object sender, EventArgs e)
        {
            dateTimePickerLimite.Value = DateTime.Now.AddHours(-6);
        }

        private void Btn12Ore_Click(object sender, EventArgs e)
        {
            dateTimePickerLimite.Value = DateTime.Now.AddHours(-12);
        }

        private void Btn1giorno_Click(object sender, EventArgs e)
        {
            dateTimePickerLimite.Value = DateTime.Now.AddDays(-1);
        }

        private void Btn7giorni_Click(object sender, EventArgs e)
        {
            dateTimePickerLimite.Value = DateTime.Now.AddDays(-7);
        }
    }
}

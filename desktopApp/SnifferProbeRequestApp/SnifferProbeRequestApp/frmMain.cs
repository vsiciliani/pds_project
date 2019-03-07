using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void btnIdentifica_Click(object sender, EventArgs e)
        {
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

        private void btnConfigura_Click(object sender, EventArgs e)
        {
            Int32 xPosition = 0, yPosition = 0;
            //check sui parametri (device selezionato e valore contenuto nella cella)
            if (lstBoxNoConfDevice.SelectedItem == null) {
                MessageBox.Show("Selezionare un device dall'elenco", 
                    "Seleziona il device da configurare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (! int.TryParse(txtXPosition.Text, out xPosition)) {
                MessageBox.Show("Posizione X non valida",
                    "Inserire un valore numerico per la posizione X ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (!int.TryParse(txtYPosition.Text, out yPosition)) {
                MessageBox.Show("Posizione Y non valida",
                    "Inserire un valore numerico per la posizione Y", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else { //i parametri sono validi

                //leggo l'IP del device selezionato
                String ipAddress = lstBoxNoConfDevice.SelectedItem.ToString();
                
                //creo l'oggetto Device
                Device device = new Device(lstBoxNoConfDevice.SelectedItem.ToString(), 1, xPosition, yPosition);
                try {
                    //devo togliere il device dalla lista dei non configurati e aggiungerlo tra quelli configurati

                    //aggiungo il device alla lista degi device configurati e lancio il delegato associato
                    CommonData.lstConfDevices.TryAdd(device.ipAddress, device);
                    CommonData.OnLstConfDevicesChanged(this, EventArgs.Empty);

                    //elimino il device dalla lista dei device non configurati e lancio il delegato associato
                    //semaforo per gestire la concorrenza con il thread che gestisce la connessione con il device
                    ManualResetEvent deviceEvent = null;
                    CommonData.lstNoConfDevices.TryRemove(device.ipAddress, out deviceEvent);
                    
                    deviceEvent.Set();
                    CommonData.OnLstNoConfDevicesChanged(this, EventArgs.Empty);

                    //ripulisco la textbox
                    txtXPosition.Clear();
                    txtYPosition.Clear();
                } catch (Exception)
                {
                    MessageBox.Show("Errore", "Si è verificato un errore durante la configurazione del dispositivo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                CommonData.lstNoConfDevices.TryAdd(deviceIp, null);
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
            KeyValuePair<DateTime, Int32> result = dbManager.countDevice();

            chartNumberDevice.Series[0].Points.AddXY(result.Key, result.Value);
        }

        private void updateNoConfDevice(object sender, EventArgs e) {
            BeginInvoke((Action)(() =>
            {
                lblNumDeviceNonConf.Text = "Numero device non configurati: " + CommonData.lstNoConfDevices.Count;

                lstBoxNoConfDevice.Items.Clear();
                foreach (KeyValuePair<String, ManualResetEvent> device in CommonData.lstNoConfDevices) {
                    lstBoxNoConfDevice.Items.Add(device.Key);

                }
                
            }));
        }

        private void updateConfDevice(object sender, EventArgs e) {
            
            lblNumDeviceConf.BeginInvoke((Action)(() => {
                lblNumDeviceConf.Text = "Numero device configurati: " + CommonData.lstConfDevices.Count;

                tabDeviceConf.TabPages.Clear();

                foreach (KeyValuePair<String, Device> device in CommonData.lstConfDevices) {
                    // 
                    // lblIpDeviceConf
                    // 
                    Label lblIpDeviceConf = new Label();
                    lblIpDeviceConf.AutoSize = true;
                    lblIpDeviceConf.Location = new Point(7, 4);
                    lblIpDeviceConf.Size = new Size(80, 18);
                    lblIpDeviceConf.Text = "Indirizzo IP: " + device.Value.ipAddress;
                    // 
                    // lblPosizioneDevice
                    // 
                    Label lblPosizioneDeviceConf = new Label();
                    lblPosizioneDeviceConf.AutoSize = true;
                    lblPosizioneDeviceConf.Location = new Point(7, 26);
                    lblPosizioneDeviceConf.Size = new Size(72, 18);
                    lblPosizioneDeviceConf.Text = "Posizione: ("+device.Value.x_position +","+device.Value.y_position +")";
                    // 
                    // lblModificaCoordinate
                    // 
                    Label lblModificaCoordinate = new Label();
                    lblModificaCoordinate.AutoSize = true;
                    lblModificaCoordinate.Location = new Point(7, 48);
                    lblModificaCoordinate.Size = new Size(72, 18);
                    lblModificaCoordinate.Text = "Modifica coordinate:";
                    // 
                    // lblModificaX
                    // 
                    Label lblModificaX = new Label();
                    lblModificaX.AutoSize = true;
                    lblModificaX.Location = new Point(7, 70);
                    lblModificaX.Size = new Size(72, 18);
                    lblModificaX.Text = "X:";
                    // 
                    // txtModificaX
                    // 
                    TextBox txtModificaX = new TextBox();
                    txtModificaX.Location = new Point(30, 70);
                    txtModificaX.Size = new Size(57, 25);
                    // 
                    // lblModificaY
                    // 
                    Label lblModificaY = new Label();
                    lblModificaY.AutoSize = true;
                    lblModificaY.Location = new Point(93, 70);
                    lblModificaY.Size = new Size(72, 18);
                    lblModificaY.Text = "Y:";
                    // 
                    // txtModificaY
                    // 
                    TextBox txtModificaY = new TextBox();
                    txtModificaY.Location = new Point(117, 70);
                    txtModificaY.Size = new Size(57, 25);
                    // 
                    // btnModificaCoordinate
                    // 
                    Button btnModificaCoordinate = new Button();
                    btnModificaCoordinate.Location = new Point(189, 70);
                    btnModificaCoordinate.Size = new Size(75, 27);
                    btnModificaCoordinate.Text = "Modifica";
                    btnModificaCoordinate.UseVisualStyleBackColor = true;
                    btnModificaCoordinate.Click += delegate { btnModificaCoordinate_Click(sender, e,
                        device.Value, txtModificaX.Text, txtModificaY.Text);
                    };
                    // 
                    // btnEliminaConfDevice
                    // 
                    Button btnEliminaConfDevice = new Button();
                    btnEliminaConfDevice.Location = new Point(7, 97);
                    btnEliminaConfDevice.Size = new Size(133, 27);
                    btnEliminaConfDevice.Text = "Elimina device";
                    btnEliminaConfDevice.UseVisualStyleBackColor = true;
                    btnEliminaConfDevice.Click += delegate { btnEliminaConfDevice_Click(sender, e, device.Key); };


                    //aggiorno elenco devices configurati
                    TabPage tabPage = new TabPage();

                    tabPage.Controls.Add(lblIpDeviceConf);
                    tabPage.Controls.Add(lblPosizioneDeviceConf);
                    tabPage.Controls.Add(lblModificaCoordinate);
                    tabPage.Controls.Add(lblModificaX);
                    tabPage.Controls.Add(txtModificaX);
                    tabPage.Controls.Add(lblModificaY);
                    tabPage.Controls.Add(txtModificaY);
                    tabPage.Controls.Add(btnModificaCoordinate);
                    tabPage.Controls.Add(btnEliminaConfDevice);
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

        
    }
}

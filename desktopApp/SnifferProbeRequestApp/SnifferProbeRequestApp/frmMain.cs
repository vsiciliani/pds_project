using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            } else {
                //i parametri sono validi
                String ipAddress = lstBoxNoConfDevice.SelectedItem.ToString();
                
                Device device = new Device(lstBoxNoConfDevice.SelectedItem.ToString(), 1, xPosition, yPosition);
                try {
                    //devo togliere il device dalla lista dei non configurati e aggiungerlo tra quelli configurati
                    CommonData.lstConfDevices.TryAdd(device.ipAddress, device);
                    CommonData.OnLstConfDevicesChanged(this, EventArgs.Empty);
                   
                    CommonData.lstNoConfDevices.TryRemove(device.ipAddress, out device);
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

        //CUSTOM PROCEDURE
        public void updateChartNumberDevice() {
            KeyValuePair<DateTime, Int32> result = dbManager.countDevice();

            chartNumberDevice.Series[0].Points.AddXY(result.Key, result.Value);
        }

        private void updateNoConfDevice(object sender, EventArgs e) {
            lblNumDeviceConf.BeginInvoke((Action)(() =>
            {
                lblNumDeviceConf.Text = "Numero device non configurati: " + CommonData.lstNoConfDevices.Count;

                lstBoxNoConfDevice.Items.Clear();
                foreach (KeyValuePair<String, Device> device in CommonData.lstNoConfDevices) {
                    lstBoxNoConfDevice.Items.Add(device.Value.ipAddress);

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
                    lblPosizioneDevice.AutoSize = true;
                    lblPosizioneDevice.Location = new Point(7, 26);
                    lblPosizioneDevice.Size = new Size(72, 18);
                    lblPosizioneDevice.Text = "Posizione: ("+device.Value.x_position +","+device.Value.y_position +")";

                    //aggiorno elenco devices configurati
                    TabPage tabPage = new TabPage();

                    tabPage.Controls.Add(lblPosizioneDevice);
                    tabPage.Controls.Add(lblIpDeviceConf);
                    tabPage.Location = new Point(4, 27);
                    tabPage.Padding = new Padding(3);
                    tabPage.Size = new Size(298, 59);
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

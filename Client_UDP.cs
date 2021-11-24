using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace reseau
{
    public delegate void UdpDataReceivedEventHandler(Byte[] data);

    public class Client_UDP
    {
        public event UdpDataReceivedEventHandler UDP_DataRecues;
        private int UDP_NumPort;
        public UdpClient MonUDP;
        private Thread UDP_Thread;
        private IPAddress AdresseIP;
        private IPEndPoint PtAttache;


        public Client_UDP() { }		// TODO : ajoutez ici la logique du constructeur

        public Client_UDP(System.Net.IPAddress IPAdd, int PortAdd)
        {
            AdresseIP = IPAdd;
            UDP_NumPort = PortAdd;
        }

        public object AdresseIP_Interface { get; internal set; }

        public IPEndPoint Ptreseau { get { return (PtAttache); } }

        public IPAddress IPConnexion
        {
            get { return (AdresseIP); }
            set { AdresseIP = value; }
        }

        public int Port { set { UDP_NumPort = value; } }
        public string Interface_Address { get; set; }

        ~Client_UDP()
        {

            //UDP_StopServer();


        }		// Destructeur

        public void UDP_Acquisition()
        {

            UDP_Thread = new Thread(new ThreadStart(UDP_StartServer));
            UDP_Thread.Priority = ThreadPriority.Highest;
            UDP_Thread.Start();
        }

        public void UDP_StopServer()
        {
            if (AdresseIP != null)
            {
                string ctrlmulticast = AdresseIP.ToString().Split(new string[] { "." }, StringSplitOptions.None)[0];
                if (Convert.ToInt32(ctrlmulticast) > 223)
                    try
                    {
                        MonUDP.DropMulticastGroup(AdresseIP);
                    }
                    catch { }
                try
                {
                      UDP_Thread.Abort();
                      MonUDP.Close();
                    }
                catch { }
            }

        }

        private void UDP_StartServer()
        {
            IPAddress curAdd = IPConnexion;
            try
            {
                //pour une utilisation multiple du port multicast 
                //par plusieurs applications
                MonUDP = new UdpClient();
                MonUDP.ExclusiveAddressUse = false;
                MonUDP.Client.ReceiveTimeout = 1;
               
                MonUDP.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                MonUDP.Client.EnableBroadcast = true;
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, this.UDP_NumPort);
                PtAttache = RemoteIpEndPoint;
                MonUDP.Client.Bind(PtAttache);
                //MonUDP.Client.DontFragment = false;
                MonUDP.Client.ReceiveBufferSize = 65536;

            }
            catch
            {
                //MessageBox.Show("Pas de connexion réseau valide  l'application va être fermée", "Problème connexion");
            }

            IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());

            if (curAdd.ToString() == "127.0.0.1")
            {
                AdresseIP = curAdd;
            }
            else
            {
                //groupe multicast IP est comprise entre 224.0.0.0 et 239.255.255.255.
                try
                {
                    if (Convert.ToInt32(AdresseIP.ToString().Split(new string[] { "." }, StringSplitOptions.None)[0]) > 223) 
                        MonUDP.JoinMulticastGroup(AdresseIP, IPAddress.Parse(Interface_Address));
                }
                catch
                { }
            }
            
            while (true)
            {
                //try
                //{
                    if (MonUDP.Available > 0)
                    {

                        UDP_DataRecues(MonUDP.Receive(ref PtAttache));
                    }
                Thread.Sleep(1);
                //}
                //catch (Exception e)
                //{
                //    string erreur = e.ToString();
                //  //  break;
                //}
            }

        }
    }
}

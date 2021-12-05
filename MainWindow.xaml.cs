using reseau;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace NeuroneToCsv
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        UdpClient EnvoiViewmap3D = null;
        UdpClient EnvoiViewmap = null;

        Configuration conf;
        private void ouvreconf()
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(Configuration));
            StreamReader reader = new StreamReader("ConFNeurons.xml");
            conf = (Configuration)mySerializer.Deserialize(reader);
            reader.Close();
        }
        private void sauveconfig()
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(Configuration));
            StreamWriter Writer = new StreamWriter("ConFNeurons.xml");
            mySerializer.Serialize(Writer, ((conf)));
            Writer.Close();

        }
        Client_UDP Mon_ecoute = null;
        List<Label> Lbinfosneurons = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


          
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "ConFNeurons.xml"))
            {
                ouvreconf();
            }
            else
            {
                conf = new Configuration();
                sauveconfig();
            }
            Lbinfosneurons= InfosNeurons.Children.OfType<Label>().ToList();
            EnvoiViewmap3D = new UdpClient(conf.AdresseViewmap3D, conf.PortViewmap3D);
            EnvoiViewmap = new UdpClient(conf.AdresseViewmap, conf.PortViewmap);

        }

        private void Bt_Ecoute_Click(object sender, RoutedEventArgs e)
        {
            if (Mon_ecoute == null)
            {
                Mon_ecoute = new Client_UDP();
                Mon_ecoute.Interface_Address = conf.AdresseCarte;
                Mon_ecoute.IPConnexion = IPAddress.Parse(conf.AdresseEcoute);
                Mon_ecoute.Port = conf.Port_Ecoute;
                Mon_ecoute.UDP_DataRecues += monlecteur_UDP_DataRecues;
                Mon_ecoute.UDP_Acquisition();
                Bt_Ecoute.Content = "Ecoute en cours";

            }
            else
            {
                Bt_Ecoute.Content = "Ecoute";
                Mon_ecoute.UDP_StopServer();
                Mon_ecoute = null;


            }
        }


        Neurons CurrentNeurons = new Neurons();
        private void monlecteur_UDP_DataRecues(byte[] data)
        {
            try
            {

                CurrentNeurons = new Neurons(data);
                if (CurrentNeurons.Etat)
                {
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        affich_reception(CurrentNeurons);
                    }));
                }

            }
            catch { }
        }

        string le_currentSerial = "1101";
        List<Neurons> lesNeurons = new List<Neurons>();
        int indexRecept = 0;
        private void affich_reception(Neurons currentNeurons)
        {
   
            Lbinfosneurons[0].Content = currentNeurons.serialNumber;
            Lbinfosneurons[1].Content = currentNeurons.Seconde;
            Lbinfosneurons[2].Content = currentNeurons.Gps_Fix;
            Lbinfosneurons[3].Content = currentNeurons.Latitude;
            Lbinfosneurons[4].Content = currentNeurons.Longitude;
            Lbinfosneurons[5].Content = currentNeurons.Altitude;
            Lbinfosneurons[6].Content = currentNeurons.Heading;
            Lbinfosneurons[7].Content = currentNeurons.Vh;
            Lbinfosneurons[8].Content = currentNeurons.Vz;

            List<Neurons> tmp = (from n in lesNeurons
                                 where n.serialNumber == currentNeurons.serialNumber
                                 select n).ToList();

            if (tmp.Count > 0)
            {
                tmp[0].Seconde = currentNeurons.Seconde;
                tmp[0].Gps_Fix = currentNeurons.Gps_Fix;
                tmp[0].Latitude = currentNeurons.Latitude;
                tmp[0].Longitude = currentNeurons.Longitude;
                tmp[0].Altitude = currentNeurons.Altitude;
                tmp[0].Heading = currentNeurons.Heading;
                tmp[0].Vh = currentNeurons.Vh;
                tmp[0].Vz = currentNeurons.Vz;

                //if (currentNeurons.serialNumber != le_currentSerial) { return; }

                double teste = tmp[0].CalRoulis(tmp[0].ValVh, 9.81);

                debug.Content = tmp[0].Delai.ToString();


                string chaine = tmp[0].CreateCmdViewmap3D(true);

                chaine = chaine.Replace("AviMOBPARAS", "Dup_"+ tmp[0].nomMobile );
                byte[] chainebyte = Encoding.UTF8.GetBytes(chaine);
                EnvoiViewmap3D.Send(chainebyte, chainebyte.Length);
                EnvoiViewmap.Send(tmp[0].CreateCmdViewmap(), 64);



            }
            if (tmp.Count == 0)
            {
              //if  (currentNeurons.serialNumber != "1"){ return; }
                currentNeurons.ValHeadingPrec = currentNeurons.ValHeading;
                if (currentNeurons.serialNumber == "1" || currentNeurons.serialNumber == "5" ||
                    currentNeurons.serialNumber == "10" || currentNeurons.serialNumber == "15" || currentNeurons.serialNumber == "15")
                { currentNeurons.Type = "pc7"; }
                if (currentNeurons.serialNumber == "31" || currentNeurons.serialNumber == "25" ||
                              currentNeurons.serialNumber == "20" )
                { currentNeurons.Type = "Tigre1"; }
                currentNeurons.startCptTemps();
              
                currentNeurons.nomMobile = currentNeurons.Type+"_ID" + currentNeurons.serialNumber;
                currentNeurons.nom_camera =  "Camera_"+ currentNeurons.Type+"_ID" + currentNeurons.serialNumber;
                lesNeurons.Add(currentNeurons);
                
                string duplicate = "DUPLICATE;C"+currentNeurons.Type+";ID" + currentNeurons.serialNumber;
                byte[] chainebyte = Encoding.UTF8.GetBytes(duplicate);
                EnvoiViewmap3D.Send(chainebyte, chainebyte.Length);


               
                string chaine = currentNeurons.CreateCmdViewmap3D(true);
                chaine = chaine.Replace("AviMOBPARAS", "Dup_" + currentNeurons.nomMobile );
                chainebyte = Encoding.UTF8.GetBytes(chaine);
                EnvoiViewmap3D.Send(chainebyte, chainebyte.Length);
          


                Lbneurons.ItemsSource = null;
                Lbneurons.ItemsSource = lesNeurons;
               // ajoute neurons 
                //roulis=0
                //envoi do
            }
            ValNeurons.DataContext = currentNeurons;
         }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Mon_ecoute!=null)
            {
                Mon_ecoute.UDP_StopServer();
            }
            sauveconfig();
        }

        private void Lbneurons_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
             //  le_currentSerial = ((Neurons)(Lbneurons.SelectedItem)).serialNumber;
            if (Lbneurons.SelectedItem == null) { return; }
            string chaine = "CAMS;" + ((Neurons)(Lbneurons.SelectedItem)).nom_camera;
            byte[]  chainebyte = Encoding.UTF8.GetBytes(chaine);
            EnvoiViewmap3D.Send(chainebyte, chainebyte.Length);

            // le_currentSerial=
        }
    }
}
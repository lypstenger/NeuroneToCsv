using reseau;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
   
        }

        private void Bt_Ecoute_Click(object sender, RoutedEventArgs e)
        {
            if (Mon_ecoute == null)
            {
                Mon_ecoute = new Client_UDP();
                Mon_ecoute.Interface_Address = conf.AdresseCarte;
                Mon_ecoute.IPConnexion = IPAddress.Parse(conf.AdresseEcoute);
                //Mon_ecoute.IPConnexion = IPAddress.Parse("127.0.0.1");
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

            if (tmp.Count>0)
            {

             if (tmp[0].serialNumber != "10") { return; }
  
            double roulis=    tmp[0].CalRoulis(0, currentNeurons.ValVh, 9.81);

                 debug.Content = tmp[0].Delai.ToString()+"    "+roulis.ToString() ;
                //calcul roulis 
                //envoi do
            }

            if (tmp.Count == 0)
            {

                currentNeurons.ValHeadingPrec = currentNeurons.ValHeading;
                currentNeurons.startCptTemps();
                lesNeurons.Add(currentNeurons);

               // ajoute neurons 
               //roulis=0
               //envoi do
            }



     


            //ValNeurons.DataContext = null;
            ValNeurons.DataContext = currentNeurons;




         }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Mon_ecoute!=null)
            {
                Mon_ecoute.UDP_StopServer();
            }
        }
    }
}
﻿using reseau;
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
                this.Dispatcher.BeginInvoke(new Action(delegate
                {
                    affich_reception(CurrentNeurons);
                }));


            }
            catch { }
        }

        private void affich_reception(Neurons currentNeurons)
        {
            Lbinfosneurons[0].Content = currentNeurons.serialNumber;
        }
    }
}
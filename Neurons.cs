using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroneToCsv
{
   public class Neurons : INotifyPropertyChanged
    {
        public Neurons() { }
        public Neurons(byte[]data) {

            dcomm(data);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            }
        }
        public string serialNumber { get; set; }
        public string Seconde { get; set; }
        public string Gps_Fix { get; set; }
        public string Latitude { get; set; }
        public double ValLatitude

        {
            get
            {
                return Convert.ToDouble(Latitude) / 10000;
            }
        }
        public string Longitude { get; set; }
        public double ValLongitude

        {
            get
            {
                return Convert.ToDouble(Longitude) / 10000;
            }
        }
        public string Altitude { get; set; }
        public double ValAltitude

        {
            get
            {
                return Convert.ToDouble(Altitude) ;
            }
        }
        public string Heading { get; set; }
        public int ValHeading

        {
            get
            {
                return Convert.ToInt32(Heading);
            }
        }

        public string Vh { get; set; }
        public double ValVh

        {
            get
            {
                return Convert.ToDouble(Vh) / 1000;
            }
        }
        public string Vz { get; set; }
        public double ValVz

        {
            get
            {
                return Convert.ToDouble(Vz) / 1000;
            }
        }
        public bool Etat { get; set; } = false;

        public void dcomm(byte[] data)
        {
            string s = "";
            try
            {
               s = Encoding.ASCII.GetString(data);
                if (s.IndexOf("$FLNO1") != 0) { return; }
           }
            catch { return; }

  
            try
            {
                string[] Tabinfos = s.Split(new string[] { "," }, StringSplitOptions.None);
                serialNumber = Tabinfos[1];
                Seconde = Tabinfos[2];
                Gps_Fix = Tabinfos[3];
                Latitude = Tabinfos[4];
                Longitude = Tabinfos[5];
                Altitude = Tabinfos[6];
                Heading = Tabinfos[7];
                Vh = Tabinfos[8];
                Vz = Tabinfos[9];
            }
            catch { return; }
            //its OK
            Etat = true;

            NotifyPropertyChanged("Etat");
        }
    }
}

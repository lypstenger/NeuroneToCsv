using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
                return Convert.ToDouble(Latitude) / 100000;
            }
        }
        public string Longitude { get; set; }
        public double ValLongitude

        {
            get
            {
                return Convert.ToDouble(Longitude) / 100000;
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
        public int ValHeadingPrec { get; set; }
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

        public double Temps1 { get; set; } = 0;
        public double Temps0 { get; set; } = 0;
        public double Pos_cap { get; set; } = 0;
        public double Pos_cap0 { get; set; } = 0;

        public double Delai { get; set; } = 0;






        public double CalRoulis(double omega, double vitesse, double g)
        {

            stopWatch.Stop();
           Delai = stopWatch.ElapsedMilliseconds;

            double Vomega = 1000 / Delai;
            if ((Pos_cap - Pos_cap0) > 170)
            {
                Pos_cap0 = Pos_cap;
            }
            if ((Pos_cap0 - Pos_cap) > 170)
            {
                Pos_cap0 = Pos_cap;
            }


            omega = ((float)((Pos_cap - Pos_cap0)));



             omega = omega*Vomega * Math.PI / 180f;


            double tge = vitesse * omega / g;


            double retour = Math.Atan(tge) * 180f / Math.PI;

            Pos_cap0 = Pos_cap;
            stopWatch.Restart();
            return retour;

        }

        private  Stopwatch stopWatch = new Stopwatch();

        public void startCptTemps()
        {
            stopWatch.Start();
   
        }

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
                string[] Tabinfos = s.Split(new string[] { ",","\r\n" }, StringSplitOptions.None);
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

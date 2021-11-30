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
        public Neurons()
        {
               }
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

        // AviMOBPARAS   AviMOBPC7  HelMOB1
        public string ChxMobile { get; set; } = "AviMOBPARAS";
        public double Temps1 { get; set; } = 0;
        public double Temps0 { get; set; } = 0;
  
        public double Delai { get; set; } = 0;

        public string Type { get; set; } = "Parachute";
        //pour viewmap3d
        public string nomMobile { get; set; }
        public string nom_camera { get; set; }
        public double Roulis { get; set; } = 0;
        public double Pitch { get; set; } = 0;

        public string CreateCmdViewmap3D(bool paras)
        {
            if (paras) { ChxMobile = "AviMOBPARAS"; }else { ChxMobile = "AviMOBPC7"; }

            string cmd = ChxMobile + ";" + ValLatitude.ToString("0.000000") + ";" + ValLongitude.ToString("0.000000") + ";" + ValAltitude.ToString("0.00") + ";" + ValHeading.ToString("0.0") + ";" + Roulis.ToString("00.000") + ";" + Pitch;
            return cmd;
        }
        public byte[] CreateCmdViewmap()
        {
                 byte[] env = new byte[64];
            //byte[] dfdfd = Encoding.Unicode.GetBytes("kcvbxcbdfwvbwb∑t∑€@t");s
            string st = nomMobile;
                if (st.Length>8)
            {  st = nomMobile.Substring(nomMobile.Length - 8, 8); }
            while (st.Length < 8)
            {
                st = " "+st;
            }
                Buffer.BlockCopy(Encoding.ASCII.GetBytes(st), 0, env, 0, 8);
                Buffer.BlockCopy(BitConverter.GetBytes(ValLatitude), 0, env, 16, 8);
                Buffer.BlockCopy(BitConverter.GetBytes(ValLongitude), 0, env, 24, 8);
                Buffer.BlockCopy(BitConverter.GetBytes(ValAltitude), 0, env, 32, 8);
   

            return env;
            }
        public override string ToString()
        {
            return serialNumber +" Type= "+Type;
        }

        public double CalRoulis(  double vitesse, double g)
        {

            stopWatch.Stop();
           Delai = stopWatch.ElapsedMilliseconds;

            double Vomega = 1000 / Delai;
            if ((ValHeading - ValHeadingPrec) > 170)
            {
                ValHeadingPrec = ValHeading;
            }
            if ((ValHeadingPrec - ValHeading) > 170)
            {
                ValHeadingPrec = ValHeading;
            }


            double omega = ((float)((ValHeading - ValHeadingPrec)));



             omega = omega*Vomega * Math.PI / 180f;


            double tge = vitesse * omega / g;


            Roulis = Math.Atan(tge) * 180f / Math.PI;


            Pitch = Math.Atan(ValVz / ValVh) * 180f / Math.PI;

            ValHeadingPrec = ValHeading;
            stopWatch.Restart();
            return Roulis;

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

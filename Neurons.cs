using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroneToCsv
{
   public class Neurons
    {
        public Neurons() { }
        public Neurons(byte[]data) {

            dcomm(data);
        }

        public string serialNumber { get; set; }
        public string Seconde { get; set; }
        public string Gps_Fix { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Altitude { get; set; }
        public string Heading { get; set; }
        public string Vh { get; set; }
        public string Vz { get; set; }
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
   

        }
    }
}

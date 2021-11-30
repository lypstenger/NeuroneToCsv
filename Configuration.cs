using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroneToCsv
{
  public  class Configuration
    {
        public string AdresseEcoute { get; set; } = "192.168.1.1";
        public int Port_Ecoute { get; set; } = 1983;
        public string AdresseCarte { get; set; } = "192.168.1.1";

        public string AdresseViewmap3D { get; set; } = "127.0.0.1";
        public int PortViewmap3D { get; set; } = 2024;
        public string AdresseViewmap { get; set; } = "127.0.0.1";
        public int PortViewmap { get; set; } = 2021;

        public Configuration() { }
    }
}

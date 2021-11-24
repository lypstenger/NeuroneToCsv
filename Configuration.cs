using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroneToCsv
{
  public  class Configuration
    {
        public string AdresseEcoute { get; set; } = "225.0.0.1";
        public int Port_Ecoute { get; set; } = 1983;
        public string AdresseCarte { get; set; } = "192.168.1.63";


        public Configuration() { }
    }
}

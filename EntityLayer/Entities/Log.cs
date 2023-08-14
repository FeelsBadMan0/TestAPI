using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Log
    {
        public int ID { get; set; }
        public string KAYITKODU { get; set; }
        public string CONTROLLERADI { get; set; }
        public string ACTIONADI { get; set; }
        public string IPADRES { get; set; }
        public DateTime TARIH { get; set; }
    }
}

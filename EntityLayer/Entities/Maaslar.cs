using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Maaslar
    {
        public int ID { get; set; }
        public string KAYITKODU { get; set; }
        public string CALISAN { get; set; }
        public Calisanlar Casilanlar { get; set; }
        public string DEPARTMAN { get; set; }
        public Departmanlar Departmanlar { get; set; }
        public decimal MAAS { get; set; }
    }
}

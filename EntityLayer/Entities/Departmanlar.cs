using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Departmanlar
    {
        public int ID { get; set; }
        public string KAYITKODU { get; set; }
        public string DEPARTMAN { get; set; }
        public int CALISANSAYISI { get; set; }

        public List<Maaslar> Maaslars { get; set; }
    }
}

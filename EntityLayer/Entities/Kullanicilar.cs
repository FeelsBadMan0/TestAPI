using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Kullanicilar
    {
        public int ID { get; set; }
        public string KULLANICIADI { get; set; }
        public string SIFRE { get; set; }
        public string KAYITKODU { get; set; }
        public DateTime KAYITTARIHI { get; set; }
        public string ROL { get; set; }
        public short AKTIF { get; set; }
        public bool BENIHATIRLA { get; set; }
        public byte[] PAROLASIFRELE { get; set; }
        public byte[] PAROLATUZLA { get; set; }
    }
}

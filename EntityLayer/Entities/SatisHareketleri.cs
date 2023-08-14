using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class SatisHareketleri
    {
        public int ID { get; set; }
        public string KAYITKODU { get; set; }
        public string URUN { get; set; }
        public Urunler Urunler { get; set; }
        public string CARI { get; set; }
        public Cariler Cariler { get; set; }
        public int MIKTAR { get; set; }
        public decimal FIYAT { get; set; }
        public DateTime SATISTARIHI { get; set; }
    }
}

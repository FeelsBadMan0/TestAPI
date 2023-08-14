using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Urunler
    {
        public int ID { get; set; }
        public string KAYITKODU { get; set; }
        public string URUN { get; set; }
        public int STOK { get; set; }
        public string KATEGORI { get; set; }
        public Kategoriler Kategoriler { get; set; }
        public string GORSEL { get; set; }
        public short AKTIF { get; set; }

        public List<SatisHareketleri> SatisHareketleris { get; set; }
    }
}

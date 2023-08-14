using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Kategoriler
    {
        public int ID { get; set; }
        public string KAYITKODU { get; set; }
        public string KATEGORI { get; set; }
        public short AKTIF { get; set; }
        public List<Urunler> Urunlers { get; set; }
    }
}

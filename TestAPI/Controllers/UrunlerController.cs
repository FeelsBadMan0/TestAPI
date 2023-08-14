using DataAccessLayer.GenericRepository;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TestAPI.Loglama;

namespace TestAPI.Controllers
{
    [Authorize]
    public class UrunlerController : ApiController
    {
        private readonly DapperGenericRepository _repository;
        private readonly TestLog _log;
        public UrunlerController()
        {
            _repository = new DapperGenericRepository();
            _log = new TestLog();
        }



        [HttpGet]
        public IEnumerable<Urunler> Listele()
        {
            //Log için bilgileri alıyoruz
            var controllerAdi = this.ControllerContext.ControllerDescriptor.ControllerName;
            var actionAdi = this.ActionContext.ActionDescriptor.ActionName;
            string ip;
            ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            //Veri tabanı ve listeleme işlemleri
            _log.Loglama(controllerAdi, actionAdi, ip, "API listeleme işlemi yapıldı.");
            return _repository.Query<Urunler>("sp_UrunGetir");
        }


        [Authorize(Roles = "Sistem Yöneticisi,AdminS")]
        [HttpPost]
        public IHttpActionResult UrunEkLe()
        {
            //Log için bilgileri alıyoruz
            var controllerAdi = this.ControllerContext.ControllerDescriptor.ControllerName;
            var actionAdi = this.ActionContext.ActionDescriptor.ActionName;
            string ip;
            ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }



            //Veri tabanı ve ekleme işlemleri
            var dosya = HttpContext.Current.Request.Files.Count > 0 ?
        HttpContext.Current.Request.Files[0] : null;
            var urunler = HttpContext.Current.Request.Form;
            var urun = urunler.GetValues("URUN")[0].ToString();
            var kategori = urunler.GetValues("KATEGORI")[0].ToString();
            var stok = Convert.ToInt32(urunler.GetValues("STOK")[0]);
            var kayitKodu = Guid.NewGuid().ToString();
            var aktif = Convert.ToInt16(1);

            if (dosya != null && dosya.ContentLength > 0 && urun != null && kategori != null && stok != 0)
            {
                var dosyaAdi = Path.GetFileName(dosya.FileName);

                var yol = Path.Combine(
                    HttpContext.Current.Server.MapPath("~/Images/"),
                    dosyaAdi
                );

                object parametreler = new
                {
                    @URUN = urun,
                    @KAYITKODU = kayitKodu,
                    @STOK = stok,
                    @KATEGORI = kategori,
                    @GORSEL = dosyaAdi,
                    @AKTIF = aktif
                };
                dosya.SaveAs(yol);
                int ekleme = _repository.Execute("sp_UrunEkle", parametreler);
                if (ekleme != 0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapıldı.");
                    return Ok("Ürün Ekleme İşlemi Başarılı!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Ürün Eklenirken Bir Hata Oluştu!");
                }


            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken parametreler boş bırakıldı.");
                return BadRequest("Ürün Alanları Boş Bırakılamaz!");
            }





        }

        [Authorize(Roles = "Sistem Yöneticisi")]
        [HttpPut]
        public IHttpActionResult Sil(Urunler urun)
        {
            //Log için bilgileri alıyoruz
            var controllerAdi = this.ControllerContext.ControllerDescriptor.ControllerName;
            var actionAdi = this.ActionContext.ActionDescriptor.ActionName;
            string ip;
            ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }


            //Veri tabanı ve silme işlemleri
            if (urun.KAYITKODU != null && urun.KAYITKODU != "")
            {
                object parametreler = new
                {
                    @KAYITKODU = urun.KAYITKODU
                };
                int sil = _repository.Execute("sp_UrunSil", parametreler);
                if (sil == 1)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapıldı.");
                    return Ok("Ürün Başarıyla Silindi!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Ürün Silinirken Bir Hata Oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapılırken KAYITKODU boş bırakıldı.");
                return BadRequest("Ürün Kayıt Kodu Boş Olamaz!");
            }
        }

        [HttpGet]
        public IHttpActionResult UrunGetir(Urunler urun)
        {
            //Log için bilgileri alıyoruz
            var controllerAdi = this.ControllerContext.ControllerDescriptor.ControllerName;
            var actionAdi = this.ActionContext.ActionDescriptor.ActionName;
            string ip;
            ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }


            //Veri tabanı ve ID'ye göre ürün getirme işlemleri
            if (urun.ID != 0)
            {
                object parametreler = new
                {
                    @ID = urun.ID
                };
                Urunler urunGetir = _repository.QueryFirstOrDefault<Urunler>("sp_UrunIDGetir", parametreler);
                if (urunGetir != null)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ID'ye göre veri çekme işlemi yapıldı.");
                    return Ok(urunGetir);
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ID'ye göre veri çekme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Ürün Bulunamadı!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API ID'ye göre veri çekme işlemi yapılırken ID boş bırakıldı.");
                return BadRequest("Ürün Alanı Boş Geçilemez!");
            }
        }

        [Authorize(Roles = "Sistem Yöneticisi,AdminS")]
        [HttpPut]
        public IHttpActionResult UrunGuncelle()
        {
            //Log için bilgileri alıyoruz
            var controllerAdi = this.ControllerContext.ControllerDescriptor.ControllerName;
            var actionAdi = this.ActionContext.ActionDescriptor.ActionName;
            string ip;
            ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }


            //Veri tabanı ve güncelleme işlemleri
            var dosya = HttpContext.Current.Request.Files.Count > 0 ?
            HttpContext.Current.Request.Files[0] : null;
            var urunler = HttpContext.Current.Request.Form;
            var id = Convert.ToInt32(urunler.GetValues("ID")[0]);
            var urun = urunler.GetValues("URUN")[0].ToString();
            var kategori = urunler.GetValues("KATEGORI")[0].ToString();
            var stok = Convert.ToInt32(urunler.GetValues("STOK")[0]);

            if (dosya != null && dosya.ContentLength > 0 && urun != null && kategori != null && stok != 0)
            {
                var dosyaAdi = Path.GetFileName(dosya.FileName);

                var yol = Path.Combine(
                    HttpContext.Current.Server.MapPath("~/Images/"),
                    dosyaAdi
                );

                dosya.SaveAs(yol);

                object parametreler = new
                {
                    @URUN = urun,
                    @STOK = stok,
                    @KATEGORI = kategori,
                    @GORSEL = dosyaAdi,
                    @ID = id
                };

                int guncelle = _repository.Execute("sp_UrunApiGuncelle", parametreler);
                if (guncelle == 1)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapıldı.");
                    return Ok("Ürün Güncelleme İşlemi Başarılı!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Ürün Güncellenirken Bir Hata Oluştu!");
                }

            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapılırken ID boş bırakıldı.");
                return BadRequest("Ürün Alanları Boş Bırakılamaz!");
            }
        }
    }
}

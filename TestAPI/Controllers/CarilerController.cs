using DataAccessLayer.GenericRepository;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestAPI.Loglama;

namespace TestAPI.Controllers
{
    [Authorize]
    public class CarilerController : ApiController
    {
        private readonly DapperGenericRepository _repository;
        private readonly TestLog _log;

        public CarilerController()
        {
            _repository = new DapperGenericRepository();
            _log = new TestLog();
        }

        [HttpGet]
        public IEnumerable<Cariler> Listele()
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
            return _repository.Query<Cariler>("sp_CariGetir");
        }

        [Authorize(Roles = "Sistem Yöneticisi,AdminS")]
        [HttpPost]
        public IHttpActionResult CariEkle(Cariler cari)
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
            if (cari.ADSOYAD != null && cari.ADSOYAD != "" && cari.MAIL != null && cari.MAIL != "" && cari.VERGINO != 0)
            {
                 cari.KAYITKODU = Guid.NewGuid().ToString();
                 cari.AKTIF = 1;
                DateTime kayitTarihi = cari.KAYITTARIHI = DateTime.Now;

                object parametreler = new
                {
                    @ADSOYAD = cari.ADSOYAD,
                    @MAIL = cari.MAIL,
                    @KAYITKODU = cari.KAYITKODU,
                    @KAYITTARIHI = cari.KAYITTARIHI,
                    @VERGINO = cari.VERGINO,
                    @AKTIF = cari.AKTIF
                };
                int ekle = _repository.Execute("sp_CariEkle", parametreler);
                if (ekle == 1)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapıldı.");
                    return Ok("Kişi başarıyla eklendi!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Cari Eklenirken Bir Hata Oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken parametreler boş bırakıldı.");
                return BadRequest("Lütfen Cari Bilgilerini Doldurun!");
            }
        }

        [Authorize(Roles = "Sistem Yöneticisi")]
        [HttpPut]
        public IHttpActionResult Sil(Cariler cari)
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
            if (cari.KAYITKODU != null && cari.KAYITKODU != "")
            {
                object parametreler = new
                {
                    @KAYITKODU = cari.KAYITKODU
                };

                int sil = _repository.Execute("sp_CariSil", parametreler);
                if (sil == 1)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapıldı.");
                    return Ok("Kişi Başarıyla Silindi!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Cari Silinirken Bir Hata Oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi sırasında KAYITKODU boş bırakıldı.");
                return BadRequest("Kayıt Kodu Boş Bırakılamaz!");
            }
        }


        [HttpGet]
        public IHttpActionResult CariGetir(Cariler cari)
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

            //Veri tabanı ve ID'ye göre veri çekme işlemleri
            if (cari.ID != 0)
            {
                object parametreler = new
                {
                    @ID = cari.ID
                };

                Cariler cariler = _repository.QueryFirstOrDefault<Cariler>("sp_CariIdGetir", parametreler);
                if (cariler != null)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ID'ye göre veri çekme işlemi yapıldı.");
                    return Ok(cariler);
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ID'ye göre veri çekme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Cari Eklenirken Bir Hata Oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API ID'ye göre veri çekme işlemi yapılırken ID boş bırakıldı.");
                return BadRequest("ID Alanı Boş Bırakılamaz!");
            }
        }

        [Authorize(Roles = "Sistem Yöneticisi,AdminS")]
        [HttpPut]
        public IHttpActionResult CariGuncelle(Cariler cari)
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
            if (cari.ADSOYAD != null && cari.ADSOYAD != "" && cari.MAIL != null && cari.MAIL != "" && cari.VERGINO != 0 && cari.ID != 0)
            {
                object parametreler = new
                {
                    @ADSOYAD = cari.ADSOYAD,
                    @MAIL = cari.MAIL,
                    @VERGINO = cari.VERGINO,
                    @ID = cari.ID
                };

                int guncelle = _repository.Execute("sp_CariGuncelle", parametreler);
                if (guncelle == 1)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapıldı.");
                    return Ok("Cari Güncelleme Başarılı Bir Şekilde Gerçekleşti!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Cari Güncellenirken Bir Hata Oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapılırken ID boş bırakıldı.");
                return BadRequest("Cari Bilgileri Boş Bırakılamaz!");
            }
        }
    }
}
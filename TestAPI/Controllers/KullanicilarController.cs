using DataAccessLayer.GenericRepository;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestAPI.Hash;
using TestAPI.Loglama;

namespace TestAPI.Controllers
{
    [Authorize(Roles = "Sistem Yöneticisi")]
    public class KullanicilarController : ApiController
    {
        private readonly ParolaSifreleme _sifrele;
        private readonly DapperGenericRepository _repository;
        private readonly TestLog _log;

        public KullanicilarController()
        {
            _repository = new DapperGenericRepository();
            _log = new TestLog();
            _sifrele= new ParolaSifreleme();
        }

        [HttpGet]
        public IEnumerable<Kullanicilar> Listele()
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

            //Veri tabanı ve liteleme işlemleri
            _log.Loglama(controllerAdi, actionAdi, ip, "API Listeleme işlemi yapıldı");
            return _repository.Query<Kullanicilar>("sp_KullaniciListele");
        }

        [HttpPost]
        public IHttpActionResult KullaniciEkle(Kullanicilar kullanici)
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
            if (!string.IsNullOrEmpty(kullanici.KULLANICIADI) && !string.IsNullOrEmpty(kullanici.SIFRE) && !string.IsNullOrEmpty(kullanici.ROL))
            {
                kullanici.KAYITKODU = Guid.NewGuid().ToString();
                kullanici.KAYITTARIHI = DateTime.Now;
                kullanici.AKTIF = 1;
                _sifrele.ParolaSifrele(kullanici.SIFRE, out byte[] sifreliparola, out byte[] tuzlama);

                object parametreler = new
                {
                    @KULLANICIADI = kullanici.KULLANICIADI,
                    @PAROLASIFRELE = sifreliparola,
                    @PAROLATUZLA= tuzlama,
                    @ROL = kullanici.ROL,
                    @KAYITKODU = kullanici.KAYITKODU,
                    @KAYITTARIHI = kullanici.KAYITTARIHI,
                    @AKTIF = kullanici.AKTIF
                };

                int ekle = _repository.Execute("sp_KullaniciEkle", parametreler);
                if (ekle != 0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API Ekleme işlemi yapıldı.");
                    return Ok("Kullanıcı Başarıyla Eklendi");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API Ekleme işlemi gerçekleşirken hata oluştu!.");
                    return BadRequest("Kullanıcı Eklenirken Bir Hata Oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API Ekleme işleminde parametreler boş geçildi.");
                return BadRequest("Kullanıcı Parametreeri Boş Geçilemez!");
            }

        }

        [HttpPut]
        public IHttpActionResult Sil(Kullanicilar kullanici)
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
            if (!string.IsNullOrEmpty(kullanici.KAYITKODU))
            {
                object parametreler = new
                {
                    @KAYITKODU = kullanici.KAYITKODU
                };

                int sil = _repository.Execute("sp_KullaniciSil", parametreler);
                if (sil != 0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API Silme işlemi yapıldı");
                    return Ok("Silme işlemi başarılı!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API Silme işlemi gerçekleşirken bir hata oluştu!");
                    return BadRequest("Silme işlemi gerçekleştirilirken bir hata oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API Silme işleminde KAYITKODU boş geçildi.");
                return BadRequest("Kullanıcı Parametreeri Boş Geçilemez!");
            }
        }

        [HttpGet]
        public IHttpActionResult KullaniciGetir(Kullanicilar kullanici)
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


            //Veri tabanı ve ID'ye göre veri getirme işlemleri
            if (kullanici.ID != 0)
            {
                object parametreler = new
                {
                    @ID = kullanici.ID
                };

                Kullanicilar getir = _repository.QueryFirstOrDefault<Kullanicilar>("sp_KullaniciIDGetir", parametreler);

                if (getir != null)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API IP'ye göre kullanıcı getirildi.");
                    return Ok(getir);
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API IP'ye göre kullanıcı bulunamadı.");
                    return BadRequest("Kullanıcı Bulunamadı!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API IP'ye kullanıcı parametreleri boş geçildi.");
                return BadRequest("IP boş geçilemez!");
            }
        }

        [HttpPut]
        public IHttpActionResult KullaniciGuncelle(Kullanicilar kullanici)
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
            if (!string.IsNullOrEmpty(kullanici.KULLANICIADI) && !string.IsNullOrEmpty(kullanici.ROL) && !string.IsNullOrEmpty(kullanici.SIFRE) && kullanici.ID != 0)
            {
                object parametreler = new
                {
                    @KULLANICIADI = kullanici.KULLANICIADI,
                    @SIFRE = kullanici.SIFRE,
                    @ROL = kullanici.ROL,
                    @ID = kullanici.ID
                };

                int guncelle = _repository.Execute("sp_KullaniciGuncelle", parametreler);
                if (guncelle > 0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi başarılı.");
                    return Ok("Güncelleme işlemi başarılı.");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi gerçekleştirilirken bir hata oluştu.");
                    return BadRequest("Kullanıcı güncellenirken bir hata oluştu.");
                }

            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme parametreleri boş bırakıldı.");
                return BadRequest("Kullanıcı bilgileri boş bırakılamaz.");
            }
        }
    }
}
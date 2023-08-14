using DataAccessLayer.GenericRepository;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestAPI2.Loglama;

namespace TestAPI2.Controllers
{
    [Authorize]
    public class CalisanlarController : ApiController
    {
        private readonly DapperGenericRepository _repository;
        private readonly TestLog _log;

        public CalisanlarController()
        {
            _repository = new DapperGenericRepository();
            _log = new TestLog();
        }

        [HttpGet]
        public IEnumerable<Calisanlar> Listele()
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
            return _repository.Query<Calisanlar>("sp_CalisanlarListele");
        }

        [Authorize(Roles = "Sistem Yöneticisi,AdminC")]
        [HttpPost]
        public IHttpActionResult CalisanEkle(Calisanlar calisan)
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

            if (!string.IsNullOrEmpty(calisan.ADSOYAD) && !string.IsNullOrEmpty(calisan.DEPARTMAN))
            {
                calisan.KAYITKODU = Guid.NewGuid().ToString();
                calisan.AKTIF = 1;

                object parametreler = new
                {
                    @KAYITKODU = calisan.KAYITKODU,
                    @ADSOYAD = calisan.ADSOYAD,
                    @DEPARTMAN = calisan.DEPARTMAN,
                    @AKTIF = calisan.AKTIF
                };

                int ekle = _repository.Execute("sp_CalisanlarEkle", parametreler);
                if (ekle > 0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapıldı.");
                    return Ok("Çalışan başarıyla eklendi!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Çalışan Eklenirken Bir Hata Oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken parametreler boş bırakıldı.");
                return BadRequest("Lütfen Çalışan Bilgilerini Doldurun!");
            }
        }

        [Authorize(Roles = "Sistem Yöneticisi")]
        [HttpPut]
        public IHttpActionResult CalisanSil(Calisanlar calisan)
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

            if (!string.IsNullOrEmpty(calisan.KAYITKODU))
            {
                object parametreler = new
                {
                    @KAYITKODU = calisan.KAYITKODU
                };

                int sil = _repository.Execute("sp_CalisanlarSil", parametreler);

                if (sil > 0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapıldı.");
                    return Ok("Çalışan başarıyla silindi!");

                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Çalışan silinirken bir hata oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapılırken parametreler boş bırakıldı.");
                return BadRequest("Çalışan silinirken parametreler boş bırakılamaz!");
            }

        }

        [HttpGet]
        public IHttpActionResult CalisanGetir(Calisanlar casilanlar)
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

            if (!string.IsNullOrEmpty(casilanlar.KAYITKODU))
            {
                object parametreler = new
                {
                    @KAYITKODU = casilanlar.KAYITKODU
                };

                Calisanlar getir = _repository.QueryFirstOrDefault<Calisanlar>("sp_CalisanlarGetir", parametreler);
                if (getir != null)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API IP'ye göre veri getirme işlemi yapıldı.");
                    return Ok(getir);
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API IP'ye göre veri getirme işlemi gerçekleştirilirken bir hata oluştu.");
                    return BadRequest("Çalışan getirilirken bir hata oluştu!");

                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API IP'ye göre veri getirme işleminde KAYITKODU boş bırakıldı.");
                return BadRequest("Çalışan getirilirken parametreler boş bırakılamaz!");
            }
        }

        [Authorize(Roles = "Sistem Yöneticisi,AdminC")]
        [HttpPut]
        public IHttpActionResult CalisanGuncelle(Calisanlar calisanlar)
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

            if (!string.IsNullOrEmpty(calisanlar.DEPARTMAN) && !string.IsNullOrEmpty(calisanlar.ADSOYAD) && !string.IsNullOrEmpty(calisanlar.KAYITKODU))
            {
                object parametreler = new
                {
                    @DEPARTMAN = calisanlar.DEPARTMAN,
                    @ADSOYAD = calisanlar.ADSOYAD,
                    @KAYITKODU = calisanlar.KAYITKODU
                };

                int guncelle = _repository.Execute("sp_CalisanlarGuncelle", parametreler);
                if (guncelle > 0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapıldı.");
                    return Ok("Çalışan güncelleme işlemi başarıyla gerçekleşti!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi gerçekleştirilirken bir hata oluştu.");
                    return BadRequest("Çalışan güncellenirken bir hata oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi gerçekleştirilirken parametreler boş bırakıldı.");
                return BadRequest("Çalışan gücellenirken parametreler boş bırakılamaz!");
            }
        }
    }
}
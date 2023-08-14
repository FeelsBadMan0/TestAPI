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
    public class MaaslarController : ApiController
    {
        private readonly DapperGenericRepository _repository;
        private readonly TestLog _log;

        public MaaslarController()
        {
            _repository = new DapperGenericRepository();
            _log = new TestLog();
        }

        [HttpGet]
        public IEnumerable<Maaslar> Listele()
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
            return _repository.Query<Maaslar>("sp_MaasListele");
        }

        [Authorize(Roles = "Sistem Yöneticisi,AdminC")]
        [HttpPost]
        public IHttpActionResult MaasEkle(Maaslar maaslar)
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

            if(!string.IsNullOrEmpty(maaslar.CALISAN) && !string.IsNullOrEmpty(maaslar.DEPARTMAN) && maaslar.MAAS != 0)
            {
                maaslar.KAYITKODU = Guid.NewGuid().ToString();

                object parametreler = new
                {
                    @KAYITKODU= maaslar.KAYITKODU,
                    @CALISAN=maaslar.CALISAN,
                    @DEPARTMAN=maaslar.DEPARTMAN,
                    @MAAS=maaslar.MAAS
                };

                int ekle = _repository.Execute("sp_MaasEkle", parametreler);
                if (ekle >0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapıldı.");
                    return Ok("Maaş başarıyla eklendi!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Maaş Eklenirken Bir Hata Oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken parametreler boş bırakıldı.");
                return BadRequest("Lütfen Maaş Bilgilerini Doldurun!");
            }
        }

        [Authorize(Roles = "Sistem Yöneticisi")]
        [HttpDelete]
        public IHttpActionResult MaasSil(Maaslar maas)
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

            if(!string.IsNullOrEmpty(maas.KAYITKODU)) 
            {
                object parametreler = new
                {
                    @KAYITKODU = maas.KAYITKODU
                };

                int sil = _repository.Execute("sp_MaasSil", parametreler);

                if(sil>0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapıldı.");
                    return Ok("Maaş başarıyla silindi!");

                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Maaş silinirken bir hata oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapılırken parametreler boş bırakıldı.");
                return BadRequest("Maaş silinirken parametreler boş bırakılamaz!");
            }

        }

        
        [HttpGet]
        public IHttpActionResult MaasGetir(Maaslar maas)
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

            if(!string.IsNullOrEmpty(maas.KAYITKODU)) 
            {
                object parametreler = new
                {
                    @KAYITKODU=maas.KAYITKODU
                };

                Maaslar getir = _repository.QueryFirstOrDefault<Maaslar>("sp_MaasGetir", parametreler);
                if (getir != null)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API IP'ye göre veri getirme işlemi yapıldı.");
                    return Ok(getir);
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API IP'ye göre veri getirme işlemi gerçekleştirilirken bir hata oluştu.");
                    return BadRequest("Maaş getirilirken bir hata oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API IP'ye göre veri getirme işleminde KAYITKODU boş bırakıldı.");
                return BadRequest("Maaş getirilirken parametreler boş bırakılamaz!");
            }
        }

        [Authorize(Roles = "Sistem Yöneticisi,AdminC")]
        [HttpPut]
        public IHttpActionResult MaasGuncelle(Maaslar maas)
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

            if(!string.IsNullOrEmpty(maas.DEPARTMAN) && !string.IsNullOrEmpty(maas.CALISAN) && maas.MAAS != 0 && !string.IsNullOrEmpty(maas.KAYITKODU))
            {
                object parametreler = new
                {
                    @DEPARTMAN= maas.DEPARTMAN,
                    @CALISAN= maas.CALISAN,
                    @MAAS=maas.MAAS,
                    @KAYITKODU=maas.KAYITKODU
                };

                int guncelle = _repository.Execute("sp_MaasGuncelle", parametreler);
                if(guncelle > 0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapıldı.");
                    return Ok("Maaş güncelleme işlemi başarıyla gerçekleşti!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi gerçekleştirilirken bir hata oluştu.");
                    return BadRequest("Maaş güncellenirken bir hata oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi gerçekleştirilirken parametreler boş bırakıldı.");
                return BadRequest("Maaş gücellenirken parametreler boş bırakılamaz!");
            }
        }
    }
}

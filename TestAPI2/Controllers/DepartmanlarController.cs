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
    public class DepartmanlarController : ApiController
    {
        private readonly DapperGenericRepository _repository;
        private readonly TestLog _log;

        public DepartmanlarController()
        {
            _repository = new DapperGenericRepository();
            _log = new TestLog();
        }

        [HttpGet]
        public IEnumerable<Departmanlar> Listele()
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
            return _repository.Query<Departmanlar>("sp_DepartmanListele");
        }

        [Authorize(Roles = "Sistem Yöneticisi,AdminC")]
        [HttpPost]
        public IHttpActionResult DepartmanEkle(Departmanlar departman)
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

            if (!string.IsNullOrEmpty(departman.DEPARTMAN) && departman.CALISANSAYISI!=0)
            {
                departman.KAYITKODU = Guid.NewGuid().ToString();

                object parametreler = new
                {
                    @KAYITKODU = departman.KAYITKODU,
                    @CALISANSAYISI = departman.CALISANSAYISI,
                    @DEPARTMAN = departman.DEPARTMAN,
                };

                int ekle = _repository.Execute("sp_DepartmanEkle", parametreler);
                if (ekle > 0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapıldı.");
                    return Ok("Departman başarıyla eklendi!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Departman Eklenirken Bir Hata Oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken parametreler boş bırakıldı.");
                return BadRequest("Departman Çalışan Bilgilerini Doldurun!");
            }
        }

        [HttpGet]
        public IHttpActionResult DepartmanGetir(Departmanlar departman)
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

            if (!string.IsNullOrEmpty(departman.KAYITKODU))
            {
                object parametreler = new
                {
                    @KAYITKODU = departman.KAYITKODU
                };

                Departmanlar getir = _repository.QueryFirstOrDefault<Departmanlar>("sp_DepartmanGetir", parametreler);
                if (getir != null)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API IP'ye göre veri getirme işlemi yapıldı.");
                    return Ok(getir);
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API IP'ye göre veri getirme işlemi gerçekleştirilirken bir hata oluştu.");
                    return BadRequest("Departman getirilirken bir hata oluştu!");

                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API IP'ye göre veri getirme işleminde KAYITKODU boş bırakıldı.");
                return BadRequest("Departman getirilirken parametreler boş bırakılamaz!");
            }
        }

        [Authorize(Roles = "Sistem Yöneticisi,AdminC")]
        [HttpPut]
        public IHttpActionResult DepartmanGuncelle(Departmanlar departman)
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

            if (!string.IsNullOrEmpty(departman.DEPARTMAN) && !string.IsNullOrEmpty(departman.KAYITKODU) && departman.CALISANSAYISI!=0)
            {
                object parametreler = new
                {
                    @DEPARTMAN = departman.DEPARTMAN,
                    @CALISANSAYISI = departman.CALISANSAYISI,
                    @KAYITKODU = departman.KAYITKODU
                };

                int guncelle = _repository.Execute("sp_DepartmanGuncelle", parametreler);
                if (guncelle > 0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapıldı.");
                    return Ok("Departman güncelleme işlemi başarıyla gerçekleşti!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi gerçekleştirilirken bir hata oluştu.");
                    return BadRequest("Departman güncellenirken bir hata oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi gerçekleştirilirken parametreler boş bırakıldı.");
                return BadRequest("Departman gücellenirken parametreler boş bırakılamaz!");
            }
        }
    }
}

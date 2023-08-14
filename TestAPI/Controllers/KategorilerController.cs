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
    public class KategorilerController : ApiController
    {
        private readonly DapperGenericRepository _repository;
        private readonly TestLog _log;

        public KategorilerController()
        {
            _repository = new DapperGenericRepository();
            _log = new TestLog();
        }



        [HttpGet]
        public IEnumerable<Kategoriler> Listele()
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
            return _repository.Query<Kategoriler>("sp_KategoriGetir");
        }

        [Authorize(Roles = "Sistem Yöneticisi,AdminS")]
        [HttpPost]
        public IHttpActionResult KategoriEkle(Kategoriler kategori)
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
            if (kategori.KATEGORI != null && kategori.KATEGORI != "")
            {
                string kayitKodu = kategori.KAYITKODU = Guid.NewGuid().ToString();
                short aktif = kategori.AKTIF = 1;
                kategori.AKTIF = 1;

                object parametreler = new
                {
                    @KATEGORI = kategori.KATEGORI,
                    @KAYITKODU = kategori.KAYITKODU,
                    @AKTIF = kategori.AKTIF
                };

                int kategoriEkle = _repository.Execute("sp_KategoriEkle", parametreler);
                if (kategoriEkle == 1)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapıldı.");
                    return Ok("Kategori Başarıyla Eklendi!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Kategori Eklenrken Bir Hata Oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken parametreler boş bırakıldı.");
                return BadRequest("Lütfen Kategori Bilgilerini Boş Bırakmayın!");
            }
        }

        [Authorize(Roles = "Sistem Yöneticisi")]
        [HttpPut]
        public IHttpActionResult Sil(Kategoriler kategori)
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
            if (kategori.KAYITKODU != null && kategori.KAYITKODU != "")
            {
                object parametreler = new
                {
                    @KAYITKODU = kategori.KAYITKODU
                };

                int kategoriSil = _repository.Execute("sp_KategoriSil", parametreler);
                if (kategoriSil == 1)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapıldı.");
                    return Ok("Kategori Başarıya Silindi!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Böyle Bir Kullanıcı Yok!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API silme işlemi yapılırken KAYITKODU boş bırakıldı.");
                return BadRequest("Kayıt Kodunu Boş Bırakmayın!");
            }
        }

        [HttpGet]
        public IHttpActionResult KategoriGetir(Kategoriler kategori)
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
            if (kategori.ID != 0)
            {
                object parametreler = new
                {
                    @ID = kategori.ID
                };

                Kategoriler kategoriGetir = _repository.QueryFirstOrDefault<Kategoriler>("sp_KategoriIDGetir", parametreler);
                if (kategoriGetir != null)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ID'ye göre veri getirme işlemi yapıldı.");
                    return Ok(kategoriGetir);
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ID'ye göre veri getirme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Kullanıcı Bulunamadı!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API ID'ye göre veri getirme işleminde ID boş bırakıldı.");
                return BadRequest("ID alanını boş bırakmayın!");
            }
        }

        [Authorize(Roles = "Sistem Yöneticisi,AdminS")]
        [HttpPut]
        public IHttpActionResult KategoriGuncelle(Kategoriler kategori)
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
            if (kategori.ID != 0)
            {
                if (kategori.KATEGORI != null && kategori.KATEGORI != "" && kategori.ID != 0)
                {
                    object parametreler = new
                    {
                        @ID = kategori.ID,
                        @KATEGORI = kategori.KATEGORI
                    };

                    int guncelle = _repository.Execute("sp_KategoriGuncelle", parametreler);
                    if (guncelle > 1)
                    {
                        _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapıldı.");
                        return Ok("Kategori Başarıyla Güncellendi!");
                    }
                    else
                    {
                        _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapılırken bir hata oluştu.");
                        return BadRequest("Kategori Eklenirken Bir Hata Oluştu!");
                    }
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapılırken parametre boş bırakıldı.");
                    return BadRequest("Kategori Adı Boş Bırakılamaz!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API güncelleme işlemi yapılırken ID boş bırakıldı.");
                return BadRequest("ID Alanı Boş Bırakılamaz!");
            }
        }
    }
}
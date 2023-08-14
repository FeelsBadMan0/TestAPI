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
    public class SatisHareketleriController : ApiController
    {
        private readonly DapperGenericRepository _repository;
        private readonly TestLog _log;
        public SatisHareketleriController()
        {
            _repository = new DapperGenericRepository();
            _log = new TestLog();
        }


        [HttpGet]
        public IEnumerable<SatisHareketleri> Listele()
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
            return _repository.Query<SatisHareketleri>("sp_SatisGetir");
        }


        [Authorize(Roles = "Sistem Yöneticisi,AdminS")]
        [HttpPost]
        public IHttpActionResult SatisEkle(SatisHareketleri satis)
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
            if (satis.CARI != null && satis.CARI != "" && satis.URUN != null && satis.URUN != "" && satis.FIYAT != 0 && satis.MIKTAR != 0)
            {
                satis.KAYITKODU = Guid.NewGuid().ToString();
                satis.SATISTARIHI = DateTime.Now;

                object parametreler = new
                {
                    @URUN = satis.URUN,
                    @KAYITKODU = satis.KAYITKODU,
                    @CARI = satis.CARI,
                    @MIKTAR = satis.MIKTAR,
                    @FIYAT = satis.FIYAT,
                    @SATISTARIHI = satis.SATISTARIHI
                };

                int ekle = _repository.Execute("sp_SatisEkle", parametreler);
                if (ekle != 0)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapıldı.");
                    return Ok("Satış Ekleme İşlemi Baarılı!");
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Satış Yapılırken Bir Hata Oluştu!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API ekleme işlemi yapılırken parametreler boş bırakıldı.");
                return BadRequest("Satış Alanları Boş Bırakılamaz!");
            }
        }



        [HttpGet]
        public IHttpActionResult SatisGetir(SatisHareketleri satis)
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


            //Veri tabanı ve ID'ye gre veri çekme işlemleri
            if (satis.ID != 0)
            {
                object parametreler = new
                {
                    @ID = satis.ID,
                };
                SatisHareketleri getir = _repository.QueryFirstOrDefault<SatisHareketleri>("sp_SatisIDGetir", parametreler);
                if (getir != null)
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ID'ye veri çekme işlemi yapıldı.");
                    return Ok(getir);
                }
                else
                {
                    _log.Loglama(controllerAdi, actionAdi, ip, "API ID'ye veri çekme işlemi yapılırken bir hata oluştu.");
                    return BadRequest("Böyle Bir Satış Bulunamadı!");
                }
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "API ID'ye veri çekme işlemi yapılırken ID boş bırakıldı.");
                return BadRequest("Böyle Bir Satış ID'si Bulunamadı!");
            }
        }
    }
}
using DataAccessLayer.GenericRepository;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestAPI.Hash;
using TestAPI.JWT;
using TestAPI.Loglama;

namespace TestAPI.Controllers
{
    [AllowAnonymous]
    public class LoginController : ApiController
    {
        private readonly DapperGenericRepository _repository;
        private readonly TestLog _log;
        private readonly ParolaSifreleme _sifrele;
        public LoginController()
        {
            _repository = new DapperGenericRepository();
            _log = new TestLog();
            _sifrele = new ParolaSifreleme();
        }


        [HttpPost]
        public IHttpActionResult GirisYap(Kullanicilar kullanici)
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



            //Kullanıcı girişi ve token işlemleri
            object parametreler = new
            {
                @KULLANICIADI = kullanici.KULLANICIADI
            };

            Kullanicilar kullanicilar = _repository.QueryFirstOrDefault<Kullanicilar>("sp_KullaniciKontrol", parametreler);

            if (_sifrele.SifreDogrulama(kullanici.SIFRE, kullanicilar.PAROLASIFRELE, kullanicilar.PAROLATUZLA))
            {
                var token = JwtManeger.GetToken(kullanicilar.KULLANICIADI, kullanicilar.ROL, kullanicilar.AKTIF, kullanicilar.ID);
                _log.Loglama(controllerAdi, actionAdi, ip, "Kullanıcı girişi yapıldı.");
                return Ok(new { kullanici.KULLANICIADI, kullanici.SIFRE, token });
            }
            else
            {
                _log.Loglama(controllerAdi, actionAdi, ip, "Kullanıcı bilgileri yanlış girildi .");
                return BadRequest("Kullanıcı Adı veya Şifre yanlış!");
            }
        }
    }
}

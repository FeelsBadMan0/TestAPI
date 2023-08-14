using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace TestAPI.JWT
{
    public class JwtManeger
    {
        public static Object GetToken(string userName, string rol, short aktif, int id)
        {
            //Jwt keyimizi Web.config'den çekiyoruz.
            var key = ConfigurationManager.AppSettings["JwtKey"];
            // Jwt Issuerimizi Web.config'den çekiyoruz. Issuer tokeni veren kişi.Audience ise tokenin alıcısıdır.
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];

            // Web.config'de belirttiğimiz Jwt keyi SymmetricSecurityKey ile şifreleyerek byte tipine alıyoruz.
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            //Byte tipindeki Jwt keyimizi HmacSha256 tipinde şifreliyoruz.
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Kullanıcı bilgilerini API'de tutabilmek için Session yerine Claim kullanıp değerlerini saklıyoruz.
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim(ClaimTypes.Name, userName)); //Kullanıcı adını alıyouz
            permClaims.Add(new Claim(ClaimTypes.Role, rol));     // Rol bilgisini alıyoruz
            permClaims.Add(new Claim("ID", aktif.ToString()));  //ID bilgisini alıyoruz
            permClaims.Add(new Claim("AKTIF", id.ToString()));  //Kullanıcının Aktif bilgisini alıyoruz.

            //Aldığımız değerleri JWT'nin içine atıyoruz.
            var token = new JwtSecurityToken(issuer, //Issuer değerini ekliyoruz.  
                            issuer,  //Audience değerini de issuer ile aynı tutuyoruz, sebebi localde çalışdığımız için pek bir önemi yok.  
                            permClaims, //Kullanıcıdan aldığımız bilgileri ekliyoruz.
                            expires: DateTime.Now.AddDays(1), //JWT'nin ne kadar geçerli olacağını belirtiyoruz.
                            signingCredentials: credentials); //Şifrelediğimiz Securtiy Key'i ekliyoruz.
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token); // Gelen JWT değerini JwtSecurityTokenHandler ile yazdırıyoruz.
            return new { data = jwt_token }; // Gelen değeri geriye dönüyoruz.
        }
    }
}
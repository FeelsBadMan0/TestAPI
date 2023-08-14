using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TestAPI2.Hash
{
    public class ParolaSifreleme
    {
        public void ParolaSifrele(string parola, out byte[] sifreliparola, out byte[] tuzlama)
        {
            using (var hmac = new HMACSHA512())
            {
                tuzlama = hmac.Key;
                sifreliparola = hmac.ComputeHash(Encoding.UTF8.GetBytes(parola));
            }
        }

        public bool SifreDogrulama(string parola, byte[] sifreliparola, byte[] tuzlama)
        {
            using (var hmac = new HMACSHA512(tuzlama))
            {
                var sifrele = hmac.ComputeHash(Encoding.UTF8.GetBytes(parola));
                return sifrele.SequenceEqual(sifreliparola);
            }
        }
    }
}
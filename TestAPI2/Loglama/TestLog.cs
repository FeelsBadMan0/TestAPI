using DataAccessLayer.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestAPI2.Loglama
{
    public class TestLog
    {
        private readonly DapperGenericRepository _repository;

        public TestLog()
        {
            _repository = new DapperGenericRepository();
        }
        public void Loglama(string controller, string action, string ip, string kod)
        {

            object parametreler = new
            {
                @CONTROLLER = controller,
                @ACTION = action,
                @IPADRES = ip,
                @ISLEM = kod,
                @TARIH = DateTime.Now
            };
            _repository.Execute("sp_TestogEkle", parametreler);
        }
    }
}

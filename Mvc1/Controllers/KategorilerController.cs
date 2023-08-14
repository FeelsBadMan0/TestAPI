using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc1.Controllers
{
    public class KategorilerController : Controller
    {
        // GET: Kategoriler
        public ActionResult Index()
        {
            return View();
        }
    }
}
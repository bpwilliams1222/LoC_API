using LoCWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoCWebApp.Controllers
{
    public class MembersController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Members
        public ActionResult Index()
        {
            return View();
        }
    }
}
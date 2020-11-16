using DoubleKPhoneShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoubleKPhoneShop.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult ListGuest()
        {
            var order = db.Orders;
            var user = db.Users.Where(u => u.Email != "firstAdmin@gmail.com").Include(u=>u.Orders);
            return View(user);
        }
    }
}
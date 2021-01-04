using DoubleKPhoneShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DoubleKPhoneShop.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult ListGuest()
        {
            var order = db.Orders;
            var user = db.Users.Where(u => u.Email != "firstAdmin@gmail.com").Include(u=>u.Orders).ToList();             
            return View(user);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult SendGuestCode(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }            
            ViewBag.UserId = new SelectList(db.Users.Where(u=>u.Id == id),"Id","FullName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult SendGuestCode([Bind(Include = "CodeID,Code,UseDate,Value,UserId")] PromotionCodeModel promotionCode,string id)
        {
            if (ModelState.IsValid)
            {
                db.PromotionCodeModels.Add(promotionCode);
                db.SaveChanges();
                return RedirectToAction("ListGuest", "Admin");
            }
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.Id == id), "Id", "FullName");
            return View(promotionCode);
        }

        public ActionResult CreateCode()
        {
            var model = db.PromotionCodeModels;
            //var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            //var stringChars = new char[8];
            //var random = new Random();

            //for (int i = 0; i < stringChars.Length; i++)
            //{
            //    stringChars[i] = chars[random.Next(chars.Length)];
            //}            
            //var finalString = new String(stringChars);
            //var code = new PromotionCodeModel();
            //code.Code = finalString;
            //db.PromotionCodeModels.Add(code);           
            //db.SaveChanges();
            return View(model);
        }
    }
}
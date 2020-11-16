using DoubleKPhoneShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DoubleKPhoneShop.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //public ActionResult Test()
        //{         
        //    var products = db.Products.ToList();
        //    return View(products);
        //}
        //[HttpPost]
        //public ActionResult Test(string find)
        //{
        //    var products = db.Products.Where(p=>p.Category.CategoryName == find).ToList();
        //    return PartialView("_Product", products);
        //}
        public PartialViewResult productview;
        public ActionResult Index(string category)
        {
            //db.Configuration.ProxyCreationEnabled = false;
            ViewModel model = new ViewModel();
            var b = db.Products.Where(p => p.Category.CategoryName == category);
            model.Categories = db.Categories.ToList();
            var products = db.Products.ToList();
            model.Products = products;
            if (category != null)
            {
                model.Products = db.Products.Where(u => u.Category.CategoryName == category).ToList();
                //productview = PartialView("_Products", model);
            }
            ViewBag.Phone = db.Products.ToList();
            return View(model);
        }
        public ActionResult Test(string category)
        {
            var product = db.Products.Where(p => p.Category.CategoryName == category);
            return RedirectToAction("Index", "Home", HttpMethod.Post);         
        }

        [HttpPost]
        public ActionResult Index(string category, double? price, string order)
        {
            //db.Configuration.ProxyCreationEnabled = false;
            ViewModel model = new ViewModel();            
            model.Categories = db.Categories.ToList();
            var products = db.Products.ToList();
            model.Products = products;
            if (category!=null)
            {
                model.Products = db.Products.Where(u => u.Category.CategoryName == category).ToList();
            }                       
            if (price != 0)
            {
                //var product = db.Products.ToList();   
                if (price < 3000000)
                {
                    model.Products = db.Products.Where(p => p.Price < price).ToList();
                }
                if (price == 3000000)
                {
                    model.Products = db.Products.Where(p => p.Price >= price && p.Price <= 7000000).ToList();
                }
                if (price == 7000000)
                {
                    model.Products = db.Products.Where(p => p.Price >= price && p.Price <= 10000000).ToList();
                }
                if (price == 10000000)
                {
                    model.Products = db.Products.Where(p => p.Price >= price).ToList();
                }
                if (price == 20000000)
                {
                    model.Products = db.Products.Where(p => p.Price >= price).ToList();
                }            
            }
            if (price == 0)
            {
                model.Products = db.Products.ToList();    
            }
            if (order != null)
            {
                if (order == "down")
                {
                    model.Products = model.Products.OrderByDescending(o => o.Price).ToList();
                }
                else
                {
                    model.Products = model.Products.OrderBy(o => o.Price).ToList();
                }
            }
            productview = PartialView("_Products", model);
            return productview;
            //return PartialView("_Products", model);            
        }
    }
}
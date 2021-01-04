using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DoubleKPhoneShop.Models;

namespace DoubleKPhoneShop.Controllers
{    
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        

        // GET: Product
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }
        [Authorize]
        public ActionResult Rating(double rate,int id)
        {
            var product = db.Products.Include(p => p.Category).Where(p=>p.ProductId == id).First();
            if (product.Rate != 0)
            {
                product.Rate = (product.Rate + rate) / 2;
            }
            else
                product.Rate = (product.Rate + rate);
            product.RateCount = product.RateCount + 1;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            ////
            //var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            //var stringChars = new char[8];
            //var random = new Random();

            //for (int i = 0; i < stringChars.Length; i++)
            //{
            //    stringChars[i] = chars[random.Next(chars.Length)];
            //}

            //var finalString = new String(stringChars);
            ////
            return View(product);
        }

        public ActionResult Find(string category,string key)
        {
            var product = db.Products.ToList();
            if(key != "")
            {
                product = db.Products.Where(p => p.ProductName.Contains(key)).ToList();
            }
            if(category!=null)
            {
                product = db.Products.Where(p => p.Category.CategoryName == category).ToList();
            }
            if(key== "" && category == null)
            {
                product = null;
            }
            return View(product);
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Product/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.date = DateTime.Now;
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "ProductId,ProductName,Description,Image,Price,Quantity,Color,CreateDate,CategoryId,Screen,OperatingSystem,Cameras,CPU,RAM,ROM,Batery")] Product product,string color, HttpPostedFileBase Image,string status)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Image != null)
                    {
                        var fileName = Path.GetFileName(Image.FileName);
                        product.Image = fileName;
                        string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                        Image.SaveAs(path);
                    }
                    else product.Image = "Icon.png";
                }
                catch (Exception e)
                {
                    Response.Write(e.Message);
                }
                product.CreateDate = DateTime.Now;
                product.Color = color;
                product.Status = status;
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            ViewBag.date = (DateTime)product.CreateDate;
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,Description,Image,Price,Quantity,Color,s,CategoryId,Screen,OperatingSystem,Cameras,CPU,RAM,ROM,Batery")] Product product,int? id, string color, HttpPostedFileBase Image,string status)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Image != null)
                    {
                        var fileName = Path.GetFileName(Image.FileName);
                        product.Image = fileName;
                        string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                        Image.SaveAs(path);
                    }
                    else product.Image = db.Products.Where(o => o.ProductId == id).Select(o => o.Image).FirstOrDefault();
                }
                catch (Exception e)
                {
                    Response.Write(e.Message);
                }
                product.CreateDate = db.Products.Where(o => o.ProductId == id).Select(o => o.CreateDate).FirstOrDefault();                
                product.Color = color;
                product.Status = status;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Product/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using DoubleKPhoneShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoubleKPhoneShop.Controllers
{
    public class CompareController : Controller
    {
        // GET: Compare
        private ApplicationDbContext db = new ApplicationDbContext();
        private const string CompareSession = "CompareSession";
        public ActionResult Index()
        {
            var com = Session[CompareSession];
            var list = new List<CompareModel>();
            if (com != null)
            {
                list = (List<CompareModel>)com;
            }
            ViewBag.Phone = db.Products.ToList();
            return View(list);
        }
        //public JsonResult GetGuestList(string GuestPhone)
        //{
        //    db.Configuration.ProxyCreationEnabled = false;
        //    var list = db.Guests.Where(u => u.PhoneNumber == GuestPhone);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
        private static int count = 0;
        public ActionResult AddProduct(int id)
        {            
            Product product = db.Products.Find(id);  // tim sp theo sanPhamID
            var com = Session[CompareSession];
            if (com != null)
            {
                var list = (List<CompareModel>)com;
                if (!list.Exists(x => x.Product.ProductId == id))
                {
                    //tạo mới so sánh nếu chưa có
                    var item = new CompareModel();
                    item.Product = product;
                    //if (count >= 2)
                    //{
                    //    list.RemoveAt(0);
                    //    list.Add(item);
                    //    count++;
                    //}
                    if (count<2)
                    {
                        list.Add(item);
                        count++;
                    }
                                   
                }        
                //gán vào session
                Session[CompareSession] = list;
            }
            else
            {
                //tạo mới so sánh nếu chưa có
                var item = new CompareModel();
                item.Product = product;                
                var list = new List<CompareModel>();
                list.Add(item);
                count++;
                //gán vào session
                Session[CompareSession] = list;
            }
            return RedirectToAction("Index");
        }
        //public ActionResult DeleteAll()
        //{
        //    Session[CompareSession] = null;
        //    count = 0;
        //    return View("Index");
        //}

        public ActionResult Delete(int id)
        {
            var com = (List<CompareModel>)Session[CompareSession];
            com.RemoveAll(x => x.Product.ProductId == id);
            count = count-1;
            Session[CompareSession] = com;
            return RedirectToAction("Index");
        }
    }
}
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;

namespace shopOnline.Controllers
{
    public class HomeController : Controller
    {
        menfsEntities db = new menfsEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(Contact contact)
        {
            contact.dateContact = DateTime.Now;
            db.Contacts.Add(contact);
            var result = db.SaveChanges();
            if (result > 0)
            {
                ViewBag.MessageSuccess = "Your message has been received by us. We will contact you as soon as possible.";
            }
            return View(contact);
        }
        public ActionResult Error()
        {
            return View();
        }
        public PartialViewResult ProductHome()
        {
            var list = db.Products.OrderByDescending(model => model.dateCreate).Take(8).ToList();
            return PartialView(list);
        }
        public PartialViewResult BlogHome()
        {
            var list = db.Articles.OrderByDescending(model => model.dateCreate).Take(3).ToList();
            return PartialView(list);
        }
    }
}
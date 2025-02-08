using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;
using PagedList;

namespace ShopOnline.Areas.Admin.Controllers
{
    [Authorize]
    public class CRUDinvoinceController : Controller
    {
        menfsEntities1 db = new menfsEntities1();
        public ActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;
            var orderlist = db.Invoinces.OrderByDescending(model => model.dateCreate).Include(model => model.Member).ToPagedList(pageNumber, pageSize);
            return View(orderlist);
        }
        public ActionResult InvoinceDetail(Guid invoinceNo)
        {

            var infor = db.Invoinces.Where(model => model.invoinceId == invoinceNo).Include(model => model.Member).FirstOrDefault();
            Session["information"] = infor;
            ViewBag.invoinceNo = invoinceNo;
            var detail = db.InvoinceDetails.Where(model => model.invoinceId == invoinceNo).Include(model => model.Product).ToList();
            return View(detail);
        }
        public ActionResult Delete(Guid id)
        {
            List<InvoinceDetail> ctdh = db.InvoinceDetails.Where(model => model.invoinceId == id).ToList();
            foreach (var i in ctdh)
            {
                db.InvoinceDetails.Remove(i);
            }
            db.SaveChanges();
            Invoince invoince = db.Invoinces.Find(id);
            db.Invoinces.Remove(invoince);
            TempData["DeleteOrder"] = "Successfully delete!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult DeliverySuccess(Guid id)
        {
            Invoince invoince = db.Invoinces.Find(id);
            invoince.deliveryDate = DateTime.Now;
            invoince.deliveryStatus = true;
            TempData["Delivery"] = "Order ID " + invoince.invoinceId + " has been successfully delivered ";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ExportPDF()
        {
            return new Rotativa.ActionAsPdf("InvoinceDetail")
            {
                FileName = "Invoice.pdf" // Tùy chọn đặt tên file PDF
            };
        }
    }
}
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Rotativa.Options;
using ShopOnline.Models;
namespace ShopOnline.Areas.Admin.Controllers
{
    public class CRUDinvoinceController : Controller
    {
        menfsEntities db = new menfsEntities();

        [CustomAuthorize("Admin")]
        public ActionResult Index()
        {
            var orderlist = db.Invoinces.OrderByDescending(model => model.dateCreate).ToList();
            return View(orderlist);
        }

        [CustomAuthorize("Admin")]
        public ActionResult InvoinceDetail(Guid invoinceNo)
        {

            Invoince invoince = db.Invoinces.Where(model => model.invoinceId == invoinceNo).FirstOrDefault();
            return View(invoince);
        }

        [CustomAuthorize("Admin")]
        public ActionResult Comfirmed(Guid id)
        {
            Invoince invoince = db.Invoinces.Find(id);
            invoince.status = "comfirmed";
            TempData["Delivery"] = "Order ID " + invoince.invoinceId + " has been comfirmed";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [CustomAuthorize("Admin")]
        public ActionResult Shipping(Guid id)
        {
            Invoince invoince = db.Invoinces.Find(id);
            invoince.status = "shipping";
            TempData["Delivery"] = "Order ID " + invoince.invoinceId + " is shipping";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [CustomAuthorize("Admin")]
        public ActionResult Completed(Guid id)
        {
            Invoince invoince = db.Invoinces.Find(id);
            invoince.status = "completed";
            invoince.paymentStatus = "paid";
            TempData["Delivery"] = "Order ID " + invoince.invoinceId + " has been successfully delivered";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [CustomAuthorize("Admin")]
        public ActionResult Cancel(Guid id)
        {
            Invoince invoince = db.Invoinces.Find(id);
            if (invoince.status == "cancelled" || invoince.status == "completed" || invoince.paymentStatus == "paid")
            {
                TempData["msgCancelOrder"] = "Order ID " + invoince.invoinceId + " isn't cancel";
                return RedirectToAction("Index");
            }
            invoince.status = "cancelled";
            TempData["Delivery"] = "Order ID " + invoince.invoinceId + " has been cancelled";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //public ActionResult Delete(Guid id)
        //{
        //    List<InvoinceDetail> ctdh = db.InvoinceDetails.Where(model => model.invoinceId == id).ToList();
        //    foreach (var i in ctdh)
        //    {
        //        db.InvoinceDetails.Remove(i);
        //    }
        //    db.SaveChanges();
        //    Invoince invoince = db.Invoinces.Find(id);
        //    db.Invoinces.Remove(invoince);
        //    TempData["DeleteOrder"] = "Successfully delete!";
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        [CustomAuthorize("Admin")]
        public ActionResult ExportPDF(Guid id)
        {
            return new Rotativa.ActionAsPdf("PrintInvoince", new { id = id })
            {
                FileName = "Invoice.pdf", // Tùy chọn đặt tên file PDF
                PageSize = Size.A5,
                PageMargins = new Margins(5, 5, 5, 5),
                IsLowQuality = false
            };
        }
        
        public ActionResult PrintInvoince(Guid id)
        {
            Invoince invoince = db.Invoinces.Where(model => model.invoinceId == id).FirstOrDefault();
            return View(invoince);
        }        
    }
}
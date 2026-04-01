using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Rotativa.Options;
using ShopOnline.Models;
namespace ShopOnline.Areas.Admin.Controllers
{
    public class CRUDinvoiceController : Controller
    {
        menfsEntities db = new menfsEntities();

        [CustomAuthorize("Manager")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetInvoice()
        {
            try
            {                
                var listInvoice = db.Invoinces.OrderByDescending(i => i.dateCreate)
                                                      .Select(i => new
                                                      {
                                                          invoiceId = i.invoinceId,
                                                          dateCreate = i.dateCreate,
                                                          customer = i.Member.firstName,
                                                          totalMoney = i.totalMoney,
                                                          paymentMethod = i.paymentMethod,
                                                          paymentStatus = i.paymentStatus,
                                                          note = i.note,
                                                          status = i.status
                                                      }).ToList();

                return Json(new { success = true, listInvoice, msg = "Successfully get list Invoice!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Get list Invoice false: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize("Manager")]
        public ActionResult InvoiceDetail(Guid invoiceNo)
        {
            Invoince invoice = db.Invoinces.Where(model => model.invoinceId == invoiceNo).FirstOrDefault();
            return View(invoice);
        }

        [CustomAuthorize("Manager")]
        public ActionResult Confirmed(Guid id)
        {
            Invoince invoice = db.Invoinces.Find(id);
            invoice.status = "confirmed";
            TempData["Delivery"] = "Order ID " + invoice.invoinceId + " has been confirmed";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [CustomAuthorize("Manager")]
        public ActionResult Shipping(Guid id)
        {
            Invoince invoice = db.Invoinces.Find(id);
            invoice.status = "shipping";
            TempData["Delivery"] = "Order ID " + invoice.invoinceId + " is shipping";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [CustomAuthorize("Manager")]
        public ActionResult Completed(Guid id)
        {
            Invoince invoice = db.Invoinces.Find(id);
            invoice.status = "completed";
            invoice.paymentStatus = "paid";
            TempData["Delivery"] = "Order ID " + invoice.invoinceId + " has been successfully delivered";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [CustomAuthorize("Manager")]
        public ActionResult Cancel(Guid id)
        {
            Invoince invoice = db.Invoinces.Find(id);
            if (invoice.status == "cancelled" || invoice.status == "completed" || invoice.paymentStatus == "paid")
            {
                TempData["msgCancelOrder"] = "Order ID " + invoice.invoinceId + " isn't cancel";
                return RedirectToAction("Index");
            }
            invoice.status = "cancelled";
            TempData["Delivery"] = "Order ID " + invoice.invoinceId + " has been cancelled";
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

        [CustomAuthorize("Manager")]
        public ActionResult ExportPDF(Guid id)
        {
            return new Rotativa.ActionAsPdf("PrintInvoice", new { id = id })
            {
                FileName = "Invoice.pdf", // Tùy chọn đặt tên file PDF
                PageSize = Size.A5,
                PageMargins = new Margins(5, 5, 5, 5),
                IsLowQuality = false
            };
        }
        
        public ActionResult PrintInvoice(Guid id)
        {
            Invoince invoice = db.Invoinces.Where(model => model.invoinceId == id).FirstOrDefault();
            return View(invoice);
        }        
    }
}
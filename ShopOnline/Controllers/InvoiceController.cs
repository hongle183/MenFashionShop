using ShopOnline.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace shopOnline.Controllers
{
    [CustomAuthorize("Customer Member")]
    public class InvoiceController : Controller
    {
        private readonly menfsEntities db = new menfsEntities();
        [HttpGet]
        public ActionResult MyOrder(string status)
        {
            Member user = (Member)Session["info"];
            var orders = db.Invoinces.OrderByDescending(model => model.dateCreate).Where(model => model.memberId == user.memberId);

            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(model => model.status == status);
            }
            return View(orders.ToList());
        }
        [HttpGet]
        public PartialViewResult GetOrdersByStatus(string status)
        {
            Member user = (Member)Session["info"];
            var orders = db.Invoinces.Where(model => model.memberId == user.memberId);

            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                orders = orders.Where(model => model.status == status);
            }

            return PartialView("_ListOrder", orders.ToList());
        }
        [HttpGet]
        public ActionResult InvoiceDetail(string invoiceId)
        {
            if (!Guid.TryParse(invoiceId, out Guid parsedId))
            {
                return RedirectToAction("MyOrder", "Invoice");
            }
            var orders = db.Invoinces.FirstOrDefault(model => model.invoinceId == parsedId);
            return View(orders);
        }
        public ActionResult Cancel(Guid id)
        {
            Invoince invoice = db.Invoinces.FirstOrDefault(model => model.invoinceId == id);
            if (invoice.status == "cancelled" || invoice.status == "completed" || invoice.paymentStatus == "paid")
            {
                TempData["msgCancelOrderFailed"] = "Order ID " + invoice.invoinceId + " không thể hủy!";
                return RedirectToAction("MyOrder", "Invoice");
            }
            invoice.status = "cancelled";
            TempData["msgCancelOrder"] = "Order ID " + invoice.invoinceId + " đã được hủy.";
            db.SaveChanges();
            return RedirectToAction("MyOrder", "Invoice");
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
        //    TempData["msgDeleteOrder"] = "Successfully delete order!";
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
    }
}
        
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;

namespace ShopOnline.Areas.Admin.Controllers
{
    [Authorize]
    public class CRUDcustomerController : Controller
    {
        menfsEntities1 db = new menfsEntities1();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult DsCustomer()
        {
            try
            {
                var dsCustomer = (from i in db.Members
                                  orderby i.lastName ascending
                                  select new { i.memberId, i.firstName, i.lastName, i.email, i.phone, i.address, i.dateCreate }).ToList();
                return Json(new { code = 200, dsCustomer, msg = "Successfully get list Customer!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Get list Customer false: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult Delete(Guid customerId)
        {
            try
            {
                Member customer = (from i in db.Members select i).SingleOrDefault(model => model.memberId == customerId);
                db.Members.Remove(customer);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Successfully delete!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Failed delete! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
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
        [CustomAuthorize("User")]
        [HttpGet]
        public ActionResult EditProfile()
        {
            Member user = (Member)Session["info"];
            Session["imgPath"] = user.avatar;
            return View(user);
        }
        [CustomAuthorize("User")]
        [HttpPost]
        public ActionResult EditProfile(Member member, HttpPostedFileBase uploadFile)
        {
            try
            {
                Member user = (Member)Session["info"];
                if (ModelState.IsValid)
                {
                    // Lấy entity thật từ DB để cập nhật
                    var dbMember = db.Members.FirstOrDefault(m => m.memberId == user.memberId);

                    // Cập nhật thông tin được phép thay đổi
                    dbMember.firstName = member.firstName?.Trim();
                    dbMember.lastName = member.lastName?.Trim();
                    dbMember.email = member.email?.Trim();
                    dbMember.phone = member.phone?.Trim();
                    dbMember.address = member.address?.Trim();
                    dbMember.birthday = member.birthday;

                    string oldAvatarRel = dbMember.avatar;
                    string oldAvatarAbs = !string.IsNullOrEmpty(oldAvatarRel) ? Request.MapPath(oldAvatarRel) : null;

                    // Nếu có upload file mới
                    if (uploadFile != null)
                    {
                        // Tạo đường dẫn file mới
                        var fileName = Path.GetFileName(uploadFile.FileName);
                        var newAvatarRel = "~/Content/img/avatar/" + fileName;
                        var newAvatarAbs = Path.Combine(Server.MapPath("~/Content/img/avatar"), fileName);

                        dbMember.avatar = newAvatarRel;
                        // Lưu file mới
                        uploadFile.SaveAs(newAvatarAbs);

                        // Xóa ảnh cũ nếu không còn ai dùng
                        if (!string.IsNullOrEmpty(oldAvatarAbs))
                        {
                            bool isUsed = db.Members.Any(m => m.avatar == oldAvatarRel && m.memberId != dbMember.memberId);
                            if (System.IO.File.Exists(oldAvatarAbs) && !isUsed)
                            {
                                System.IO.File.Delete(oldAvatarAbs);
                            }
                        }

                    }
                    db.SaveChanges();

                    // Cập nhật session với dữ liệu mới
                    Session["info"] = dbMember;
                    TempData["msgEditProfile"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Index");
                }

                ViewBag.roleId = new SelectList(db.Roles, "roleId", "roleName", user.roleId);
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["msgEditProfileFailed"] = "Đã xảy ra lỗi: " + ex.Message + ".";
                return RedirectToAction("Index");
            }
        }
        [CustomAuthorize("User")]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [CustomAuthorize("User")]
        [HttpPost]
        public ActionResult ChangePassword(FormCollection collection)
        {
            try
            {
                Member user = (Member)Session["info"];
                var CurrentPassword = collection["CurPassword"]; // Mật khẩu hiện tại
                var NewPassword = collection["NewPassword"]; // Mật khẩu mới

                CurrentPassword = Encryptor.MD5Hash(CurrentPassword.Trim());
                NewPassword = Encryptor.MD5Hash(NewPassword.Trim());

                // Lấy lại bản ghi user từ DB
                var member = db.Members.FirstOrDefault(model => model.memberId == user.memberId);
                if (member != null && member.password == CurrentPassword)
                {
                    member.password = NewPassword;
                    db.SaveChanges();

                    // Cập nhật lại session
                    Session["info"] = member;
                    TempData["msgChangePassword"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Sai mật khẩu!");
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                TempData["msgChangePasswordFailed"] = "Đã xảy ra lỗi: " + ex.Message + ".";
                return RedirectToAction("Index");
            }
        }
        [CustomAuthorize("User")]
        [HttpGet]
        public ActionResult MyOrder()
        {
            Member user = (Member)Session["info"];
            var orders = db.Invoinces.OrderByDescending(model => model.dateCreate).Where(model => model.memberId == user.memberId).ToList();
            return View(orders);
        }
        [CustomAuthorize("User")]
        [HttpGet]
        public JsonResult GetOrdersByStatus(string status)
        {
            Member user = (Member)Session["info"];
            var orders = db.Invoinces.Where(model => model.memberId == user.memberId);

            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                orders = orders.Where(model => model.status == status);
            }

            var result = orders.Select(order => new
            {
                invoinceId = order.invoinceId,
                memberId = user.memberId,
                status = order.status,
                totalMoney = order.totalMoney,
                paymentStatus = order.paymentStatus,
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("User")]
        [HttpGet]
        public ActionResult InvoinceDetail(string invoinceId)
        {
            Member user = (Member)Session["info"];
            if (!Guid.TryParse(invoinceId, out Guid parsedId))
            {
                return RedirectToAction("MyOrder", "Home");
            }
            var orders = db.Invoinces.FirstOrDefault(model => model.invoinceId == parsedId);
            return View(orders);
        }
        [CustomAuthorize("User")]
        public ActionResult Cancel(Guid id)
        {
            Member member = (Member)Session["info"]; // Lấy thông tin tài khoản từ session 
            Invoince invoince = db.Invoinces.FirstOrDefault(model => model.invoinceId == id);
            if (invoince.status == "cancelled" || invoince.status == "completed" || invoince.paymentStatus == "paid")
            {
                TempData["msgCancelOrderFailed"] = "Order ID " + invoince.invoinceId + " không thể hủy!";
                return RedirectToAction("MyOrder", "Home", new { memberId = member.memberId });
            }
            invoince.status = "cancelled";
            TempData["msgCancelOrder"] = "Order ID " + invoince.invoinceId + " đã được hủy.";
            db.SaveChanges();
            return RedirectToAction("MyOrder", "Home", new { memberId = member.memberId });
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
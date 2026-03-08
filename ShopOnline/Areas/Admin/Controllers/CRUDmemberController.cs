using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopOnline.Models;
using System.IO;
using System.Web.Security;

namespace ShopOnline.Areas.Admin.Controllers
{
    [CustomAuthorize("Manager")]
    public class CRUDmemberController : Controller
    {
        menfsEntities db = new menfsEntities();
        const string PASSWORD_DEFAULT = "123456";

        // GET: Admin/CRUDmember
        public ActionResult Index(string role)
        {
            ViewBag.role = role;
            return View();
        }

        [HttpGet]
        public JsonResult GetMember(string role)
        {
            try
            {
                var query = db.Members.AsQueryable();

                if (!string.IsNullOrEmpty(role))
                {
                    query = query.Where(x => x.Role.roleName == role);
                }

                // không lấy ICollection<>, List<>, hoặc navigation property → vòng lặp vô tận JSON.
                var listMember = query.OrderByDescending(i => i.dateCreate)
                                                      .Select(i => new
                                                      {
                                                          memberId = i.memberId,
                                                          avatar = i.avatar,
                                                          email = i.email,
                                                          name = i.firstName + " " + i.lastName,
                                                          birthday = i.birthday,
                                                          phone = i.phone,
                                                          status = i.status,
                                                          role = i.Role.roleName
                                                      }).ToList();

                return Json(new { success = true, listMember, msg = "Get list Member successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        // GET: Admin/CRUDmember/Create
        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.roleId = new SelectList(db.Roles, "roleId", "roleName");
            return PartialView();
        }

        // POST: Admin/CRUDmember/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Member member, HttpPostedFileBase uploadFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, msg = "Data is invalid", errors });
            }

            try
            {
                member.email = member.email.Trim();
                member.phone = member.phone.Trim();
                // Lấy tên đăng nhập đã nhập vào kiểm tra có trùng k
                var check = db.Members.SingleOrDefault(model => model.phone == member.phone || model.email == member.email);
                if (check != null)
                    return Json(new { success = false, msg = "This phone number or email has already been used!" });

                // ✅ Xử lý upload ảnh mới (nếu có)
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(uploadFile.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                        return Json(new { success = false, msg = "Invalid file type. Only JPG, JPEG, or PNG are allowed." });

                    // Xử lí ảnh
                    var fileName = Path.GetFileName(uploadFile.FileName);
                    member.avatar = "/Content/img/avatar/" + fileName;

                    var savePath = Path.Combine(Server.MapPath("/Content/img/avatar"), fileName);
                    uploadFile.SaveAs(savePath);
                }
                else
                {
                    member.avatar = "/Content/img/avatar/avatar.png";
                }

                member.password = Encryptor.MD5Hash(PASSWORD_DEFAULT);
                member.firstName = member.firstName.Trim();
                member.lastName = member.lastName.Trim();
                member.address = member.address?.Trim();
                member.status = true;
                member.dateCreate = DateTime.Now;

                db.Members.Add(member);
                db.SaveChanges();

                return Json(new { success = true, msg = "New account added successfully.!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }

        // GET: Admin/CRUDmember/Edit/5
        [HttpGet]
        public PartialViewResult Edit(Guid id)
        {
            Member member = db.Members.FirstOrDefault(i => i.memberId == id);
            //Session["imgPath"] = member.avatar;
            ViewBag.roleId = new SelectList(db.Roles, "roleId", "roleName", member.roleId);
            return PartialView(member);
        }

        // POST: Admin/CRUDmember/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(Member member, HttpPostedFileBase uploadFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, msg = "Data is invalid", errors });
            }

            try
            {
                var existing = db.Members.FirstOrDefault(i => i.memberId == member.memberId);
                if (existing == null)
                {
                    return Json(new { success = false, msg = "Member not found!" });
                }

                // Lấy tên đăng nhập đã nhập vào kiểm tra có trùng k
                var check = db.Members.SingleOrDefault(model => model.memberId != existing.memberId && (model.phone == member.phone || model.email == member.email));
                if (check != null)
                    return Json(new { success = false, msg = "This phone number or email has already been used!" });
                
                existing.email = member.email.Trim();
                existing.phone = member.phone.Trim();

                string oldAvatarRel = existing.avatar;
                string oldAvatarAbs = !string.IsNullOrEmpty(oldAvatarRel) ? Server.MapPath(oldAvatarRel) : null;
                // ✅ Xử lý upload ảnh mới (nếu có)
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(uploadFile.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                        return Json(new { success = false, msg = "Invalid file type. Only JPG, JPEG, or PNG are allowed." });

                    // Tạo đường dẫn file mới
                    var fileName = Path.GetFileName(uploadFile.FileName);
                    var newAvatarRel = "/Content/img/avatar/" + fileName;
                    var newAvatarAbs = Path.Combine(Server.MapPath("/Content/img/avatar"), fileName);

                    // Lưu file mới
                    uploadFile.SaveAs(newAvatarAbs);
                    existing.avatar = newAvatarRel;

                    // Xóa ảnh cũ nếu không còn ai dùng
                    if (!string.IsNullOrEmpty(oldAvatarAbs))
                    {
                        bool isUsedByOther = db.Members.Any(m => m.avatar == oldAvatarRel && m.memberId != existing.memberId);
                        if (System.IO.File.Exists(oldAvatarAbs) && !isUsedByOther)
                        {
                            System.IO.File.Delete(oldAvatarAbs);
                        }
                    }
                }

                existing.firstName = member.firstName.Trim();
                existing.lastName = member.lastName.Trim();
                existing.birthday = member.birthday;
                existing.address = member.address?.Trim();
                existing.roleId = member.roleId;
                existing.dateCreate = DateTime.Now;
                existing.status = member.status;

                db.SaveChanges();
                return Json(new { success = true, msg = "Updated account " + member.firstName + " " + member.lastName + "successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }

        // POST: Admin/CRUDmember/Delete/5
        //[HttpPost]
        //public ActionResult Delete(Guid id)
        //{
        //    try
        //    {
        //        Member member = db.Members.Find(id);
        //        var checkArticle = db.Articles.FirstOrDefault(model => model.memberId == id);
        //        var checkInvoice = db.Invoinces.FirstOrDefault(model => model.memberId == id);
        //        var checkProduct = db.Products.FirstOrDefault(model => model.memberId == id);
        //        string currentImg = Request.MapPath(member.avatar);
        //        // Kiểm tra xem  tài khoản có trong bảng Article, bảng Invoice hoặc bảng Product không
        //        if (checkArticle != null || checkInvoice != null || checkProduct != null)
        //        {
        //            TempData["msgDeleteFailed"] = "Không thể xóa tài khoản này!";
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            Member m = (Member)Session["infoAdmin"];
        //            // Kiểm tra xem tài khoản đăng nhập hiện tại có trùng với tài khoản xóa hay không
        //            if (m.memberId == member.memberId) // Nếu trùng thì xóa tài khoản và đăng xuất, chuyển về trang đăng nhập
        //            {
        //                TempData["msgDeleteFailed"] = "Không thể xóa tài khoản này!";
        //                return RedirectToAction("Index");
        //            }
        //            else
        //            {
        //                var avatarName = member.avatar.ToString(); // Lấy đường dẫn ảnh (relative path)
        //                var checkAvatart = db.Members.Where(model => model.avatar == avatarName).ToList(); // Kiểm tra ảnh có trùng với avatar của member nào không
        //                if (System.IO.File.Exists(currentImg) && checkAvatart.Count < 2)
        //                {
        //                    System.IO.File.Delete(currentImg);
        //                }
        //                db.Members.Remove(member);
        //                db.SaveChanges();
        //                TempData["msgDelete"] = "Đã xóa tài khoản " + member.firstName + " " + member.lastName + ".";
        //                return RedirectToAction("Index");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["msgDeleteFailed"] = "Không thể xóa! " + ex.Message + ".";
        //        return RedirectToAction("Index");
        //    }
        //}
    }
}
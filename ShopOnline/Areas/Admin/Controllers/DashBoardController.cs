using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopOnline.Models;

namespace ShopOnline.Areas.Admin.Controllers
{
    [CustomAuthorize("Manager")]
    public class DashBoardController : Controller
    {
        menfsEntities db = new menfsEntities();
        public ActionResult Index()
        {
            /*---------------------------------------THỐNG KÊ------------------------------*/
            // Lấy ra tổng số tiền các đơn hàng - lợi nhuận
            ViewBag.profit = db.InvoinceDetails.Sum(model => model.price) ?? 0;
            // Lấy ra tổng số lượng các user khách hàng đang có trong database - tổng số tài khoản hiện tại
            ViewBag.users = db.Members.Where(model => model.roleId == new Guid("54ed1855-5103-4121-811c-3997ce4c2241")).Count();
            // Lấy ra tổng số các đơn đặt hàng đang có trong database - tổng số đơn hàng 
            ViewBag.orders = db.Invoinces.Count();
            // Lấy số lượng người truy cập từ Application đã tạo
            ViewBag.SoNguoiTruyCap = HttpContext.Application["SoNguoiTruyCap"].ToString();
            return View();
        }
        public PartialViewResult Chart()
        {

            // Danh thu sản phẩm cao nhất, thu được:
            /*(tên sản phẩm - hình ảnh - tổng danh thu dựa trên đơn hàng)*/
            var getTopProduct = (db.Products.Join(db.InvoinceDetails,
                                sanPham => sanPham.productId,
                                ctDonHang => ctDonHang.productId,
                                (sanPham, ctDonHang) => new {
                                    tensanpham = sanPham.productName,
                                    hinh = sanPham.image,
                                    gia = ctDonHang.quanlity * sanPham.price,
                                })).ToList();
            var test = getTopProduct;

            // Đếm số lượng sản phẩm trong từng loại sản phẩm, thu được: (Đã đổ dữ liệu vào pie-chart)
            /*(tên loại sản phẩm - số lượng sản phẩm thuộc loại đó)*/
            var countProductOfCategory = (from c in db.ProductCategories
                                          join p in db.Products on c.categoryId equals p.categoryId into temp1
                                          from temp2 in temp1.DefaultIfEmpty()
                                          group temp2 by c.categoryName into grouped
                                          select new
                                          {
                                              ten = grouped.Key,
                                              count = grouped.Count(t => t.productId != null)
                                          }).ToList();
            var categoriesData = countProductOfCategory;

            List<string> categories = new List<string>();
            List<string> categoriesCount = new List<string>();
            foreach (var i in categoriesData)
            {
                categories.Add(i.ten);
            }
            foreach (var i in categoriesData)
            {
                categoriesCount.Add(i.count.ToString());
            }
            ViewBag.categories = categories.ToList();
            ViewBag.categoriesCount = categoriesCount.ToList();

            // Thống kê doanh thu bán hàng theo ngày, thu được: (Đã đổ dữ liệu vào Line-chart)
            /*(ngày bán - doanh thu)*/
            
            var doanhthu = db.vDoanhThuTheoNgays.OrderByDescending(model => model.dateCreate).Take(7).ToList(); // Sắp xếp giảm dần theo ngày và lấy 7 ngày gần nhất
            // Đảo ngược mảng để hiển thị ngày theo thứ tự tăng dần
            doanhthu.Reverse();

            List<string> date = new List<string>();
            List<int> income = new List<int>();
            foreach (var i in doanhthu)
            {
                date.Add(i.dateCreate);
            }
            foreach (var i in doanhthu)
            {
                income.Add((int)i.income);
            }
            ViewBag.date = date.ToList();
            ViewBag.income = income.ToList();
            // Chart
            return PartialView();
        }
        public PartialViewResult InvoiceList()
        {
            var list = db.Invoinces.OrderByDescending(model => model.dateCreate).ToList();
            return PartialView(list);
        }
        [HttpGet]
        public ActionResult EditProfile()
        {
            Member user = (Member)Session["infoAdmin"];
            Session["imgPath"] = user.avatar;
            return View(user);
        }
        [HttpPost]
        public JsonResult EditProfile(Member member, HttpPostedFileBase uploadFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, msg = "Data is invalid", errors });
            }

            try
            {
                // Lấy entity thật từ DB để cập nhật
                var dbMember = db.Members.FirstOrDefault(m => m.memberId == member.memberId);

                // Cập nhật thông tin được phép thay đổi
                dbMember.firstName = member.firstName?.Trim();
                dbMember.lastName = member.lastName?.Trim();
                dbMember.email = member.email?.Trim();
                dbMember.phone = member.phone?.Trim();
                dbMember.address = member.address?.Trim();
                dbMember.birthday = member.birthday;

                string oldAvatarRel = dbMember.avatar;
                string oldAvatarAbs = !string.IsNullOrEmpty(oldAvatarRel) ? Server.MapPath(oldAvatarRel) : null;

                // Nếu có upload file mới
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(uploadFile.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                    {
                        return Json(new { success = false, msg = "Invalid file type. Only JPG, JPEG, or PNG are allowed." });
                    }

                    // Tạo đường dẫn file mới
                    var fileName = Path.GetFileName(uploadFile.FileName);
                    var newAvatarRel = "/Content/img/avatar/" + fileName;
                    var newAvatarAbs = Path.Combine(Server.MapPath("/Content/img/avatar"), fileName);

                    // Lưu file mới
                    uploadFile.SaveAs(newAvatarAbs);
                    dbMember.avatar = newAvatarRel;

                    // Xóa ảnh cũ nếu không còn ai dùng
                    if (!string.IsNullOrEmpty(oldAvatarAbs))
                    {
                        bool isUsedByOther = db.Members.Any(m => m.avatar == oldAvatarRel && m.memberId != dbMember.memberId);
                        if (System.IO.File.Exists(oldAvatarAbs) && !isUsedByOther)
                        {
                            System.IO.File.Delete(oldAvatarAbs);
                        }
                    }

                }
                db.SaveChanges();

                // Cập nhật session với dữ liệu mới
                Session["infoAdmin"] = dbMember;
                return Json(new { success = true, msg = "Update profile successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ChangePassword(Member member, FormCollection collection)
        {
            try
            {
                var CurrentPassword = collection["CurPassword"]; // Mật khẩu hiện tại
                var NewPassword = collection["NewPassword"]; // Mật khẩu mới
                //var ConfirmPassword = collection["Confirm"];

                CurrentPassword = Encryptor.MD5Hash(CurrentPassword.Trim());
                NewPassword = Encryptor.MD5Hash(NewPassword.Trim());

                var check = db.Members.Where(model => model.password == CurrentPassword && model.memberId == member.memberId).FirstOrDefault();
                if (check!= null)
                {
                    check.password = NewPassword;
                    db.SaveChanges();
                    return Json(new { success = true, msg = "Change password successfully!" });
                }
                return Json(new { success = false, msg = "Password incorrect" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
    }
}
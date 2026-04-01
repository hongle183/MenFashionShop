using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopOnline.Models;
using ShopOnline.Services;

namespace ShopOnline.Areas.Admin.Controllers
{
    [CustomAuthorize("Manager")]
    public class CRUDproductController : Controller
    {
        private readonly menfsEntities db = new menfsEntities();

        // GET: Admin/CRUDproduct
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetProduct()
        {
            try
            {
                // không lấy ICollection<>, List<>, hoặc navigation property → vòng lặp vô tận JSON.
                var listProduct = db.Products.OrderByDescending(i => i.dateCreate)
                                                      .Select(i => new
                                                      {
                                                          productId = i.productId,
                                                          image = i.image,
                                                          productName = i.productName,
                                                          price = i.price,
                                                          discount = i.discount,
                                                          quantity = i.quanlity,
                                                          category = i.ProductCategory.categoryName,
                                                          status = i.status
                                                      }).ToList();

                return Json(new { success = true, listProduct, msg = "Get list Product successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        // GET: Admin/CRUDproduct/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.categoryId = new SelectList(db.ProductCategories, "categoryId", "categoryName");
            return View();
        }

        // POST: Admin/CRUDproduct/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Product product, HttpPostedFileBase uploadFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, msg = "Data is invalid", errors });
            }
            try
            {
                var productName = product.productName.Trim();
                // Lấy tên sản phẩm để kiểm tra có trùng k
                var check = db.Products.SingleOrDefault(model => model.productName == productName);
                if (check != null)
                    return Json(new { success = false, msg = "This product name already exists!" });

                if (product.discount >= 100)
                    return Json(new { success = false, msg = "Discount must be less than 100." });

                if (uploadFile == null || uploadFile.ContentLength == 0)
                    return Json(new { success = false, msg = "Please select an image file to upload." });
                // Xử lí ảnh
                ImageServices imageServices = new ImageServices();
                string url = "/Content/img/product/";
                var savePath = Server.MapPath(url);
                string newImage = url + imageServices.SaveCroppedImage(savePath, uploadFile, 400, 500);

                product.productName = product.productName.Trim();
                product.characteristic = product.characteristic != null ? product.characteristic.Trim() : "";
                product.description = product.description;
                product.meta = product.meta.Trim();
                product.image = newImage;
                Member member = (Member)Session["infoAdmin"];
                product.memberId = member.memberId;
                product.dateCreate = DateTime.Now;
                product.status = true;

                db.Products.Add(product);
                db.SaveChanges();

                return Json(new { success = true, redirectUrl = Url.Action("Index", "CRUDproduct", new { area = "Admin" }), msg = "New product added successfully.!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }

        // GET: Admin/CRUDproduct/Edit/:id
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            Product product = db.Products.Find(id);
            Session["imgPath"] = product.image;
            ViewBag.categoryId = new SelectList(db.ProductCategories, "categoryId", "categoryName", product.categoryId);
            return View(product);
        }

        // GET: Admin/CRUDproduct/Edit/:id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(Product product, HttpPostedFileBase uploadFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, msg = "Data is invalid", errors });
            }
            try
            {
                var existing = db.Products.FirstOrDefault(p => p.productId == product.productId);
                if (existing == null)
                    return Json(new { success = false, msg = "This product not found." });

                // Cập nhật thông tin chung
                existing.productName = product.productName?.Trim();
                existing.meta = product.meta?.Trim();
                existing.characteristic = product.characteristic?.Trim();
                existing.discount = product.discount;
                existing.price = product.price;
                existing.quanlity = product.quanlity;
                existing.description = product.description;
                existing.dateCreate = DateTime.Now;
                existing.status = product.status;

                // ✅ Xử lý upload ảnh mới (nếu có)
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    // Xử lí ảnh
                    ImageServices imageServices = new ImageServices();
                    string url = "/Content/img/product/";
                    var savePath = Server.MapPath(url);
                    string newImage = url + imageServices.SaveCroppedImage(savePath, uploadFile, 400, 500);

                    // Xóa ảnh cũ nếu không có product nào dùng
                    if (!string.IsNullOrEmpty(existing.image))
                    {
                        string oldPath = Server.MapPath(existing.image);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    existing.image = newImage;
                }

                db.SaveChanges();
                return Json(new { success = true, redirectUrl = Url.Action("Index", "CRUDproduct", new { area = "Admin" }), msg = "Updated \"" + existing.productName + "\" successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }

        // POST: Admin/CRUDproduct/Delete/:id
        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            try
            {
                var hasInvoices = db.InvoinceDetails.FirstOrDefault(model => model.invoinceId == id);

                if (hasInvoices != null)
                {
                    return Json(new { success = false, msg = "Cannot delete this product because it still contains invoices!" });
                }

                var product = db.Products.FirstOrDefault(i => i.productId == id);
                if (product == null)
                    return Json(new { success = false, msg = "Product not found!" });

                // Xóa ảnh cũ nếu không còn product dùng
                if (!string.IsNullOrEmpty(product.image))
                {
                    string currentImg = Server.MapPath(product.image);
                    bool isUsed = db.Products.Any(m => m.image == product.image && m.productId != product.productId);
                    if (System.IO.File.Exists(currentImg) && !isUsed)
                    {
                        System.IO.File.Delete(currentImg);
                    }
                }

                db.Products.Remove(product);
                db.SaveChanges();
                return Json(new { success = true, msg = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }

    }
}
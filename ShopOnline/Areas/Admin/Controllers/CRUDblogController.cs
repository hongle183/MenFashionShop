using ShopOnline.Models;
using ShopOnline.Services;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopOnline.Areas.Admin.Controllers
{
    [CustomAuthorize("Manager")]
    public class CRUDblogController : Controller
    {
        private readonly menfsEntities db = new menfsEntities();

        // GET: Admin/CRUDblog
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetBlog()
        {
            try
            {
                // không lấy ICollection<>, List<>, hoặc navigation property → vòng lặp vô tận JSON.
                var listBlog = db.Articles.OrderBy(i => i.title)
                                                      .Select(i => new
                                                      {
                                                          articleId = i.articleId,
                                                          title = i.title,
                                                          image = i.image,
                                                          author = i.Member.lastName,
                                                          dateCreate = i.dateCreate,
                                                          status = i.status
                                                      }).ToList();

                return Json(new { success = true, listBlog, msg = "Get list Blog successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        // GET: Admin/CRUDblog/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.categoryId = new SelectList(db.ProductCategories, "categoryId", "categoryName");
            return View();
        }
        // POST: Admin/CRUDblog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Article article, HttpPostedFileBase uploadFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, msg = "Data is invalid", errors });
            }
            try
            {
                if (uploadFile == null || uploadFile.ContentLength == 0)
                    return Json(new { success = false, msg = "Please select an image file to upload." });
                // Xử lí ảnh
                ImageServices imageServices = new ImageServices();
                string url = "/Content/img/blog/";
                var savePath = Server.MapPath(url);
                string newImage = url + imageServices.SaveCroppedImage(savePath, uploadFile, 1500, 600);

                article.title = article.title.Trim();
                article.description = article.description.Trim();
                article.content = article.content;
                article.meta = article.meta.Trim();
                article.image = newImage;
                article.memberId = new Guid("e4d33c53-b8a3-4f82-9ff3-e611912631fe");
                article.dateCreate = DateTime.Now;
                article.status = true;

                db.Articles.Add(article);
                db.SaveChanges();

                return Json(new { success = true, redirectUrl = Url.Action("Index", "CRUDblog", new { area = "Admin" }), msg = "New blog added successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }

        // GET: Admin/CRUDblog/Edit/5
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            Article article = db.Articles.Find(id);
            Session["imgPath"] = article.image;
            /*ViewBag.categoryId = new SelectList(db.ProductCategories, "categoryId", "categoryName", article.categoryId);*/
            return View(article);
        }
        // POST: Admin/CRUDblog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Article article, HttpPostedFileBase uploadFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, msg = "Data is invalid", errors });
            }
            try
            {
                var existing = db.Articles.FirstOrDefault(p => p.articleId == article.articleId);
                if (existing == null)
                    return Json(new { success = false, msg = "This blog not found." });

                // Cập nhật thông tin chung
                existing.title = article.title.Trim();
                existing.description = article.description.Trim();
                existing.content = article.content;
                existing.meta = article.meta.Trim();
                existing.status = article.status;
                existing.dateCreate = DateTime.Now;

                // ✅ Xử lý upload ảnh mới (nếu có)
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    ImageServices imageServices = new ImageServices();
                    string url = "/Content/img/blog/";
                    var savePath = Server.MapPath(url);
                    string newImage = url + imageServices.SaveCroppedImage(savePath, uploadFile, 1500, 600);

                    // Xóa ảnh cũ nếu không có article nào dùng
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

                return Json(new { success = true, redirectUrl = Url.Action("Index", "CRUDblog", new { area = "Admin" }), msg = "Blog updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
        // POST: Admin/CRUDblog/Delete/5
        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            try
            {
                var article = db.Articles.FirstOrDefault(i => i.articleId == id);
                if (article == null)
                    return Json(new { success = false, msg = "Blog not found!" });

                // Xóa ảnh cũ nếu không còn blog dùng
                if (!string.IsNullOrEmpty(article.image))
                {
                    string currentImg = Server.MapPath(article.image);
                    bool isUsed = db.Articles.Any(m => m.image == article.image && m.articleId != article.articleId);
                    if (System.IO.File.Exists(currentImg) && !isUsed)
                    {
                        System.IO.File.Delete(currentImg);
                    }
                }

                db.Articles.Remove(article);
                db.SaveChanges();
                return Json(new { success = true, msg = "Blog deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }

    }
}
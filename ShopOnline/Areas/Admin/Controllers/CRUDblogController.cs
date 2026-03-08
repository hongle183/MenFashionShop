using ShopOnline.Models;
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
        menfsEntities db = new menfsEntities();

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
            try
            {
                if (ModelState.IsValid)
                {
                    if (uploadFile == null || uploadFile.ContentLength == 0)
                    {
                        ModelState.AddModelError("", "Please select an image file to upload.");

                        return View(article);
                    }

                    string extension = Path.GetExtension(uploadFile.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                    {
                        ModelState.AddModelError("", "Invalid file type. Only JPG, JPEG, or PNG are allowed.");

                        return View(article);
                    }

                    // Xử lí ảnh
                    var fileName = Path.GetFileName(uploadFile.FileName);
                    var savePath = Path.Combine(Server.MapPath("/Content/img/blog"), fileName);
                    uploadFile.SaveAs(savePath);

                    article.title = article.title.Trim();
                    article.description = article.description.Trim();
                    article.content = article.content;
                    article.meta = article.meta.Trim();
                    article.image = "/Content/img/blog/" + fileName;
                    article.memberId = new Guid("e4d33c53-b8a3-4f82-9ff3-e611912631fe");
                    article.dateCreate = DateTime.Now;
                    article.status = true;

                    db.Articles.Add(article);
                    db.SaveChanges();

                    TempData["msgCreate"] = "New blog added successfully!";
                    return RedirectToAction("Index");

                }
                return View(article);
            }
            catch (Exception ex)
            {
                TempData["msgCreatefailed"] = "Error: " + ex.Message + ".";
                return RedirectToAction("Create");
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
            try
            {
                if (ModelState.IsValid)
                {
                    var existing = db.Articles.FirstOrDefault(p => p.articleId == article.articleId);
                    if (existing == null)
                    {
                        TempData["msgEditFailed"] = "Blog not found.";
                        return RedirectToAction("Index");
                    }

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
                        string extension = Path.GetExtension(uploadFile.FileName).ToLower();
                        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                        {
                            ModelState.AddModelError("", "Invalid file type. Only JPG, JPEG, or PNG allowed.");
                            return View(article);
                        }

                        var fileName = Path.GetFileName(uploadFile.FileName);
                        var newPath = Path.Combine(Server.MapPath("/Content/img/blog"), fileName);
                        var newDbPath = "/Content/img/blog/" + fileName;


                        // Xóa ảnh cũ nếu không có article nào dùng
                        if (!string.IsNullOrEmpty(existing.image))
                        {
                            string oldPath = Server.MapPath(existing.image);
                            bool isUsed = db.Articles.Any(m => m.image == existing.image && m.articleId != existing.articleId);
                            if (System.IO.File.Exists(oldPath) && !isUsed)
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        // Lưu file mới và cập nhật DB
                        uploadFile.SaveAs(newPath);
                        existing.image = newDbPath;
                    }

                    db.SaveChanges();

                    TempData["msgEdit"] = "Blog updated successfully.";
                    return RedirectToAction("Index");
                }
                return View(article);
            }
            catch (Exception ex)
            {
                TempData["msgEditFailed"] = "Error: " + ex.Message + ".";
                return RedirectToAction("Index");
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
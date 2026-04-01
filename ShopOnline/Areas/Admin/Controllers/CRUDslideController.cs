using ShopOnline.Models;
using ShopOnline.Services;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopOnline.Areas.Admin.Controllers
{
    [CustomAuthorize("Manager")]
    public class CRUDslideController : Controller
    {
        private readonly menfsEntities db = new menfsEntities();
        // GET: Admin/CRUDSlide
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetSlide()
        {
            try
            {
                // không lấy ICollection<>, List<>, hoặc navigation property → vòng lặp vô tận JSON.
                var listSlide = db.Slides.OrderBy(i => i.order)
                                                      .Select(i => new
                                                      {
                                                          slideId = i.slideId,
                                                          subtitle = i.subtitle,
                                                          title = i.title,
                                                          description = i.description,
                                                          image = i.image,
                                                          status = i.status,
                                                          order = i.order,
                                                          dateCreate = i.dateCreate
                                                      }).ToList();

                return Json(new { success = true, listSlide, msg = "Get list Slide successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        // GET: Admin/CRUDSlide/Edit/5
        public ActionResult Edit(Guid id)
        {
            var detail = db.Slides.FirstOrDefault(i => i.slideId == id);

            return View(detail);
        }
        // POST: Admin/CRUDSlide/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(Slide slide, HttpPostedFileBase uploadFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, msg = "Data is invalid", errors });
            }
            try
            {
                var existing = db.Slides.FirstOrDefault(p => p.slideId == slide.slideId);
                if (existing == null)
                    return Json(new { success = false, msg = "This slide not found." });

                // Cập nhật thông tin chung
                existing.subtitle = slide.subtitle?.Trim();
                existing.title = slide.title?.Trim();
                existing.description = slide.description?.Trim();                
                existing.dateCreate = DateTime.Now;
                existing.status = slide.status;

                // ✅ Xử lý upload ảnh mới (nếu có)
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    // Xử lí ảnh
                    ImageServices imageServices = new ImageServices();
                    string url = "/Content/img/hero/";
                    var savePath = Server.MapPath(url);
                    string newImage = url + imageServices.SaveCroppedImage(savePath, uploadFile, 1600, 800);

                    // Xóa ảnh cũ nếu không có slide nào dùng
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
                return Json(new { success = true, redirectUrl = Url.Action("Index", "CRUDslide", new { area = "Admin" }), msg = "Updated slide successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
    }
}
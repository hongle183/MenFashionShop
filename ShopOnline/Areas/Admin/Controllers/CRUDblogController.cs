using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopOnline.Models;
using System.IO;
using PagedList;
using System.Data.Entity.Migrations;
using ShopOnline.Help;

namespace ShopOnline.Areas.Admin.Controllers
{
    [Authorize]
    public class CRUDblogController : Controller
    {
        menfsEntities1 db = new menfsEntities1();

        public ActionResult Index(int? page, string searching)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;
            var article = db.Articles.Where(model => model.title.Contains(searching) || searching == null).OrderByDescending(model => model.dateCreate).ToPagedList(pageNumber, pageSize);
            return View(article);
        }

        //CREATE
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult Create()
        {
            ViewBag.categoryId = new SelectList(db.ProductCategories, "categoryId", "categoryName");
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Article article, HttpPostedFileBase uploadFile)
        {
            try
            {
                // Xử lí ảnh
                var fileName = Path.GetFileName(uploadFile.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/img/blog"), fileName);
                string extension = Path.GetExtension(uploadFile.FileName);

                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if (uploadFile == null)
                    {
                        ModelState.AddModelError("", "Error while file uploading.");
                    }
                    else
                    {
                        article.meta = Functions.ConvertToUnSign(article.title);
                        article.image = "~/Content/img/blog/" + fileName;
                        article.memberId = new Guid("e4d33c53-b8a3-4f82-9ff3-e611912631fe");
                        article.status = true;
                        article.dateCreate = DateTime.Now;

                        db.Articles.Add(article);
                        if (db.SaveChanges() > 0)
                        {
                            uploadFile.SaveAs(path);
                            ModelState.Clear();
                            TempData["msgCreate"] = "Successfully create a new blog!";
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid File Type");
                }
                return View(article);
            }
            catch (Exception ex)
            {
                TempData["msgCreatefailed"] = "Create failed! " + ex.Message;
                return RedirectToAction("Create");
            }
        }

        //EDIT
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            Article article = db.Articles.Find(id);
            Session["imgPath"] = article.image;
            /*ViewBag.categoryId = new SelectList(db.ProductCategories, "categoryId", "categoryName", article.categoryId);*/
            return View(article);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Article article, HttpPostedFileBase uploadFile, FormCollection collection)
        {
            try
            {
                var contentTemp = collection["des"]; // Lấy giá trị nhập ở input content
                if (ModelState.IsValid)
                {
                    if (uploadFile != null)
                    {
                        var fileName = Path.GetFileName(uploadFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/img/blog"), fileName);

                        article.image = "~/Content/img/blog/" + fileName;
                        article.content = contentTemp;
                        article.dateCreate = DateTime.Now;
                        db.Articles.AddOrUpdate(article);
                       //db.Entry(article).State = EntityState.Modified;
                        string oldImgPath = Request.MapPath(Session["imgPath"].ToString());
                        if (db.SaveChanges() > 0)
                        {
                            TempData["msgEdit"] = "Successfully edited product has ID: " + article.articleId;
                            uploadFile.SaveAs(path);
                            if (System.IO.File.Exists(oldImgPath))
                            {
                                System.IO.File.Delete(oldImgPath);
                            }
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        article.image = Session["imgPath"].ToString();
                        article.content = contentTemp;
                        article.dateCreate = DateTime.Now;
                        db.Articles.AddOrUpdate(article);
                        //db.Entry(article).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            TempData["msgEdit"] = "Successfully edited product has ID: " + article.articleId;
                            return RedirectToAction("index");
                        }
                    }
                }
                return View(article);
            }
            catch(Exception ex)
            {
                TempData["msgEditFailed"] = "Edit failed! " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        //DELETE
        public ActionResult Delete(Guid? id)
        {
            try
            {
                Article article = db.Articles.Find(id);
                string currentImg = Request.MapPath(article.image);
                if (System.IO.File.Exists(currentImg))
                {
                    System.IO.File.Delete(currentImg);
                }
                db.Articles.Remove(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["msgDelete"] = "Can't delete this! " + ex.Message;
                return RedirectToAction("Index");
            }
        }

    }
}
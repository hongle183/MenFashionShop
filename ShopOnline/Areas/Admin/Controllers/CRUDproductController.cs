﻿using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopOnline.Models;
using System.IO;

namespace ShopOnline.Areas.Admin.Controllers
{
    [CustomAuthorize("Admin")]
    public class CRUDproductController : Controller
    {
        menfsEntities db = new menfsEntities();

        // GET: Admin/CRUDproduct
        public ActionResult Index(string searching)
        {
            var products = db.Products.Where(model => model.productName.Contains(searching) || searching == null && model.status == true).OrderByDescending(model => model.dateCreate).Include(model => model.Member).Include(model => model.ProductCategory).ToList();
            return View(products);
        }

        // GET: Admin/CRUDproduct/Create
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult Create()
        {
            ViewBag.categoryId = new SelectList(db.ProductCategories, "categoryId", "categoryName");
            return View();
        }

        // POST: Admin/CRUDproduct/Create
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase uploadFile)
        {
            try
            {
                var productName = product.productName.Trim();
                // Lấy tên sản phẩm để kiểm tra có trùng k
                var check = db.Products.SingleOrDefault(model => model.productName == productName);
                // Xử lí ảnh
                var fileName = Path.GetFileName(uploadFile.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/img/product"), fileName);
                string extension = Path.GetExtension(uploadFile.FileName);

                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if (check != null)
                    {
                        ModelState.AddModelError("", "Tên sản phẩm đã tồn tại");
                    }
                    else
                    {
                        if (uploadFile == null)
                        {
                            ModelState.AddModelError("", "Đã xảy ra lỗi khi upload file.");
                        }
                        else
                        {
                            if (product.discount >= 100)
                            {
                                ModelState.AddModelError("", "Giảm giá phải nhỏ hơn 100.");
                            }
                            else
                            {
                                product.productName = product.productName.Trim();
                                product.characteristic = product.characteristic != null ? product.characteristic.Trim() : "";
                                product.meta = product.meta.Trim();
                                product.image = "~/Content/img/product/" + fileName;
                                Member member = (Member)Session["infoAdmin"];
                                product.memberId = member.memberId;
                                product.dateCreate = DateTime.Now;
                                product.status = true;
                                db.Products.Add(product);
                                if (db.SaveChanges() > 0)
                                {
                                    uploadFile.SaveAs(path);
                                    ModelState.Clear();
                                    TempData["msgCreate"] = "Thêm mới sản phẩm thành công!";
                                    return RedirectToAction("Index");
                                }
                            }
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Invalid File Type");
                }
                ViewBag.categoryId = new SelectList(db.ProductCategories, "categoryId", "categoryName", product.categoryId);
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["msgCreatefailed"] = "Đã xảy ra lỗi: " + ex.Message;
                return RedirectToAction("Create");
            }
        }

        // GET: Admin/CRUDproduct/Edit/:id
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            Product product = db.Products.Find(id);
            Session["imgPath"] = product.image;
            ViewBag.categoryId = new SelectList(db.ProductCategories, "categoryId", "categoryName", product.categoryId);
            return View(product);
        }
        
        // GET: Admin/CRUDproduct/Edit/:id
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Product product, HttpPostedFileBase uploadFile, FormCollection collection)
        {
            try
            {
                var descriptiontemp = collection["des"];
                if (ModelState.IsValid)
                {
                    if (uploadFile != null)
                    {
                        var fileName = Path.GetFileName(uploadFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/img/product"), fileName);
                        product.image = "~/Content/img/product/" + fileName;
                        product.description = descriptiontemp;
                        product.dateCreate = DateTime.Now;
                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                        string oldImgPath = Request.MapPath(Session["imgPath"].ToString());
                        if (db.SaveChanges() > 0)
                        {
                            TempData["msgEdit"] = "Đã cập nhật sản phẩm " + product.productName + ".";
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
                        product.image = Session["imgPath"].ToString();
                        product.description = descriptiontemp;
                        product.dateCreate = DateTime.Now;
                        //db.Products.AddOrUpdate(product);
                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            TempData["msgEdit"] = "Đã cập nhật sản phẩm " + product.productName + ".";
                            return RedirectToAction("index");
                        }
                    }
                }
                ViewBag.categoryId = new SelectList(db.ProductCategories, "categoryId", "categoryName", product.categoryId);
                return View(product);
            }
            catch(Exception ex)
            {
                TempData["msgEditFailed"] = "Đã xảy ra lỗi: " + ex.Message + ".";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/CRUDproduct/Delete/:id
        public ActionResult Delete(Guid id)
        {
            try
            {
                var checkInvoice = db.InvoinceDetails.FirstOrDefault(model => model.invoinceId == id);
                // Kiểm tra xem với mã Product có tồn tại trong bảng InvoinceDetail không?
                if (checkInvoice != null) // Nếu có giá trị thì xuất thông báo lỗi
                {
                    TempData["msgDeleteFailed"] = "Không thể xóa!";
                    return RedirectToAction("Index");
                }
                else
                {
                    Product product = db.Products.Find(id);
                    string currentImg = Request.MapPath(product.image);
                    if (System.IO.File.Exists(currentImg))
                    {
                        System.IO.File.Delete(currentImg);
                    }
                    db.Products.Remove(product);
                    db.SaveChanges();
                    TempData["msgDelete"] = "Xóa thành công sản phẩm " + product.productName + ".";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["msgDeleteFailed"] = "Không thể xóa! " + ex.Message + ".";
                return RedirectToAction("Index");
            }
        }

    }
}
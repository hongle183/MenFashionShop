﻿using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ShopOnline.Models;

namespace ShopOnline.Areas.Admin.Controllers
{
    [CustomAuthorize("Admin")]
    public class CRUDmenusController : Controller
    {
        private menfsEntities db = new menfsEntities();

        // GET: Admin/CRUDmenus
        public ActionResult Index()
        {
            return View(db.Menus.OrderBy(model => model.order).ToList());
        }


        // GET: Admin/CRUDmenus/Create
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/CRUDmenus/Create
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Menu menu)
        {
            try
            {
                var name = menu.name.Trim();
                // Lấy tên menu để kiểm tra có trùng k
                var check = db.Menus.SingleOrDefault(model => model.name == name);
                if (check != null)
                {
                    ModelState.AddModelError("", "Tên menu này đã tồn tại.");
                }
                else
                {
                    menu.name = menu.name.Trim();
                    menu.meta = menu.meta.Trim();
                    menu.order = menu.order;
                    menu.dateCreate = DateTime.Now;
                    menu.status = true;
                    db.Menus.Add(menu);
                    if (db.SaveChanges() > 0)
                    {
                        ModelState.Clear();
                        TempData["msgCreate"] = "Thêm mới menu thành công!";
                        return RedirectToAction("Index");
                    }
                }               
                return View(menu);
            }
            catch (Exception ex)
            {
                TempData["msgCreatefailed"] = "Đã xảy ra lỗi: " + ex.Message + ".";
                return RedirectToAction("Create");
            }
        }

        // GET: Admin/CRUDmenus/Edit/5
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menu menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        // POST: Admin/CRUDmenus/Edit/5
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Menu menu)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    menu.dateCreate = DateTime.Now;
                    db.Entry(menu).State = System.Data.Entity.EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        TempData["msgEdit"] = "Đã cập nhật menu " + menu.name + ".";
                    }
                    return RedirectToAction("Index");
                }
                return View(menu);
            }
            catch (Exception ex)
            {
                TempData["msgEditFailed"] = "Đã xảy ra lỗi: " + ex.Message + ".";
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/CRUDmenus/Delete/5
        public ActionResult Delete(Guid? id)
        {
            try
            {
                Menu menu = db.Menus.Find(id);
                db.Menus.Remove(menu);
                db.SaveChanges();
                TempData["msgDelete"] = "Xóa thành công menu " + menu.name + ".";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["msgDeleteFailed"] = "Không thể xóa! " + ex.Message + ".";
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

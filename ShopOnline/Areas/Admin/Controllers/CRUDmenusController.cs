using ShopOnline.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ShopOnline.Areas.Admin.Controllers
{
    [CustomAuthorize("Manager")]
    public class CRUDmenusController : Controller
    {
        private readonly menfsEntities db = new menfsEntities();

        // GET: Admin/CRUDmenus
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetMenu()
        {
            try
            {
                var listMenu = db.Menus.OrderByDescending(i => i.order).ToList();

                return Json(new { success = true, listMenu, msg = "Get list Menu successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        // GET: Admin/CRUDmenus/Create
        [HttpGet]
        public ActionResult Create()
        {
            var orders = db.Menus.Select(m => m.order).ToList();
            int maxOrder = db.Menus.Count();

            // Range từ 0 đến ... n phần tử
            var availableOrders = Enumerable.Range(0, maxOrder + 2)
                                            .Where(i => !orders.Contains(i))
                                            .ToList();

            ViewBag.AvailableOrders = new SelectList(availableOrders);

            return View();
        }

        // POST: Admin/CRUDmenus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Menu menu)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, msg = "Data is invalid", errors });
            }

            try
            {
                var name = menu.name.Trim();
                // Lấy tên menu để kiểm tra có trùng k
                var check = db.Menus.SingleOrDefault(model => model.name == name);
                if (check != null)
                {
                    return Json(new { success = false, msg = "This menu already exists!" });
                }

                menu.name = menu.name.Trim();
                menu.meta = menu.meta.Trim();
                menu.order = menu.order;
                menu.dateCreate = DateTime.Now;
                menu.status = true;
                db.Menus.Add(menu);
                db.SaveChanges();

                return Json(new { success = true, msg = "New menu category added successfully.!!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }

        // GET: Admin/CRUDmenus/Edit/5
        [HttpGet]
        public PartialViewResult Edit(Guid id)
        {
            var detail = db.Menus.FirstOrDefault(i => i.menuId == id);

            var orders = db.Menus
                    .Where(m => m.menuId != id)
                    .Select(m => m.order)
                    .ToList();

            int maxOrder = db.Menus.Count();

            // Lấy danh sách thứ tự còn trống + thứ tự hiện tại
            var availableOrders = Enumerable.Range(0, maxOrder + 1)
                .Where(i => !orders.Contains(i) || i == detail.order)
                .ToList();

            ViewBag.AvailableOrders = new SelectList(availableOrders, detail.order);
            return PartialView(detail);
        }

        // POST: Admin/CRUDmenus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(Menu menu)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, msg = "Data is invalid", errors });
            }

            try
            {
                var existing = db.Menus.FirstOrDefault(i => i.menuId == menu.menuId);
                if (existing == null)
                {
                    return Json(new { success = false, msg = "Menu not found!" });
                }

                existing.name = menu.name.Trim();
                existing.status = menu.status;
                existing.order = menu.order;
                existing.dateCreate = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true, msg = "Menu updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }

        // POST: Admin/CRUDmenus/Delete/5
        public JsonResult Delete(Guid id)
        {
            try
            {
                var menu = db.Menus.FirstOrDefault(i => i.menuId == id);
                if (menu == null)
                    return Json(new { success = false, msg = "Menu not found!" });

                db.Menus.Remove(menu);
                db.SaveChanges();
                return Json(new { success = true, msg = "Menu deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
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

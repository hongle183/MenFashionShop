﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopOnline.Models;

namespace shopOnline.Controllers
{
    public class HomeController : Controller
    {
        menfsEntities1 db = new menfsEntities1();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(Contact contact)
        {
            contact.dateContact = DateTime.Now;
            db.Contacts.Add(contact);
            var result = db.SaveChanges();
            if (result > 0)
            {
                ViewBag.MessageSuccess = "Your message has been received by us. We will contact you as soon as possible.";
            }
            return View(contact);
        }
        public ActionResult Error()
        {
            return View();
        }    
        [HttpGet]
        public ActionResult EditProfie(Guid memberId)
        {
            Member member = db.Members.Find(memberId);
            Session["imgPath"] = member.avatar;
            return View(member);
        }            
        [HttpPost]
        public ActionResult EditProfie(Member member, HttpPostedFileBase uploadFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (uploadFile != null)
                    {
                        var fileName = Path.GetFileName(uploadFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/img/avatar"), fileName);

                        member.avatar = "~/Content/img/avatar/" + fileName;
                        db.Entry(member).State = System.Data.Entity.EntityState.Modified;

                        string oldImgPath = Request.MapPath(Session["imgPath"].ToString()); // Lấy đường dẫn ảnh (absolute path)
                        var avatarName = Session["imgPath"].ToString(); // Lấy đường dẫn ảnh (relative path)
                        var checkAvatart = db.Members.Where(model => model.avatar == avatarName).ToList(); // Kiểm tra ảnh có trùng với avatar của member nào không

                        if (db.SaveChanges() > 0)
                        {
                            uploadFile.SaveAs(path);
                            if (System.IO.File.Exists(oldImgPath) && checkAvatart.Count < 2) // Nếu tồn tại hình trong folder và không member nào có hình này thì xóa ra khỏi folder
                            {
                                System.IO.File.Delete(oldImgPath);
                            }
                            var info = db.Members.Where(model => model.memberId == member.memberId).SingleOrDefault();// Lấy thông tin mới cập nhập lưu vào session
                            Session["info"] = info;
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        member.avatar = Session["imgPath"].ToString();
                        db.Entry(member).State = System.Data.Entity.EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            var info = db.Members.Where(model => model.memberId == member.memberId).SingleOrDefault();// Lấy thông tin mới cập nhập lưu vào session
                            Session["info"] = info;
                            return RedirectToAction("Index");
                        }
                    }
                }
                ViewBag.roleId = new SelectList(db.Roles, "roleId", "roleName", member.roleId);
                return View(member);
            }
            catch (Exception ex)
            {
                TempData["msgEditProfieFailed"] = "Edit failed! " + ex.Message;
                return RedirectToAction("Index");
            }
        }    
        [HttpGet]
        public ActionResult ChangePassword(Guid memberId)
        {
            Member member = db.Members.Find(memberId);
            return View();
        }        
        [HttpPost]
        public ActionResult ChangePassword(Member member, FormCollection collection)
        {
            try
            {
                var CurrentPassword = collection["CurPassword"]; // Mật khẩu hiện tại
                var NewPassword = collection["NewPassword"]; // Mật khẩu mới
                //var ConfirmPassword = collection["Confirm"];

                CurrentPassword = Encryptor.MD5Hash(CurrentPassword.Trim());
                NewPassword = Encryptor.MD5Hash(NewPassword.Trim());

                var check = db.Members.Where(model => model.password == CurrentPassword && model.memberId == member.memberId).FirstOrDefault();
                if (check != null)
                {
                    check.password = NewPassword;
                    db.SaveChanges();
                    TempData["msgChangePassword"] = "Successfully change password!";
                    return RedirectToAction("index");
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect your password!");
                    return View(member);
                }
            }
            catch (Exception ex)
            {
                TempData["msgChangePasswordFailed"] = "Edit failed! " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public ActionResult MyOrder(Guid memberId)
        {
            var orders = db.Invoinces.OrderByDescending(model => model.dateCreate).Where(model => model.memberId == memberId).ToList();
            return View(orders);
        }
        [HttpGet]
        public ActionResult InvoinceDetail(Guid invoinceId)
        {
            var detail = db.InvoinceDetails.Where(model => model.invoinceId == invoinceId).Include(model => model.Product).ToList();
            var infor = db.Invoinces.Where(model => model.invoinceId == invoinceId).Include(model => model.Member).FirstOrDefault();
            ViewBag.invoinceId = invoinceId;
            Session["information"] = infor;

            return View(detail);
        }
        public ActionResult Delete(Guid id)
        {
            List<InvoinceDetail> ctdh = db.InvoinceDetails.Where(model => model.invoinceId == id).ToList();
            foreach (var i in ctdh)
            {
                db.InvoinceDetails.Remove(i);
            }
            db.SaveChanges();
            Invoince invoince = db.Invoinces.Find(id);
            db.Invoinces.Remove(invoince);
            TempData["msgDeleteOrder"] = "Successfully delete order!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public PartialViewResult ProductHome()
        {
            var list = db.Products.OrderByDescending(model => model.dateCreate).Take(8).ToList();
            return PartialView(list);
        }
        public PartialViewResult BlogHome()
        {
            var list = db.Articles.OrderByDescending(model => model.dateCreate).Take(3).ToList();
            return PartialView(list);
        }
    }
}
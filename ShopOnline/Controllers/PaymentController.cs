using System;
using System.Collections.Generic;
using ShopOnline.Models;
using System.Web.Mvc;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.Linq;
using System.Net;
using CSharpVitamins;

namespace ShopOnline.Controllers
{
    public class PaymentController : Controller
    {
        menfsEntities db = new menfsEntities();
        
        // GET: Payment
        public ActionResult CreatePaymentUrl(string invoinceId)
        {
            Member user = (Member)Session["info"];
            if (!Guid.TryParse(invoinceId, out Guid parsedId))
            {
                return RedirectToAction("MyOrder", "Home", new { memberId = user.memberId });
            }

            Invoince bill = db.Invoinces.Find(parsedId);
            if (bill == null)
            {
                return RedirectToAction("MyOrder", "Home", new { memberId = user.memberId });
            }            

            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"];

            // Tạo danh sách tham số gửi đến VNPay
            Dictionary<string, string> vnp_Params = new Dictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", vnp_TmnCode },
            { "vnp_Amount", (bill.totalMoney * 100).ToString() }, // Số tiền tính bằng VND (x100)
            { "vnp_BankCode", "" },
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", invoinceId }, // Mã giao dịch duy nhất
            { "vnp_OrderInfo", "Thanh toán đơn hàng " + invoinceId },
            { "vnp_OrderType", "orther" },
            { "vnp_Locale", "vn" },
            { "vnp_ReturnUrl", vnp_Returnurl },
            { "vnp_IpAddr", Request.UserHostAddress },
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
        };

            // Tạo query string từ tham số
            string queryString = GenerateQueryString(vnp_Params);

            // Tạo chữ ký
            string vnp_SecureHash = HmacSHA512(vnp_HashSecret, queryString);

            // Tạo URL thanh toán
            vnp_Url += "?" + queryString + "&vnp_SecureHash=" + vnp_SecureHash;

            return Redirect(vnp_Url);
        }

        public ActionResult PaymentReturn()
        {
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            var vnp_Params = Request.QueryString.AllKeys.ToDictionary(k => k, k => Request.QueryString[k]);

            // Kiểm tra xem key "vnp_SecureHash" có tồn tại không
            string vnp_SecureHash = vnp_Params.ContainsKey("vnp_SecureHash") ? vnp_Params["vnp_SecureHash"] : "";
            vnp_Params.Remove("vnp_SecureHash");

            // Sắp xếp tham số theo thứ tự alphabet
            vnp_Params = vnp_Params.OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.Value);

            // Tạo query string từ các tham số
            string queryString = GenerateQueryString(vnp_Params);

            // Tính toán hash với chuỗi dữ liệu đã tạo
            string computedHash = HmacSHA512(vnp_HashSecret, queryString);

            // So sánh hash tính toán với hash từ tham số trả về
            if (computedHash.Equals(vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase))
            {
                string invoinceId = vnp_Params["vnp_TxnRef"];
                ShortGuid gid = (ShortGuid)invoinceId;
                var item = db.Invoinces.Where(model => model.invoinceId == gid.Guid).FirstOrDefault();
                if (vnp_Params["vnp_ResponseCode"] == "00")
                {
                    item.paymentMethod = "vnpay";
                    item.paymentStatus = "paid";                    
                    item.transactionId = vnp_Params["vnp_TransactionNo"];
                    db.SaveChanges();

                    ViewBag.times = vnp_Params["vnp_PayDate"];
                    return View(item);
                }
                else
                {
                    item.paymentStatus = "failed";
                    db.SaveChanges();
                    TempData["msgPaidFailed"] = "Thanh toán thất bại!";
                }
            }
            else
            {
                TempData["msgPaidFailed"] = "Đã xảy ra lỗi! Vui lòng thử lại";
            }

            Member user = (Member)Session["info"];
            return RedirectToAction("MyOrder", "Home", new { memberId = user.memberId });
        }

        private string GenerateQueryString(Dictionary<string, string> vnp_Params)
        {
            StringBuilder data = new StringBuilder();

            // Sắp xếp tham số theo thứ tự alphabet
            vnp_Params = vnp_Params.OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.Value);

            foreach (KeyValuePair<string, string> kv in vnp_Params)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            // Xóa dấu "&" cuối cùng nếu có
            string queryString = data.ToString();
            if (queryString.Length > 0)
            {
                queryString = queryString.Remove(queryString.Length - 1, 1);
            }

            return queryString;
        }

        public static String HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
    }
}
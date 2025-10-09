using System;
using ShopOnline.Models;
using System.Web.Mvc;
using System.Configuration;
using System.Linq;

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

            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (bill.totalMoney * 100).ToString()); // Số tiền tính bằng VND (x100)
            vnpay.AddRequestData("vnp_BankCode", "");
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng " + invoinceId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", invoinceId); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));
            //Billing
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                Guid invoinceId = Guid.Parse(vnpay.GetResponseData("vnp_TxnRef"));
                 var item = db.Invoinces.Where(model => model.invoinceId == invoinceId).FirstOrDefault();

                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (item == null)
                    {
                        TempData["msgPaidFailed"] = "Hóa đơn không tồn tại!";
                        return RedirectToAction("MyOrder", "Home");
                    }

                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        item.paymentMethod = "vnpay";
                        item.paymentStatus = "paid";
                        item.transactionId = vnpay.GetResponseData("vnp_TransactionNo");
                        db.SaveChanges();

                        ViewBag.times = vnpay.GetResponseData("vnp_PayDate");
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
                    TempData["msgPaidFailed"] = "Có lỗi trong quá trình xử lý.";
                }
            }      

            Member user = (Member)Session["info"];
            return RedirectToAction("MyOrder", "Home", new { memberId = user.memberId});
        }
    }
}
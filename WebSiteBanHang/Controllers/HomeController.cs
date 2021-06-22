using Capcha.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    
    public class HomeController : Controller
    {

        // GET: Home
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        public ActionResult Index()
        {
            // Lần lượt tạo các view bag để lấy list sản phẩm từ CSDL
            // List điện thoại mới nhất
            var lstDTM = db.SanPhams.Where(n => n.MaLoaiSP == 1 && n.Moi == 1 && n.DaXoa == false);
            ViewBag.ListDTM = lstDTM;

            // List lap top mới nhất
            var lstLT = db.SanPhams.Where(n => n.MaLoaiSP == 2 && n.Moi == 1 && n.DaXoa == false);
            ViewBag.ListLTM = lstLT;

            // List mays tinhs bang mới nhất
            var lstMTB = db.SanPhams.Where(n => n.MaLoaiSP == 3 && n.Moi == 1 && n.DaXoa == false);
            ViewBag.ListMTBM = lstMTB;
            return View();
        }
        public ActionResult MenuPartial()
        {
            // Truy vấn lấy list sp
            var lstSP = db.SanPhams;
            return PartialView(lstSP);
        }

        [HttpGet]
        public ActionResult DangKy()
        {
            ViewBag.CauHoi = new SelectList(LoadCauHoi());
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(ThanhVien tv)
        {
            if (ModelState.IsValid)
            {

                // Kiểm tra capcha
                ViewBag.CauHoi = new SelectList(LoadCauHoi());


                tv.MaLoaiTV = 3;
                db.ThanhViens.Add(tv); // add vào dataset trước

                db.SaveChanges(); // Lấy từ dataset và chuyển qua csdl
                ViewBag.ThongBao = "Thêm thành công";

                return View();
            }
            ViewBag.ThongBao = "Thêm thất bại";
            return View();

            // Thêm khách hàng vào csdl

        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            
            return View();
        }

        // Cách 1 đăng nhập dùng session
        /*[HttpPost]
        public ActionResult DangNhap(FormCollection frm)
        {
            // Kiểm tra tên đn mk,
            string sTaiKhoan = frm["TaiKhoan"].ToString();
            string sMatKhau = frm["MatKhau"].ToString();
            ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.TaiKhoan == sTaiKhoan && n.MatKhau == sMatKhau);
            if(tv != null)
            {
                Session["TaiKhoan"] = tv;
                return RedirectToAction("Index");

            }
            return Content("Tài khoản hoặc mật khẩu không đúng!");

        }*/

        [HttpPost]
        public ActionResult DangNhap(FormCollection frm)
        {
            string taiKhoan = frm["TaiKhoan"].ToString();
            string matKhau = frm["MatKhau"].ToString();
            // Truy vấn kiểm tra đăng nhập  lấy thông tin thành viên
            ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.TaiKhoan == taiKhoan && n.MatKhau == matKhau);
            if(tv != null)
            {
                // Lấy ra list quyền  của thành viên  tương ứng với loại thành viên
                var lstQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == tv.MaLoaiTV);
                string Quyen = "";
                if (lstQuyen.Count() != 0)
                {
                    foreach (var item in lstQuyen)
                    {
                        Quyen += item.Quyen.MaQuyen + ",";
                    }
                    Quyen = Quyen.Substring(0, Quyen.Length - 1); // Cắt dấu phẩy
                    PhanQuyen(tv.TaiKhoan.ToString(), Quyen);
                    Session["TaiKhoan"] = tv;
                    return Content("<script>window.location.reload();</script>");
                }
            }
            return Content("Tài khoản hoặc mật khẩu không đúng!");

        }

        public void PhanQuyen(string TaiKhoan, string Quyen)
        {
            // Khởi tạo
            FormsAuthentication.Initialize();
            var ticket = new FormsAuthenticationTicket(
                1,
                TaiKhoan, // user
                DateTime.Now, // Thời gian khởi tạo
                DateTime.Now.AddHours(3), // Thời gian kết thúc
                false,
                Quyen, // permission.. "admin" or for more than one "admin,marketing,sales"
                FormsAuthentication.FormsCookiePath); // Tự lấy ra đường dẫn của cookie

            // Lưu ticket vs cookie
            // Mã hóa tất cả thông tin của ticket
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            
            // Kiểm tra xem cookie đã hiện hữu chưa
            if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;
            Response.Cookies.Add(cookie);

        }



        //Tạo trang ngăn chặn quyền truy cậpp
        public ActionResult LoiPhanQuyen()
        {
            return View();
        }









        // cách 1 dùng session 
      /*  public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("Index");
        }*/

        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }



        public List<string> LoadCauHoi()
        {
            List<string> lstCauHoi = new List<string>();
            lstCauHoi.Add("Con vật mà bạn yêu thích?");
            lstCauHoi.Add("Ca sĩ mà bạn yêu thích?");
            lstCauHoi.Add("Công việc mà bạn yêu thích?");
            return lstCauHoi;
        }
    }
}
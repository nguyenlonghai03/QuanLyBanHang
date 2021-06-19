using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    public class ThongKeController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // GET: ThongKe
        public ActionResult Index()
        {
            ViewBag.SoNguoiTruyCap = HttpContext.Application["SoNguoiTruyCap"].ToString(); //Lấy số lượng người truy cập từ application
            ViewBag.SoNguoiDangOnline = HttpContext.Application["SoNguoiDangOnline"].ToString(); //Lấy số lượng người truy cập từ application
            ViewBag.TongDoanhThu = ThongKeDoanhThu(); // Thống kê tổng doanh thu
            ViewBag.TongDDH = ThongKeDonHang();
            ViewBag.TongThanhVien = ThongKeThanhVien();
            return View();
        }

        public decimal ThongKeDoanhThu()
        {
            // Thống kê theo tất cả doanh thu từ khi website thành lập
            decimal TongDoanhThu = db.ChiTietDonDatHangs.Sum(n => n.SoLuong * n.DonGia).Value;
            return TongDoanhThu;
        }

        public decimal ThongKeDoanhThuThang(int Thang, int Nam)
        {
            // Thống kê theo tất cả doanh thu theo tháng
            // Lsst ra những đon hàng nào có tháng năm tương ứng
            var lstDDH = db.DonDatHangs.Where(n => n.NgayDat.Value.Month == Thang && n.NgayDat.Value.Year == Nam);
            decimal TongTien = 0;

            //Duyệt chi tiết dđh đó và lâys tổng tiền của tất cả các sản phẩm thuộc đơn hàng đó
            foreach(var item in lstDDH)
            {
                TongTien += decimal.Parse(item.ChiTietDonDatHangs.Sum(n => n.SoLuong * n.DonGia).Value.ToString());
            }
            return TongTien;

        }

        public double ThongKeDonHang()
        {
            //Đếm đơn đặt hàng
            double slddh = db.DonDatHangs.Count();
            return slddh;
        }

        public double ThongKeThanhVien()
        {
            //Đếm đơn đặt hàng
            double slTV = db.ThanhViens.Count();
            return slTV;
        }




        // Giúp xóa bỏ những biến không xài nữa
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)

                    db.Dispose();

                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
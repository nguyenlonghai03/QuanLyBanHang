using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    // Roles là từ khóa của phân quyền, gán quyền
    [Authorize(Roles = "QuanTri,QuanLySanPham")] // Quyền quản trị được vào trang này
    // QUản trị có thể vào được tất cả action của controller đó
    public class KhachHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // GET: KhachHang
        // SO sánh quyền có đúng như vầy k
        [Authorize(Roles="QuanLySanPham")] // Quản lý sản phẩm sẽ đc vào action này
                                           // Quản trị ko được vào
        public ActionResult Index()
        {
            // Truy vấn dữ liệu thông qua câu lệnh
            // Đối tượng lstKhachHang sẽ lấy toàn bộ dữ liệu từ bảng khách hàng

            // Cách 1
            //var lstKH = from KH in db.KhachHangs select KH;   

            // Cách 2
            var lstKH = db.KhachHangs.ToList();
            return View(lstKH);
        }


        // Quản lý sp ko đc vào thằng này vì chưa xét quyền 
        [Authorize(Roles = "QuanTri")] // Chỉ quản trị vòa đc
        public ActionResult Index1()
        {

            var lstKhachHang = from KH in db.KhachHangs select KH;
            return View(lstKhachHang);
        }
        public ActionResult TruyVan1DoiTuong()
        {
            // Truy vấn 1 đối tượng
            // Bước 1 lấy ra danh sách khách hàng
            var lstKH = from kh in db.KhachHangs where kh.MaKH==1 select kh ;
            // Bước 2:
            // Lấy ra thằng đầu tiên do first
            //KhachHang Khang = lstKH.FirstOrDefault();

            // C2
            KhachHang Khang = db.KhachHangs.SingleOrDefault(n => n.MaKH == 1);
            return View(Khang);
        }

        public ActionResult SortDuLieu()
        {
            // Phương thức sắp sếp
            List<KhachHang> lstKH = db.KhachHangs.OrderBy(n => n.TenKH).ToList();
            return View(lstKH);
        }

        public ActionResult GroupDuLieu()
        {
            List<ThanhVien> lstKH = db.ThanhViens.OrderByDescending(n => n.TaiKhoan).ToList();
            return View(lstKH);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
using PagedList;

namespace WebSiteBanHang.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: SanPham
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        public ActionResult SanPhamStyle1Partial()
        {
            return PartialView();
        }
        public ActionResult SanPhamStyle2Partial()
        {
            return PartialView();
        }

        // Xây dựng trang chi tiết
        public ActionResult XemChiTiet(int? id, string tensp)
        {
            // Kiểm tra tham số truyền vào có rỗng hay ko
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            // Nếu không thì truy xuất tới csdl lấy ra sp tương ứng
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id && n.DaXoa == false);
            if (sp == null)
            {
                return HttpNotFound(); // Lỗi 404 ko tìm thấy
            }

            return View(sp);
        }

        // Xây dựng 1 action load sản phẩm theo mã loại sản phẩm và mã nhà sản xuất
        public ActionResult SanPham(int? MaLoaiSp, int? MaNSX,int? page)
        {
            /*if(Session["TaiKhoan"] == null || Session["TaiKhoan"] == "")
            {
                
                return RedirectToAction("Index", "Home");
            }*/
            if(MaLoaiSp == null || MaNSX == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Load sp theo 2 tiêu chí MaLoaiSP va MaNSX trong bảng sp
            var lstSP = db.SanPhams.Where(n => n.MaLoaiSP == MaLoaiSp && n.MaNSX == MaNSX);
            if(lstSP.Count() == 0)
            {
                return HttpNotFound();
            }
            if(Request.HttpMethod != "GET")
            {
                page = 1;
            }

            // Thực hiện chức năng phân trang
            // Tạo biến số sản phẩm trên trang
            int PageSize = 10;
            // Tạo biến thứ 2: Số trang hiện tại
            // Nếu page ko có thì gán = 1
            int PageNumber = (page ?? 1);
            ViewBag.MaLoaiSP = MaLoaiSp;
            ViewBag.MaNSX = MaNSX;

            return View(lstSP.OrderBy(n => n.MaSP).ToPagedList(PageNumber, PageSize));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
using PagedList;

namespace WebSiteBanHang.Controllers
{
    public class TimKiemController : Controller
    {
        // GET: TimKiem
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        [HttpGet]
        public ActionResult KQTimKiem(string sTuKhoa, int? page)
        {
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }

            // Thực hiện chức năng phân trang
            // Tạo biến số sản phẩm trên trang
            int PageSize = 10;
            // Tạo biến thứ 2: Số trang hiện tại
            // Nếu page ko có thì gán = 1
            int PageNumber = (page ?? 1);
           
            // TÌm kiếm theo tên sp
            var lstSP = db.SanPhams.Where(n => n.TenSP.Contains(sTuKhoa));
            ViewBag.TuKhoa = sTuKhoa;
            return View(lstSP.OrderBy(n => n.TenSP).ToPagedList(PageNumber, PageSize));
        }

        [HttpPost]
        public ActionResult LayTuKhoaTimKiem(string sTuKhoa)
        {
            // Không phải action nào cũng trả về view
            return RedirectToAction("KQTimKiem", new { @sTuKhoa = sTuKhoa }); // Gọi về 1 action khác

        }

        // Tìm kiếm ajax
        [HttpPost]
        public ActionResult KQTimKiemPartial(string sTuKhoa)
        {
           
            var lstSP = db.SanPhams.Where(n => n.TenSP.Contains(sTuKhoa));
            ViewBag.TuKhoa = sTuKhoa;
            return PartialView(lstSP.OrderBy(n => n.DonGia));
        }


    }
}
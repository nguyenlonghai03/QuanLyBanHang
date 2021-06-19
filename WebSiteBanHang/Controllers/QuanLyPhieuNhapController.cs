using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    public class QuanLyPhieuNhapController : Controller
    {
        // GET: QuanLyPhieuNhap
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
    
        [HttpGet]
        public ActionResult NhapHang()
        {

            ViewBag.MaNCC = db.NhaCungCaps;
            ViewBag.ListSanPham = db.SanPhams;

            return View();
        }

        [HttpPost]
        public ActionResult NhapHang(PhieuNhap model, IEnumerable<ChiTietPhieuNhap> lstModel)
        {
            SanPham sp;
            ViewBag.MaNCC = db.NhaCungCaps;
            ViewBag.ListSanPham = db.SanPhams;
            // Sau khi kiểm tra tất cả dữ liệu đầu vào
            model.DaXoa = false;
            db.PhieuNhaps.Add(model); // mới thêm vào data set thôi
            db.SaveChanges(); // Sau khi save thì mã phiếu nhập ms xuất hiện
            // save change để lấy đc mã pn gán cho lstChiTietPhieuNhap
            foreach(var item in lstModel)
            {
                // Cập nhật số lượng tồn
                sp = db.SanPhams.Single(n => n.MaSP == item.MaSP);
                sp.SoLuongTon += item.SoLuongNhap;

                // Gán mã phiếu nhập cho tất cả chi tiết phiếu nhập
                item.MaPN = model.MaPN;

            }
            // add range là add nguyên 1 cái lst
            db.ChiTietPhieuNhaps.AddRange(lstModel);
            db.SaveChanges();
            return View();
        }

        [HttpGet]
        public ActionResult DSSPHetHang()
        {
            // Danh sách sản phẩm gần hết hàng với số lượng tồn bé hơn hoặc bằng 5
            var lstSP = db.SanPhams.Where(n => n.DaXoa == false && n.SoLuongTon < 5);
            return View(lstSP);
        }


        [HttpGet]
        public ActionResult NhapHangDon(int? id)
        {
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            if(id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if(sp == null)
            {
                return HttpNotFound();
            }
            return View(sp);
        }

        [HttpPost]
        public ActionResult NhapHangDon(PhieuNhap model, ChiTietPhieuNhap ctpn)
        {
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", model.MaNCC);
            // Sau khi kiểm tra tất cả dữ liệu đầu vào
            model.DaXoa = false;
            db.PhieuNhaps.Add(model); // mới thêm vào data set thôi
            db.SaveChanges(); // Sau khi save thì mã phiếu nhập ms xuất hiện
            // save change để lấy đc mã pn gán cho lstChiTietPhieuNhap
            ctpn.MaPN = model.MaPN;
            model.NgayNhap = DateTime.Now;

            // cập nhật tồn
            SanPham sp = db.SanPhams.Single(n => n.MaSP == ctpn.MaSP);
            sp.SoLuongTon += ctpn.SoLuongNhap;

            // add range là add nguyên 1 cái lst
            db.ChiTietPhieuNhaps.Add(ctpn);
            db.SaveChanges();
            return View(sp);
        }


        // Giải phóng biến cho vùng nhớ
        //protected là từ khóa dành cho những đối tượng kế thừa
        // C# có dọn rác tự động
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(db != null)
                
                    db.Dispose();
                
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
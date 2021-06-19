using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    public class GioHangController : Controller
    {

        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // Lấy giỏ hàng
        public List<ItemGioHang> LayGioHang()
        {
            // GIỏ hàng đã tồn tại
            // Toán tử as để ép kiểm session giỏ hàng về list giỏ hàng
            // Nếu session ko tồn tại hoặc xảy ra lỗi thì sẽ trả về null
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if(lstGioHang == null)
            {
                // Nếu session giỏ hàng chưa tồn tại thì khởi tạo  giỏ hàng
                lstGioHang = new List<ItemGioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;

        }


        public ActionResult ThemGioHang(int MaSP, string strURL)
        {
            // Kiểm tra sản phẩm có tồn tại trong csdl hay ko
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if(sp == null)
            {
                // Trang đường dẫn không hợp lệ

                Response.StatusCode = 404;
                return null;

            }
            // Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = LayGioHang();
            // Trường hợp 1 sản phẩm đã tồn tại trong giỏ hàng thì tăng sl lên
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if(spCheck != null)
            {
                // Kiểm tra số lượng tồn trước khi cho khách hàng mua hàng
                if(sp.SoLuongTon < spCheck.SoLuong)
                {
                    return View("ThongBao");
                }
                spCheck.SoLuong++;
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                return Redirect(strURL);
            }

            
            ItemGioHang itemGH = new ItemGioHang(MaSP);
            if (sp.SoLuongTon < itemGH.SoLuong)
            {
                return View("ThongBao");
            }
            lstGioHang.Add(itemGH);
            return Redirect(strURL);
           
        }

        // Tính tổng số lượng
        public double TinhTongSoLuong()
        {
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if(lstGioHang == null)
            {
                return 0;
            }
            return lstGioHang.Sum(n => n.SoLuong);
        }

        //Tính tổng tiền
        public decimal TinhTongTien()
        {
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null)
            {
                return 0;
            }
            return lstGioHang.Sum(n => n.ThanhTien);
        }



        public ActionResult XemGioHang()
        {
            List<ItemGioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }
        public ActionResult GioHangPartial()
        {
            if(TinhTongSoLuong() == 0)
            {
                ViewBag.TongSoLuong = 0;
                ViewBag.TongTien = 0;
                return PartialView();
            }
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();

            return PartialView();
        }

        // CHỉnh sửa giỏ hàng
        [HttpGet]
        public ActionResult SuaGioHang(int MaSP)
        {
            // Kiểm tra sesion giỏ hàng tồn tại chưa
            if(Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Kiểm tra sản phẩm có tồn tại trong csdl hay ko
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                // Trang đường dẫn không hợp lệ

                Response.StatusCode = 404;
                return null;

            }
            // Lấy list giỏ hàng từ session 

            List<ItemGioHang> lstGioHang = LayGioHang();
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if(spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Lấy list giỏ hàng tạo giao diện
            ViewBag.GioHang = lstGioHang;

            return View(spCheck);
        }


        // Xử lý cập nhật giỏ hàng
        [HttpPost]
        public ActionResult CapNhatGioHang(ItemGioHang itemGH)
        {


            SanPham spCheck = db.SanPhams.SingleOrDefault(n => n.MaSP == itemGH.MaSP);
            if (spCheck.SoLuongTon < itemGH.SoLuong)
            {
                return View("ThongBao");

            }

            // Cập nhật số lượng trong session giỏ hàng
            // Bước 1: Lấy List<GioHang> từ sesion giỏ hàng
            List<ItemGioHang> lstGH = LayGioHang();
            // Bước 2: Lấy sản phẩm cần cập nhật từ trong list<GH> ra
            ItemGioHang itemGHUpdate = lstGH.Find(n => n.MaSP == itemGH.MaSP);
            // Bước 3 tiến hành cập nhật
            itemGHUpdate.SoLuong = itemGH.SoLuong;
            itemGHUpdate.ThanhTien = itemGHUpdate.SoLuong * itemGHUpdate.DonGia;

            return RedirectToAction("XemGioHang");
        }
    
        public ActionResult XoaGioHang (int MaSP)
        {
            // Kiểm tra sesion giỏ hàng tồn tại chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Kiểm tra sản phẩm có tồn tại trong csdl hay ko
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                // Trang đường dẫn không hợp lệ

                Response.StatusCode = 404;
                return null;

            }
            // Lấy list giỏ hàng từ session 

            List<ItemGioHang> lstGioHang = LayGioHang();
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            lstGioHang.Remove(spCheck);
            return RedirectToAction("XemGioHang");

        }

        // Xây dựng chức nawgn đặt hàng
        public ActionResult DatHang(KhachHang kh)
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            KhachHang khang = new KhachHang();
            if (Session["TaiKhoan"] == null)
            {
                // Thêm khách hàng vào bảng khách hàng
                // Đối với khách hàng chưa có tài khoản
                
                khang = kh;
                db.KhachHangs.Add(khang);
                db.SaveChanges();
            } else
            {
                ThanhVien tv = Session["TaiKhoan"] as ThanhVien;
                khang.TenKH = tv.HoTen;
                khang.DiaChi = tv.DiaChi;
                khang.Email = tv.Email;
                khang.SoDienThoai = tv.SoDienThoai;
                khang.MaThanhVien = tv.MaLoaiTV;
                db.KhachHangs.Add(khang);
                db.SaveChanges();
            }
            // Thêm đơn hàng
            DonDatHang ddh = new DonDatHang();
            ddh.MaKH = khang.MaKH;
            ddh.NgayDat = DateTime.Now;
            ddh.TinhTrangGiaoHang = false;
            ddh.DaThanhToan = false;
            ddh.UuDai = 0;
            ddh.DaHuy = false;
            ddh.DaXoa = false;
            db.DonDatHangs.Add(ddh);
            db.SaveChanges(); // Save change để phát sinh mã đơn hàng
            // Thêm chi tiết đơn đặt hàng
            List<ItemGioHang> lstGH = LayGioHang();
            foreach(var item in lstGH)
            {
                ChiTietDonDatHang ctdh = new ChiTietDonDatHang();
                ctdh.MaDDH = ddh.MaDDH;
                ctdh.MaSP = item.MaSP;
                ctdh.TenSP = item.TenSP;
                ctdh.SoLuong = item.SoLuong;
                ctdh.DonGia = item.DonGia;
                db.ChiTietDonDatHangs.Add(ctdh);
            }
            db.SaveChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XemGioHang");
        }



        // Thêm giỏ hàng ajax
        public ActionResult ThemGioHangAjax(int MaSP, string strURL)
        {
            // Kiểm tra sản phẩm có tồn tại trong csdl hay ko
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                // Trang đường dẫn không hợp lệ

                Response.StatusCode = 404;
                return null;

            }
            // Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = LayGioHang();
            // Trường hợp 1 sản phẩm đã tồn tại trong giỏ hàng thì tăng sl lên
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck != null)
            {
                // Kiểm tra số lượng tồn trước khi cho khách hàng mua hàng
                if (sp.SoLuongTon < spCheck.SoLuong)
                {
                    return Content("<script>alert(\"Sản phẩm đã hết hàng!\")</script>");
                }
                spCheck.SoLuong++;
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                ViewBag.TongSoLuong = TinhTongSoLuong();
                ViewBag.TongTien = TinhTongTien();
                return PartialView("GioHangPartial");
            }


            ItemGioHang itemGH = new ItemGioHang(MaSP);
            if (sp.SoLuongTon < itemGH.SoLuong)
            {
                return Content("<script>alert(\"Sản phẩm đã hết hàng!\")</script>");
            }
            lstGioHang.Add(itemGH);
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();
            return PartialView("GioHangPartial");

        }

    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    [Authorize(Roles = "QuanTri,QuanLySanPham")]

    public class QuanLySanPhamController : Controller
    {

        // GET: QuanLySanPham
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        public ActionResult Index()
        {
            return View(db.SanPhams.Where(n => n.DaXoa == false).OrderByDescending(n => n.MaSP));
        }
        [HttpGet]
        public ActionResult TaoMoi()
        {
            // Load dropdownlist NCC
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai");
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX");

            
            return View();
        }

        // Tắt đi ms sd đc tinymce
        [ValidateInput(false)]
        [HttpPost]
        // Truyền vào mảng, khi truyền vào mảng những cái thẻ nào, html nào trùng cái name với nhau thì nó sẽ tự nhận đó là 1 phần tử của 1 mảng 
       // Nhớ thuộc tính name của các hình ảnh phải trùng nhau
        public ActionResult TaoMoi(SanPham sp, HttpPostedFileBase[] HinhAnh) 
        {
            // Load dropdownlist NCC
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai");
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX");
            // Kiểm tra hình tồn tại trong csdl chưua
            int loi = 0;
            for(int i = 0; i < HinhAnh.Count(); i ++)
            {
                if(HinhAnh[i] != null)
                {
                    if (HinhAnh[i].ContentLength > 0)
                    {
                        // Kiểm tra định dạng hình ảnh
                        if (HinhAnh[i].ContentType != "image/jpeg" && HinhAnh[i].ContentType != "image/png" && HinhAnh[i].ContentType != "image/gif" && HinhAnh[i].ContentType != "image/jpg")
                        {
                            ViewBag.upload += "Hình ảnh" + i + " không hợp lệ<br/>";
                            loi++;
                        }
                        else
                        {
                            // Lấy tên hình ảnh
                            var fileName = Path.GetFileName(HinhAnh[i].FileName);
                            // Lấy hình ảnh chuyển vào thư mục hình ảnh
                            var path = Path.Combine(Server.MapPath("~/Content/images/product-slide/"), fileName);
                            if (System.IO.File.Exists(path))
                            {
                                ViewBag.upload = "Hình ảnh đã tồn tại";
                                loi++;
                                return View();

                            }
                            else
                            {
                                // Lấy hình ảnh đưa vào thư mục chứa hình ảnh
                                HinhAnh[i].SaveAs(path);
                                sp.HinhAnh = fileName;

                            }
                        }
                    }
                }
               
            }
            

            if(loi > 0)
            {
                return View(sp);
            }
            db.SanPhams.Add(sp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult ChinhSua(int? id)
        {
            // Lấy sp cần chỉnh sửa
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
            // Load dropdownlist NCC
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX", sp.MaNSX);
            ViewBag.NgayCapNhat = sp.NgayCapNhat.Value.ToString("yyyy-MM-dd");
            return View(sp);
        }

        // Sửa
        [ValidateInput(false)] // Nhớ có tinymce thì phải có cái này, đề phòng mã độc
        [HttpPost]
        public ActionResult ChinhSua(SanPham model)
        {
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == model.MaSP);

            sp.TenSP = model.TenSP;
            sp.DonGia = model.DonGia;
            sp.NgayCapNhat = model.NgayCapNhat;
            sp.CauHinh = model.CauHinh;
            sp.MoTa = model.MoTa;
            sp.HinhAnh = model.HinhAnh;
            sp.SoLanMua = model.SoLanMua;
            sp.SoLuongTon = model.SoLuongTon;
            sp.LuotXem = model.LuotXem;
            sp.LuotBinhChon = model.LuotBinhChon;
            sp.MaNCC = model.MaNCC;
            sp.MaNSX = model.MaNSX;
            sp.MaLoaiSP = model.MaLoaiSP;
            sp.DaXoa = model.DaXoa;

            db.SaveChanges();


            // Load dropdownlist NCC
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", model.MaNCC);
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai", model.MaLoaiSP);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX", model.MaNSX);
            ViewBag.NgayCapNhat = model.NgayCapNhat.Value.ToString("yyyy-MM-dd");

            return View(model);
        }

        [HttpGet]
        public ActionResult Xoa(int? id)
        {

            // Lấy sp cần chỉnh sửa
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            // Load dropdownlist NCC
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX", sp.MaNSX);
            ViewBag.NgayCapNhat = sp.NgayCapNhat.Value.ToString("yyyy-MM-dd");
            return View(sp);
        }

        /// Xóa
        [HttpPost]
        public ActionResult Xoa(int id)
        {
            ChiTietDonDatHang ctddh = db.ChiTietDonDatHangs.SingleOrDefault(n => n.MaSP == id);
            if(ctddh != null)
            {
                return Content("<script>alert(\"Sản phẩm đang tồn tại trong đơn đặt hàng của khách hàng!\")</script>");
            }
            // Lấy sp cần chỉnh sửa
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham model = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            // Load dropdownlist NCC
            db.SanPhams.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        

    }
}
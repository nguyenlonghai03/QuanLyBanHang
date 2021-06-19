using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace WebSiteBanHang
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // Khi ứng dụng chạy sẽ chạy hàm này
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Khi người dùng vào trang web thì sẽ khởi tạo cái này
            Application["SoNguoiTruyCap"] = 0;
            Application["SoNguoiDangOnline"] = 0;
        }

        protected void Session_Start()
        {
            Application.Lock(); // Dùng để đồng bộ hóa nếu ko thì 3000 ng truy cập cùng lúc thì sẽ bị loạn
            Application["SoNguoiTruyCap"] = (int)Application["SoNguoiTruyCap"] + 1;
            Application["SoNguoiDangOnline"] = (int)Application["SoNguoiDangOnline"] + 1;
            //Application["Online"] = (int)Application["Online"] + 1;
            Application.UnLock();
        }

        protected void Session_End()
        {
            // Trong webconfig <sessionState timeout="1"></sessionState> 1 phút out session
            Application.Lock(); // Dùng để đồng bộ hóa nếu ko thì 3000 ng truy cập cùng lúc thì sẽ bị loạn
            
            Application["SoNguoiDangOnline"] = (int)Application["SoNguoiDangOnline"] - 1;

            Application.UnLock();

        }
        public void Application_End()
        {
            Application.Lock();
            Application["SoNguoiDangOnline"] = (int)Application["SoNguoiDangOnline"] - 1;
            Application.UnLock();
        }

        // mỖI KHI ĐỤNG TS SERVER THÌ SẼ CHẠY VÀO CÁI NÀY
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            // Câu truy vấn trả về cookie
            var TaiKhoanCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if(TaiKhoanCookie != null) // Nếu như tồn tại tức đã đăng nhập rồi
            {
                // Bên kia encrypt mã hóa bên đây decrypt giải mã
                var authTicket = FormsAuthentication.Decrypt(TaiKhoanCookie.Value);
                var Quyen = authTicket.UserData.Split(new Char[] { ',' });
                var userPrincipal = new GenericPrincipal(new GenericIdentity(authTicket.Name), Quyen);
                Context.User = userPrincipal;
            }
        }  
    }
}

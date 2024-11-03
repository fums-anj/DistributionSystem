using BH.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BHWeb.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        public static string _userId { get; set; }
        public static string _fullName { get; set; }
        public static string _userName { get; set; }
        public static int? _shopId { get; set; }
        public static int? _CompanyId { get; set; }
        public static string _userRole { get; set; }
        public static bool IsAdmin { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = ((ControllerBase)filterContext.Controller).ControllerContext.ActionDescriptor.ControllerName;
            var actionName = ((ControllerBase)filterContext.Controller).ControllerContext.ActionDescriptor.ActionName;
            var controller = (ControllerBase)filterContext.Controller;
            if (User != null && User.Claims != null && User.Claims.Count() > 0)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                _userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                _userRole = HttpContext.User.FindFirstValue(ClaimTypes.Role);
                _userName = HttpContext.User.FindFirstValue(ClaimTypes.Name);
                if (_userRole == SDRoles.Role_ShopAdmin && controllerName == "Customers" && actionName == "AccessDenied")
                {
                    filterContext.Result = controller.RedirectToAction("Index", "Home", new { area = "Admin" });
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}

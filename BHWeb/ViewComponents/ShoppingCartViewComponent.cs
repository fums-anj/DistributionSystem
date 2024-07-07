using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BHWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SDSessionCart.SessionCart) != null)
                {
                    return View(HttpContext.Session.GetInt32(SDSessionCart.SessionCart));
                }
                else
                {
                    HttpContext.Session.SetInt32(SDSessionCart.SessionCart,
                        _unitOfWork.ShopCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32(SDSessionCart.SessionCart));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}

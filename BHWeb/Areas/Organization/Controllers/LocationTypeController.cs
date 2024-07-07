using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.OrganizationManagement;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BHWeb.Areas.Organization.Controllers;
[Area("Organization")]
[Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin)]
public class LocationTypeController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private ApplicationUser applicationUser;
    public LocationTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
    }

    public IActionResult Index()
    {
        if (applicationUser == null)
        {
            applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
        }
        IEnumerable<LocationType> objList = _unitOfWork.LocationType.GetAll().Where(x => x.ShopId == _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId).ShopId);
        return View(objList);
    }

    //GET
    public IActionResult Create()
    {
        return View();
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(LocationType obj)
    {
        if (ModelState.IsValid)
        {
            obj.CreatedBy = _userId;
            obj.CreatedDate = DateTime.Now;
            obj.ShopId = applicationUser.ShopId;
            obj.IsDisable = false;
            _unitOfWork.LocationType.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "LocationType created successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    //GET
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        var FromDbFirst = _unitOfWork.LocationType.GetFirstOrDefault(u => u.Id == id);

        if (FromDbFirst == null)
        {
            return NotFound();
        }

        return View(FromDbFirst);
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(LocationType obj)
    {

        if (ModelState.IsValid)
        {
            obj.ModifiedBy = _userId;
            obj.ModifiedDate = DateTime.UtcNow;
            _unitOfWork.LocationType.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "LocationType updated successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        var FromDbFirst = _unitOfWork.LocationType.GetFirstOrDefault(u => u.Id == id);

        if (FromDbFirst == null)
        {
            return NotFound();
        }

        return View(FromDbFirst);
    }

    //POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePOST(int? id)
    {
        var obj = _unitOfWork.LocationType.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return NotFound();
        }

        _unitOfWork.LocationType.Remove(obj);
        _unitOfWork.Save();
        TempData["success"] = "LocationType deleted successfully";
        return RedirectToAction("Index");

    }
}


using BH.DataAccess.Data;
using BH.Models.OrganizationManagement;
using BH.Models;
using BH.Utility;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BH.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly BHContext _db;

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            BHContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }


        public void Initialize()
        {
            //migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(SDRoles.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SDRoles.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SDRoles.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SDRoles.Role_User_Indi)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SDRoles.Role_User_Comp)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SDRoles.Role_ShopAdmin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SDRoles.Role_ShopUser)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SDRoles.Role_ShopEmployee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SDRoles.Role_Saleman)).GetAwaiter().GetResult();

                //if roles are not created, then we will create admin user as well

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@buyhappily.net",
                    Email = "admin@buyhappily.net",
                    FirstName = "Muhammad Shahzad",
                    LastName = "Anjum",
                    PhoneNumber = "+923445011525",
                    StreetAddress = "Malakwal, Mandi Bahauddin",
                    State = "Punjab",
                    PostalCode = "50530",
                    City = "Malakwal"
                }, "Password@123").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@buyhappily.net");

                _userManager.AddToRoleAsync(user, SDRoles.Role_Admin).GetAwaiter().GetResult();
            }
            //if (!_roleManager.RoleExistsAsync(SDRoles.Role_Saleman).GetAwaiter().GetResult())
            //{
            //    _roleManager.CreateAsync(new IdentityRole(SDRoles.Role_Saleman)).GetAwaiter().GetResult();
            //}
            return;
        }
    }
}

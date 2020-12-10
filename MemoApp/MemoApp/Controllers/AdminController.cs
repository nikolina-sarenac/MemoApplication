using MemoApp.BusinessLogic.Interfaces;
using MemoApp.Common;
using MemoApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemoApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IMemoService _memoService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(IMemoService memoService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _memoService = memoService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Roles()
        {
            if (ModelState.IsValid)
            {
                var roles = _memoService.GetAllRoles();
                return View(roles.Value);
            }
            return View("Error", new ErrorViewModel() { RequestId = "404" });
        }

        public IActionResult Users()
        {
            var users = _memoService.GetAllUsers();
            return View(users.Value);
        }

        public IActionResult CreateRole()
        {
            if(ModelState.IsValid)
            {
                return View();
            }
            return Unauthorized();
        }

        public IActionResult CreateNewRole(RoleViewModel role)
        {
            if(ModelState.IsValid)
            {
                var result =_memoService.CreateRole(role);
                if (result.Status != StatusEnum.Success)
                    return View("Error", new ErrorViewModel() { RequestId = result.Message } );
                return RedirectToAction("Roles");
            }
            return Unauthorized();
        }

        public IActionResult DeleteRole(string id)
        {
            if(ModelState.IsValid)
            {
                var role = _roleManager.FindByIdAsync(id).Result;
                if (role != null)
                    return View(new RoleViewModel() { Id = role.Id, Name = role.Name });
                else
                    return View("Error", new ErrorViewModel() { RequestId = "role not found" });
            }
            return Unauthorized();
        }

        public IActionResult ConfirmDeleteRole(string id)
        {
            var feedback = _memoService.DeleteRole(id);
            if (ModelState.IsValid)
            {
                if (feedback.Status != StatusEnum.Success)
                {
                    return View("Error", new ErrorViewModel() { RequestId = feedback.Message });
                }
                return RedirectToAction("Roles");
            }
            else
                return Unauthorized();
        }

        public IActionResult RoleDetails(string id)
        {
            var role = _roleManager.FindByIdAsync(id).Result;
            if(ModelState.IsValid)
            {
                if (role != null)
                    return View(new RoleViewModel() { Id = role.Id, Name = role.Name });
                else
                    return View("Error", new ErrorViewModel() { RequestId = "role not found" });
            }
            return Unauthorized();
        }

        public IActionResult EditRole(string id)
        {
            var role = _roleManager.FindByIdAsync(id).Result;
            if (ModelState.IsValid)
            {
                if (role != null)
                    return View(new RoleViewModel() { Id = role.Id, Name = role.Name });
                else
                    return View("Error", new ErrorViewModel() { RequestId = "role not found" });
            }
            return Unauthorized();
        }

        public IActionResult SaveEditRole(RoleViewModel role)
        {
            var r = _roleManager.FindByIdAsync(role.Id).Result;
            if (ModelState.IsValid)
            {
                if (role != null)
                {
                    r.Name = role.Name;
                    var result = _roleManager.UpdateAsync(r).Result;
                    if (result.Succeeded)
                        return RedirectToAction("Roles");
                    else
                        return View("Error", new ErrorViewModel() { RequestId = "not able to update" });

                }
                else
                    return View("Error", new ErrorViewModel() { RequestId = "role not found" });
            }
            return Unauthorized();
        }

        public IActionResult AddOrChangeRole (string id) 
        {
            var roles = _roleManager.Roles.Select(r => new RoleViewModel { Id = r.Id, Name = r.Name } ).ToList();
            var user = _userManager.FindByIdAsync(id).Result;
            var roleName = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            string roleId;
            if (roleName != null)
                roleId = _roleManager.FindByNameAsync(roleName).Result.Id;
            else
            {
                roleId = "0";
                roleName = "";
            }

            var role = new RoleViewModel() { Id = roleId, Name = roleName };
                
            return View(new UserViewModel() { Id = user.Id, Email = user.Email, UserName = user.UserName, Role = role, AllRoles = roles});
        }

        [HttpPost]
        public IActionResult AddOrChangeRole (UserViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = _userManager.FindByIdAsync(model.Id).Result;
                var roles = _userManager.GetRolesAsync(user).Result;

                var res = _userManager.RemoveFromRolesAsync(user, roles).Result;

                var roleName = _roleManager.FindByIdAsync(model.Role.Id).Result.Name;

                var result = _userManager.AddToRoleAsync(user,roleName).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Users");
                }
                else
                    return Unauthorized();
            }
            return Unauthorized();
        }

        
    }
}

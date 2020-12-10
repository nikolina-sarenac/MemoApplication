using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MemoApp.BusinessLogic.Interfaces;
using MemoApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MemoApp.Controllers
{
    [Authorize]
    public class MemoController : Controller
    {
        private IMemoService _memoService;
        public MemoController(IMemoService memoService)
        {
            _memoService = memoService;
        }
        public IActionResult Index()
        {
            var result = _memoService.GetAll();
            return View(result.Value);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            var result = _memoService.GetMemoById(id);
            return View(result.Value);
        }

        public IActionResult Details(int id)
        {
            var result = _memoService.GetMemoById(id);
            return View(result.Value);
        }

        public IActionResult Delete(int id)
        {
            _memoService.DeleteMemoById(id);
            return RedirectToAction("Index");
            //return View(memo);
        }

        [HttpPost]
        public IActionResult AddNewMemo(CreateMemoViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.userId = userId;
            _memoService.CreateMemo(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditMemo (MemoViewModel model)
        {
            _memoService.EditMemo(model);
            return RedirectToAction("Index");
        }

        public IActionResult detailspartial(int id)
        {
            var memo = _memoService.GetMemoById(id).Value;
            return PartialView("_DetailsPartial", memo);
        }

        public IActionResult editpartial(int id)
        {
            var memo = _memoService.GetMemoById(id).Value;
            return PartialView("_EditPartial", memo);
        }

    }
}
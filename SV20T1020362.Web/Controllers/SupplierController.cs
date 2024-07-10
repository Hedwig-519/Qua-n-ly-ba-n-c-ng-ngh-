﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020362.BusinessLayers;
using SV20T1020362.DomainModels;
using SV20T1020362.Web.AppCodes;
using SV20T1020362.Web.Models;

namespace SV20T1020362.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class SupplierController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string CUSTOMER_SEARCH = "customer_search";//Tên biến dùng để lưu trong session

        public IActionResult Index()
        {
            //Lấy đầu vào tìm kiếm hiện đang lưu lại trong session
            PaginationSearchInput input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);
            //Trường hợp trong session chưa có điều kiện thì tạo điều kiện mới
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
        }
        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfSuppliers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new SupplierSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            // lưu lại điều kiện tìm kiếm vào trong session
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            Supplier model = new Supplier()
            {
                SupplierID = 0
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
            Supplier? model = CommonDataService.GetSupplier(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Supplier data)
        {
            try
            {
                ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật thông tin nhà cung cấp";
                //Kiểm soát đầu vào đưa các thông báo lỗi và trong ModelState (Nếu có)
                if (string.IsNullOrWhiteSpace(data.SupplierName))
                    ModelState.AddModelError("SupplierName", "Tên nhà cung câ không được để trống");
                if (string.IsNullOrWhiteSpace(data.ContactName))
                    ModelState.AddModelError("ContactName", "Tên liên lạc không được để trống");
                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError("Email", "Email không được để trống");
                if (string.IsNullOrEmpty(data.Province))
                    ModelState.AddModelError("Province", "Vui lòng chọn tỉnh thành");
                // Thông qua thuộc tính IsValid của ModelState kiểm tra xem có tồn tại lỗi hay không
                if (!ModelState.IsValid)
                {
                    return View("Edit", data);
                }
                if (data.SupplierID == 0)
                {
                    int id = CommonDataService.AddSupplier(data);
                }
                else
                {
                    bool result = CommonDataService.UpdateSupplier(data);

                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            };
            var model = CommonDataService.GetSupplier(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !CommonDataService.IsUsedSupplier(id);

            return View(model);
        }
    }
}
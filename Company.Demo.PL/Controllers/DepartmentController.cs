using Company.Demo.BLL.Interfaces;
using Company.Demo.DAL.Models;
using Company.Demo.PL.ViewModels.Departments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Company.Demo.BLL;

namespace Company.Demo.PL.Controllers
{
    /// <summary>
    /// MVC controller for Department management.
    /// </summary>
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// Displays a paged, sorted, and filtered list of departments.
        /// </summary>
        [HttpGet] //Get:/Department /index
        public IActionResult Index(int page = 1, int pageSize = 10, string? sortField = null, bool sortAsc = true, string? search = null)
        {
            var pagedResult = _departmentService.GetPaged(page, pageSize, sortField, sortAsc, search);
            return View(pagedResult);
        }

        /// <summary>
        /// Shows the create department form.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            if (!User.IsInRole("Admin"))
            {
                TempData["AccessDeniedMessage"] = "ليس لديك صلاحية الوصول لهذه الصفحة. يجب أن تكون أدمن.";
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        /// <summary>
        /// Handles department creation.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(DepartmentViewModel model)
        {
            if (!User.IsInRole("Admin"))
            {
                TempData["AccessDeniedMessage"] = "ليس لديك صلاحية تنفيذ هذه العملية. يجب أن تكون أدمن.";
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                var department = new Department
                {
                    Name = model.Name,
                    Code = model.Code,
                    DateOfCreation = model.DateOfCreation
                };
                var result = _departmentService.Add(department);
                if (result.Success)
                {
                    TempData["Message"] = "Department created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                TempData["Message"] = result.ErrorMessage ?? "An error occurred while creating the department.";
            }
            return View(model);
        }

        /// <summary>
        /// Shows department details.
        /// </summary>
        [HttpGet] //Get /Department /Details
        public IActionResult Details(int? id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();

            var department = _departmentService.Get(id.Value);

            if (department is null)
                return NotFound();

            var viewModel = new DepartmentViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                DateOfCreation = department.DateOfCreation
            };
            return View(viewName, viewModel);
        }

        /// <summary>
        /// Shows the edit department form.
        /// </summary>
        [HttpGet] //Get /Department/Edit
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            return Details(id, "Edit");
        }

        /// <summary>
        /// Handles department editing.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int? id, DepartmentViewModel model)
        {
            try
            {
                if (id != model.Id)
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    var department = new Department
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Code = model.Code,
                        DateOfCreation = model.DateOfCreation
                    };
                    var result = _departmentService.Update(department);
                    if (result.Success)
                    {
                        TempData["Message"] = "Department updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    TempData["Message"] = result.ErrorMessage ?? "An error occurred while updating the department.";
                }
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, Ex.Message);
                TempData["Message"] = Ex.Message;
            }
            return View(model);
        }

        /// <summary>
        /// Shows the delete department confirmation.
        /// </summary>
        [HttpGet] //Get /Department/Delete
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            return Details(id, "Delete");
        }

        /// <summary>
        /// Handles department deletion.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromRoute] int? id, DepartmentViewModel model)
        {
            try
            {
                if (id != model.Id)
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    var department = new Department
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Code = model.Code,
                        DateOfCreation = model.DateOfCreation
                    };
                    var Count = _departmentService.Delete(department);
                    if (Count > 0)
                    {
                        TempData["Message"] = "Department deleted successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    TempData["Message"] = "An error occurred while deleting the department.";
                }
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, Ex.Message);
                TempData["Message"] = Ex.Message;
            }
            return View(model);
        }
    }
}

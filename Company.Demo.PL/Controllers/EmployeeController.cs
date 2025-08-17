using AutoMapper;
using Company.Demo.BLL.Interfaces;
using Company.Demo.DAL.Models;
using Company.Demo.PL.ViewModels.Employees;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Company.Demo.BLL;

namespace Company.Demo.PL.Controllers
{
    /// <summary>
    /// MVC controller for Employee management.
    /// </summary>
    public class EmployeeController : Controller
    {
      
 private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        //private readonly IDepartmentRepository _departmentRepository;

        public EmployeeController(
            IEmployeeService employeeService,
            IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            //_departmentRepository = departmentRepository;
        }
        /// <summary>
        /// Displays a paged, sorted, and filtered list of employees.
        /// </summary>
        //Get:/Employee /index
        public IActionResult Index(string InputSearch, int page = 1, int pageSize = 10, string? sortField = null, bool sortAsc = true)
        {
            var pagedResult = _employeeService.GetPaged(page, pageSize, sortField, sortAsc, InputSearch);
            // Optionally, set ViewData/ViewBag for messages as before
            if (TempData["Message"] != null)
                ViewBag.TempDataMessage = TempData["Message"];
            return View(pagedResult);
        }
        /// <summary>
        /// Shows the create employee form.
        /// </summary>
        //Create
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            if (!User.IsInRole("Admin"))
            {
                TempData["AccessDeniedMessage"] = "ليس لديك صلاحية الوصول لهذه الصفحة. يجب أن تكون أدمن.";
                return RedirectToAction("Login", "Account");
            }

            //var departments = _departmentRepository.GetAll();

            //ViewData["Department"] = departments;



            return View();
        }

        /// <summary>
        /// Handles employee creation.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(EmployeeViewModel employeeVM)
        {
            if (!User.IsInRole("Admin"))
            {
                TempData["AccessDeniedMessage"] = "ليس لديك صلاحية تنفيذ هذه العملية. يجب أن تكون أدمن.";
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                var mappedEmp = _mapper.Map<Employee>(employeeVM);
                var result = _employeeService.Add(mappedEmp);
                if (result.Success)
                {
                    TempData["Message"] = "Employee created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                TempData["Message"] = result.ErrorMessage ?? "An error occurred while creating the employee.";
            }
            return View(employeeVM);
        }

        /// <summary>
        /// Shows employee details.
        /// </summary>
        #region Detail
        [HttpGet] //Get /Employee /Details
        public IActionResult Details(int? id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();

            var employee = _employeeService.Get(id.Value);

            if (employee is null)
                return NotFound();
          var mappedEmp=  _mapper.Map<EmployeeViewModel>(employee);

            return View(viewName, mappedEmp);

        }
        #endregion

        /// <summary>
        /// Shows the edit employee form.
        /// </summary>
        #region Edit
        [HttpGet] //Get /Employee/Edit
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            //if (id is null)
            //    return BadRequest(); //400

            //var Employee = _EmployeeRepository.Get(id.Value);

            //if (Employee is null)
            //    return NotFound();  //404
            //var departments = _departmentRepository.GetAll();

            //ViewData["Department"] = departments;


            //return View(Employee);
            return Details(id, "Edit");

        }
       

        /// <summary>
        /// Handles employee editing.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]//prevent any tool to edit such as postman
        public IActionResult Edit([FromRoute] int? id, EmployeeViewModel employeeVM)
        {
            try
            {
                if (id != employeeVM.Id)
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    var mappedEmp = _mapper.Map<Employee>(employeeVM);
                    var result = _employeeService.Update(mappedEmp);
                    if (result.Success)
                    {
                        TempData["Message"] = "Employee updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    TempData["Message"] = result.ErrorMessage ?? "An error occurred while updating the employee.";
                }
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, Ex.Message);
                TempData["Message"] = Ex.Message;
            }
            return View(employeeVM);
        }


        #endregion

        /// <summary>
        /// Shows the delete employee confirmation.
        /// </summary>
        #region Delete
        [HttpGet] //Get /Employee/Delete
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {

            return Details(id, "Delete");

        }
        /// <summary>
        /// Handles employee deletion.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]//prevent any tool to edit such as postman

     
        public IActionResult Delete([FromRoute] int? id, EmployeeViewModel employeeVM)
        {
            try
            {
                if (id != employeeVM.Id)
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    var mappedEmp = _mapper.Map<Employee>(employeeVM);
                    var Count = _employeeService.Delete(mappedEmp);
                    if (Count > 0)
                    {
                        TempData["Message"] = "Employee deleted successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    TempData["Message"] = "An error occurred while deleting the employee.";
                }
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, Ex.Message);
                TempData["Message"] = Ex.Message;
            }
            return View(employeeVM);
        }
        #endregion 
    }
}

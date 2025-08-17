using System;
using System.ComponentModel.DataAnnotations;

namespace Company.Demo.PL.ViewModels.Departments
{
    /// <summary>
    /// ViewModel for Department data used in views.
    /// </summary>
    public class DepartmentViewModel
    {
        /// <summary>
        /// Department ID.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Department name.
        /// </summary>
        [Required(ErrorMessage = "Name is required !!")]
        public string Name { get; set; } = null!;
        /// <summary>
        /// Department code.
        /// </summary>
        [Required(ErrorMessage = "Code is required !!")]
        public string Code { get; set; } = null!;
        /// <summary>
        /// Date the department was created.
        /// </summary>
        [Display(Name = "Date of Creation")]
        public DateTime DateOfCreation { get; set; }
    }
} 
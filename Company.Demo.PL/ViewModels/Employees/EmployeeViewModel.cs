using Company.Demo.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Company.Demo.PL.ViewModels.Employees
{
    /// <summary>
    /// ViewModel for Employee data used in views.
    /// </summary>
    public class EmployeeViewModel
    {
        /// <summary>
        /// Employee ID.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Employee name.
        /// </summary>
        [Required(ErrorMessage = "Name is Required !!")]
        [MaxLength(50, ErrorMessage = "Max Length of Name Is 50 Chars")]
        [MinLength(3, ErrorMessage = "Min Length Of Name Is 3 Chars")]
        public string Name { get; set; } = null!;
        /// <summary>
        /// Employee age.
        /// </summary>
        [Range(18, 65, ErrorMessage = "Age must be between 18 and 65")]
        public int? Age { get; set; }
        /// <summary>
        /// Employee address.
        /// </summary>
        [RegularExpression(@"^[0-9]{1,3}-[a-zA-Z]{5,10}-[a-zA-Z]{4,10}-[a-zA-Z]{5,10}$", ErrorMessage = "Address must be like 123-Street-City-Country")]
        public string? Address { get; set; }
        /// <summary>
        /// Employee salary.
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "Salary must be positive.")]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }
        /// <summary>
        /// Whether the employee is active.
        /// </summary>
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        /// <summary>
        /// Employee email address.
        /// </summary>
        [EmailAddress]
        public string? Email { get; set; }
        /// <summary>
        /// Employee phone number.
        /// </summary>
        [Phone]
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// Date the employee was hired.
        /// </summary>
        [Display(Name = "Hiring Date")]
        [DataType(DataType.Date)]
        public DateTime HiringDate { get; set; }
        /// <summary>
        /// Department ID (foreign key).
        /// </summary>
        [Required(ErrorMessage = "Department is required.")]
        public int? DepartmentId { get; set; }
        /// <summary>
        /// Navigation property for the employee's department.
        /// </summary>
        public Department? Department { get; set; }
    }
}

using Company.Demo.DAL.Models;
using System.Collections.Generic;

namespace Company.Demo.BLL.Interfaces
{
    /// <summary>
    /// Service interface for Employee business logic.
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Gets all employees.
        /// </summary>
        IEnumerable<Employee> GetAll();
        /// <summary>
        /// Gets an employee by its ID.
        /// </summary>
        Employee? Get(int id);
        /// <summary>
        /// Gets employees by name (search).
        /// </summary>
        IEnumerable<Employee> GetByName(string name);
        /// <summary>
        /// Adds a new employee with business validation.
        /// </summary>
        (bool Success, string? ErrorMessage) Add(Employee employee);
        /// <summary>
        /// Updates an existing employee with business validation.
        /// </summary>
        (bool Success, string? ErrorMessage) Update(Employee employee);
        /// <summary>
        /// Deletes an employee.
        /// </summary>
        int Delete(Employee employee);
        /// <summary>
        /// Gets a paged, sorted, and filtered list of employees.
        /// </summary>
        /// <param name="page">The page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="sortField">The field to sort by (Name, Email, Salary).</param>
        /// <param name="sortAsc">True for ascending, false for descending.</param>
        /// <param name="search">Optional search query (by name).</param>
        /// <returns>PagedResult of Employee.</returns>
        Company.Demo.BLL.PagedResult<Employee> GetPaged(int page, int pageSize, string? sortField, bool sortAsc, string? search);
    }
} 
using Company.Demo.DAL.Models;
using System.Collections.Generic;

namespace Company.Demo.BLL.Interfaces
{
    /// <summary>
    /// Service interface for Department business logic.
    /// </summary>
    public interface IDepartmentService
    {
        /// <summary>
        /// Gets all departments.
        /// </summary>
        IEnumerable<Department> GetAll();
        /// <summary>
        /// Gets a department by its ID.
        /// </summary>
        Department? Get(int id);
        /// <summary>
        /// Adds a new department with business validation.
        /// </summary>
        (bool Success, string? ErrorMessage) Add(Department department);
        /// <summary>
        /// Updates an existing department with business validation.
        /// </summary>
        (bool Success, string? ErrorMessage) Update(Department department);
        /// <summary>
        /// Deletes a department.
        /// </summary>
        int Delete(Department department);
        /// <summary>
        /// Gets a paged, sorted, and filtered list of departments.
        /// </summary>
        /// <param name="page">The page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="sortField">The field to sort by (Name, Code).</param>
        /// <param name="sortAsc">True for ascending, false for descending.</param>
        /// <param name="search">Optional search query (by name).</param>
        /// <returns>PagedResult of Department.</returns>
        Company.Demo.BLL.PagedResult<Department> GetPaged(int page, int pageSize, string? sortField, bool sortAsc, string? search);
    }
} 
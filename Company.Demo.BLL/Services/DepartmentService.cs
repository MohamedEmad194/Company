using Company.Demo.BLL.Interfaces;
using Company.Demo.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Company.Demo.BLL.Services
{
    /// <summary>
    /// Service implementation for Department business logic.
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMemoryCache _cache;
        public DepartmentService(IDepartmentRepository departmentRepository, IMemoryCache cache)
        {
            _departmentRepository = departmentRepository;
            _cache = cache;
        }
        /// <inheritdoc/>
        public IEnumerable<Department> GetAll()
        {
            return _cache.GetOrCreate("departments_all", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _departmentRepository.GetAll().ToList();
            });
        }
        /// <inheritdoc/>
        public Department? Get(int id) => _departmentRepository.Get(id);
        /// <inheritdoc/>
        public (bool Success, string? ErrorMessage) Add(Department department)
        {
            _cache.Remove("departments_all");
            _cache.Remove("departments_paged");
            if (_departmentRepository.GetAll().Any(d => d.Code == department.Code))
                return (false, "A department with this code already exists.");
            if (_departmentRepository.GetAll().Any(d => d.Name == department.Name))
                return (false, "A department with this name already exists.");
            var count = _departmentRepository.Add(department);
            return (count > 0, count > 0 ? null : "Failed to add department.");
        }
        /// <inheritdoc/>
        public (bool Success, string? ErrorMessage) Update(Department department)
        {
            _cache.Remove("departments_all");
            _cache.Remove("departments_paged");
            if (_departmentRepository.GetAll().Any(d => d.Code == department.Code && d.Id != department.Id))
                return (false, "A department with this code already exists.");
            if (_departmentRepository.GetAll().Any(d => d.Name == department.Name && d.Id != department.Id))
                return (false, "A department with this name already exists.");
            var count = _departmentRepository.Update(department);
            return (count > 0, count > 0 ? null : "Failed to update department.");
        }
        /// <inheritdoc/>
        public int Delete(Department department)
        {
            _cache.Remove("departments_all");
            _cache.Remove("departments_paged");
            return _departmentRepository.Delete(department);
        }
        /// <inheritdoc/>
        public Company.Demo.BLL.PagedResult<Department> GetPaged(int page, int pageSize, string? sortField, bool sortAsc, string? search)
        {
            string cacheKey = $"departments_paged_{page}_{pageSize}_{sortField}_{sortAsc}_{search}";
            return _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var query = _departmentRepository.GetAll().AsQueryable();
                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(d => d.Name.Contains(search));
                if (!string.IsNullOrWhiteSpace(sortField))
                {
                    switch (sortField.ToLower())
                    {
                        case "name":
                            query = sortAsc ? query.OrderBy(d => d.Name) : query.OrderByDescending(d => d.Name);
                            break;
                        case "code":
                            query = sortAsc ? query.OrderBy(d => d.Code) : query.OrderByDescending(d => d.Code);
                            break;
                        default:
                            query = query.OrderBy(d => d.Id);
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(d => d.Id);
                }
                var total = query.Count();
                var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new Company.Demo.BLL.PagedResult<Department>
                {
                    Items = items,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalItems = total,
                    SortField = sortField,
                    SortAscending = sortAsc,
                    SearchQuery = search
                };
            });
        }
    }
} 
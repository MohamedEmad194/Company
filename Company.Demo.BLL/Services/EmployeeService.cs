using Company.Demo.BLL.Interfaces;
using Company.Demo.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace Company.Demo.BLL.Services
{
    /// <summary>
    /// Service implementation for Employee business logic.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMemoryCache _cache;
        public EmployeeService(IEmployeeRepository employeeRepository, IMemoryCache cache)
        {
            _employeeRepository = employeeRepository;
            _cache = cache;
        }
        /// <inheritdoc/>
        public IEnumerable<Employee> GetAll()
        {
            return _cache.GetOrCreate("employees_all", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _employeeRepository.GetAll().ToList();
            });
        }
        /// <inheritdoc/>
        public Employee? Get(int id) => _employeeRepository.Get(id);
        /// <inheritdoc/>
        public IEnumerable<Employee> GetByName(string name) => _employeeRepository.GetByName(name);
        /// <inheritdoc/>
        public (bool Success, string? ErrorMessage) Add(Employee employee)
        {
            _cache.Remove("employees_all");
            _cache.Remove("employees_paged");
            if (string.IsNullOrWhiteSpace(employee.Email) || !Regex.IsMatch(employee.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return (false, "Invalid email format.");
            if (_employeeRepository.GetAll().Any(e => e.Email == employee.Email))
                return (false, "An employee with this email already exists.");
            if (employee.Salary <= 0)
                return (false, "Salary must be greater than zero.");
            if (employee.Age is null || employee.Age < 18 || employee.Age > 65)
                return (false, "Employee age must be between 18 and 65.");
            if (employee.DepartmentId is null)
                return (false, "Employee must be assigned to a department.");
            if (employee.HiringDate > DateTime.Now)
                return (false, "Hiring date cannot be in the future.");
            if (!string.IsNullOrWhiteSpace(employee.PhoneNumber) && _employeeRepository.GetAll().Any(e => e.PhoneNumber == employee.PhoneNumber && !string.IsNullOrWhiteSpace(e.PhoneNumber)))
                return (false, "An employee with this phone number already exists.");
            var count = _employeeRepository.Add(employee);
            return (count > 0, count > 0 ? null : "Failed to add employee.");
        }
        /// <inheritdoc/>
        public (bool Success, string? ErrorMessage) Update(Employee employee)
        {
            _cache.Remove("employees_all");
            _cache.Remove("employees_paged");
            if (string.IsNullOrWhiteSpace(employee.Email) || !Regex.IsMatch(employee.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return (false, "Invalid email format.");
            if (_employeeRepository.GetAll().Any(e => e.Email == employee.Email && e.Id != employee.Id))
                return (false, "An employee with this email already exists.");
            if (employee.Salary <= 0)
                return (false, "Salary must be greater than zero.");
            if (employee.Age is null || employee.Age < 18 || employee.Age > 65)
                return (false, "Employee age must be between 18 and 65.");
            if (employee.DepartmentId is null)
                return (false, "Employee must be assigned to a department.");
            if (employee.HiringDate > DateTime.Now)
                return (false, "Hiring date cannot be in the future.");
            if (!string.IsNullOrWhiteSpace(employee.PhoneNumber) && _employeeRepository.GetAll().Any(e => e.PhoneNumber == employee.PhoneNumber && e.Id != employee.Id && !string.IsNullOrWhiteSpace(e.PhoneNumber)))
                return (false, "An employee with this phone number already exists.");
            var count = _employeeRepository.Update(employee);
            return (count > 0, count > 0 ? null : "Failed to update employee.");
        }
        /// <inheritdoc/>
        public int Delete(Employee employee)
        {
            _cache.Remove("employees_all");
            _cache.Remove("employees_paged");
            return _employeeRepository.Delete(employee);
        }
        /// <inheritdoc/>
        public Company.Demo.BLL.PagedResult<Employee> GetPaged(int page, int pageSize, string? sortField, bool sortAsc, string? search)
        {
            string cacheKey = $"employees_paged_{page}_{pageSize}_{sortField}_{sortAsc}_{search}";
            return _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var query = _employeeRepository.GetAll().AsQueryable();
                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(e => e.Name.Contains(search));
                if (!string.IsNullOrWhiteSpace(sortField))
                {
                    switch (sortField.ToLower())
                    {
                        case "name":
                            query = sortAsc ? query.OrderBy(e => e.Name) : query.OrderByDescending(e => e.Name);
                            break;
                        case "email":
                            query = sortAsc ? query.OrderBy(e => e.Email) : query.OrderByDescending(e => e.Email);
                            break;
                        case "salary":
                            query = sortAsc ? query.OrderBy(e => e.Salary) : query.OrderByDescending(e => e.Salary);
                            break;
                        default:
                            query = query.OrderBy(e => e.Id);
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(e => e.Id);
                }
                var total = query.Count();
                var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new Company.Demo.BLL.PagedResult<Employee>
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
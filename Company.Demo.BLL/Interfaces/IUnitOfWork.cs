using System;
using System.Threading.Tasks;

namespace Company.Demo.BLL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        IDepartmentRepository Departments { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
} 
using Company.Demo.BLL.Interfaces;
using Company.Demo.DAL.Data.Contexts;
using System.Threading.Tasks;

namespace Company.Demo.BLL.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IEmployeeRepository Employees { get; }
        public IDepartmentRepository Departments { get; }

        public UnitOfWork(ApplicationDbContext context, IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _context = context;
            Employees = employeeRepository;
            Departments = departmentRepository;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
} 
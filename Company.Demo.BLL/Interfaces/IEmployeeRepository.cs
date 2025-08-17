using Company.Demo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Demo.BLL.Interfaces
{
    public interface IEmployeeRepository:IGenericRepository<Employee>
    {
        IEnumerable<Employee> GetByName(string name);
    }
}

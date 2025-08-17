using Company.Demo.BLL.Interfaces;
using Company.Demo.DAL.Data.Contexts;
using Company.Demo.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Demo.BLL.Repositories
{
    public class DepartmentRepository :GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext dbContext):base(dbContext)//Ask ClR for Creating Object From ApplicationDbContext
        {
        }

        
    }
}

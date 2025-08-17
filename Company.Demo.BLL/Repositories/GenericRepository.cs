using Company.Demo.BLL.Interfaces;
using Company.Demo.DAL.Data.Contexts;
using Company.Demo.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Demo.BLL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : ModelBase
    {
        private protected readonly ApplicationDbContext _dbContext;//allow inheritance in this project only  

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<T> GetAll()
        {
            if (typeof(T) == typeof(Employee))
                return (IEnumerable<T>)_dbContext.Employees.Include(E => E.Department).AsNoTracking().ToList();

            return _dbContext.Set<T>().AsNoTracking().ToList();
        }
        public T? Get(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }


        public int Add(T entity)
        {
            _dbContext.Add(entity);
            //_dbContext.Set<T>().Add(entity);
            return _dbContext.SaveChanges();
        }

        public int Delete(T entity)
        {
            _dbContext.Remove(entity);
            return _dbContext.SaveChanges();
        }


        public int Update(T entity)
        {
            _dbContext.Update(entity);
            return _dbContext.SaveChanges();
        }

    }
}

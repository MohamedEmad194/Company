using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Company.Demo.DAL.Models
{
    public class Employee:ModelBase
    {
        //Model: class DB Poco
      
        public string Name { get; set; } = null!;


        public int? Age { get; set; }


        public string? Address { get; set; }


        public decimal Salary { get; set; }


        public bool IsActive { get; set; }
        
        public string? Email { get; set; }
      
       public string? PhoneNumber { get;set; }


        public DateTime HiringDate { get; set; }

        public  bool IsDeleted { get; set; }

        public DateTime CreationDate { get; set; }

        public int? DepartmentId { get; set; } //FK

        //Naviagtional Property[one]
        public virtual Department? Department { get; set; } 

    }
}

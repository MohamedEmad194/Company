using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Demo.DAL.Models
{
    public class Department:ModelBase
    {
        [Required(ErrorMessage = "Name is required !!")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Code is required !!")]
        public string Code { get; set; } = null!;

        //public string Description { get; set; } = null!;
        [DisplayName("Date of Creation")]
        public DateTime DateOfCreation { get; set; }



        public virtual ICollection<Employee>? Employees { get; set; } = new HashSet<Employee>();
    }
}

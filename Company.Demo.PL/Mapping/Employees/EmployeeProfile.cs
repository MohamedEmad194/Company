using AutoMapper;
using Company.Demo.DAL.Models;
using Company.Demo.PL.ViewModels.Employees;

namespace Company.Demo.PL.Mapping.Employees
{
    public class EmployeeProfile:Profile
    {

        public EmployeeProfile()
        {
            CreateMap<EmployeeViewModel, Employee>().ReverseMap();
            //CreateMap<Employee, EmployeeViewModel>();

        }
    }
}

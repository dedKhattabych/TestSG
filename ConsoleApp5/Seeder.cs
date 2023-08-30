using ConsoleApp5.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    public class Seeder
    {
        private static ApplicationContext _db;

        public Seeder(ApplicationContext db)
        {
            _db = db;
        }
        public void SeedData(List<List<string>> data, string path)
        {
            string fileName = Path.GetFileName(path).ToLower();

            if (fileName == "jobtitle.tsv")
            {
                SeedJobTitle(data[0]);
            }
            if (fileName == "employees.tsv")
            {
                SeedEmployees(data);
            }
            if (fileName == "departments.tsv")
            {
                SeedDepartment(data);
            }
        }
        private static void SeedJobTitle(List<string> jobTitles)
        {
            for (int i = 0; i < jobTitles.Count; i++)
            {
                var jobTitleName = jobTitles[i];
                if (_db.JobTitles.FirstOrDefault(u => u.Name == jobTitleName) == null)
                {
                    _db.JobTitles.Add(new JobTitle { Name = jobTitleName });
                    _db.SaveChanges();
                }
            }
        }
        private static void SeedDepartment(List<List<string>> data)
        {
            var Id = data[0];
            var parentId = data[1];
            var nameEmpl = data[2];
            var phone = data[3];

            for (int i = 0; i < nameEmpl.Count; i++)
            {
                if (_db.Employees.FirstOrDefault(u => u.FullName == nameEmpl[i]) == null)
                {
                    _db.Employees.Add(new Employees { FullName = nameEmpl[i] }); // ПОДУМАТЬ О ТОМ ЧТО ПРИ ДОБАВЛЕНИИ ЕМПЛОУЕЕ БУДЕТ НЕПОЛНОЦЕННЫЙ ЕМПЛОУЕ ДОБАВЛЕННЫЙ ЗДЕСЬ
                }
            }

            for (int i = 0; i < Id.Count; i++)
            {
                var entity = _db.Department.FirstOrDefault(u => u.Name == Id[i]);

                if (entity != null) // обновляю
                {
                    entity.Phone = phone[i];
                    entity.Name = Id[i];

                    if (nameEmpl[i] != "")
                    {
                        entity.ManagerID = _db.Employees.FirstOrDefault(u => u.FullName == nameEmpl[i]).ID;
                    }
                    if (parentId[i] != "")
                    {
                        entity.ParentID = _db.Department.FirstOrDefault(u => u.Name == parentId[i]).ID;
                    }

                    _db.Update(entity);
                    _db.SaveChanges();
                    continue;
                }

                entity = new Department
                {
                    Phone = phone[i],
                    Name = Id[i],
                };
                _db.Department.Add(entity);
                _db.SaveChanges();

                var empl = _db.Employees.FirstOrDefault(u => u.FullName == nameEmpl[i]);
                var dep = _db.Department.FirstOrDefault(u => u.Name == parentId[i]);

                if (empl != null)
                {
                    entity.ManagerID = empl.ID;
                }
                if (dep != null)
                {
                    entity.ParentID = dep.ID;
                }

                _db.Update(entity);
                _db.SaveChanges();
            }
        }
        private static void SeedEmployees(List<List<string>> data)
        {
            //добавляю новые профессии которых еще нет в бд
            SeedJobTitle(data[4]);

            //добавляю департаменты которых еще нет в бд
            var departments = data[0];
            for (int i = 0; i < departments.Count; i++)
            {
                var departmentName = departments[i];
                if (departmentName == "")
                {
                    continue;
                }

                if (_db.Department.FirstOrDefault(u => u.Name == departmentName) == null)
                {
                    _db.Department.Add(new Department { Name = departmentName });
                    _db.SaveChanges();
                }
            }

            var jobTitles = data[4];
            var fullNames = data[1];
            var passwords = data[3];
            var logins = data[2];

            for (int i = 0; i < data[0].Count; i++)
            {
                var departmentName = departments[i];
                var jobTitleName = jobTitles[i];

                var jobTitle = _db.JobTitles.FirstOrDefault(u => u.Name == jobTitleName);
                var department = _db.Department.FirstOrDefault(u => u.Name == departmentName);

                if (department == null)
                {
                    department = _db.Department.FirstOrDefault(u => u.ID == 1);
                }


                // update если имя сотрудника уже есть, а остальные поля пусты (при добавлении departments.tsv первым)
                var entity = _db.Employees.FirstOrDefault(u => u.FullName == fullNames[i]);
                if (entity != null)
                {
                    entity.DepartmentId = department.ID;
                    entity.Login = logins[i];
                    entity.Password = passwords[i];
                    entity.JobTitleId = jobTitle.ID;

                    _db.Update(entity);
                    _db.SaveChanges();
                    continue;
                }

                Employees employee = new Employees
                {
                    Department = department,
                    DepartmentId = department.ID,
                    FullName = fullNames[i],
                    Login = logins[i],
                    Password = passwords[i],
                    JobTitle = jobTitle,
                    JobTitleId = jobTitle.ID,
                };
                _db.Employees.Add(employee);
                _db.SaveChanges();
            }
        }
    }
}

using ConsoleApp5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace ConsoleApp5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //args = new string[] { "C:\\Users\\User\\Downloads\\jobtitle.tsv" };

            if (args.Length == 0)
            {
                GetDataFromDB();
                return;
            }

            foreach (string arg in args)
            {
                var data = GetDataFromTSV(arg);

                var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                using (ApplicationContext db = new ApplicationContext(connectionString))
                {
                    Seeder seeder = new Seeder(db);
                    seeder.SeedData(data, arg);
                }
            }

            // вывод текущего состояния
        }
        public static void ShowAll()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // создаем дерево где корневая нода - род подразделение

            var allDep = new List<SimpleTreeNode<Department>>();

            using (ApplicationContext db = new ApplicationContext(connectionString))
            {
                var departments = db.Department.Select(x => x).ToList();
                foreach (var department in departments)
                {
                    var node = new SimpleTreeNode<Department>(department);
                    allDep.Add(node);
                }
                foreach (var node in allDep)
                {
                    node.Parent = allDep.FirstOrDefault(x => x.NodeValue.ID == node.NodeValue.ParentID);
                    if (node.Parent != null)
                    {
                        node.Parent.Children.Add(node);
                    }
                }

                var root = allDep.FirstOrDefault(x => x.NodeValue.ID == 2);
                var tree = new SimpleTree<Department>(root);

                var result = tree.GetAllNodes();

                for (int i = 0; i < result.Count; i++)
                {
                    var departmentHead = db.Employees.FirstOrDefault(u => u.ID == result[i].NodeValue.ManagerID);
                    var employees = db.Employees.Where(u => u.DepartmentId == result[i].NodeValue.ID
                                                         && u.ID != result[i].NodeValue.ManagerID);

                    Console.WriteLine(new string('=', result[i].Depth + 1) + result[i].NodeValue.Name);
                    Console.WriteLine(new string(' ', result[i].Depth + 1) + '*' + departmentHead.FullName);

                    foreach (var node in employees)
                    {
                        Console.WriteLine(new string(' ', result[i].Depth + 1) + '-' + node.FullName);
                    }
                }
            }
        }
        public static void GetDataFromDB()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            int? id = 0;
            while (true)
            {
                Console.WriteLine("Введите department ID");
                var text = Console.ReadLine();

                if (int.TryParse(text, out int result))
                {
                    id = result;
                    break;
                }
            }

            // если ввели ноль то вытаскиваем все
            if (id < 2)
            {
                ShowAll();
                Console.ReadKey();

                return;
            }

            List<Department> departments = new List<Department>();

            using (ApplicationContext db = new ApplicationContext(connectionString))
            {
                var entity = db.Department.FirstOrDefault(u => u.ID == id);
                if (entity == null)
                {
                    return;
                }
                id = entity.ParentID;
                departments.Add(entity);

                while (id != null)
                {
                    entity = db.Department.FirstOrDefault(u => u.ID == id);
                    id = entity.ParentID;

                    departments.Add(entity);
                }
                int spacesCount = 0;
                for (int i = departments.Count - 1; i >= 0; i--)
                {
                    spacesCount++;
                    Console.WriteLine(new string('=', spacesCount) + departments[i].Name);
                }
                var employee = db.Employees.FirstOrDefault(u => u.ID == departments[0].ManagerID).FullName;
                Console.WriteLine(new string(' ', spacesCount) + '*' + employee);
                Console.ReadKey();
            }
        }
        public static List<List<string>> GetDataFromTSV(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            string[] headers = lines[0].Split('\t');
            List<List<string>> data = headers.Select(_ => new List<string>()).ToList();

            foreach (string line in lines.Skip(1))
            {
                var values = line.Split('\t');

                if (values.All(s => s == ""))
                {
                    continue;
                }

                for (int j = 0; j < values.Length; j++)
                {
                    string result = String.Join(" ", values[j].Split(' ', StringSplitOptions.RemoveEmptyEntries));
                    data[j].Add(result.ToLower().Trim());
                }
            }
            return data;
        }
    }
}

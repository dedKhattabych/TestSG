using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp5.Models
{
    //[Index(nameof)]
    public class Employees
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [AllowNull]
        public int? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        [AllowNull]
        public int? JobTitleId { get; set; }

        [ForeignKey("JobTitleId")]
        public virtual JobTitle JobTitle { get; set; }
    }
}

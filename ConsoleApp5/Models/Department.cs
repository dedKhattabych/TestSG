using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5.Models
{
    public class Department
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [AllowNull]
        public int? ParentID { get; set; }

        [ForeignKey("ParentID")]
        public virtual Department depart { get; set; }

        [AllowNull]
        public int? ManagerID { get; set; }

        [ForeignKey("ManagerID")]
        public virtual Employees Employees { get; set; }


        public string Name { get; set; }
        public string Phone { get; set; }
    }
}

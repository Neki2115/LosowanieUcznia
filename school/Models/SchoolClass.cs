using school.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace school.Models
{
    public class SchoolClass
    {
        public string ClassName { get; set; }
        public List<Student> Students { get; set; } = new();
    }
}
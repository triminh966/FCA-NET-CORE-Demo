using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCA.Data.Entities
{
    [Table("Student")]
    public class Student
    {
        [Key]
        public int IdStudent { get; set; }
        public string StudentName { get; set; }
        public int Gender { get; set; }
        public string Note { get; set; }
    }
}

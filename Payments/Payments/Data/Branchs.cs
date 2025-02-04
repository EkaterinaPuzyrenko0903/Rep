using System.ComponentModel.DataAnnotations;

namespace Payments.Data
{
    public class Branchs
    {
        [Key]
        public int Id { get; set; }
        public int NumberBranch { get; set; }//Код Филиала
        public string? NameBranch { get; set; }//Название филиала

    }
}

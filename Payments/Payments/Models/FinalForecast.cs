using System.ComponentModel.DataAnnotations;

namespace Payments.Models
{
    public class FinalForecast
    {
        [Key]
        public int Id { get; set; }
        public int NumberBranch { get; set; }
        //Название филиала
        public string? Name { get; set; }
        public DateTime Date { get; set; }
        public double FinalAmount { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Payments.Models
{
    public class Coeff
    {
        [Key]
        public int Id { get; set; }
        public int Kod { get; set; }
        //Название филиала
        public string? Name { get; set; }
        public string? PayCondition { get; set; }
        public int DayPay { get; set; }
        public double SumPayment { get; set; }

    }
}

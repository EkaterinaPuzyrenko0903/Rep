using System.ComponentModel.DataAnnotations;

namespace Payments.Data
{
    public class PaymentCondition
    {
        [Key]
        public int Id { get; set; }
        public int NumberBranch { get; set; }//Код Филиал
        public string? Name { get; set; } //Название филиала
        public string? PayCondition { get; set; }//Условие платежа
        public double Share { get; set; }//Доля,%
    }
}


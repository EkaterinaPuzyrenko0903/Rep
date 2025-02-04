using System.ComponentModel.DataAnnotations;

namespace Payments.Data
{
    public class PayConditionInput
    {
        [Key]
        public int Id { get; set; }
        public int NumberPayCond { get; set; }//Код условия платежа
        public string? NamePayCond { get; set; }//Кол-во дней до оплаты
    }
}

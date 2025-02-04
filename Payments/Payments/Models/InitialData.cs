using System.ComponentModel.DataAnnotations;

namespace Payments.Models
{
    public class InitialData
    {
        [Key]
        public int Id { get; set; }
        //Филиал код
        public int IdKod { get; set; }
        //Название филиала
        public string? Name { get; set; }
        //Код клиента
        public int KodClient { get; set; }
        //Номер документа
        public string? NumberDoc { get; set; }
        //Дата документа
        public DateTime DateDoc { get; set; }
        //Вид документа
        public string? ViewDoc { get; set; }
        //Условие платежа
        public string? ConditionPay { get; set; }
        //Сумма документа
        public double Sum { get; set; }
    }
}

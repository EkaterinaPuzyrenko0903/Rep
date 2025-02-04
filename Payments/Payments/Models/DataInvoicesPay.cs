using Microsoft.AspNetCore.Routing.Constraints;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payments.Models
{
    public class DataInvoicesPay
    {
        //Таблица результат обработки данных
        [Key]
        public int Id { get; set; }
        public int Kod { get; set; }//Филиал код
        public string? Name { get; set; }//Филиал Название
        public int IdClient { get; set; }//Код клиента
        public string? IdScore { get; set; }//Номер док счета
        public DateTime DateScore { get; set; }//Дата док счета
        public string? ViewScore { get; set; }//Вид док счета
        public string? payCondition { get; set; }//Условие платежа, дней
        public string? IdPayment { get; set; }//Номер док платежа
        public DateTime DatePayment { get; set; }//Дата док платежа
        public string? ViewPayment { get; set; }//Вид док платежа
        public double DayPay { get; set; }//Кол-во дней до оплаты
        public double SumScore { get; set; }//Сумма счета
        public double SumPayment { get; set; }//Сумма платежа
        public double Sum { get; set; }//Сумма платежа к счету
        public int InitialDatesId { get; set; } //Внешний ключ 



    }
}

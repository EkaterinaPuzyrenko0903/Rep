namespace Payments.Models
{
    public class ForecastData
    {

        public int Id { get; set; }
        //Филиал код
        public int IdKod { get; set; }
        //Название филиала
        public string? Name { get; set; }
        //Код клиента 
        public int KodClient { get; set; }
        //Номер документа 
        public string? NumberDoc { get; set; }
        //Дата докумета
        public DateTime DateDoc { get; set; }
        //Условие платежа 
        public string? ConditionPay { get; set; }
        //Сумма
        public double Sum { get; set; }


    }
}

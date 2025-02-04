using System.ComponentModel.DataAnnotations;

namespace Payments.Models
{
    public class ForecastViewModel
    {
        [Key]
        public int Id { get; set; }
        //Филиал код
        public int IdKod { get; set; }
        //Название филиала
        public string? Name { get; set; }
        //Дата документа
        public DateTime DateDoc { get; set; }

        //Сумма
        public double Sum { get; set; }

    }
}

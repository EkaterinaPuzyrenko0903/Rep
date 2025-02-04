namespace Payments.Data
{
    public class PlanRevenue
    {
        public int Id { get; set; }
        public int NumberBranch { get; set; }//Филиал Код
        public string Name { get; set; } //Название филиала
        public string? Month { get; set; } //Месяц
        public int Year { get; set; }//Год
        public double Revenue { get; set; }//Выручка
    }
}

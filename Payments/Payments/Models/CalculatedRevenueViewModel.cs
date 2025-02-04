namespace Payments.Models
{
    public class CalculatedRevenueViewModel
    {
        public int Id { get; set; }
        public int NumberBranch { get; set; }
        public string? Name { get; set; }  //Название филиала
        public string? Month { get; set; }
        public int Year { get; set; }
        public string? PaymentCondition { get; set; }
        public double RevenueRes { get; set; }
    }
}

namespace Payments.Models
{
    public class PaymentForecast
    {
        public int Id { get; set; }
        public int NumberBranch { get; set; }
        public string? NameBranch { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
    }
}

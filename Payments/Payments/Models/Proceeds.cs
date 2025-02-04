namespace Payments.Models
{
    public class Proceeds
    {
        public int Id { get; set; }
        public int NumberBranch { get; set; }
        public string Name { get; set; }
        public string? Month { get; set; }
        public string? PaymentCondition { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }

    }
}
    
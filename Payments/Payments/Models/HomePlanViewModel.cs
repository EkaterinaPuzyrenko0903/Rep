using Library;
using Payments.Data;
namespace Payments.Models
{
    public class HomePlanViewModel
    {
        public PlanRevenue? Revenue { get; set; }
        public List<PlanRevenue> Revenues { get; set; }
        public PaymentCondition? Condition { get; set; }
        public List<PaymentCondition> PaymentConditions { get; set; }
        public Branchs? Branch { get; set; }
        public List<Branchs> Branches { get; set; }
        public PayConditionInput? PayConditionInputs { get; set; }
        public List<PayConditionInput> PayConditionInputes { get; set; }
    }
}

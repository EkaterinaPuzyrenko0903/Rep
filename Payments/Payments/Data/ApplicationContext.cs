using Microsoft.EntityFrameworkCore;
using Payments.Models;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Payments.Data
{
    public class ApplicationContext:DbContext
    {
        public DbSet<DataInvoicesPay> DataInvoicesPays { get; set; }
        public DbSet<InitialData> InitialDates { get; set; }
        public DbSet<Coeff> coeffs { get; set; }
        public DbSet<UnpaidInv> Unpaids { get; set; }
        public DbSet<ForecastData> Forecasts { get; set; }
        public DbSet<ForecastViewModel> forecastViewModels { get; set; }
        public DbSet<Branchs> Branches { get; set; }
        public DbSet<PlanRevenue> PlanRevenues { get; set; }
        public DbSet<PaymentCondition> PaymentConditions { get; set; }
        public DbSet<CalculatedRevenueViewModel> calculatedRevenueViewModels { get; set; }
        public DbSet<Proceeds> proceeds { get; set; }
        public DbSet<Paying> Payings { get; set; }
        public DbSet<PaymentForecast> paymentForecasts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PayConditionInput> PayConditionInputs { get; set; }
        public DbSet<FinalForecast> FinalForecasts { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }
    }
}

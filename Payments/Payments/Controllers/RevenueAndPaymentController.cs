using Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payments.Data;
using Payments.Models;
using System.Diagnostics;
using System.Globalization;

namespace Payments.Controllers
{
    public class RevenueAndPaymentController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<RevenueAndPaymentController> _logger;

        public RevenueAndPaymentController(ILogger<RevenueAndPaymentController> logger, ApplicationContext applicationContext)
        {
            _logger = logger;
            _context = applicationContext;
        }
        [HttpGet]
        public IActionResult PlanRevenueInput(int? VarId)
        {
            var viewModel = new HomePlanViewModel();

            if (VarId != null)
            {
                viewModel.Revenue = _context.PlanRevenues
                    .FirstOrDefault(x => x.Id == VarId);
            }
            viewModel.Revenues = _context.PlanRevenues.ToList();
            viewModel.Branches = _context.Branches.ToList(); // Получение списка филиалов
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult PlanRevenueInputResult(Input input)
        {
            if (!ModelState.IsValid)
            {
                // Обработка неверных данных
                return View("Error");
            }
            // Сохранение варианта исходных данных
            if (!int.IsNegative(input.NumberBranch))
            {
                var existingPlan = _context.PlanRevenues.FirstOrDefault(x =>
                    x.Month == input.Month &&
                    x.Year == input.Year);

                if (existingPlan != null)
                {
                    // Если запись существует, обновляем данные
                    existingPlan.Name = input.Name;
                    existingPlan.Revenue = input.Revenue; // Обновляем выручку

                    _context.PlanRevenues.Update(existingPlan);
                }
                else
                {
                    // Если запись не существует, добавляем новую запись
                    var newPlan = new PlanRevenue
                    {
                        NumberBranch = input.NumberBranch,
                        Name = input.Name,
                        Month = input.Month,
                        Year = input.Year,
                        Revenue = input.Revenue,
                    };

                    _context.PlanRevenues.Add(newPlan);
                }

                _context.SaveChanges();
                // Вызываем метод автоматического обновления данных
                UpdateCalculatedRevenue(input.NumberBranch,input.Month, input.Year, input.Revenue);
                UpdateProceeds();
                UpdatePaying();
                
                UpdatePaymentForecast();
            }

            return RedirectToAction("PlanRevenueInput");
        }



        public IActionResult RemovePlanRevenue(int? VarId)
        {
            var variant = _context.PlanRevenues
                .FirstOrDefault(x => x.Id == VarId);
            if (variant != null)
            {
                _context.PlanRevenues.Remove(variant);
                _context.SaveChanges();

                TempData["message"] = $"Вариант {variant.NumberBranch} удален.";
                // Вызываем метод автоматического обновления данных
                UpdateCalculatedRevenue(variant.NumberBranch, variant.Month, variant.Year, variant.Revenue);
                UpdateProceeds();
                UpdatePaying();
                
                UpdatePaymentForecast();
            }
            else
            {
                TempData["message"] = $"Вариант не найден.";
            }
            return RedirectToAction(nameof(PlanRevenueInput));
        }
        [HttpGet]
        public IActionResult PaymentConditionInput(int? VarId)
        {
            var viewModel = new HomePlanViewModel();

            if (VarId != null)
            {
                viewModel.Condition = _context.PaymentConditions

                    .FirstOrDefault(x => x.Id == VarId);
            }
            viewModel.PaymentConditions = _context.PaymentConditions

                .ToList();
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult PaymentConditionInputResult(Input input)
        {
            if (!ModelState.IsValid)
            {
                // Обработка неверных данных
                return View("Error");
            }

            //Сохранение варианта исходных данных
            if (!int.IsNegative(input.NumberBranch))
            {
                var existVar = _context.PaymentConditions.FirstOrDefault(x =>
                                x.NumberBranch == input.NumberBranch &&
                                x.Name == input.Name &&
                                x.PayCondition == input.PayCondition);
                if (existVar != null)
                {
                    // Если запись существует, обновляем данные
                    existVar.Share = input.Share;//Обновляем сумму

                    _context.PaymentConditions.Update(existVar);
                    _context.SaveChanges();
                }
                else
                {
                    var plan = new PaymentCondition
                    {
                        NumberBranch = input.NumberBranch,
                        Name = input.Name,
                        PayCondition = input.PayCondition,
                        Share = input.Share,
                    };
                    _context.PaymentConditions.Add(plan);
                    _context.SaveChanges();
                }
               
                // Вызываем метод автоматического обновления данных
                UpdateCalculatedRevenue(input.NumberBranch, input.Month, input.Year, input.Revenue);
                UpdateProceeds();
                UpdatePaying();
                
                UpdatePaymentForecast();
            }
            
            return RedirectToAction("PaymentConditionInput");

        }
        public IActionResult RemovePaymentCondition(int? VarId)
        {
            var variant = _context.PaymentConditions
                .FirstOrDefault(x => x.Id == VarId);
            if (variant != null)
            {
                // Получаем данные перед удалением
                int numberBranch = variant.NumberBranch;
                string month = variant.PayCondition; // Предположим, что PayCondition содержит информацию о месяце или времени
                int year = DateTime.Now.Year; // Используем текущий год, если информация о годе отсутствует
                double revenue = 0; // Начальная сумма дохода, измените при необходимости для получения корректного значения

                // Удаляем вариант
                _context.PaymentConditions.Remove(variant);
                _context.SaveChanges();

                TempData["message"] = $"Вариант {variant.NumberBranch} удален.";

                // Обновляем другие данные
                UpdateCalculatedRevenue(numberBranch, month, year, revenue);
                UpdateProceeds();
                UpdatePaying();
                
                UpdatePaymentForecast();
                
            }
            else
            {
                TempData["message"] = $"Вариант не найден.";
            }
            return RedirectToAction(nameof(PaymentConditionInput));
        }


        public void UpdateCalculatedRevenue(int numberBranch, string month, int year, double revenue)
        {
            // Получаем текущие записи в таблице calculatedRevenueViewModels
            var existingRecords = _context.calculatedRevenueViewModels.ToList();

            // Получаем данные для обновления таблицы
            var paymentConditions = _context.PaymentConditions.AsEnumerable();
            var planRevenues = _context.PlanRevenues.AsEnumerable();

            var joinedPlan = (from paycon in paymentConditions
                              join reven in planRevenues on paycon.NumberBranch equals reven.NumberBranch
                              select new CalculatedRevenueViewModel
                              {
                                  NumberBranch = paycon.NumberBranch,
                                  Name = reven.Name,
                                  Month = reven.Month,
                                  Year = reven.Year,
                                  PaymentCondition = paycon.PayCondition,
                                  RevenueRes = paycon.Share * reven.Revenue
                              }).ToList();

            // Список для новых или обновленных записей
            var newOrUpdatedRecords = new List<CalculatedRevenueViewModel>();

            // Обновляем существующие записи или добавляем новые записи
            foreach (var item in joinedPlan)
            {
                var existingRecord = existingRecords.FirstOrDefault(r =>
                    r.NumberBranch == item.NumberBranch && r.Month == item.Month && r.Year == item.Year && r.PaymentCondition == item.PaymentCondition);

                if (existingRecord != null)
                {
                    // Обновляем существующую запись
                    existingRecord.RevenueRes = item.RevenueRes;
                    existingRecord.Name = item.Name; 
                    _context.Entry(existingRecord).State = EntityState.Modified;
                }
                else
                {
                    // Добавляем новую запись
                    newOrUpdatedRecords.Add(item);
                }
            }

            // Добавляем новые записи
            _context.calculatedRevenueViewModels.AddRange(newOrUpdatedRecords);

            // Удаляем старые записи, которых больше нет в новых данных
            var recordsToDelete = existingRecords
                .Where(r => !joinedPlan.Any(j =>
                    j.NumberBranch == r.NumberBranch && j.Month == r.Month && j.Year == r.Year && j.PaymentCondition == r.PaymentCondition))
                .ToList();
            _context.calculatedRevenueViewModels.RemoveRange(recordsToDelete);

            // Сохраняем изменения в базе данных
            _context.SaveChanges();
        }

        [HttpPost]
        public IActionResult CalculatedRevenue()
        {
            var calculatedRevenues = _context.calculatedRevenueViewModels.ToList();
            return View(calculatedRevenues);
        }


        public void UpdateProceeds()
        {
            var calcRevenue = _context.calculatedRevenueViewModels.AsEnumerable();
            var existingProceeds = _context.proceeds.ToList();
            var allProceeds = new List<Proceeds>();

            foreach (var revenue in calcRevenue)
            {
                var year = revenue.Year;
                var month = DateTime.ParseExact(revenue.Month, "MMMM", CultureInfo.CurrentCulture).Month;
                var daysInMonth = DateTime.DaysInMonth(year, month);

                for (int day = 1; day <= daysInMonth; day++)
                {
                    var proceed = new Proceeds
                    {
                        NumberBranch = revenue.NumberBranch,
                        Name = revenue.Name,
                        Month = revenue.Month,
                        PaymentCondition = revenue.PaymentCondition,
                        Date = new DateTime(year, month, day), // Устанавливаем дату как текущий день месяца
                        Amount = Math.Round(revenue.RevenueRes / daysInMonth, 2)
                    };

                    // Проверяем, существует ли уже запись для данной даты и условия платежа в базе данных
                    var existingRecord = existingProceeds.FirstOrDefault(r =>
                        r.NumberBranch == proceed.NumberBranch && r.Date == proceed.Date && r.PaymentCondition == proceed.PaymentCondition);

                    if (existingRecord != null)
                    {
                        // Если запись уже существует, обновляем ее
                        existingRecord.Name = revenue.Name;
                        existingRecord.PaymentCondition = proceed.PaymentCondition;
                        existingRecord.Amount = proceed.Amount;
                        _context.Entry(existingRecord).State = EntityState.Modified;
                    }
                    else
                    {
                        // Если записи нет, добавляем новую
                        _context.proceeds.Add(proceed);
                    }

                    allProceeds.Add(proceed);
                }
            }

            // Удаляем записи, которые больше не существуют в allProceeds
            foreach (var record in existingProceeds)
            {
                if (!allProceeds.Any(j =>
                    j.NumberBranch == record.NumberBranch && j.Date == record.Date && j.PaymentCondition == record.PaymentCondition))
                {
                    _context.proceeds.Remove(record);
                }
            }

            _context.SaveChanges();
        }

        [HttpPost]
        public IActionResult Proceeds()
        {
            var proceeds = _context.proceeds.OrderBy(x=>x.Date).ToList();
            return View(proceeds);
        }
        //[HttpGet]
        public void UpdatePaying()
        {
            // Получаем существующие записи о платежах из базы данных
            var existingPayings = _context.Payings.ToList();

            // Получаем данные о выручке и описания платежей из базы данных
            var proceeds = _context.proceeds.AsEnumerable();
            var coeffs = _context.coeffs.AsEnumerable();

            // Создаем список для хранения результирующих данных о платежах
            var newDataList = new List<Paying>();

            // Проходимся по каждой строке данных о выручке
            foreach (var prc in proceeds)
            {
                // Получаем количество дней в месяце для данной даты
                var daysInMonth = DateTime.DaysInMonth(prc.Date.Year, prc.Date.Month);

                // Получаем список дат для данной строки выручки в рамках текущего месяца
                var dates = Enumerable.Range(1, daysInMonth)
                    .Select(day => new DateTime(prc.Date.Year, prc.Date.Month, day))
                    .ToList();

                // Проходимся по каждой дате
                foreach (var date in dates)
                {
                    // Находим все соответствующие условия платежа для данной даты
                    var relevantCoeffs = coeffs.Where(c => c.PayCondition == prc.PaymentCondition);

                    // Проходимся по каждому соответствующему условию платежа
                    foreach (var coeff in relevantCoeffs)
                    {
                        // Вычисляем новую дату платежа, учитывая выход за пределы месяца
                        var paymentDate = date.AddDays(coeff.DayPay);

                        // Вычисляем сумму платежа
                        var amount = prc.Amount * coeff.SumPayment;

                        // Создаем объект платежа
                        var newPayment = new Paying
                        {
                            NumberBranch = prc.NumberBranch,
                            Name = prc.Name,
                            PaymentCondition = prc.PaymentCondition,
                            Date = paymentDate,
                            Sum = Math.Round(amount, 2)
                        };

                        // Проверка на дублирование перед добавлением
                        if (!newDataList.Any(j =>
                            j.NumberBranch == newPayment.NumberBranch && j.Name == newPayment.Name && j.PaymentCondition == newPayment.PaymentCondition && j.Date == newPayment.Date && j.Sum == newPayment.Sum))
                        {
                            newDataList.Add(newPayment);
                        }
                    }
                }
            }

            // Добавляем новые записи и обновляем существующие записи
            foreach (var newData in newDataList)
            {
                var existing = existingPayings.FirstOrDefault(p =>
                    p.NumberBranch == newData.NumberBranch && p.Date == newData.Date && p.PaymentCondition == newData.PaymentCondition);

                if (existing != null)
                {
                    // Обновляем существующую запись
                    existing.Name = newData.Name;
                    existing.Sum = newData.Sum;
                    _context.Entry(existing).State = EntityState.Modified;
                }
                else
                {
                    // Добавляем новую запись
                    _context.Payings.Add(newData);
                }
            }

            // Удаляем старые записи, которых больше нет
            var recordsToDelete = existingPayings
                .Where(p => !newDataList.Any(j =>
                    j.NumberBranch == p.NumberBranch && j.Date == p.Date && j.PaymentCondition == p.PaymentCondition))
                .ToList();
            _context.Payings.RemoveRange(recordsToDelete);

            // Сохраняем изменения в базе данных
            _context.SaveChanges();
 
        }
        [HttpGet]
        public IActionResult Paying()
        {
            var payings = _context.Payings.OrderBy(x => x.Date).ToList();

            return View(payings);
        }
      
        public void UpdatePaymentForecast()
        {
            // Получение существующих записей прогнозов платежей из базы данных
            var existingForecasts = _context.paymentForecasts.ToList();

            // Получение данных о платежах из базы данных
            var payings = _context.Payings.ToList();

            var groupedData = payings
             .GroupBy(x => new { x.Date, x.NumberBranch })
             .Select(x => new PaymentForecast
                  {
                      NumberBranch = x.Key.NumberBranch,
                      NameBranch = x.First().Name, // Заполняем название филиала
                      Date = x.Key.Date,
                      Amount = x.Sum(f => f.Sum)
                  })
             .OrderBy(x => x.Date)
             .ToList();

            // Список для новых или обновленных записей прогнозов платежей
            var newDataList = new List<PaymentForecast>();

            // Добавляем новые записи и обновляем существующие записи
            foreach (var newForecast in groupedData)
            {
                var existing = existingForecasts.FirstOrDefault(p =>
                    p.NumberBranch == newForecast.NumberBranch && p.Date == newForecast.Date);

                if (existing != null)
                {
                    // Обновляем существующую запись
                    existing.NameBranch = newForecast.NameBranch;
                    existing.Amount = newForecast.Amount;
                    _context.Entry(existing).State = EntityState.Modified;
                }
                else
                {
                    // Добавляем новую запись
                    newDataList.Add(newForecast);
                }
            }

            // Добавляем новые записи
            _context.paymentForecasts.AddRange(newDataList);

            // Удаляем старые записи, которых больше нет
            var recordsToDelete = existingForecasts
                .Where(p => !groupedData.Any(j =>
                    j.NumberBranch == p.NumberBranch && j.Date == p.Date))
                .ToList();
            _context.paymentForecasts.RemoveRange(recordsToDelete);

            // Сохраняем изменения в базе данных
            _context.SaveChanges();
        }
        [HttpGet]
        public IActionResult PaymentForecast()
        {
            var paymentforecasts = _context.paymentForecasts.OrderBy(x => x.Date).ToList();
            return View(paymentforecasts);
        }
    }
}

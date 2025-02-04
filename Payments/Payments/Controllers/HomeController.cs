using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payments.Data;
using Payments.Models;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ExcelDataReader;
using System.Text;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Library;
using System.ComponentModel;
using OfficeOpenXml;
using System.Xml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace Payments.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationContext applicationContext)
        {
            _logger = logger;
            _context = applicationContext;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var listpay = _context.InitialDates.OrderBy(x=>x.KodClient).ToList();
            return View(listpay);
        }
       
        public void UpdateDataInvoicesPays()
        {
            // Получаем текущие записи в таблице DataInvoicesPays
            var existingRecords = _context.DataInvoicesPays.ToList();

            // Получаем данные из таблицы InitialDates
            var initialData = _context.InitialDates.ToList();

            // Группируем данные по клиенту
            var groupedData = initialData.GroupBy(d => d.KodClient);

            // Список для хранения результатов
            List<DataInvoicesPay> resultList = new List<DataInvoicesPay>();

            foreach (var clientGroup in groupedData) // Для каждого клиента
            {
                double remainingBalance = 0; // Начальное значение остатка от предыдущего платежа

                // Получаем счета для клиента
                var invoices = clientGroup.Where(d => d.ViewDoc == "INV").OrderBy(d => d.DateDoc).ToList();

                // Список для хранения платежей для данного клиента
                var payments = clientGroup.Where(d => d.ViewDoc == "PAY").OrderBy(d => d.DateDoc).ToList();

                foreach (var invoice in invoices) // Для каждого счета клиента
                {
                    double invoiceAmountCopy = invoice.Sum; // Копия суммы счета

                    if (remainingBalance != 0)
                    {
                        // Создаем запись для результата
                        var dataInvoicePay = new DataInvoicesPay
                        {
                            Kod = invoice.IdKod,
                            Name = invoice.Name,
                            IdClient = invoice.KodClient,
                            IdScore = invoice.NumberDoc,
                            DateScore = invoice.DateDoc,
                            ViewScore = invoice.ViewDoc,
                            payCondition = invoice.ConditionPay,
                            IdPayment = payments[payments.Count - 1].NumberDoc,
                            DatePayment = payments[payments.Count - 1].DateDoc,
                            ViewPayment = payments[payments.Count - 1].ViewDoc,
                            DayPay = (int)(payments[payments.Count - 1].DateDoc - invoice.DateDoc).TotalDays,
                            SumScore = invoice.Sum,
                            SumPayment = payments[payments.Count - 1].Sum,
                            Sum = (int)remainingBalance * -1
                        };

                        resultList.Add(dataInvoicePay);
                        break;
                    }

                    foreach (var payment in payments) // Для каждого платежа клиента
                    {
                        if (payment.Sum <= 0)
                            continue;

                        double paymentAmount = Math.Min(invoiceAmountCopy, payment.Sum); // Сумма платежа(минимум между суммой платежа и суммой счета)

                        invoiceAmountCopy -= payment.Sum; // Вычитаем сумму платежа из суммы счета

                        // Создаем запись для результата
                        var dataInvoicePay = new DataInvoicesPay
                        {
                            Kod = invoice.IdKod,
                            Name = invoice.Name,
                            IdClient = invoice.KodClient,
                            IdScore = invoice.NumberDoc,
                            DateScore = invoice.DateDoc,
                            ViewScore = invoice.ViewDoc,
                            payCondition = invoice.ConditionPay,
                            IdPayment = payment.NumberDoc,
                            DatePayment = payment.DateDoc,
                            ViewPayment = payment.ViewDoc,
                            DayPay = (int)(payment.DateDoc - invoice.DateDoc).TotalDays,
                            SumScore = invoice.Sum,
                            SumPayment = payment.Sum,
                            Sum = (int)paymentAmount,
                        };

                        resultList.Add(dataInvoicePay);

                        if (invoiceAmountCopy <= 0) // Если счет полностью оплачен, выходим из цикла
                            break;
                    }

                    remainingBalance = invoiceAmountCopy; // Обновляем остаток для следующего счета
                }
            }

            // Список для новых и обновленных записей
            var newOrUpdatedRecords = new List<DataInvoicesPay>();

            foreach (var item in resultList)
            {
                var existingRecord = existingRecords.FirstOrDefault(r =>
                    r.Kod == item.Kod && r.IdClient == item.IdClient && r.IdScore == item.IdScore && r.IdPayment == item.IdPayment);

                if (existingRecord != null)
                {
                    // Обновить существующую запись
                    existingRecord.Name = item.Name;
                    existingRecord.DateScore = item.DateScore;
                    existingRecord.ViewScore = item.ViewScore;
                    existingRecord.payCondition = item.payCondition;
                    existingRecord.DatePayment = item.DatePayment;
                    existingRecord.ViewPayment = item.ViewPayment;
                    existingRecord.DayPay = item.DayPay;
                    existingRecord.SumScore = item.SumScore;
                    existingRecord.SumPayment = item.SumPayment;
                    existingRecord.Sum = item.Sum;
                    _context.Entry(existingRecord).State = EntityState.Modified;
                }
                else
                {
                    // Добавить новую запись
                    newOrUpdatedRecords.Add(item);
                }
            }

            // Добавить новые записи
            _context.DataInvoicesPays.AddRange(newOrUpdatedRecords);

            // Удалить старые записи, которых больше нет в новых данных
            var recordsToDelete = existingRecords
                .Where(r => !resultList.Any(j =>
                    j.Kod == r.Kod && j.IdClient == r.IdClient && j.IdScore == r.IdScore && j.IdPayment == r.IdPayment))
                .ToList();
            _context.DataInvoicesPays.RemoveRange(recordsToDelete);

            // Сохранить изменения в базе данных
            _context.SaveChanges();

            // Получаем данные из базы данных и передаем их в представление
            var model = _context.DataInvoicesPays.ToList();
            //return View(model);
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Result()
        {
            var listpay = _context.DataInvoicesPays.OrderBy(x=>x.DateScore).ToList();
            return View(listpay);
        }


        public void UpdateCalculationCoeff()
        {
            // Удаление всех существующих записей из таблицы coeffs
            _context.coeffs.RemoveRange(_context.coeffs);
            _context.SaveChanges();

            // Получение всех платежей из таблицы DataInvoicesPays
            var payments = _context.DataInvoicesPays.AsEnumerable();

            // Группируем платежи по филиалу и условиям платежа
            var groupedPayments = payments
               .GroupBy(x => new { x.payCondition, x.Kod })
               .Select(x => new
               {
                   Kod = x.Key.Kod, // Филиал
                   PayCondition = x.Key.payCondition, // Группа условий платежа
                   SumPayment = x.Sum(p => p.Sum)
               });

            // Создание списка для хранения новых коэффициентов
            List<Coeff> data = new List<Coeff>();

            // Перебор каждой группы платежей
            foreach (var pay in groupedPayments)
            {
                // Группировка платежей по дате платежа для текущей группы
                var daypayments = payments.Where(x => x.Kod == pay.Kod && x.payCondition == pay.PayCondition)
                                          .GroupBy(x => x.DayPay)
                                          .Select(x => new
                                          {
                                              DayPay = x.Key,
                                              SumPayment = x.Sum(p => p.Sum)
                                          });

                // Получение названия филиала из таблицы Branches
                var branchName = _context.Branches.FirstOrDefault(b => b.NumberBranch == pay.Kod)?.NameBranch;

                // Создание новых записей коэффициентов и добавление их в список data
                foreach (var daypay in daypayments)
                {
                    var row = new Coeff
                    {
                        Kod = pay.Kod,                      // Филиал
                        Name = branchName,                  // Название филиала
                        PayCondition = pay.PayCondition,    // Группа условий платежа
                        DayPay = (int)daypay.DayPay,        // Дата платежа 
                        SumPayment = Math.Round((daypay.SumPayment / pay.SumPayment), 2) // Расчет коэффициента
                    };
                    data.Add(row);

                    _context.coeffs.Add(row);
                }
            }

            // Сохранение изменений в базе данных
            _context.SaveChanges();

            // Обновление неоплаченных счетов с использованием новых коэффициентов
            var branchIds = groupedPayments.Select(gp => gp.Kod).Distinct().ToArray();
            UpdateUnpaidInvResult(branchIds);
            UpdateForecastDateResult(branchIds);
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult CalculationCoeff()
        {
            var listpay = _context.coeffs.ToList();
            return View(listpay);
        }
        [HttpGet]
        public IActionResult UnpaidInv()
        {
            var unpaidInv = _context.Unpaids.OrderBy(x => x.DateDoc).ToList();
            return View(unpaidInv);
        }
       
        public void UpdateUnpaidInvResult(int[] branchIds)
        {
            // Получение текущих записей из таблицы Forecasts
            var existingForecasts = _context.Forecasts.ToList();

            // Получение данных о неоплаченных счетах и условиях платежа из базы данных
            var unpaidInv = _context.Unpaids.AsEnumerable();
            var coeff = _context.coeffs.AsEnumerable();

            // Фильтрация данных, если предоставлены идентификаторы филиалов
            if (branchIds != null && branchIds.Length > 0)
            {
                unpaidInv = unpaidInv.Where(x => branchIds.Contains(x.IdKod));
                coeff = coeff.Where(x => branchIds.Contains(x.Kod));
            }

            // Выполнение запроса к базе данных для получения данных о неоплаченных счетах (INV) и условиях платежа (ConditionPay)
            var newForecasts = (from inv in unpaidInv
                                join pc in coeff on new { inv.IdKod, inv.ConditionPay } equals new { IdKod = pc.Kod, ConditionPay = pc.PayCondition }
                                select new ForecastData
                                {
                                    IdKod = inv.IdKod,
                                    Name = inv.Name,
                                    KodClient = inv.KodClient,
                                    NumberDoc = inv.NumberDoc,
                                    ConditionPay = inv.ConditionPay,
                                    DateDoc = inv.DateDoc.AddDays(pc.DayPay),
                                    Sum = inv.Sum * pc.SumPayment
                                }).ToList();


            // Обновление существующих записей и добавление новых
            foreach (var newForecast in newForecasts)
            {
                var existingRecord = existingForecasts.FirstOrDefault(ef =>
                    ef.IdKod == newForecast.IdKod &&
                    ef.Name == newForecast.Name &&
                    ef.KodClient == newForecast.KodClient &&
                    ef.NumberDoc == newForecast.NumberDoc &&
                    ef.ConditionPay == newForecast.ConditionPay &&
                    ef.DateDoc == newForecast.DateDoc);

                if (existingRecord != null)
                {
                    // Обновление существующей записи
                    existingRecord.Sum = newForecast.Sum;
                    _context.Entry(existingRecord).State = EntityState.Modified;
                }
                else
                {
                    // Добавление новой записи
                    _context.Forecasts.Add(newForecast);
                }
            }

            // Удаление старых записей, которых больше нет в новых данных
            var recordsToDelete = existingForecasts
                .Where(ef => !newForecasts.Any(nf =>
                    nf.IdKod == ef.IdKod &&
                    nf.Name == ef.Name &&
                    nf.KodClient == ef.KodClient &&
                    nf.NumberDoc == ef.NumberDoc &&
                    nf.ConditionPay == ef.ConditionPay &&
                    nf.DateDoc == ef.DateDoc))
                .ToList();

            _context.Forecasts.RemoveRange(recordsToDelete);
           
            // Сохранение изменений в базе данных
            _context.SaveChanges();

            //return View(newForecasts);
        }
        [HttpPost]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult UnpaidInvResult()
        {
            var listpay = _context.Forecasts.OrderBy(x=>x.DateDoc).ToList();
            return View(listpay);
        }
        public void UpdateForecastDateResult(int[] branchIds)
        {
            // Получение текущих записей из таблицы прогнозов
            var existingForecasts = _context.forecastViewModels.ToList();

            // Фильтрация данных, если предоставлены идентификаторы филиалов
            var forecastData = _context.Forecasts.AsQueryable();
            if (branchIds != null && branchIds.Length > 0)
            {
                forecastData = forecastData.Where(x => branchIds.Contains(x.IdKod));
            }

            
            // Группировка и агрегация данных
            var group = forecastData
                .AsEnumerable()
                .GroupBy(x => new { x.IdKod, x.DateDoc })
                .Select(x => new ForecastViewModel
                {
                    IdKod = x.Key.IdKod,
                    // Получение названия филиала из контекста базы данных или другого источника данных
                    Name = _context.Branches.FirstOrDefault(b => b.NumberBranch == x.Key.IdKod)?.NameBranch,
                    DateDoc = x.Key.DateDoc,
                    Sum = x.Sum(f => f.Sum)
                }).ToList();

            // Обновление существующих записей и добавление новых
            foreach (var forecast in group)
            {
                var existingRecord = existingForecasts.FirstOrDefault(ef =>
                    ef.IdKod == forecast.IdKod &&
                    ef.DateDoc == forecast.DateDoc);

                if (existingRecord != null)
                {
                    existingRecord.Sum = forecast.Sum;
                    existingRecord.Name = forecast.Name;
                    _context.Entry(existingRecord).State = EntityState.Modified;
                }
                else
                {
                    _context.forecastViewModels.Add(forecast);
                }
            }

            // Удаление старых записей, которых больше нет в новых данных
            var recordsToDelete = existingForecasts
                .Where(ef => !group.Any(nf =>
                    nf.IdKod == ef.IdKod &&
                    nf.DateDoc == ef.DateDoc))
                .ToList();

            _context.forecastViewModels.RemoveRange(recordsToDelete);

            // Сохранение изменений в базе данных
            _context.SaveChanges();
        }


        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ForecastDateResult([FromQuery] int[] branchIds)
        {
            var forecastData = _context.forecastViewModels.AsQueryable();

            if (branchIds != null && branchIds.Length > 0)
            {
                forecastData = forecastData.Where(x => branchIds.Contains(x.IdKod));
            }

            var unpaidInv = forecastData.OrderBy(x => x.DateDoc).ToList();
            ViewBag.Branches = _context.Branches.ToList();
            return View(unpaidInv);
        }




        [HttpGet]
        public IActionResult BranchInput(int? VarId)
        {
            var viewModel = new HomePlanViewModel();

            if (VarId != null)
            {
                viewModel.Branch = _context.Branches
                    .FirstOrDefault(x => x.Id == VarId);
            }
            viewModel.Branches = _context.Branches.ToList();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult BranchInputResult(Input input)
        {
            //Сохранение варианта исходных данных
            if (!int.IsNegative(input.NumberBranch))
            {
                var existBranch = _context.Branches.FirstOrDefault(x =>
                x.NumberBranch == input.NumberBranch);
                if (existBranch != null)
                {
                    //Если запись существует, обновляем данные
                    existBranch.NumberBranch = input.NumberBranch;
                    existBranch.NameBranch = input.NameBranch;
                    _context.Branches.Update(existBranch);
                }
                else
                {
                    // Если запись не существует, добавляем новую запись
                    var plan = new Branchs
                    {

                        NumberBranch = input.NumberBranch,
                        NameBranch = input.NameBranch

                    };
                    _context.Branches.Add(plan);
                }

                _context.SaveChanges();

            }
            return RedirectToAction("BranchInput");
        }

        public IActionResult RemoveBranch(int? VarId)
        {
            var variant = _context.Branches
                .FirstOrDefault(x => x.Id == VarId);
            if (variant != null)
            {
                _context.Branches.Remove(variant);
                _context.SaveChanges();

                TempData["message"] = $"Вариант {variant.NumberBranch} удален.";
            }
            else
            {
                TempData["message"] = $"Вариант не найден.";
            }
            return RedirectToAction(nameof(BranchInput));
        }
        [HttpGet]
        public IActionResult PayCondInput(int? VarId)
        {
            var viewModel = new HomePlanViewModel();

            if (VarId != null)
            {
                viewModel.PayConditionInputs = _context.PayConditionInputs
                    .FirstOrDefault(x => x.Id == VarId);
            }
            viewModel.PayConditionInputes = _context.PayConditionInputs.ToList();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult PayCondInputResult(Input input)
        {
            //Сохранение варианта исходных данных
            if (!int.IsNegative(input.NumberPayCond))
            {
                var existPayCond = _context.PayConditionInputs.FirstOrDefault(x =>
                x.NumberPayCond == input.NumberPayCond);
                if (existPayCond != null)
                {
                    //Если запись существует, обновляем данные
                    existPayCond.NumberPayCond = input.NumberPayCond;
                    existPayCond.NamePayCond = input.NamePayCond;
                    _context.PayConditionInputs.Update(existPayCond);
                }
                else
                {
                    // Если запись не существует, добавляем новую запись
                    var plan = new PayConditionInput
                    {

                        NumberPayCond = input.NumberPayCond,
                        NamePayCond = input.NamePayCond

                    };
                    _context.PayConditionInputs.Add(plan);
                }

                _context.SaveChanges();

            }
            return RedirectToAction("PayCondInput");
        }
        
        public IActionResult RemovePayCond(int? VarId)
        {
            var variant = _context.PayConditionInputs
                .FirstOrDefault(x => x.Id == VarId);
            if (variant != null)
            {
                _context.PayConditionInputs.Remove(variant);
                _context.SaveChanges();

                TempData["message"] = $"Вариант {variant.NumberPayCond} удален.";
            }
            else
            {
                TempData["message"] = $"Вариант не найден.";
            }
            return RedirectToAction(nameof(PayCondInput));
        }

        [HttpGet]
        public IActionResult ImportFromExcelIndex()
        {
            return View(); // Возвращаем представление для загрузки файла Excel
        }

        [HttpPost]
        public IActionResult ImportFromExcelIndex(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                ModelState.AddModelError("File", "Please select a valid Excel file.");
                return View(); // Возвращаем представление с сообщением об ошибке
            }

            // Register CodePagesEncodingProvider for Windows-1252 support
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = file.OpenReadStream())
            {
                using (var excelReader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                {
                    FallbackEncoding = Encoding.UTF8
                }))
                {
                    // Читаем и импортируем данные из первого листа в таблицу InitialDates
                    var dataSet1 = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    var dataTable1 = dataSet1.Tables[0];
                    var existingData = _context.InitialDates.ToList();
                    bool changesMade = false;

                    foreach (DataRow row in dataTable1.Rows)
                    {
                        var idKod = Convert.ToInt32(row["ФилиалКод"]);
                        var name = row["ФилиалНазвание"]?.ToString();
                        var kodClient = Convert.ToInt32(row["КодКлиента"]);
                        var numberDoc = row["НомерДокумента"]?.ToString();
                        var dateDoc = Convert.ToDateTime(row["ДатаДокумента"]);
                        var viewDoc = row["ВидДокумента"]?.ToString();
                        var conditionPay = row["УсловиеПлатежа"]?.ToString();
                        var sum = Convert.ToDouble(row["СуммаДокумента"]);

                        var existingEntry = existingData.FirstOrDefault(e =>
                            e.IdKod == idKod &&
                            e.KodClient == kodClient &&
                            e.NumberDoc == numberDoc);

                        if (existingEntry != null)
                        {
                            if (existingEntry.Name != name ||
                                existingEntry.DateDoc != dateDoc ||
                                existingEntry.ViewDoc != viewDoc ||
                                existingEntry.ConditionPay != conditionPay ||
                                existingEntry.Sum != sum)
                            {
                                existingEntry.Name = name;
                                existingEntry.DateDoc = dateDoc;
                                existingEntry.ViewDoc = viewDoc;
                                existingEntry.ConditionPay = conditionPay;
                                existingEntry.Sum = sum;
                                _context.Update(existingEntry);
                                changesMade = true;
                            }
                        }
                        else
                        {
                            var newEntry = new InitialData
                            {
                                IdKod = idKod,
                                Name = name,
                                KodClient = kodClient,
                                NumberDoc = numberDoc,
                                DateDoc = dateDoc,
                                ViewDoc = viewDoc,
                                ConditionPay = conditionPay,
                                Sum = sum
                            };
                            _context.Add(newEntry);
                            changesMade = true;
                        }
                    }

                    // Удаляем записи из базы данных, которых нет в данных из Excel
                    foreach (var entryInDB in existingData)
                    {
                        var correspondingRowInExcel = dataTable1.AsEnumerable().FirstOrDefault(row =>
                            Convert.ToInt32(row["ФилиалКод"]) == entryInDB.IdKod &&
                            Convert.ToInt32(row["КодКлиента"]) == entryInDB.KodClient &&
                            row["НомерДокумента"]?.ToString() == entryInDB.NumberDoc);

                        if (correspondingRowInExcel == null)
                        {
                            _context.InitialDates.Remove(entryInDB);
                            changesMade = true;
                        }
                    }

                    // Сохраняем все изменения в БД
                    if (changesMade)
                    {
                        _context.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Данные уже загружены и совпадают.");
                        return View(); // Возвращаем представление с сообщением об ошибке
                    }
                }
            }
            UpdateDataInvoicesPays();
            UpdateCalculationCoeff();
            return RedirectToAction("Index"); // Перенаправляем пользователя на страницу с результатами импорта
        }


        [HttpGet]
        public IActionResult ImportFromExcelUnpaidInv()
        {
            return View(); // Возвращаем представление для загрузки файла Excel
        }

        [HttpPost]
        public IActionResult ImportFromExcelUnpaidInv(IFormFile file, [FromQuery] int[] branchIds)
        {
            if (file == null || file.Length <= 0)
            {
                ModelState.AddModelError("File", "Please select a valid Excel file.");
                return View(); // Возвращаем представление с сообщением об ошибке
            }

            // Register CodePagesEncodingProvider for Windows-1252 support
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = file.OpenReadStream())
            {
                using (var excelReader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                {
                    FallbackEncoding = Encoding.UTF8
                }))
                {
                    var dataSet1 = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    var dataTable1 = dataSet1.Tables[0];
                    var existingData = _context.Unpaids.ToList();
                    bool changesMade = false;
            

                    foreach (DataRow row in dataTable1.Rows)
                    {
                        var idKod = Convert.ToInt32(row["ФилиалКод"]);
                        var name = row["ФилиалНазвание"]?.ToString();
                        var kodClient = Convert.ToInt32(row["КодКлиента"]);
                        var numberDoc = row["НомерДокумента"]?.ToString();
                        var dateDoc = Convert.ToDateTime(row["ДатаДокумента"]);
                        var viewDocInv = row["ВидДокСчета"]?.ToString();
                        var conditionPay = row["УсловиеПлатежа"]?.ToString();
                        var sum = Convert.ToDouble(row["Сумма"]);

                        var existingEntry = existingData.FirstOrDefault(e =>
                            e.IdKod == idKod &&
                            e.KodClient == kodClient &&
                            e.NumberDoc == numberDoc);

                        if (existingEntry != null)
                        {
                            if (existingEntry.Name != name ||
                                existingEntry.DateDoc != dateDoc ||
                                existingEntry.ViewDocInv != viewDocInv ||
                                existingEntry.ConditionPay != conditionPay ||
                                existingEntry.Sum != sum)
                            {
                                existingEntry.Name = name;
                                existingEntry.DateDoc = dateDoc;
                                existingEntry.ViewDocInv = viewDocInv;
                                existingEntry.ConditionPay = conditionPay;
                                existingEntry.Sum = sum;
                                _context.Update(existingEntry);
                                changesMade = true;
                            }
                        }
                        else
                        {
                            var newEntry = new UnpaidInv
                            {
                                IdKod = idKod,
                                Name = name,
                                KodClient = kodClient,
                                NumberDoc = numberDoc,
                                DateDoc = dateDoc,
                                ViewDocInv = viewDocInv,
                                ConditionPay = conditionPay,
                                Sum = sum
                            };
                            _context.Add(newEntry);
                            changesMade = true;
                        }
                    }

                    // Удаляем записи из базы данных, которых нет в данных из Excel
                    foreach (var entryInDB in existingData)
                    {
                        var correspondingRowInExcel = dataTable1.AsEnumerable().FirstOrDefault(row =>
                            Convert.ToInt32(row["ФилиалКод"]) == entryInDB.IdKod &&
                            Convert.ToInt32(row["КодКлиента"]) == entryInDB.KodClient &&
                            row["НомерДокумента"]?.ToString() == entryInDB.NumberDoc);

                        if (correspondingRowInExcel == null)
                        {
                            _context.Unpaids.Remove(entryInDB);
                            changesMade = true;
                        }
                    }

                    // Сохраняем все изменения в БД
                    if (changesMade)
                    {
                        _context.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Данные уже загружены и совпадают.");
                        return View(); // Возвращаем представление с сообщением об ошибке
                    }
                }
            }

            // Обновляем прогнозные даты
            UpdateUnpaidInvResult(branchIds);
            UpdateForecastDateResult(branchIds);

            return RedirectToAction("UnpaidInv"); // Перенаправляем пользователя на страницу с результатами импорта
        }


        [HttpGet]
        public IActionResult ExportToExcel(int? branchId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            List<ForecastViewModel> filteredData;

            // Проверяем, был ли указан параметр фильтрации
            if (branchId.HasValue && branchId >= 0)
            {
                // Получаем отфильтрованные данные из вашей базы данных
                filteredData = _context.forecastViewModels
                    .Where(x => x.IdKod == branchId)
                    .OrderBy(x => x.DateDoc)
                    .ToList();
            }
            else
            {
                // Получаем все данные из вашей базы данных
                filteredData = _context.forecastViewModels.OrderBy(x => x.DateDoc).ToList();
            }

            // Создаем новый пакет Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Данные");
                // Создаем массив заголовков столбцов
                var columnHeaders = new[]
                {
                    "Филиал код", 
                    "Филиал название",
                    "Дата платежа",
                    "Сумма платежа, руб.",
        };

                // Заполняем заголовки столбцов в Excel
                for (int i = 0; i < columnHeaders.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = columnHeaders[i];
                }

                // Заполняем лист Excel данными из базы данных
                for (int i = 0; i < filteredData.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = filteredData[i].IdKod;
                    worksheet.Cells[i + 2, 2].Value = filteredData[i].Name;
                    worksheet.Cells[i + 2, 3].Value = filteredData[i].DateDoc;
                    worksheet.Cells[i + 2, 3].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[i + 2, 4].Value = filteredData[i].Sum;
                }

                // Сконвертируем пакет Excel в байтовый массив
                var excelBytes = package.GetAsByteArray();

                // Определяем имя файла Excel
                string fileName = "Данные.xlsx";

                // Отправляем файл Excel как ответ клиенту
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        [HttpGet]
        public async Task<IActionResult> FinalForecast()
        {
            // Извлечь существующие записи из базы данных
            var existingRecords = await _context.FinalForecasts.ToListAsync();

            // Извлечь данные из других таблиц
            var forecasts = await _context.forecastViewModels.ToListAsync();
            var payments = await _context.paymentForecasts.ToListAsync();
            var branches = await _context.Branches.ToListAsync();

            // Объединить и суммировать данные
            var combinedData = forecasts.Select(f => new { Branch = f.IdKod, Date = f.DateDoc, Amount = f.Sum })
                                        .Concat(payments.Select(p => new { Branch = p.NumberBranch, Date = p.Date, Amount = p.Amount }));

            var summarizedPayments = combinedData.GroupBy(x => new { x.Branch, x.Date })
                                                 .Select(g => new FinalForecast
                                                 {
                                                     NumberBranch = g.Key.Branch,
                                                     Name = branches.FirstOrDefault(b => b.NumberBranch == g.Key.Branch)?.NameBranch,
                                                     Date = g.Key.Date,
                                                     FinalAmount = g.Sum(x => x.Amount)
                                                 })
                                                 .OrderBy(x => x.Date)
                                                 .ToList();

            // Обновить существующие записи или добавить новые записи
            var newOrUpdatedRecords = new List<FinalForecast>();

            foreach (var item in summarizedPayments)
            {
                var existingRecord = existingRecords.FirstOrDefault(r =>
                    r.NumberBranch == item.NumberBranch && r.Date == item.Date);

                if (existingRecord != null)
                {
                    // Обновить существующую запись
                    existingRecord.FinalAmount = item.FinalAmount;
                    _context.Entry(existingRecord).State = EntityState.Modified;
                }
                else
                {
                    // Добавить новую запись
                    newOrUpdatedRecords.Add(item);
                }
            }

            // Добавить новые записи
            _context.FinalForecasts.AddRange(newOrUpdatedRecords);

            // Удалить старые записи, которых больше нет в новых данных
            var recordsToDelete = existingRecords
                .Where(r => !summarizedPayments.Any(j =>
                    j.NumberBranch == r.NumberBranch && j.Date == r.Date))
                .ToList();
            _context.FinalForecasts.RemoveRange(recordsToDelete);

            // Сохранить изменения в базе данных
            await _context.SaveChangesAsync();

            // Вернуть представление с данными
            return View(summarizedPayments);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
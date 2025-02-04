using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebObogrev.Models;
using Google.OrTools.LinearSolver;
namespace WebObogrev.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DataInput()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Result(DataInputViewModel model)
        {
            ResultModel result = new ResultModel();
            Solver solver = Solver.CreateSolver("GLOP");
            Variable xa1t1 = solver.MakeNumVar(0.0, double.PositiveInfinity, "xa1t1");
            Variable xa1t2 = solver.MakeNumVar(0.0, double.PositiveInfinity, "xa1t2");
            Variable xa1t3 = solver.MakeNumVar(0.0, double.PositiveInfinity, "xa1t3");

            Variable xa2t1 = solver.MakeNumVar(0.0, double.PositiveInfinity, "xa2t1");
            Variable xa2t2 = solver.MakeNumVar(0.0, double.PositiveInfinity, "xa2t2");
            Variable xa2t3 = solver.MakeNumVar(0.0, double.PositiveInfinity, "xa2t3");

            Variable xa3t1 = solver.MakeNumVar(0.0, double.PositiveInfinity, "xa3t1");
            Variable xa3t2 = solver.MakeNumVar(0.0, double.PositiveInfinity, "xa3t2");
            Variable xa3t3 = solver.MakeNumVar(0.0, double.PositiveInfinity, "xa3t3");

            Variable xa4t1 = solver.MakeNumVar(0.0, double.PositiveInfinity, "xa4t1");
            Variable xa4t2 = solver.MakeNumVar(0.0, double.PositiveInfinity, "xa4t2");
            Variable xa4t3 = solver.MakeNumVar(0.0, double.PositiveInfinity, "xa4t3");

            solver.Add(
               (xa1t1+xa1t2+xa1t3)>=(model.T1)
               );
            solver.Add(
               (xa2t1+xa2t2+xa2t3)>=(model.T2)
               );
            solver.Add(
               (xa3t1 + xa3t2 + xa3t3) >= (model.T3)
               );
            solver.Add(
               (xa4t1 + xa4t2 + xa4t3) >= (model.T4)
               );
            solver.Add(
               (xa1t1+xa2t1+xa3t1+xa4t1) <= (model.resource1)
               );
            solver.Add(
               (xa1t2 + xa2t2 + xa3t2 + xa4t2) <= (model.resource2)
               );
            solver.Add(
               (xa1t3 + xa2t3 + xa3t3 + xa4t3) <= (model.resource3)
               );
            solver.Maximize(
                (model.A1T1 * xa1t1 + model.A1T2 * xa1t2 + model.A1T3 * xa1t3)+
                (model.A2T1 * xa2t1 + model.A2T2 * xa2t2 +model.A2T3* xa2t3)+
                (model.A3T1 *xa3t1 + model.A3T2*xa3t2 + model.A3T3 * xa3t3)+
                (model.A4T1*xa4t1 + model.A4T2*xa4t2 + model.A4T3*xa4t3));
            Solver.ResultStatus resultStatus = solver.Solve();
            result.targetFunc = Math.Round(solver.Objective().Value(), 0);

            result.XA1T1 = Math.Round(xa1t1.SolutionValue(), 0);
            result.XA1T2 = Math.Round(xa1t2.SolutionValue(), 0);
            result.XA1T3 = Math.Round(xa1t3.SolutionValue(), 0);

            result.XA2T1 = Math.Round(xa2t1.SolutionValue(), 0);
            result.XA2T2 = Math.Round(xa2t2.SolutionValue(), 0);
            result.XA2T3 = Math.Round(xa2t3.SolutionValue(), 0);

            result.XA3T1 = Math.Round(xa3t1.SolutionValue(), 0);
            result.XA3T2 = Math.Round(xa3t2.SolutionValue(), 0);
            result.XA3T3 = Math.Round(xa3t3.SolutionValue(), 0);

            result.XA4T1 = Math.Round(xa4t1.SolutionValue(), 0);
            result.XA4T2 = Math.Round(xa4t2.SolutionValue(), 0);
            result.XA4T3 = Math.Round(xa4t3.SolutionValue(), 0);


            if (resultStatus != Solver.ResultStatus.OPTIMAL)
            {
                TempData["message"] = "Математическая модель не имеет оптимальных решений";
                return RedirectToAction(nameof(Index));
            }

            return View(result);

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
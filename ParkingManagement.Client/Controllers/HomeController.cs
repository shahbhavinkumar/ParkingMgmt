using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Client.Models;
using System.Diagnostics;
using Shared;

namespace ParkingManagement.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApiClient _client;

        public HomeController(ILogger<HomeController> logger, IApiClient apiClient)
        {
            _logger = logger;
            _client = apiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult ParkingInformation(string tagNumber, string actionToPerform)
        {
            ParkingInformation vehicle = new ParkingInformation()
            {
                TagNumber = tagNumber,
                InTime = DateTime.Now,
                Rate = Double.Parse(ConfigurationManager.AppSetting["Configs:hourlyFee"])
            };

            if (actionToPerform == "IN")
            {
                if (Spots.SpotsAvailable == 0)
                {
                    return RedirectToAction("Error", "Home", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = "No Spots Available" });
                }

             
                if (ParkVehicle(vehicle))
                {
                    Spots.SpotsAvailable = Spots.SpotsAvailable - 1;
                    List<ParkingInformation>? parkingInfoObject = GetParkingData().Result?.ToList();
                    return PartialView(parkingInfoObject);
                }
                else
                {
                    // throw  new Exception("Vehicle Entry Already Present");
                    return RedirectToAction("Error", "Home", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = "Vehicle Entry Already Present" });//;new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message ="Vehicle Entry Already Present" });
                }
            }

            if (actionToPerform == "OUT")
            {
                if (Out(vehicle))
                {
                    Spots.SpotsAvailable = Spots.SpotsAvailable + 1;
                    List<ParkingInformation>? parkingInfoObject = GetParkingData().Result?.ToList();
                    return PartialView(parkingInfoObject);
                }
            }

            if (actionToPerform == "REFRESH") //used for page referesh
            {
                List<ParkingInformation>? parkingInfoObject = GetParkingData().Result?.ToList();

                if (parkingInfoObject != null)
                {
                    Spots.SpotsAvailable = Spots.TotalSpots - parkingInfoObject.Count();
                }

                return PartialView(parkingInfoObject);
            }

            return PartialView();
        }

        private bool Out(ParkingInformation vehicleInfo)
        {
            var result = _client.PostAsJsonAsync("/api/vehicle/out", vehicleInfo).Result;
            return result.Content.ReadAsStringAsync().Result == "true" ? true : false;
        }

        private bool ParkVehicle(ParkingInformation vehicleInfo)
        {
            var result =  _client.PostAsJsonAsync("/api/vehicle/parkvehicle", vehicleInfo).Result;
            return result.Content.ReadAsStringAsync().Result == "true" ? true : false ;
        }

        private async Task<ParkingInformation[]?> GetParkingData()
        {
            return await _client.GetFromJsonAsync<ParkingInformation[]>("api/vehicle/getvehicledata");
        }

       public IActionResult GetReport()
        {
            StatsReport? report = _client.GetFromJsonAsync<StatsReport>("api/vehicle/getreport").Result;
            if (report != null)
            {
                report.SpotsAvailable = Spots.SpotsAvailable;
            }
            return PartialView(report);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
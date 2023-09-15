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

        public IActionResult Input()
        {
            return View();
        }


        public ActionResult ParkingInformation(string tagNumber, string actionToPerform)
        {
            switch (actionToPerform)
            {
                case "IN":
                    return HandleParkingIn(tagNumber);

                case "AVAILABLESPOTS":
                    return Json(Spots.SpotsAvailable);

                case "REFRESH":
                    return HandlePageRefresh();

                default:
                    return PartialView();
            }
        }

        private ActionResult HandleParkingIn(string tagNumber)
        {
            ParkingInformation vehicle = new ParkingInformation()
            {
                TagNumber = tagNumber,
                InTime = DateTime.Now,
                Rate = Double.Parse(ConfigurationManager.AppSetting["Configs:hourlyFee"])
            };

            if (ParkVehicle(vehicle))
            {
                Spots.SpotsAvailable--;
                List<ParkingInformation>? parkingInfoObject = GetParkingData()?.ToList();
                return PartialView(parkingInfoObject);
            }
         
            return PartialView();
        }

        private ActionResult HandlePageRefresh()
        {
            List<ParkingInformation>? parkingInfoObject = GetParkingData()?.ToList();

            if (parkingInfoObject != null)
            {
                Spots.SpotsAvailable = Spots.TotalSpots - parkingInfoObject.Count;
            }

            return PartialView(parkingInfoObject);
        }



        private bool ParkVehicle(ParkingInformation vehicleInfo)
        {
            var result =  _client.PostAsJsonAsync("/api/vehicle/parkvehicle", vehicleInfo).Result;
            return result.Content.ReadAsStringAsync().Result == "true" ? true : false ;
        }

        private ParkingInformation[]? GetParkingData()
        {
            return  _client.GetFromJsonAsync<ParkingInformation[]>("api/vehicle/getvehicledata").Result;
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
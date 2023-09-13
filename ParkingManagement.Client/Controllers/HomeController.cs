using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Client.Models;
using System.Diagnostics;
using Shared;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Linq;

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
                    return PartialView("Error", new ErrorViewModel {  Message = "No Spots Available" });
                }

             
                if (ParkVehicle(vehicle))
                {
                    Spots.SpotsAvailable = Spots.SpotsAvailable - 1;
                    List<ParkingInformation>? parkingInfoObject = GetParkingData()?.ToList();
                    return PartialView(parkingInfoObject);
                }
               
            }

            /*
            if (actionToPerform == "OUT")
            {
                ParkingInformation? vehicleParkingInfo = Out(vehicle);
         
                if (vehicleParkingInfo != null)
                {
                    ++Spots.SpotsAvailable;
                    List<ParkingInformation>? parkingInfoObject = GetParkingData()?.ToList();
                    return PartialView(parkingInfoObject);
                }
            }*/

            if (actionToPerform == "REFRESH") //used for page referesh
            {
                List<ParkingInformation>? parkingInfoObject = GetParkingData()?.ToList();

                if (parkingInfoObject != null)
                {
                    Spots.SpotsAvailable = Spots.TotalSpots - parkingInfoObject.Count();
                }

                return PartialView(parkingInfoObject);
            }

            return PartialView();
        }

        private ParkingInformation? Out(ParkingInformation vehicleInfo)
        {
            var result = _client.PostAsync("/api/vehicle/out", vehicleInfo).Result;
            return result;
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
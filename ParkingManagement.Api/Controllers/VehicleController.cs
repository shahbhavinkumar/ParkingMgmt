using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace ParkingManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        IVehicleService _service;

        public VehicleController(IVehicleService service)
        {
            _service = service;
        }

        [HttpGet, Route("getvehicledata")]
        public  IActionResult GetVehicleData()
        {
            var results = _service.GetParkingInformation();
            return Ok(results);
        }

        [HttpPost, Route("iscarregisteredinparking")]
        public IActionResult IsCarRegisteredInParkingLot([FromBody] ParkingInformation vehicle)
        {
            var results = _service.IsCarRegisteredInParkingLot(vehicle);
            return Ok(results);
        }

        [HttpGet, Route("getreport")]
        public IActionResult GetReport()
        {
            var results = _service.GetReport();
            return Ok(results);
        }
        

        [HttpPost, Route("parkvehicle")]
        public IActionResult PostTag([FromBody] ParkingInformation vehicle)
        {
            var isVehicleAlreadyParked = _service.ParkVehicle(vehicle);
            return Ok(isVehicleAlreadyParked);
        }

        [HttpPost, Route("out")]
        public IActionResult Out([FromBody] ParkingInformation vehicle)
        {
            var isVehicleAlreadyParked = _service.Out(vehicle);
            return Ok(isVehicleAlreadyParked);
        }

        public string Index()
        {
            return "This is my default action...";
        }

    }
}

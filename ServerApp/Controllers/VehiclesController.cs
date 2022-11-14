using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VehicleLeasing.Data;
using VehicleLeasing.Dto;
using VehicleLeasing.Models;
using VehicleLeasing.Services;
using VehicleLeasing.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VehicleLeasing.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService vehicleService;
        private readonly IServerTime serverTime;
        private readonly IOptions<AppConfig> appConfig;

        public VehiclesController(IVehicleService vehicleService, IServerTime serverTime, IOptions<AppConfig> appConfig)
        {
            this.vehicleService = vehicleService;
            this.serverTime = serverTime;
            this.appConfig = appConfig;
        }

        // GET: api/<VehiclesController>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return Unauthorized();
            }

            var vehiclesResult = await vehicleService.GetVehicles();

            if (vehiclesResult.IsSome)
            {

                return Ok(ResponseDto.Of(vehiclesResult.Value
                                            .Select(vehicle => VehicleResponseDto.Of(vehicle, serverTime, appConfig.Value.ImagePathPrefix))));
            }
            return BadRequest();
        }
    }
}

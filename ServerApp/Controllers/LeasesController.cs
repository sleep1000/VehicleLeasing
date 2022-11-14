using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using VehicleLeasing.Data;
using VehicleLeasing.Dto;
using VehicleLeasing.Hubs;
using VehicleLeasing.Exceptions;
using VehicleLeasing.Models;
using VehicleLeasing.Services;
using VehicleLeasing.Util;

namespace VehicleLeasing.ServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeasesController : ControllerBase
    {
        private readonly ILeasingService leasingService;
        private readonly IVehicleService vehicleService;
        private readonly UserManager<Driver> userManager;
        private readonly IServerTime serverTime;
        private readonly IHubContext<LeasesHub> leasesHubContext;

        public LeasesController(ILeasingService leasingService,
            IVehicleService vehicleService, UserManager<Driver> userManager, IServerTime serverTime,
            IHubContext<LeasesHub> leasesHubContext)
        {
            this.leasingService = leasingService;
            this.vehicleService = vehicleService;
            this.userManager = userManager;
            this.serverTime = serverTime;
            this.leasesHubContext = leasesHubContext;
        }

        // GET: api/Leases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lease>>> GetLeases()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return Unauthorized();
            }

            var driverId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(driverId))
            {
                var driver = await userManager.FindByIdAsync(driverId);

                var driverLeasesResult = await leasingService.GetDriverLeases(driver, false);

                if (driverLeasesResult.IsSome)
                {
                    return Ok(driverLeasesResult.Value.Select(lease => LeaseResponseDto.ResponseOf(lease, serverTime)));
                }
            }

            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        // POST: api/Leases
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostLease(LeaseRequestDto lease)
        {
            if (! User?.Identity?.IsAuthenticated ?? false)
            {
                return Unauthorized();
            }

            var driverId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(driverId))
            {
                var driver = await userManager.FindByIdAsync(driverId);
                var vehicleResult = await vehicleService.GetVehicleById(lease.VehicleId);

                if (vehicleResult.IsSome)
                {
                    var newLeaseResult = await leasingService.CreateLease(driver, vehicleResult.Value, lease.LeaseDuration);

                    if (newLeaseResult.IsSome)
                    {
                        IClientProxy signalRClientsToSend;
                        if (!string.IsNullOrWhiteSpace(lease.SignalRConnectionId))
                        {
                            signalRClientsToSend = leasesHubContext.Clients.AllExcept(lease.SignalRConnectionId);
                        }
                        else
                        {
                            signalRClientsToSend = leasesHubContext.Clients.All;
                        }

                        var response = LeaseResponseDto.ResponseOf(newLeaseResult.Value, serverTime);

                        await signalRClientsToSend.SendAsync("VehicleLeased", response);
                        return CreatedAtAction("GetLeases", response);
                    }

                    if (newLeaseResult.Error is VehicleInLeaseException)
                    {
                        var leasesResult = await leasingService.GetLeases();
                        if (leasesResult.IsSome)
                        {
                            var leasesResponses = leasesResult.Value.Select(lease => LeaseResponseDto.Of(lease, serverTime));
                            return Ok(ResponseDto.Of(leasesResponses, ResponseStatuses.VehicleInLease));
                        }
                    }

                    if (newLeaseResult.Error is LeasesLimitExceeded)
                    {
                        return Ok(ResponseDto.Of(null, ResponseStatuses.LeasesLimitExceeded));
                    }
                }
            }

            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}

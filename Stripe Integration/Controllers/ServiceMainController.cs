using Microsoft.AspNetCore.Mvc;
using Stripe_Integration.Services;

namespace Stripe_Integration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceMainController : ControllerBase
    {
        private readonly ServiceMainService serviceMainService;
        public ServiceMainController(ServiceMainService _serviceMainService )
        {
            serviceMainService = _serviceMainService;
        }

        [HttpGet("/plans")]
        public async Task<List<ServiceMainDto>> GetPlans()
        {
            var plans = await serviceMainService.GetPlans();
            return plans;
        }
        [HttpGet("plan/{planId}")]
        public async Task<ServiceMainDto> GetPlanDetails(int planId)
        {
            var plan = await serviceMainService.GetServiceById(planId);
            return plan;
        }
        [HttpGet("/available-labs")]
        public async Task<List<ServiceMainDto>> GetAvailableLabs()
        {
            var labTests = await serviceMainService.GetLabTests();
            return labTests;
        }
    }
}

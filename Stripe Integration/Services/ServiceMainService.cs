using AutoMapper;
using Stripe_Integration.Models;
using Stripe_Integration.Repositories;

namespace Stripe_Integration.Services
{
    public class ServiceMainDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<string> Features { get; set; }
    }
    public class ServiceMainService
    {
        private readonly ServiceMainRepository repository;
        private readonly IMapper mapper;
        private readonly ILogger<ServiceMainService> logger;

        public ServiceMainService(IMapper _mapper, ServiceMainRepository repository, ILogger<ServiceMainService> _logger)
        {
            this.repository = repository;
            mapper = _mapper;
            logger = _logger;
        }

        public async Task <IEnumerable<ServiceMain>> GetAllServices()
        {
            return await repository.GetAllAsync();
        }

        public async Task<List<ServiceMainDto>> GetPlans()
        {
            var plans = await repository.GetAllByType("Main");
            var plansDtos = new List<ServiceMainDto>();
            plans.ForEach(p => {
                plansDtos.Add(mapper.Map<ServiceMainDto>(p));
                //logger.LogInformation($"Plan Details: {p.ServiceDetails.Count}");
                });
            return plansDtos;
        }

        public async Task<ServiceMainDto> GetServiceById(int planId)
        {
            var plan = await repository.GetById(planId);
            var planDto = mapper.Map<ServiceMainDto>(plan);
            return planDto;
        }

        public async Task<List<ServiceMainDto>> GetLabTests()
        {
            var plans = await repository.GetAllByType("Lab Work");
            var plansDtos = new List<ServiceMainDto>();
            plans.ForEach(p => {
                plansDtos.Add(mapper.Map<ServiceMainDto>(p));
                //logger.LogInformation($"Lab Test Details: {p.ServiceDetails.FirstOrDefault()!.DetailItemDescription}");
            });
            return plansDtos;
        }
        public async Task<ServiceMain> AddService(ServiceMain service)
        {
            return await repository.AddAsync(service);
        }
    }
}

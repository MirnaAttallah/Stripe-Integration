using AutoMapper;
using Stripe;

namespace Stripe_Integration.Configurations
{
    //public class ProductMappingProfile : Profile
    //{
    //    public ProductMappingProfile()
    //    {
    //        CreateMap<DTOs.CreateProductDTO, Models.ServiceMain>()
    //            .AfterMap((src, dest) => dest.PlanID = src.GetPlanID())
    //            .ReverseMap();
    //        CreateMap<Product, Models.ServiceMain>()
    //            .AfterMap((src, dest) =>
    //            {
    //                dest.ShortDescription = src.Name;
    //                dest.Price = src.Metadata != null && src.Metadata.ContainsKey("Price") ? decimal.Parse(src.Metadata["Price"]) : 0;
    //                dest.PlanType = src.Description;
    //                dest.PlanID = src.Id;
    //            });
    //    }
    //}
}

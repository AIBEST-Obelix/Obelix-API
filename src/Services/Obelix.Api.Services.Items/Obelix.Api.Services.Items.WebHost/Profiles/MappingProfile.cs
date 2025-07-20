using AutoMapper;
using Obelix.Api.Services.Items.Data.Models;
using Obelix.Api.Services.Items.Shared.Models;

namespace Obelix.Api.Services.Identity.WebHost.Profiles;

/// <summary>
/// Mapping profile.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappingProfile"/> class.
    /// </summary>
    public MappingProfile()
    {
        this.CreateMap<Item, ItemVM>();
        this.CreateMap<Item, ItemIM>();
        this.CreateMap<ItemIM, ItemVM>();
    }
}

using Obelix.Api.Services.Identity.Data.Models.Identity;
using Obelix.Api.Services.Identity.Shared.Models.User;
using AutoMapper;

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
        this.CreateMap<User, UserVM>();
        this.CreateMap<UserUM, UserVM>();
    }
}

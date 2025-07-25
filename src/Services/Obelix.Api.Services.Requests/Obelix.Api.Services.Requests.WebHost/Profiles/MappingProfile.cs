using AutoMapper;
using Obelix.Api.Services.Requests.Data.Models;
using Obelix.Api.Services.Requests.Shared.Models;

namespace Obelix.Api.Services.Requests.WebHost.Profiles;

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
        this.CreateMap<Request, RequestVM>();
        this.CreateMap<RequestVM, Request>();
        this.CreateMap<Request, RequestIM>();
    }
}

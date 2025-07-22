using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Obelix.Api.Services.Items.Shared.Models;

public class ItemAnalyzeIM
{
    [Required]
    public List<IFormFile> Files { get; set; } = [];
}
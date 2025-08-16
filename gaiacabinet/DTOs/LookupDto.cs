using System.ComponentModel.DataAnnotations;

namespace gaiacabinet_api.DTOs;

public class LookupDto
{
    [Required, EmailAddress, MaxLength(250)]
    public required string Mail { get; set; }
}
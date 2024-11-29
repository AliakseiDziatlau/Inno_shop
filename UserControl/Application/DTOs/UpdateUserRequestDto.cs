using System.ComponentModel.DataAnnotations;

namespace UserControl.Application.DTOs;

public class UpdateUserRequestDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
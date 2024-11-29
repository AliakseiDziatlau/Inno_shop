using System.ComponentModel.DataAnnotations;

namespace UserControl.Application.DTOs;

public class RegisterRequestDto : UpdateUserRequestDto
{
    public string Password { get; set; }
}
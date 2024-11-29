using System.Security.Claims;
using MediatR;
namespace ProductControl.Application.Commands;

public class BaseCommand<IResponse> : IRequest<IResponse>
{
    public ClaimsPrincipal? User { get; set; }
}
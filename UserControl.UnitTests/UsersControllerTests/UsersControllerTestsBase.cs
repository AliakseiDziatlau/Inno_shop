using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserControl.Presentation.Controllers;

namespace UserControl.UnitTests.UsersControllerTests;

public abstract class UsersControllerTestsBase
{
    protected readonly Mock<IMediator> MediatorMock;
    protected readonly UsersController Controller;

    protected UsersControllerTestsBase()
    {
        MediatorMock = new Mock<IMediator>();
        Controller = new UsersController(MediatorMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = CreateFakeUser() 
                }
            }
        };
    }

    protected ClaimsPrincipal CreateFakeUser()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"), 
            new Claim(ClaimTypes.Role, "Admin") 
        }, "mock"));
    }
}
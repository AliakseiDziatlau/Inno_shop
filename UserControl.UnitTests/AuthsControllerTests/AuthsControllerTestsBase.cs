using MediatR;
using Moq;
using UserControl.Presentation.Controllers;

namespace UserControl.UnitTests.AuthsControllerTests;

public abstract class AuthsControllerTestsBase
{
    protected readonly Mock<IMediator> MediatorMock;
    protected readonly AuthsController Controller;

    protected AuthsControllerTestsBase()
    {
        MediatorMock = new Mock<IMediator>();
        Controller = new AuthsController(MediatorMock.Object);
    }
}
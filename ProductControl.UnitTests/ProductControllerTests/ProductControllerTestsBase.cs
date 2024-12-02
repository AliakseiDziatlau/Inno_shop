using MediatR;
using Moq;
using ProductControl.Presentation.Controllers;

namespace UserControlTests.ProductControllerTests;

public abstract class ProductsControllerTestsBase
{
    protected readonly Mock<IMediator> MediatorMock;
    protected readonly ProductsController Controller;

    protected ProductsControllerTestsBase()
    {
        MediatorMock = new Mock<IMediator>();
        Controller = new ProductsController(MediatorMock.Object);
    }
}
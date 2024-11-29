using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductControl.Application.Commands;

namespace UserControlTests.ProductControl.UnitTests.ProductControllerTests;

public class DeleteProductTests : ProductsControllerTestsBase
{
    [Fact]
    public async Task DeleteProduct_ReturnsNoContent_WhenProductIsDeletedSuccessfully()
    {
        var productId = 1;
        var user = new ClaimsPrincipal();

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.DeleteProduct(productId);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductCommand>(c => c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()), Times.Never);
    }
    

    [Fact]
    public async Task DeleteProduct_VerifiesCorrectMediatorCall()
    {
        var productId = 1;
        var user = new ClaimsPrincipal();

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        await Controller.DeleteProduct(productId);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductCommand>(c => c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNoContent_WhenProductAlreadyDeleted()
    {
        var productId = 1;
        var user = new ClaimsPrincipal();

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.DeleteProduct(productId);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductCommand>(c => c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()), Times.Never);
    }
}

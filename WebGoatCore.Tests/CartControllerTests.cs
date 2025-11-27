using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using WebGoatCore.Controllers;
using Xunit;

namespace WebGoatCore.Tests.Controllers;
public class CartControllerTests
{
    [Theory]
    [InlineData((short)0)]
    [InlineData((short)-1)]
    [InlineData((short)-10)]
    [InlineData((short)101)]
    [InlineData((short)1000)]
    public void AddOrder_InvalidQuantity_RedirectsToProductDetailsAndSetsError(short quantity)
    {
        var controller = CreateController();
        int productId = 123;

        var result = controller.AddOrder(productId, quantity);

        var redirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Details", redirect.ActionName);
        Assert.Equal("Product", redirect.ControllerName);
        Assert.Equal(productId, redirect.RouteValues["productId"]);

        Assert.Equal(
            "Quantity must be a positive number.",
            controller.TempData["Error"]);
    }

    // Hjælper-metode, oprette en CartController med TempData
    private CartController CreateController()
    {
        // productRepository er null,  fordi der kun testes ugyldige quantities
        var controller = new CartController(productRepository: null!);

        var httpContext = new DefaultHttpContext();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        controller.TempData = new TempDataDictionary(
            httpContext,
            new SimpleTempDataProvider());

        return controller;
    }

    // Simpel TempData-provider.
    private class SimpleTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context)
            => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
            // Ikke nødvendig at gemme noget.
        }
    }
}

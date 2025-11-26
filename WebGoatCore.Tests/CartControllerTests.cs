using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using WebGoatCore.Controllers;
using Xunit;

namespace WebGoatCore.Tests.Controllers
{
    // Minimal fake TempDataProvider so TempData works in tests
    internal class FakeTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context)
            => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
            // no-op
        }
    }

    public class CartControllerTests
    {
        private CartController CreateController()
        {
            // We pass null for ProductRepository here because the
            // validation branch (quantity <= 0) never uses it.
            var controller = new CartController(productRepository: null);

            var httpContext = new DefaultHttpContext();

            controller.TempData = new TempDataDictionary(
                httpContext,
                new FakeTempDataProvider());

            return controller;
        }

        [Fact]
        public void AddOrder_WithNonPositiveQuantity_RedirectsToProductDetailsAndSetsError()
        {
            // Arrange
            var controller = CreateController();
            int productId = 123;

            // Act
            var result = controller.AddOrder(productId, quantity: 0);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            // Redirects back to the Product details page
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("Product", redirect.ControllerName);
            Assert.Equal(productId, redirect.RouteValues["productId"]);

            // TempData contains the error message
            Assert.Equal(
                "Quantity must be a positive number.",
                controller.TempData["Error"]);
        }
    }
}

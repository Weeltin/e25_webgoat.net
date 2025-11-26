using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using WebGoatCore.Controllers;
using Xunit;

namespace WebGoatCore.Tests.Controllers
{
    // fake TempDataProvider
    internal class FakeTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context)
            => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {

        }
    }

    public class CartControllerTests
    {
        private CartController CreateController()
        {
            // Pass null for ProductRepository here because the
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
            var controller = CreateController();
            int productId = 123;

            var result = controller.AddOrder(productId, quantity: 0);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

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

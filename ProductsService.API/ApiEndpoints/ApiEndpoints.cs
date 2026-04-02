using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ProductsService.API.Filters;
using ProductsService.Core.DTO;
using ProductsService.Core.ServiceContracts;

namespace ProductsService.API.ApiEndpoints
{
    public static class ApiEndpoints
    {
        public static IEndpointRouteBuilder AddProductMinimalApiEndpoints(this IEndpointRouteBuilder app)
        {
            // group all the product related endpoints under a common route prefix and apply the FluentValidationFilter to all of them
            var productsGroup = app.MapGroup("/api/products")
                       .AddEndpointFilter<FluentValidationFilter>();

            // GET: /api/products
            productsGroup.MapGet("/", async (IProductService productsService) =>
            {
                return Results.Ok(await productsService.GetProducts());
            });

            // GET: /api/products/search/product-id/{productId}
            productsGroup.MapGet("/search/product-id/{productId}", async (Guid productId, IProductService productsService) =>
            {
                ProductResponse? product = await productsService.GetProductByCondition(p => p.ProductID == productId);
                return Results.Ok(product);
            });

            // GET: /api/products/search/{searchString}
            productsGroup.MapGet("/search/{searchString}", async (string searchString, IProductService productsService) =>
            {
                string searchPattern = $"%{searchString}%";
                return Results.Ok(await productsService.GetProductsbyCondition(
                    p => EF.Functions.Like(p.ProductName, searchPattern)
                    || EF.Functions.Like(p.Category, searchPattern)));
            });

            // POST: /api/products/
            productsGroup.MapPost("/", async (ProductAddRequest productAddRequest, IProductService productsService) =>
            {
                ProductResponse? addedProduct = await productsService.AddProduct(productAddRequest);
                if (addedProduct != null)
                {
                    return Results.Created($"/api/products/search/product-id/{addedProduct.ProductID}", addedProduct);
                }
                return Results.BadRequest("Failed to add the product.");
            });

            // PUT: /api/products
            productsGroup.MapPut("/", async (ProductUpdateRequest productUpdateRequest, IProductService productsService) =>
            {
                ProductResponse? updatedProduct = await productsService.UpdateProduct(productUpdateRequest);
                if (updatedProduct != null)
                {
                    return Results.Ok(updatedProduct);
                }
                return Results.BadRequest("Failed to update the product.");
            });

            // DELETE: /api/products/{productId}
            productsGroup.MapDelete("/{productId}", async (Guid productId, IProductService productsService) =>
            {
                bool isDeleted = await productsService.DeleteProduct(productId);
                if (isDeleted)
                {
                    return Results.Ok($"Deleted Successflully.");
                }
                return Results.BadRequest($"Failed to delete the product.");
            });

            return app;
        }
    }
}

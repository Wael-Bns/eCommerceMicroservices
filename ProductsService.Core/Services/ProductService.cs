using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using ProductsService.Core.DTO;
using ProductsService.Core.Entities;
using ProductsService.Core.RabbitMQ;
using ProductsService.Core.RepositoryContracts;
using ProductsService.Core.ServiceContracts;

namespace ProductsService.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ProductAddRequest> _productAddRequestValidator;
        private readonly IValidator<ProductUpdateRequest> _productUpdateRequestValidator;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;
        public ProductService(
            IProductsRepository productsRepository,
            IMapper mapper,
            IValidator<ProductUpdateRequest> productUpdateRequestValidator,
            IValidator<ProductAddRequest> productAddRequestValidator,
            IRabbitMQPublisher rabbitMQPublisher)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
            _productUpdateRequestValidator = productUpdateRequestValidator;
            _productAddRequestValidator = productAddRequestValidator;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
        {
            if(productAddRequest == null)
            {
                throw new ArgumentNullException(nameof(productAddRequest), "ProductAddRequest object cannot be null.");
            }

            await _productAddRequestValidator.ValidateAndThrowAsync(productAddRequest);

            Product productToAdd = _mapper.Map<Product>(productAddRequest);
            productToAdd.ProductID = Guid.NewGuid();

            Product? addedProduct = await _productsRepository.AddProduct(productToAdd);
            return _mapper.Map<ProductResponse?>(addedProduct);
        }

        public async Task<bool> DeleteProduct(Guid productID)
        {
            var existingProduct = await _productsRepository.GetProductByCondition(p => p.ProductID == productID);
            if (existingProduct == null)
            {
                throw new ArgumentException("No product found with the given ProductID.", nameof(productID));
            }
            bool isDeleted = await _productsRepository.DeleteProduct(productID);
            return isDeleted;
        }

        public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            if (conditionExpression == null)
            {
                throw new ArgumentNullException(nameof(conditionExpression), "Condition expression cannot be null.");
            }

            Product? product = await _productsRepository.GetProductByCondition(conditionExpression);
            return _mapper.Map<ProductResponse?>(product);
        }

        public async Task<List<ProductResponse>> GetProducts()
        {
            List<Product> products = (await _productsRepository.GetProducts()).ToList();
            return _mapper.Map<List<ProductResponse>>(products);
        }

        public async Task<List<ProductResponse>> GetProductsbyCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            if(conditionExpression == null)
            {
                throw new ArgumentNullException(nameof(conditionExpression), "Condition expression cannot be null.");
            }
            List<Product> products = (await _productsRepository.GetProductsByCondition(conditionExpression)).ToList();
            return _mapper.Map<List<ProductResponse>>(products);
        }

        public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
        {
            if(productUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(productUpdateRequest), "ProductUpdateRequest object cannot be null.");
            }
            var existingProduct = await _productsRepository.GetProductByCondition(p => p.ProductID == productUpdateRequest.ProductID);
            if (existingProduct == null)
            {
                throw new ArgumentException("No product found with the given ProductID.", nameof(productUpdateRequest.ProductID));
            }

            // make model validation on the productUpdateRequest object using FluentValidation
            await _productUpdateRequestValidator.ValidateAndThrowAsync(productUpdateRequest);

            string routingKey = "product.update";

            Product? updatedProduct = await _productsRepository.UpdateProduct(_mapper.Map<Product>(productUpdateRequest));
            return _mapper.Map<ProductResponse?>(updatedProduct);
        }
    }
}

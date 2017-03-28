using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BusinessEntities;
using DataModel;
using DataModel.UnitOfWork;

namespace BusinessServices
{
    public class ProductServices:IProductServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets the product by identifier.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <returns></returns>
        public ProductEntity GetProductById(int productId)
        {
            var product = _unitOfWork.ProductRepository.GetByID(productId);
            if (product != null)
            {
                return MapProductToProductEntity(product);
            }
            return null;
        }

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductEntity> GetAllProducts()
        {
            var products = _unitOfWork.ProductRepository.GetAll().ToList();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    yield return MapProductToProductEntity(product);
                }
            }
        }

        /// <summary>
        /// Creates the product.
        /// </summary>
        /// <param name="productEntity">The product entity.</param>
        /// <returns></returns>
        public int CreateProduct(ProductEntity productEntity)
        {
            using (var scope = new TransactionScope())
            {
                var product = new Product();
                product.ProductName = productEntity.ProductName;
                _unitOfWork.ProductRepository.Insert(product);
                _unitOfWork.Save();
                scope.Complete();
                return product.ProductId;
            }
        }

        /// <summary>
        /// Updates the product.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="productEntity">The product entity.</param>
        /// <returns></returns>
        public bool UpdateProduct(int productId, ProductEntity productEntity)
        {
            var success = false;
            if (productEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var product = _unitOfWork.ProductRepository.GetByID(productId);
                    if (product != null)
                    {
                        product.ProductName = productEntity.ProductName;
                        _unitOfWork.ProductRepository.Update(product);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        public bool DeleteProduct(int productId)
        {
            var success = false;
            if (productId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var product = _unitOfWork.ProductRepository.GetByID(productId);
                    if (product != null)
                    {
                        _unitOfWork.ProductRepository.Delete(product);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }


        /// <summary>
        /// Maps the product to product entity.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns></returns>
        public ProductEntity MapProductToProductEntity(Product product)
        {
            var productEntity = new ProductEntity
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName
            };
            return productEntity;
        }

        public Product MapProductEntityToProduct(ProductEntity productEntity)
        {
            var product = new Product
            {
                ProductId = productEntity.ProductId,
                ProductName = productEntity.ProductName
            };
            return product;
        }
    }
}


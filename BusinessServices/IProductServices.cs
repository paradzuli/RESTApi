using System.Collections.Generic;
using BusinessEntities;
using DataModel;

namespace BusinessServices
{
    public interface IProductServices
    {
        ProductEntity GetProductById(int productId);
        IEnumerable<ProductEntity> GetAllProducts();
        int CreateProduct(ProductEntity productEntity);
        bool UpdateProduct(int productId, ProductEntity productEntity);
        bool DeleteProduct(int productId);

        ProductEntity MapProductToProductEntity(Product product);
        Product MapProductEntityToProduct(ProductEntity productEntity);
    }
}

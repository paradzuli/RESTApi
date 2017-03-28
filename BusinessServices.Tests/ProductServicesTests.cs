using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessEntities;
using DataModel;
using DataModel.GenericRepository;
using DataModel.UnitOfWork;
using Moq;
using NUnit.Framework;
using TestHelper;

namespace BusinessServices.Tests
{
    [TestFixture]
    public class ProductServicesTests
    {
        private IProductServices _productService;
        private IUnitOfWork _unitOfWork;
        private List<Product> _products;
        private GenericRepository<Product> _productRepository;
        private WebApiDbEntities _dbEntities;

        [OneTimeSetUp]
        public void Setup()
        {
            _products = SetUpProducts();
        }

        [SetUp]
        public void ReInitializeTest()
        {
            _dbEntities = new Mock<WebApiDbEntities>().Object;
            _productRepository = SetUpProductRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.ProductRepository).Returns(_productRepository);
            _unitOfWork = unitOfWork.Object;
            _productService = new ProductServices(_unitOfWork);
        }

        private static List<Product> SetUpProducts()
        {
            var prodId = new int();
            var products = DataInitializer.GetAllProducts();
            foreach (Product prod in products)
                prod.ProductId = ++prodId;
            return products;
        }


        private GenericRepository<Product> SetUpProductRepository()
        {
            //Initialise repository
            var mockRepo = new Mock<GenericRepository<Product>>(MockBehavior.Default, _dbEntities);

            //Setup mocking behavior
            mockRepo.Setup(p => p.GetAll()).Returns(_products);

            mockRepo.Setup(p => p.GetByID(It.IsAny<int>()))
                .Returns(new Func<int, Product>(id => _products.Find(p => p.ProductId.Equals(id))));

            mockRepo.Setup(p => p.Insert(It.IsAny<Product>()))
                .Callback(new Action<Product>(newProduct =>
                {
                    dynamic maxProductID = _products.Last().ProductId;
                    dynamic nextProductID = maxProductID + 1;
                    newProduct.ProductId = nextProductID;
                    _products.Add(newProduct);
                }));

            mockRepo.Setup(p => p.Update(It.IsAny<Product>())).Callback(new Action<Product>(prod =>
            {
                var oldProduct = _products.Find(a => a.ProductId == prod.ProductId);
                oldProduct = prod;
            }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Product>())).Callback(new Action<Product>(prod =>
            {
                var productToRemove = _products.Find(a => a.ProductId == prod.ProductId);
                if (productToRemove != null)
                    _products.Remove(productToRemove);
            }));

            return mockRepo.Object;

        }

        public IEnumerable<ProductEntity> GetAllProducts()
        {
            var products = _unitOfWork.ProductRepository.GetAll().ToList();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    yield return _productService.MapProductToProductEntity(product);
                }
            }
            else
            yield return null;
        }

        [Test]
        public void GetAllProductsTest()
        {
            var products = _productService.GetAllProducts();
            var productList =
                products.Select(
                    productEntity =>
                        new Product {ProductId = productEntity.ProductId, ProductName = productEntity.ProductName})
                    .ToList();
            var comparer = new ProductComparer();
            CollectionAssert.AreEqual(productList.OrderBy(product=>product,comparer),_products.OrderBy(product=>product,comparer),comparer);
        }


        [Test]
        public void GetAllProductsTestForNull()
        {
            _products.Clear();
            var products = _productService.GetAllProducts();
            Assert.AreEqual(products.Count(),0);
            SetUpProducts();
        }

        [Test]
        public void GetProductByRightIdTest()
        {
            var mobileProduct = _productService.GetProductById(2);
            if (mobileProduct!=null)
            {
                var product = _productService.MapProductEntityToProduct(mobileProduct);
                AssertObjects.PropertyValuesAreEquals(product, _products.Find(z=>z.ProductName.Contains("Mobile")));
            }
        }

        [Test]
        public void GetProductByWrongIdTest()
        {
            var product = _productService.GetProductById(0);
            Assert.Null(product);
        }

        [Test]
        public void AddNewProductTest()
        {
            var newProduct = new ProductEntity();
            newProduct.ProductName = "Android Phone";

            var maxProductIdBeforeAdd = _products.Max(a => a.ProductId);
            newProduct.ProductId = maxProductIdBeforeAdd + 1;
            _productService.CreateProduct(newProduct);
            var addedProduct = new Product{ProductId = newProduct.ProductId, ProductName = newProduct.ProductName};
            AssertObjects.PropertyValuesAreEquals(addedProduct,_products.Last());
            Assert.That(maxProductIdBeforeAdd+1, Is.EqualTo(_products.Last().ProductId));
        }

        [Test]
        public void UpdateProductTest()
        {
            var firstProduct = _products.First();
            firstProduct.ProductName = "Laptop updated";
            var updateProduct = new ProductEntity
            {
                ProductName = firstProduct.ProductName,
                ProductId = firstProduct.ProductId
            };
            _productService.UpdateProduct(firstProduct.ProductId, updateProduct);
            Assert.That(firstProduct.ProductId,Is.EqualTo(1));
            Assert.That(firstProduct.ProductName, Is.EqualTo("Laptop updated"));
        }

        [Test]
        public void DeleteProductTest()
        {
            int maxId = _products.Max(a => a.ProductId);
            var lastProduct = _products.Last();
            _productService.DeleteProduct(lastProduct.ProductId);
            Assert.That(maxId,Is.GreaterThan(_products.Max(a=>a.ProductId)));
        }

        [TearDown]
        public void DisposeTest()
        {
            _productService = null;
            _unitOfWork = null;
            _productRepository = null;
            if(_dbEntities!=null)
                _dbEntities.Dispose();
        }


        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _products = null;
        }

    }
}

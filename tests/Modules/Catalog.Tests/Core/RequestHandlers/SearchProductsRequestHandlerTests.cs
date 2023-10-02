// using AutoFixture.Xunit2;
//
// using Catalog.Core.Model;
// using Catalog.Core.Repository;
// using Catalog.Core.RequestHandlers;
// using Catalog.Core.Requests;
// using Catalog.Infrastructure.Repositories;
// using Catalog.Tests.Fixtures;
//
// using Shouldly;
//
// namespace Catalog.Tests.Core.RequestHandlers;
//
// [UsesVerify]
// public sealed class SearchProductsRequestHandlerTests : IClassFixture<PostgresSqlFixture>, IDisposable
// {
//     private readonly PostgresSqlFixture _postgresSqlFixture;
//     private readonly IProductFilter _productFilter;
//     private readonly SearchProductsRequestHandler _searchProductsRequestHandler;
//     private readonly IProductsWriter _productsWriter;
//     public SearchProductsRequestHandlerTests(PostgresSqlFixture postgresSqlFixture)
//     {
//         _postgresSqlFixture = postgresSqlFixture;
//         _productFilter = new EfCoreProductFilter(_postgresSqlFixture.DbContextFactory);
//         _productsWriter = new EfCoreProductsWriter(_postgresSqlFixture.DbContextFactory);
//         _searchProductsRequestHandler = new SearchProductsRequestHandler(_productFilter);
//     }
//     
//
//     [Theory]
//     [InlineAutoData]
//     public async Task ReadFilterProductsWhenNoExistsShouldReturnEmptyCollection(SearchProducts query)
//     {
//         // Act
//         var subject = await _searchProductsRequestHandler.Handle(query, CancellationToken.None);
//         
//         subject.ShouldNotBeNull();
//         subject.Count.ShouldBe(0u);
//         subject.Data.ShouldBeEmpty();
//     }
//
//
//     [Theory]
//     [AutoData]
//     public async Task FilterProductWhenNiveaProductExistsShouldReturnProductsWithNameOrDescriptionContainsNiveaKeyword(string keyword)
//     {
//         // Arrange 
//         using var cts = new CancellationTokenSource();
//         cts.CancelAfter(TimeSpan.FromSeconds(30));
//
//         var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription($"some {keyword}"),
//             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
//         var product2 = new Product(ProductId.New(), new ProductName($"{keyword.ToUpperInvariant()} xDDD"), new ProductDescription("xDDD"),
//             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
//         var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
//             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
//         await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
//         // Act
//
//         var subject = await _searchProductsRequestHandler.Handle(new SearchProducts() { Query = keyword }, cts.Token);
//         
//         // Assert
//         subject.ShouldNotBeNull();
//         subject.Data.ShouldNotBeEmpty();
//         
//         subject.Data.ShouldContain(dto => dto.ProductId == product1.Id.Value);
//         subject.Data.ShouldContain(dto => dto.ProductId == product2.Id.Value);
//         subject.Data.ShouldNotContain(dto => dto.ProductId == product3.Id.Value);
//     }
//     
//     [Fact]
//     public async Task FilterProductWhenProductsInPriceConditionExistsShouldReturnProductsWithPriceCondition()
//     {
//         // Arrange 
//         
//         var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
//             new ProductPrice(new Price(5m), new Price(1m)), new AvailableQuantity(10), Tags.Empty);
//         var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
//             new ProductPrice(new Price(11m)), new AvailableQuantity(10), Tags.Empty);
//         var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
//             new ProductPrice(new Price(20m), new Price(20m)), new AvailableQuantity(10), Tags.Empty);
//         await _productsWriter.AddProducts(new[] { product1, product2, product3 });
//         // Act
//         var subject =
//             await _searchProductsRequestHandler.Handle(new SearchProducts() { PriceFrom = 2m, PriceTo = 12m }, default);
//
//         // Assert
//         subject.ShouldNotBeNull();
//         subject.Data.ShouldNotBeEmpty();
//         subject.Count.ShouldBe(1u);
//         
//         await Verify(subject);
//     }
//     
//     [Fact]
//     public async Task FilterProductWhenNiveaProductExistsShouldReturnOneProductWithNameOrDescriptionContainsNiveaKeyword()
//     {
//         // Arrange 
//         var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
//             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
//         var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
//             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
//         var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
//             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
//         await _productsWriter.AddProducts(new[] { product1, product2, product3 });
//         // Act
//         var subject =
//             await _searchProductsRequestHandler.Handle(new SearchProducts() { Query = "nivea", PageSize = 1 }, default);
//
//         // Assert
//         subject.ShouldNotBeNull();
//         subject.Data.ShouldNotBeEmpty();
//         subject.Count.ShouldBe(1u);
//         
//         await Verify(subject);
//     }
//     
//     [Fact]
//     public async Task FilterProductWhenNiveaProductExistsShouldReturnOneProductWithNameOrDescriptionContainsNiveaKeywordOnTheSecondPage()
//     {
//         // Arrange 
//         using var cts = new CancellationTokenSource();
//         cts.CancelAfter(TimeSpan.FromSeconds(30));
//
//         var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
//             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
//         var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
//             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
//         var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
//             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty);
//         await _productsWriter.AddProducts(new[] { product1, product2, product3 }, cts.Token);
//         // Act
//
//         var subject =
//             await _searchProductsRequestHandler.Handle(new SearchProducts() { Query = "nivea", PageSize = 1, Page = 2 },
//                 cts.Token);
//
//         // Assert
//         subject.ShouldNotBeNull();
//         subject.Data.ShouldNotBeEmpty();
//         subject.Count.ShouldBe(1u);
//         
//         await Verify(subject);
//     }
//
// #pragma warning disable CA1816
// #pragma warning disable CA1063
//     public void Dispose()
// #pragma warning restore CA1063
// #pragma warning restore CA1816
//     {
//         _postgresSqlFixture.DbContext.Products.RemoveRange(_postgresSqlFixture.DbContext.Products);
//         _postgresSqlFixture.DbContext.SaveChanges();
//     }
// }
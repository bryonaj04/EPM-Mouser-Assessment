
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EPM.Mouser.Interview.Web.Controllers;
using Moq.Protected;
using System.Reflection;
using EPM.Mouser.Interview.Models;
using EPM.Mouser.Interview.Data;

namespace EPM.Mouser.Interview.Tests;

[TestFixture]
public class Tests
{
    [Test]
    public async Task GetProduct_WithValidId_ReturnsProduct()
    {
        // Arrange
        var id = 1; 

        var expectedProduct = new Product{
         Id = 1,
         InStockQuantity = 1,
         Name = "Test",
         ReservedQuantity = 0};

        var mockWarehouseRepository = new Mock<IWarehouseRepository>();
        mockWarehouseRepository.Setup(repo => repo.Get(id)).ReturnsAsync(expectedProduct);
        var warehouseApiController = new WarehouseApi(mockWarehouseRepository.Object);

        // Act
        var result = await warehouseApiController.GetProduct(id);

        // Assert
        Assert.That(result, Is.EqualTo(expectedProduct));
    }


    [Test]
    public async Task GetProductList_ReturnsListOfProduct()
    {
        // Arrange  
   
        var expectedProducts = new List<Product>()
        {
          new Product { Id = 1 , InStockQuantity = 2,  ReservedQuantity = 1, Name = "Test"},
          new Product { Id = 2 , InStockQuantity = 2,  ReservedQuantity = 1, Name = "Test1"},
          new Product { Id = 3 , InStockQuantity = 2,  ReservedQuantity = 1, Name = "Test2"},
          new Product { Id = 4 , InStockQuantity = 2,  ReservedQuantity = 1, Name = "Test3"}
        };

        var mockWarehouseRepository = new Mock<IWarehouseRepository>();
        mockWarehouseRepository.Setup(repo => repo.List()).ReturnsAsync(expectedProducts);

        var warehouseApiController = new WarehouseApi(mockWarehouseRepository.Object);

        // Act
        var result = await warehouseApiController.GetPublicInStockProducts();

        // Assert
        Assert.That(result, Is.EqualTo(expectedProducts));
    }


    [Test]
    public async Task GetProductList_ReturnsFilteredListOfProduct()
    {
        // Arrange  

        var listOfProducts = new List<Product>()
        {
          new Product { Id = 1 , InStockQuantity = 2,  ReservedQuantity = 1, Name = "Test"},
          new Product { Id = 2 , InStockQuantity = 2,  ReservedQuantity = 1, Name = "Test1"},
          new Product { Id = 3 , InStockQuantity = 2,  ReservedQuantity = 1, Name = "Test2"},
          new Product { Id = 4 , InStockQuantity = 1,  ReservedQuantity = 1, Name = "Test3"}
        };

        var expectedProducts = listOfProducts
        .Where(x => x.InStockQuantity > 0 && x.InStockQuantity > x.ReservedQuantity)
        .ToList();


        var mockWarehouseRepository = new Mock<IWarehouseRepository>();
        mockWarehouseRepository.Setup(repo => repo.List()).ReturnsAsync(expectedProducts);

        var warehouseApiController = new WarehouseApi(mockWarehouseRepository.Object);

        // Act
        var result = await warehouseApiController.GetPublicInStockProducts();

        // Assert
        Assert.That(result, Is.EqualTo(expectedProducts));
    }

    [Test]
    public async Task OrderItem_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var mockWarehouseRepository = new Mock<IWarehouseRepository>();
        var warehouseApiController = new WarehouseApi(mockWarehouseRepository.Object); 

        var updateQuantityRequest = new UpdateQuantityRequest
        {
            Id = 123, 
            Quantity = 5 
        };


        mockWarehouseRepository
            .Setup(r => r.Get(updateQuantityRequest.Id))
            .ReturnsAsync(new Product
            {
                
                InStockQuantity = 10,
                ReservedQuantity = 2
            });

        // Act
        var result = await warehouseApiController.OrderItem(updateQuantityRequest);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorReason);

    }


    [Test]
    public async Task ShipItem_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var mockWarehouseRepository = new Mock<IWarehouseRepository>();
        var warehouseApiController = new WarehouseApi(mockWarehouseRepository.Object);

        var updateQuantityRequest = new UpdateQuantityRequest
        {
            Id = 123,
            Quantity = 5
        };


        mockWarehouseRepository
            .Setup(r => r.Get(updateQuantityRequest.Id))
            .ReturnsAsync(new Product
            {

                InStockQuantity = 10,
                ReservedQuantity = 2
            });

        // Act
        var result = await warehouseApiController.ShipItem(updateQuantityRequest);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorReason);

    }

    [Test]
    public async Task Restocktem_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var mockWarehouseRepository = new Mock<IWarehouseRepository>();
        var warehouseApiController = new WarehouseApi(mockWarehouseRepository.Object);

        var updateQuantityRequest = new UpdateQuantityRequest
        {
            Id = 123,
            Quantity = 5
        };


        mockWarehouseRepository
            .Setup(r => r.Get(updateQuantityRequest.Id))
            .ReturnsAsync(new Product
            {

                InStockQuantity = 10,
                ReservedQuantity = 2
            });

        // Act
        var result = await warehouseApiController.RestockItem(updateQuantityRequest);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorReason);

    }
    [Test]
    public async Task AddNewProduct_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var mockWarehouseRepository = new Mock<IWarehouseRepository>();
        var warehouseApiController = new WarehouseApi(mockWarehouseRepository.Object);

        var listOfProducts = new List<Product>()
        {
          new Product { Id = 1 , InStockQuantity = 2,  ReservedQuantity = 1, Name = "Test"},
          new Product { Id = 2 , InStockQuantity = 2,  ReservedQuantity = 1, Name = "Test1"},
          new Product { Id = 3 , InStockQuantity = 2,  ReservedQuantity = 1, Name = "Test2"},
          new Product { Id = 4 , InStockQuantity = 1,  ReservedQuantity = 1, Name = "Test3"}
        };

        mockWarehouseRepository.Setup(repo => repo.List()).ReturnsAsync(listOfProducts);

        var Product = new Product
        {
            Id = 1,
            InStockQuantity = 1,
            Name = "Test",
            ReservedQuantity = 0
        };

        // Act
        var result = await warehouseApiController.AddNewProduct(Product);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorReason);
    }
}

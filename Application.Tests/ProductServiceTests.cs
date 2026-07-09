using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _service = new ProductService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    ProductName = "Laptop",
                    CreatedBy = "Admin",
                    CreatedOn = new DateTime(2026,01,01)
                },
                new Product
                {
                    Id = 2,
                    ProductName = "Mouse",
                    CreatedBy = "Admin",
                    CreatedOn = new DateTime(2026,01,02)
                }
            };

            _repositoryMock.Setup(x => x.GetAllAsync())
                           .ReturnsAsync(products);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoProducts()
        {
            // Arrange
            _repositoryMock.Setup(x => x.GetAllAsync())
                           .ReturnsAsync(new List<Product>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Keyboard",
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2026, 01, 01)
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1))
                           .ReturnsAsync(product);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.ProductName.Should().Be("Keyboard");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductNotFound()
        {
            // Arrange
            _repositoryMock.Setup(x => x.GetByIdAsync(1))
                           .ReturnsAsync((Product?)null);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddAsync_ShouldCallRepositoryAndSaveChanges()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                ProductName = "Monitor",
                CreatedBy = "Admin"
            };

            // Act
            await _service.AddAsync(dto);

            // Assert
            _repositoryMock.Verify(x =>
                x.AddAsync(It.Is<Product>(p =>
                    p.ProductName == dto.ProductName &&
                    p.CreatedBy == dto.CreatedBy)),
                Times.Once);

            _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Old Product"
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1))
                           .ReturnsAsync(product);

            var dto = new UpdateProductDto
            {
                ProductName = "New Product",
                ModifiedBy = "Admin"
            };

            // Act
            await _service.UpdateAsync(1, dto);

            // Assert
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);
            _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldNotUpdate_WhenProductNotFound()
        {
            // Arrange
            _repositoryMock.Setup(x => x.GetByIdAsync(1))
                           .ReturnsAsync((Product?)null);

            var dto = new UpdateProductDto
            {
                ProductName = "New Product",
                ModifiedBy = "Admin"
            };

            // Act
            await _service.UpdateAsync(1, dto);

            // Assert
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);
            _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Laptop"
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1))
                           .ReturnsAsync(product);

            // Act
            await _service.DeleteAsync(1);

            // Assert
            _repositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Product>()), Times.Once);
            _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotDelete_WhenProductNotFound()
        {
            // Arrange
            _repositoryMock.Setup(x => x.GetByIdAsync(1))
                           .ReturnsAsync((Product?)null);

            // Act
            await _service.DeleteAsync(1);

            // Assert
            _repositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Product>()), Times.Never);
            _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
    }
}
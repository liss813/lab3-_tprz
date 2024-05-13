using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using ShoppingCart.Tests.Datasets;
using ShoppingCart.Web.Areas.Admin.Controllers;
using Xunit;

namespace ShoppingCart.Tests
{
    public class UnitTestForCC
    {
        [Fact]
        public void CreateUpdate_InvalidModel_ThrowsException()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var controller = new CategoryController(mockUnitOfWork.Object);
            controller.ModelState.AddModelError("Name", "Name is required");
            var vm = new CategoryVM();
            Assert.Throws<Exception>(() => controller.CreateUpdate(vm));
        }
 
        [Fact]
        public void DeleteCategory_NonexistentId_ThrowsException()
        {
            int nonExistentId = 12;
            Mock<ICategoryRepository> repositoryMock = new Mock<ICategoryRepository>();
            repositoryMock.Setup(r => r.Delete(null))
                .Throws<Exception>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Category).Returns(repositoryMock.Object);
            var controller = new CategoryController(mockUnitOfWork.Object);
            Assert.Throws<Exception>(() => controller.DeleteData(nonExistentId));
        }
    
        [Fact]
        public void UpdateCategory_ValidModel_UpdatesCategory()
        {
            int categoryId = 2;
            var category = new Category { Id = categoryId, Name = "Updated Name" };
            var vm = new CategoryVM { Category = category };
            Mock<ICategoryRepository> repositoryMock = new Mock<ICategoryRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Category).Returns(repositoryMock.Object);
            var controller = new CategoryController(mockUnitOfWork.Object);
            controller.CreateUpdate(vm);
            repositoryMock.Verify(r => r.Update(It.Is<Category>(c => c.Id == categoryId && c.Name == category.Name)));
            mockUnitOfWork.Verify(uow => uow.Save());
        }
        [Fact]
        public void GetCategories_All_ReturnAllCategories()
        {
            Mock<ICategoryRepository> repositoryMock = new Mock<ICategoryRepository>();
            repositoryMock.Setup(r => r.GetAll(It.IsAny<string>()))
                .Returns(() => CategoryDataset.Categories);
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Category).Returns(repositoryMock.Object);
            var controller = new CategoryController(mockUnitOfWork.Object);
            var result = controller.Get();
            Assert.Equal(CategoryDataset.Categories, result.Categories);
        }

        [Fact]
        public void CreateCategory_ValidModel_SavesCategory()
        {
            var category = new Category { Name = "Test Category" };
            var categoryVM = new CategoryVM { Category = category };
            Mock<ICategoryRepository> repositoryMock = new Mock<ICategoryRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Category).Returns(repositoryMock.Object);
            var controller = new CategoryController(mockUnitOfWork.Object);
            controller.CreateUpdate(categoryVM);
            repositoryMock.Verify(r => r.Add(It.Is<Category>(c => c.Name == category.Name)));
            mockUnitOfWork.Verify(uow => uow.Save());
        }
    }
}

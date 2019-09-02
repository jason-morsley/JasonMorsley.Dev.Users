using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Users.API.Entities;
using Users.API.Helpers;
using Users.API.Models;
using Users.API.Services;
using Users.API.UnitTests.Models;
using Xunit;

namespace Users.API.UnitTests
{
    public class ObjectExtensionTests
    {
        private Fixture _fixture;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public ObjectExtensionTests()
        {
            _typeHelperService = new TypeHelperService();
            _propertyMappingService = new PropertyMappingService();
            _fixture = new Fixture();
        }

        [Fact]
        public void Given_An_Object_When_I_Call_ShapeData_With_Fields_Then_The_Object_Without_The_Fields_Corresponding_To_Fields_Should_Be_Returned()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var usersResourceParameters = new UsersResourceParameters {Fields = "Id"};

            // Act
            var shapedUser = user.ShapeData(usersResourceParameters.Fields);

            // Assert
            Assert.True(shapedUser.Count() == 1);
        }

        [Fact]
        public void Given_An_Object_When_I_Call_ShapeData_Without_Fields_Then_The_Object_With_All_Original_Fields_Should_Be_Returned()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var usersResourceParameters = new UsersResourceParameters { Fields = "" };

            // Act
            var shapedUser = user.ShapeData(usersResourceParameters.Fields);

            // Assert
            Assert.True(shapedUser.Count() == 3);
        }

        [Fact]
        public void Given_A_Null_Object_When_I_Call_ShapeData_Without_Fields_Then_The_Object_With_All_Original_Fields_Should_Be_Returned()
        {
            // Arrange
            var usersResourceParameters = new UsersResourceParameters { Fields = "Id" };

            // Act
            var shapedUserException = Assert.Throws<System.ArgumentNullException>(() => ((object) null).ShapeData(usersResourceParameters.Fields));

            // Assert
            Assert.Equal("Value cannot be null.\r\nParameter name: source", shapedUserException.Message);
        }

        [Fact]
        public void Given_Invalid_PropertyMapping_When_GetPropertyMapping_Is_Called_Then_Throws_Exception()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var usersResourceParameters = new UsersResourceParameters { Fields = "Invalid Field To Shape An Object By" };

            // Act
            var shapedUserException = Assert.Throws<System.Exception>((() => user.ShapeData(usersResourceParameters.Fields)));

            // Assert
            Assert.Equal("Property Invalid Field To Shape An Object By wasn't found on Users.API.Entities.User", shapedUserException.Message);
        }
    }
}

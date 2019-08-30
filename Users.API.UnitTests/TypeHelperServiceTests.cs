using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Users.API.Entities;
using Users.API.Helpers;
using Users.API.Services;
using Xunit;

namespace Users.API.UnitTests
{
    public class TypeHelperServiceTests
    {
        private Fixture _fixture;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public TypeHelperServiceTests()
        {
            _typeHelperService = new TypeHelperService();
            _propertyMappingService = new PropertyMappingService();
            _fixture = new Fixture();
        }

        [Fact]
        public void Given_A_Object_When_I_Call_TypeHasProperties_On_That_Object_With_A_Valid_Field_Then_Returns_True()
        {
            // Arrange
            var usersResourceParameters = new UsersResourceParameters { Fields = "Id"};
            
            // Act
            var typeHasProperties = _typeHelperService.TypeHasProperties<User>(usersResourceParameters.Fields);

            // Assert
            Assert.True(typeHasProperties);
        }

        [Fact]
        public void Given_A_Object_When_I_Call_TypeHasProperties_On_That_Object_With_A_Invalid_Field_Then_Returns_False()
        {
            // Arrange
            var usersResourceParameters = new UsersResourceParameters { Fields = "Invalid field" };

            // Act
            var typeHasProperties = _typeHelperService.TypeHasProperties<User>(usersResourceParameters.Fields);

            // Assert
            Assert.False(typeHasProperties);
        }

        [Fact]
        public void Given_A_Object_When_I_Call_TypeHasProperties_On_That_Object_With_An_Empty_Field_Then_Returns_True()
        {
            // Arrange
            var usersResourceParameters = new UsersResourceParameters { Fields = "" };

            // Act
            var typeHasProperties = _typeHelperService.TypeHasProperties<User>(usersResourceParameters.Fields);

            // Assert
            Assert.True(typeHasProperties);
        }
    }
}

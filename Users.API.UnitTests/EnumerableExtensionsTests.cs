using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Users.API.Entities;
using Users.API.Helpers;
using Users.API.Models;
using Users.API.Profile;
using Users.API.Services;
using Xunit;

namespace Users.API.UnitTests
{
    public class EnumerableExtensionsTests
    {
        private Fixture _fixture;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public EnumerableExtensionsTests()
        {
            _typeHelperService = new TypeHelperService();
            _propertyMappingService = new PropertyMappingService();
            _fixture = new Fixture();
        }

        [Fact]
        public void Given_An_Object_When_I_Call_ShapeData_Then_The_Object_Should_Be_Returned()
        {
            // Arrange
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<UserDto, User>(pmd);
            _propertyMappingService.AddPropertyMapping<UserDto, User>(pm);
            var user = _fixture.Create<User>();
            var usersResourceParameters = new UsersResourceParameters { Fields = "Id" };
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);
            var usersRepository = new UsersRepository(context, _propertyMappingService);
            context.Add(user);
            context.SaveChanges();
            var usersFromRepo = usersRepository.GetUsers(usersResourceParameters);

            // Act
            var shapedUsers = IEnumerableExtensions.ShapeData(usersFromRepo, usersResourceParameters.Fields);

            // Assert
            Assert.True(shapedUsers.Count() == 1);
        }

        [Fact]
        public void Given_An_Object_When_I_Call_ShapeData_With_Null_Fields_Then_The_Object_Should_Be_Returned()
        {
            // Arrange
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<UserDto, User>(pmd);
            _propertyMappingService.AddPropertyMapping<UserDto, User>(pm);
            var user = _fixture.Create<User>();
            var usersResourceParameters = new UsersResourceParameters { Fields = "" };
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);
            var usersRepository = new UsersRepository(context, _propertyMappingService);
            context.Add(user);
            context.SaveChanges();
            var usersFromRepo = usersRepository.GetUsers(usersResourceParameters);

            // Act
            var shapedUsers = IEnumerableExtensions.ShapeData(usersFromRepo, usersResourceParameters.Fields);

            // Assert
            Assert.True(shapedUsers.Count() == 1);
        }

        [Fact]
        public void Given_An_Object_When_I_Call_ShapeData_With_Null_PropertyInfo_Then_The_Object_Should_Be_Returned()
        {
            // Arrange
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<UserDto, User>(pmd);
            _propertyMappingService.AddPropertyMapping<UserDto, User>(pm);
            var user = _fixture.Create<User>();
            var usersResourceParameters = new UsersResourceParameters { Fields = "Invalid Field To Shape An Object By" };
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);
            var usersRepository = new UsersRepository(context, _propertyMappingService);
            context.Add(user);
            context.SaveChanges();
            var usersFromRepo = usersRepository.GetUsers(usersResourceParameters);

            // Act
            var shapedUserException = Assert.Throws<System.Exception>(() => IEnumerableExtensions.ShapeData(usersFromRepo, usersResourceParameters.Fields));

            // Assert
            Assert.Equal("Property Invalid Field To Shape An Object By wasn't found on Users.API.Entities.User", shapedUserException.Message);
        }
    }
}
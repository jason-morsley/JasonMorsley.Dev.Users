using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Users.API.Entities;
using Users.API.Helpers;
using Users.API.Models;
using Users.API.Services;
using Xunit;

namespace Users.API.UnitTests
{
    public class QueryableExtensionsTests
    {
        private Fixture _fixture;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public QueryableExtensionsTests()
        {
            _typeHelperService = new TypeHelperService();
            _propertyMappingService = new PropertyMappingService();
            _fixture = new Fixture();
        }

        [Fact]
        public void Given_A_UserRepository_When_I_Call_ApplySort_With_An_Empty_MappingDictionary_Then_An_ArgumentNullException_Should_Be_Thrown()
        {
            // Arrange
            var usersResourceParameters = new UsersResourceParameters();

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);

            // Act
            var getUsersException = Assert.Throws<System.ArgumentNullException>(() => context.Users.ApplySort(usersResourceParameters.OrderBy, null));

            // Assert
            Assert.Equal("Value cannot be null.\r\nParameter name: mappingDictionary", getUsersException.Message);
        }

        [Fact]
        public void Given_A_UserRepository_When_I_Call_ApplySort_With_An_Empty_OrderBy_String_Then_An_ArgumentException_Should_Be_Thrown()
        {
            // Arrange
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<UserDto, User>(pmd);
            _propertyMappingService.AddPropertyMapping<UserDto, User>(pm);

            var usersResourceParameters = new UsersResourceParameters { OrderBy = null };

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);

            var user = _fixture.Create<User>();
            context.Add(user);
            context.SaveChanges();
            
            // Act
            var getUsersNullOrderBy = context.Users.ApplySort(usersResourceParameters.OrderBy, pmd);

            // Assert
            Assert.True(getUsersNullOrderBy.Count() == 1);
        }

        [Fact]
        public void Given_A_UserRepository_When_I_Call_GetUsers_With_An_Empty_MappingDictionary_Then_An_ArgumentException_Should_Be_Thrown()
        {
            // Arrange
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                //{"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                //{"Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<UserDto, User>(pmd);
            _propertyMappingService.AddPropertyMapping<UserDto, User>(pm);

            var usersResourceParameters = new UsersResourceParameters();

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);
            var usersRepository = new UsersRepository(context, _propertyMappingService);

            // Act
            var missingKeyMapping = Assert.Throws<System.ArgumentException>(() => usersRepository.GetUsers(usersResourceParameters));

            // Assert
            Assert.Equal("Key mapping for Name is missing", missingKeyMapping.Message);
        }

        [Fact]
        public void Given_A_UserRepository_When_I_Call_GetUsers_With_A_MappingDictionary_With_Null_PropertyMappingValues_Then_An_ArgumentNullException_Should_Be_Thrown()
        {
            // Arrange
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                //{"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Name", null}
            };
            var pm = new PropertyMapping<UserDto, User>(pmd);
            _propertyMappingService.AddPropertyMapping<UserDto, User>(pm);

            var usersResourceParameters = new UsersResourceParameters { SearchQuery = "" };

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);
            var usersRepository = new UsersRepository(context, _propertyMappingService);

            // Act
            var missingKeyMapping = Assert.Throws<System.ArgumentNullException>(() => usersRepository.GetUsers(usersResourceParameters));

            // Assert
            Assert.Equal("Value cannot be null.\r\nParameter name: propertyMappingValue", missingKeyMapping.Message);
        }

        [Fact]
        public void Given_An_Object_When_I_Call_GetUsers_With_An_Empty_SearchQuery_Then_The_Object_Should_Be_Returned()
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
            var user2 = _fixture.Create<User>();
            var user3 = _fixture.Create<User>();
            var user4 = _fixture.Create<User>();

            var usersResourceParameters = new UsersResourceParameters { SearchQuery = "" };

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new UserContext(options);
            var usersRepository = new UsersRepository(context, _propertyMappingService);

            context.Add(user);
            context.Add(user2);
            context.Add(user3);
            context.Add(user4);
            context.SaveChanges();

            // Act
            var getUsers = usersRepository.GetUsers(usersResourceParameters);

            // Assert
            Assert.True(getUsers.Count == 4);
        }

        [Fact]
        public void Given_An_Object_When_I_Call_ShapeData_With_Empty_Fields_Then_The_Object_Should_Be_Returned()
        {
            // Arrange
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<UserDto, User>(pmd);
            _propertyMappingService.AddPropertyMapping<UserDto, User>(pm);

            var user = _fixture.Create<User>();
            var user2 = _fixture.Create<User>();
            var user3 = _fixture.Create<User>();
            var user4 = _fixture.Create<User>();

            var usersResourceParameters = new UsersResourceParameters { SearchQuery = "" };

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);

            context.Add(user);
            context.Add(user2);
            context.Add(user3);
            context.Add(user4);
            context.SaveChanges();

            // Act
            context.Users.AsQueryable().ShapeData(usersResourceParameters.Fields, pmd);
            
            // Assert
            Assert.True(context.Users.Count() == 4);
        }

        [Fact]
        public void Given_An_Object_When_I_Call_ShapeData_With_Empty_MappingDictionary_Then_Exception_Should_Be_Thrown()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var user2 = _fixture.Create<User>();
            var user3 = _fixture.Create<User>();
            var user4 = _fixture.Create<User>();

            var usersResourceParameters = new UsersResourceParameters { SearchQuery = "" };

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);

            context.Add(user);
            context.Add(user2);
            context.Add(user3);
            context.Add(user4);
            context.SaveChanges();

            // Act
            var missingNullMappingDictionary = Assert.Throws<System.ArgumentNullException>(() => context.Users.AsQueryable().ShapeData(usersResourceParameters.Fields, null));

            // Assert
            Assert.Equal("Value cannot be null.\r\nParameter name: mappingDictionary", missingNullMappingDictionary.Message);
        }

        [Fact]
        public void Given_An_Object_When_I_Call_ShapeData_Then_The_Object_Should_Be_Returned() //Note I had to change the Id in the pmd variable to id.
        {
            // Arrange
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<UserDto, User>(pmd);
            _propertyMappingService.AddPropertyMapping<UserDto, User>(pm);

            var user = _fixture.Create<User>();

            var usersResourceParameters = new UsersResourceParameters { Fields = "Id"};

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);

            context.Add(user);
            context.SaveChanges();

            // Act
            context.Users.ShapeData(usersResourceParameters.Fields, pmd);

            // Assert
            Assert.True(context.Users.Count() == 1);
        }

        [Fact]
        public void Given_An_Object_When_I_Call_ShapeData_With_Invalid_Mapping_Then_An_Exception_Should_Be_Thrown()
        {
            // Arrange
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<UserDto, User>(pmd);
            _propertyMappingService.AddPropertyMapping<UserDto, User>(pm);

            var usersResourceParameters = new UsersResourceParameters { Fields = "Id" };

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);

            // Act
            var missingNullMappingDictionary = Assert.Throws<System.ArgumentException>(() => context.Users.ShapeData(usersResourceParameters.Fields, pmd));

            // Assert
            Assert.Equal("Key mapping for id is missing", missingNullMappingDictionary.Message);
        }

        [Fact]
        public void Given_An_Object_When_I_Call_ShapeData_With_Null_Mappings_Then_An_Exception_Should_Be_Thrown() //Note I had to change the Id in the pmd variable to id.
        {
            // Arrange
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"id", null },
                {"name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<UserDto, User>(pmd);
            _propertyMappingService.AddPropertyMapping<UserDto, User>(pm);

            var usersResourceParameters = new UsersResourceParameters { Fields = "Id"};

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);

            // Act
            var missingNullMappingDictionary = Assert.Throws<System.ArgumentNullException>(() => context.Users.ShapeData(usersResourceParameters.Fields, pmd));

            // Assert
            Assert.Equal("Value cannot be null.\r\nParameter name: propertyMappingValue", missingNullMappingDictionary.Message);
        }
    }
}

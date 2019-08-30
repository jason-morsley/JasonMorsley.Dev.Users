using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Users.API.Entities;
using Users.API.Services;
using NSubstitute;
using Users.API.Helpers;
using Users.API.Models;
using Xunit;

namespace Users.API.UnitTests
{
    public class UsersRepositoryTests
    {
        private Fixture _fixture;
        private IPropertyMappingService _propertyMappingService;

        public UsersRepositoryTests()
        {
            _propertyMappingService = new PropertyMappingService();;
            _fixture = new Fixture();
        }

        [Fact]
        //Given_?_When_?_Then_?
        public void Given_A_UserRepository_When_I_Call_AddUser_Then_A_User_Should_Be_Added()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);
            var mapping = Substitute.For<IPropertyMappingService>();
            var usersRepository = new UsersRepository(context, mapping);

            // Act
            usersRepository.AddUser(user);
            usersRepository.Save();

            // Assert
            Assert.Equal(1, context.Users.Count());
        }

        [Fact]
        public void Given_A_UserRepository_When_I_Call_DeleteUser_Then_A_User_Should_Be_Deleted()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);
            var usersRepository = new UsersRepository(context, _propertyMappingService);
            context.Add(user);
            // Act
            usersRepository.DeleteUser(user);
            usersRepository.Save();

            // Assert
            Assert.Equal(0, context.Users.Count());
        }

        [Fact]
        public void Given_A_UserRepository_When_I_Call_GetUsers_With_No_SearchQuery_Then_A_List_Of_Users_Should_Be_Returned()
        {
            // Arrange
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<UserDto, User>(pmd);
            var _propertyMappingService = new PropertyMappingService();
            _propertyMappingService.AddPropertyMapping<UserDto, User>(pm);

            var user = _fixture.Create<User>();
            var usersResourceParameters = new UsersResourceParameters {SearchQuery = ""};

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new UserContext(options);
            var usersRepository = new UsersRepository(context, _propertyMappingService);

            context.Add(user);
            context.SaveChanges();
            
            // Act
            var getUsers = usersRepository.GetUsers(usersResourceParameters);

            // Assert
            Assert.True(getUsers.Count == 1);
        }

        [Fact]
        public void Given_A_UserRepository_When_I_Call_GetUsers_With_A_SearchQuery_And_PageSize_And_Fields_Then_A_List_Of_Users_Should_Be_Returned()
        {
            // Arrange
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<UserDto, User>(pmd);
            var _propertyMappingService = new PropertyMappingService();
            _propertyMappingService.AddPropertyMapping<UserDto, User>(pm);

            var user = _fixture.Create<User>();
            var usersResourceParameters = new UsersResourceParameters { SearchQuery = "Name", PageSize = 1, Fields = "Id", PageNumber = 1, OrderBy = "Id"};

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new UserContext(options);
            var usersRepository = new UsersRepository(context, _propertyMappingService);

            context.Add(user);
            context.SaveChanges();
            
            // Act
            var getUsers = usersRepository.GetUsers(usersResourceParameters);
            
            // Assert
            Assert.True(getUsers.Count == 1);
        }

        [Fact]
        public void Given_A_UserRepository_When_I_Call_GetUser_Passing_A_userId_Then_A_User_Should_Be_Returned()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);
            var usersRepository = new UsersRepository(context, _propertyMappingService);
            context.Add(user);
            context.SaveChanges();
            // Act

            var xd = usersRepository.GetUser(user.Id);
            
            // Assert
            Assert.True(xd.Id == user.Id);
        }

        [Fact]
        public void Given_A_UserRepository_When_I_Call_UpdateUser_Passing_A_userId_Then_An_Updated_User_Should_Be_Returned()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);
            var usersRepository = new UsersRepository(context, _propertyMappingService);
            user.Id = new Guid("76053df4-6687-4353-8937-b45556748abe");
            context.Add(user);
            context.SaveChanges();

            user.Id = new Guid("89053df4-6687-4353-8937-b45556748abe");

            // Act
            usersRepository.UpdateUser(user);

            // Assert
            Assert.Contains(user.Id.ToString(), "89053df4-6687-4353-8937-b45556748abe");
        }

        [Fact]
        public void Given_A_UserRepository_When_I_Call_UserExists_Passing_A_userId_Then_True_Should_Be_Returned_As_It_Exists()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new UserContext(options);
            var usersRepository = new UsersRepository(context, _propertyMappingService);
            user.Id = new Guid("76053df4-6687-4353-8937-b45556748abe");
            context.Add(user);
            context.SaveChanges();


            // Act
            bool userExists = usersRepository.UserExists(user.Id);

            // Assert
            Assert.True(userExists);
        }
    }
}

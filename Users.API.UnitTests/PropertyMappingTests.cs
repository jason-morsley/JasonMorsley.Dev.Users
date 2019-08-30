using System;
using System.Collections.Generic;
using Users.API.Entities;
using Users.API.Models;
using Users.API.Services;
using Users.API.UnitTests.Models;
using Xunit;

namespace Users.API.UnitTests
{
    public class PropertyMappingTests
    {
        [Fact]
        //Given_?_When_?_Then_?
        public void Given_A_Valid_Mapping_When_ValidMappingExistsFor_Is_Called_Then_Return_True()
        {
            // Arrange
            // Test setup
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<Cat, Dog>(pmd);
            var pms = new PropertyMappingService();
            pms.AddPropertyMapping<Cat, Dog>(pm);

            // Act
            // Do the thing you want to test
            var result = pms.ValidMappingExistsFor<Cat, Dog>("Id");

            // Assert
            //Did the thing you did actually work
            Assert.True(result);
        }

        [Fact]
        //Given_?_When_?_Then_?
        public void Given_A_Non_Valid_Mapping_When_ValidMappingExistsFor_Is_Called_Then_Return_False()
        {
            // Arrange
            // Test setup
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<Cat, Dog>(pmd);
            var pms = new PropertyMappingService();
            pms.AddPropertyMapping<Cat, Dog>(pm);


            // Act
            // Do the thing you want to test
            var result = pms.ValidMappingExistsFor<Cat, Dog>("non valid mapping");

            // Assert
            //Did the thing you did actually work
            Assert.False(result);
        }

        [Fact]
        //Given_?_When_?_Then_?
        public void Given_NullOrWhiteSpace_Mapping_When_ValidMappingExistsFor_Is_Called_Then_Return_False()
        {
            var pmd = new Dictionary<string, PropertyMappingValue>
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})}
            };
            var pm = new PropertyMapping<Cat, Dog>(pmd);
            var pms = new PropertyMappingService();
            pms.AddPropertyMapping<Cat, Dog>(pm);


            var result = pms.ValidMappingExistsFor<Cat, Dog>("");


            Assert.True(result);
        }

        [Fact]
        //Given_?_When_?_Then_?
        public void Given_Invalid_PropertyMapping_When_GetPropertyMapping_Is_Called_Then_Throws_Exception()
        {
            var pms = new PropertyMappingService();
            
            var exception = Assert.Throws<System.Exception>((() => pms.GetPropertyMapping<Cat, Cat>()));
            
            Assert.Equal("Cannot find exact property mapping instance for " +
                         "Users.API.UnitTests.Models.Cat,Users.API.UnitTests.Models.Cat",
                    exception.Message);
        }
    }
}

using FluentValidation.Validators;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Shop.API.Controllers;
using Shop.Data;
using Shop.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Shop.API.Tests
{
    public sealed class CustomerValidatorTests
    {
        private readonly CustomerValidator _sut = new CustomerValidator();

        [Fact]
        public void ForenameRule_ShouldMatchEFModelConfiguration()
        {
            var validator = new CustomerValidator();

            // Get the rules for the Forename field in the CustomerValidator
            var foreNameLengthValidator = validator
                .GetValidatorsForMember(t => t.Forename).OfType<MaximumLengthValidator>().First();

            var foreNameNotEmptyValidator = validator
                .GetValidatorsForMember(t => t.Forename).OfType<NotEmptyValidator>().FirstOrDefault();

            // Get the EF EntityTypeBuilder<T> for our Customer entity
            var entityTypeBuilder = GetCustomerEntityConfigurationMetadata();

            var foreNameDbProperty = entityTypeBuilder.Metadata.FindDeclaredProperty(nameof(Customer.Forename));

            // Rule Should have the same length as EF Configuration
            Assert.Equal(foreNameDbProperty.GetMaxLength(), foreNameLengthValidator.Max);

            // If the Column is required (NOTNULL) in the EF configuration, the rule should exist
            if (!foreNameDbProperty.IsColumnNullable())
                Assert.NotNull(foreNameNotEmptyValidator);
            else
                Assert.Null(foreNameNotEmptyValidator);
        }

        [Fact]
        public void Validator_MaxLengthRules_ShouldHaveSameLengthAsEfEntity()
        {
            var propertiesToValidate = new string[]
            {
                nameof(Customer.Surname),
                nameof(Customer.Forename),
                nameof(Customer.Address),
            };

            var entityBuilder = TestExtensions
                .GetEntityTypeBuilder<Customer, CustomerEntityTypeConfiguration>();

            // Get the validators for the fields above
            Dictionary<string, LengthValidator> validatorsDict = propertiesToValidate
                .Select(p => new { Key = p, Validator = _sut.GetValidatorsForMember(p).OfType<LengthValidator>().First() })
                .ToDictionary(key => key.Key, value => value.Validator);

            // Get the database metadata for each field as configured in EF Core
            Dictionary<string, IMutableProperty> expectedDbProperties = propertiesToValidate
                .Select(p => new { Key = p, FieldMetadata = entityBuilder.Metadata.FindDeclaredProperty(p) })
                .ToDictionary(key => key.Key, value => value.FieldMetadata);

            foreach (var propValidator in validatorsDict)
            {
                // grab the db metadata by the field name
                var expectedDbMetadata = expectedDbProperties[propValidator.Key];

                // Validator Length and Db should have the same values
                Assert.Equal(expectedDbMetadata.GetMaxLength(), propValidator.Value.Max);
            }
        }

        private EntityTypeBuilder<Customer> GetCustomerEntityConfigurationMetadata()
        {
            // Construct the optionsBuilder using InMemory SqlLite
            var options = new DbContextOptionsBuilder<ShopDbContext>()
                    .UseSqlite(new SqliteConnection("DataSource=:memory:"))
                    .Options;

            var sut = new ShopDbContext(options);

            // Get the convention set for this db
            var conventionSet = ConventionSet.CreateConventionSet(sut);

            // Now create the ModelBuilder
            var modelBuilder = new ModelBuilder(conventionSet);

            // Get the EntityTypeBuilder for Customer
            var entityTypeBuilder = modelBuilder.Entity<Customer>();

            // Apply the EntityConfiguration to our entityTypeBuilder
            var customerEntityConfiguration = new CustomerEntityTypeConfiguration();
            customerEntityConfiguration.Configure(entityTypeBuilder);

            return entityTypeBuilder;
        }
    }
}

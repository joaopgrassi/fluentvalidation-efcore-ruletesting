﻿using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
        private readonly CustomerValidator _sut = new();

        /// <summary>
        /// This test does the initial approach I showed in the post. 
        /// We need to get each field "manually". See the test below for a better
        /// way to handle same rules for different properties
        /// </summary>
        [Fact]
        public void ForenameRule_ShouldMatchEFModelConfiguration()
        {
            // Get the rules for the Forename field in the CustomerValidator
            var foreNameLengthValidator = _sut
                .GetValidatorsForMember(t => t.Forename).OfType<IMaximumLengthValidator>().First();

            var foreNameNotEmptyValidator = _sut
                .GetValidatorsForMember(t => t.Forename).OfType<INotEmptyValidator>().FirstOrDefault();

            // Get the EF EntityTypeBuilder<T> for our Customer entity
            var entityTypeBuilder = TestExtensions
                .GetEntityTypeBuilder<Customer, CustomerEntityTypeConfiguration>();

            var foreNameDbProperty = entityTypeBuilder.Metadata.FindDeclaredProperty(nameof(Customer.Forename));

            // Rule Should have the same length as EF Configuration
            Assert.Equal(foreNameDbProperty!.GetMaxLength(), foreNameLengthValidator.Max);

            // If the Column is required (NOTNULL) in the EF configuration, the rule should exist
            if (!foreNameDbProperty.IsColumnNullable())
                Assert.NotNull(foreNameNotEmptyValidator);
            else
                Assert.Null(foreNameNotEmptyValidator);
        }

        [Fact]
        public void Validator_MaxLengthRules_ShouldHaveSameLengthAsEfEntity()
        {
            // We want to test all length of these fields
            var propertiesToValidate = new[]
            {
                nameof(Customer.Surname),
                nameof(Customer.Forename),
                nameof(Customer.Address),
            };

            var entityTypeBuilder = TestExtensions
                .GetEntityTypeBuilder<Customer, CustomerEntityTypeConfiguration>();

            // Get the validators for the fields above
            Dictionary<string, ILengthValidator> validatorsDict = propertiesToValidate
                .Select(p => new { Key = p, Validator = _sut.GetValidatorsForMember(p).OfType<ILengthValidator>().First() })
                .ToDictionary(key => key.Key, value => value.Validator);

            // Get the database metadata for each field as configured in EF Core
            Dictionary<string, IMutableProperty> expectedDbProperties = propertiesToValidate
                .Select(p => new { Key = p, FieldMetadata = entityTypeBuilder.Metadata.FindDeclaredProperty(p) })
                .ToDictionary(key => key.Key, value => value.FieldMetadata);

            foreach (var propValidator in validatorsDict)
            {
                // grab the db metadata by the field name
                var expectedDbMetadata = expectedDbProperties[propValidator.Key];

                // Validator Length and Db should have the same values
                Assert.Equal(expectedDbMetadata.GetMaxLength(), propValidator.Value.Max);
            }
        }
    }
}

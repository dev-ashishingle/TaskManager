using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using TaskManager.Application.Validators;
using TaskManager.Tests.Common;

namespace TaskManager.Tests.Services
{
    public class CreateTaskRequestValidatorTests
    {
        private readonly CreateTaskRequestValidator _validator = new();

        [Fact]
        public async Task Validate_WithValidRequest_PassesValidation()
        {
            var request = TestDataBuilder.BuildCreateTaskRequest();
            var result = await _validator.ValidateAsync(request);
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Validate_WithEmptyTitle_FailsValidation(string title)
        {
            var request = TestDataBuilder.BuildCreateTaskRequest() with { Title = title };
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public async Task Validate_WithTitleExceeding200Chars_FailsValidation()
        {
            var request = TestDataBuilder.BuildCreateTaskRequest()
                with
            { Title = new string('A', 201) };
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Theory]
        [InlineData("Low")]
        [InlineData("Medium")]
        [InlineData("High")]
        [InlineData("Critical")]
        [InlineData("low")]      // case insensitive
        [InlineData("HIGH")]
        public async Task Validate_WithValidPriority_PassesValidation(string priority)
        {
            var request = TestDataBuilder.BuildCreateTaskRequest() with { Priority = priority };
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WithInvalidPriority_FailsValidation()
        {
            var request = TestDataBuilder.BuildCreateTaskRequest() with { Priority = "Urgent" };
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Priority");
        }

        [Fact]
        public async Task Validate_WithPastDueDate_FailsValidation()
        {
            var request = TestDataBuilder.BuildCreateTaskRequest()
                with
            { DueDate = DateTime.UtcNow.AddDays(-1) };
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "DueDate");
        }

        [Fact]
        public async Task Validate_WithNullDueDate_PassesValidation()
        {
            // DueDate is optional — null should be fine
            var request = TestDataBuilder.BuildCreateTaskRequest() with { DueDate = null };
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
    }
}

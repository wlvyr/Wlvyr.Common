/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using Xunit;

namespace Wlvyr.Common.Tests;

public class ResultTests
{
    [Fact]
    public void DefaultConstructor_ShouldBeSuccessful()
    {
        var result = new Result();

        Assert.True(result.IsSuccessful);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Constructor_WithIsSuccessfulFalse_ShouldBeUnsuccessful()
    {
        var result = new Result(false);

        Assert.False(result.IsSuccessful);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Constructor_WithError_ShouldBeUnsuccessfulAndHaveError()
    {
        var expectedError = "Error occurred";

        var result = new Result(expectedError);

        Assert.False(result.IsSuccessful);
        Assert.Single(result.Errors);
        Assert.Equal(expectedError, result.Errors[0]);
        Assert.Equal(expectedError, result.Error);
    }

    [Fact]
    public void Constructor_WithErrorList_ShouldHaveAllErrors()
    {
        var expectedError = new List<string> { "Error1", "Error2" };
        var result = new Result(expectedError);

        Assert.False(result.IsSuccessful);

        // List of errors
        Assert.Equal(expectedError, result.Errors);

        // Concatenated error.
        Assert.Equal("Error1, Error2", result.Error);
    }

    [Fact]
    public void IsSuccessful_ShouldBeFalse_IfErrorsExist_EvenIfFlagTrue()
    {
        var result = new Result(true);
        result.Errors.Add("Error");

        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void StaticSuccessful_ShouldReturnSuccessfulResult()
    {
        var result = Result.Successful();

        Assert.True(result.IsSuccessful);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void StaticFailed_ShouldReturnUnsuccessfulResult()
    {
        var result = Result.Failed();

        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void StaticFailedWithError_ShouldHaveError()
    {
        var expectedError = "Failure";
        var result = Result.Failed(expectedError);

        Assert.False(result.IsSuccessful);
        Assert.Single(result.Errors);
        Assert.Equal(expectedError, result.Error);
    }

    [Fact]
    public void StaticFailedWithErrorList_ShouldHaveErrors()
    {
        var errors = new List<string> { "Err1", "Err2" };
        var result = Result.Failed(errors);

        Assert.False(result.IsSuccessful);
        Assert.Equal(errors, result.Errors);
        Assert.Equal("Err1, Err2", result.Error);
    }
}

public class ResultOfTTests
{
    [Fact]
    public void DefaultConstructor_ShouldBeSuccessful()
    {
        var result = new Result<int>();

        Assert.True(result.IsSuccessful);
        Assert.Empty(result.Errors);
        Assert.Equal(default(int), result.Data);

        var null_result = new Result<int?>();
        Assert.Null(null_result.Data);
    }

    [Fact]
    public void Constructor_WithData_ShouldSetData()
    {
        var expectedData = 42;
        var result = new Result<int>(expectedData);

        Assert.True(result.IsSuccessful);
        Assert.Equal(expectedData, result.Data);
    }

    [Fact]
    public void Constructor_WithIsSuccessfulFalse_ShouldBeUnsuccessful()
    {
        var result = new Result<int>(false);

        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void Constructor_WithErrors_ShouldBeUnsuccessful()
    {
        var expectedErrors = new List<string> { "Error" };

        var result = new Result<int>(expectedErrors);
        Assert.False(result.IsSuccessful);
        Assert.Equal(expectedErrors, result.Errors);
    }

    [Fact]
    public void StaticSuccessful_ShouldReturnSuccessfulResult()
    {
        var result = Result<int>.Successful();

        Assert.True(result.IsSuccessful);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void StaticSuccessfulWithData_ShouldSetData()
    {
        var expectedData = 100;
        var result = Result<int>.Successful(expectedData);

        Assert.True(result.IsSuccessful);
        Assert.Equal(expectedData, result.Data);
    }

    [Fact]
    public void StaticFailed_ShouldReturnUnsuccessfulResult()
    {
        var result = Result<int>.Failed();

        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void StaticFailedWithError_ShouldHaveError()
    {
        var expectedError = "Failure";
        var result = Result<int>.Failed(expectedError);

        Assert.False(result.IsSuccessful);
        Assert.Single(result.Errors);
        Assert.Equal(expectedError, result.Error);
    }

    [Fact]
    public void StaticFailedWithErrorList_ShouldHaveErrors()
    {
        var expectedErrors = new List<string> { "Err1", "Err2" };

        var result = Result<int>.Failed(expectedErrors);

        Assert.False(result.IsSuccessful);
        Assert.Equal(expectedErrors, result.Errors);
    }
}
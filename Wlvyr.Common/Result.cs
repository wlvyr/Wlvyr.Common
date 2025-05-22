/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Wlvyr.Common;

public class Result
{
    protected bool _isSuccessful = true;

    public Result() { }

    public Result(bool isSuccessful)
    {
        _isSuccessful = isSuccessful;
    }

    public Result(string error)
    {
        this.Errors.Add(error);
    }

    public Result(List<string> errors)
    {
        this.Errors = errors;
    }

    public List<string> Errors { get; protected set; } = new List<string>();

    public bool IsSuccessful => _isSuccessful && !this.Errors.Any();

    public string Error
    {
        get
        {
            return this.GetError();
        }
    }

    public string GetError()
    {
        return string.Join(", ", this.Errors);
    }

    public static Result Successful()
    {
        return new Result();
    }

    public static Result Failed()
    {
        var r = new Result(isSuccessful: false);
        return r;
    }

    public static Result Failed(string error)
    {
        return new Result(error);
    }

    public static Result Failed(List<string> errors)
    {
        return new Result(errors);
    }
}

public class Result<T> : Result
{
    public Result() { }

    public Result(T? data)
    {
        this.Data = data;
    }

    public Result(bool isSuccessful)
    {
        _isSuccessful = isSuccessful;
    }

    public Result(List<string> errors)
    {
        this.Errors = errors;
    }

    public Result(string error) : base(error) { }

    public T? Data { get; init; }

    public new static Result<T> Successful()
    {
        return new Result<T>();
    }

    public static Result<T> Successful(T data)
    {
        return new Result<T>(data);
    }

    public new static Result<T> Failed()
    {
        var r = new Result<T>(isSuccessful: false);
        return r;
    }

    public new static Result<T> Failed(string error)
    {
        return new Result<T>(error);
    }

    public new static Result<T> Failed(List<string> errors)
    {
        return new Result<T>(errors);
    }
}
using System;
using System.ComponentModel.DataAnnotations;

public class Timestamp_Is_Today_Or_Later : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is ulong timestamp)
        {
            ulong currentUnix = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return timestamp >= currentUnix;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a valid UNIX timestamp from today or later.";
    }
}

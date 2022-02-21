using System.Collections.Generic;
using System.Linq;
using EasyTalker.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace EasyTalker.Core;

public static class IdentityErrors
{
    private static readonly Dictionary<string, string> ErrorsDesc = new()
    {
        ["DefaultError"] = "Default Error",
        ["ConcurrencyFailure"] = "Concurrency Failure",
        ["PasswordMismatch"] = "Current password is incorrect",
        ["InvalidToken"] = "Invalid Token",
        ["LoginAlreadyAssociated"] = "Login Already Associated",
        ["InvalidUserName"] = "Invalid User Name",
        ["InvalidEmail"] = "Invalid Email",
        ["DuplicateUserName"] = "Duplicate User Name",
        ["DuplicateEmail"] = "Duplicate Email",
        ["InvalidRoleName"] = "Invalid Role Name",
        ["DuplicateRoleName"] = "Duplicate Role Name",
        ["UserAlreadyHasPassword"] = "User Already Has Password",
        ["UserLockoutNotEnabled"] = "User Lockout Not Enabled",
        ["UserAlreadyInRole"] = "User Already In Role",
        ["UserNotInRole"] = "User Not In Role",
        ["PasswordTooShort"] = "Password is too short",
        ["PasswordRequiresNonAlphanumeric"] = "Password requires non alphanumeric",
        ["PasswordRequiresDigit"] = "Password requires digit",
        ["PasswordRequiresLower"] = "Password requires lower",
        ["PasswordRequiresUpper"] = "Password requires upper"
    };

    public static string[] GetErrors(IdentityResult identityResult)
    {
        if (identityResult.Succeeded || identityResult.Errors == null)
            return null;

        var errors = ErrorsDesc
            .GetSame(identityResult.Errors, x => x.Key, x => x.Code)
            .Select(x => x.Value)
            .ToArray();
        
        if (errors.Length != identityResult.Errors.Count())
        {
            var missingDesc = identityResult.Errors
                .GetUniques(errors, x => x.Code, x => x)
                .Select(x => x.Code)
                .ToArray();
            
            Log.Error("{0} | Missing error desc={1}", 
                nameof(IdentityErrors), 
                string.Join(", ", missingDesc));
        }

        return errors;
    }
}
using EasyTalker.Authentication.Database.Entities;
using EasyTalker.Core;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.User;
using EasyTalker.Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalker.Authentication.Database.Store;

public class UserStore : IUserStore
{
    private readonly UserManager<UserDb> _userManager;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UserStore(UserManager<UserDb> userManager, IServiceScopeFactory serviceScopeFactory)
    {
        _userManager = userManager;
        _serviceScopeFactory = serviceScopeFactory;
    }
        
    public async Task<UserDto> RegisterUser(string username, string email, string password)
    {
        if (await _userManager.Users.AnyAsync(x => x.UserName == username))
            throw new Exception($"User {username} already exists.");
        
        if (await _userManager.Users.AnyAsync(x => x.Email == email))
            throw new Exception($"Email {email} already exists.");

        var passwordValidator = _userManager.PasswordValidators.SingleOrDefault()
                                ?? throw new Exception("Missing password validator");

        var userDb = new UserDb
        {
            IsActive = true,
            UserName = username,
            Email = email
        };

        await ValidatePassword(userDb, password);
        
        var result = await _userManager.CreateAsync(userDb, password);
        if (!result.Succeeded)
            throw new Exception(string.Join(';', result.Errors.Select(x => x.Description)));

        return userDb.ToUserDto();
    }

    public async Task<UserDto> GetById(string userId)
    {
        var userDb = await _userManager.FindByIdAsync(userId)
                     ?? throw new Exception($"User with id {userId} not found");

        return userDb.ToUserDto();
    }

    public async Task<UserDto[]> GetAll()
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerAuthenticationContext>();

        return await dbContext.Users
            .Select(x => x.ToUserDto())
            .ToArrayAsync();
    }

    public async Task UpdateUserConnectionStatus(string userId, bool isOnline)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerAuthenticationContext>();
        
        var user = await dbContext.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsOnline = isOnline;
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task SetAllUsersAsOffline()
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerAuthenticationContext>();
        
        var users = await dbContext.Users.ToListAsync();
        users.ForEach(u => u.IsOnline = false);
        await dbContext.SaveChangesAsync();
    }

    public async Task<string[]> ChangePassword(string userId, string currentPassword, string newPassword)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerAuthenticationContext>();

        var user = await _userManager.FindByIdAsync(userId)
                   ?? throw new Exception($"User with id {userId} not found");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return IdentityErrors.GetErrors(result);
    }

    private async Task ValidatePassword(UserDb userDb, string password)
    {
        var passwordValidator = _userManager.PasswordValidators.SingleOrDefault()
                                ?? throw new Exception("Missing password validator");
        
        var validatorResult = await passwordValidator.ValidateAsync(_userManager, userDb, password);
        if (validatorResult.Succeeded)
            return;
            
        var errors = IdentityErrors.GetErrors(validatorResult);
        throw new PasswordValidatorException(errors);
    }
}
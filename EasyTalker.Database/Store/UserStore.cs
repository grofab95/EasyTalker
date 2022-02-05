using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EasyTalker.Core;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.User;
using EasyTalker.Core.Exceptions;
using EasyTalker.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalker.Database.Store;

public class UserStore : IUserStore
{
    private readonly UserManager<UserDb> _userManager;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMapper _mapper;

    public UserStore(UserManager<UserDb> userManager, IServiceScopeFactory serviceScopeFactory, IMapper mapper)
    {
        _userManager = userManager;
        _serviceScopeFactory = serviceScopeFactory;
        _mapper = mapper;
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

        return _mapper.Map<UserDto>(userDb);
    }

    public async Task<UserDto> GetById(string userId)
    {
        var userDb = await _userManager.FindByIdAsync(userId)
                     ?? throw new Exception($"User with id {userId} not found");

        return _mapper.Map<UserDto>(userDb);
    }

    public async Task<UserDto[]> GetAll()
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        return await dbContext.Users
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToArrayAsync();
    }

    public async Task UpdateUserConnectionStatus(string userId, bool isOnline)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
        
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
            .GetRequiredService<EasyTalkerContext>();
        
        var users = await dbContext.Users.ToListAsync();
        users.ForEach(u => u.IsOnline = false);
        await dbContext.SaveChangesAsync();
    }

    public async Task<string[]> ChangePassword(string userId, string currentPassword, string newPassword)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

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
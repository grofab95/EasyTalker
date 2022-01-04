using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.User;
using EasyTalker.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyTalker.Database.Store;

public class UserStore : IUserStore
{
    private readonly UserManager<UserDb> _userManager;
    private readonly IMapper _mapper;

    public UserStore(UserManager<UserDb> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }
        
    public async Task<UserDto> RegisterUser(string username, string email, string password)
    {
        if (await _userManager.Users.AnyAsync(x => x.UserName == username))
            throw new Exception($"User {username} already exists.");
        
        if (await _userManager.Users.AnyAsync(x => x.Email == email))
            throw new Exception($"Email {email} already exists.");

        var userDb = new UserDb
        {
            IsActive = true,
            UserName = username,
            Email = email
        };

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
}
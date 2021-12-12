using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyTalker.Core.Adapters;
using EasyTalker.Database.Entities;
using EasyTalker.Infrastructure.Dto.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyTalker.Database.Dao;

public class UserDao : IUserDao
{
    private readonly UserManager<UserDb> _userManager;
    private readonly IMapper _mapper;

    public UserDao(UserManager<UserDb> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }
        
    public async Task<UserDto> RegisterUser(string username, string password)
    {
        if (await _userManager.Users.AnyAsync(x => x.UserName == username))
            throw new Exception($"User {username} already exists.");

        var userDb = new UserDb
        {
            IsActive = true,
            UserName = username
        };

        var result = await _userManager.CreateAsync(userDb, password);
        if (!result.Succeeded)
            throw new Exception(string.Join(';', result.Errors.Select(x => x.Description)));

        return _mapper.Map<UserDto>(userDb);
    }
}
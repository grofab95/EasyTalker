using System;
using System.Threading.Tasks;
using EasyTalker.Api.Models;
using EasyTalker.Api.Requests;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalker.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserStore _userStore;

    public UsersController(IUserStore userStore)
    {
        _userStore = userStore;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ApiResponse<UserDto>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return ApiResponse<UserDto>.Failure(string.Empty);

        try
        {
            var user = await _userStore.RegisterUser(request.Username, request.Email, request.Password);
            
            return ApiResponse<UserDto>.Success(user);
        }
        catch (Exception ex)
        {
            return ApiResponse<UserDto>.Failure(ex.Message);
        }
    }
    
    [HttpGet("{userId}")]
    public async Task<ApiResponse<UserDto>> GetUser([FromRoute] string userId)
    {
        try
        {
            var result = await _userStore.GetById(userId);
            return ApiResponse<UserDto>.Success(result);
        }
        catch (Exception ex)
        {
            return ApiResponse<UserDto>.Failure(ex.Message);
        }
    }
}
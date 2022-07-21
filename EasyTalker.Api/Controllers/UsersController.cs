using System;
using System.Linq;
using System.Threading.Tasks;
using Easy.MessageHub;
using EasyTalker.Api.Models;
using EasyTalker.Api.Requests;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.User;
using EasyTalker.Core.Events;
using EasyTalker.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalker.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserStore _userStore;
    private readonly IMessageHub _messageHub;

    public UsersController(IUserStore userStore, IMessageHub messageHub)
    {
        _userStore = userStore;
        _messageHub = messageHub;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ApiResponse<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return ApiResponse<RegisterResponse>.Failure(string.Empty);

        try
        {
            var user = await _userStore.RegisterUser(request.Username, request.Email, request.Password);
            _messageHub.Publish(new UserRegistered(user));

            return ApiResponse<RegisterResponse>.Success(new RegisterResponse(user));
        }
        catch (PasswordValidatorException ex)
        {
            return ApiResponse<RegisterResponse>.Failure(ex.Errors);
        }
        catch (Exception ex)
        {
            return ApiResponse<RegisterResponse>.Failure(ex);
        }
    }
    
    [HttpGet("{userId}")]
    public async Task<ApiResponse<UserDto>> GetUser([FromRoute] string userId)
    {
        try
        {
            var user = await _userStore.GetById(userId);
            return ApiResponse<UserDto>.Success(user);
        }
        catch (Exception ex)
        {
            return ApiResponse<UserDto>.Failure(ex);
        }
    }
    
    [HttpGet]
    public async Task<ApiResponse<UserDto[]>> GetAll()
    {
        try
        {
            var users = await _userStore.GetAll();
            return ApiResponse<UserDto[]>.Success(users);
        }
        catch (Exception ex)
        {
            return ApiResponse<UserDto[]>.Failure(ex);
        }
    }

    [HttpPost]
    [Route("{userId}/change-password")]
    public async Task<ApiResponse> ChangePassword([FromRoute] string userId, 
        [FromBody] ChangePasswordRequest request)
    {
        try
        {
            var errors = await _userStore.ChangePassword(userId, request.CurrentPassword, request.NewPassword);
            if (errors?.Any() ?? false)
            {
                return ApiResponse.Failure(errors);
            }
            
            return ApiResponse.Success();
        }
        catch (Exception ex)
        {
            return ApiResponse.Failure(ex);
        }
    }
}
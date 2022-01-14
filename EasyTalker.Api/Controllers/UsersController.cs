using System;
using System.Threading.Tasks;
using Easy.MessageHub;
using EasyTalker.Api.Models;
using EasyTalker.Api.Requests;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Dto.User;
using EasyTalker.Core.Events;
using EasyTalker.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public async Task<ApiResponse<UserDto>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return ApiResponse<UserDto>.Failure(string.Empty);

        try
        {
            var user = await _userStore.RegisterUser(request.Username, request.Email, request.Password);
            
            // _messageHub.Publish(new MessageChanged(new MessageDto
            // {
            //     Id = 55,
            //     Sender = user,
            //     Text = "From register",
            //     CreatedAt = DateTime.Now
            // }));
            
            _messageHub.Publish(new UserRegistered(user));
            
            return ApiResponse<UserDto>.Success(user);
        }
        catch (Exception ex)
        {
            return ApiResponse<UserDto>.Failure(ex);
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
}
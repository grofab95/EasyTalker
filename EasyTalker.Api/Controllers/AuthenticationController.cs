using System;
using System.Threading.Tasks;
using EasyTalker.Api.Authentication.Services;
using EasyTalker.Api.Dto;
using EasyTalker.Api.Models;
using EasyTalker.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
        
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ApiResponse<AuthenticationResultDto>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return ApiResponse<AuthenticationResultDto>.Failure(string.Empty);

        try
        {
            var result = await _authenticationService.Authenticate(request.Username, request.Password, GetRequestIpAddress());
            
            return ApiResponse<AuthenticationResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthenticationResultDto>.Failure(ex.Message);
        }
    }
    
    [HttpPost("refresh-token")]
    public async Task<ApiResponse<AuthenticationResultDto>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _authenticationService.RefreshToken(request.RefreshToken);
            
            return ApiResponse<AuthenticationResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthenticationResultDto>.Failure(ex.Message);
        }
    }
    
    [HttpPost("revoke-token")]
    public async Task<ApiResponse> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        try
        {
            await _authenticationService.RevokeToken(request.RefreshToken);
            
            return ApiResponse.Success();
        }
        catch (Exception ex)
        {
            return ApiResponse.Failure(ex.Message);
        }
    }
    
    private string GetRequestIpAddress()
    {
        return Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"]
            : HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4().ToString();
    }
}
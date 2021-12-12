using System;
using System.Threading.Tasks;
using EasyTalker.Api.Authentication.Services;
using EasyTalker.Api.Dto;
using EasyTalker.Api.Models;
using EasyTalker.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalker.Api.Controllers;

public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
        
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ApiResponse<AuthenticationResultDto>> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
            return ApiResponse<AuthenticationResultDto>.Failure(string.Empty);

        try
        {
            var result = await _authenticationService.Authenticate(request.Username, request.Password, "ip");
            
            return ApiResponse<AuthenticationResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthenticationResultDto>.Failure(ex.Message);
        }
    }
}
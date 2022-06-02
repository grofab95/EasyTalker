using System;
using System.Threading.Tasks;
using EasyTalker.Api.Models;
using EasyTalker.Api.Requests;
using EasyTalker.Authentication.Services;
using EasyTalker.Core.Dto.Authentications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalker.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("login")]
    public async Task<ApiResponse<AuthenticationResultDto>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return ApiResponse<AuthenticationResultDto>.Failure(string.Empty);

        try
        {
            var result = await _authenticationService.Authenticate(request.Username, request.Password);
            
            return ApiResponse<AuthenticationResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthenticationResultDto>.Failure(ex.Message);
        }
    }
}
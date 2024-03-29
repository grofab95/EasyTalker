﻿using EasyTalker.Core.Dto;
using EasyTalker.Database.Entities;

namespace EasyTalker.Authentication.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResultDto> Authenticate(string username, string password, string ipAddress);
    Task<AuthenticationResultDto> RefreshToken(string token);
    Task RevokeToken(string token);
    Task<string> GetAccessToken(UserDb user);
    RefreshTokenDb GetRefreshToken();
}
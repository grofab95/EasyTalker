using System;
using System.Threading.Tasks;
using EasyTalker.Api.Models;
using EasyTalker.Api.Requests;
using EasyTalker.Core.Adapters;
using EasyTalker.Infrastructure.Dto.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalker.Api.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IUserDao _userDao;

        public UserController(IUserDao userDao)
        {
            _userDao = userDao;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ApiResponse<UserDto>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return ApiResponse<UserDto>.Failure(string.Empty);

            try
            {
                var user = await _userDao.RegisterUser(request.Username, request.Password);
            
                return ApiResponse<UserDto>.Success(user);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.Failure(ex.Message);
            }
        }
    }
}
﻿using AssetManagement.Application.Models.Requests;
using AssetManagement.Application.Services;
using AssetManagement.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AssetManagement.WebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterRequest request)
        {
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new GeneralCreateResponse
                    {
                        Success = false,
                        Message = "Invalid request data.",
                        Data = ModelState
                    });
                }

                try
                {
                    var userResponse = await _userService.AddUserAsync(request);

                    return Ok(new GeneralCreateResponse
                    {
                        Success = true,
                        Message = "User registered successfully.",
                        Data = userResponse
                    });
                }
                catch (ArgumentException ex)
                {
                    return Conflict(new GeneralCreateResponse
                    {
                        Success = false,
                        Message = ex.Message
                    });
                }
                catch (InvalidOperationException ex)
                {
                    return Conflict(new GeneralBoolResponse
                    {
                        Success = false,
                        Message = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new GeneralBoolResponse
                    {
                        Success = false,
                        Message = "An error occurred while registering the user.",
                    });
                }

            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetFilteredUsers(
            [FromQuery] string location,
            [FromQuery] string? searchTerm,
            [FromQuery] string? role,
            [FromQuery] string sortBy = "StaffCode",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 15)
        {
            try
            {
                var users = await _userService.GetFilteredUsersAsync(location, searchTerm, role, sortBy, sortDirection, pageNumber, pageSize);
                return Ok(new GeneralGetsResponse
                {
                    Success = true,
                    Message = "Successfully.",
                    Data = users.Items,
                    TotalCount = users.TotalCount
                });
            }
            catch (ArgumentException ex)
            {
                return Conflict(new GeneralGetsResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new GeneralGetsResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralGetsResponse
                {
                    Success = false,
                    Message = ex.Message,
                });
            }
        }
    }
}
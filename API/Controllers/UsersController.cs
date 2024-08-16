﻿using System.Security.Claims;
using API.Data;
using API.DTO;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Authorize]
public class UsersController(IUserRepository userRepository,IMapper mapper) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers(){
        var users=await userRepository.GetMembersAsync();
        return Ok(users);
    }

    [HttpGet("{username}")]     // /api/users/2
    public async Task<ActionResult<MemberDTO>> GetUser(string username){
        var user= await userRepository.GetMemberAsync(username);
        if(user==null) return NotFound();  
        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO){
        var username=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(username==null) return BadRequest("No Username found");
        var user=await userRepository.GetUserByusernameAsync(username);
        if(user==null) return BadRequest("Could not find user");
        mapper.Map(memberUpdateDTO, user);
        if(await userRepository.SaveAllAsync()) return NoContent();
        return BadRequest("Failed to update");
    }

}

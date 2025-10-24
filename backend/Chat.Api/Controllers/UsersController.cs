using Chat.Api.Contracts;
using Chat.Api.Data;
using Chat.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(AppDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<User>> Create(CreateUserRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Nickname))
            return BadRequest("Nickname is required");

        var exists = await db.Users.AnyAsync(u => u.Nickname == req.Nickname);
        if (exists) return Conflict("Nickname is taken");

        var user = new User { Nickname = req.Nickname };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> Get(int id)
    {
        var user = await db.Users.FindAsync(id);
        return user is null ? NotFound() : Ok(user);
    }
}

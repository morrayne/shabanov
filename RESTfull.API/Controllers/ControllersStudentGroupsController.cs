using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTfull.Infrastructure.Data;

namespace RESTfull.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentGroupsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StudentGroupsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var groups = await _db.StudentGroups
            .Select(g => new
            {
                g.Id,
                g.Name,
                g.Number,
                g.StudyProgramId
            })
            .ToListAsync();

        return Ok(groups);
    }
}

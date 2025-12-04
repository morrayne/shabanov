using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTfull.Infrastructure.Data;

namespace RESTfull.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudyProgramsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StudyProgramsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var programs = await _db.StudyPrograms
            .Select(p => new
            {
                p.Id,
                p.Institution,
                p.Faculty,
                p.Direction,
                p.Speciality
            })
            .ToListAsync();

        return Ok(programs);
    }
}

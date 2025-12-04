using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTfull.Domain.Entities;
using RESTfull.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RESTfull.API.Models;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(AppDbContext db, ILogger<StudentsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        IQueryable<Student> query = _db.Students
            .Include(s => s.StudyProgram)
            .Include(s => s.StudentGroup);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim();
            if (int.TryParse(q, out var id))
            {
                query = query.Where(s => s.Id == id);
            }
            else
            {
                query = query.Where(s =>
                    (!string.IsNullOrEmpty(s.FirstName) && EF.Functions.Like(s.FirstName, $"%{q}%")) ||
                    (!string.IsNullOrEmpty(s.LastName) && EF.Functions.Like(s.LastName, $"%{q}%")) ||
                    (!string.IsNullOrEmpty(s.MiddleName) && EF.Functions.Like(s.MiddleName, $"%{q}%"))
                );
            }
        }

        var students = await query.Select(s => new
        {
            s.Id,
            s.FirstName,
            s.LastName,
            s.MiddleName,
            s.BirthDate,
            StudyProgram = s.StudyProgram == null ? null : new
            {
                s.StudyProgram.Id,
                s.StudyProgram.Institution,
                s.StudyProgram.Faculty,
                s.StudyProgram.Direction,
                s.StudyProgram.Speciality
            },
            StudentGroup = s.StudentGroup == null ? null : new
            {
                s.StudentGroup.Id,
                s.StudentGroup.Name,
                s.StudentGroup.Number,
                s.StudentGroup.StudyProgramId
            },
        }).ToListAsync();

        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var s = await _db.Students
            .Include(st => st.StudyProgram)
            .Include(st => st.StudentGroup)
            .Where(st => st.Id == id)
            .Select(st => new
            {
                st.Id,
                st.FirstName,
                st.LastName,
                st.MiddleName,
                st.BirthDate,
                StudyProgram = st.StudyProgram == null ? null : new
                {
                    st.StudyProgram.Id,
                    st.StudyProgram.Institution,
                    st.StudyProgram.Faculty,
                    st.StudyProgram.Direction,
                    st.StudyProgram.Speciality
                },
                StudentGroup = st.StudentGroup == null ? null : new
                {
                    st.StudentGroup.Id,
                    st.StudentGroup.Name,
                    st.StudentGroup.Number,
                    st.StudentGroup.StudyProgramId
                },
            })
            .FirstOrDefaultAsync();

        if (s == null) return NotFound();
        return Ok(s);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudentCreateDto dto)
    {
        if (dto == null) return BadRequest("Empty payload");
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            return BadRequest("FirstName and LastName are required");

        // Validate provided FK ids
        int? spId = null;
        if (dto.StudyProgramId.HasValue)
        {
            var spExists = await _db.StudyPrograms.AnyAsync(x => x.Id == dto.StudyProgramId.Value);
            if (!spExists) return BadRequest($"StudyProgram with id {dto.StudyProgramId.Value} does not exist");
            spId = dto.StudyProgramId.Value;
        }

        int? groupId = null;
        if (dto.StudentGroupId.HasValue)
        {
            var gexists = await _db.StudentGroups.AnyAsync(g => g.Id == dto.StudentGroupId.Value);
            if (!gexists) return BadRequest($"StudentGroup with id {dto.StudentGroupId.Value} does not exist.");
            groupId = dto.StudentGroupId.Value;
        }

        try
        {
            var student = new Student
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? string.Empty : dto.MiddleName.Trim(),
                BirthDate = dto.BirthDate,
                StudyProgramId = spId,
                StudentGroupId = groupId
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            var result = await _db.Students
                .Include(s => s.StudyProgram)
                .Include(s => s.StudentGroup)
                .Where(s => s.Id == student.Id)
                .Select(s => new
                {
                    s.Id,
                    s.FirstName,
                    s.LastName,
                    s.MiddleName,
                    s.BirthDate,
                    StudyProgram = s.StudyProgram == null ? null : new
                    {
                        s.StudyProgram.Id,
                        s.StudyProgram.Institution,
                        s.StudyProgram.Faculty,
                        s.StudyProgram.Direction,
                        s.StudyProgram.Speciality
                    },
                    StudentGroup = s.StudentGroup == null ? null : new
                    {
                        s.StudentGroup.Id,
                        s.StudentGroup.Name,
                        s.StudentGroup.Number,
                        s.StudentGroup.StudyProgramId
                    },
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(Get), new { id = student.Id }, result);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database update failed while creating student");
            var detail = GetInnermostExceptionMessage(dbEx);
            return StatusCode(500, new { error = "Database update failed", detail });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create failed");
            var detail = GetInnermostExceptionMessage(ex);
            return StatusCode(500, new { error = "Create failed", detail });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] StudentCreateDto dto)
    {
        if (dto == null) return BadRequest("Empty payload");
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            return BadRequest("FirstName and LastName are required");

        var exists = await _db.Students.FindAsync(id);
        if (exists == null) return NotFound();

        try
        {
            int? groupId = null;
            if (dto.StudentGroupId.HasValue)
            {
                var gexists = await _db.StudentGroups.AnyAsync(g => g.Id == dto.StudentGroupId.Value);
                if (!gexists) return BadRequest($"StudentGroup with id {dto.StudentGroupId.Value} does not exist.");
                groupId = dto.StudentGroupId.Value;
            }

            exists.FirstName = dto.FirstName.Trim();
            exists.LastName = dto.LastName.Trim();
            exists.MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? string.Empty : dto.MiddleName.Trim();
            exists.BirthDate = dto.BirthDate;
            exists.StudentGroupId = groupId;
            exists.StudyProgramId = dto.StudyProgramId;

            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database update failed while updating student");
            var detail = GetInnermostExceptionMessage(dbEx);
            return StatusCode(500, new { error = "Database update failed", detail });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update failed");
            var detail = GetInnermostExceptionMessage(ex);
            return StatusCode(500, new { error = "Update failed", detail });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Students.FindAsync(id);
        if (entity == null) return NotFound();
        _db.Students.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<StudyProgram?> GetOrCreateStudyProgramAsync(StudentCreateDto dto)
    {
        // unused now
        return null;
    }

    private static string GetInnermostExceptionMessage(Exception ex)
    {
        if (ex == null) return string.Empty;
        while (ex.InnerException != null) ex = ex.InnerException;
        return ex.Message;
    }
}


using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using System.Security.Claims; 
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities;
using Domain.Enums;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly ITokenRevocationService _tokenRevocationService;

    private readonly ApplicationContext _context;
    private readonly ILogger<CourseController> _logger;

    public CourseController(
        ApplicationContext context,
        ILogger<CourseController> logger, ITokenRevocationService tokenRevocationService)
    {
        _context = context;
        _logger = logger;
        _tokenRevocationService = tokenRevocationService;

    }

    /// <summary>
    /// Создать курс
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CourseModel>> CreateCourse(
        [FromBody] CourseCreateModel model)

    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
        }

        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (_tokenRevocationService.IsTokenRevoked(token))
        {
            return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // СДЕЛАТЬ ПРОВЕРКУ НА УНИКАЛЬНОСТЬ КОДА
            var teachersCode = GenerateRandomCode();
            var studentsCode = GenerateRandomCode();
            var course = new CourseModel
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Chapter = model.Chapter,
                Subject = model.Subject,
                Audience = model.Audience,
                TeachersCode = teachersCode,
                StudentsCode = studentsCode,
                CreateTime = DateTime.UtcNow
            };

            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return BadRequest(new { message = "Недействительный токен" });
            }

            var userId = Guid.Parse(userIdClaim.Value);

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            
            UserCorse userCorse = new UserCorse
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Role = Role.Owner,
                CourseId = course.Id
            };

            _context.UsersCorses.Add(userCorse);
            await _context.SaveChangesAsync();


            return CreatedAtAction(
                nameof(GetCourse),
                new { id = course.Id },
                course);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании курса");
            return StatusCode(500, "Произошла ошибка при создании курса");
        }
    }


    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<CourseModel>> GetCourse(Guid id)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return BadRequest(new { message = "Недействительный токен" });
        }

        var userId = Guid.Parse(userIdClaim.Value);

         var object1 = _context.UsersCorses
            .Where(uc => uc.UserId == userId && uc.CourseId == id)
            .FirstOrDefault();
            
        if (object1 == null)
        {
            return NotFound();
        }
        var course = await _context.Courses
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound();
        }


        return Ok(course);
    }

    /// <summary>
    /// Узнать роль на курсе
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>

    [HttpGet("{id}/Role")]
    [Authorize]
    public async Task<ActionResult<Role>> GetCorseRole(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);

        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return BadRequest(new { message = "Недействительный токен" });
        }

        var userId = Guid.Parse(userIdClaim.Value);

        var role = _context.UsersCorses
            .Where(uc => uc.UserId == userId && uc.CourseId == id)
            .Select(uc => uc.Role)
            .FirstOrDefault();
            
        if (course == null || role == null)
        {
            return NotFound();
        }


        return Ok(role);
    }

    /// <summary>
    /// Зайти на курс через код
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>

    [HttpPost("register")]
    [Authorize]
    public async Task<IActionResult> JoinCourseByCode(string code)
    {
        // Получаем ID пользователя из токена
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return BadRequest(new { message = "Недействительный токен" });
        }

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return BadRequest(new { message = "Некорректный ID пользователя" });
        }

       
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.StudentsCode == code || c.TeachersCode == code);

        if (course == null)
        {
            return NotFound(new { message = "Курс с таким кодом не найден" });
        }

        var userAlreadyInCourse = await _context.UsersCorses
            .AnyAsync(uc => uc.UserId == userId && uc.CourseId == course.Id);

        if (userAlreadyInCourse)
        {
            return Conflict(new { message = "Пользователь уже добавлен в этот курс" });
        }

        var userCourse = new UserCorse
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = course.Id,
            Role = course.StudentsCode == code ? Role.Student : Role.Teacher
        };

        _context.UsersCorses.Add(userCourse);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Пользователь успешно добавлен в курс", courseId = course.Id });

    }


    /// <summary>
    /// Покинуть курс
    /// </summary>
    /// <param name="courseId"></param>
    /// <returns>аыва</returns>
    /// 

    [HttpPost("leave/{courseId}")]
    [Authorize]
    public async Task<IActionResult> LeaveCourse(Guid courseId)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { message = "Недействительный токен" });
        }

        var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
        {
            return NotFound(new { message = "Курс не найден" });
        }

        var userCourse = await _context.UsersCorses
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

        if (userCourse == null)
        {
            return BadRequest(new { message = "Пользователь не состоит в этом курсе" });
        }

        if (userCourse.Role == Role.Owner)
        {
            return BadRequest(new
            {
                message = "Нельзя покинуть курс, так как вы являетесь владельцем. " +
                         "Сначала передайте права владельца другому пользователю или удалите курс."
            });
        }

        _context.UsersCorses.Remove(userCourse);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Вы успешно покинули курс" });
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
        }

        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (_tokenRevocationService.IsTokenRevoked(token))
        {
            return Unauthorized(new { status = "error", message = "Токен недействителен" });
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { status = "error", message = "Неверный идентификатор пользователя" });
        }
    
        var course = await _context.Courses
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound(new { message = "Курс не найден" });
        }


        // // 3. Удаляем все задачи курса
        // if (course.Tasks != null && course.Tasks.Any())
        // {
        //     _context.Tasks.RemoveRange(course.Tasks);
        // }


        _context.UsersCorses.RemoveRange();
        
        _context.Courses.Remove(course);
        
        var recordsToDelete = _context.UsersCorses.Where(uc => uc.CourseId == id).ToList();

        if (recordsToDelete.Any())
        {
            _context.UsersCorses.RemoveRange(recordsToDelete);
        }

        try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении курса");
                return StatusCode(500, new { status = "error", message = "Ошибка при удалении курса" });
            }
    }


    [HttpGet("{id}/users")]
    [Authorize]
    public async Task<ActionResult<UserList>> GetCourseUsers(Guid id)
    {
        try
        {
            if (!await _context.Courses.AnyAsync(c => c.Id == id))
            {
                return NotFound(new { message = "Курс не найден" });
            }

            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!await _context.UsersCorses.AnyAsync(uc => uc.CourseId == id && uc.UserId == currentUserId))
            {
                return Forbid();
            }

            var usersData = await _context.UsersCorses
                .Where(uc => uc.CourseId == id)
                .Join(_context.Users,
                    userCourse => userCourse.UserId,
                    user => user.Id,
                    (userCourse, user) => new
                    {
                        User = user,
                        userCourse.Role
                    })
                .ToListAsync();

            var response = new UserList
            {
                Owner = usersData
                    .Where(x => x.Role == Role.Owner)
                    .Select(x => MapToUserDto(x.User))
                    .FirstOrDefault(),
                Teachers = usersData
                    .Where(x => x.Role == Role.Teacher)
                    .Select(x => MapToUserDto(x.User))
                    .ToList(),
                Students = usersData
                    .Where(x => x.Role == Role.Student)
                    .Select(x => MapToUserDto(x.User))
                    .ToList()
            };

            if (response.Owner == null)
            {
                return StatusCode(500, new { message = "У курса не назначен владелец" });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении пользователей курса");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }

    [HttpGet("list")]
    [Authorize]
    public async Task<ActionResult<UserCoursesResponse>> GetUserCourses()
    {
        try
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "Недействительный токен" });
            }

            var courses = await _context.UsersCorses
                .Where(uc => uc.UserId == userId)
                .Join(
                    _context.Courses,
                    userCourse => userCourse.CourseId,
                    course => course.Id,
                    (userCourse, course) => new UserCourseDto
                    {
                        CourseId = course.Id,
                        CourseName = course.Name,
                        Subject = course.Subject,
                        Chapter = course.Chapter,
                        Role = userCourse.Role.ToString(),
                    })
                .ToListAsync();

            var response = new UserCoursesResponse
            {
                OwnedCourses = courses
                    .Where(c => c.Role == Role.Owner.ToString())
                    .ToList(),
                TeachingCourses = courses
                    .Where(c => c.Role == Role.Teacher.ToString())
                    .ToList(),
                StudentCourses = courses
                    .Where(c => c.Role == Role.Student.ToString())
                    .ToList()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка курсов пользователя");
            return StatusCode(500, new { message = "Произошла ошибка при обработке запроса" });
        }
    }

    private UserDto MapToUserDto(UserModel user)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim(),
            Email = user.Email,
            Birthday = user.Birthday.ToString("yyyy-MM-dd"),
        };
    }

    

 
    private string GenerateRandomCode(int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
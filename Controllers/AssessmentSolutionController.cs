
// using Microsoft.AspNetCore.Mvc;
// using System.Collections.Generic;
// using System.Linq;
// using Microsoft.EntityFrameworkCore;
// using Api.Models;
// using System.Security.Claims; 
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
// using System.IdentityModel.Tokens.Jwt;
// using Microsoft.AspNetCore.Authorization;
// using Domain.Entities;
// using Domain.Enums;


// namespace MyApi.MapControllers
// {


//     [Route("api/[controller]")]
//     [ApiController]
//     [Produces("application/json")]

//     [Route("api/checks")]
//     public class CheckAssignmentsController : ControllerBase
//     {
//         private readonly ApplicationContext _context;

//         private readonly ITokenRevocationService _tokenRevocationService;


//         public CheckAssignmentsController(
//         ApplicationContext context, ITokenRevocationService tokenRevocationService)
//         {
//             _context = context;
//             _tokenRevocationService = tokenRevocationService;

//         }

//         [HttpGet("List")]
//         [Authorize]
//         public async Task<ActionResult<IEnumerable<PackageCheckModel>>> GetMyChecks()
//         {
//             var currentUserId = GetCurrentUserId(); // Метод для получения ID текущего пользователя

//             var checks = await _context.PackageChecks
//                 .Where(p => p.SolutionForCheckTasks.Any(s => s.AuthortId == currentUserId))
//                 .Include(p => p.SolutionForCheckTasks)
//                     .ThenInclude(s => s.Solution)
//                 .ToListAsync();

//             return Ok(checks);
//         }

//         // 2. Получить конкретное задание на проверку
//         [HttpGet("{id}")]
//         public async Task<ActionResult<SolutionForCheckModel>> GetCheckTask(Guid id)
//         {
//             var currentUserId = GetCurrentUserId();
            
//             var checkTask = await _context.SolutionForChecks
//                 .Include(s => s.Solution)
//                 .FirstOrDefaultAsync(s => s.Id == id && s.AuthortId == currentUserId);

//             if (checkTask == null)
//             {
//                 return NotFound("Задание на проверку не найдено или у вас нет к нему доступа");
//             }

//             return Ok(checkTask);
//         }

//         // 3. Отправить результат проверки
//         [HttpPost("{id}/submit")]
//         public async Task<IActionResult> SubmitCheck(
//             Guid id,
//             [FromBody] CheckSubmissionDto submission)
//         {
//             var currentUserId = GetCurrentUserId();
            
//             var checkTask = await _context.SolutionForChecks
//                 .FirstOrDefaultAsync(s => s.Id == id && s.AuthortId == currentUserId);

//             if (checkTask == null)
//             {
//                 return NotFound("Задание на проверку не найдено или у вас нет к нему доступа");
//             }

//             checkTask.Comment = submission.Comment;
//             checkTask.IsCompleted = true;
//             checkTask.CompletionDate = DateTime.UtcNow;

//             await _context.SaveChangesAsync();

//             return Ok("Результат проверки успешно сохранен");
//         }

//             private Guid GetCurrentUserId()
//             {
//                 var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
//                 var userId = Guid.Parse(userIdClaim.Value);
//                 return userId;
//         }
//     }

//     // DTO для отправки результата проверки
//         public class CheckSubmissionDto
//         {
//             public string Comment { get; set; }
//         }
//     }
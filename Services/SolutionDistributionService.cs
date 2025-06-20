// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;

// public class SolutionDistributionService
// {
//     private readonly ApplicationContext _context;

//     public SolutionDistributionService(ApplicationContext context)
//     {
//         _context = context;
//     }

//     public async Task DistributeSolutionsAfterDeadline()
//     {

//         var expiredTasks = await _context.Tasks
//             .Where(t => t.Deadline < DateTime.UtcNow && !t.SolutionsDistributed)
//             .Include(t => t.Solution)
//             .ThenInclude(s => s.Student)
//             .Include(t => t.Course)
//             .ThenInclude(c => c.Participants)
//             .ToListAsync();

//         foreach (var task in expiredTasks)
//         {
//             await DistributeSolutionsForTask(task);
//         }

//         await _context.SaveChangesAsync();
//     }

//     private async Task DistributeSolutionsForTask(TaskModel task)
// {
//     var solutions = task.Solution?.ToList() ?? new List<SolutionModel>();
    
//     var onTimeSolutions = solutions.Where(s => s.SubmissionDate <= task.Deadline).ToList();
//     var lateSolutions = solutions.Where(s => s.SubmissionDate > task.Deadline).ToList();

//     var submitters = onTimeSolutions
//         .Select(s => s.Student)
//         .Distinct()
//         .ToList();

//     var checkAssignments = new List<SolutionForCheckModel>();

//     if (onTimeSolutions.Count > 0 && submitters.Count > 1) 
//     {
//         foreach (var solution in onTimeSolutions)
//         {
//             var availableReviewers = submitters
//                 .Where(u => u.Id != solution.StudentId)
//                 .OrderBy(x => Guid.NewGuid()) 
//                 .ToList();

//             var existingChecks = checkAssignments
//                 .Count(c => c.SolutionId == solution.Id);

//             var neededChecks = task.M - existingChecks;

//             for (int i = 0; i < neededChecks && i < availableReviewers.Count; i++)
//             {
//                 checkAssignments.Add(new SolutionForCheckModel
//                 {
//                     SolutionId = solution.Id,
//                     AuthortId = availableReviewers[i].Id,
//                     DueTime = DateTime.UtcNow.AddDays(7),
//                     Comment = string.Empty
//                 });
//             }
//         }
//     }

//     foreach (var lateSolution in lateSolutions)
//     {
//         for (int i = 0; i < task.M; i++)
//         {
//             checkAssignments.Add(new SolutionForCheckModel
//             {
//                 SolutionId = lateSolution.Id,
//                 AuthortId = task.AuthorId,
//                 DueTime = DateTime.UtcNow.AddDays(7),
//                 Comment = "Решение сдано после дедлайна"
//             });
//         }
//     }

//     await _context.PackageChecks.AddAsync(new PackageCheckModel
//     {
//         SolutionForCheckTasks = checkAssignments,
//         Deadline = DateTime.UtcNow.AddDays(7).ToString("o")
//     });

//     task.SolutionsDistributed = true;
// }
// }
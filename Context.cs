using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Api.Models;
public class ApplicationContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserCorse> UsersCorses { get; set; }
    public DbSet<CourseModel> Courses {get;set;}
    public DbSet<InvitationLink> Lessons { get; set; }


    public bool TestConnection()
    {
        try
        {
            return Database.CanConnect(); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка подключения к базе данных: {ex.Message}");
            return false;
        }
    }
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
}
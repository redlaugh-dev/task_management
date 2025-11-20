using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManagement.DataBase.Entities;

namespace TaskManagement.DataBase;

public class DataBaseContext : DbContext
{
    public DbSet<UserType> TypeUsers { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<StatusTask> StatusTasks { get; set; } = null!;
    public DbSet<EmployeeTask>  EmployeeTasks { get; set; } = null!;

    public DataBaseContext()
    {
        var conn = (SqliteConnection)Database.GetDbConnection();
        if (System.OperatingSystem.IsWindows())
        {
            conn.LoadExtension("ext\\unicode.dll");
        }
        else if (System.OperatingSystem.IsLinux())
        {
            conn.LoadExtension("ext/unicode.so");
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserType>(e =>
        {
            e.HasKey(e => e.Id).HasName("type_user_pk");
            e.Property(e => e.Type).HasMaxLength(100);
            e.HasData(new UserType[]
            {
                new UserType() { Id = 1, Type = "Сотрудник"},
                new UserType() { Id = 2, Type = "Руководитель"},
                new UserType() { Id = 3, Type = "Администратор"},
            });
        });
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(e => e.Id).HasName("user_pk");
            e.HasIndex(e => e.Login).IsUnique().HasName("login_uq");
            e.Property(e => e.Password).HasMaxLength(100);
            e.Property(e => e.Surname).HasMaxLength(100);
            e.Property(e => e.Name).HasMaxLength(100);
            e.HasOne(e => e.UserType)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.TypeUserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("type_user_fk");
            e.HasData(new User()
            {
                Id = 1, 
                TypeUserId = 3,
                Login = "admin",
                Password = "admin",
                Surname = "Админов",
                Name = "Админ",
            });

        });
        modelBuilder.Entity<StatusTask>(e =>
        {
            e.HasKey(e => e.Id).HasName("status_task_pk");
            e.Property(e => e.Name).HasMaxLength(100);
            e.HasData(new StatusTask[]
            {
                new StatusTask() { Id = 1, Name = "Новая"},
                new StatusTask() { Id = 2, Name = "В работе"},
                new StatusTask() { Id = 3, Name = "На проверке"},
                new StatusTask() { Id = 4, Name = "Выполнена"},
            });
        });
        modelBuilder.Entity<EmployeeTask>(e =>
        {
            e.HasKey(e => e.Id).HasName("status_task_pk");
            e.Property(e => e.Title).HasMaxLength(200);
            e.Property(e => e.Description).HasMaxLength(500);
            e.Property(e => e.DateCreate).HasColumnType("datetime");
            e.HasOne(e => e.StatusTask)
                .WithMany(p => p.Tasks)
                .HasForeignKey(e => e.StatusTaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("status_task_fk");
            e.HasOne(e=>e.Author)
                .WithMany(e =>e.AuthorTasks)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("author_fk");
            e.HasOne(e=>e.Executor)
                .WithMany(e =>e.ExecutorTasks)
                .HasForeignKey(e => e.ExecutorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("executor_fk");

        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Filename=database.db");
    }
}
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Areas.Identity.Data;
using TaskManager.Models;

namespace TaskManager.Data;

public static class DbInitializer
{
    public static void Initialize(TaskManagerContext context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
    {
        if (context.Projects.Any())
        {
            return;
        }

        var projectId1 = Guid.NewGuid();
        var projectId2 = Guid.NewGuid();
        var projectId3 = Guid.NewGuid();
        var projects = new Project[]
        {
            new Project
                {
                    Id = projectId1,
                    Name = "Project 01",
                    Description = "First Project",
                    Tag = "Tag1",
                    GoalDate = DateTime.Parse("2022-4-10")
                },
                new Project
                {
                    Id = projectId2,
                    Name = "Project 02",
                    Description = "Second Project",
                    Tag = "Tag1",
                    GoalDate = DateTime.Parse("2022-5-22")
                },
                new Project
                {
                    Id = projectId3,
                    Name = "Project 03",
                    Description = "Third Project",
                    Tag = "TagB",
                    GoalDate = DateTime.Parse("2022-7-19")
                }
        };
        foreach (Project p in projects)
        {
            context.Projects.Add(p);
        }
        context.SaveChanges();


        var assignments = new ProjectAssignment[]
        {
            new ProjectAssignment
            {
                ProjectId = projectId1,
                ApplicationUserId = "f1142c8a-79f0-42d9-af4b-c7ca07c6f908",
                IsManager = true
            },
            new ProjectAssignment
            {
                ProjectId = projectId1,
                ApplicationUserId = "f98add5c-efdd-418d-a492-6e5a7b31cb7b",
                IsManager = false
            },
            new ProjectAssignment
            {
                ProjectId = projectId2,
                ApplicationUserId = "f98add5c-efdd-418d-a492-6e5a7b31cb7b",
                IsManager = true
            },
            new ProjectAssignment
            {
                ProjectId = projectId3,
                ApplicationUserId = "f1142c8a-79f0-42d9-af4b-c7ca07c6f908",
                IsManager = true
            },
            new ProjectAssignment
            {
                ProjectId = projectId3,
                ApplicationUserId = "f98add5c-efdd-418d-a492-6e5a7b31cb7b",
                IsManager = false
            }
        };
        foreach (ProjectAssignment pa in assignments)
        {
            context.ProjectAssignments.Add(pa);
        }
        context.SaveChanges();
    }
}

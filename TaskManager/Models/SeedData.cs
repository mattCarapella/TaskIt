using Microsoft.EntityFrameworkCore;
using TaskManager.Data;

namespace TaskManager.Models
{
    public class SeedData
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new TaskManagerContext(
                serviceProvider.GetRequiredService<DbContextOptions<TaskManagerContext>>()))
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
                        ApplicationUserId = "017b4647-8ff4-4093-a599-90fd2d1e56d7",
                        IsManager = true
                    },
                    new ProjectAssignment
                    {
                        ProjectId = projectId1,
                        ApplicationUserId = "3dbb26d4-fbc1-4898-90bd-18d83a83f2ad",
                        IsManager = false
                    },
                    new ProjectAssignment
                    {
                        ProjectId = projectId2,
                        ApplicationUserId = "3dbb26d4-fbc1-4898-90bd-18d83a83f2ad",
                        IsManager = true
                    },
                    new ProjectAssignment
                    {
                        ProjectId = projectId3,
                        ApplicationUserId = "017b4647-8ff4-4093-a599-90fd2d1e56d7",
                        IsManager = true
                    },
                    new ProjectAssignment
                    {
                        ProjectId = projectId3,
                        ApplicationUserId = "3dbb26d4-fbc1-4898-90bd-18d83a83f2ad",
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
    }
}

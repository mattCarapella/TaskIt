using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Migrations
{
    public partial class SeedRoles : Migration
    {
        private string ManagerRoleId = Guid.NewGuid().ToString();
        private string UserRoleId = Guid.NewGuid().ToString();
        private string AdminRoleId = Guid.NewGuid().ToString();

        private string User1Id = Guid.NewGuid().ToString();
        private string User2Id = Guid.NewGuid().ToString();

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SeedRolesSQL(migrationBuilder);

            SeedUser(migrationBuilder);

            SeedUserRoles(migrationBuilder);
        }

        private void SeedRolesSQL(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"INSERT INTO [dbo].[AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp])
            VALUES ('{AdminRoleId}', 'Administrator', 'ADMINISTRATOR', null);");
            migrationBuilder.Sql(@$"INSERT INTO [dbo].[AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp])
            VALUES ('{ManagerRoleId}', 'Manager', 'MANAGER', null);");
            migrationBuilder.Sql(@$"INSERT INTO [dbo].[AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp])
            VALUES ('{UserRoleId}', 'User', 'USER', null);");
        }



        private void SeedUser(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @$"INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserName], [NormalizedUserName], 
[Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], 
[PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount],
[ProfilePicture], [EmployeeId], [JobTitle], [CreatedAt], [LastLoggedInAt]) 
VALUES 
(N'{User1Id}', N'Joe', N'Blow', N'JoeBlow', N'JOEBLOW', 
N'joe@test.com', N'JOE@TEST.COM', 0, 
N'AQAAAAEAACcQAAAAEEdbMTTpvlGGIHC1yCPem2y1KClVlmYwsZ8NazGiHCniuYY+okEL9A12jXYY/78OMw==', 
N'KBLZILOSEDV6FGP7KJKXC3WQCXQR6XWI', N'd8236396-195b-4bd3-81ad-dd154e6aad3b', NULL, 0, 0, NULL, 1, 0,
N'', N'743116800', N'Developer II', NULL, NULL)");

            migrationBuilder.Sql(
                @$"INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserName], [NormalizedUserName], 
[Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], 
[PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount],
[ProfilePicture], [EmployeeId], [JobTitle], [CreatedAt], [LastLoggedInAt])
VALUES 
(N'{User2Id}', N'Rico', N'Suave', N'Rico69', N'RICO69', 
N'rico@test.com', N'RICO@TEST.COM', 0, 
N'AQAAAAEAACcQAAAAEEdbMTTpvlGGIHC1yCPem2y1KClVlmYwsZ8NazGiHCniuYY+okEL9A12jXYY/78OMw==', 
N'KBLZILOSEDV6FGP7KJKXC3WQCXQR6XWI', N'd8236396-195b-4bd3-81ad-dd154e6aad3b', NULL, 0, 0, NULL, 1, 0,
N'', N'A3440', N'Team Lead', NULL, NULL)");
        }


        private void SeedUserRoles(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
        INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
        VALUES
           ('{User1Id}', '{ManagerRoleId}');
        INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
        VALUES
           ('{User1Id}', '{UserRoleId}');");

            migrationBuilder.Sql(@$"
        INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
        VALUES
           ('{User2Id}', '{AdminRoleId}');
        INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
        VALUES
           ('{User2Id}', '{ManagerRoleId}');");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
﻿namespace TaskManager.Core;

public class Constants
{
    public static class Roles
    {
        public const string Adminstrator = "Administrator";
        public const string Manager = "Manager";
        public const string User = "User";
    }

    public static class Policies
    {
        public const string RequireAdmin = "RequireAdmin";
        public const string RequireManager = "RequireManager";
    }

}

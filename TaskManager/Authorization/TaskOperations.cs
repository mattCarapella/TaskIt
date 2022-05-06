using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TaskManager.Authorization;

public class TaskOperations
{
    public static OperationAuthorizationRequirement Create =
      new OperationAuthorizationRequirement { Name = AuthConstants.CreateOperationName };
    public static OperationAuthorizationRequirement Read =
      new OperationAuthorizationRequirement { Name = AuthConstants.ReadOperationName };
    public static OperationAuthorizationRequirement Update =
      new OperationAuthorizationRequirement { Name = AuthConstants.UpdateOperationName };
    public static OperationAuthorizationRequirement Delete =
      new OperationAuthorizationRequirement { Name = AuthConstants.DeleteOperationName };
    //public static OperationAuthorizationRequirement Approve =
    //  new OperationAuthorizationRequirement { Name = AuthConstants.ApproveOperationName };
    //public static OperationAuthorizationRequirement Reject =
    //  new OperationAuthorizationRequirement { Name = AuthConstants.RejectOperationName };
}

public class AuthConstants
{
    public static readonly string CreateOperationName = "Create";
    public static readonly string ReadOperationName = "Read";
    public static readonly string UpdateOperationName = "Update";
    public static readonly string DeleteOperationName = "Delete";
    //public static readonly string ApproveOperationName = "Approve";
    //public static readonly string RejectOperationName = "Reject";
}
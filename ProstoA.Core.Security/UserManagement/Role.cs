namespace ProstoA.Core.Security.UserManagement;

public class Role
{
    // https://appwrite.io/docs/advanced/platform/permissions
    
    /// <summary>
    /// Grants access to anyone.
    /// </summary>
    public static Role Any() => throw new NotImplementedException();
    
    /// <summary>
    /// Grants access to any guest user without a session.
    /// </summary>
    /// <remarks>
    /// Authenticated users don't have access to this role.
    /// </remarks>
    public static Role Guests() => throw new NotImplementedException();
    
    /// <summary>
    /// Grants access to any authenticated or anonymous user.
    /// </summary>
    /// You can optionally pass the verified or unverified string to target specific types of users.
    public static Role AnyUsers(/*UserStatus*/) => throw new NotImplementedException();
    
    /// <summary>
    /// Grants access to a specific user by user ID.
    /// </summary>
    /// You can optionally pass the verified or unverified string to target specific types of users.
    public static Role User(/*UserId, UserStatus*/) => throw new NotImplementedException();
    
    /// <summary>
    /// Grants access to any member of the specific team and who possesses a specific role in a team.
    /// </summary>
    /// <remarks>
    /// To gain access to this permission, the user must be a member of the specific team and have the given role assigned to them.
    /// Team roles can be assigned when inviting a user to become a team member.
    /// </remarks>
    public static Role Team(/*TeamId, TeamRole */) => throw new NotImplementedException();
    
    /// <summary>
    /// Grants access to a specific member of a team.
    /// When the member is removed from the team, they will no longer have access.
    /// </summary>
    public static Role Member(/*MembershipId*/) => throw new NotImplementedException();
    
    /// <summary>
    /// Grants access to all accounts with a specific label ID.
    /// Once the label is removed from the user, they will no longer have access.
    /// </summary>
    public static Role Label(/*LabelId*/) => throw new NotImplementedException();
    
    // Custom permission
    // user:[USER_ID] or team:[TEAM_ID]/[ROLE]
}
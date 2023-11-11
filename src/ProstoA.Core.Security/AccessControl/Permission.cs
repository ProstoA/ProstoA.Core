using ProstoA.Core.Security.UserManagement;

namespace ProstoA.Core.Security.AccessControl;

// https://keystonejs.com/docs/config/access-control

/*
    session	Текущий объект сеанса. Подробности см. в Sessions API.
    context	KeystoneContext Объект исходной операции GraphQL.
    listKey	Ключ из списка, с которым выполняется операция.
    fieldKey	Ключ поля, с которым выполняется операция.
    operation	Выполняемая операция ('read', 'create', 'update').
    inputData	Для операций create и update это значениеdata, переданное в мутацию.
    item	Существующий элемент считывается / обновляется в read и update операциях.
*/

public class Permission
{
    // https://appwrite.io/docs/advanced/platform/permissions
    
    /*[
        Permission.read(Role.any()),                  // Anyone can view this document
        Permission.update(Role.team("writers")),      // Writers can update this document
        Permission.update(Role.team("admin")),        // Admins can update this document
        Permission.delete(Role.user("5c1f88b42259e")), // User 5c1f88b42259e can delete this document
        Permission.delete(Role.team("admin"))          // Admins can delete this document
        
        Permission.read(Role.team("5c1f88b87435e")),             // Only users of team 5c1f88b87435e can read the document
        Permission.update(Role.team("5c1f88b87435e", "owner")), // Only users of team 5c1f88b87435e with the role owner can update the document
        Permission.delete(Role.team("5c1f88b87435e", "owner"))  // Only users of team 5c1f88b87435e with the role owner can delete the document
       
    ]*/
    
    /// <summary>
    /// Access to read a resource.
    /// </summary>
    public static Permission Read(Role role) => throw new NotImplementedException();
    
    /// <summary>
    /// Access to create new resources.
    /// </summary>
    public static Permission Create(Role role) => throw new NotImplementedException();
    
    /// <summary>
    /// Access to change a resource, but not remove or create new resources.
    /// </summary>
    public static Permission Update(Role role) => throw new NotImplementedException();
    
    /// <summary>
    /// Access to change a resource, but not remove or create new resources.
    /// </summary>
    public static Permission Delete(Role role) => throw new NotImplementedException();
    
    /// <summary>
    /// Alias to grant create, update, and delete access for resources.
    /// </summary>
    public static Permission Write(Role role) => throw new NotImplementedException();
}
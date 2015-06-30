using System.Collections.Generic;
using System.Linq;
using A4EPARC.Models;
using A4EPARC.ViewModels;

namespace A4EPARC.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public int Add(IUser user)
        {
            return
                Execute(
                    @"INSERT INTO [dbo].[User] (Email, Password, Salt, IsSuperAdmin, IsAdmin, IsViewer, CompanyId, CreatedDate, IsActive) VALUES (@Email, @Password, @Salt, @IsSuperAdmin, @IsAdmin, @IsViewer, @CompanyId, @CreatedDate, @IsActive)SELECT CAST(SCOPE_IDENTITY() as int)", user);
        }

        public int Save(IUser user)
        {
            return
                Execute(
                    @"UPDATE [dbo].[User] SET IsSuperAdmin = @IsSuperAdmin, IsAdmin = @IsAdmin, IsViewer = @IsViewer, CompanyId = @CompanyId, IsActive = @IsActive  WHERE Id = @Id", user);
        }


        public int UpdatePassword(IUser user)
        {
            return
                Execute(
                    @"UPDATE [dbo].[User] SET Password = @Password, Salt = @Salt WHERE Email = @Email", user);
        }

        public List<UserViewModel> GetJtableView()
        {
            return
                Query<UserViewModel>(
                    @"SELECT u.Id, u.Email, u.IsSuperAdmin, u.IsAdmin, u.IsViewer, c.Id As CompanyId, c.Name AS CompanyName, u.IsActive FROM [User] u INNER JOIN CompanyNew c ON c.Id = u.CompanyId")
                    .ToList();
        }
    }

    public interface IUserRepository : IRepository<User>
    {
        int Add(IUser user);
        int Save(IUser user);
        int UpdatePassword(IUser user);
        List<UserViewModel> GetJtableView();
    }
}
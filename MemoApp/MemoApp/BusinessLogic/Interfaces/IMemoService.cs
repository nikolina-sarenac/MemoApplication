using MemoApp.Common;
using MemoApp.Data;
using MemoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemoApp.BusinessLogic.Interfaces
{
    public interface IMemoService
    {
        IFeedback<IEnumerable<MemoViewModel>> GetAll();
        IFeedback<NoValue> DeleteMemoById(long Id);
        IFeedback<NoValue> CreateMemo(CreateMemoViewModel memo);
        IFeedback<MemoViewModel> GetMemoById(int id);
        IFeedback<NoValue> EditMemo(MemoViewModel memo);

        IFeedback<IEnumerable<RoleViewModel>> GetAllRoles();
        IFeedback<IEnumerable<UserViewModel>> GetAllUsers();
        IFeedback<NoValue> CreateRole(RoleViewModel role);
        IFeedback<NoValue> DeleteRole(string id);
    }
}

using MemoApp.BusinessLogic.Interfaces;
using MemoApp.Common;
using MemoApp.Data;
using MemoApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace MemoApp.BusinessLogic.Services
{
    public class MemoService : IMemoService
    {
        private readonly MemoEntities _entities;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MemoService(MemoEntities entities, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _entities = entities;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IFeedback<NoValue> CreateMemo(CreateMemoViewModel memo)
        {
            var feedback = new Feedback<NoValue>();

            try
            {
                Memo newMemo = new Memo()
                {
                    AspNetUserId = memo.userId,
                    CreatedAt = DateTime.Now,
                    Note = memo.Note,
                    StatusId = (int)StatusEnum.Active,
                    Title = memo.Title
                };

                _entities.Memo.Add(newMemo);

                List<string> tags = memo.Tags.Split(' ').ToList();
                var tagList = new List<Tag>();

                foreach (var tag in tags)
                {
                    if (!String.IsNullOrWhiteSpace(tag))
                    {
                        tagList.Add(new Tag() { Name = tag, Memo = newMemo });
                    }
                }

                _entities.Tag.AddRange(tagList);
                _entities.SaveChanges();
                feedback.Status = StatusEnum.Success;
                Log.Information("New memo added - " + memo.Title);
            }
            catch (Exception ex)
            {
                feedback.Status = StatusEnum.Error;
                feedback.Message = ex.GetBaseException().Message;
                Log.Debug(ex.GetBaseException().Message);
            }

            return feedback;
        }

        public IFeedback<NoValue> CreateRole(RoleViewModel role)
        {
            var feedback = new Feedback<NoValue>();
            try
            {
                if (!_roleManager.RoleExistsAsync(role.Name).Result)
                {
                    IdentityRole newRole = new IdentityRole();
                    newRole.Name = role.Name;
                    IdentityResult roleResult = _roleManager.CreateAsync(newRole).Result;

                    if(roleResult.Succeeded)
                    {
                        feedback.Status = StatusEnum.Success;
                        Log.Information("New role " + newRole.Name + " added.");
                    }
                    else
                    {
                        feedback.Status = StatusEnum.Error;
                        feedback.Message = "Role could not be added";
                        Log.Error(feedback.Message);
                    }
                    
                }
                else
                {
                    feedback.Status = StatusEnum.Error;
                    feedback.Message = "Same role name already exists.";
                    Log.Error(feedback.Message);
                }
            }
            catch (Exception ex)
            {
                feedback.Status = StatusEnum.Error;
                feedback.Message = ex.GetBaseException().Message;
                Log.Debug(ex.GetBaseException().Message);
            }
            return feedback;
        }

        public IFeedback<NoValue> DeleteMemoById(long Id)
        {
            var feedback = new Feedback<NoValue>();
            try
            {
                var memo = _entities.Memo.First(m => m.Id == Id);
                if(memo != null)
                {
                    memo.StatusId = (int)StatusEnum.Deleted;
                    _entities.Update(memo);
                    var tags = _entities.Tag.Where(t => t.MemoId == Id).ToList();
                    _entities.Tag.RemoveRange(tags);
                    _entities.SaveChanges();
                    feedback.Status = StatusEnum.Deleted;
                    Log.Information("Memo - " + memo.Title + "deleted");
                }
                else
                {
                    feedback.Status = StatusEnum.Error;
                    feedback.Message = "Memo not found";
                    Log.Error("Delete memo by Id - " + feedback.Message);
                }
            }
            catch (Exception ex)
            {
                feedback.Status = StatusEnum.Error;
                feedback.Message = ex.GetBaseException().Message;
                Log.Debug(ex.GetBaseException().Message);
            }
            return feedback;
        }

        public IFeedback<NoValue> DeleteRole(string id)
        {
            var feedback = new Feedback<NoValue>();

            var role = _roleManager.FindByIdAsync(id).Result;
            if(role != null)
            {
                var result = _roleManager.DeleteAsync(role).Result;
                if(result.Succeeded)
                {
                    feedback.Status = StatusEnum.Success;
                }
            }
            else
            {
                feedback.Status = StatusEnum.Error;
                feedback.Message = "Role not found";
            }

            return feedback;
        }

        public IFeedback<NoValue> EditMemo(MemoViewModel memo)
        {
            var feedback = new Feedback<NoValue>();
            try
            {
                var m = _entities.Memo.Where(me => me.Id == memo.Id).First();
                if (m != null)
                {
                    m.Title = memo.Title;
                    m.Note = memo.Note;
                    m.UpdatedAt = DateTime.Now;

                    var tags = _entities.Tag.Where(t => t.MemoId == memo.Id).ToList();
                    _entities.Tag.RemoveRange(tags);

                    if(!string.IsNullOrWhiteSpace(memo.Tags))
                    {
                        List<string> newTagNames = memo.Tags.Split(' ').ToList();
                        var tagList = new List<Tag>();

                        foreach (var t in newTagNames)
                        {
                            if (!String.IsNullOrWhiteSpace(t))
                            {
                                tagList.Add(new Tag() { Name = t, MemoId = memo.Id });
                            }
                        }

                        _entities.Tag.AddRange(tagList);
                    }
                    
                    _entities.Update(m);
                    _entities.SaveChanges();
                    feedback.Status = StatusEnum.Success;
                    Log.Information("Memo edited - " + memo.Title);
                }
                else
                {
                    feedback.Message = "Memo not found";
                    feedback.Status = StatusEnum.Error;
                    Log.Error("EditMemo - " + feedback.Message);
                }
            }
            catch (Exception ex)
            {
                feedback.Message = ex.GetBaseException().Message;
                feedback.Status = StatusEnum.Error;
                Log.Debug(ex.GetBaseException().Message);
            }

            return feedback;
        }

        public IFeedback<IEnumerable<MemoViewModel>> GetAll()
        {
            var feedback = new Feedback<IEnumerable<MemoViewModel>>();
            try
            {
                var memos = _entities.Memo
                    .Include(m => m.Status)
                    .Where(m => m.StatusId != (int)StatusEnum.Deleted)
                    .Select(m => new MemoViewModel {
                        AspNetUserId = m.AspNetUserId,
                        Id = m.Id,
                        Note = m.Note,
                        CreatedAt = m.CreatedAt,
                        CreatedAtStr = m.CreatedAt.ToString(),
                        UpdatedAt = m.UpdatedAt,
                        UpdatedAtStr = m.UpdatedAt.ToString(),
                        Status = m.Status.Name,
                        StatusId = m.StatusId,
                        Title = m.Title
                    }).ToList();

                feedback.Value = memos;
                feedback.Status = StatusEnum.Success;
                Log.Information("Memo list retrieved");
            }
            catch (Exception ex)
            {
                // error should be logged here 
                feedback.Message = ex.Message;
                feedback.Status = StatusEnum.Error;
                Log.Debug(ex.GetBaseException().Message);
            }

            return feedback;
        }

        public IFeedback<IEnumerable<RoleViewModel>> GetAllRoles()
        {
            var feedback = new Feedback<IEnumerable<RoleViewModel>>();

            try
            {
                var roles = _roleManager.Roles
                    .Select(r => new RoleViewModel
                    {
                        Id = r.Id,
                        Name = r.Name
                    }).ToList();

                feedback.Value = roles;
                feedback.Status = StatusEnum.Success;
                Log.Information("Roles list retrieved");
            }
            catch (Exception ex)
            {
                feedback.Message = ex.Message;
                feedback.Status = StatusEnum.Error;
                Log.Debug(ex.GetBaseException().Message);
            }

            return feedback;
        }

        public IFeedback<IEnumerable<UserViewModel>> GetAllUsers()
        {
            var feedback = new Feedback<IEnumerable<UserViewModel>>();

            try
            {
                var users = _userManager.Users
                    .Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        Role = new RoleViewModel() { Name = _userManager.GetRolesAsync(u).Result.FirstOrDefault(), Id = "" }
                    }).ToList();

                feedback.Value = users;
                feedback.Status = StatusEnum.Success;
                Log.Information("Users list retrieved");
            }
            catch (Exception ex)
            {
                feedback.Message = ex.Message;
                feedback.Status = StatusEnum.Error;
                Log.Debug(ex.GetBaseException().Message);
            }

            return feedback;
        }

        public IFeedback<MemoViewModel> GetMemoById(int id)
        {
            var feedback = new Feedback<MemoViewModel>();
            try
            {
                var memo = _entities.Memo
                    .Include(m => m.Status)
                    .Where(m => m.StatusId != (int)StatusEnum.Deleted && m.Id == id)
                    .Select(m => new MemoViewModel
                    {
                        AspNetUserId = m.AspNetUserId,
                        Id = m.Id,
                        Note = m.Note,
                        CreatedAt = m.CreatedAt,
                        CreatedAtStr = m.CreatedAt.ToString(),
                        UpdatedAt = m.UpdatedAt,
                        UpdatedAtStr = m.UpdatedAt.ToString(),
                        Status = m.Status.Name,
                        StatusId = m.StatusId,
                        Title = m.Title
                    }).ToList();

                if(memo.Count != 0)
                {
                    feedback.Value = memo.First();

                    var tags = _entities.Tag.Where(t => t.MemoId == id)
                        .Select(s => s.Name)
                        .ToList();
                    string tagsStr = "";
                    foreach (var t in tags)
                    {
                        tagsStr += t;
                        tagsStr += " ";
                    }
                    feedback.Value.Tags = tagsStr;

                    feedback.Status = StatusEnum.Success;
                    Log.Information("Memo retrieved - " + feedback.Value.Title);
                }
                else
                {
                    feedback.Message = "Memo not found";
                    feedback.Status = StatusEnum.Error;
                    Log.Error("GetMemo by Id - " + feedback.Message);
                }
                
            }
            catch (Exception ex)
            {
                // error should be logged here 
                feedback.Message = ex.Message;
                feedback.Status = StatusEnum.Error;
                Log.Debug(ex.GetBaseException().Message);
            }

            return feedback;
        }
    }
}

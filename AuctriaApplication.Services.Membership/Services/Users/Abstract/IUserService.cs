﻿using AuctriaApplication.Services.Membership.Dto;
using AuctriaApplication.Services.Membership.Dto.ViewModel;
using UserViewModel = AuctriaApplication.Services.Membership.Dto.UserViewModel;

namespace AuctriaApplication.Services.Membership.Services.Users.Abstract;

public interface IUserService
{
    Task<List<Dto.ViewModel.UsersViewModel>> GetListAsync();

    Task<UserViewModel> RegisterAsync(RegisterDto registerDto);

    Task<UserViewModel?> LoginAsync(LoginDto loginDto);
    
    Task<UserViewModel> CurrentUserAsync(Guid userId);

    Task<bool> SetTempCodeAsync(
        Guid userId,
        int validationType,
        string code,
        CancellationToken cancellationToken = default);
    
    Task<(bool, string?)> VerifyEmailAsync(
        Guid userId,
        int verificationType,
        string userCode);
    
    Task<(bool, string?)> VerifyPhoneAsync(
        Guid userId, 
        int verificationType,
        string userCode);
    
    Task<bool> LockOutAsync(
        Guid targetUserId, 
        int days);

    Task<bool> UnLockAsync(Guid targetUserId);

    Task<bool> IsUserLockedAsync(
        Guid? userId = null,
        string? email = null);

    Task<bool> IsFieldVerifiedAsync(
        Guid userId,
        string fieldName);

}
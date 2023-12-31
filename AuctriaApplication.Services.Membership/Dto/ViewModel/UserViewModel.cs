﻿namespace AuctriaApplication.Services.Membership.Dto.ViewModel;

public class UserViewModel
{
    public string? Username { get; set; }
    public string Token { get; set; }
    public string Role { get; set; }
    public bool IsPhoneVerified { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsLock { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
}
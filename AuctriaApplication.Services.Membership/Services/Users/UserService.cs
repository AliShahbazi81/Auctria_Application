using System.Linq.Expressions;
using AuctriaApplication.DataAccess.DbContext;
using AuctriaApplication.DataAccess.Entities.Users;
using AuctriaApplication.Domain.Helper;
using AuctriaApplication.Domain.Variables;
using AuctriaApplication.Services.Membership.Dto;
using AuctriaApplication.Services.Membership.Exceptions;
using AuctriaApplication.Services.Membership.Services.Token;
using AuctriaApplication.Services.Membership.Services.Users.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserViewModel = AuctriaApplication.Services.Membership.Dto.ViewModel.UserViewModel;

namespace AuctriaApplication.Services.Membership.Services.Users;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IDbContextFactory<ApplicationDbContext> _context;
    private readonly ITokenService _tokenService;

    public UserService(
        IDbContextFactory<ApplicationDbContext> context,
        ITokenService tokenService, 
        UserManager<User> userManager)
    {
        _context = context;
        _tokenService = tokenService;
        _userManager = userManager;
    }
    
    public async Task<List<Dto.ViewModel.UsersViewModel>> GetListAsync()
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        var query = from user in dbContext.User
            join userRole in dbContext.UserRoles on user.Id equals userRole.UserId
            join role in dbContext.Roles on userRole.RoleId equals role.Id
            select new
            {
                user,
                userRole,
                role.Name
            };
        

        var userList = await query
            .Select(u => new Dto.ViewModel.UsersViewModel
            {
                Id = u.user.Id,
                Username = u.user.UserName!,
                Name = u.user.Name, 
                Surname = u.user.Surname, 
                Role = u.Name,
            })
            .ToListAsync();

        return userList;
    }
    
    public async Task<UserViewModel> RegisterAsync(RegisterDto registerDto)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        registerDto.Email = StringHelper.ConvertToLowerCaseNoSpaces(registerDto.Email)!;

        // Check if the user already exists
        var existingUser = await dbContext.User
            .SingleOrDefaultAsync(u => u.Email!.ToLower().Replace(" ","") == registerDto.Email);

        if (existingUser != null)
        {
            // User already exists
            throw new Exception("User already exists with the provided email.");
        }

        // Register new user
        var newUser = new User
        {
            Name = registerDto.Name,
            Surname = registerDto.Surname,
            Email = registerDto.Email,
            UserName = registerDto.Username,
            PhoneNumber = registerDto.PhoneNumber,
            LockoutEnabled = false,
            TwoFactorEnabled = false
        };

        var createResult = await _userManager.CreateAsync(newUser, registerDto.Password);

        if (!createResult.Succeeded)
            throw new Exception("User creation failed.");

        // Assign role "Member"
        await _userManager.AddToRoleAsync(newUser, SharedRolesVar.Member);

        await dbContext.SaveChangesAsync();

        return await ToUserDto(newUser, dbContext);
    }
    
    public async Task<UserViewModel?> LoginAsync(LoginDto loginDto)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        loginDto.Email = StringHelper.ConvertToLowerCaseNoSpaces(loginDto.Email)!;

        var user = await dbContext.User
            .SingleOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null)
            return null;

        var passwordCheck = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!passwordCheck)
            return null; // Password mismatch

        return await ToUserDto(user, dbContext);
    }
    
    private async Task<UserViewModel> ToUserDto(User user, ApplicationDbContext dbContext)
    {
        return new UserViewModel
        {
            Username = user.UserName,
            Token = await _tokenService.GenerateAsync(user),
            Role = (await UserRoleAsync(user.Id, dbContext))!,
            IsPhoneVerified = user.PhoneNumberConfirmed,
            IsEmailVerified = user.EmailConfirmed,
            IsTwoFactorEnabled = user.TwoFactorEnabled,
            IsLock = user.LockoutEnabled
        };
    }

    public async Task<UserViewModel> CurrentUserAsync(Guid userId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        var user = await dbContext.User
            .Where(x => x.Id == userId)
            .SingleOrDefaultAsync();

        return new UserViewModel
        {
            Username = user!.UserName,
            Token = await _tokenService.GenerateAsync(user),
            Role = (await UserRoleAsync(user.Id, dbContext))!,
            IsPhoneVerified = user.PhoneNumberConfirmed,
            IsEmailVerified = user.EmailConfirmed,
            IsLock = user.LockoutEnabled,
            IsTwoFactorEnabled = user.TwoFactorEnabled
        };
    }
    
    public async Task<bool> SetTempCodeAsync(
        Guid userId,
        int validationType,
        string code,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);
        
        _ = dbContext.UserTempValidations.Add(new UserTempValidation
        {
            UserId = userId,
            TypeValidation = validationType,
            Code = code,
            
        }).Entity;
        
        var isSaved = await dbContext.SaveChangesAsync(cancellationToken) > 0;

        return isSaved;
    }
    
    public async Task<(bool, string?)> VerifyEmailAsync(
        Guid userId,
        int verificationType,
        string userCode)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        var tempCode = await dbContext.UserTempValidations
            .SingleAsync(x => x.UserId == userId);

        var userWithCode = await dbContext.User
            .SingleAsync(user => user.Id == userId);

        if (tempCode.Expiry < DateTime.UtcNow)
        {
            dbContext.UserTempValidations.Remove(tempCode);
            await dbContext.SaveChangesAsync();
            return (false, "The code has expired, please request a new code!");
        }

        if (tempCode.TypeValidation != verificationType)
            return (false, "Code is not generated for Email Verification!");

        if (userCode != tempCode.Code)
            return (false, "The entered code is Incorrect!");

        userWithCode.EmailConfirmed = true;
        dbContext.UserTempValidations.Remove(tempCode);

        var isCompleted = await dbContext.SaveChangesAsync() > 0;

        return isCompleted 
            ? (true, null) 
            : (false, "We have encountered an error while verifying your email address!");
    }
    
    public async Task<(bool, string?)> VerifyPhoneAsync(
        Guid userId, 
        int verificationType,
        string userCode)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        var tempCode = await dbContext.UserTempValidations
            .SingleAsync(x => x.UserId == userId);

        var userWithCode = await dbContext.User
            .SingleAsync(user => user.Id == userId);

        // Ask user to request a new code if the code is expired
        if (tempCode.Expiry < DateTime.UtcNow)
        {
            dbContext.UserTempValidations.Remove(tempCode);
            await dbContext.SaveChangesAsync();
            return (false, "The code has expired, please request a new code!");
        }

        if (tempCode.TypeValidation != verificationType)
            return (false, "Code is not generated for Phone Verification!");

        if (userCode != tempCode.Code)
            return (false, "The entered code is Incorrect!");

        userWithCode.PhoneNumberConfirmed = true;
        dbContext.UserTempValidations.Remove(tempCode);

        var isCompleted = await dbContext.SaveChangesAsync() > 0;

        return isCompleted 
            ? (true, null) 
            : (false, "We have encountered an error while verifying your phone number!");
    }
    
    public async Task<bool> LockOutAsync(
        Guid targetUserId, 
        int days)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        var user = await dbContext.User
            .SingleOrDefaultAsync(x => x.Id == targetUserId);
        
        if (user == null)
            throw new UserNotFoundException();

        user.LockoutEnabled = true;
        user.LockoutEnd = DateTime.UtcNow.AddDays(days);

        return await dbContext.SaveChangesAsync() > 0;
    }
    
    public async Task<bool> UnLockAsync(Guid targetUserId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        var user = await dbContext.User
            .SingleOrDefaultAsync(x => x.Id == targetUserId);
        
        if (user == null)
            throw new UserNotFoundException();

        user.LockoutEnabled = false;
        user.LockoutEnd = null;

        return await dbContext.SaveChangesAsync() > 0;
    }
    
    public async Task<IEnumerable<string?>> GetSuperAdminEmailsAsync()
    {
        await using var dbContext = await _context.CreateDbContextAsync();
    
        var emails = await dbContext.Users
            .Join(dbContext.UserRoles, 
                user => user.Id, 
                userRole => userRole.UserId, 
                (user, userRole) => new { user, userRole })
            .Join(dbContext.Roles,
                userRole => userRole.userRole.RoleId,
                role => role.Id,
                (userRole, role) => new { userRole.user, role.Name })
            .Where(ur => ur.Name == SharedRolesVar.SuperAdmin && ur.user.Email != null) 
            .Select(ur => ur.user.Email)
            .ToListAsync();

        return emails.Where(email => email != null);
    }
    
    public async Task<bool> IsUserLockedAsync(
        Guid? userId = null, 
        string? email = null)
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        
        // Define the predicate for the query based on the provided IDs
        Expression<Func<User, bool>> predicate = user =>
            (userId.HasValue && user.Id == userId) ||
            (email != null && user.Email == email);

        var user = await dbContext.User
            .Where(predicate)
            .Select(x => new
            {
                x.LockoutEnabled,
                x.LockoutEnd
            })
            .SingleAsync();

        return user.LockoutEnabled && user.LockoutEnd >= DateTime.UtcNow;
    }
    
    public async Task<bool> IsFieldVerifiedAsync(
        Guid userId, 
        string fieldName)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        var user = await dbContext.User.FindAsync(userId);

        var propertyInfo = typeof(User).GetProperty(fieldName);

        return (bool) propertyInfo?.GetValue(user)!;
    }
    
    private static async Task<string?> UserRoleAsync(
        Guid userId, 
        ApplicationDbContext dbContext)
    {
        var roleName = await dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(dbContext.Roles.AsNoTracking(),
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => role.Name)
            .SingleOrDefaultAsync();

        return roleName;
    }
}
using AuctriaApplication.Infrastructure.Payment.Guard;
using AuctriaApplication.Infrastructure.Payment.Services.Abstract;
using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Infrastructure.Services;
using AuctriaApplication.Infrastructure.Services.Abstract;
using AuctriaApplication.Services.Membership.Services.Users.Abstract;
using AuctriaApplication.Services.MessagingAPI.Services.Email;
using AuctriaApplication.Services.MessagingAPI.Templates.Email;
using AuctriaApplication.Services.Payment.Dto;
using AuctriaApplication.Services.Payment.Services.Abstract;
using AuctriaApplication.Services.Store.Services.Abstract;

namespace AuctriaApplication.Infrastructure.Payment.Services;

public class PaymentManager : IPaymentManager
{
    private readonly IPaymentService _paymentService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;

    public PaymentManager(
        IPaymentService paymentService,
        IShoppingCartService shoppingCartService,
        IUserAccessor userAccessor,
        IUserService userService,
        IEmailService emailService)
    {
        _paymentService = paymentService;
        _shoppingCartService = shoppingCartService;
        _userAccessor = userAccessor;
        _userService = userService;
        _emailService = emailService;
    }

    public async Task<Result<string>> PayAsync(
        Guid shoppingCartId,
        UserCardInfoDto cardDto)
    {
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<string>.Failure("Sorry but your account is locked.");

        // Check if the card is expired
        if (PaymentGuard.IsCardExpired(cardDto.Month, cardDto.Year))
            return Result<string>.Failure("Sorry, but it seems like your card is expired!");

        // Check if Card Number is 16 digits
        if (!PaymentGuard.IsCardNumberValid(cardDto.CardNumber))
            return Result<string>.Failure("Card number should be 16 digits!");

        // Check if Cvv is 3 digits
        if (!PaymentGuard.IsCvvValid(cardDto.Cvv))
            return Result<string>.Failure("Cvv should be 3 digits!");

        // Check if ShoppingCart is available
        if (!await _shoppingCartService.IsShoppingCartAsync(shoppingCartId))
            return Result<string>.Failure("Sorry, but we could not find the shopping cart you are looking for.");

        // Check if user already paid the payment
        if (await _paymentService.IsPaidAsync(shoppingCartId))
            return Result<string>.Failure("You have already paid the payment for the items!");

        // Check for quantity of items and reduce them
        var (areItemsReduced, lowQuantityProducts) = await _shoppingCartService.AreItemsReducedAsync(shoppingCartId);
        if (!areItemsReduced)
            return Result<string>.Failure("Sorry, but it seems like we faced an issue while reducing the items.");

        // Get the total amount of the shopping cart
        var cost = await _shoppingCartService.GetCostAsync(shoppingCartId);

        // Process the payment
        var isPaid = await _paymentService.PayAsync(shoppingCartId, cardDto, cost);
        if (!isPaid)
            return Result<string>.Failure("Sorry, but it seems we faced an issue while processing the payment");

        // Inform user via email
        await _emailService.SendEmailAsync(
            _userAccessor.GetUserEmail(),
            EmailTemplate.SuccessfulPayment(_userAccessor.GetUserUsername()),
            "Order is now being processed");

        // Inform SuperAdmins if the items bought by the user has quantity of 10 or less
        if (lowQuantityProducts != null && !lowQuantityProducts.Any())
            return Result<string>.Success("Thanks for your payment!");

        var superAdminEmails = await _userService.GetSuperAdminEmailsAsync();
        var emailBody = CreateLowQuantityAlertEmail(lowQuantityProducts!);

        foreach (var email in superAdminEmails)
        {
            if (email is null)
                break;
            
            await _emailService.SendEmailAsync(email, emailBody, "Low Quantity Alert");
        }

        return Result<string>.Success("Thanks for your payment!");
    }

    private static string CreateLowQuantityAlertEmail(Dictionary<string, int> lowQuantityProducts)
    {
        var itemNames = lowQuantityProducts.Keys.ToArray();
        var quantities = lowQuantityProducts.Values.ToArray();

        // Call the EmailTemplate method to create the email body
        return EmailTemplate.QuantityAlert(itemNames, quantities);
    }
}
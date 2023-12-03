using AuctriaApplication.DataAccess.DbContext;
using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Domain.Helper;
using AuctriaApplication.Services.Payment.Dto;
using AuctriaApplication.Services.Payment.Exceptions;
using AuctriaApplication.Services.Payment.Services.Abstract;
using AuctriaApplication.Services.Payment.Services.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace AuctriaApplication.Services.Payment.Services;

public class PaymentService : IPaymentService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;
    private readonly string _paymentConfig;

    public PaymentService(
        IOptions<PaymentConfig> options,
        IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
        _paymentConfig = options.Value.SecretKey;
    }

    public async Task<bool> PayAsync(
        Guid shoppingCardId,
        UserCardInfoDto cardDto,
        decimal cost)
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            // Initialize Stripe configuration
            StripeConfiguration.ApiKey = _paymentConfig;

            // Create a token for the card
            var tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = cardDto.CardNumber,
                    ExpMonth = cardDto.Month,
                    ExpYear = cardDto.Year,
                    Cvc = cardDto.Cvv,
                    Name = cardDto.HolderName
                }
            };

            var tokenService = new TokenService();
            var stripeToken = await tokenService.CreateAsync(tokenOptions);

            // Create the charge using the token
            var chargeOptions = new ChargeCreateOptions
            {
                Amount = Convert.ToInt64(cost * 100),
                Currency = Currency.Usd.ToString(),
                Description = "For testing stuffs",
                Source = stripeToken.Id
            };

            var chargeService = new ChargeService();
            var charge = await chargeService.CreateAsync(chargeOptions);

            if (charge.Status != StringHelper.ConvertToLowerCaseNoSpaces(PaymentStatus.Succeeded.ToString()))
                throw new PaymentFailedException("Payment failed with status: " + charge.Status);

            // Save returned data into database
            dbContext.Payments.Add(new DataAccess.Entities.Stores.Payment
            {
                StripeChargeId = charge.Id,
                Amount = charge.Amount,
                Currency = Currency.Usd,
                PaymentStatus = EnumHelper.ConvertToEnum<PaymentStatus>(StringHelper.ConvertFirstLetterToUpper(charge.Status)),
                CustomerStripeId = charge.CustomerId,
                PaymentMethodDetails = charge.PaymentMethodDetails.ToString(),
                ReceiptUrl = charge.ReceiptUrl,
                ShoppingCartId = shoppingCardId
            });

            var isSaved = await dbContext.SaveChangesAsync() > 0;

            if (!isSaved)
                return false;

            await transaction.CommitAsync();
            return true;
        }
        catch (StripeException stripeEx)
        {
            await transaction.RollbackAsync();
            throw new PaymentFailedException(stripeEx.Message);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw new PaymentFailedException(e.ToString());
        }
    }

    public async Task<bool> IsPaidAsync(Guid shoppingCartId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        return await dbContext.Payments
            .AsNoTracking()
            .Select(x => new
            {
                x.ShoppingCartId,
                x.PaymentStatus
            })
            .AnyAsync(x => 
                x.ShoppingCartId == shoppingCartId && 
                x.PaymentStatus == PaymentStatus.Succeeded);
    }
}
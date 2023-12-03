using AuctriaApplication.Infrastructure.Payment.Services.Abstract;
using AuctriaApplication.Services.Payment.Dto;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Auctria_Application.Controllers.v1.PaymentControllers;

public class PaymentController : BaseApiController
{
    private readonly IPaymentManager _paymentManager;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IPaymentManager paymentManager,
        ILogger<PaymentController> logger)
    {
        _paymentManager = paymentManager;
        _logger = logger;
    }

    [HttpPost("Payment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Pay(Guid shoppingCartId, UserCardInfoDto cardInfoDto)
    {
        try
        {
            return HandleResult(await _paymentManager.PayAsync(shoppingCartId, cardInfoDto));
        }
        catch (StripeException stripeEx)
        {
            _logger.LogError($"We have faced an issue for stripe payment. Error: {stripeEx.Message}");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while processing the payment. Error: {e.Message}");
            throw;
        }
    }
    
}
using System.Text;

namespace AuctriaApplication.Services.MessagingAPI.Templates.Email;

public static class EmailTemplate
{
    private static string GetCommonStyles()
    {
        return @"
            <style>
                body {
                    font-family: Arial, sans-serif;
                    font-size: 16px;
                    line-height: 1.5;
                }
                .container {
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 20px;
                    background-color: #ffffff;
                    border: 1px solid #cccccc;
                }
                h1 {
                    font-size: 24px;
                    font-weight: bold;
                    margin-bottom: 20px;
                }
                .code {
                    font-size: 20px;
                    font-weight: bold;
                    color: #333333;
                }
                p {
                    margin-bottom: 10px;
                }
            </style>";
    }

    public static string Verification(string code, string name)
    {
        var commonStyles = GetCommonStyles();
        var emailBody = $@"
            <html>
            <head>
                {commonStyles}
            </head>
            <body>
                <div class='container'>
                    <h1>Email Verification Code</h1>
                    <p>Dear {name},</p>
                    <p>Thank you for signing up on Auctria. To verify your email address, please enter the following verification code:</p>
                    <p class='code'>{code}</p>
                    <p>This code is valid for the next 5 minutes. If the code expires, please request a new one.</p>
                </div>
            </body>
        </html>";

        return emailBody;
    }

    public static string SuccessfulPayment(string username)
    {
        var commonStyles = GetCommonStyles();
        var emailBody = $@"
            <html>
            <head>
                {commonStyles}
            </head>
            <body>
                <div class='container'>
                    <h1>We have received your payment</h1>
                    <p>Dear {username},</p>
                    <p>We have successfully placed your orders and received your payment.</p>
                    <p>Your order is not being processed!</p>
                </div>
            </body>
        </html>";

        return emailBody;
    }

    public static string QuantityAlert(string[] itemNames, int[] quantities)
    {
        var commonStyles = GetCommonStyles();
        var itemsListBuilder = new StringBuilder();

        for (var i = 0; i < itemNames.Length; i++)
        {
            itemsListBuilder.AppendLine($"<li>{itemNames[i]} - Quantity: {quantities[i]}</li>");
        }

        var emailBody = $@"
        <html>
        <head>
            {commonStyles}
        </head>
        <body>
            <div class='container'>
                <h1>Low quantity alert</h1>
                <p>Dear Admin,</p>
                <p>The following items have low quantities:</p>
                <ul>
                    {itemsListBuilder}
                </ul>
                <p>Please restock them as soon as possible.</p>
            </div>
        </body>
        </html>";

        return emailBody;
    }

}
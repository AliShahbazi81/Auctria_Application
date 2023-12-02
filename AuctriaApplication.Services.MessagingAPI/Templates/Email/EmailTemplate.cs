namespace AuctriaApplication.Services.MessagingAPI.Templates.Email;

public static class EmailTemplate
{
    public static string Verification(string code, string name)
    {
        var emailBody = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        font-size: 16px;
                        line-height: 1.5;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                        background-color: #ffffff;
                        border: 1px solid #cccccc;
                    }}
                    h1 {{
                        font-size: 24px;
                        font-weight: bold;
                        margin-bottom: 20px;
                    }}
                    .code {{
                        font-size: 20px;
                        font-weight: bold;
                        color: #333333;
                    }}
                    p {{
                        margin-bottom: 10px;
                    }}
                </style>
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
}
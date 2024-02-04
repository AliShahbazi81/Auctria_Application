<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Auctria Application README</title>
</head>
<body>
    <h1>Auctria Application</h1>
    <p>Auctria Application is an innovative e-commerce platform designed for auctioning products with a focus on efficiency and scalability. This exclusively backend application caters to both administrators managing the platform and users participating in auctions. It is built using a clean architecture that separates concerns, allowing for easy maintenance and scalability.</p>
    <h2>Key Features</h2>
    <ul>
        <li><strong>Product Management:</strong> Admins can add, update, and remove products, ensuring the auction inventory is always up to date.</li>
        <li><strong>Category Management:</strong> Organize products into categories for easier navigation and management.</li>
        <li><strong>Shopping Cart:</strong> Users can add products to a shopping cart, facilitating a seamless checkout process.</li>
        <li><strong>Payment Integration:</strong> Secure payment processing with integrated payment gateways.</li>
        <li><strong>User Management:</strong> Robust user management, including registration, authentication, and user roles.</li>
        <li><strong>Notification Service:</strong> Email and SMS notifications keep users informed about auction activities.</li>
    </ul>
    <h2>Architecture Overview</h2>
    <p>The Auctria Application architecture is built around several core components:</p>
    <ul>
        <li><strong>Data Access Layer:</strong> Manages data persistence and retrieval, interfacing with the database.</li>
        <li><strong>Domain Entities:</strong> Represents the business logic and rules of the application.</li>
        <li><strong>Service Layer:</strong> Encapsulates business logic, providing a clear separation from the presentation layer.</li>
        <li><strong>Controllers:</strong> Handle incoming HTTP requests and respond with the appropriate action.</li>
    </ul>
    <h2>Technologies Used</h2>
    <ul>
        <li><strong>Backend:</strong> .NET 5 for building RESTful APIs.</li>
        <li><strong>Database:</strong> Entity Framework Core as the ORM with SQL Server for data storage.</li>
        <li><strong>Authentication:</strong> ASP.NET Identity for user management and authentication.</li>
        <li><strong>Payment Gateway:</strong> Stripe API for handling payments.</li>
        <li><strong>Notifications:</strong> Twilio for SMS services and SMTP for email notifications.</li>
    </ul>
    <h2>Getting Started</h2>
    <h3>Prerequisites</h3>
    <ul>
        <li>.NET 5 SDK</li>
        <li>SQL Server</li>
    </ul>
    <h3>Setup and Installation</h3>
    <ol>
        <li>Clone the repository:
            <pre><code>git clone https://github.com/AliShahbazi81/Auctria_Application.git</code></pre>
        </li>
        <li>Navigate to the project directory and restore dependencies:
            <pre><code>dotnet restore</code></pre>
        </li>
        <li>Update the database:
            <pre><code>dotnet ef database update</code></pre>
        </li>
        <li>Start the application:
            <pre><code>dotnet run</code></pre>
        </li>
    </ol>
    <p>Note: This project is a backend-only application and does not include a frontend component.</p>
    <h2>Contributing</h2>
    <p>We welcome contributions to the Auctria Application! To contribute, please fork the repository, make your changes, and submit a pull request.</p>
    <h2>License</h2>
    <p>This project is licensed under the MIT License. See the LICENSE file for more details.</p>
</body>
</html>

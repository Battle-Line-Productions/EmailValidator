# Advanced Email Validator

Welcome to the Advanced Email Validator, a comprehensive email validation library built to provide a robust solution for validating email addresses. This library offers a variety of validations like Regex Validation, MX record Validation, Typo Detection, Disposable Email Detection, and much more.

## Features

- **Regex Validation**: Validates email addresses against standard and simple regex patterns. You also have an option to provide a custom regex pattern.
- **MX Record Validation**: Checks if the email domain has a valid MX record.
- **Typo Detection**: Checks for common typos in the email address based on a specific algorithm. Useful in detecting and suggesting corrections for user input.
- **Disposable Email Detection**: Checks if the email address belongs to a commonly known disposable email domain.

## Getting Started

### Installation

Firstly, add the NuGet package to your .NET project. In the NuGet package manager console, run:

```
Install-Package AdvancedEmailValidator
```

### Configuration

In your Startup.cs file, use the provided extension method to add the necessary services to your dependency injection container:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddEmailValidator();
    // Other services...
}
```

### Usage

Inject the `IEmailValidator` interface where you need to perform email validation:

```csharp
public class SomeService
{
    private readonly IEmailValidator _emailValidator;

    public SomeService(IEmailValidator emailValidator)
    {
        _emailValidator = emailValidator;
    }

    public async Task DoSomething(string email)
    {
        var validationResult = await _emailValidator.ValidateAsync(email);
        //...
    }
}
```

By default, all validations are enabled. You can customize this

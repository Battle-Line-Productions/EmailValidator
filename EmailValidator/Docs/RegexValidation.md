# Email Validation via Regex

We use two standard regex for validation to pass as many emails as possible while still satisfying security needs and malicious injection into strings.

## Simple Validator: 
```c#
Regex SimpleRegex = new("^[^\\s]+@[^\\s]+$", RegexOptions.IgnoreCase)
```
This validator provided the following checks:

1. At least one @ Sign
2. At least one non-whitespace character before the @ sign
3. At least one non-whitespace character after the @ sign

## Standard Validation

```c#
Regex Regex = new("^[^-.;<>'\"\\s]+(\\.[^-.;<>'\"\\s]+)*@[^-.;<>'\"\\s]+(\\.[^-.;<>'\"\\s]+)*$", RegexOptions.IgnoreCase);
```
This validator provided the following checks: 

1. At least one @ Sign
2. string before the @ sign does not begin with a period, string after the @ sign does not begin with a period
3. string before the @ sign will never contain two periods in a row, string after the @ sign will never contain two periods in a row
4. No whitespace characters are contains in the section before or after the @ sign
5. The email does not contain symbols ; < > ' "

## Customize regex validation to your needs

We also provided the ability for you to provide a custom validation regex to continue to meet your needs while using the functionality of the remainder of our package
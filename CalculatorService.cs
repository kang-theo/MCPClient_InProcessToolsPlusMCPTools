using System.ComponentModel;

public class CalculatorService
{
  [Description("Add two numbers together")]
  public double Add(
    [Description("The first number")] double a,
    [Description("The second number")] double b)
  {
    return a + b;
  }

  [Description("Subtract the second number from the first number")]
  public double Subtract(
    [Description("The first number")] double a,
    [Description("The second number")] double b)
  {
    return a - b;
  }

  [Description("Multiply two numbers together")]
  public double Multiply(
    [Description("The first number")] double a,
    [Description("The second number")] double b)
  {
    return a * b;
  }

  [Description("Divide the first number by the second number")]
  public double Divide(
    [Description("The dividend")] double a,
    [Description("The divisor")] double b)
  {
    if (b == 0)
      throw new ArgumentException("Cannot divide by zero", nameof(b));
    return a / b;
  }

  [Description("Calculate the square root of a number")]
  public double SquareRoot([Description("The number to calculate square root for")] double number)
  {
    if (number < 0)
      throw new ArgumentException("Cannot calculate square root of negative number", nameof(number));
    return Math.Sqrt(number);
  }

  [Description("Calculate the power of a number")]
  public double Power(
    [Description("The base number")] double baseNumber,
    [Description("The exponent")] double exponent)
  {
    return Math.Pow(baseNumber, exponent);
  }
}
class Day25 : IDayCommand
{
  public string Execute()
  {     
    var total = new FileReader(25).Read().Select(line => ConvertFromSNAFU2Decimal(line)).Sum();
    return $"You must supply {ConvertFromDecimal2SNAFU(total)} to the console";
  }

  public long ConvertFromSNAFU2Decimal(string snafu)
  {
    long decimalNumber = 0;
    var reversed = snafu.Reverse().ToArray();
    for (long exponent = 0; exponent < reversed.Length; exponent++)
    {
      var numberBase = reversed[exponent] switch {
        '-' => -1,
        '=' => -2,
        _ => (long) char.GetNumericValue(reversed[exponent])
      };
      decimalNumber += (long) Math.Pow(5, exponent) * numberBase;
    }
    return decimalNumber;
  }

  public string ConvertFromDecimal2SNAFU(long decimalNumber)
  {
    var result = string.Empty;
    var number = decimalNumber;
    while(number != 0)
    {
      var remaining = number % 5;
      number = number / 5;
      switch (remaining)
      {
        case 0:
          result += "0";
          break;
        case 1:
          result += "1";
          break;
        case 2:
          result += "2";
          break;
        case 3:
          result += "=";
          number += 1;
          break;
        case 4:
          result += "-";
          number += 1;
          break;
        default:
          throw new ArithmeticException();
      }
    }
    return string.Join("", result.Reverse());
  }
}

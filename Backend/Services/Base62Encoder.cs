namespace Backend.Services;

public static class Base62Encoder
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static string Encode(long value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Value must not be negative.");
        }

        if (value == 0)
        {
            return Alphabet[0].ToString();
        }

        var chars = new Stack<char>();
        while (value > 0)
        {
            chars.Push(Alphabet[(int)(value % 62)]);
            value /= 62;
        }

        return new string(chars.ToArray());
    }

    public static bool TryDecode(string encoded, out long value)
    {
        value = 0;

        if (string.IsNullOrEmpty(encoded))
        {
            return false;
        }

        try
        {
            checked
            {
                long result = 0;
                foreach (var c in encoded)
                {
                    var digit = Alphabet.IndexOf(c);
                    if (digit < 0)
                    {
                        return false;
                    }

                    result = result * 62 + digit;
                }

                value = result;
                return true;
            }
        }
        catch (OverflowException)
        {
            value = 0;
            return false;
        }
    }
}

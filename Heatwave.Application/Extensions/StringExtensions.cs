using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Heatwave.Application.Extensions;
public static class StringExtensions
{
    // <summary>
    /// Is it a non whitespace character
    /// </summary>
    public static bool IsNotEmpty(this string? @this)
    {
        return @this != "";
    }
    /// <summary>
    /// Is it not a NULL or whitespace character
    /// </summary>
    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this string? @this)
    {
        return !string.IsNullOrEmpty(@this);
    }
    /// <summary>
    /// Is it not a NULL or whitespace character
    /// </summary>
    public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
    /// <summary>
    /// Is the List empty or without data
    /// </summary>
    public static bool IsNotNullOrAny<TSource>([NotNullWhen(true)] this IEnumerable<TSource>? source)
    {
        return source != null && source.Any();
    }

    /// <summary>
    /// Does it contain a string
    /// </summary>
    public static bool Contains(this string @this, string value)
    {
        return @this.IndexOf(value) != -1;
    }
    /// <summary>
    /// Does it include features
    /// </summary>
    public static bool Contains(this string @this, string value, StringComparison comparisonType)
    {
        return @this.IndexOf(value, comparisonType) != -1;
    }

    /// <summary>
    /// Is it an integer
    /// </summary>
    public static bool IsNumeric(this string @this)
    {
        return !Regex.IsMatch(@this, "[^0-9]");
    }

    /// <summary>
    /// Returns the normalized attribute name
    /// </summary>
    public static string? GetPropertyName<T>(this string? @this)
    {
        if (string.IsNullOrWhiteSpace(@this))
        {
            return null;
        }
        var propertyInfo = typeof(T).GetProperty(@this.Trim(), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo != null)
        {
            return propertyInfo.Name;
        }
        return null;
    }


    /// <summary>
    /// Returns the comma concatenation of sorted field names
    /// </summary>
    public static string CreateSort<T>(this string? @this, string? orderby)
    {
        if (string.IsNullOrWhiteSpace(@this))
        {
            return "-Id";
        }
        var sort = string.Empty;

        var fieldsAfterSplit = @this.Split(',');
        foreach (var field in fieldsAfterSplit)
        {
            var propertyName = field.Trim();
            var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                continue;
            }

            sort += orderby == "desc" ? $"-{propertyInfo.Name}," : $"{propertyInfo.Name},";
        }
        return sort.IsNotNullOrEmpty() ? sort.TrimEnd(',') : "-Id";
    }

    public static IReadOnlyList<string> ToList(this string? str, char separator = ',')
    {
        if (str.IsNullOrEmpty())
            return [];
        return str!.Split(separator).ToList();
    }

    /// <summary>
    /// Encrypted String
    /// </summary>
    /// <param name="nums"></param>
    /// <returns></returns>
    public static string EncryptionNum(string nums)
    {
        if (nums.Length > 4)
        {
            string hideNum = nums.Substring(nums.Length / 4, (nums.Length * 3 / 4) - (nums.Length / 4) + 1);
            string Asterisk = "";
            for (int i = 0; i < hideNum.Length; i++)
            {
                Asterisk += "*";
            }
            nums = nums.Substring(0, nums.Length / 4) + Asterisk + nums.Substring((nums.Length * 3 / 4) + 1, nums.Length - (nums.Length * 3 / 4) - 1);
            return nums;
        }
        else
        {
            return nums;
        }
    }

    public static string EncodeMD5(this string str)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(str);
        var md5Array = md5.ComputeHash(bytes);
        StringBuilder sb = new StringBuilder();
        foreach (var item in md5Array)
        {
            sb.Append(item.ToString("X2"));
        }
        return sb.ToString();
    }
}

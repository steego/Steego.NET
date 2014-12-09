using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Steego.Conversions {
  public static class StringExtensions {
    private static Regex EmailRegex = new Regex("^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,4}$", RegexOptions.IgnoreCase);
    private static Regex LeadsWithIntPattern = new Regex("^\\s*\\d+");

    public static bool IsBlank(this string value) {
      return string.IsNullOrEmpty(value);
    }

    public static bool IsNotBlank(this string value) {
      return !string.IsNullOrEmpty(value);
    }

    public static string IfBlank(this string val, string defaultVal) {
      return val.IsBlank() ? defaultVal : val;
    }

    public static string Abbreviate(this string value, int maxLength) {
      value = value.IfBlank("");
      if(value.Length <= maxLength) return value;
      return value.Substring(0, maxLength);
    }

    public static string Abbreviate(this string value, int maxLength, string suffix) {
      value = value.IfBlank("");
      if(value.Length <= maxLength) return value;
      return value.Abbreviate(maxLength + suffix.Length) + suffix;
    }

    public static string Before(this string val, string find) {
      if(val == null) return null;
      var length = val.IndexOf(find);
      if(length >= 0) return val.Left(length);
      return val;
    }

    public static string After(this string val, string find) {
      if(val == null)
        return "";
      int length = find.Length;
      int num = val.IndexOf(find);
      if(num < 0) return "";
      return val.Mid(num + length + 1);
    }

    public static string Left(this string str, int length) {
      if(length < 0) {
        throw new ArgumentException("Length needs to be greater than 0");
      }
      if(length == 0 || str == null) return "";
      if(length >= str.Length) return str;
      return str.Substring(0, length);
    }

    public static string Mid(this string str, int start) {
      return str.Mid(start, str.Length);
    }

    public static string Mid(this string str, int start, int length) {
      if(start <= 0) throw new ArgumentException("Start cannot be less than 0");
      if(length < 0) throw new ArgumentException("Length needs to be greater than 0");

      if(length == 0 || str == null) return "";
      var strLength = str.Length;
      if(start > strLength) return "";
      if(checked(start + length) > strLength)
        return str.Substring(checked(start - 1));
      return str.Substring(checked(start - 1), length);
    }

    public static string Right(this string str, int length) {
      if(length < 0) throw new ArgumentException("Length needs to be greater than 0");

      if(length == 0 || str == null) return "";
      var strLength = str.Length;
      if(length >= strLength) return str;
      return str.Substring(checked(strLength - length), length);
    }

    public static string AfterLast(this string val, string find) {
      if(val == null) return "";
      int length = find.Length;
      int num = val.LastIndexOf(find);
      if(num >= 0)
        return val.Mid(num + length + 1);
      return "";
    }

    public static string ReplaceExp(this string input, string pattern, string replacement) {
      return Regex.Replace(input, pattern, replacement);
    }

    public static string LCase(this string input) {
      return input.IfBlank("").ToLower();
    }

    public static string UCase(this string input) {
      return input.IfBlank("").ToUpper();
    }

    public static string PCase(this string input) {
      if(input == null) return null;
      return Regex.Replace(input, "\\b([\\w'])([\\w']*)", (m => m.Groups[1].Value.ToUpper() + m.Groups[2].Value.ToLower()));
    }

    public static bool IsValidEmail(this string email) {
      return email.IsNotBlank() && EmailRegex.IsMatch(email.Trim());
    }
  }
}

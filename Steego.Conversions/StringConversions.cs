using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steego.Conversions {
  public static class StringConversions {
    public static int? ToInt(this string input) {
      int intValue;
      if(Int32.TryParse(input, out intValue)) return intValue;
      return null;
    }

    public static int ToInt(this string input, int defaultValue) {
      int intValue;
      if(Int32.TryParse(input, out intValue)) return intValue;
      return defaultValue;
    }

    public static long? ToLong(this string input) {
      Int64 intValue;
      if(Int64.TryParse(input, out intValue)) return intValue;
      return null;
    }

    public static long ToLong(this string input, long defaultValue) {
      Int64 intValue;
      if(Int64.TryParse(input, out intValue)) return intValue;
      return defaultValue;
    }

    public static Guid? ToGuid(this string input) {
      Guid guidValue;
      if(Guid.TryParse(input, out guidValue)) return guidValue;
      return null;
    }

    public static DateTime? ToDate(this string input) {
      DateTime date;
      if(DateTime.TryParse(input, out date)) return date;
      return null;
    }

    public static DateTime? ToDate(this string input, DateTime defaultDate) {
      DateTime date;
      if(DateTime.TryParse(input, out date)) return date;
      return defaultDate;
    }

    public static double? ToDbl(this string value) {
      double returnValue;
      return Double.TryParse(value, out returnValue) ? returnValue : new Nullable<double>();
    }

    public static double ToDbl(this string value, double defaultValue) {
      double returnValue;
      return Double.TryParse(value, out returnValue) ? returnValue : defaultValue;
    }

    public static decimal? ToDec(this string value) {
      decimal returnValue;
      return Decimal.TryParse(value, out returnValue) ? returnValue : new Nullable<decimal>();
    }

    public static decimal ToDec(this string value, decimal defaultValue) {
      decimal returnValue;
      return Decimal.TryParse(value, out returnValue) ? returnValue : defaultValue;
    }

    public static bool ToBool(this string s) {
      bool result;
      return (Boolean.TryParse(s, out result)) ? result : false;
    }

    public static T ParseEnum<T>(this string text, T defaultValue) {
      T obj;
      try {
        obj = (T) Enum.Parse(typeof(T), text, true);
      } catch(Exception ex) {
        obj = defaultValue;
      }
      return obj;
    }
  }
}

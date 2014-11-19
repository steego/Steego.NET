﻿using System;
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

    public static double ToDbl(this string value, double defaultValue) {
      double returnValue;
      return Double.TryParse(value, out returnValue) ? returnValue : defaultValue;
    }

    public static bool ToBool(this string s) {
      bool result;
      return (Boolean.TryParse(s, out result)) ? result : false;
    }
  }
}

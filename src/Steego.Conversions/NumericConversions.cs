using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steego.Conversions {
  public static class NumericConversions {
    public static int ToInt(this double value) {
      return checked((int) Math.Round(value));
    }

    public static int? ToInt(this decimal? value) {
      if(!value.HasValue) {
        return null;
      } 
      return Convert.ToInt32(Math.Round(value.Value));
    }
  }


}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Steego.Conversions {
  public static class XmlExtensions {

    public static string GetAttribute(this XElement e, string name) {
      var a = e.Attribute(name);
      if(a == null)
        return null;
      return a.Value;
    }

    public static string GetAttribute(this XElement e, params string[] names) {
      return (from name in names
              let a = e.Attribute(name)
              where a != null
              select a.Value).FirstOrDefault();
    }

    public static XDocument TryParse(this string xmlText) {
      XDocument doc;
      try {
        doc = XDocument.Parse(xmlText);
      } catch(Exception) {
        return null;
      }
      return doc;
    }

    public static DateTime? ParseDate(this string Date) {
      try {
        string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt", "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss", 
                           "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt", "M/d/yyyy h:mm", "M/d/yyyy h:mm", 
                           "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm", "yyyy-MM-ddTHH:mm:ss:fff", "yyyy-MM-ddTHH:mm:ss","O","u"};

        return DateTime.ParseExact(Date, formats, new CultureInfo("en-US"), DateTimeStyles.None);
      } catch(Exception) {
        return null;
      }
    }

    public static string SerializeWithDC<T>(this T o) {
      if(o == null) return null;
      var ds = new DataContractSerializer(typeof(T));
      using(var sw = new StringWriter()) {
        using(var xw = new XmlTextWriter(sw)) {
          ds.WriteObject(xw, o);
          return sw.ToString();
        }
      }
    }

    public static T ReadWithDC<T>(this string input) {
      if(input == null) return default(T);
      var ds = new DataContractSerializer(typeof(T));
      using(var newStringReader = new StringReader(input)) {
        using(var xr = new XmlTextReader(newStringReader)) {
          return (T) ds.ReadObject(xr);
        }
      }
    }

  }
}

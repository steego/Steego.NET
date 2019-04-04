module Steego.LinqPad.Html.Utilities

open System.IO
open System.Text.RegularExpressions


let internal cssProp (html:TextWriter) (key: string) (value: obj) =
  html.Write key
  html.Write ':'

let private slugRegex = Regex("([A-Z])", RegexOptions.Compiled)
let inline internal slugKey key =
  slugRegex.Replace(string key, "-$1").ToLower()


// Adapted from https://github.com/facebook/react/blob/37e4329bc81def4695211d6e3795a654ef4d84f5/packages/react-dom/src/server/escapeTextForBrowser.js#L49
let escapeHtml (sb:TextWriter) (str: string) =
  if isNull str then () else
  for c in str.ToCharArray() do
    match c with
    | '"' -> sb.Write("&quot")
    | '&' -> sb.Write("&amp;")
    | ''' -> sb.Write("&#x27;") // modified from escape-html; used to be '&#39'
    | '<' -> sb.Write("&lt;")
    | '>' -> sb.Write("&gt;")
    | c   -> sb.Write(c)

let inline boolAttr (html:TextWriter) (key: string) (value: bool) =
  if value then html.Write key

let inline strAttr (html:TextWriter) (key: string) (value: string) =
  html.Write key
  html.Write "=\""
  escapeHtml html value
  html.Write '"'

let inline objAttr (html:TextWriter) (key: string) (value: obj) = strAttr html key (string value)

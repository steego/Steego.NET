module Steego.Fs.WinDump.Browser

open Steego.Fs
open System
open System.Windows.Forms

type Browser() = 
    let form = new Form (Visible = true, Width = 400, Height = 768, 
                        Location = Drawing.Point (0, 0), Text = "Browser",
                        TopMost = true)
    let browser = new WebBrowser (Dock = DockStyle.Fill)
    let bootStrap = 
        "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/css/bootstrap.min.css"
    let fillTemplate content = 
        Html.BasicTemplates.cssPage [bootStrap] [ content ]
    do
        form.Controls.Add browser
        form.Show()
        browser.Url <- Uri("about:blank")
    member this.SetTopMost() = form.TopMost <- true; this
    member this.SetHtml(html) = browser.Document.Body.InnerHtml <- html; this
    member this.Dump<'a>(value:'a) = 
        value |> Printer.printHtml 3 
              |> fillTemplate 
              |> fun h -> h.ToString()
              |> this.SetHtml
        //value |> Printer.print 3 |> this.SetHtml
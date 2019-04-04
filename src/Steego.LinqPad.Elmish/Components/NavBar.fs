module Steego.LinqPad.Elmish.Components.NavBar

open Steego.LinqPad.Elmish
open Steego.LinqPad.Elmish.Props
open Steego.LinqPad.Elmish.Helpers   

let greeting name = 
    div [] [ str("Hello "); strong[] [ str(name) ] ]

let navBar = 
    nav [ Class "navbar navbar-expand-lg navbar-light bg-light" ]
        [ a [ Class "navbar-brand"
              Href "#" ]
            [ str "Navbar" ]
          button [ Class "navbar-toggler"
                   Type "button"
                   DataToggle "collapse"
                   HTMLAttr.Data ("target", "#navbarSupportedContent")
                   HTMLAttr.Custom ("aria-controls", "navbarSupportedContent")
                   AriaExpanded false
                   HTMLAttr.Custom ("aria-label", "Toggle navigation") ]
            [ span [ Class "navbar-toggler-icon" ]
                [ ] ]
          div [ Class "collapse navbar-collapse"
                Id "navbarSupportedContent" ]
            [ ul [ Class "navbar-nav mr-auto" ]
                [ li [ Class "nav-item active" ]
                    [ a [ Class "nav-link"
                          Href "#" ]
                        [ str "Home"
                          span [ Class "sr-only" ]
                            [ str "(current)" ] ] ]
                  li [ Class "nav-item" ]
                    [ a [ Class "nav-link"
                          Href "#" ]
                        [ str "Link" ] ]
                  li [ Class "nav-item dropdown" ]
                    [ a [ Class "nav-link dropdown-toggle"
                          Href "#"
                          Id "navbarDropdown"
                          Role "button"
                          DataToggle "dropdown"
                          AriaHasPopup true
                          AriaExpanded false ]
                        [ str "Dropdown" ]
                      div [ Class "dropdown-menu"
                            HTMLAttr.Custom ("aria-labelledby", "navbarDropdown") ]
                        [ a [ Class "dropdown-item"
                              Href "#" ]
                            [ str "Action" ]
                          a [ Class "dropdown-item"
                              Href "#" ]
                            [ str "Another action" ]
                          div [ Class "dropdown-divider" ]
                            [ ]
                          a [ Class "dropdown-item"
                              Href "#" ]
                            [ str "Something else here" ] ] ]
                  li [ Class "nav-item" ]
                    [ a [ Class "nav-link disabled"
                          Href "#" ]
                        [ str "Disabled" ] ] ]
              form [ Class "form-inline my-2 my-lg-0" ]
                [ input [ Class "form-control mr-sm-2"
                          Type "search"
                          Placeholder "Search"
                          HTMLAttr.Custom ("aria-label", "Search") ]
                  button [ Class "btn btn-outline-success my-2 my-sm-0"
                           Type "submit" ]
                    [ str "Search" ] ] ] ]
module Steego.Web.Server

//  Encapsulate everything in 
//open Suave
//
//let private started = ref false
//let private mainWebPart = ref (Successful.OK "Welcome")
//
//let update(newPart) = lock mainWebPart (fun () -> mainWebPart := newPart)
//
//let start() = 
//    if not started.Value then
//        lock started (fun () -> started := true)
//        async {
//                startWebServer defaultConfig (fun(ctx : HttpContext) ->
//                    async {
//                        let part = lock mainWebPart (fun () -> mainWebPart.Value)
//                        return! part ctx
//                    })
//        } |> Async.Start
//
//start()

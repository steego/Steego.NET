//  An experiment in combining recursive functions

module Steego.Rules

type Rule<'a,'b> = ('a -> 'b) -> 'a -> 'b option

let rule<'a,'b>(r:Rule<'a,'b>) = r

let rec combineMax(max:int)(defaultRule:'a -> 'b)(allRules:Rule<'a,'b> list)(o:'a) = 
    if max = 0 then defaultRule o
    else 
        let recurse = combineMax (max - 1) defaultRule allRules
        let result = allRules |> List.tryPick(fun rule -> rule recurse o)
        match result with
        | Some(result) -> result
        | None -> defaultRule(o)

let rec combine (defaultRule:'a -> 'b) (allRules:Rule<'a,'b> list) = 
    fun (o:'a) ->
        let recurse = combine defaultRule allRules
        let result = allRules |> List.tryPick(fun rule -> rule recurse o)
        match result with
        | Some(result) -> result
        | None -> defaultRule(o)
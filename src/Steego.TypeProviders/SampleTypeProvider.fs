namespace Steego.TypeProviders

open System
open System.Reflection
open ProviderImplementation.ProvidedTypes
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations

// This type defines the type provider. When compiled to a DLL, it can be added
// as a reference to an F# command-line compilation, script, or project.
[<TypeProvider>]
type StringTypeProvider(config: TypeProviderConfig) as this = 

    // Inheriting from this type provides implementations of ITypeProvider 
    // in terms of the provided types below.
    inherit TypeProviderForNamespaces()

    let namespaceName = "Steego.TypeProviders"
    let thisAssembly = Assembly.GetExecutingAssembly()
    let staticParams = [ProvidedStaticParameter(parameterName="value", 
                                                parameterType=typeof<string>, 
                                                parameterDefaultValue = "")]

//    let buildType (className:string) =
//        let t = ProvidedTypeDefinition(thisAssembly, namespaceName, 
//                                       className=className, baseType=Some typeof<obj>)

//        do t.DefineStaticParameters(
//            parameters = staticParams,
//            instantiationFunction = (fun typeName paramValues ->
//                match paramValues with
//                | [| :? string as value |] -> 
//
//                    let ty = ProvidedTypeDefinition(assembly=thisAssembly, namespaceName=namespaceName, 
//                                                    className=typeName, baseType=Some typeof<obj>)
//                    let ctor = ProvidedConstructor(parameters=[], InvokeCode=fun args -> <@@ value :> obj @@>)
//                    ctor.AddXmlDoc "Initialize the awesomes"
//                    ty.AddMember ctor
//                    let lengthProp = ProvidedProperty("Length", typeof<int>, GetterCode = fun args -> <@@ value.Length @@>)
//                    ty.AddMember lengthProp
//
//                    let charProps = [ for c in value do
//                                        let p = ProvidedProperty(
//                                                    c.ToString(),
//                                                    typeof<char>,
//                                                    GetterCode = fun args -> <@@ c @@>
//                                                )
//                                        let doc = sprintf "The char %s" (c.ToString())
//                                        p.AddXmlDoc doc
//                                        yield p ]
//                    ty.AddMembersDelayed (fun () -> charProps)
//
//                    ty
//                | _ -> failwith "Not supported"
//            )
//        )
//
//        t
    
    let buildType(className:string) = 
        let t = ProvidedTypeDefinition(assembly=thisAssembly,namespaceName=namespaceName,className=className,
                                         baseType = Some typeof<obj>)
        t.AddXmlDocDelayed (fun () -> sprintf "This provided type %s" (className))

        let staticProp = ProvidedProperty(propertyName="StaticProperty", propertyType=typeof<string>,
                                          IsStatic=true,GetterCode=(fun args -> <@@ className @@>))
        staticProp.AddXmlDocDelayed(fun () -> "This is a static property")
        t.AddMember staticProp
        let ctor = ProvidedConstructor(parameters = [ ], 
                               InvokeCode= (fun args -> <@@ "The object data" :> obj @@>))
        ctor.AddXmlDocDelayed(fun () -> "This is a constructor")
        t.AddMember ctor

        t

    //do t.DefineStaticParameters(parameters, buildType)

    // And add them to the namespace
    let t = buildType("TestClass")
    do this.AddNamespace(namespaceName, [t])

[<assembly:TypeProviderAssembly>] 
do()
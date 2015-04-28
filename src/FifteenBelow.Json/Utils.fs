namespace FifteenBelow.Json

open System
open System.Linq
open System.Collections.Generic
open System.Reflection
open Newtonsoft.Json
open Newtonsoft.Json.Converters
open Newtonsoft.Json.Serialization


[<RequireQualifiedAccess>]
module Utils =

    let private flags = BindingFlags.NonPublic ||| BindingFlags.Static
    let private info = typeof<DefaultContractResolver>.GetMember ("BuiltInConverters", flags)
    let private converters =
         match (info.[0] :?> FieldInfo).GetValue (null) with
         | :? List<JsonConverter> as c -> c
         | x -> new ResizeArray<JsonConverter>()


    let uninstallDefaultConverter (converter: Type) =
        match converters.FirstOrDefault (fun x -> x.GetType () = converter) with
        | null -> ()
        | x -> converters.Remove (x) |> fun _ -> ()

    let uninstallDefaultUnionConverter () =
        uninstallDefaultConverter (typeof<DiscriminatedUnionConverter>)

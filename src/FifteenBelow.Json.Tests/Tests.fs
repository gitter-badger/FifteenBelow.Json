module FifteenBelow.Json.Tests.Converters


open FifteenBelow.Json
open NUnit.Framework
open System.Collections.Generic
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
let converters =
    [ OptionConverter () :> JsonConverter
      TupleConverter () :> JsonConverter
      ListConverter () :> JsonConverter
      MapConverter () :> JsonConverter
      BoxedMapConverter () :> JsonConverter
      UnionConverter () :> JsonConverter ] |> List.toArray :> IList<JsonConverter>

let settings =
    JsonSerializerSettings (
        ContractResolver = CamelCasePropertyNamesContractResolver (), 
        Converters = converters,
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore)


type MyDU = MyCase of int[]

[<Test>]
let ``a DU with a single array parameter should work`` () =
    let initial = MyCase [|1;2;3|]
    let x = JsonConvert.SerializeObject(initial, settings)
    let result = JsonConvert.DeserializeObject<MyDU>(x, settings)
    Assert.AreEqual(initial, result)
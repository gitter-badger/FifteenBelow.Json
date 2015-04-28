module FifteenBelow.Json.Tests.Converters


open NUnit.Framework
open System.Collections.Generic
open FifteenBelow.Json
open Newtonsoft.Json
open Newtonsoft.Json.Serialization

let converters =
    [ OptionConverter () :> JsonConverter
      TupleConverter () :> JsonConverter
      ListConverter () :> JsonConverter
      MapConverter () :> JsonConverter
      BoxedMapConverter () :> JsonConverter
      UnionConverter () :> JsonConverter ] 
    |> fun x -> ResizeArray<JsonConverter>(x) 
    :> IList<JsonConverter>

let settings =
    JsonSerializerSettings (
        ContractResolver = CamelCasePropertyNamesContractResolver (), 
        Converters = converters,
        Formatting = Formatting.None,
        NullValueHandling = NullValueHandling.Ignore)

type Unions =
    | One
    | Two of string
    | Rec of Unions
    | Tup of int * string
    | Arr of int []

let areEqual expected actual =
    if expected = actual then ()
    else failwith (sprintf "expected %A\r\nBut was: %A" expected actual)

let roundtrip<'T when 'T : equality> (o : 'T) =
    let json = JsonConvert.SerializeObject(o, settings)
    let result = JsonConvert.DeserializeObject<'T>(json, settings)
    areEqual o result

[<Test>]
let ``empty array`` () = [||] |> roundtrip
[<Test>]
let ``nested array`` () = [|[|1|]|] |> roundtrip
[<Test>]
let ``really nested array`` () = [|[|[|1|]|]|] |> roundtrip
[<Test>]
let ``int array`` () = [|1;2;3|] |> roundtrip
[<Test>]
let ``int array option`` () = Some [|1;2;3|] |> roundtrip

[<Test>]
let ``empty list`` () = [] |> roundtrip
[<Test>]
let ``int list`` () = [1;2;3] |> roundtrip
[<Test>]
let ``DU list`` () = [One;Two "hey";Rec One;Arr [|1;2;3|]] |> roundtrip
[<Test>]
let ``int array list`` () = [[|1;2;3|]] |> roundtrip
[<Test>]
let ``empty int array list`` () = [[||]] |> roundtrip

[<Test>]
let ``DU case with a single array parameter`` () = Arr [|1;2;3|] |> roundtrip
[<Test>]
let ``DU case with a single empty array parameter`` () = Arr [||] |> roundtrip
[<Test>] 
let ``empty DU case`` () = One |> roundtrip 
[<Test>] 
let ``DU with string parameter`` () = Two "test" |> roundtrip 
[<Test>] 
let ``DU with empty string parameter`` () = Two "" |> roundtrip 
[<Test>] 
let ``DU with null string parameter`` () = Two null |> roundtrip 
[<Test>] 
let ``DU recursive parameter`` () = Rec (Two "test") |> roundtrip 

[<Test>] 
let ``tuple`` () = Tup (1, "test") |> roundtrip 

type ARec =
    { DU : Unions
      S : string
      ARec : ARec option
      Arr : int [] }

let def =
    { DU = One
      S = "test"
      ARec = None
      Arr = [||] }

[<Test>] 
let ``default record`` () = def |> roundtrip 
[<Test>] 
let ``recursive record`` () = { def with ARec = Some def } |> roundtrip 

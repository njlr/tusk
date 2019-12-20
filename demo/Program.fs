module Tusk.Demo

open Fable
open Fable.Core

type MyRecord = JsonProvider.Generator<"""{
  "x": 123,
  "y": "abc"
}""">

[<EntryPoint>]
let program args =
  promise {
    let db = createDatabase InitializationOptions.defaults "postgres://postgres@localhost:5432/postgres"

    do! db |> connect

    let! records =
      db
      |> query<MyRecord, _>
        ("""
          SELECT 1 AS x, 'abc' AS y
          UNION ALL
          SELECT 2 AS x, 'def' AS y
          UNION ALL
          SELECT 3 AS x,  $<y> AS y
        """)
        {| y = "xxx" |}

    JS.console.log (records)
  }
  |> Promise.catch (fun e -> printfn "%O" e)
  |> Promise.start

  0

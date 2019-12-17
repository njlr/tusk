module Tusk.Demo

open Fable
open Fable.Core
open Tusk

type MyRecord = JsonProvider.Generator<"""{
  "x": 123,
  "y": "abc"
}""">

[<EntryPoint>]
let program args =
  promise {
    let db = createDatabase InitializationOptions.defaults "postgres://postgres@localhost:5432/postgres"

    do! db.Connect ()

    let! records = db.Query<MyRecord> ("""
      SELECT 1 AS x, 'abc' AS y
      UNION ALL
      SELECT 2 AS x, 'def' AS y
      UNION ALL
      SELECT 3 AS x, 'ghi' AS y
    """)

    JS.console.log (records)
  }
  |> Promise.start

  0

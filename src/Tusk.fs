module Tusk

open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.JS

type InitializationOptions =
  {
    PgFormatting : bool
    CapSQL : bool
  }

module InitializationOptions =

  let defaults =
    {
      PgFormatting = false
      CapSQL = false
    }

// Interface for the pg-promise database object
type private IDatabase =
  abstract connect : Unit -> Promise<Unit>

  abstract query : string -> obj -> Promise<ResizeArray<obj>>

  abstract one : string -> Promise<obj>

  abstract oneOrNone : string -> Promise<obj>

[<Import("*", "pg-promise")>]
let private pgp : obj -> string -> IDatabase = jsNative

[<Emit("(x => x)")>]
let private magicCast<'T> : obj -> 'T = jsNative

type DatabaseWrapper =
  private {
    Database : IDatabase
  }

let createDatabase initializationOptions connectionString =
  let opts =
    createObj
      [
        "pgFormatting" ==> initializationOptions.PgFormatting
        "capSQL" ==> initializationOptions.CapSQL
      ]

  let db = pgp opts connectionString

  { Database = db }

let connect db = promise {
  do! db.Database.connect ()
}

let query<'TParams, 'TRow> sql p db = promise {
  let! records = db.Database.query sql p

  return
    records
    |> magicCast<ResizeArray<'TRow>>
    // |> List.ofSeq
}

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
type IDatabase =
  abstract connect : Unit -> Promise<Unit>

  abstract query : string -> Promise<ResizeArray<obj>>

  abstract one : string -> Promise<obj>

  abstract oneOrNone : string -> Promise<obj>

[<Import("*", "pg-promise")>]
let private pgp : obj -> string -> IDatabase = jsNative

[<Emit("(x => x)")>]
let private magicCast<'T> : obj -> 'T = jsNative

[<Emit("$1.map($0)")>]
let mapArray (_ : 'T -> 'U) (_ : ResizeArray<'T>) : ResizeArray<'U> = jsNative

type Database (db : IDatabase) =

  member this.Connect () =
    db.connect ()

  member this.Query<'RowType> (sql : string) =
    promise {
      let! records = db.query sql

      return
        records
        |> mapArray magicCast<'RowType>
        // |> List.ofSeq
    }

let createDatabase initializationOptions connectionString =
  let opts =
    createObj
      [
        "pgFormatting" ==> initializationOptions.PgFormatting
        "capSQL" ==> initializationOptions.CapSQL
      ]

  let db = pgp opts connectionString

  Database db

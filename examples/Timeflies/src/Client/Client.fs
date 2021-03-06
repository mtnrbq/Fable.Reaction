module Client

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fable.Reaction
open Reaction
open Elmish
open Elmish.React

// The model holds data that you want to keep track of while the
// application is running
type Model = {
    Letters: Map<int, string * int * int>
}

// The Msg type defines what events/actions can occur while the
// application is running. The state of the application changes *only*
// in reaction to these events
type Msg =
    | Letter of int * string * int * int

// The update function computes the next state of the application based
// on the current state and the incoming messages
let update (msg : Msg) (currentModel : Model) : Model =
    match currentModel.Letters, msg with
    | _, Letter (i, c, x, y) ->
        { currentModel with Letters = currentModel.Letters.Add (i, (c, x, y)) }

let view (model : Model) (dispatch : Dispatch<Msg>) =
    let letters = model.Letters
    let offsetX x i = x + i * 10 + 15

    div [ Style [ FontFamily "Consolas, monospace"]] [
        for KeyValue(i, (c, x, y)) in letters do
            yield span [ Style [Top y; Left (offsetX x i); Position "absolute"] ] [
                str c
            ]
    ]

let init () : Model =
    { Letters = Map.empty }

// Query for message stream transformation (same as below)
let query' msgs =
    Seq.toList "TIME FLIES LIKE AN ARROW"
    |> Seq.mapi (fun i c -> i, c)
    |> ofSeq
    |> flatMap (fun (i, c) ->
        fromMouseMoves ()
        |> delay (100 * i)
        |> map (fun m -> Letter (i, string c, int m.clientX, int m.clientY)))

// Query for message stream transformation (expression style)
let query msgs =
    rx {
        let! i, c = Seq.toList "TIME FLIES LIKE AN ARROW"
                    |> Seq.mapi (fun i c -> i, c)
                    |> ofSeq
        let ms = fromMouseMoves () |> delay (100 * i)
        for m in ms do
            yield Letter (i, string c, int m.clientX, int m.clientY)
    }

Program.mkSimple init update view
|> Program.withQuery query
|> Program.withReactUnoptimized "elmish-app"
|> Program.run
module FractalXam.Core.Fractal

[<Struct>]
type vec2 =
    val X : single
    val Y : single

    new (x, y) = { X = x; Y = y }

[<Struct>]
type Line =
    val X : vec2
    val Y : vec2

    new (x, y) = { X = x; Y = y }

let torad = 0.0174532925f

let lrad = 20.f * torad
let rrad = -lrad

let inline makeEndpoint rads length (v: vec2) = vec2 (v.X + length * cos rads, v.Y + length * sin rads)

let inline makeDrawLine rads length (line: Line) = Line (line.Y, makeEndpoint rads length line.Y)

let makeLines degrees length (line: Line) =

    let rec makeLines rads length (lines: Line list) cont = function
        | 10 -> cont lines
        | n ->
            let ldeg = rads + lrad
            let rdeg = rads + rrad
            let ll = makeDrawLine ldeg length lines.Head
            let rl = makeDrawLine rdeg length lines.Head
            let n = n + 1
            let length = length * 0.7f
      
            makeLines ldeg length (ll :: lines) (fun x ->
                makeLines rdeg length (rl :: x) cont n) n

    makeLines (degrees * torad) length [line] (fun x -> x) 0

[<CompiledName("Create")>]
let create length =
    let beginPoint = vec2 (0.f, -1.f)
    let endPoint = vec2 (0.f, -0.5f)
    let drawLine = Line (beginPoint, endPoint)
    makeLines 90.f (length) drawLine
    |> Array.ofList
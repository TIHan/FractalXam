[<RequireQualifiedAccess>]
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

type Config =
    {
        Degrees: float32
        Length: float32
    }

[<Literal>]
let torad = 0.0174532925f

let inline makeEndpoint rads length (v: vec2) = vec2 (v.X + length * cos rads, v.Y + length * sin rads)

let inline makeLine rads length (line: Line) = Line (line.Y, makeEndpoint rads length line.Y)

let makeLines config (line: Line) =

    let rec makeLines rads length (lines: Line list) cont = function
        | 10 -> cont lines
        | n ->
            let lrad = config.Degrees * torad
            let rrad = -config.Degrees * torad
            let ldeg = rads + lrad
            let rdeg = rads + rrad
            let ll = makeLine ldeg length lines.Head
            let rl = makeLine rdeg length lines.Head
            let n = n + 1
            let length = length * 0.7f
      
            makeLines ldeg length (ll :: lines) (fun x ->
                makeLines rdeg length (rl :: x) cont n) n

    makeLines (90.f * torad) config.Length [line] (fun x -> x) 0

[<CompiledName("Create")>]
let create config =
    let beginPoint = vec2 (0.f, -1.f)
    let endPoint = vec2 (0.f, -1.f * (1.f - config.Length))
    let drawLine = Line (beginPoint, endPoint)
    makeLines config drawLine
    |> Array.ofList
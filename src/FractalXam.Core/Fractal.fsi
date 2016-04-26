[<RequireQualifiedAccess>]
module FractalXam.Core.Fractal

[<Struct>]
type private vec2 =
    val X : single
    val Y : single

    new : single * single -> vec2

[<Struct>]
type Line =
    val private X : vec2
    val private Y : vec2

    private new : vec2 * vec2 -> Line

type Config =
    {
        Degrees: float32
        Length: float32
    }

[<CompiledName("Create")>]
val create : config: Config -> Line []

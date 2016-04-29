namespace FractalXam.Android

open System

open Android.App
open Android.Content
open Android.OS
open Android.Runtime
open Android.Views
open Android.Widget

open FractalXam.Android.Views

[<Activity (Label = "FractalXam.Android", MainLauncher = true)>]
type MainActivity () =
    inherit Activity ()

    let mutable mainLinearLayout : LinearLayout = null
    let mutable controlsLinearLayout : LinearLayout = null
    let mutable fractalView : FractalView = Unchecked.defaultof<FractalView>

    let mutable stateLength = 0
    let mutable stateFirstLength = 0
    let mutable stateDegrees = 0

    member this.AddSeekBar (labelText, f) =
        let layoutParams = new LinearLayout.LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
        layoutParams.SetMargins (16, 16, 16, 16)

        let linearLayout = new LinearLayout (this)
        linearLayout.Orientation <- Orientation.Horizontal
        linearLayout.LayoutParameters <- layoutParams

        let label = new TextView (this)
        label.Text <- labelText
        label.SetMinimumWidth (256)

        let seekBar = new SeekBar (this)
        f (seekBar)

        linearLayout.AddView (label)
        linearLayout.AddView (seekBar, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent))

        controlsLinearLayout.AddView (linearLayout)

    override this.OnCreate (bundle) =
        base.OnCreate (bundle)

        if bundle <> null then
            stateLength <- bundle.GetInt ("Length")
            stateFirstLength <- bundle.GetInt ("FirstLength")
            stateDegrees <- bundle.GetInt ("Degrees")
        else
            stateLength <- int (0.4f * 1000.f)
            stateFirstLength <- stateLength
            stateDegrees <- 20

        mainLinearLayout <- new LinearLayout (this)
        mainLinearLayout.Orientation <- Orientation.Vertical
        mainLinearLayout.SetGravity (GravityFlags.CenterHorizontal)

        controlsLinearLayout <- new LinearLayout (this)
        controlsLinearLayout.Orientation <- Orientation.Vertical

        fractalView <- new FractalView (this)
        mainLinearLayout.AddView (fractalView)
        mainLinearLayout.AddView (controlsLinearLayout)

        this.AddSeekBar ("Length", fun seekBar ->
            seekBar.Max <- 1000
            seekBar.Progress <- stateLength
            fractalView.SetFractalLength (float32 stateLength * 0.001f)
            seekBar.ProgressChanged.Add (fun e ->
                stateLength <- e.Progress
                fractalView.SetFractalLength (float32 stateLength * 0.001f)
            )
        )

        this.AddSeekBar ("First Length", fun seekBar ->
            seekBar.Max <- 1000
            seekBar.Progress <- stateFirstLength
            fractalView.SetFractalFirstLength (float32 stateFirstLength * 0.001f)
            seekBar.ProgressChanged.Add (fun e ->
                stateFirstLength <- e.Progress;
                fractalView.SetFractalFirstLength (float32 stateFirstLength * 0.001f)
            )
        )

        this.AddSeekBar ("Degrees", fun seekBar ->
            seekBar.Max <- 360
            seekBar.Progress <- stateDegrees
            fractalView.SetFractalDegrees (float32 stateDegrees)
            seekBar.ProgressChanged.Add (fun e ->
                stateDegrees <- e.Progress
                fractalView.SetFractalDegrees (float32 stateDegrees)
            )
        )

        this.SetContentView (mainLinearLayout)

        mainLinearLayout.LayoutChange.Add (fun e ->
            let parent = fractalView.Parent :?> View
            let mutable size = 0

            if this.Resources.Configuration.Orientation = Android.Content.Res.Orientation.Portrait then
                mainLinearLayout.Orientation <- Orientation.Vertical
                size <- parent.Height / 2
            else
                mainLinearLayout.Orientation <- Orientation.Horizontal
                size <- parent.Width / 2

            fractalView.LayoutParameters <- new LinearLayout.LayoutParams (size, size);
        )

    override this.OnSaveInstanceState outState =
        outState.PutInt ("Length", stateLength)
        outState.PutInt ("FirstLength", stateFirstLength)
        outState.PutInt ("Degrees", stateDegrees)
        base.OnSaveInstanceState (outState)

    override this.OnRestoreInstanceState savedInstanceState =
        base.OnRestoreInstanceState (savedInstanceState);
        stateLength <- savedInstanceState.GetInt ("Length")
        stateFirstLength <- savedInstanceState.GetInt ("FirstLength")
        stateDegrees <- savedInstanceState.GetInt ("Degrees")
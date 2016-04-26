using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Opengl;
using Javax.Microedition.Khronos.Egl;
using Javax.Microedition.Khronos.Opengles;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using FractalXam.Core;

namespace FractalXamDroid
{
    [Activity(Label = "FractalXamDroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        const float DefaultProgress = 0.4f;

        void AddSeekBar(LinearLayout linearLayout, FractalView fractalView)
        {
            var seekBarLayout = new LinearLayout(this);
            var seekBar = new SeekBar(this);
            seekBarLayout.AddView(seekBar, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            linearLayout.AddView(seekBarLayout);

            seekBar.Max = 1000;
            seekBar.Progress = (int)(DefaultProgress * 1000f);
            fractalView.SetFractalLength(DefaultProgress);
            seekBar.ProgressChanged += (sender, e) =>
            {
                fractalView.SetFractalLength(e.Progress * 0.001f);
            };
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var linearLayout = new LinearLayout(this);
            linearLayout.Orientation = Orientation.Vertical;
            linearLayout.SetGravity(GravityFlags.CenterHorizontal);

            var fractalView = new FractalView(this);
            linearLayout.AddView(fractalView, new ViewGroup.LayoutParams(1024, 1024));

            AddSeekBar(linearLayout, fractalView);

            SetContentView(linearLayout);
        }
    }
}


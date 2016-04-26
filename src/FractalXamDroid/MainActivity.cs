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

        LinearLayout m_linearLayout;
        FractalView m_fractalView;

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

            m_linearLayout = new LinearLayout(this);
            m_linearLayout.Orientation = Orientation.Vertical;
            m_linearLayout.SetGravity(GravityFlags.CenterHorizontal);

            m_fractalView = new FractalView(this);
            m_linearLayout.AddView(m_fractalView);

            AddSeekBar(m_linearLayout, m_fractalView);

            SetContentView(m_linearLayout);

            m_linearLayout.LayoutChange += (sender, e) =>
            {
                var parent = (View)m_fractalView.Parent;
                var size = 0;

                if (Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait)
                {
                    m_linearLayout.Orientation = Orientation.Vertical;
                    size = parent.Width;
                }
                else
                {
                    m_linearLayout.Orientation = Orientation.Horizontal;
                    size = parent.Height;
                }

                m_fractalView.LayoutParameters = new LinearLayout.LayoutParams(size, size);
            };
        }
    }
}


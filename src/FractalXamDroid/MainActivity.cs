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
        LinearLayout m_mainLinearLayout;
        LinearLayout m_controlsLinearLayout;
        FractalView m_fractalView;

        int m_stateLength = 0;
        int m_stateFirstLength = 0;
        int m_stateDegrees = 0;

        void AddSeekBar(string labelText, Action<SeekBar> f)
        {
            var layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            layoutParams.SetMargins(16, 16, 16, 16);

            var linearLayout = new LinearLayout(this);
            linearLayout.Orientation = Orientation.Horizontal;
            linearLayout.LayoutParameters = layoutParams;

            var label = new TextView(this);
            label.Text = labelText;
            label.SetMinimumWidth(256);

            var seekBar = new SeekBar(this);
            f(seekBar);

            linearLayout.AddView(label);
            linearLayout.AddView(seekBar, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

            m_controlsLinearLayout.AddView(linearLayout);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (bundle != null)
            {
                m_stateLength = bundle.GetInt("Length");
                m_stateFirstLength = bundle.GetInt("FirstLength");
                m_stateDegrees = bundle.GetInt("Degrees");
            }
            else
            {
                m_stateLength = (int)(0.4f * 1000f);
                m_stateFirstLength = m_stateLength;
                m_stateDegrees = 20;
            }


            m_mainLinearLayout = new LinearLayout(this);
            m_mainLinearLayout.Orientation = Orientation.Vertical;
            m_mainLinearLayout.SetGravity(GravityFlags.CenterHorizontal);

            m_controlsLinearLayout = new LinearLayout(this);
            m_controlsLinearLayout.Orientation = Orientation.Vertical;

            m_fractalView = new FractalView(this);
            m_mainLinearLayout.AddView(m_fractalView);
            m_mainLinearLayout.AddView(m_controlsLinearLayout);

            AddSeekBar("Length", seekBar =>
            {
                seekBar.Max = 1000;
                seekBar.Progress = m_stateLength;
                m_fractalView.SetFractalLength(m_stateLength * 0.001f);
                seekBar.ProgressChanged += (sender, e) =>
                {
                    m_stateLength = e.Progress;
                    m_fractalView.SetFractalLength(m_stateLength * 0.001f);
                };
            });

            AddSeekBar("First Length", seekBar =>
            {
                seekBar.Max = 1000;
                seekBar.Progress = m_stateFirstLength;
                m_fractalView.SetFractalFirstLength(m_stateFirstLength * 0.001f);
                seekBar.ProgressChanged += (sender, e) =>
                {
                    m_stateFirstLength = e.Progress;
                    m_fractalView.SetFractalFirstLength(m_stateFirstLength * 0.001f);
                };
            });

            AddSeekBar("Degrees", seekBar =>
            {
                seekBar.Max = 360;
                seekBar.Progress = m_stateDegrees;
                m_fractalView.SetFractalDegrees(m_stateDegrees);
                seekBar.ProgressChanged += (sender, e) =>
                {
                    m_stateDegrees = e.Progress;
                    m_fractalView.SetFractalDegrees(m_stateDegrees);
                };
            });

            SetContentView(m_mainLinearLayout);

            m_mainLinearLayout.LayoutChange += (sender, e) =>
            {
                var parent = (View)m_fractalView.Parent;
                var size = 0;

                if (Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait)
                {
                    m_mainLinearLayout.Orientation = Orientation.Vertical;
                    size = parent.Height / 2;
                }
                else
                {
                    m_mainLinearLayout.Orientation = Orientation.Horizontal;
                    size = parent.Width / 2;
                }

                m_fractalView.LayoutParameters = new LinearLayout.LayoutParams(size, size);
            };
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt("Length", m_stateLength);
            outState.PutInt("FirstLength", m_stateFirstLength);
            outState.PutInt("Degrees", m_stateDegrees);
            base.OnSaveInstanceState(outState);
        }
    }
}


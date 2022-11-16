﻿using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Media;
using System.IO;
using System.Runtime.CompilerServices;
using Android.Widget;
using Java.Lang;
using System.Threading.Tasks;

namespace G_NeuroApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        MediaRecorder recorder;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

			AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            Button btnStart = FindViewById<Button>(Resource.Id.btnStart);
            btnStart.Click += btn_Start_Click;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void btn_Start_Click(object sender, EventArgs eventArgs)
        {
            RecordAudio();
        }


		/*void RecordAudio(System.String filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                if(recorder == null)
                {
                    recorder = new MediaRecorder();
                }
                else
                {
                    recorder.Reset();
                    recorder.SetAudioSource(AudioSource.Mic);
                    recorder.SetOutputFormat(OutputFormat.ThreeGpp);
                    recorder.SetAudioEncoder(AudioEncoder.AmrNb);
                    recorder.SetOutputFile(filePath);
                    recorder.Prepare();
                    recorder.Start();
                    Thread.Sleep(3000);
                    recorder.Stop();
                }
            }
            catch(System.Exception ex)
            {
                Console.Out.WriteLine(ex.StackTrace);
            }
        }*/

		void PlayAudioTrack(byte[] audioBuffer)
		{
			AudioTrack audioTrack = new AudioTrack(
			  // Stream type
			  Android.Media.Stream.Music,
			  // Frequency
			  11025,
			  // Mono or stereo
			  ChannelOut.Stereo,
			  // Audio encoding
			  Android.Media.Encoding.Pcm16bit,
			  // Length of the audio clip.
			  audioBuffer.Length,
			  // Mode. Stream or static.
			  AudioTrackMode.Stream);

			audioTrack.Play();
			audioTrack.Write(audioBuffer, 0, audioBuffer.Length);
		}

        void Timer(AudioRecord audio, byte[] buffer)
        {
            while (true)
            {
                try
                {
                    // Keep reading the buffer while there is audio input.
                    audio.Read(buffer, 0, buffer.Length);

                    PlayAudioTrack(buffer);
                    // Write out the audio file.
                }
                catch (System.Exception ex)
                {
                    Console.Out.WriteLine(ex.Message);
                    break;
                }
            }
		}
		
        async void RecordAudio()
		{
			byte[] audioBuffer = new byte[100000];
			var audRecorder = new AudioRecord(
			  // Hardware source of recording.
			  AudioSource.Mic,
			  // Frequency
			  11025,
			  // Mono or stereo
			  ChannelIn.Stereo,
			  // Audio encoding
			  Android.Media.Encoding.Pcm16bit,
			  // Length of the audio clip.
			  audioBuffer.Length
			);
			audRecorder.StartRecording();

            await Task.Run(() => Timer(audRecorder, audioBuffer));
            //await Task.Run(() => RecordAudio());
			/*while (true)
			{
				try
				{
					// Keep reading the buffer while there is audio input.
					audRecorder.Read(audioBuffer, 0, audioBuffer.Length);

					PlayAudioTrack(audioBuffer);
					// Write out the audio file.
				}
				catch (System.Exception ex)
				{
					Console.Out.WriteLine(ex.Message);
					break;
				}
			}*/
		}
	}
}

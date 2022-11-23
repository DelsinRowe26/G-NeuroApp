using System;
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
        bool clickStop;
		
		byte[] audioBuffer1 = new byte[6000];
		byte[] audioBuffer = new byte[8192];



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

            /*Button btnStop = FindViewById<Button>(Resource.Id.btnStop);
            btnStop.Click += btn_Stop_Click;*/
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

        private async void btn_Start_Click(object sender, EventArgs eventArgs)
        {
            clickStop = true;
            await Task.Run (() => RecordAudio());

        }

        /*private void btn_Stop_Click(object sender, EventArgs eventArgs)
        {
            clickStop = false;
            //audRecorder.Stop();
            //audioTrack.Stop();
            //Timer(audRecorder, audioBuffer, audioBuffer1);
        }*/

		async void PlayAudioTrack(byte[] audioBuffer)
		{
			AudioTrack audioTrack = new AudioTrack(
			  // Stream type
			  Android.Media.Stream.Music,
			  // Frequency
			  44100,
			  // Mono or stereo
			  ChannelOut.Mono,
			  // Audio encoding
			  Android.Media.Encoding.Pcm16bit,
			  // Length of the audio clip.
			  audioBuffer.Length,
			  // Mode. Stream or static.
			  AudioTrackMode.Stream);
			await Task.Run(() => audioTrack.Play());
			await Task.Run(() => audioTrack.Write(audioBuffer, 0, audioBuffer.Length));
		}

        async void Timer(AudioRecord audio, byte[] buffer, byte[] buffer1)
        {
            int i = 0, count = 0;
            while (clickStop)
            {
                try
                {
                    // Keep reading the buffer while there is audio input.
                    await Task.Run(() => audio.Read(buffer, 0, buffer.Length));
                    await Task.Run(() => PlayAudioTrack(buffer));
                }
                catch (System.Exception ex)
                {
                    Console.Out.WriteLine(ex.Message);
                    //break;
                }
            }
		}
		
        async void RecordAudio()
		{
			
			AudioRecord audRecorder = new AudioRecord(
		      // Hardware source of recording.
		      AudioSource.Mic,
		      // Frequency
		      44100,
		      // Mono or stereo
		      ChannelIn.Mono,
		      // Audio encoding
		      Android.Media.Encoding.Pcm16bit,
		      // Length of the audio clip.
		      audioBuffer.Length
		    );
			await Task.Run(() => audRecorder.StartRecording());

            await Task.Run(() => Timer(audRecorder, audioBuffer, audioBuffer1));

		}
	}
}

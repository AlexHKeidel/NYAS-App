using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Java.Math;

namespace NYASApp
{
	[Activity (Label = "NYAS-App", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature (WindowFeatures.NoTitle); //removing top bar from the app
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Splash_Screen_Layout); //setting layout

			ImageView SplashImage = (ImageView)FindViewById (Resource.Id.SplashImage);

			int number;
			Random randy = new Random ();
			number = randy.Next (100); //roll random number between 0 (inclusive) and 100 (exclusive)
			//rolling 0 or 1 is too unreliable and seems to always return 0
			//Toast.MakeText (this, "number = " + number, ToastLength.Long).Show();
			if (number <= 50) {
				SplashImage.SetImageDrawable (Resources.GetDrawable(Resource.Drawable.Splash_Screen_One));
			} else {
				SplashImage.SetImageDrawable (Resources.GetDrawable(Resource.Drawable.Splash_Screen_Two));
			}

			SplashImage.Click += delegate { //setting button listener to start the homepage activity
				startHomepageActivity();
			};
			// Get our button from the layout resource,
			// and attach an event to it
		}

		private void startHomepageActivity(){
			StartActivity (typeof(HomepageActivity)); //start a new HomepageActivity
			Finish ();
		}
	}
}



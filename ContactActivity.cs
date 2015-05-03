
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


//Created by Alexander Keidel (22397868), last edited 03/05/2015
namespace NYASApp
{
	/// <summary>
	/// Contact activity used to directly contact NYAS via email or freephone.
	/// </summary>
	[Activity (Label = "ContactActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class ContactActivity : Activity
	{
		Button CallFreephone, SendEmail;

		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature (WindowFeatures.NoTitle); //removing top bar from the app
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Contact_Activity_Layout); //setting layout
			SetupResources ();
			SetupListeners ();
		}

		/// <summary>
		/// Sets up the resources for this activity, such as the buttons.
		/// </summary>
		private void SetupResources(){
			CallFreephone = (Button)FindViewById (Resource.Id.CallFreePhone);
			SendEmail = (Button)FindViewById (Resource.Id.SendEmail);
		}

		/// <summary>
		/// Sets up the listeners for the buttons of this activity.
		/// </summary>
		private void SetupListeners(){
			CallFreephone.Click += delegate { // see http://developer.xamarin.com/recipes/android/fundamentals/intent/launch_the_phone_dialer/
				var uri = Android.Net.Uri.Parse ("tel:08088081001"); // This is the NYAS Freephone number, according to https://www.nyas.net/freephone PLEASE NOT THAT THIS MAY CHARGE YOU MONEY!
				var intent = new Intent (Intent.ActionView, uri); 
				StartActivity (intent); 
			};
			SendEmail.Click += delegate { //see http://developer.xamarin.com/recipes/android/networking/email/send_an_email/
				var email = new Intent (Android.Content.Intent.ActionSend);
				email.PutExtra (Android.Content.Intent.ExtraEmail, 
					new string[]{"alexander.keidel@go.edgehill.ac.uk"} ); //here you would put help@nyas.net

				email.PutExtra (Android.Content.Intent.ExtraSubject, "Contact NYAS Example"); //replace with the desired subject

				email.PutExtra (Android.Content.Intent.ExtraText, 
					"This is an automatically generated test.\nThis may be used by NYAS if they want to implement this feature."); //generated text for the email.
				email.SetType ("message/rfc822");
				StartActivity (email);
			};
		}
	}
}


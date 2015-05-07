
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

//Created by Alexander Keidel (22397868), last edited 07/05/2015
namespace NYASApp
{
	/// <summary>
	/// Information activity displaying some relevant information based upon the selected topic.
	/// </summary>
	[Activity (Label = "InformationActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class InformationActivity : Activity
	{
		const int WHATS_NYAS_DO_STATE = 0; //THIS HAS TO BE IDENTICAL TO Resources.GetInteger(Resource.Integer.IAWhatsNYASDoContext);
		TextView TitleField, ContentField;
		String WhatsNYASDoTitle, WhatsNYASDoContent;
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature (WindowFeatures.NoTitle); //removing top bar from the app
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Information_Activity_Layout);
			SetupResources ();
			SetupLayouts ();

			int chosenState = Intent.GetIntExtra ("CONTEXT", -1); //getting the context passed to this activity via the bundle
			ApplyState (chosenState);
		}

		/// <summary>
		/// Sets up the resources for this activity.
		/// </summary>
		private void SetupResources(){
			TitleField = (TextView)FindViewById (Resource.Id.InformationTitle);
			ContentField = (TextView)FindViewById (Resource.Id.InformationContent);
			WhatsNYASDoTitle = Resources.GetString (Resource.String.IAWhatsNYASDoTitle);
			WhatsNYASDoContent = Resources.GetString (Resource.String.IAWhatsNYASDoContent);
		}

		/// <summary>
		/// Applies the layouts for the activity such as making the content textview scroll.
		/// </summary>
		private void SetupLayouts(){
			ContentField.MovementMethod = Android.Text.Method.ScrollingMovementMethod.Instance; //setting the movement method
			ContentField.SetMaxLines (10); //maximum lines after which the textview starts scrolling
		}

		/// <summary>
		/// Applies the selected state.
		/// This is chosen depending on the value passed to this activity.
		/// </summary>
		/// <param name="state">State.</param>
		private void ApplyState(int state){
			switch (state) {
			case WHATS_NYAS_DO_STATE:
				TitleField.Text = WhatsNYASDoTitle;
				ContentField.Text = WhatsNYASDoContent;
				break;
			default:
				TitleField.Text = WhatsNYASDoTitle;
				ContentField.Text = WhatsNYASDoContent;
				break;
			}
		}
	}
}


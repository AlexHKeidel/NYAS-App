
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
	/// Profile activity used to edit and store the users profile.
	/// </summary>
	[Activity (Label = "ProfileActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class ProfileActivity : Activity
	{
		EditText NameField, AgeField, EmailField, PhoneNumberField;
		Button SaveChangesButton;
		FileManager MyFileManager;
		const char SEPARATOR = ',';

		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature (WindowFeatures.NoTitle); //removing top bar from the app
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Profile_Activity_Layout); //setting layout
			SetupResources ();
			SetupListeners ();
			ShowProfile ();
		}

		/// <summary>
		/// Sets up all the resources for this activity.
		/// </summary>
		private void SetupResources(){
			NameField = (EditText)FindViewById (Resource.Id.EnterNameField);
			AgeField = (EditText)FindViewById (Resource.Id.EnterAgeField);
			EmailField = (EditText)FindViewById (Resource.Id.EnterEmailField);
			PhoneNumberField = (EditText)FindViewById (Resource.Id.EnterPhoneField);
			SaveChangesButton = (Button)FindViewById (Resource.Id.SaveChanges);
			MyFileManager = new FileManager (BaseContext.FilesDir.AbsolutePath);
		}

		/// <summary>
		/// Sets up all the listeners for relevant view of this activity.
		/// </summary>
		private void SetupListeners(){
			SaveChangesButton.Click += delegate {
				SaveAllChanges();
			};
		}

		/// <summary>
		/// Saves all changes and writes them to the correct file.
		/// This also checks the user has entered something into each text field.
		/// </summary>
		private void SaveAllChanges(){
			if (NameField.Text.Contains (SEPARATOR) || AgeField.Text.Contains (SEPARATOR) || EmailField.Text.Contains (SEPARATOR) || PhoneNumberField.Text.Contains (SEPARATOR)) { //the user has used commas in their entry, reject these entries and tell the user not to use special characters
				Toast.MakeText(this, "Please do not use special characters.", ToastLength.Long).Show();
				return;
			}
			if (NameField.Text.Length != 0 && AgeField.Text.Length != 0 && EmailField.Text.Length != 0 && PhoneNumberField.Text.Length != 0) { //No Field is empty
				String ProfileString = NameField.Text + SEPARATOR + AgeField.Text + SEPARATOR + EmailField.Text + SEPARATOR + PhoneNumberField.Text;
				MyFileManager.WriteProfile (ProfileString);
				Toast.MakeText (this, "Changes saved. Press back to close.", ToastLength.Long).Show ();
			} else {
				Toast.MakeText (this, "Please enter your details.", ToastLength.Long).Show ();
			}
		}

		/// <summary>
		/// Shows the profile on the screen inside the text fields.
		/// </summary>
		private void ShowProfile(){
			String temp = MyFileManager.ReadProfile ();
			if(temp.Equals("No Profile Set")){
				return;
			}
			String[] ProfileData = temp.Split (SEPARATOR); //Splitting the string at commas
			NameField.Text = ProfileData [0]; //setting the text areas to the correct values from the array. This must be the same order as they are saved in!
			AgeField.Text = ProfileData [1];
			EmailField.Text = ProfileData [2];
			PhoneNumberField.Text = ProfileData [3];
		}
	}
}


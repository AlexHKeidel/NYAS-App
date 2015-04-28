
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

//Created by Alexander Keidel (22397868), last edited 28/04/2015
namespace NYASApp
{
	[Activity (Label = "AppointmentsActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class AppointmentsActivity : Activity
	{
		ListView MyListView;
		ArrayAdapter ad;

		FileManager myFileManager;
		String Directory, Splitter;
		String [] Appointments;
		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature (WindowFeatures.NoTitle); //removing top bar from the app
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Appointments_Activity_Layout); //setting layout
			SetupResources();
			SetupListView ();
		}

		/// <summary>
		/// Sets up the relevant resources for this activity, including all views and classes.
		/// </summary>
		private void SetupResources ()
		{
			MyListView = (ListView)FindViewById (Resource.Id.appointmentListView);
			Directory = BaseContext.FilesDir.AbsolutePath;
			myFileManager = new FileManager (Directory);
			Splitter = myFileManager.ReadAppointments ();
			Appointments = Splitter.Split (',');
			ad = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1); //see http://stackoverflow.com/questions/21953965/listview-with-dynamic-strings-coded-in-c-sharp-xamarin-mono
			MyListView.Adapter = ad;
		}

		private void SetupListView(){
			for (int i = 0; i < Appointments.Length; i++) {
				try{
				if ((i & 1) == 0) { //index is an even number since the first bit is 0
					ad.Add(Appointments[i] + " at " + Appointments [i + 1]);
					}
				} catch (IndexOutOfRangeException){
				}
			}
		}
	}
}


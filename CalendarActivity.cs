
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

namespace NYASApp
{
	[Activity (Label = "CalendarActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class CalendarActivity : Activity
	{
		CalendarView Calendar;
		Button MakeAppointmentButton;
		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature (WindowFeatures.NoTitle); //removing top bar from the app
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Calendar_Activity_Layout); //setting layout
			SetupResources(); //setting up all resource files to the defined objects
			SetupLayout(); //setting up the layout of the activity
		}

		private void SetupResources(){
			Calendar = (CalendarView)FindViewById (Resource.Id.calendarView1);
			MakeAppointmentButton = (Button)FindViewById (Resource.Id.makeAppointmentButton);
		}

		private void SetupLayout(){
			var metrics = Resources.DisplayMetrics; //getting the display metrics (resolution) of the devices screen
			Calendar.SetPadding(0,0,0, (metrics.HeightPixels * 1/3));
			var dateText = Calendar.GetChildAt (0); //position of the actual background of the calendar
			dateText.SetBackgroundColor (Resources.GetColor (Resource.Color.calendarBackground));

		}
	}
}


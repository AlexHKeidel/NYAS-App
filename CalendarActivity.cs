
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

//Created by Alexander Keidel (22397868), last edited 02/05/2015
namespace NYASApp
{
	[Activity (Label = "CalendarActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class CalendarActivity : Activity
	{
		const int TIME_DIALOG_ID = 0;
		int hour;
		int minute;
		String time;

		String directory;
		CalendarView Calendar;
		Button MakeAppointmentButton, ViewAppointmentsButton;

		FileManager MyFileManager;

		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature (WindowFeatures.NoTitle); //removing top bar from the app
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Calendar_Activity_Layout); //setting layout
			SetupResources(); //setting up all resource files to the defined objects
			SetupLayout(); //setting up the layout of the activity
			SetupButtonListeners();

		}

		/// <summary>
		/// Sets up the relevant resources for this activity, including all views and classes.
		/// </summary>
		private void SetupResources(){
			Calendar = (CalendarView)FindViewById (Resource.Id.calendarView1);
			MakeAppointmentButton = (Button)FindViewById (Resource.Id.makeAppointmentButton);
			ViewAppointmentsButton = (Button)FindViewById (Resource.Id.ViewAppointments);
			directory = BaseContext.FilesDir.AbsolutePath;
			MyFileManager = new FileManager (directory);
		}

		/// <summary>
		/// Sets up the layout for this activity.
		/// This uses display metrics to determine the screen resolution of the used device.
		/// </summary>
		private void SetupLayout(){
			var metrics = Resources.DisplayMetrics; //getting the display metrics (resolution) of the devices screen
			Calendar.SetPadding(15,15,15, (metrics.HeightPixels * 1/3));
			var dateText = Calendar.GetChildAt (0); //position of the actual background of the calendar
			dateText.Background = Resources.GetDrawable (Resource.Drawable.alternative_orange_gradient);
			dateText.Alpha = 0.75f; //making the calendar slightly transparent (75% opaque or 25% transparent)
		}

		private void SetupButtonListeners(){ 
			MakeAppointmentButton.Click += (object sender, EventArgs e) => ShowDialog(TIME_DIALOG_ID);
			ViewAppointmentsButton.Click += delegate {
				StartActivity(typeof(AppointmentsActivity));
			};

		}

		/// <summary>
		/// Converts date from the Calendar and returns a formatted String.
		/// Format: DD/MM/YYYY
		/// </summary>
		/// <returns>String in the defined short format.</returns>
		private String ConvertDate(){ 
			DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // see http://stackoverflow.com/questions/4964634/how-to-convert-long-type-datetime-to-datetime-with-correct-time-zone
			DateTime converter = dt.AddMilliseconds(Calendar.Date).ToLocalTime();
			return converter.ToShortDateString();
		}

		protected override Dialog OnCreateDialog (int id)
		{
			if (id == TIME_DIALOG_ID)
				return new TimePickerDialog (this, TimePickerCallback, hour, minute, false);
			return null;
		}

		private void TimePickerCallback (object sender, TimePickerDialog.TimeSetEventArgs e)
		{
			hour = e.HourOfDay;
			minute = e.Minute;
			time = string.Format ("{0}:{1}", hour, minute.ToString ().PadLeft (2, '0')); //formatting the string to give the correct time in a readable string format
			Console.WriteLine ("Picked time: " + time); //this is only printed when the user picks "set"
			//On cancel nothing will happen
			MyFileManager.WriteAppointment(ConvertDate() + "," + time + ","); //writing the date + time into the file, separated with a comma between time and date as well as at the end of the entry
		}
	}
}


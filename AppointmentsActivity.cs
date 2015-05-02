
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
	/// <summary>
	/// Appointments activity displaying all the appointments the user has saved in the Appointments.txt file inside the apps resources on the users device.
	/// </summary>
	[Activity (Label = "AppointmentsActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class AppointmentsActivity : Activity
	{
		ListView MyListView;
		ArrayAdapter MyArrayAdapter;

		FileManager myFileManager;
		String Directory;
		String [] Appointments;
		List<String> AppointmentList = new List<string>();

		/// <summary>
		/// OnCreate used by activity when it is first created.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
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
			Appointments = SplitAppointments (myFileManager.ReadAppointments ()); //default value for the appointments
			MyArrayAdapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1); //see http://stackoverflow.com/questions/21953965/listview-with-dynamic-strings-coded-in-c-sharp-xamarin-mono
			MyListView.Adapter = MyArrayAdapter;
			MyListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => StartCustomDialog(e.Position, MyListView.GetItemAtPosition(e.Position).ToString()); //adding button listener that returns the id of the clicked item
		}

		/// <summary>
		/// Splits the given string at commas
		/// </summary>
		/// <returns>The appointments.</returns>
		/// <param name="Input">Input String</param>
		private String [] SplitAppointments(String Input){
			return  Input.Split (',');
		}

		/// <summary>
		/// Sets up the list view displaying all appointments by adding them to the ArrayAdapter
		/// </summary>
		private void SetupListView(){
			for (int i = 0; i < (Appointments.Length - 1); i++) {
				if (i % 2 == 0) { //the index is even
					MyArrayAdapter.Add(Appointments[i] + " at " + Appointments[i + 1]);
				}
			}
		}

		private void StartCustomDialog(int position, String appointment){
			AlertDialog.Builder ad = new AlertDialog.Builder (this);
			ad.SetMessage (appointment + "\nChoose an option.");
			ad.SetCancelable (true);
			ad.SetPositiveButton("Delete", delegate {
				DeleteEntry(position);
			});
			ad.SetNegativeButton ("Edit", delegate {
				Console.WriteLine ("Editing not currently supported");
				Toast.MakeText(this, "Editing not currently supported", ToastLength.Long).Show();
			});
			ad.Show ();
		}

		private void DeleteEntry(int position){
			Console.WriteLine ("Delete item at position " + position);
			AppointmentList.Clear ();
			foreach (String s in Appointments) {
				AppointmentList.Add (s);
				Console.WriteLine (s);
			}

			AppointmentList.RemoveAt (position * 2); //removing the two items that are at the position of the selected item in the list (i.e. 4 and 5, corressponding to item index 2)
			AppointmentList.RemoveAt (position * 2); //the first removed item shifts the list so that you can just remove the same index again, as the second part (time) will have moved to that position in the list.

			Appointments = AppointmentList.ToArray (); //updating the appointments string array with the updated format
			String temp = "";
			foreach (String s in Appointments) { //adding all strings into a single string and comma separating them
				if(s.Equals("")){
					//do nothing, empty record
					//these seem to appear because of the line that adds " at " between the date and the time of the appointment record
				} else {
				temp += s + ",";
				Console.WriteLine ("s = " + s);
				}
			}
			Console.WriteLine ("new appointments " + temp);

			myFileManager.OverwriteAppointments (temp); //overwrite the appointments file
			MyArrayAdapter.Clear (); //clearing the array adapter
			SetupListView (); //repopulating the array adapter
		}
	}
}
using System;
//using Java.IO;
using System.IO;
using Android;

//Created by Alexander Keidel (22397868), last edited 28/04/2015
namespace NYASApp
{
	public class FileManager
	{	
		const String AppointmentFile = "AppointmentFile.txt";
		String dir;
		String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // see http://stackoverflow.com/questions/21707138/cannot-create-files-on-android-with-xamarin
		StreamWriter sw;
		StreamReader sr;
		FileStream fr;

		public FileManager (string directory)
		{
			dir = directory + AppointmentFile;
		}
		/// <summary>
		/// Writes the appointment.
		/// </summary>
		/// <param name="text">Appointment as string, formatted as DD/MM/YYYY,HH:MM-</param>
		public void WriteAppointment(String text){
			if (!File.Exists (dir)) { // see https://forums.xamarin.com/discussion/5445/write-in-a-file
				fr = new FileStream (dir, FileMode.Create);
			} else {
				fr = new FileStream (dir, FileMode.Append);
			}
			sw = new StreamWriter (fr);
			sw.Write (text);
			sw.Flush ();
			sw.Close ();
			sw.Dispose ();
			fr.Dispose ();
		}

		/// <summary>
		/// Reads the appointments.
		/// </summary>
		/// <returns>All appointments in a string, formatted as DD/MM/YYYY,HH:MM-</returns>
		public String ReadAppointments(){
			if (!File.Exists (dir)) {
				//the file does not exist
				return "No Appointments";
			}
			fr = new FileStream (dir, FileMode.Open);
			sr = new StreamReader (fr);
			String temp = sr.ReadToEnd ();
			sr.Close ();
			sr.Dispose ();
			fr.Dispose ();
			return temp;
		}

		public void destroyFile(){
			
		}
	}
}


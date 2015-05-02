using System;
//using Java.IO;
using System.IO;
using Android;

//Created by Alexander Keidel (22397868), last edited 02/05/2015
namespace NYASApp
{
	/// <summary>
	/// File manager created to take care of any file reading or writing.
	/// This custom class uses a FileStream with the apps specific file location.
	/// A StreamWriter is used to write to files.
	/// A StreamReader is used to read from files.
	/// </summary>
	public class FileManager
	{	
		const String AppointmentFile = "AppointmentFile.txt";
		const String PinFile = "Pin.txt";
		String AppointmentDirectory, PinDirectory;
		String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // see http://stackoverflow.com/questions/21707138/cannot-create-files-on-android-with-xamarin
		StreamWriter sw;
		StreamReader sr;
		FileStream fr;

		public FileManager (string directory)
		{
			AppointmentDirectory = directory + AppointmentFile;
			PinDirectory = directory + PinFile;
		}
		/// <summary>
		/// Writes the appointment.
		/// </summary>
		/// <param name="text">Appointment as string, formatted as DD/MM/YYYY,HH:MM-</param>
		public void WriteAppointment(String text){
			if (!File.Exists (AppointmentDirectory)) { // see https://forums.xamarin.com/discussion/5445/write-in-a-file
				fr = new FileStream (AppointmentDirectory, FileMode.Create);
			} else {
				fr = new FileStream (AppointmentDirectory, FileMode.Append);
			}
			sw = new StreamWriter (fr);
			sw.Write (text);
			sw.Flush ();
			sw.Close ();
			sw.Dispose ();
			fr.Dispose ();
		}

		/// <summary>
		/// Overwrites the appointments.
		/// </summary>
		/// <param name="text">Appointments with comma separators</param>
		public void OverwriteAppointments(String text){
			sw = new StreamWriter (AppointmentDirectory, false); //do not append but just rewrite the file
			sw.Write(text);
			sw.Flush ();
			sw.Close ();
			sw.Dispose ();
		}

		/// <summary>
		/// Reads the appointments.
		/// </summary>
		/// <returns>All appointments in a string, formatted as DD/MM/YYYY,HH:MM-</returns>
		public String ReadAppointments(){
			if (!File.Exists (AppointmentDirectory)) {
				//the file does not exist
				return "No Appointments";
			}
			fr = new FileStream (AppointmentDirectory, FileMode.Open);
			sr = new StreamReader (fr);
			String temp = sr.ReadToEnd ();
			sr.Close (); //closing reader
			sr.Dispose (); //disposing reader
			fr.Dispose (); //disposing file
			if(temp.Equals("")){
				return "No Appointments";
			}
			return temp; //returning the read string
		}

		/// <summary>
		/// Writes the pin to its file.
		/// </summary>
		/// <param name="Pin">String containing 4 symbol pin</param>
		public void WritePin(String Pin){
			if (!File.Exists (PinDirectory)) {
				fr = new FileStream (PinDirectory, FileMode.Create); //create pin
			} else {
				fr = new FileStream (PinDirectory, FileMode.Open); //override pin
			}
			sw = new StreamWriter (fr); //new stream writer with file stream fr
			sw.Write(Pin); //writing pin to file
			sw.Flush (); //flushing writer
			sw.Close (); //closing writer
			sw.Dispose (); //disposing of writer
			fr.Dispose (); //disposing of file
		}

		/// <summary>
		/// Deletes the pin.
		/// </summary>
		public void DeletePin(){
			sw = new StreamWriter (PinDirectory, false); //new stream writer with the file directory and telling it not to append (false)!
			sw.Write(""); //writing empty line to the file that overrides the pin
			sw.Flush (); //flushing writer
			sw.Close (); //closing writer
			sw.Dispose (); //disposing of writer
		}

		/// <summary>
		/// Reads the pin saved in the pin directory within the apps local storage on the device.
		/// </summary>
		/// <returns>The pin as String</returns>
		public String ReadPin(){
			try{
			fr = new FileStream (PinDirectory, FileMode.Open); //open mode for reading files
			sr = new StreamReader (fr); //initialise the reader
			String Pin = sr.ReadLine (); //read the line (This file should only ever have a single line)
			sr.Close (); //clsing the reader
			sr.Dispose (); //disposing of the reader
			fr.Dispose (); //disposing of the file stream
			return Pin; //return the read line as a String.
			} catch (Exception){
				return "No pin set";
			}
		}
	}
}


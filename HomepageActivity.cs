
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
	/// Homepage activity shown after entering through the splash screen.
	/// </summary>
	[Activity (Label = "HomepageActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class HomepageActivity : Activity
	{
		Button buttonTopLeft, buttonTopRight, buttonBottomLeft, buttonBottomRight; //four buttons represented on the screen in order as you would read (top left to bottom right)
		Button[] buttons;
		ImageView speechBubble, nyasLogo;
		TextView speechBubbleText, bottomTextBox;

		FileManager MyFileManager;

		const int DEFAULT_TEXT_SIZE = 0; //Constsant values (keyword 'final' in Java) are used throughout this activity to remember states of buttons, text sizes, content and more.
		const int BIG_TEXT_SIZE = 1;
		int currentTextSize = DEFAULT_TEXT_SIZE;

		const int DEFAULT_BOX_CONTENT = Resource.String.AboutUsDetailed;
		const int LOGIN_INSTRUCTIONS_BOX_CONTENT = Resource.String.PinInstructions;
		const int CORRECT_PIN_BOX_CONTENT = Resource.String.CorrectPin;
		const int INCORRECT_PIN_BOX_CONTENT = Resource.String.WrongPin;
		const int PIN_COLON_BOX_CONTENT = Resource.String.Pin;
		const int CONFIRM_PIN_BOX_CONTENT = Resource.String.Confirm;
		const int PIN_CONFIRMED_BOX_CONTENT = Resource.String.PinConfirmed;
		int currentBoxContent = -1;

		const int DEFAULT_HOME_STATE = 0;
		const int KIDS_ZONE_STATE = 1;
		const int CARER_INFO_STATE = 2;
		const int LOGIN_STATE = 3;
		const int MORE_INFO_STATE = 4;
		const int MY_NYAS_STATE = 5;
		const int OPTION_STATE = 6;
		int currentState = DEFAULT_HOME_STATE;
		int previousState = -1; //used for error catching

		List<String> UserPin = new List<string>(); //used to store the users actual pin
		List<String> InputPin = new List<string>(); //used to compare the users input to the actual pin
		List<String> SecondInputPin = new List<string>(); //used to compare the users second input of the pin before setting it
		bool PinSet = false;
		bool PinEntered = false;

		const String TopLeft = "TopLeft";
		const String TopRight = "TopRight";
		const String BottomLeft = "BottomLeft";
		const String BottomRight = "BottomRight";

		String[] DefaultStrings;
		int[] DefaultStringIDs;

		String[] KidsZoneStrings;
		int[] KidsZoneStringIDs;

		String[] CarerInfoStrings;
		int[] CarerInfoStringIDs;

		String[] LoginStrings;
		int[] LoginStringIDs;

		String[] MoreInfoStrings;
		int[] MoreInfoStringIDs;

		String[] MyNyasStrings;
		int[] MyNyasStringIDs;

		String[] OptionStrings;
		int[] OptionStringIDs;

		/// <summary>
		/// OnCreate used by activity when it is first created.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature (WindowFeatures.NoTitle); //removing top bar from the app
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.HomepageLayout);
			SetupLayouts (); //setting up all button and view layouts
			SetupResourceStringIDs (); //setting up the string ids associated with each possible state of this activity
			ApplyState (DEFAULT_HOME_STATE); //initialising default state of the page
			ApplyButtonListeners (); //setting up button listeners
			ApplyRandomBubbleMessage ();
			ResizeButtonText (DEFAULT_TEXT_SIZE); //resizing the button text size to enforce it, as the xml does not seem to update it correctly

			//setting up filemanager
			MyFileManager = new FileManager(BaseContext.FilesDir.AbsolutePath);
		}

		/// <summary>
		/// Sets up all the layouts with the help of displaymetrics that interrogates the screen resolution of the used device.
		/// </summary>
		private void SetupLayouts()
		{
			//holding the display metrics in a variable
			var metrics = Resources.DisplayMetrics;
			var widthInDp = ConvertPixelsToDp(metrics.WidthPixels);
			var heightInDp = ConvertPixelsToDp(metrics.HeightPixels);

			//setting the button dimensions to the width of the screen divided by two and a little margin (15 pixels)
			int buttonDimensions = metrics.WidthPixels / 2 - 15; //!!!
			int remainingScreenHeight = metrics.HeightPixels; //this is used to keep track of the remaining screen height

			//finding the buttons via their id and setting their background to the gradient drawable
			buttonTopLeft = (Button) FindViewById (Resource.Id.buttonTopLeft);
			//buttonTopLeft.SetBackgroundDrawable (Resources.GetDrawable(Resource.Drawable.rectangle_blue_gradient));
			buttonTopLeft.Background = Resources.GetDrawable(Resource.Drawable.rectangle_blue_gradient); //new way of setting background drawable after library update
			buttonTopLeft.SetWidth (buttonDimensions); //using same dimensions for width and height to make buttons square
			buttonTopLeft.SetHeight (buttonDimensions);
			remainingScreenHeight -= buttonDimensions;

			buttonTopRight = (Button) FindViewById (Resource.Id.buttonTopRight);
			buttonTopRight.Background = Resources.GetDrawable(Resource.Drawable.rectangle_green_gradient);
			buttonTopRight.SetWidth (buttonDimensions);
			buttonTopRight.SetHeight (buttonDimensions);
			remainingScreenHeight -= buttonDimensions;

			buttonBottomLeft = (Button) FindViewById (Resource.Id.buttonBottomLeft);
			buttonBottomLeft.Background = Resources.GetDrawable(Resource.Drawable.rectangle_orange_gradient);
			buttonBottomLeft.SetWidth (buttonDimensions);
			buttonBottomLeft.SetHeight (buttonDimensions);
			remainingScreenHeight -= buttonDimensions;

			buttonBottomRight = (Button) FindViewById (Resource.Id.buttonBottomRight);
			buttonBottomRight.Background = Resources.GetDrawable(Resource.Drawable.rectangle_purple_gradient);
			buttonBottomRight.SetWidth (buttonDimensions);
			buttonBottomRight.SetHeight (buttonDimensions);
			remainingScreenHeight -= buttonDimensions;

			nyasLogo = (ImageView)FindViewById (Resource.Id.nyasLogo);
			nyasLogo.SetAdjustViewBounds (true); //making the imageview scaleable
			nyasLogo.SetMaxWidth (metrics.WidthPixels / 2); //scale to half the screen size
			nyasLogo.RefreshDrawableState (); //force redraw / refresh
			nyasLogo.Click += delegate {
				//starting a webview with the given url
				var uri = Android.Net.Uri.Parse ("https://www.nyas.net/"); //http://developer.xamarin.com/recipes/android/fundamentals/intent/open_a_webpage_in_the_browser_application/
				var intent = new Intent (Intent.ActionView, uri); 
				StartActivity (intent);
			};

			speechBubble = (ImageView)FindViewById (Resource.Id.speechBubble);
			speechBubble.SetAdjustViewBounds (true); //making the imageview scaleable
			speechBubble.SetMaxWidth (metrics.WidthPixels / 2); //scale to half the screen size
			speechBubble.RefreshDrawableState (); //force redraw / refresh
			speechBubble.Click += delegate {
				ApplyState (DEFAULT_HOME_STATE); //speech bubble takes you back to the default menu
			};

			speechBubbleText = (TextView)FindViewById (Resource.Id.speechBubbleText);
			//setting speech bubble text max height and width
			speechBubbleText.SetMaxWidth ((int) (metrics.WidthPixels / 2.3));
			speechBubbleText.SetMaxHeight (metrics.WidthPixels / 2);

			bottomTextBox = (TextView)FindViewById (Resource.Id.BottomInfoText);
			bottomTextBox.Background =  Resources.GetDrawable(Resource.Drawable.rectangle_peach_gradient);
			bottomTextBox.Gravity = GravityFlags.CenterHorizontal;
			bottomTextBox.SetText (DEFAULT_BOX_CONTENT); //setting the default resource for the bottom box of text
			currentBoxContent = DEFAULT_BOX_CONTENT; //remembering its state

			//adding the buttons into an array, as you would read them from top left to bottom right
			buttons = new Button[4];
			buttons [0] = buttonTopLeft;
			buttons [1] = buttonTopRight;
			buttons [2] = buttonBottomLeft;
			buttons [3] = buttonBottomRight;
		}

		/// <summary>
		/// Converts the pixels to density pixels (dp).
		/// </summary>
		/// <returns>The pixels in dp</returns>
		/// <param name="pixelValue">Pixel value.</param>
		private int ConvertPixelsToDp(float pixelValue)
		{
			return (int) ((pixelValue)/Resources.DisplayMetrics.Density); //returning converted pixelValue as an integer
		}

		/// <summary>
		/// Setups the resource string I ds.
		/// Getting the relevant array (of Strings) from the Strings.xml file.
		/// Saving the resource ids of the Strings contained within the arrays in their respective integer array.
		/// This is done so the activity can remember or interrogate its on state istead of just changing the strings within the buttons.
		/// </summary>
		private void SetupResourceStringIDs(){

			var DefaultArray = Resources.ObtainTypedArray (Resource.Array.DefaultHomeScreenStrings);
			if (DefaultArray.Length() != 4) {
				throw new Exception ("Default Home Page array was not equal to 4. Please fix this in the Strings.xml file."); //this array must be the length of 4!
			}
			DefaultStrings = new String[DefaultArray.Length ()]; //String array initialised with the size of the according array
			DefaultStringIDs = new int[DefaultArray.Length()]; //Int array initialised to the amount of strings within the according array
			
			var KidsZoneArray = Resources.ObtainTypedArray (Resource.Array.KidsZoneScreenStrings);
			if (KidsZoneArray.Length() != 4) {
				throw new Exception ("Kids Zone array was not equal to 4. Please fix this in the Strings.xml file."); //this array must be the length of 4!
			}
			KidsZoneStrings = new String[KidsZoneArray.Length()];
			KidsZoneStringIDs = new int[KidsZoneArray.Length()];

			var CarerInfoArray = Resources.ObtainTypedArray (Resource.Array.CarerInfoStrings);
			if (CarerInfoArray.Length() != 4) {
				throw new Exception ("Carer Info array was not equal to 4. Please fix this in the Strings.xml file."); //this array must be the length of 4!
			}
			CarerInfoStrings = new String[CarerInfoArray.Length()];
			CarerInfoStringIDs = new int[CarerInfoArray.Length()];

			var LoginArray = Resources.ObtainTypedArray (Resource.Array.LoginStrings);
			if (LoginArray.Length () != 4) {
				throw new Exception ("Login array was not equal to 4. Please fix this in the Strings.xml file."); //this array must be the length of 4!
			}
			LoginStrings = new string[LoginArray.Length()];
			LoginStringIDs = new int[LoginArray.Length()];

			var MoreInfoArray = Resources.ObtainTypedArray (Resource.Array.MoreInfoStrings);
			if (MoreInfoArray.Length () != 4) {
				throw new Exception ("Login array was not equal to 4. Please fix this in the Strings.xml file."); //this array must be the length of 4!
			}
			MoreInfoStrings = new string[MoreInfoArray.Length()];
			MoreInfoStringIDs = new int[MoreInfoArray.Length()];

			var MyNyasArray = Resources.ObtainTypedArray (Resource.Array.MyNYASStrings);
			if (MyNyasArray.Length () != 4) {
				throw new Exception ("Login array was not equal to 4. Please fix this in the Strings.xml file."); //this array must be the length of 4!
			}
			MyNyasStrings = new string[MyNyasArray.Length()];
			MyNyasStringIDs = new int[MyNyasArray.Length()];

			var OptionsArray = Resources.ObtainTypedArray (Resource.Array.OptionStrings);
			if (OptionsArray.Length () != 4) {
				throw new Exception ("Login array was not equal to 4. Please fix this in the Strings.xml file."); //this array must be the length of 4!
			}
			OptionStrings = new string[OptionsArray.Length()];
			OptionStringIDs = new int[OptionsArray.Length()];

			for (int i = 0; i < 4; i++) { //every array has been tested to have exactly 4 items in it, so we can loop exactly 4 times
				DefaultStrings [i] = DefaultArray.GetString (i);
				DefaultStringIDs [i] = DefaultArray.GetResourceId (i, -1); //default value of -1 if resource id was not found

				KidsZoneStrings [i] = KidsZoneArray.GetString (i);
				KidsZoneStringIDs [i] = KidsZoneArray.GetResourceId (i, -1);

				CarerInfoStrings [i] = CarerInfoArray.GetString (i);
				CarerInfoStringIDs [i] = CarerInfoArray.GetResourceId (i, -1);

				LoginStrings [i] = LoginArray.GetString (i);
				LoginStringIDs [i] = LoginArray.GetResourceId (i, -1);

				MoreInfoStrings [i] = MoreInfoArray.GetString (i);
				MoreInfoStringIDs [i] = MoreInfoArray.GetResourceId (i, -1);

				MyNyasStrings [i] = MyNyasArray.GetString (i);
				MyNyasStringIDs [i] = MyNyasArray.GetResourceId (i, -1);

				OptionStrings [i] = OptionsArray.GetString (i);
				OptionStringIDs [i] = OptionsArray.GetResourceId (i, -1);
			}
		}

		/// <summary>
		/// Applies the specified state to this activity.
		/// The previous and current state values will also be updated.
		/// </summary>
		/// <param name="state">Specified state</param>
		private void ApplyState(int state){ //apply the chosen state to this activity

			if (IsTextBig ()) { //resize button text to default size if they are big
				ResizeButtonText (DEFAULT_TEXT_SIZE); //This is done because only one state uses the big text (Login state) and all other do not, that's why it isn't just in every other case, as it would be redundant.
			}

			//determine chosen state
			switch (state) {
			case DEFAULT_HOME_STATE:
				for (int i = 0; i < 4; i++) {
					buttons [i].SetText (DefaultStringIDs[i]); //changing the text of the buttons to the string ids associated with this state
					UpdateBottomTextBox (DEFAULT_BOX_CONTENT); //changing the content of the bottom box back to its default value
				}
				break;

			case KIDS_ZONE_STATE:
				for (int i = 0; i < 4; i++) {
					buttons [i].SetText (KidsZoneStringIDs[i]); //changing the text of the buttons to the string ids associated with this state
				}
				break;

			case CARER_INFO_STATE:
				for (int i = 0; i < 4; i++) {
					buttons [i].SetText (CarerInfoStringIDs[i]); //changing the text of the buttons to the string ids associated with this state
				}
				break;
			case LOGIN_STATE:
				if (PinEntered) { //if the pin was already entered, just go straight to MY_NYAS_STATE
					ApplyState (MY_NYAS_STATE);
					return;
				} else {
					CheckAndSetPin (); //check if the pin exists in the file and sets it if it does
					for (int i = 0; i < 4; i++) {
						buttons [i].SetText (LoginStringIDs [i]); //changing the text of the buttons to the string ids associated with this state
						ResizeButtonText (BIG_TEXT_SIZE); //make all text big
						UpdateBottomTextBox (PIN_COLON_BOX_CONTENT); //update the bottom text with "Pin:" ready for the pin to be put in
					}
				}
				break;
			case MORE_INFO_STATE:
				for (int i = 0; i < 4; i++) {
					buttons [i].SetText (MoreInfoStringIDs[i]); //changing the text of the buttons to the string ids associated with this state
				}
				break;
			case MY_NYAS_STATE:
				for (int i = 0; i < 4; i++) {
					buttons [i].SetText (MyNyasStringIDs[i]); //changing the text of the buttons to the string ids associated with this state
				}
				break;
			case OPTION_STATE:
				for (int i = 0; i < 4; i++) {
					buttons [i].SetText (OptionStringIDs[i]); //changing the text of the buttons to the string ids associated with this state
				}
				break;
			}
			previousState = currentState; //setting previous state
			currentState = state; //setting new state
		}

		/// <summary>
		/// Decides the action to be taken when a specific button has been pressed.
		/// The first part of the decision is based on the current state of the activity.
		/// Nested inside the first switch-case are other switch-cases that check for the selected button.
		/// Knowing the state of the activity and which button has been pressed will decide what happens.
		/// You can not access the current state of the button class via for example GetText(), such a method does not exist.
		/// This means that this activity works around this by assigning const values to the state of the button or the location to the button, see near class declaration.
		/// </summary>
		/// <param name="selectedButton">Selected button.</param>
		private void DecideAction(String selectedButton){ //decide which action the button press will cause
			switch (currentState) {
			case DEFAULT_HOME_STATE:
				switch (selectedButton) {
				case TopLeft:
					ApplyState (LOGIN_STATE);
					break;
				case TopRight:

					break;
				case BottomLeft:
					ApplyState (MORE_INFO_STATE);
					break;

				case BottomRight:
					ApplyState (CARER_INFO_STATE);
					break;
				}
				break;

			case KIDS_ZONE_STATE:
				switch(selectedButton){
				case TopLeft:
					
					break;
				case TopRight:

					break;
				case BottomLeft:

					break;

				case BottomRight:
					
					break;
				}
				break;

			case LOGIN_STATE:
				switch(selectedButton){
				case TopLeft:
					EnterPin (LoginStrings [0], LoginStringIDs [0]); //pass the relevant String to the method that takes care of loggin in with the pin
					break;
				case TopRight:
					EnterPin (LoginStrings [1], LoginStringIDs [1]);
					break;
				case BottomLeft:
					EnterPin (LoginStrings [2], LoginStringIDs [2]);
					break;
				case BottomRight:
					EnterPin (LoginStrings [3], LoginStringIDs [3]);
					break;
				}
				break;
			case MY_NYAS_STATE:
				switch (selectedButton) {
				case TopLeft:

					break;
				case TopRight:
					StartCalendarActivity ();
					break;
				case BottomLeft:

					break;

				case BottomRight:
					ApplyState (OPTION_STATE);
					break;
				}
				break;

			case MORE_INFO_STATE:
				switch (selectedButton) {
				case TopLeft:

					break;
				case TopRight:
					
					break;
				case BottomLeft:

					break;

				case BottomRight:
					//start a new webview with the given url
					var uri = Android.Net.Uri.Parse ("https://mydonate.bt.com/charities/nyas"); //http://developer.xamarin.com/recipes/android/fundamentals/intent/open_a_webpage_in_the_browser_application/
					var intent = new Intent (Intent.ActionView, uri); 
					StartActivity (intent);
					break;
				}
				break;

			case OPTION_STATE:
				switch(selectedButton){
				case TopLeft:
					MyFileManager.DeletePin (); //delete the pin
					PinSet = false;
					PinEntered = false;
					InputPin.Clear ();
					UserPin.Clear ();
					ApplyState(LOGIN_STATE); //promt the user to the login state
					break;
				case TopRight:

					break;
				case BottomLeft:

					break;

				case BottomRight:

					break;
				}
				break;

			}
		}

		/// <summary>
		/// This method is called by the buttons when in LOGIN_STATE.
		/// Checks for the current state of the Pin and appends it
		/// Compares the pin the user has put in 
		/// </summary>
		/// <param name="Symbol">Symbol.</param>
		/// <param name="resID">Res I.</param>
		private void EnterPin(String Symbol, int resID){
			//getting the expected UserPin from the file it was saved in
			if (InputPin.Count > 4) {
				InputPin.Clear (); //something went wrong and the pin is greater than it should be.
			}

			InputPin.Add (Symbol); //add the given Symbol to the InputPin
			AppendBottomTextBox (Symbol);

			if (InputPin.Count == 4) { //pin has been entered
				if (PinSet) { //if the pin has alraedy been set
					if (ComparePins (InputPin, UserPin)) { //CORRECT PIN ENTERED
						PinEntered = true;
						UpdateBottomTextBox (CORRECT_PIN_BOX_CONTENT);
						ApplyState (MY_NYAS_STATE);
					} else { //INCORRECT PIN ENTERED
						UpdateBottomTextBox (INCORRECT_PIN_BOX_CONTENT);
						InputPin.Clear ();
					}
				} else { //the pin has not already been set
					ClonePin(InputPin, UserPin); //clone the input pin into the userpin, you can not say UserPin = InputPin, as this will create a pointer refernce and update the UserPin whenever Inputpin is changed. This leads to any pin being accepted as correct.
					PinSet = true;
					UpdateBottomTextBox (PIN_CONFIRMED_BOX_CONTENT);
					MyFileManager.WritePin (ReturnPinAsString(InputPin));
					Console.WriteLine (ReturnPinAsString (InputPin));
					InputPin.Clear ();
				}
			}
		}

			/** REWORKING THIS LOGIC
			if (PinSet) { //the pin is already set
				if (InputPin.Count >= 3) { //the user has put in 4 characters
					InputPin.Add (Symbol); //add the given Symbol to the InputPin;
					AppendBottomTextBox (Symbol);
					//Compare the Input Pin to the expected values in Userpin
					if (ComparePins(InputPin, UserPin)) { //CORRECT PIN
						UpdateBottomTextBox(CORRECT_PIN_BOX_CONTENT);
						ApplyState (KIDS_ZONE_STATE); //go to kids zone
					} else { //INCORRECT PIN
						InputPin.Clear ();
						UpdateBottomTextBox (INCORRECT_PIN_BOX_CONTENT); //Tell the user in the bottom box that they entered an incorrect pin
					}
				} else {
					InputPin.Add (Symbol); //add the given Symbol to the InputPin;
					AppendBottomTextBox (Symbol);
				}
			} else if (InputPin.Count >= 4) { //the user already entered their first part of the verification process
				if (SecondInputPin.Count >= 3) { //they also entered their second part of the verification process
					SecondInputPin.Add (Symbol); //adding Symbol to the SecondInputPin
					AppendBottomTextBox (Symbol);
					if (ComparePins(InputPin, SecondInputPin)) { //if they are identical
						UserPin = InputPin; //setting UserPin
						PinSet = true; //pin has been set
						UpdateBottomTextBox (PIN_CONFIRMED_BOX_CONTENT); //update the box with the message that the pin has been confirmed and saved
						//TODO Save the pin inside UserPin in the shared preferences or a text file or other
					} else { //The two pins are NOT identical, reset them and promt the user
						InputPin.Clear ();
						SecondInputPin.Clear ();
						UpdateBottomTextBox (INCORRECT_PIN_BOX_CONTENT); //Tell the user in the bottom box that they entered an incorrect pin
					}
				} else { //SecondInputPin is not yet completed
					if (SecondInputPin.Count == 0) { //if there is nothing in the in input at the moment reset the box to say "Confirm:"
						UpdateBottomTextBox (CONFIRM_PIN_BOX_CONTENT);
					}
					SecondInputPin.Add (Symbol); //adding Symbol to the SecondInputPin
					AppendBottomTextBox (Symbol);
				}
			} else { //the first Pin has not been entered yet
				if (InputPin.Count == 0) { //if there is nothing in the in input at the moment reset the box to say "Pin:"
					UpdateBottomTextBox (PIN_COLON_BOX_CONTENT);
				}
				InputPin.Add (Symbol); //add the Symbol to the pin
				AppendBottomTextBox (Symbol);
			} 
		} */

		/// <summary>
		/// Resizes the button text.
		/// </summary>
		/// <param name="mode">Mode to be set</param>
		private void ResizeButtonText(int mode){ //resize all buttons text
			switch (mode) {
			case DEFAULT_TEXT_SIZE: //to default text size
				foreach (Button b in buttons) {
					b.SetTextSize (Android.Util.ComplexUnitType.Sp, Resources.GetDimension (Resource.Dimension.ButtonTextSize));
				}
				currentTextSize = DEFAULT_TEXT_SIZE;
				break;
			case BIG_TEXT_SIZE: //to big text
				foreach (Button b in buttons) {
					b.SetTextSize (Android.Util.ComplexUnitType.Sp, Resources.GetDimension (Resource.Dimension.SpecialButtonSize));
				}
				currentTextSize = BIG_TEXT_SIZE;
				break;
			}
		}

		/// <summary>
		/// Compares to list of strings if each element is identical.
		/// This method presumes that the second list is the same size as the first.
		/// Make sure the lists are the same length.
		/// </summary>
		/// <returns><c>true</c> if all ements are identical<c>false</c> if any element is not identical </returns>
		/// <param name="l1">First list of comparison</param>
		/// <param name="l2">Second list of comparison</param>
		private bool ComparePins(List<String> l1, List<String> l2){ //compare two lists of strings for their content
			for (int i = 0; i < l1.Count; i++) { //taking the size of the first list and assuming they are the same size
				if(!l1 [i].Equals(l2[i])){ return false; } //if the elements are not euqal return false
				Console.WriteLine(l1[i] + l2[i]);
			}
			return true; //the loop has run through, return true
		}

		/// <summary>
		/// Returns the pin as string.
		/// </summary>
		/// <returns>The pin as string.</returns>
		/// <param name="Pin">Pin as list of strings</param>
		private String ReturnPinAsString(List<String> Pin){
			String PinString = "";
			foreach(String s in Pin){
				PinString += s;
			}
			return PinString;
		}

		/// <summary>
		/// Returns the pin as list.
		/// </summary>
		/// <returns>The pin as list.</returns>
		/// <param name="Pin">Pin as string</param>
		private List<String> ReturnPinAsList(String Pin){
			List<String> temp = new List<String> ();
			foreach (char c in Pin) {
				temp.Add (c.ToString());
			}
			return temp;
		}

		/// <summary>
		/// Checks if the pin exists and if it does it will be set.
		/// </summary>
		/// <returns><c>true</c>, if and set pin was checked, <c>false</c> otherwise.</returns>
		private bool CheckAndSetPin(){
			try{
			if (MyFileManager.ReadPin ().Equals ("No pin set")) {
				return false;
			}
			} catch (NullReferenceException){
				return false;
			}
			UserPin = ReturnPinAsList (MyFileManager.ReadPin ()); //setting user pin
			PinSet = true;
			return true;
		}

		/// <summary>
		/// Clones the pin from one list of strings to another
		/// </summary>
		/// <param name="Origin">Origin that is copied from</param>
		/// <param name="Destination">Destination that is copied to</param>
		private void ClonePin(List<String> Origin, List<String> Destination){
			foreach (String s in Origin) {
				Destination.Add (s);
			}
		}

		/// <summary>
		/// Updates the bottom text box.
		/// </summary>
		/// <param name="resID">Res ID.</param>
		private void UpdateBottomTextBox(int resID){ //update the text box with a resource id
			bottomTextBox.SetText (resID); //setting the text to the chosen String from Strings.xml
			currentBoxContent = resID; //remembering the state of the box
		}

		/// <summary>
		/// Appends the bottom text box.
		/// </summary>
		/// <param name="text">Text as String</param>
		private void AppendBottomTextBox(String text){
			bottomTextBox.Append (text); //appending the text with the given text
		}

		/// <summary>
		/// Determines the state of the bottom box content and returns true if the passed value is the current state
		/// </summary>
		/// <returns><c>true</c> if bottom box content is identical to the specified resID; otherwise, <c>false</c>.</returns>
		/// <param name="resID">Res I.</param>
		private bool IsBottomBoxContent(int resID){ //returns true if the current state is the same as the passed value, otherwise returns false
			return currentBoxContent == resID;
		}

		/// <summary>
		/// Determines whether or not the is text big.
		/// </summary>
		/// <returns><c>true</c> if this instance is text big; otherwise, <c>false</c>.</returns>
		private bool IsTextBig(){ //is the text big?
			return currentTextSize == BIG_TEXT_SIZE; //returns true if the current text size is BIG_TEXT_SIZE, otherwise returns false
		}

		/// <summary>
		/// Applies the button listeners.
		/// </summary>
		private void ApplyButtonListeners(){
			buttonTopLeft.Click += delegate {
				DecideAction(TopLeft);
			};

			buttonTopRight.Click += delegate {
				DecideAction(TopRight);
			};

			buttonBottomLeft.Click += delegate {
				DecideAction(BottomLeft);
			};

			buttonBottomRight.Click += delegate {
				DecideAction(BottomRight);
			};
		}

		/// <summary>
		/// Applies the random bubble message.
		/// </summary>
		private void ApplyRandomBubbleMessage(){ //apply a random message to the text view on top of the speech bubble
			Random randy = new Random ();
			int temp = randy.Next (1, 101) / 25; //roll random number between 1 (inclusive) and 101 (exclusive)
			switch (temp) {
			case 1:
				speechBubbleText.SetText (Resource.String.BubbleMsg1);
				break;
			case 2:
				speechBubbleText.SetText (Resource.String.BubbleMsg2);
				break;
			case 3:
				speechBubbleText.SetText (Resource.String.BubbleMsg3);
				break;
			case 4:
				speechBubbleText.SetText (Resource.String.BubbleMsg4);
				break;
			default:
				speechBubbleText.SetText (Resource.String.BubbleMsg1);
				break;
			}
		}

		/// <summary>
		/// Called when the activity has detected the user's press of the back
		///  key.
		/// This is overriden so that only when the screen is in default state it will quit the app.
		/// Otherwise it will return to the previous state.
		/// </summary>
		public override void OnBackPressed ()
		{
			if (previousState == -1 || currentState == DEFAULT_HOME_STATE) { //default value, quit the app!
				base.OnBackPressed ();
			} else {
			ApplyState (previousState); //go to the previous state of the application
			}
		}

		/// <summary>
		/// Starts the calendar activity.
		/// </summary>
		private void StartCalendarActivity(){
			StartActivity (typeof(CalendarActivity)); //starting a new calendar activity
		}
	}
}
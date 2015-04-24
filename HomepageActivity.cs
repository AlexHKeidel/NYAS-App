
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
	[Activity (Label = "HomepageActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class HomepageActivity : Activity
	{
		Button buttonTopLeft, buttonTopRight, buttonBottomLeft, buttonBottomRight; //four buttons represented on the screen in order as you would read (top left to bottom right)
		Button[] buttons;
		ImageView speechBubble, nyasLogo;
		TextView speechBubbleText;

		int currentState;
		const int DEFAULT_HOME_STATE = 0;
		const int KIDS_ZONE_STATE = 1;
		const int CARER_INFO_STATE = 2;

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


		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature (WindowFeatures.NoTitle); //removing top bar from the app
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.HomepageLayout);
			setupLayouts (); //setting up all button and view layouts
			setupResourceStringIDs (); //setting up the string ids associated with each possible state of this activity
			applyState (DEFAULT_HOME_STATE); //initialising default state of the page
			applyButtonListeners (); //setting up button listeners
			applyRandomBubbleMessage ();
		}

		private void setupLayouts()
		{
			//holding the display metrics in a variable
			var metrics = Resources.DisplayMetrics;
			var widthInDp = ConvertPixelsToDp(metrics.WidthPixels);
			var heightInDp = ConvertPixelsToDp(metrics.HeightPixels);

			//setting the button dimensions to the width of the screen divided by two and a little margin (15 pixels)
			int buttonDimensions = metrics.WidthPixels / 2 - 15; //!!!

			//finding the buttons via their id and setting their background to the gradient drawable
			buttonTopLeft = (Button) FindViewById (Resource.Id.buttonTopLeft);
			buttonTopLeft.SetBackgroundDrawable (Resources.GetDrawable(Resource.Drawable.rectangle_blue_gradient));
			buttonTopLeft.SetWidth (buttonDimensions); //using same dimensions for width and height to make buttons square
			buttonTopLeft.SetHeight (buttonDimensions);

			buttonTopRight = (Button) FindViewById (Resource.Id.buttonTopRight);
			buttonTopRight.SetBackgroundDrawable (Resources.GetDrawable(Resource.Drawable.rectangle_green_gradient));
			buttonTopRight.SetWidth (buttonDimensions);
			buttonTopRight.SetHeight (buttonDimensions);

			buttonBottomLeft = (Button) FindViewById (Resource.Id.buttonBottomLeft);
			buttonBottomLeft.SetBackgroundDrawable (Resources.GetDrawable(Resource.Drawable.rectangle_orange_gradient));
			buttonBottomLeft.SetWidth (buttonDimensions);
			buttonBottomLeft.SetHeight (buttonDimensions);

			buttonBottomRight = (Button) FindViewById (Resource.Id.buttonBottomRight);
			buttonBottomRight.SetBackgroundDrawable (Resources.GetDrawable(Resource.Drawable.rectangle_purple_gradient));
			buttonBottomRight.SetWidth (buttonDimensions);
			buttonBottomRight.SetHeight (buttonDimensions);

			nyasLogo = (ImageView)FindViewById (Resource.Id.nyasLogo);
			nyasLogo.SetAdjustViewBounds (true); //making the imageview scaleable
			nyasLogo.SetMaxWidth (metrics.WidthPixels / 2); //scale to half the screen size
			nyasLogo.RefreshDrawableState (); //force redraw / refresh

			speechBubble = (ImageView)FindViewById (Resource.Id.speechBubble);
			speechBubble.SetAdjustViewBounds (true); //making the imageview scaleable
			speechBubble.SetMaxWidth (metrics.WidthPixels / 2); //scale to half the screen size
			speechBubble.RefreshDrawableState (); //force redraw / refresh

			speechBubbleText = (TextView)FindViewById (Resource.Id.speechBubbleText);
			//setting speech bubble text max height and width
			speechBubbleText.SetMaxWidth ((int) (metrics.WidthPixels / 2.3));
			speechBubbleText.SetMaxHeight (metrics.WidthPixels / 2);

			//adding the buttons into an array, as you would read them from top left to bottom right
			buttons = new Button[4];
			buttons [0] = buttonTopLeft;
			buttons [1] = buttonTopRight;
			buttons [2] = buttonBottomLeft;
			buttons [3] = buttonBottomRight;
		}

		private int ConvertPixelsToDp(float pixelValue)
		{
			return (int) ((pixelValue)/Resources.DisplayMetrics.Density); //returning converted pixelValue as an integer
		}

		private void setupResourceStringIDs(){
			//getting the relevant array (of Strings) from the Strings.xml file
			//saving the resource ids of the Strings contained within the arrays in their respective integer array
			//this is done because you can not set the text of a button to a string, but only to a resource id
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

			for (int i = 0; i < 4; i++) {
				DefaultStrings [i] = DefaultArray.GetString (i);
				DefaultStringIDs [i] = DefaultArray.GetResourceId (i, -1); //default value of -1 if resource id was not found

				KidsZoneStrings [i] = KidsZoneArray.GetString (i);
				KidsZoneStringIDs [i] = KidsZoneArray.GetResourceId (i, -1);

				CarerInfoStrings [i] = CarerInfoArray.GetString (i);
				CarerInfoStringIDs [i] = CarerInfoArray.GetResourceId (i, -1);
			}
		}

		private void applyState(int state){ //apply the chosen state to this activity
			//determine chosen state
			switch (state) {
			case DEFAULT_HOME_STATE:
				for (int i = 0; i < 4; i++) {
					buttons [i].SetText (DefaultStringIDs[i]); //changing the text of the buttons to the string ids associated with this state
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
			}
			currentState = state; //setting new state
		}

		private void decideAction(String selectedButton){
			switch (currentState) {
			case DEFAULT_HOME_STATE:
				switch (selectedButton) {
				case TopLeft:
					enterKidsZone ();
					break;
				case TopRight:

					break;
				case BottomLeft:

					break;

				case BottomRight:
					applyState (CARER_INFO_STATE);
					break;
				}
				break;
			}
		}

		private void enterKidsZone(){
			//TODO pin validation of the user
			applyState (KIDS_ZONE_STATE);
		}


		private void applyButtonListeners(){
			buttonTopLeft.Click += delegate {
				decideAction(TopLeft);
			};

			buttonTopRight.Click += delegate {
				decideAction(TopRight);
			};

			buttonBottomLeft.Click += delegate {
				decideAction(BottomLeft);
			};

			buttonBottomRight.Click += delegate {
				decideAction(BottomRight);
			};
		}

		private void applyRandomBubbleMessage(){ //apply a random message to the text view on top of the speech bubble
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
	}
}


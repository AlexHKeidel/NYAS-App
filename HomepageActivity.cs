
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
	[Activity (Label = "HomepageActivity")]			
	public class HomepageActivity : Activity
	{
		Button buttonTopLeft, buttonTopRight, buttonBottomLeft, buttonBottomRight; //four buttons represented on the screen in order as you would read (top left to bottom right)
		Button[] buttons;
		ImageView speechBubble, nyasLogo;
		TextView speechBubbleText;

		int currentState;
		const int DEFAULT_HOME_STATE = 0;
		const int KIDS_ZONE_STATE = 1;

		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature (WindowFeatures.NoTitle); //removing top bar from the app
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.HomepageLayout);
			setupLayouts (); //setting up all button and view layouts
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
			var dp = (int) ((pixelValue)/Resources.DisplayMetrics.Density);
			return dp;
		}

		private void applyState(int state){ //apply the chosen state to this activity
			//determine chosen state
			Console.WriteLine ("state = " + state + " DEFAULT_HOME_STATE = " + DEFAULT_HOME_STATE);
			switch (state) {

			case DEFAULT_HOME_STATE:
				currentState = state;
				//getting the chosen array of strings from the Strings.xml file
				var defR = Resources.ObtainTypedArray(Resource.Array.DefaultHomeScreenStrings); //getting an array of the RESOURCE IDS NOT STRINGS FFS HOLY SHEET OMG WHY DOES BUTTON.SETTEXT ONY ACCEPT FUCKING RESOURCE VALUES..........
				for (int i = 0; i < defR.Length(); i++) {
					var defID = defR.GetResourceId(i, -1); //getting the resource id from the specified part of the string array in Strings.xml
					//REMEMBER: FOR THIS TO WORK THE STRINGS INSIDE THE STRING ARRAY MUST BE REFERNCES TO STRINGS AS IN @string/BLA not just BLA
					//apply the text to the buttons
					buttons [i].SetText (defID); //THIS ONLY ACCEPTS RESOURCE IDS NOT STRINGS! OKAY XAMARIN, OKAY, HAVE IT YOUR BLOODY WAY!
				}
				break;

			case KIDS_ZONE_STATE:
				var kidsR = Resources.ObtainTypedArray(Resource.Array.KidsZoneScreenStrings);
				for (int i = 0; i < kidsR.Length(); i++){
					var kidsID = kidsR.GetResourceId (i, -1);
					buttons [i].SetText (kidsID);
				}
				break;
			}
		}

		private void applyButtonListeners(){
			buttonTopLeft.Click += delegate {
				applyState (KIDS_ZONE_STATE);
			};

			buttonTopRight.Click += delegate {
				applyState (DEFAULT_HOME_STATE);
			};
		}

		public void applyRandomBubbleMessage(){ //apply a random message to the text view on top of the speech bubble
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


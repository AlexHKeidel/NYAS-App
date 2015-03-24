
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
		ImageView speechBubble, nyasLogo;
		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature (WindowFeatures.NoTitle); //removing top bar from the app
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.HomepageLayout);
			setupLayouts ();
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
			buttonTopLeft.SetWidth (buttonDimensions);
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

		}

		private int ConvertPixelsToDp(float pixelValue)
		{
			var dp = (int) ((pixelValue)/Resources.DisplayMetrics.Density);
			return dp;
		}
	}
}


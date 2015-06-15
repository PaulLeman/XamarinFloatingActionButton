using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using XamarinFloatingActionButton;

namespace XamarinFloatingActionButton.Sample
{
    [Activity(Label = "XamarinFloatingActionButton.Sample", MainLauncher = true)]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_main);

            //FindViewById(Resource.Id.pink_icon).Click += (s, e) =>
            //{
            //    Toast.MakeText(this, "Clicked pink Floating Action Button", ToastLength.Short).Show();
            //};

            //FloatingActionButton button = FindViewById<FloatingActionButton>(Resource.Id.setter);
            //button.Size = FloatingActionButton.SIZE_MINI;
            //button.setColorNormalResId(Resource.Color.pink);
            //button.setColorPressedResId(Resource.Color.pink_pressed);
            //button.setIcon(Resource.Drawable.ic_fab_star);
            //button.setStrokeVisible(false);

            View actionB = FindViewById(Resource.Id.action_b);

            FloatingActionButton actionC = new FloatingActionButton(BaseContext);
            actionC.setTitle("Hide/Show Action above");
            actionC.Click += (s, e) =>
            {
                actionB.Visibility = (actionB.Visibility == ViewStates.Gone ? ViewStates.Visible : ViewStates.Gone);
            };


            FindViewById<FloatingActionsMenu>(Resource.Id.multiple_actions).addButton(actionC);

            //FloatingActionButton removeAction = (FloatingActionButton)FindViewById(Resource.Id.button_remove);
            //removeAction.Click += (s, e) =>
            //{
            //    FindViewById<FloatingActionsMenu>(Resource.Id.multiple_actions_down).removeButton(removeAction);
            //};

            //ShapeDrawable drawable = new ShapeDrawable(new OvalShape());
            //drawable.Paint.Color = Resources.GetColor(Resource.Color.white);
            //((FloatingActionButton)FindViewById(Resource.Id.setter_drawable)).setIconDrawable(drawable);

            //FloatingActionButton actionA = FindViewById<FloatingActionButton>(Resource.Id.action_a);
            //actionA.Click += (s, e) =>
            //{
            //    actionA.setTitle("Action A clicked");
            //};

            //// Test that FAMs containing FABs with visibility GONE do not cause crashes
            //FindViewById(Resource.Id.button_gone).Visibility = ViewStates.Gone;

            //FloatingActionsMenu rightLabels = FindViewById<FloatingActionsMenu>(Resource.Id.right_labels);
            //FloatingActionButton addedOnce = new FloatingActionButton(this);
            //addedOnce.setTitle("Added once");
            //rightLabels.addButton(addedOnce);

            //FloatingActionButton addedTwice = new FloatingActionButton(this);
            //addedTwice.setTitle("Added twice");
            //rightLabels.addButton(addedTwice);
            //rightLabels.removeButton(addedTwice);
            //rightLabels.addButton(addedTwice);
        }
    }
}


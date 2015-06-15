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
using Android.Animation;
using Android.Util;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Views.Animations;
using Android.Graphics;
using Java.Interop;


namespace XamarinFloatingActionButton
{
    [Register("com.getbase.floatingactionbutton.FloatingActionsMenu")]
    public class FloatingActionsMenu : ViewGroup
    {
        public const int EXPAND_UP = 0;
        public const int EXPAND_DOWN = 1;
        public const int EXPAND_LEFT = 2;
        public const int EXPAND_RIGHT = 3;

        public const int LABELS_ON_LEFT_SIDE = 0;
        public const int LABELS_ON_RIGHT_SIDE = 1;

        private const int ANIMATION_DURATION = 300;
        private const float COLLAPSED_PLUS_ROTATION = 0f;
        private const float EXPANDED_PLUS_ROTATION = 90f + 45f;

        private int mAddButtonPlusColor;
        private int mAddButtonColorNormal;
        private int mAddButtonColorPressed;
        private int mAddButtonSize;
        private bool mAddButtonStrokeVisible;
        private int mExpandDirection;

        private int mButtonSpacing;
        private int mLabelsMargin;
        private int mLabelsVerticalOffset;

        private bool mExpanded;

        private AnimatorSet mExpandAnimation = (AnimatorSet)new AnimatorSet().SetDuration(ANIMATION_DURATION);
        private AnimatorSet mCollapseAnimation = (AnimatorSet)new AnimatorSet().SetDuration(ANIMATION_DURATION);
        private AddFloatingActionButton mAddButton;
        private RotatingDrawable mRotatingDrawable;
        private int mMaxButtonWidth;
        private int mMaxButtonHeight;
        private int mLabelsStyle;
        private int mLabelsPosition;
        private int mButtonsCount;

        private TouchDelegateGroup mTouchDelegateGroup;

        private OnFloatingActionsMenuUpdateListener mListener;

        public interface OnFloatingActionsMenuUpdateListener
        {
            void onMenuExpanded();
            void onMenuCollapsed();
        }

        public FloatingActionsMenu(Context context)
            : this(context, null)
        {
        }

        public FloatingActionsMenu(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            init(context, attrs);
        }

        public FloatingActionsMenu(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            init(context, attrs);
        }

        private void init(Context context, IAttributeSet attributeSet)
        {
            mButtonSpacing = (int)(Resources.GetDimension(Resource.Dimension.fab_actions_spacing) - Resources.GetDimension(Resource.Dimension.fab_shadow_radius) - Resources.GetDimension(Resource.Dimension.fab_shadow_offset));
            mLabelsMargin = Resources.GetDimensionPixelSize(Resource.Dimension.fab_labels_margin);
            mLabelsVerticalOffset = Resources.GetDimensionPixelSize(Resource.Dimension.fab_shadow_offset);

            mTouchDelegateGroup = new TouchDelegateGroup(this);
            TouchDelegate = mTouchDelegateGroup;

            TypedArray attr = context.ObtainStyledAttributes(attributeSet, Resource.Styleable.FloatingActionsMenu, 0, 0);
            mAddButtonPlusColor = attr.GetColor(Resource.Styleable.FloatingActionsMenu_fab_addButtonPlusIconColor, getColor(Android.Resource.Color.White));
            mAddButtonColorNormal = attr.GetColor(Resource.Styleable.FloatingActionsMenu_fab_addButtonColorNormal, getColor(Android.Resource.Color.HoloBlueDark));
            mAddButtonColorPressed = attr.GetColor(Resource.Styleable.FloatingActionsMenu_fab_addButtonColorPressed, getColor(Android.Resource.Color.HoloBlueLight));
            mAddButtonSize = attr.GetInt(Resource.Styleable.FloatingActionsMenu_fab_addButtonSize, FloatingActionButton.SIZE_NORMAL);
            mAddButtonStrokeVisible = attr.GetBoolean(Resource.Styleable.FloatingActionsMenu_fab_addButtonStrokeVisible, true);
            mExpandDirection = attr.GetInt(Resource.Styleable.FloatingActionsMenu_fab_expandDirection, EXPAND_UP);
            mLabelsStyle = attr.GetResourceId(Resource.Styleable.FloatingActionsMenu_fab_labelStyle, 0);
            mLabelsPosition = attr.GetInt(Resource.Styleable.FloatingActionsMenu_fab_labelsPosition, LABELS_ON_LEFT_SIDE);
            attr.Recycle();

            if (mLabelsStyle != 0 && expandsHorizontally())
            {
                throw new Java.Lang.IllegalStateException("Action labels in horizontal expand orientation is not supported.");
            }

            createAddButton(context);
        }

        public void setOnFloatingActionsMenuUpdateListener(OnFloatingActionsMenuUpdateListener listener)
        {
            mListener = listener;
        }

        private bool expandsHorizontally()
        {
            return mExpandDirection == EXPAND_LEFT || mExpandDirection == EXPAND_RIGHT;
        }

        private class RotatingDrawable : LayerDrawable
        {
            public RotatingDrawable(Drawable drawable)
                : base(new Drawable[] { drawable })
            {
            }

            private float mRotation;

            [Export]
            public float getRotation()
            {
                return mRotation;
            }

            [Export]
            public void setRotation(float rotation)
            {
                mRotation = rotation;
                InvalidateSelf();
            }

            public override void Draw(Android.Graphics.Canvas canvas)
            {

                canvas.Save();
                canvas.Rotate(mRotation, Bounds.CenterX(), Bounds.CenterY());
                base.Draw(canvas);
                canvas.Restore();
            }
        }

        private class CustomAddFloatingActionButton : AddFloatingActionButton
        {
            Action<AddFloatingActionButton> _updateBackgroundDelegate;
            public delegate Drawable GetIconDrawableDelegate(Drawable baseIconDrawable);
            GetIconDrawableDelegate _getIconDrawableDelegate;

            public CustomAddFloatingActionButton(Context context, Action<AddFloatingActionButton> updateBackgroundDelegate, GetIconDrawableDelegate getIconDrawableDelegate)
                : base(context)
            {
                _updateBackgroundDelegate = updateBackgroundDelegate;
                _getIconDrawableDelegate = getIconDrawableDelegate;

                this.UpdateBackground();
            }

            protected override void UpdateBackground()
            {
                if (_updateBackgroundDelegate != null)
                    _updateBackgroundDelegate(this);
                base.UpdateBackground();
            }
            protected override Drawable getIconDrawable()
            {
                Drawable baseIconDrawable = base.getIconDrawable();
                if (_getIconDrawableDelegate == null)
                    return baseIconDrawable;
                else
                    return _getIconDrawableDelegate(baseIconDrawable);
            }
        }

        private void createAddButton(Context context)
        {
            mAddButton = new CustomAddFloatingActionButton(context,
                updateBackgroundDelegate: (addFloatingActionButton) =>
                    {
                        addFloatingActionButton.mPlusColor = mAddButtonPlusColor;
                        addFloatingActionButton.mColorNormal = mAddButtonColorNormal;
                        addFloatingActionButton.mColorPressed = mAddButtonColorPressed;
                        addFloatingActionButton.mStrokeVisible = mAddButtonStrokeVisible;
                    },
                getIconDrawableDelegate: (baseIconDrawable) =>
                    {
                        RotatingDrawable rotatingDrawable = new RotatingDrawable(baseIconDrawable);
                        mRotatingDrawable = rotatingDrawable;

                        OvershootInterpolator interpolator = new OvershootInterpolator();

                        ObjectAnimator collapseAnimator = ObjectAnimator.OfFloat(rotatingDrawable, "Rotation", EXPANDED_PLUS_ROTATION, COLLAPSED_PLUS_ROTATION);
                        ObjectAnimator expandAnimator = ObjectAnimator.OfFloat(rotatingDrawable, "Rotation", COLLAPSED_PLUS_ROTATION, EXPANDED_PLUS_ROTATION);

                        collapseAnimator.SetInterpolator(interpolator);
                        expandAnimator.SetInterpolator(interpolator);

                        mExpandAnimation.Play(expandAnimator);
                        mCollapseAnimation.Play(collapseAnimator);

                        return rotatingDrawable;

                    });


            mAddButton.Id = Resource.Id.fab_expand_menu_button;
            mAddButton.Size = mAddButtonSize;
            mAddButton.Click += (s, e) =>
            {
                toggle();
            };


            AddView(mAddButton, base.GenerateDefaultLayoutParams());
        }

        public void addButton(FloatingActionButton button)
        {
            AddView(button, mButtonsCount - 1);
            mButtonsCount++;

            if (mLabelsStyle != 0)
            {
                createLabels();
            }
        }

        public void removeButton(FloatingActionButton button)
        {
            RemoveView(button.getLabelView());
            RemoveView(button);
            button.SetTag(Resource.Id.fab_label, null);
            mButtonsCount--;
        }

        private int getColor(int colorResId)
        {
            return this.Resources.GetColor(colorResId);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            MeasureChildren(widthMeasureSpec, heightMeasureSpec);

            int width = 0;
            int height = 0;

            mMaxButtonWidth = 0;
            mMaxButtonHeight = 0;
            int maxLabelWidth = 0;

            for (int i = 0; i < mButtonsCount; i++)
            {
                View child = GetChildAt(i);

                if (child.Visibility == ViewStates.Gone)
                {
                    continue;
                }

                switch (mExpandDirection)
                {
                    case EXPAND_UP:
                    case EXPAND_DOWN:
                        mMaxButtonWidth = Math.Max(mMaxButtonWidth, child.MeasuredWidth);
                        height += child.MeasuredHeight;
                        break;
                    case EXPAND_LEFT:
                    case EXPAND_RIGHT:
                        width += child.MeasuredWidth;
                        mMaxButtonHeight = Math.Max(mMaxButtonHeight, child.MeasuredHeight);
                        break;
                }

                if (!expandsHorizontally())
                {
                    TextView label = (TextView)child.GetTag(Resource.Id.fab_label);
                    if (label != null)
                    {
                        maxLabelWidth = Math.Max(maxLabelWidth, label.MeasuredWidth);
                    }
                }
            }

            if (!expandsHorizontally())
            {
                width = mMaxButtonWidth + (maxLabelWidth > 0 ? maxLabelWidth + mLabelsMargin : 0);
            }
            else
            {
                height = mMaxButtonHeight;
            }

            switch (mExpandDirection)
            {
                case EXPAND_UP:
                case EXPAND_DOWN:
                    height += mButtonSpacing * (ChildCount - 1);
                    height = adjustForOvershoot(height);
                    break;
                case EXPAND_LEFT:
                case EXPAND_RIGHT:
                    width += mButtonSpacing * (ChildCount - 1);
                    width = adjustForOvershoot(width);
                    break;
            }

            SetMeasuredDimension(width, height);
        }

        private int adjustForOvershoot(int dimension)
        {
            return dimension * 12 / 10;
        }


        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            switch (mExpandDirection)
            {
                case EXPAND_UP:
                case EXPAND_DOWN:
                    bool expandUp = mExpandDirection == EXPAND_UP;

                    if (changed)
                    {
                        mTouchDelegateGroup.clearTouchDelegates();
                    }

                    int addButtonY = expandUp ? b - t - mAddButton.MeasuredHeight : 0;
                    // Ensure mAddButton is centered on the line where the buttons should be
                    int buttonsHorizontalCenter = mLabelsPosition == LABELS_ON_LEFT_SIDE
                        ? r - l - mMaxButtonWidth / 2
                        : mMaxButtonWidth / 2;
                    int addButtonLeft = buttonsHorizontalCenter - mAddButton.MeasuredWidth / 2;
                    mAddButton.Layout(addButtonLeft, addButtonY, addButtonLeft + mAddButton.MeasuredWidth, addButtonY + mAddButton.MeasuredHeight);

                    int labelsOffset = mMaxButtonWidth / 2 + mLabelsMargin;
                    int labelsXNearButton = mLabelsPosition == LABELS_ON_LEFT_SIDE
                        ? buttonsHorizontalCenter - labelsOffset
                        : buttonsHorizontalCenter + labelsOffset;

                    int nextY = expandUp ?
                        addButtonY - mButtonSpacing :
                        addButtonY + mAddButton.MeasuredHeight + mButtonSpacing;

                    for (int i = mButtonsCount - 1; i >= 0; i--)
                    {
                        View child = GetChildAt(i);

                        if (child == mAddButton || child.Visibility == ViewStates.Gone) continue;

                        int childX = buttonsHorizontalCenter - child.MeasuredWidth / 2;
                        int childY = expandUp ? nextY - child.MeasuredHeight : nextY;
                        child.Layout(childX, childY, childX + child.MeasuredWidth, childY + child.MeasuredHeight);

                        float collapsedTranslation = addButtonY - childY;
                        float expandedTranslation = 0f;

                        child.TranslationY = mExpanded ? expandedTranslation : collapsedTranslation;
                        child.Alpha = mExpanded ? 1f : 0f;

                        LayoutParams parameters = (LayoutParams)child.LayoutParameters;
                        parameters.mCollapseDir.SetFloatValues(expandedTranslation, collapsedTranslation);
                        parameters.mExpandDir.SetFloatValues(collapsedTranslation, expandedTranslation);
                        parameters.setAnimationsTarget(child);

                        View label = (View)child.GetTag(Resource.Id.fab_label);
                        if (label != null)
                        {
                            int labelXAwayFromButton = mLabelsPosition == LABELS_ON_LEFT_SIDE
                                ? labelsXNearButton - label.MeasuredWidth
                                : labelsXNearButton + label.MeasuredWidth;

                            int labelLeft = mLabelsPosition == LABELS_ON_LEFT_SIDE
                                ? labelXAwayFromButton
                                : labelsXNearButton;

                            int labelRight = mLabelsPosition == LABELS_ON_LEFT_SIDE
                                ? labelsXNearButton
                                : labelXAwayFromButton;

                            int labelTop = childY - mLabelsVerticalOffset + (child.MeasuredHeight - label.MeasuredHeight) / 2;

                            label.Layout(labelLeft, labelTop, labelRight, labelTop + label.MeasuredHeight);

                            Rect touchArea = new Rect(
                                Math.Min(childX, labelLeft),
                                childY - mButtonSpacing / 2,
                                Math.Max(childX + child.MeasuredWidth, labelRight),
                                childY + child.MeasuredHeight + mButtonSpacing / 2);
                            mTouchDelegateGroup.addTouchDelegate(new TouchDelegate(touchArea, child));

                            label.TranslationY = mExpanded ? expandedTranslation : collapsedTranslation;
                            label.Alpha = mExpanded ? 1f : 0f;

                            LayoutParams labelParams = (LayoutParams)label.LayoutParameters;
                            labelParams.mCollapseDir.SetFloatValues(expandedTranslation, collapsedTranslation);
                            labelParams.mExpandDir.SetFloatValues(collapsedTranslation, expandedTranslation);
                            labelParams.setAnimationsTarget(label);
                        }

                        nextY = expandUp ?
                            childY - mButtonSpacing :
                            childY + child.MeasuredHeight + mButtonSpacing;
                    }
                    break;

                case EXPAND_LEFT:
                case EXPAND_RIGHT:
                    bool expandLeft = mExpandDirection == EXPAND_LEFT;

                    int addButtonX = expandLeft ? r - l - mAddButton.MeasuredWidth : 0;
                    // Ensure mAddButton is centered on the line where the buttons should be
                    int addButtonTop = b - t - mMaxButtonHeight + (mMaxButtonHeight - mAddButton.MeasuredHeight) / 2;
                    mAddButton.Layout(addButtonX, addButtonTop, addButtonX + mAddButton.MeasuredWidth, addButtonTop + mAddButton.MeasuredHeight);

                    int nextX = expandLeft ?
                        addButtonX - mButtonSpacing :
                        addButtonX + mAddButton.MeasuredWidth + mButtonSpacing;

                    for (int i = mButtonsCount - 1; i >= 0; i--)
                    {
                        View child = GetChildAt(i);

                        if (child == mAddButton || child.Visibility == ViewStates.Gone) continue;

                        int childX = expandLeft ? nextX - child.MeasuredWidth : nextX;
                        int childY = addButtonTop + (mAddButton.MeasuredHeight - child.MeasuredHeight) / 2;
                        child.Layout(childX, childY, childX + child.MeasuredWidth, childY + child.MeasuredHeight);

                        float collapsedTranslation = addButtonX - childX;
                        float expandedTranslation = 0f;

                        child.TranslationX = mExpanded ? expandedTranslation : collapsedTranslation;
                        child.Alpha = mExpanded ? 1f : 0f;

                        LayoutParams parameters = (LayoutParams)child.LayoutParameters;
                        parameters.mCollapseDir.SetFloatValues(expandedTranslation, collapsedTranslation);
                        parameters.mExpandDir.SetFloatValues(collapsedTranslation, expandedTranslation);
                        parameters.setAnimationsTarget(child);

                        nextX = expandLeft ?
                            childX - mButtonSpacing :
                            childX + child.MeasuredWidth + mButtonSpacing;
                    }

                    break;
            }
        }

        protected override ViewGroup.LayoutParams GenerateDefaultLayoutParams()
        {
            return new LayoutParams(this, base.GenerateDefaultLayoutParams());
        }
        public override ViewGroup.LayoutParams GenerateLayoutParams(IAttributeSet attrs)
        {
            return new LayoutParams(this, base.GenerateLayoutParams(attrs));
        }

        protected override ViewGroup.LayoutParams GenerateLayoutParams(ViewGroup.LayoutParams p)
        {
            return new LayoutParams(this, base.GenerateLayoutParams(p));
        }

        protected override bool CheckLayoutParams(ViewGroup.LayoutParams p)
        {
            return base.CheckLayoutParams(p);
        }



        private static IInterpolator sExpandInterpolator = new OvershootInterpolator();
        private static IInterpolator sCollapseInterpolator = new DecelerateInterpolator(3f);
        private static IInterpolator sAlphaExpandInterpolator = new DecelerateInterpolator();

        private new class LayoutParams : ViewGroup.LayoutParams
        {
            public ObjectAnimator mExpandDir = new ObjectAnimator();
            public ObjectAnimator mExpandAlpha = new ObjectAnimator();
            public ObjectAnimator mCollapseDir = new ObjectAnimator();
            public ObjectAnimator mCollapseAlpha = new ObjectAnimator();
            private bool animationsSetToPlay;
            private FloatingActionsMenu _parent;

            public LayoutParams(FloatingActionsMenu parent, ViewGroup.LayoutParams source)
                : base(source)
            {
                _parent = parent;
                mExpandDir.SetInterpolator(sExpandInterpolator);
                mExpandAlpha.SetInterpolator(sAlphaExpandInterpolator);
                mCollapseDir.SetInterpolator(sCollapseInterpolator);
                mCollapseAlpha.SetInterpolator(sCollapseInterpolator);



                mCollapseAlpha.SetProperty(Property.Of(Java.Lang.Class.FromType(typeof(View)), Java.Lang.Float.Type, "Alpha"));
                mCollapseAlpha.SetFloatValues(1f, 0f);

                mExpandAlpha.SetProperty(Property.Of(Java.Lang.Class.FromType(typeof(View)), Java.Lang.Float.Type, "Alpha"));
                mExpandAlpha.SetFloatValues(0f, 1f);

                switch (_parent.mExpandDirection)
                {
                    case EXPAND_UP:
                    case EXPAND_DOWN:
                        mCollapseDir.SetProperty(Property.Of(Java.Lang.Class.FromType(typeof(View)), Java.Lang.Float.Type, "TranslationY"));
                        mExpandDir.SetProperty(Property.Of(Java.Lang.Class.FromType(typeof(View)), Java.Lang.Float.Type, "TranslationY"));
                        break;
                    case EXPAND_LEFT:
                    case EXPAND_RIGHT:
                        mCollapseDir.SetProperty(Property.Of(Java.Lang.Class.FromType(typeof(View)), Java.Lang.Float.Type, "TranslationX"));
                        mExpandDir.SetProperty(Property.Of(Java.Lang.Class.FromType(typeof(View)), Java.Lang.Float.Type, "TranslationX"));
                        break;
                }
            }

            public void setAnimationsTarget(View view)
            {
                mCollapseAlpha.SetTarget(view);
                mCollapseDir.SetTarget(view);
                mExpandAlpha.SetTarget(view);
                mExpandDir.SetTarget(view);

                // Now that the animations have targets, set them to be played
                if (!animationsSetToPlay)
                {
                    _parent.mCollapseAnimation.Play(mCollapseAlpha);
                    _parent.mCollapseAnimation.Play(mCollapseDir);
                    _parent.mExpandAnimation.Play(mExpandAlpha);
                    _parent.mExpandAnimation.Play(mExpandDir);
                    animationsSetToPlay = true;
                }
            }
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();

            BringChildToFront(mAddButton);
            mButtonsCount = ChildCount;

            if (mLabelsStyle != 0)
            {
                createLabels();
            }
        }


        private void createLabels()
        {
            Context context = new ContextThemeWrapper(Context, mLabelsStyle);

            for (int i = 0; i < mButtonsCount; i++)
            {
                FloatingActionButton button = (FloatingActionButton)GetChildAt(i);
                String title = button.getTitle();

                if (button == mAddButton || title == null ||
                    button.GetTag(Resource.Id.fab_label) != null) continue;

                TextView label = new TextView(context);
                label.SetTextAppearance(Context, mLabelsStyle);
                label.Text = button.getTitle();
                AddView(label);

                button.SetTag(Resource.Id.fab_label, label);
            }
        }

        public void collapse()
        {
            if (mExpanded)
            {
                mExpanded = false;
                mTouchDelegateGroup.setEnabled(false);
                mCollapseAnimation.Start();
                mExpandAnimation.Cancel();

                if (mListener != null)
                {
                    mListener.onMenuCollapsed();
                }
            }
        }

        public void toggle()
        {
            if (mExpanded)
            {
                collapse();
            }
            else
            {
                expand();
            }
        }

        public void expand()
        {
            if (!mExpanded)
            {
                mExpanded = true;
                mTouchDelegateGroup.setEnabled(true);
                mCollapseAnimation.Cancel();
                mExpandAnimation.Start();

                if (mListener != null)
                {
                    mListener.onMenuExpanded();
                }
            }
        }

        public bool isExpanded()
        {
            return mExpanded;
        }

        protected override IParcelable OnSaveInstanceState()
        {
            var superState = base.OnSaveInstanceState();
            SavedState savedState = new SavedState(superState);
            savedState.mExpanded = mExpanded;

            return savedState;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            if (state is SavedState)
            {
                SavedState savedState = (SavedState)state;
                mExpanded = savedState.mExpanded;
                mTouchDelegateGroup.setEnabled(mExpanded);

                if (mRotatingDrawable != null)
                {
                    mRotatingDrawable.setRotation(mExpanded ? EXPANDED_PLUS_ROTATION : COLLAPSED_PLUS_ROTATION);
                }

                base.OnRestoreInstanceState(savedState.SuperState);
            }
            else
            {
                base.OnRestoreInstanceState(state);
            }
        }

        public class SavedState : BaseSavedState
        {
            public bool mExpanded;

            public SavedState(IParcelable parcel)
                : base(parcel)
            {
            }

            private SavedState(Parcel source)
                : base(source)
            {
                mExpanded = source.ReadInt() == 1;
            }

            public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
            {
                base.WriteToParcel(dest, flags);
                dest.WriteInt(mExpanded ? 1 : 0);
            }

            [ExportField("CREATOR")]
            static SavedStateCreator InitializeCreator()
            {
                return new SavedStateCreator();
            }

            class SavedStateCreator : Java.Lang.Object, IParcelableCreator
            {

                #region IParcelableCreator Members

                public Java.Lang.Object CreateFromParcel(Parcel source)
                {
                    return new SavedState(source);
                }

                public Java.Lang.Object[] NewArray(int size)
                {
                    return new SavedState[size];
                }

                #endregion
            }


        }
    }
}
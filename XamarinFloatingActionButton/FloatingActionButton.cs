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
using Android.Graphics.Drawables;
using Android.Util;
using Android.Content.Res;
using Java.Lang;
using Android.Graphics;
using Android.Graphics.Drawables.Shapes;

namespace XamarinFloatingActionButton
{
    [Register("xamarinfloatingactionbutton.FloatingActionButton")]
    public class FloatingActionButton : ImageButton
    {

        public const int SIZE_NORMAL = 0;
        public const int SIZE_MINI = 1;


        public int mColorNormal;
        public int mColorPressed;
        public int mColorDisabled;
        string mTitle;

        private int mIcon;
        private Drawable mIconDrawable;
        private int mSize;

        private float mCircleSize;
        private float mShadowRadius;
        private float mShadowOffset;
        private int mDrawableSize;
        public bool mStrokeVisible;

        public FloatingActionButton(Context context)
            : this(context, null)
        {

        }

        public FloatingActionButton(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Init(context, attrs);
        }

        public FloatingActionButton(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Init(context, attrs);
        }

        protected virtual void Init(Context context, IAttributeSet attributeSet)
        {
            TypedArray attr = context.ObtainStyledAttributes(attributeSet, Resource.Styleable.FloatingActionButton, 0, 0);
            mColorNormal = attr.GetColor(Resource.Styleable.FloatingActionButton_fab_colorNormal, getColor(Android.Resource.Color.HoloBlueDark));
            mColorPressed = attr.GetColor(Resource.Styleable.FloatingActionButton_fab_colorPressed, getColor(Android.Resource.Color.HoloBlueLight));
            mColorDisabled = attr.GetColor(Resource.Styleable.FloatingActionButton_fab_colorDisabled, getColor(Android.Resource.Color.DarkerGray));
            mSize = attr.GetInt(Resource.Styleable.FloatingActionButton_fab_size, SIZE_NORMAL);
            mIcon = attr.GetResourceId(Resource.Styleable.FloatingActionButton_fab_icon, 0);
            mTitle = attr.GetString(Resource.Styleable.FloatingActionButton_fab_title);
            mStrokeVisible = attr.GetBoolean(Resource.Styleable.FloatingActionButton_fab_stroke_visible, true);
            attr.Recycle();

            UpdateCircleSize();
            mShadowRadius = this.Resources.GetDimension(Resource.Dimension.fab_shadow_radius);
            mShadowOffset = this.Resources.GetDimension(Resource.Dimension.fab_shadow_offset);
            UpdateDrawableSize();

            UpdateBackground();
        }

        private void UpdateDrawableSize()
        {
            mDrawableSize = (int)(mCircleSize + 2 * mShadowRadius);
        }

        private void UpdateCircleSize()
        {
            mCircleSize = getDimension(mSize == SIZE_NORMAL ? Resource.Dimension.fab_size_normal : Resource.Dimension.fab_size_mini);
        }

        public int Size
        {
            get { return mSize; }
            set
            {
                if (value != SIZE_MINI && value != SIZE_NORMAL)
                {
                    throw new IllegalArgumentException("Use @FAB_SIZE constants only!");
                }

                if (mSize != value)
                {
                    mSize = value;
                    UpdateCircleSize();
                    UpdateDrawableSize();
                    UpdateBackground();
                }
            }
        }



        public virtual void setIcon(int iconResId)
        {
            if (mIcon != iconResId)
            {
                mIcon = iconResId;
                mIconDrawable = null;
                UpdateBackground();
            }
        }

        public void setIconDrawable(Drawable iconDrawable)
        {
            if (mIconDrawable != iconDrawable)
            {
                mIcon = 0;
                mIconDrawable = iconDrawable;
                UpdateBackground();
            }
        }


        /**
         * @return the current Color for normal state.
         */
        public int getColorNormal()
        {
            return mColorNormal;
        }

        public void setColorNormalResId(int colorNormalResId)
        {
            setColorNormal(getColor(colorNormalResId));
        }

        public void setColorNormal(int color)
        {
            if (mColorNormal != color)
            {
                mColorNormal = color;
                UpdateBackground();
            }
        }

        /**
         * @return the current color for pressed state.
         */
        public int getColorPressed()
        {
            return mColorPressed;
        }

        public void setColorPressedResId(int colorPressedResId)
        {
            setColorPressed(getColor(colorPressedResId));
        }

        public void setColorPressed(int color)
        {
            if (mColorPressed != color)
            {
                mColorPressed = color;
                UpdateBackground();
            }
        }

        /**
        * @return the current color for disabled state.
        */
        public int getColorDisabled()
        {
            return mColorDisabled;
        }

        public void setColorDisabledResId(int colorDisabledResId)
        {
            setColorDisabled(getColor(colorDisabledResId));
        }

        public void setColorDisabled(int color)
        {
            if (mColorDisabled != color)
            {
                mColorDisabled = color;
                UpdateBackground();
            }
        }

        public void setStrokeVisible(bool visible)
        {
            if (mStrokeVisible != visible)
            {
                mStrokeVisible = visible;
                UpdateBackground();
            }
        }

        public bool isStrokeVisible()
        {
            return mStrokeVisible;
        }

        protected int getColor(int colotResid)
        {
            return Resources.GetColor(colotResid);
        }

        protected float getDimension(int dimenResid)
        {
            return Resources.GetDimension(dimenResid);
        }

        public void setTitle(string title)
        {
            mTitle = title;
            TextView label = getLabelView();
            if (label != null)
            {
                label.Text = title;
            }
        }

        public TextView getLabelView()
        {
            return (TextView)GetTag(Resource.Id.fab_label);
        }

        public string getTitle()
        {
            return mTitle;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            SetMeasuredDimension(mDrawableSize, mDrawableSize);
        }



        protected virtual void UpdateBackground()
        {
            float strokeWidth = getDimension(Resource.Dimension.fab_stroke_width);
            float halfStrokeWidth = strokeWidth / 2f;

            LayerDrawable layerDrawable = new LayerDrawable(
                new Drawable[] {
                    Resources.GetDrawable(mSize == SIZE_NORMAL ? Resource.Drawable.fab_bg_normal : Resource.Drawable.fab_bg_mini),
                    createFillDrawable(strokeWidth),
                    createOuterStrokeDrawable(strokeWidth),
                    getIconDrawable()
                });

            int iconOffset = (int)(mCircleSize - getDimension(Resource.Dimension.fab_icon_size)) / 2;

            int circleInsetHorizontal = (int)(mShadowRadius);
            int circleInsetTop = (int)(mShadowRadius - mShadowOffset);
            int circleInsetBottom = (int)(mShadowRadius + mShadowOffset);

            layerDrawable.SetLayerInset(1,
                circleInsetHorizontal,
                circleInsetTop,
                circleInsetHorizontal,
                circleInsetBottom);

            layerDrawable.SetLayerInset(2,
                (int)(circleInsetHorizontal - halfStrokeWidth),
                (int)(circleInsetTop - halfStrokeWidth),
                (int)(circleInsetHorizontal - halfStrokeWidth),
                (int)(circleInsetBottom - halfStrokeWidth));

            layerDrawable.SetLayerInset(3,
                circleInsetHorizontal + iconOffset,
                circleInsetTop + iconOffset,
                circleInsetHorizontal + iconOffset,
                circleInsetBottom + iconOffset);

            setBackgroundCompat(layerDrawable);
        }

        protected virtual Drawable getIconDrawable()
        {
            if (mIconDrawable != null)
            {
                return mIconDrawable;
            }
            else if (mIcon != 0)
            {
                return Resources.GetDrawable(mIcon);
            }
            else
            {
                return new ColorDrawable(Color.Transparent);
            }
        }

        private StateListDrawable createFillDrawable(float strokeWidth)
        {
            StateListDrawable drawable = new StateListDrawable();
            drawable.AddState(new int[] { -Android.Resource.Attribute.StateEnabled }, createCircleDrawable(mColorDisabled, strokeWidth));
            drawable.AddState(new int[] { Android.Resource.Attribute.StatePressed }, createCircleDrawable(mColorPressed, strokeWidth));
            drawable.AddState(new int[] { }, createCircleDrawable(mColorNormal, strokeWidth));
            return drawable;
        }

        private Drawable createCircleDrawable(int color, float strokeWidth)
        {
            int alpha = Color.GetAlphaComponent(color);
            int opaqueColor = opaque(color);

            ShapeDrawable fillDrawable = new ShapeDrawable(new OvalShape());

            Paint paint = fillDrawable.Paint;
            paint.AntiAlias = true;
            paint.Color = new Color(opaqueColor);

            Drawable[] layers = {
        fillDrawable,
        createInnerStrokesDrawable(opaqueColor, strokeWidth)
    };

            LayerDrawable drawable = alpha == 255 || !mStrokeVisible
                ? new LayerDrawable(layers)
                : new TranslucentLayerDrawable(alpha, layers);

            int halfStrokeWidth = (int)(strokeWidth / 2f);
            drawable.SetLayerInset(1, halfStrokeWidth, halfStrokeWidth, halfStrokeWidth, halfStrokeWidth);

            return drawable;
        }

        private class TranslucentLayerDrawable : LayerDrawable
        {
            private readonly int mAlpha;

            public TranslucentLayerDrawable(int alpha, params Drawable[] layers)
                : base(layers)
            {
                mAlpha = alpha;
            }

            public override void Draw(Canvas canvas)
            {
                Rect bounds = Bounds;
                canvas.SaveLayerAlpha(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, mAlpha, SaveFlags.All);
                base.Draw(canvas);
                canvas.Restore();
            }


        }

        private Drawable createOuterStrokeDrawable(float strokeWidth)
        {
            ShapeDrawable shapeDrawable = new ShapeDrawable(new OvalShape());

            Paint paint = shapeDrawable.Paint;
            paint.AntiAlias = true;
            paint.StrokeWidth = strokeWidth;
            paint.SetStyle(Paint.Style.Stroke);
            paint.Color = Color.Black;
            paint.Alpha = opacityToAlpha(0.02f);

            return shapeDrawable;
        }

        private int opacityToAlpha(float opacity)
        {
            return (int)(255f * opacity);
        }

        private int darkenColor(int argb)
        {
            return adjustColorBrightness(argb, 0.9f);
        }

        private int lightenColor(int argb)
        {
            return adjustColorBrightness(argb, 1.1f);
        }

        private int adjustColorBrightness(int argb, float factor)
        {
            float[] hsv = new float[3];
            Color.ColorToHSV(new Color(argb), hsv);

            hsv[2] = System.Math.Min(hsv[2] * factor, 1f);

            return Color.HSVToColor(Color.GetAlphaComponent(argb), hsv);
        }

        private int halfTransparent(int argb)
        {
            return Color.Argb(
                Color.GetAlphaComponent(argb) / 2,
                Color.GetRedComponent(argb),
                Color.GetGreenComponent(argb),
                Color.GetBlueComponent(argb)
            );
        }

        private int opaque(int argb)
        {
            return Color.Rgb(
                Color.GetRedComponent(argb),
                Color.GetGreenComponent(argb),
                Color.GetBlueComponent(argb)
            );
        }

        private Drawable createInnerStrokesDrawable(int color, float strokeWidth)
        {
            if (!mStrokeVisible)
            {
                return new ColorDrawable(Color.Transparent);
            }

            ShapeDrawable shapeDrawable = new ShapeDrawable(new OvalShape());

            int bottomStrokeColor = darkenColor(color);
            int bottomStrokeColorHalfTransparent = halfTransparent(bottomStrokeColor);
            int topStrokeColor = lightenColor(color);
            int topStrokeColorHalfTransparent = halfTransparent(topStrokeColor);

            Paint paint = shapeDrawable.Paint;
            paint.AntiAlias = true;
            paint.StrokeWidth = strokeWidth;
            paint.SetStyle(Paint.Style.Stroke);
            shapeDrawable.SetShaderFactory(new CustomShaderFactory(
              (width, height) =>
              {
                  return new LinearGradient(width / 2, 0, width / 2, height,
                      new int[] { topStrokeColor, topStrokeColorHalfTransparent, color, bottomStrokeColorHalfTransparent, bottomStrokeColor },
                      new float[] { 0f, 0.2f, 0.5f, 0.8f, 1f },
                      Android.Graphics.Shader.TileMode.Clamp
                  );
              }));

            return shapeDrawable;
        }

        private void setBackgroundCompat(Drawable drawable)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
            {
                this.Background = (drawable);
            }
            else
            {
                SetBackgroundDrawable(drawable);
            }
        }


        public override ViewStates Visibility
        {
            set
            {
                TextView label = getLabelView();
                if (label != null)
                {
                    label.Visibility = value;
                }

                base.Visibility = value;
            }
        }

        private class CustomShaderFactory : ShapeDrawable.ShaderFactory
        {
            public delegate Shader ShaderFactoryResizeDelegate(int width, int height);
            ShaderFactoryResizeDelegate _resizeDelegate;

            public CustomShaderFactory(ShaderFactoryResizeDelegate resizeDelegate)
            {
                _resizeDelegate = resizeDelegate;
            }

            public override Shader Resize(int width, int height)
            {
                return _resizeDelegate(width, height);
            }
        }
    }
}
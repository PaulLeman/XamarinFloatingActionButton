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
using Android.Util;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Graphics;

namespace XamarinFloatingActionButton
{
    [Register("com.getbase.floatingactionbutton.AddFloatingActionButton")]
    public class AddFloatingActionButton : FloatingActionButton
    {
        public int mPlusColor;

        public AddFloatingActionButton(Context context)
            : this(context, null)
        { }


        public AddFloatingActionButton(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public AddFloatingActionButton(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {

        }

        protected override void Init(Context context, IAttributeSet attributeSet)
        {
            TypedArray attr = context.ObtainStyledAttributes(attributeSet, Resource.Styleable.AddFloatingActionButton, 0, 0);
            mPlusColor = attr.GetColor(Resource.Styleable.AddFloatingActionButton_fab_plusIconColor, getColor(Android.Resource.Color.White));
            attr.Recycle();

            base.Init(context, attributeSet);
        }



        /**
         * @return the current Color of plus icon.
         */
        public int getPlusColor()
        {
            return mPlusColor;
        }

        public void setPlusColorResId(int plusColorResId)
        {
            setPlusColor(getColor(plusColorResId));
        }

        public void setPlusColor(int color)
        {
            if (mPlusColor != color)
            {
                mPlusColor = color;
                UpdateBackground();
            }
        }

        public override void setIcon(int iconResId)
        {
            throw new Java.Lang.UnsupportedOperationException("Use FloatingActionButton if you want to use custom icon");
        }

        protected override Drawable getIconDrawable()
        {
            float iconSize = getDimension(Resource.Dimension.fab_icon_size);
            float iconHalfSize = iconSize / 2f;

            float plusSize = getDimension(Resource.Dimension.fab_plus_icon_size);
            float plusHalfStroke = getDimension(Resource.Dimension.fab_plus_icon_stroke) / 2f;
            float plusOffset = (iconSize - plusSize) / 2f;

            Shape shape = new CustomShape(
              (canvas, tmpPaint) =>
              {
                  canvas.DrawRect(plusOffset, iconHalfSize - plusHalfStroke, iconSize - plusOffset, iconHalfSize + plusHalfStroke, tmpPaint);
                  canvas.DrawRect(iconHalfSize - plusHalfStroke, plusOffset, iconHalfSize + plusHalfStroke, iconSize - plusOffset, tmpPaint);
              });

            ShapeDrawable drawable = new ShapeDrawable(shape);

            Paint paint = drawable.Paint;
            paint.Color = new Color(mPlusColor);
            paint.SetStyle(Paint.Style.Fill);
            paint.AntiAlias = true;

            return drawable;
        }

        private class CustomShape : Shape
        {
            public delegate void ShapeDrawDelegate(Canvas canvas, Paint paint);
            ShapeDrawDelegate _drawDelegate;

            public CustomShape(ShapeDrawDelegate drawDelegate)
            {
                _drawDelegate = drawDelegate;
            }

            public override void Draw(Canvas canvas, Paint paint)
            {
                _drawDelegate(canvas, paint);
            }
        }
    }
}
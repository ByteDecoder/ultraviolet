﻿using System;
using TwistedLogik.Nucleus;

namespace TwistedLogik.Ultraviolet.UI.Presentation.Media
{
    /// <summary>
    /// Represents a transformation which skews an object in two dimensions.
    /// </summary>
    [UvmlKnownType]
    public sealed class SkewTransform : Transform
    {
        /// <inheritdoc/>
        public override Matrix Value
        {
            get { return value; }
        }

        /// <inheritdoc/>
        public override Matrix? Inverse
        {
            get { return inverse; }
        }

        /// <inheritdoc/>
        public override Boolean IsIdentity
        {
            get { return isIdentity; }
        }

        /// <summary>
        /// Gets or sets the angle of skew in degrees along the x-axis.
        /// </summary>
        public Single AngleX
        {
            get { return GetValue<Single>(AngleXProperty); }
            set { SetValue<Single>(AngleXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the angle of skew in degrees along the y-axis.
        /// </summary>
        public Single AngleY
        {
            get { return GetValue<Single>(AngleYProperty); }
            set { SetValue<Single>(AngleYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the x-coordinate around which the object is rotated.
        /// </summary>
        public Double CenterX
        {
            get { return GetValue<Double>(CenterXProperty); }
            set { SetValue<Double>(CenterXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the y-coordinate around which the object is rotated.
        /// </summary>
        public Double CenterY
        {
            get { return GetValue<Double>(CenterYProperty); }
            set { SetValue<Double>(CenterYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AngleX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleXProperty = DependencyProperty.Register("AngleX", typeof(Single), typeof(SkewTransform),
            new PropertyMetadata<Single>(CommonBoxedValues.Single.Zero, PropertyMetadataOptions.None, HandleAngleChanged));

        /// <summary>
        /// Identifies the <see cref="AngleY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleYProperty = DependencyProperty.Register("AngleY", typeof(Single), typeof(SkewTransform),
            new PropertyMetadata<Single>(CommonBoxedValues.Single.Zero, PropertyMetadataOptions.None, HandleAngleChanged));

        /// <summary>
        /// Identifies the <see cref="CenterX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register("CenterX", typeof(Double), typeof(SkewTransform),
            new PropertyMetadata<Double>(CommonBoxedValues.Double.Zero, PropertyMetadataOptions.None, HandleCenterChanged));

        /// <summary>
        /// Identifies the <see cref="CenterY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register("CenterY", typeof(Double), typeof(SkewTransform),
            new PropertyMetadata<Double>(CommonBoxedValues.Double.Zero, PropertyMetadataOptions.None, HandleCenterChanged));

        /// <summary>
        /// Called when the value of the <see cref="AngleX"/> or <see cref="AngleY"/> dependency properties changes.
        /// </summary>
        private static void HandleAngleChanged(DependencyObject dobj, Single oldValue, Single newValue)
        {
            var transform = (SkewTransform)dobj;
            transform.UpdateValue();
            transform.InvalidateDependencyObject();
        }

        /// <summary>
        /// Called when the value of the <see cref="CenterX"/> or <see cref="CenterY"/> dependency properties change.
        /// </summary>
        private static void HandleCenterChanged(DependencyObject dobj, Double oldValue, Double newValue)
        {
            var transform = (SkewTransform)dobj;
            transform.UpdateValue();
            transform.InvalidateDependencyObject();
        }

        /// <summary>
        /// Updates the transform's cached value.
        /// </summary>
        private void UpdateValue()
        {
            var centerX = (Single)CenterX;
            var centerY = (Single)CenterY;

            var tanX = (Single)Math.Tan(Radians.FromDegrees(AngleX));
            var tanY = (Single)Math.Tan(Radians.FromDegrees(AngleY));

            var mtxSkew = new Matrix(
                   1, tanX, 0, 0,
                tanY,    1, 0, 0,
                   0,    0, 1, 0,
                   0,    0, 0, 1);

            var hasCenter = (centerX != 0 || centerY != 0);
            if (hasCenter)
            {
                var mtxTransformCenter = Matrix.CreateTranslation(-centerX, -centerY, 0f);
                var mtxTransformCenterInverse = Matrix.CreateTranslation(centerX, centerY, 0f);

                Matrix mtxResult;
                Matrix.Concat(ref mtxTransformCenter, ref mtxSkew, out mtxResult);
                Matrix.Concat(ref mtxResult, ref mtxTransformCenterInverse, out mtxResult);

                this.value = mtxResult;
            }
            else
            {
                this.value = mtxSkew;
            }

            Matrix invertedValue;
            this.inverse = Matrix.TryInvert(value, out invertedValue) ? invertedValue : (Matrix?)null; 
            this.isIdentity = Matrix.Identity.Equals(value);
        }

        // Property values.
        private Matrix value = Matrix.Identity;
        private Matrix? inverse;
        private Boolean isIdentity;
    }
}
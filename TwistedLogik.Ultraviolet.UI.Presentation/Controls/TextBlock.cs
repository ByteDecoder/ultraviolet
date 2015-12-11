﻿using System;
using System.ComponentModel;
using System.Text;
using TwistedLogik.Nucleus;
using TwistedLogik.Ultraviolet.Graphics.Graphics2D;
using TwistedLogik.Ultraviolet.Graphics.Graphics2D.Text;
using TwistedLogik.Ultraviolet.UI.Presentation.Controls.Primitives;

namespace TwistedLogik.Ultraviolet.UI.Presentation.Controls
{
    /// <summary>
    /// Represents a lightweight control for displaying text.
    /// </summary>
    [UvmlKnownType]
    [DefaultProperty("Text")]
    public class TextBlock : TextBlockBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlock"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="name">The element's identifying name within its namescope.</param>
        public TextBlock(UltravioletContext uv, String name)
            : base(uv, name)
        {

        }

        /// <summary>
        /// Gets the text area's text.
        /// </summary>
        /// <returns>A <see cref="String"/> instance containing the text area's text.</returns>
        public String GetText()
        {
            return GetValue<VersionedStringSource>(TextProperty).ToString();
        }

        /// <summary>
        /// Gets the text area's text.
        /// </summary>
        /// <param name="stringBuilder">A <see cref="StringBuilder"/> instance to populate with the text area's text.</param>
        public void GetText(StringBuilder stringBuilder)
        {
            Contract.Require(stringBuilder, "stringBuilder");

            var value = GetValue<VersionedStringSource>(TextProperty);

            stringBuilder.Length = 0;
            stringBuilder.AppendVersionedStringSource(value);
        }

        /// <summary>
        /// Sets the text area's text.
        /// </summary>
        /// <param name="value">A <see cref="String"/> instance to set as the text area's text.</param>
        public void SetText(String value)
        {
            SetValue(TextProperty, new VersionedStringSource(value));
        }

        /// <summary>
        /// Sets the text area's text.
        /// </summary>
        /// <param name="value">A <see cref="StringBuilder"/> instance whose contents will be set as the text area's text.</param>
        public void SetText(StringBuilder value)
        {
            SetValue(TextProperty, (value == null) ? VersionedStringSource.Invalid : new VersionedStringSource(value.ToString()));
        }

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        /// <remarks>The styling name of this dependency property is 'text'.</remarks>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(VersionedStringSource), typeof(TextBlock),
            new PropertyMetadata<VersionedStringSource>(VersionedStringSource.Invalid, PropertyMetadataOptions.AffectsMeasure, HandleTextChanged));

        /// <inheritdoc/>
        protected override void OnViewChanged(PresentationFoundationView oldView, PresentationFoundationView newView)
        {
            UpdateTextParserResult();

            base.OnViewChanged(oldView, newView);
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            UpdateTextParserResult();
            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void DrawOverride(UltravioletTime time, DrawingContext dc)
        {
            if (textLayoutCommands.Count > 0)
            {
                var position = Display.DipsToPixels(UntransformedAbsolutePosition);
                var positionRounded = dc.IsTransformed ? (Vector2)position : (Vector2)(Point2)position;
                View.Resources.TextRenderer.Draw((SpriteBatch)dc, textLayoutCommands, positionRounded, Foreground * dc.Opacity);
            }
            base.DrawOverride(time, dc);
        }

        /// <inheritdoc/>
        protected override Size2D MeasureOverride(Size2D availableSize)
        {
            if (Name == "bar")
                Console.WriteLine();

            UpdateTextLayoutResult(availableSize);

            var sizePixels = new Size2D(textLayoutCommands.ActualWidth, textLayoutCommands.ActualHeight);
            var sizeDips   = Display.PixelsToDips(sizePixels);

            return sizeDips;
        }

        /// <inheritdoc/>
        protected override Size2D ArrangeOverride(Size2D finalSize, ArrangeOptions options)
        {
            UpdateTextLayoutResult(finalSize);

            return base.ArrangeOverride(finalSize, options);
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Text"/> dependency property changes.
        /// </summary>
        private static void HandleTextChanged(DependencyObject dobj, VersionedStringSource oldValue, VersionedStringSource newValue)
        {
            var label = (TextBlock)dobj;
            label.UpdateTextParserResult();
        }

        /// <summary>
        /// Updates the cache that contains the result of parsing the label's text.
        /// </summary>
        private void UpdateTextParserResult()
        {
            textParserResult.Clear();

            if (View == null)
                return;

            var text = GetValue<VersionedStringSource>(TextProperty);
            if (text.IsValid)
            {
                var textString = text.ToString();
                View.Resources.TextRenderer.Parse(textString, textParserResult);
            }
        }

        /// <summary>
        /// Updates the cache that contains the result of laying out the label's text.
        /// </summary>
        /// <param name="availableSize">The size of the space that is available for laying out text.</param>
        private void UpdateTextLayoutResult(Size2D availableSize)
        {
            textLayoutCommands.Clear();

            if (textParserResult.Count > 0 && Font.IsLoaded)
            {
                var unconstrainedWidth  = Double.IsPositiveInfinity(availableSize.Width)  && HorizontalAlignment != HorizontalAlignment.Stretch;
                var unconstrainedHeight = Double.IsPositiveInfinity(availableSize.Height) && VerticalAlignment != VerticalAlignment.Stretch;

                var constraintX = unconstrainedWidth  ? null : (Int32?)Math.Ceiling(Display.DipsToPixels(availableSize.Width));
                var constraintY = unconstrainedHeight ? null : (Int32?)Math.Ceiling(Display.DipsToPixels(availableSize.Height));

                var flags    = LayoutUtil.ConvertAlignmentsToTextFlags(HorizontalContentAlignment, VerticalContentAlignment);
                var settings = new TextLayoutSettings(Font, constraintX, constraintY, flags, FontStyle);

                View.Resources.TextRenderer.CalculateLayout(textParserResult, textLayoutCommands, settings);
            }
        }

        // State values.
        private readonly TextParserTokenStream textParserResult = new TextParserTokenStream();
        private readonly TextLayoutCommandStream textLayoutCommands = new TextLayoutCommandStream();
    }
}

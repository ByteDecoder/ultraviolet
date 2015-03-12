﻿using System;
using TwistedLogik.Ultraviolet.Graphics.Graphics2D.Text;

namespace TwistedLogik.Ultraviolet.UI.Presentation.Elements
{
    /// <summary>
    /// Represents an element which is used to indicate the position of child content within a component template.
    /// </summary>
    [UvmlKnownType]
    public class ContentPresenter : FrameworkElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPresenter"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="id">The element's unique identifier within its view.</param>
        public ContentPresenter(UltravioletContext uv, String id)
            : base(uv, id)
        {

        }

        /// <inheritdoc/>
        protected override void DrawOverride(UltravioletTime time, DrawingContext dc)
        {
            var owner = Control as ContentControl;
            if (owner != null)
            {
                var text = owner.Content as String;
                if (text != null)
                {
                    var positionX = (Single)Display.DipsToPixels(AbsolutePosition.X);
                    var positionY = (Single)Display.DipsToPixels(AbsolutePosition.Y);
                    var position = new Vector2(positionX, positionY);
                    View.Resources.TextRenderer.Draw(dc.SpriteBatch, textLayoutResult, position, FontColor);
            }
            }
            base.DrawOverride(time, dc);
        }

        protected override Size2D MeasureOverride(Size2D availableSize)
        {
            var owner = Control as ContentControl;
            if (owner == null)
                return Size2D.Zero;

            var text = owner.Content as String;
            if (text != null)
            {
                UpdateTextParserCache();
                UpdateTextLayoutCache(availableSize);

                var textWidth = Display.PixelsToDips(textLayoutResult.ActualWidth);
                var textHeight = Display.PixelsToDips(textLayoutResult.ActualHeight);
                return new Size2D(textWidth, textHeight);
            }
            else
            {
                textParserResult.Clear();
                textLayoutResult.Clear();
            }

            var content = owner.Content as UIElement;
            if (content == null)
                return Size2D.Zero;

            content.Measure(availableSize);
            return content.DesiredSize;
        }

        protected override Size2D ArrangeOverride(Size2D finalSize, ArrangeOptions options)
        {
            var owner = Control as ContentControl;
            if (owner == null)
                return Size2D.Zero;

            var text = owner.Content as String;
            if (text != null)
        {
                UpdateTextLayoutCache(finalSize);

                var textWidth = Display.PixelsToDips(textLayoutResult.ActualWidth);
                var textHeight = Display.PixelsToDips(textLayoutResult.ActualHeight);
                return new Size2D(textWidth, textHeight);
            }

            var content = owner.Content as UIElement;
            if (content == null)
                return Size2D.Zero;

            content.Arrange(new RectangleD(0, 0, finalSize.Width, finalSize.Height), options);
            return content.RenderSize;
        }

        protected internal override UIElement GetLogicalChild(int childIndex)
        {
            var owner = Control as ContentControl;
            if (owner == null || owner.TreatContentAsLogicalChild)
                throw new ArgumentOutOfRangeException("childIndex");

            var content = owner.Content as UIElement;
            if (content == null || childIndex != 0)
                throw new ArgumentOutOfRangeException("childIndex");

            return content;
        }

        protected internal override UIElement GetVisualChild(int childIndex)
        {
            var owner = Control as ContentControl;
            if (owner == null)
                throw new ArgumentOutOfRangeException("childIndex");

            var content = owner.Content as UIElement;
            if (content == null || childIndex != 0)
                throw new ArgumentOutOfRangeException("childIndex");

            return content;
        }

        protected internal override Int32 LogicalChildrenCount
        {
            get 
            {
                var owner = Control as ContentControl;
                if (owner == null)
                    return 0;

                return owner.TreatContentAsLogicalChild ? 0 : ((owner.Content is UIElement) ? 1 : 0);
            }
        }

        protected internal override Int32 VisualChildrenCount
        {
            get 
            {
                var owner = Control as ContentControl;
                if (owner == null)
                    return 0;

                return (owner.Content is UIElement) ? 1 : 0;
            }
        }

        /// <summary>
        /// Updates the cache which contains the element's parsed text.
        /// </summary>
        private void UpdateTextParserCache()
        {
            textParserResult.Clear();

            if (View == null)
                return;

            var owner = Control as ContentControl;
            var content = owner.Content;
            var contentElement = content as UIElement;
            if (content != null && contentElement == null)
            {
                var contentAsString = content.ToString();
                View.Resources.TextRenderer.Parse(contentAsString, textParserResult);
            }

            InvalidateArrange();
        }

        /// <summary>
        /// Updates the cache which contains the element's laid-out text.
        /// </summary>
        /// <param name="availableSize">The amount of space in which the element's text can be laid out.</param>
        private void UpdateTextLayoutCache(Size2D availableSize)
        {
            textLayoutResult.Clear();

            if (View == null)
                return;

            var owner = Control as ContentControl;

            if (textParserResult.Count > 0 && Font.IsLoaded)
            {
                var availableWidth  = (Int32)Display.DipsToPixels(availableSize.Width);
                var availableHeight = (Int32)Display.DipsToPixels(availableSize.Height);

                var flags    = LayoutUtil.ConvertAlignmentsToTextFlags(owner.HorizontalContentAlignment, owner.VerticalContentAlignment);
                var settings = new TextLayoutSettings(Font, availableWidth, availableHeight, flags, FontStyle);
                View.Resources.TextRenderer.CalculateLayout(textParserResult, textLayoutResult, settings);
            }
        }

        // Cached parser/layout results for content text.
        private readonly TextParserResult textParserResult = new TextParserResult();
        private readonly TextLayoutResult textLayoutResult = new TextLayoutResult();
    }
}

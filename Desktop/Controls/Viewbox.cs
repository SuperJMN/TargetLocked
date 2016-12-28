namespace Desktop.Controls
{
    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;
    using static DoubleUtil;

    public class Viewbox : Decorator
    {
        private IControl Root => Child;

        protected override Size MeasureOverride(Size constraint)
        {
            if (Root == null)
            {
                return constraint;
            }

            var availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            Root.Measure(availableSize);
            var desiredSize = Root.DesiredSize;
            var factor = GetFactorToFillWithoutClipping(constraint, desiredSize);

            return new Size(factor.Width * desiredSize.Width, factor.Height * desiredSize.Height);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (Root == null)
            {
                return arrangeSize;
            }

            var desiredSize = Root.DesiredSize;
            var factor = GetFactorToFillWithoutClipping(arrangeSize, desiredSize);

            Root.RenderTransformOrigin = RelativePoint.TopLeft;
            Root.RenderTransform = new ScaleTransform(factor.Width, factor.Height);
            Root.Arrange(new Rect(new Point(0, 0), desiredSize));

            var width = factor.Width * desiredSize.Width;
            return new Size(width, factor.Height * desiredSize.Height);
        }

        private static Size GetFactorToFillWithoutClipping(Size availableSize, Size contentSize)
        {
            var width = IsZero(contentSize.Width) ? 0.0 : availableSize.Width / contentSize.Width;
            var height = IsZero(contentSize.Height) ? 0.0 : availableSize.Height / contentSize.Height;

            var min = Math.Min(width, height);
            return new Size(min, min);            
        }

        private static Size GetFactorFillWithClipping(Size availableSize, Size contentSize)
        {
            var width = IsZero(contentSize.Width) ? 0.0 : availableSize.Width / contentSize.Width;
            var height = IsZero(contentSize.Height) ? 0.0 : availableSize.Height / contentSize.Height;

            var min = Math.Max(width, height);
            return new Size(min, min);
        }

        private static Size GetFactorToFillWithoutPreservingAspectRatio(Size availableSize, Size contentSize)
        {
            var width = IsZero(contentSize.Width) ? 0.0 : availableSize.Width / contentSize.Width;
            var height = IsZero(contentSize.Height) ? 0.0 : availableSize.Height / contentSize.Height;

            return new Size(width, height);
        }
    }
}
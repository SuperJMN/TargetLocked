namespace Desktop.Controls
{
    using System;

    internal static class DoubleUtil
    {
        private const double Tolerance = 0.1D;

        public static bool IsZero(double contentSizeWidth)
        {
            return Math.Abs(contentSizeWidth) < Tolerance;
        }
    }
}
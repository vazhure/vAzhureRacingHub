using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace vAzhureRacingAPI
{
    public static class GraphicsExt
    {
        public static GraphicsPath RoundedRect(this RectangleF rect, float radius)
        {
            rect.Offset(-radius, -radius);
            rect.Inflate(-radius, -radius);
            radius *= 2;

            GraphicsPath rounded = new GraphicsPath();
            float x = rect.Left;
            float y = rect.Top;
            float x1 = rect.Right;
            float y1 = rect.Bottom;

            rounded.StartFigure();
            // Дуги на углах прямоугольника
            rounded.AddArc(x1, y1, radius, radius, 0, 90);
            rounded.AddArc(x, y1, radius, radius, 90, 90);
            rounded.AddArc(x, y, radius, radius, 180, 90);
            rounded.AddArc(x1, y, radius, radius, 270, 90);

            // Замыкаем контура
            rounded.CloseFigure();
            return rounded;
        }

        [Flags]
        public enum RoundEdge {None = 0, TopLeft = 1, TopRight = 1 << 1, BottomLeft = 1 << 2, BottomRight = 1 << 3, All = TopLeft | TopRight | BottomLeft | BottomRight};

        /// <summary>
        /// Скругление определенных углов прямоугольника
        /// </summary>
        /// <param name="r"></param>
        /// <param name="radius">Радиус скругления</param>
        /// <param name="edges">Флаги скругления углов</param>
        /// <returns>GraphicsPath</returns>
        public static GraphicsPath RoundRectEdges(this RectangleF r, float radius, RoundEdge edges = RoundEdge.All)
        {
            radius *= 2;

            var rounded = new GraphicsPath(FillMode.Winding);

            if (!edges.HasFlag(RoundEdge.TopLeft))
                rounded.AddLine(r.Left, r.Y, r.Left, r.Y);
            else
                rounded.AddArc(r.Left, r.Top, radius, radius, 180f, 90f);

            if (!edges.HasFlag(RoundEdge.TopRight))
                rounded.AddLine(r.Right, r.Top, r.Right, r.Top);
            else
                rounded.AddArc(r.Right - radius, r.Top, radius, radius, 270f, 90f);

            if (!edges.HasFlag(RoundEdge.BottomRight))
                rounded.AddLine(r.Right, r.Bottom, r.Right, r.Bottom);
            else
                rounded.AddArc(r.Right - radius, r.Bottom - radius, radius, radius, 0f, 90f);

            if (!edges.HasFlag(RoundEdge.BottomLeft))
                rounded.AddLine(r.Left, r.Bottom, r.Left, r.Bottom);
            else
                rounded.AddArc(r.Left, r.Bottom - radius, radius, radius, 90f, 90f);
            rounded.CloseFigure();
            return rounded;
        }

        public static void FillRoundedRect(this Graphics g, Brush brush, RectangleF rect, float radius)
        {
            using (GraphicsPath rounded = rect.RoundedRect(radius))
                g.FillPath(brush, rounded);
        }

        public static void StrokeRoundedRect(this Graphics g, Pen pen, RectangleF rect, float radius)
        {
            using (GraphicsPath rounded = rect.RoundedRect(radius))
                g.DrawPath(pen, rounded);
        }

        public static Point Center(this Rectangle rc)
        {
            return new Point((rc.Left + rc.Right) / 2, (rc.Top + rc.Bottom) / 2);
        }
        public static PointF Center(this RectangleF rc)
        {
            return new PointF((rc.Left + rc.Right) / 2, (rc.Top + rc.Bottom) / 2);
        }
    }
}

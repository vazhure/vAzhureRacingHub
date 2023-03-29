using System.Windows.Forms;

namespace Nextion35Dash
{
    internal class FlowLayoutPanel2 : FlowLayoutPanel
    {
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            DoubleBuffered = true;
        }

        protected override System.Drawing.Point ScrollToControl(Control activeControl)
        {
            return base.ScrollToControl(activeControl);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            //    base.OnMouseWheel(e); // Запрещаем прокрутку по колесу мыши
        }

        /// <summary>
        /// Делаем функцию ScrollToControl публичной
        /// </summary>
        /// <param name="c">Control</param>
        public void ScrollTo(Control c)
        {
            base.ScrollToControl(c);
        }
    }
}

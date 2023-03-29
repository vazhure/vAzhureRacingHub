using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vAzhureRacingAPI
{
    public class MovableForm : Form
    {
        #region System Constants
        private const int WM_NCHITTEST = 0x84;
        private const int HT_CLIENT = 0x1;
        private const int HT_CAPTION = 0x2;
        #endregion

        public MovableForm() : base()
        {
            FormBorderStyle = FormBorderStyle.Sizable;
        }

        /// <summary>
        /// Обработка пула сообщений Windows
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // Если проверяем попадание курсора мыши в клиентской области, то активируем режим перетаскивания за заголовок
            if (m.Msg == WM_NCHITTEST && m.Result == (IntPtr)HT_CLIENT)
                m.Result = (IntPtr)(HT_CAPTION);
        }

        /// <summary>
        /// Меняем стиль окна - убираем заголовок
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~0x00C00000; // WS_CAPTION
                return cp;
            }
        }
    }
}

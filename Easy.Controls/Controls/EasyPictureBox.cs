using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Easy.Controls
{
    /// <summary>
    /// 图片框控件
    /// </summary>
    public class EasyPictureBox : PictureBox
    {
        private string text;
        [Description("文本"), Category("Appearance")]
        public string Texts
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }
    }
}

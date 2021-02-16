using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDF_Tool_2021a01
{
    public partial class TransparentTextBox : TextBox
    {
        public TransparentTextBox()
        {
            InitializeComponent();

            SetStyle(ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
        }

        public TransparentTextBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

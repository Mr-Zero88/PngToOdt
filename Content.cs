using AODL.Document.Content.Text;
using AODL.Document.TextDocuments;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDF_Tool_2021a01
{
    public interface Content
    {
        Rectangle Box { get; set; }
        List<Control> GetControls();
        void GetParagraph(TextDocument document, string fileName);
    }
}

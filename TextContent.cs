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
    public class TextContent : Content
    {
        public string Text { get; set; }
        public Rectangle Box { get; set; }

        public TextContent(string text, Rectangle box)
        {
            Text = text;
            Box = box;
        }

        public List<Control> GetControls()
        {
            List<Control> controls = new List<Control>();

            RichTextBox textBox = new RichTextBox();
            textBox.Location = Box.Location;
            textBox.Size = Box.Size;
            textBox.Text = Text.Replace("\n", "\r\n");
            textBox.Multiline = true;
            controls.Add(textBox);

            return controls;
        }

        public void GetParagraph(TextDocument document, string filename)
        {
            //paragraph.TextContent.Add(new FormatedText(document, "Some simple text!")); //AODL.Document.Content.Text.
            foreach (string line in Text.Split('\n'))
            {
                Paragraph paragraph = ParagraphBuilder.CreateStandardTextParagraph(document);
                paragraph.TextContent.Add(new SimpleText(document, line));
                document.Content.Add(paragraph);
            }
        }
    }
}

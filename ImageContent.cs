using AODL.Document.Content.Draw;
using AODL.Document.Content.Text;
using AODL.Document.TextDocuments;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDF_Tool_2021a01
{
    public class ImageContent : Content
    {
        public Image Image { get; set; }
        public Rectangle Box { get; set; }

        public ImageContent(Image image, Rectangle box)
        {
            Image = (Image)(new Bitmap(image, box.Size));
            Box = box;
        }

        public List<Control> GetControls()
        {
            List<Control> controls = new List<Control>();

            PictureBox pictureBox = new PictureBox();
            pictureBox.Location = Box.Location;
            pictureBox.Size = Box.Size;
            pictureBox.Image = (Image)Image.Clone();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            controls.Add(pictureBox);
            return controls;
        }

        public void GetParagraph(TextDocument document, string filename)
        {
            Paragraph paragraph = ParagraphBuilder.CreateStandardTextParagraph(document);
            int number = new Random().Next(0, 10000);
            string file = $"{filename}_Temp\\image{number}.png";
            ((Image)Image.Clone()).Save(file, ImageFormat.Png);
            Frame frame = new Frame(document, $"frame{number}", $"graphic{number}", file);
            paragraph.Content.Add(frame);
            document.Content.Add(paragraph);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using iTextSharpage.text;
//using iTextSharpage.text.pdf;
//using iTextSharpage.text.pdf.parser;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using IronOcr;
using IronOcr.Exceptions;
using AForge.Imaging;
using System.Threading;
using HtmlPageBuilder;
using TheArtOfDev.HtmlRenderer.WinForms;
using AODL.Document;
using AODL.Document.Content;
using AODL.Document.TextDocuments;
using AODL.Document.Content.Text;
using AODL.Document.Content.Draw;

namespace PDF_Tool_2021a01
{
    public partial class Form1 : Form
    {
        public List<Content> imageContents;
        public double Confidence;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IronTesseract Ocr = new IronTesseract();
            Ocr.Language = OcrLanguage.GermanBest;
            Ocr.Configuration.ReadBarCodes = true;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG File|*.png";
            if (openFileDialog.ShowDialog() != DialogResult.OK) Close();
            Controls.Clear();
            System.Drawing.Image image = System.Drawing.Image.FromFile(openFileDialog.FileName);

            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = image;
            pictureBox.Size = image.Size;
            Controls.Add(pictureBox);

            ClientSize = image.Size + new Size(0, 24);

            Task task = new Task(() =>
            {
                using (var Input = new OcrInput(image))
                {
                    Input.Deskew();
                    OcrResult Result = Ocr.Read(Input);
                    imageContents = new List<Content>();
                    foreach (var Page in Result.Pages)
                    {
                        float imagesizeoffset = (float)image.Size.Width / (float)Page.ToBitmap(Input).Size.Width;
                        foreach (OcrResult.Block block in Page.Blocks)
                        {
                            Rectangle box = new Rectangle(new Point((int)(block.X * imagesizeoffset) - 2, (int)(block.Y * imagesizeoffset) - 2), new Size((int)(block.Width * imagesizeoffset), (int)(block.Height * imagesizeoffset)) + new Size(4, 4));
                            if (!string.IsNullOrWhiteSpace(block.Text.Replace("\n", " ")))
                                imageContents.Add(new TextContent(block.Text, box));
                            else
                            {
                                if ((box.Width < box.Height && box.Width < box.Height / 10) || (box.Width > box.Height && box.Height < box.Width / 10)) continue;
                                imageContents.Add(new ImageContent(block.ToBitmap(Input), box));
                            }
                        }
                    }
                    Confidence = Result.Confidence;
                }
            });

            Task.Delay(10);
            task.Start();
            task.GetAwaiter().OnCompleted(() =>
            {
                Label label2 = new Label();
                label2.Text = $"Confidence: {Confidence.ToString("0.00").Replace(",", ".")}%";
                label2.Location = Point.Subtract(new Point(label2.Width, ClientSize.Height), label2.Size);
                Controls.Add(label2);

                Button saveAsODT = new Button();
                saveAsODT.Text = "Save as .odt file.";
                saveAsODT.Click += SaveAsODT;
                saveAsODT.Location = Point.Subtract(new Point(ClientSize.Width, ClientSize.Height), saveAsODT.Size);
                Controls.Add(saveAsODT);

                foreach (Content imageContent in imageContents)
                    Controls.AddRange(imageContent.GetControls().ToArray());
                foreach (Control control in Controls)
                    if (control != pictureBox)
                        control.BringToFront();
            });

            void SaveAsODT(object _sender, EventArgs _e)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Open Office Document|*.odt";
                saveFileDialog.FileName = openFileDialog.FileName.Split('\\').Last().Replace(".png", ".odt");
                if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
                if (imageContents == null) return;

                TextDocument document = new TextDocument();
                document.New();
                if (!Directory.Exists(saveFileDialog.FileName + "_Temp\\"))
                    Directory.CreateDirectory(saveFileDialog.FileName + "_Temp\\");
                foreach (Content imageContent in imageContents)
                    imageContent.GetParagraph(document, saveFileDialog.FileName);
                document.SaveTo(saveFileDialog.FileName);
                if (Directory.Exists(saveFileDialog.FileName + "_Temp\\"))
                    Directory.Delete(saveFileDialog.FileName + "_Temp\\", true);
            }
        }
    }
}



//List<Control> controls = new List<Control>();
/*
HtmlPage page = new HtmlPage();
page.Head.Title = "Scan";
page.Head.Style = @"
body {
    height: 100%;
    margin: 0;
    border: 0;
    padding: 0;
}
h3 {
    position: absolute;
    bottom: 0;
    width: 100%;
    text-align: center;
}
";
*/

//page.Body.Content += page.Body.Image("", "test.png");
//page.Head.MetaKeywords = "my,page";
//page.Head.Style = "h1 { font-family: 'arial' } p { font-family: 'arial' } ul { font-family: 'arial' }";
//page.Body.Content += page.Body.H1Text("My Page");
//page.Body.Content += page.Body.Paragraph("This is some sample text.");
//page.Body.Content += page.Body.UnorderedList(new List<string> { "foo", "bar", "baz" });



/*
                    foreach (var Page in Result.Pages)
                    {
                        // Page object
                        int PageNumber = Page.PageNumber;
                        string PageText = Page.Text;
                        int PageWordCount = Page.WordCount;
                        OcrResult.Barcode[] Barcodes = Page.Barcodes;
                        System.Drawing.Bitmap PageImage = Page.ToBitmap(Input);
                        PageImage.Save($"test/PageImage/{PageNumber}.png");
                        int PageWidth = Page.Width;
                        int PageHeight = Page.Height;
                        foreach (var Paragraph in Page.Paragraphs)
                        {
                            // Pages -> Paragraphs
                            int ParagraphNumber = Paragraph.ParagraphNumber;
                            String ParagraphText = Paragraph.Text;
                            System.Drawing.Bitmap ParagraphImage = Paragraph.ToBitmap(Input);
                            ParagraphImage.Save($"test/ParagraphImage/{ParagraphNumber}.png");
                            int ParagraphX_location = Paragraph.X;
                            int ParagraphY_location = Paragraph.Y;
                            int ParagraphWidth = Paragraph.Width;
                            int ParagraphHeight = Paragraph.Height;
                            double ParagraphOcrAccuracy = Paragraph.Confidence;
                            OcrResult.TextFlow paragrapthText_direction = Paragraph.TextDirection;
                            foreach (var Line in Paragraph.Lines)
                            {
                                // Pages -> Paragraphs -> Lines
                                int LineNumber = Line.LineNumber;
                                String LineText = Line.Text;
                                System.Drawing.Bitmap LineImage = Line.ToBitmap(Input);
                                LineImage.Save($"test/LineImage/{LineNumber}.png");
                                int LineX_location = Line.X;
                                int LineY_location = Line.Y;
                                int LineWidth = Line.Width;
                                int LineHeight = Line.Height;
                                double LineOcrAccuracy = Line.Confidence;
                                double LineSkew = Line.BaselineAngle;
                                double LineOffset = Line.BaselineOffset;
                                foreach (var Word in Line.Words)
                                {
                                    // Pages -> Paragraphs -> Lines -> Words
                                    int WordNumber = Word.WordNumber;
                                    String WordText = Word.Text;
                                    System.Drawing.Image WordImage = Word.ToBitmap(Input);
                                    WordImage.Save($"test/WordImage/{WordNumber}.png");
                                    int WordX_location = Word.X;
                                    int WordY_location = Word.Y;
                                    int WordWidth = Word.Width;
                                    int WordHeight = Word.Height;
                                    double WordOcrAccuracy = Word.Confidence;
                                    if (Word.Font != null)
                                    {
                                        // Word.Font is only set when using Tesseract Engine Modes rather than LTSM
                                        String FontName = Word.Font.FontName;
                                        double FontSize = Word.Font.FontSize;
                                        bool IsBold = Word.Font.IsBold;
                                        bool IsFixedWidth = Word.Font.IsFixedWidth;
                                        bool IsItalic = Word.Font.IsItalic;
                                        bool IsSerif = Word.Font.IsSerif;
                                        bool IsUnderLined = Word.Font.IsUnderlined;
                                        bool IsFancy = Word.Font.IsCaligraphic;
                                    }
                                    foreach (var Character in Word.Characters)
                                    {
                                        // Pages -> Paragraphs -> Lines -> Words -> Characters
                                        int CharacterNumber = Character.CharacterNumber;
                                        String CharacterText = Character.Text;
                                        System.Drawing.Bitmap CharacterImage = Character.ToBitmap(Input);
                                        CharacterImage.Save($"test/CharacterImage/{CharacterNumber}.png");
                                        int CharacterX_location = Character.X;
                                        int CharacterY_location = Character.Y;
                                        int CharacterWidth = Character.Width;
                                        int CharacterHeight = Character.Height;
                                        double CharacterOcrAccuracy = Character.Confidence;
                                        // Output alternative symbols choices and their probability.
                                        // Very useful for spellchecking
                                        OcrResult.Choice[] Choices = Character.Choices;
                                    }
                                }
                            }
                        }
                    }
                    */



//page.Body.Content += page.Body.H3Text($"Confidence: {Result.Confidence.ToString("0.00").Replace(",", ".")}%", style: "position: absolute; bottom: 0; left: 0;");




/*
float imagesizeoffsetX = (float)image.Size.Width / (float)Page.ToBitmap(Input).Size.Width;
float imagesizeoffsetY = (float)image.Size.Height / (float)Page.ToBitmap(Input).Size.Height;
Graphics.FromImage(pictureBox.Image).DrawRectangle(new Pen(Color.Red, 0.1f), new Rectangle((int)(block.X * imagesizeoffsetX), (int)(block.Y * imagesizeoffsetY), (int)(block.Width * imagesizeoffsetX), (int)(block.Height * imagesizeoffsetY)));

TextBox textBox = new TextBox();
textBox.Location = new Point((int)(block.X * imagesizeoffsetX), (int)(block.Y * imagesizeoffsetY));
textBox.Size = new Size((int)(block.Width * imagesizeoffsetX), (int)(block.Height * imagesizeoffsetY)) + new Size(5, 5);
textBox.Text = block.Text;
controls.Add(textBox);
*/




/*HtmlPanel htmlPanel = new HtmlPanel();
htmlPanel.Text = page.ToString();
htmlPanel.Dock = DockStyle.Fill;
Controls.Add(htmlPanel);*/

//Console.WriteLine(page.ToString());

//*************************************************************************************************
//* (C) ColorfulSoft corp., 2021. All Rights reserved.
//*************************************************************************************************

using System;
using System.Threading;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ColorfulSoft.DeOldify
{

    public sealed class HelpForm : Form
    {

        private PictureBox __Preview;

        private Label __Text;

        public HelpForm() : base()
        {
            // this
            this.Text = "About";
            this.Icon = Icon.FromHandle((new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Info.png"))).GetHicon());
            this.BackColor = SystemColors.ControlDarkDark;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ClientSize = new Size(540, 358);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            // Preview
            this.__Preview = new PictureBox();
            this.__Preview.Width = 520;
            this.__Preview.Height = 280;
            this.__Preview.Top = 10;
            this.__Preview.Left = 10;
            this.__Preview.SizeMode = PictureBoxSizeMode.Zoom;
            this.__Preview.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Preview.jpg"));
            this.Controls.Add(this.__Preview);
            // Text
            this.__Text = new Label();
            this.__Text.Width = 520;
            this.__Text.Height = 48;
            this.__Text.Top = 300;
            this.__Text.Left = 10;
            this.__Text.Text = "* Neural network architecture and weights by Jason Antic (https://github.com/jantic/DeOldify)\n* This application is developed by Gleb S. Brykin from ColorfulSoft(https://github.com/ColorfulSoft)\n© ColorfulSoft corp., 2021. All Rights reserved";
            this.__Text.ForeColor = SystemColors.Control;
            this.Controls.Add(this.__Text);
        }

    }

    public sealed class StartButton : UserControl
    {

        private Graphics __Graphics;

        private void Redraw()
        {
            this.__Graphics.Clear(SystemColors.ControlDark);
            if(this.Progress == 0)
            {
                this.__Graphics.DrawString(this.Text, this.Font, SystemBrushes.ControlText, Math.Max((this.Width - this.Text.Length * this.Font.Size) / 2, 0), Math.Max((this.Height - this.Font.Height) / 2, 0));
                return;
            }
            this.__Graphics.FillRectangle(Brushes.DarkGreen, 0, 0, this.Width / 100f * this.Progress, this.Height);
            var txt = this.Text + (this.__ShowProgress ? string.Format("({0,2:0.##} %)", this.Progress) : "");
            this.__Graphics.DrawString(txt, this.Font, SystemBrushes.ControlText, Math.Max((this.Width - txt.Length * this.Font.Size) / 2, 0), Math.Max((this.Height - this.Font.Height) / 2, 0));
        }

        private bool __ShowProgress = false;

        public bool ShowProgress
        {

            get
            {
                return this.__ShowProgress;
            }

            set
            {
                this.__ShowProgress = value;
                Redraw();
            }

        }

        public StartButton() : base()
        {
            this.__Graphics = this.CreateGraphics();
            this.Paint += delegate
            {
                this.Redraw();
            };
            this.Resize += delegate
            {
                this.__Graphics = this.CreateGraphics();
                this.Redraw();
            };
            this.TextChanged += delegate
            {
                this.Redraw();
            };
            this.MouseEnter += delegate
            {
                if(this.Progress == 0)
                {
                    this.__Graphics.Clear(SystemColors.Control);
                    this.__Graphics.DrawString(this.Text, this.Font, SystemBrushes.ControlText, Math.Max((this.Width - this.Text.Length * this.Font.Size) / 2, 0), Math.Max((this.Height - this.Font.Height) / 2, 0));
                }
            };
            this.MouseLeave += delegate
            {
                if(this.Progress == 0)
                {
                    this.__Graphics.Clear(SystemColors.ControlDark);
                    this.__Graphics.DrawString(this.Text, this.Font, SystemBrushes.ControlText, Math.Max((this.Width - this.Text.Length * this.Font.Size) / 2, 0), Math.Max((this.Height - this.Font.Height) / 2, 0));
                }
            };
        }

        private float __Progress;

        public float Progress
        {
            get
            {
                return this.__Progress;
            }
            set
            {
                if((value < 0) || (value > 100))
                {
                    throw new ArgumentException("Progress should be in [0..100] range.");
                }
                this.__Progress = value;
                this.Redraw();
            }
        }

    }

    public sealed class FlatButton : UserControl
    {

        private Graphics __Graphics;

        private void Redraw()
        {
            this.__Graphics.Clear(SystemColors.ControlDark);
            this.__Graphics.DrawString(this.Text, this.Font, SystemBrushes.ControlText, Math.Max((this.Width - this.Text.Length * this.Font.Size) / 2, 0), Math.Max((this.Height - this.Font.Height) / 2, 0));
            if(this.__Image != null)
            {
                this.__Graphics.DrawImage(this.__Image, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
        }

        private Bitmap __Image;

        public Bitmap Image
        {

            get
            {
                return this.__Image;
            }

            set
            {
                this.__Image = value;
                this.Redraw();
            }

        }

        public FlatButton() : base()
        {
            this.__Graphics = this.CreateGraphics();
            this.Paint += delegate
            {
                this.Redraw();
            };
            this.Resize += delegate
            {
                this.__Graphics = this.CreateGraphics();
                this.Redraw();
            };
            this.TextChanged += delegate
            {
                this.Redraw();
            };
            this.MouseEnter += delegate
            {
                this.__Graphics.Clear(SystemColors.Control);
                this.__Graphics.DrawString(this.Text, this.Font, SystemBrushes.ControlText, Math.Max((this.Width - this.Text.Length * this.Font.Size) / 2, 0), Math.Max((this.Height - this.Font.Height) / 2, 0));
                if(this.__Image != null)
                {
                    this.__Graphics.DrawImage(this.__Image, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
                }
            };
            this.MouseLeave += delegate
            {
                this.__Graphics.Clear(SystemColors.ControlDark);
                this.__Graphics.DrawString(this.Text, this.Font, SystemBrushes.ControlText, Math.Max((this.Width - this.Text.Length * this.Font.Size) / 2, 0), Math.Max((this.Height - this.Font.Height) / 2, 0));
                if(this.__Image != null)
                {
                    this.__Graphics.DrawImage(this.__Image, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
                }
            };
        }

    }

    public sealed class MainForm : Form
    {

        private HelpForm __HelpForm;

        private FlatButton __HelpButton;

        private GroupBox __InputBox;

        private PictureBox __InputImage;

        private Bitmap __Input;

        private Bitmap __NormalInput;

        private Bitmap __BlurryInput;

        private FlatButton __OpenInput;

        private GroupBox __OutputBox;

        private PictureBox __OutputImage;

        private Bitmap __Output;

        private Bitmap __NormalOutput;

        private Bitmap __BlurryOutput;

        private FlatButton __SaveOutput;

        private Bitmap __ColorfulSoftLogo;

        private StartButton __StartButton;

        private Thread __ColorizationThread;

        private static Bitmap __Blurify(Bitmap Source)
        {
            var Output = new Bitmap(Source.Width, Source.Height);
            for(int y = 0; y < Output.Height; ++y)
            {
                for(int x = 0; x < Output.Width; ++x)
                {
                    var A = 0f;
                    var R = 0f;
                    var G = 0f;
                    var B = 0f;
                    for(int ky = 0; ky < 5; ++ky)
                    {
                        var iy = y + ky - 2;
                        if((iy < 0) || (iy >= Source.Height))
                        {
                            continue;
                        }
                        for(int kx = 0; kx < 5; ++kx)
                        {
                            var ix = x + kx - 2;
                            if((ix < 0) || (ix >= Source.Width))
                            {
                                continue;
                            }
                            var C = Source.GetPixel(ix, iy);
                            A += C.A;
                            R += C.R;
                            G += C.G;
                            B += C.B;
                        }
                    }
                    Output.SetPixel(x, y, Color.FromArgb((byte)(A / 25), (byte)(R / 25), (byte)(G / 25), (byte)(B / 25)));
                }
            }
            return Output;
        }

        private static Bitmap __Decolorize(Bitmap Source)
        {
            var Result = new Bitmap(Source);
            for(int y = 0; y < Result.Height; ++y)
            {
                for(int x = 0; x < Result.Width; ++x)
                {
                    var C = Result.GetPixel(x, y);
                    var L = (byte)((C.R + C.G + C.B) / 3);
                    Result.SetPixel(x, y, Color.FromArgb(C.A, L, L, L));
                }
            }
            return Result;
        }

        private void __SetInputImage(Bitmap Source)
        {
            Source = __Decolorize(Source);
            this.__Input = Source;
            if(Source.Height > Source.Width)
            {
                this.__InputImage.Image = new Bitmap(Source, (int)(256f / Source.Height * Source.Width), 256);
            }
            else
            {
                this.__InputImage.Image = new Bitmap(Source, 256, (int)(256f / Source.Width * Source.Height));
            }
            this.__NormalInput = (Bitmap)this.__InputImage.Image;
            this.__BlurryInput = __Blurify((Bitmap)this.__InputImage.Image);
            if(this.__OutputImage != null)
            {
                this.__Output = null;
                this.__BlurryOutput = null;
                this.__NormalOutput = null;
                this.__OutputImage.Image = this.__ColorfulSoftLogo;
                this.__OutputImage.Enabled = false;
            }
            if(this.__StartButton != null)
            {
                this.__StartButton.Enabled = true;
                this.__StartButton.Text = "DeOldify!";
                this.__StartButton.Progress = 0f;
            }
        }

        private void __SetOutputImage(Bitmap Source)
        {
            this.__Output = Source;
            if(Source.Height > Source.Width)
            {
                this.__OutputImage.Image = new Bitmap(Source, (int)(256f / Source.Height * Source.Width), 256);
            }
            else
            {
                this.__OutputImage.Image = new Bitmap(Source, 256, (int)(256f / Source.Width * Source.Height));
            }
            this.__NormalOutput = (Bitmap)this.__OutputImage.Image;
            this.__BlurryOutput = __Blurify((Bitmap)this.__OutputImage.Image);
        }

        private void StopHandler(object sender, EventArgs e)
        {
            this.__ColorizationThread.Abort();
            this.__StartButton.Text = "DeOldify!";
            this.__StartButton.Progress = 0f;
            this.__StartButton.Click -= StopHandler;
            this.__StartButton.Click += StartHandler;
            this.__InputImage.Enabled = true;
            this.__OutputImage.Enabled = true;
            this.__StartButton.ShowProgress = false;
        }

        private void StartHandler(object sender, EventArgs e)
        {
            this.__InputImage.Enabled = false;
            this.__OutputImage.Enabled = false;
            this.__StartButton.ShowProgress = true;
            this.__StartButton.Text = "Stop";
            this.__StartButton.Click -= this.StartHandler;
            this.__StartButton.Click += this.StopHandler;
            this.__ColorizationThread = new Thread(() =>
            {
                this.__Output = DeOldify.Colorize(this.__Input);
                if(this.__Output.Height > this.__Output.Width)
                {
                    this.__NormalOutput = new Bitmap(this.__Output, (int)(256f / this.__Output.Height * this.__Output.Width), 256);
                }
                else
                {
                    this.__NormalOutput = new Bitmap(this.__Output, 256, (int)(256f / this.__Output.Width * this.__Output.Height));
                }
                this.__BlurryOutput = __Blurify(this.__NormalOutput);
                this.__OutputImage.Image = this.__NormalOutput;
                this.__OutputImage.Enabled = true;
                this.__InputImage.Enabled = true;
                this.__StartButton.Text = "Done!";
                this.__StartButton.Enabled = false;
                this.__StartButton.ShowProgress = false;
                this.__StartButton.Click -= this.StopHandler;
                this.__StartButton.Click += this.StartHandler;
                GC.Collect();
            });
            this.__ColorizationThread.Start();
        }

        public MainForm() : base()
        {
            this.Text = "DeOldify.NET";
            this.Icon = Icon.FromHandle((new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Icon.png"))).GetHicon());
            this.BackColor = SystemColors.ControlDarkDark;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ClientSize = new Size(582, 363);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Closing += delegate
            {
                try
                {
                    this.__ColorizationThread.Abort();
                }
                catch
                {
                }
            };
            // HelpForm
            this.__HelpForm = new HelpForm();
            // HelpButton
            this.__HelpButton = new FlatButton();
            this.__HelpButton.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Info.png"));
            this.__HelpButton.Width = 22;
            this.__HelpButton.Height = 22;
            this.__HelpButton.Top = 1;
            this.__HelpButton.Left = 559;
            this.__HelpButton.Click += delegate
            {
                this.__HelpForm.ShowDialog();
            };
            this.Controls.Add(this.__HelpButton);
            // InputBox
            this.__InputBox = new GroupBox();
            this.__InputBox.Size = new Size(276, 286);
            this.__InputBox.Text = "B&W image";
            this.__InputBox.Top = 22;
            this.__InputBox.Left = 10;
            this.__InputBox.ForeColor = SystemColors.Control;
              //-> InputImage
              this.__InputImage = new PictureBox();
              this.__InputImage.Size = new Size(256, 256);
              this.__InputImage.Top = 20;
              this.__InputImage.Left = 10;
              this.__InputImage.BackColor = SystemColors.ControlDarkDark;
              this.__InputImage.BorderStyle = BorderStyle.FixedSingle;
              this.__InputImage.SizeMode = PictureBoxSizeMode.Zoom;
              this.__InputImage.MouseEnter += delegate
              {
                  this.__InputImage.Image = this.__BlurryInput;
                  this.__OpenInput.Show();
              };
              this.__InputImage.MouseLeave += delegate
              {
                  this.__InputImage.Image = this.__NormalInput;
                  var p = Control.MousePosition;
                  if(this.__OpenInput.ClientRectangle.Contains(this.__OpenInput.PointToClient(p)))
                  {
                      this.__InputImage.Image = this.__BlurryInput;
                      return;
                  }
                  this.__OpenInput.Hide();
              };
              this.__SetInputImage(new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Input.jpg")));
              this.__InputBox.Controls.Add(this.__InputImage);
              //-> OpenInput
              this.__OpenInput = new FlatButton();
              this.__OpenInput.Enabled = true;
              this.__OpenInput.Top = 115;
              this.__OpenInput.Left = 40;
              this.__OpenInput.Size = new Size(176, 25);
              this.__OpenInput.Text = "Open B&W image";
              this.__OpenInput.Hide();
              this.__OpenInput.Click += delegate
              {
                  this.__OpenInput.Hide();
                  var OFD = new OpenFileDialog();
                  OFD.Title = "Open";
                  OFD.Filter = "Images (*.bmp; *.emf; *.exif; *.gif; *.ico; *.jpg; *.png; *.tiff; *.wmf)|*.bmp; *.emf; *.exif; *.gif; *.ico; *.jpg; *.png; *.tiff; *.wmf|All files|*.*";
                  if(OFD.ShowDialog() == DialogResult.OK)
                  {
                      this.__SetInputImage(new Bitmap(OFD.FileName));
                  }
              };
              this.__InputImage.Controls.Add(this.__OpenInput);
            this.Controls.Add(this.__InputBox);
            //-> __OutputBox
            this.__OutputBox = new GroupBox();
            this.__OutputBox.Size = new Size(276, 286);
            this.__OutputBox.Text = "Result";
            this.__OutputBox.Top = 22;
            this.__OutputBox.Left = 296;
            this.__OutputBox.ForeColor = SystemColors.Control;
              //-> OutputImage
              this.__OutputImage = new PictureBox();
              this.__OutputImage.Size = new Size(256, 256);
              this.__OutputImage.Top = 20;
              this.__OutputImage.Left = 10;
              this.__OutputImage.BackColor = SystemColors.ControlDarkDark;
              this.__OutputImage.BorderStyle = BorderStyle.FixedSingle;
              this.__OutputImage.SizeMode = PictureBoxSizeMode.Zoom;
              this.__OutputImage.MouseEnter += delegate
              {
                  this.__OutputImage.Image = this.__BlurryOutput;
                  this.__SaveOutput.Show();
              };
              this.__OutputImage.MouseLeave += delegate
              {
                  this.__OutputImage.Image = this.__NormalOutput;
                  var p = Control.MousePosition;
                  if(this.__SaveOutput.ClientRectangle.Contains(this.__SaveOutput.PointToClient(p)))
                  {
                      this.__OutputImage.Image = this.__BlurryOutput;
                      return;
                  }
                  this.__SaveOutput.Hide();
              };
              this.__SetOutputImage(new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Output.jpg")));
              this.__OutputBox.Controls.Add(this.__OutputImage);
              //-> SaveOutput
              this.__SaveOutput = new FlatButton();
              this.__SaveOutput.Top = 115;
              this.__SaveOutput.Left = 40;
              this.__SaveOutput.Size = new Size(176, 25);
              this.__SaveOutput.Text = "Save";
              this.__SaveOutput.Hide();
              this.__SaveOutput.Click += delegate
              {
                  this.__SaveOutput.Hide();
                  var SFD = new SaveFileDialog();
                  SFD.Title = "Save colorized";
                  SFD.Filter = "Images (*.bmp)|*.bmp|Images (*.emf)|*.emf|Images (*.exif)|*.exif|Images (*.gif)|*.gif|Images (*.ico)|*.ico|Images (*.jpg)|*.jpg|Images (*.png)|*.png|Images (*.tiff)|*.tiff|Images (*.wmf)|*.wmf";
                  if(SFD.ShowDialog() == DialogResult.OK)
                  {
                      switch(SFD.FilterIndex)
                      {
                          case 1:
                          {
                              this.__Output.Save(SFD.FileName, ImageFormat.Bmp);
                              break;
                          }
                          case 2:
                          {
                              this.__Output.Save(SFD.FileName, ImageFormat.Emf);
                              break;
                          }
                          case 3:
                          {
                              this.__Output.Save(SFD.FileName, ImageFormat.Exif);
                              break;
                          }
                          case 4:
                          {
                              this.__Output.Save(SFD.FileName, ImageFormat.Gif);
                              break;
                          }
                          case 5:
                          {
                              this.__Output.Save(SFD.FileName, ImageFormat.Icon);
                              break;
                          }
                          case 6:
                          {
                              this.__Output.Save(SFD.FileName, ImageFormat.Jpeg);
                              break;
                          }
                          case 7:
                          {
                              this.__Output.Save(SFD.FileName, ImageFormat.Png);
                              break;
                          }
                          case 8:
                          {
                              this.__Output.Save(SFD.FileName, ImageFormat.Tiff);
                              break;
                          }
                          case 9:
                          {
                              this.__Output.Save(SFD.FileName, ImageFormat.Wmf);
                              break;
                          }
                      }
                  }
              };
              this.__OutputImage.Controls.Add(this.__SaveOutput);
            this.Controls.Add(this.__OutputBox);
            //
            this.__ColorfulSoftLogo = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("ColorfulSoft.png"));
            //
            this.__StartButton = new StartButton();
            this.__StartButton.Top = 318;
            this.__StartButton.Left = 10;
            this.__StartButton.Width = 562;
            this.__StartButton.Height = 25;
            this.__StartButton.Text = "DeOldify!";
            this.__StartButton.Click += StartHandler;
            this.Controls.Add(this.__StartButton);
            //
            DeOldify.Progress += (float Percent) =>
            {
                this.__StartButton.Progress = Percent;
            };
        }

    }

}

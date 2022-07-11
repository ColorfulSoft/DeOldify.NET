//*************************************************************************************************
//* (C) ColorfulSoft corp., 2021 - 2022. All Rights reserved.
//*************************************************************************************************

using System;
using System.Threading;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ColorfulSoft.DeOldify
{

    /// <summary>
    /// A form with general information about the project.
    /// </summary>
    public sealed class HelpForm : Form
    {

        /// <summary>
        /// Picture box for preview image
        /// </summary>
        private readonly PictureBox __Preview;

        /// <summary>
        /// Label with general information about DeOldify.NET.
        /// </summary>
        private readonly Label __Text;

        /// <summary>
        /// Creates the form.
        /// </summary>
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
            this.__Text.Text = "* Neural network architecture and weights by Jason Antic (https://github.com/jantic/DeOldify)\n* This application is developed by Gleb S. Brykin from ColorfulSoft(https://github.com/ColorfulSoft)\n© ColorfulSoft corp., 2021 - 2022. All Rights reserved";
            this.__Text.ForeColor = SystemColors.Control;
            this.Controls.Add(this.__Text);
        }

    }

    /// <summary>
    /// Operation execution control button. Supports progress indication like ProgressBar.
    /// </summary>
    public sealed class StartButton : UserControl
    {

        /// <summary>
        /// The graphical shell of the control.
        /// </summary>
        private Graphics __Graphics;

        /// <summary>
        /// Redraws the control.
        /// </summary>
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

        /// <summary>
        /// Indicates whether the progress of the operation should be shown.
        /// </summary>
        private bool __ShowProgress = false;

        /// <summary>
        /// Indicates whether the progress of the operation should be shown.
        /// </summary>
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

        /// <summary>
        /// Initializes the control.
        /// </summary>
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

        /// <summary>
        /// Current progress of execution.
        /// </summary>
        private float __Progress;

        /// <summary>
        /// Gets or sets the current progress of execution.
        /// </summary>
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

    /// <summary>
    /// Flat button.
    /// </summary>
    public sealed class FlatButton : UserControl
    {

        /// <summary>
        /// Graphical shell of the control.
        /// </summary>
        private Graphics __Graphics;

        /// <summary>
        /// Redraws the control.
        /// </summary>
        private void Redraw()
        {
            this.__Graphics.Clear(SystemColors.ControlDark);
            this.__Graphics.DrawString(this.Text, this.Font, SystemBrushes.ControlText, Math.Max((this.Width - this.Text.Length * this.Font.Size) / 2, 0), Math.Max((this.Height - this.Font.Height) / 2, 0));
            if(this.__Image != null)
            {
                this.__Graphics.DrawImage(this.__Image, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
        }

        /// <summary>
        /// Image on the button.
        /// </summary>
        private Bitmap __Image;

        /// <summary>
        /// Image on the button.
        /// </summary>
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

        /// <summary>
        /// Initializes the control.
        /// </summary>
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

    /// <summary>
    /// The main form of the application.
    /// </summary>
    public sealed class MainForm : Form
    {

        /// <summary>
        /// Form with general information about this project.
        /// </summary>
        private HelpForm __HelpForm;

        /// <summary>
        /// Button to open HelpForm.
        /// </summary>
        private FlatButton __HelpButton;

        /// <summary>
        /// Contains input controls.
        /// </summary>
        private GroupBox __InputBox;

        /// <summary>
        /// Input image picture box.
        /// </summary>
        private PictureBox __InputImage;

        /// <summary>
        /// Input image.
        /// </summary>
        private Bitmap __Input;

        /// <summary>
        /// Normal input image.
        /// </summary>
        private Bitmap __NormalInput;

        /// <summary>
        /// Blurrified input image.
        /// </summary>
        private Bitmap __BlurryInput;

        /// <summary>
        /// Button to open input image.
        /// </summary>
        private FlatButton __OpenInput;

        /// <summary>
        /// Contains output controls.
        /// </summary>
        private GroupBox __OutputBox;

        /// <summary>
        /// Output image picture box.
        /// </summary>
        private PictureBox __OutputImage;

        /// <summary>
        /// Output image.
        /// </summary>
        private Bitmap __Output;

        /// <summary>
        /// Normal output image.
        /// </summary>
        private Bitmap __NormalOutput;

        /// <summary>
        /// Blurrified output image.
        /// </summary>
        private Bitmap __BlurryOutput;

        /// <summary>
        /// Button to save output image.
        /// </summary>
        private FlatButton __SaveOutput;

        /// <summary>
        /// ColorfulSoft's logo.
        /// </summary>
        private Bitmap __ColorfulSoftLogo;

        /// <summary>
        /// Button to start, stop and control colorization.
        /// </summary>
        private StartButton __StartButton;

        /// <summary>
        /// Thread for neural network.
        /// </summary>
        private Thread __ColorizationThread;

        /// <summary>
        /// Blurrifies the image.
        /// </summary>
        /// <param name="source">Input image.</param>
        /// <returns>Blurrified image.</returns>
        private static Bitmap __Blurify(Bitmap source)
        {
            var output = new Bitmap(source.Width, source.Height);
            for(int y = 0; y < output.Height; ++y)
            {
                for(int x = 0; x < output.Width; ++x)
                {
                    var a = 0f;
                    var r = 0f;
                    var g = 0f;
                    var b = 0f;
                    for(int ky = 0; ky < 5; ++ky)
                    {
                        var iy = y + ky - 2;
                        if((iy < 0) || (iy >= source.Height))
                        {
                            continue;
                        }
                        for(int kx = 0; kx < 5; ++kx)
                        {
                            var ix = x + kx - 2;
                            if((ix < 0) || (ix >= source.Width))
                            {
                                continue;
                            }
                            var c = source.GetPixel(ix, iy);
                            a += c.A;
                            r += c.R;
                            g += c.G;
                            b += c.B;
                        }
                    }
                    output.SetPixel(x, y, Color.FromArgb((byte)(a / 25), (byte)(r / 25), (byte)(g / 25), (byte)(b / 25)));
                }
            }
            return output;
        }

        /// <summary>
        /// Converts the image to greyscale.
        /// </summary>
        /// <param name="source">Input image.</param>
        /// <returns>Greyscale image.</returns>
        private static Bitmap __Decolorize(Bitmap source)
        {
            var result = new Bitmap(source);
            for(int y = 0; y < result.Height; ++y)
            {
                for(int x = 0; x < result.Width; ++x)
                {
                    var c = result.GetPixel(x, y);
                    var l = (byte)((c.R + c.G + c.B) / 3);
                    result.SetPixel(x, y, Color.FromArgb(c.A, l, l, l));
                }
            }
            return result;
        }

        /// <summary>
        /// Sets the input image.
        /// </summary>
        /// <param name="source">Input image.</param>
        private void __SetInputImage(Bitmap source)
        {
            source = __Decolorize(source);
            this.__Input = source;
            if(source.Height > source.Width)
            {
                this.__InputImage.Image = new Bitmap(source, (int)(256f / source.Height * source.Width), 256);
            }
            else
            {
                this.__InputImage.Image = new Bitmap(source, 256, (int)(256f / source.Width * source.Height));
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

        /// <summary>
        /// Sets output image.
        /// </summary>
        /// <param name="source">Output image.</param>
        private void __SetOutputImage(Bitmap source)
        {
            this.__Output = source;
            if(source.Height > source.Width)
            {
                this.__OutputImage.Image = new Bitmap(source, (int)(256f / source.Height * source.Width), 256);
            }
            else
            {
                this.__OutputImage.Image = new Bitmap(source, 256, (int)(256f / source.Width * source.Height));
            }
            this.__NormalOutput = (Bitmap)this.__OutputImage.Image;
            this.__BlurryOutput = __Blurify((Bitmap)this.__OutputImage.Image);
        }

        /// <summary>
        /// Stops the colorization process. Event Handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Args.</param>
        private void StopHandler(object sender, EventArgs e)
        {
            this.__ColorizationThread.Abort();
            this.__StartButton.Text = "DeOldify!";
            this.__StartButton.Progress = 0f;
            this.__StartButton.Click -= this.StopHandler;
            this.__StartButton.Click += this.StartHandler;
            this.__InputImage.Enabled = true;
            this.__OutputImage.Enabled = true;
            this.__StartButton.ShowProgress = false;
        }

        /// <summary>
        /// Starts the colorization process. Event handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Args.</param>
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

        /// <summary>
        /// Initializes the main form.
        /// </summary>
        public MainForm() : base()
        {
            this.Text =
            #if stable
                "Stable " +
            #else
                "Artistic " +
            #endif
                "DeOldify.NET v2.0" +
            #if simd
                " with SIMD" +
            #else
                "" +
            #endif
            #if half
                " w16";
            #else
                " w32";
            #endif
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
                  #if Linux
                  this.__InputImage.Controls.Add(this.__OpenInput);
                  #else
                  this.__OpenInput.Show();
                  #endif
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
                  #if Linux
                  this.__InputImage.Controls.Remove(this.__OpenInput);
                  #else
                  this.__OpenInput.Hide();
                  #endif
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
              this.__OpenInput.Click += delegate
              {
                  #if Linux
                  this.__InputImage.Controls.Remove(this.__OpenInput);
                  #else
                  this.__OpenInput.Hide();
                  #endif
                  var OFD = new OpenFileDialog();
                  OFD.Title = "Open";
                  OFD.Filter = "Images (*.bmp; *.emf; *.exif; *.gif; *.ico; *.jpg; *.png; *.tiff; *.wmf)|*.bmp; *.emf; *.exif; *.gif; *.ico; *.jpg; *.png; *.tiff; *.wmf|All files|*.*";
                  if(OFD.ShowDialog() == DialogResult.OK)
                  {
                      this.__SetInputImage(new Bitmap(OFD.FileName));
                  }
              };
              #if Linux
              #else
              this.__OpenInput.Hide();
              this.__InputImage.Controls.Add(this.__OpenInput);
              #endif
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
                  #if Linux
                  this.__OutputImage.Controls.Add(this.__SaveOutput);
                  #else
                  this.__SaveOutput.Show();
                  #endif
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
                  #if Linux
                  this.__OutputImage.Controls.Remove(this.__SaveOutput);
                  #else
                  this.__SaveOutput.Hide();
                  #endif
              };
              this.__SetOutputImage(new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Output.jpg")));
              this.__OutputBox.Controls.Add(this.__OutputImage);
              //-> SaveOutput
              this.__SaveOutput = new FlatButton();
              this.__SaveOutput.Top = 115;
              this.__SaveOutput.Left = 40;
              this.__SaveOutput.Size = new Size(176, 25);
              this.__SaveOutput.Text = "Save";
              this.__SaveOutput.Click += delegate
              {
                  #if Linux
                  this.__OutputImage.Controls.Remove(this.__SaveOutput);
                  #else
                  this.__SaveOutput.Hide();
                  #endif
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
              #if Linux
              #else
              this.__SaveOutput.Hide();
              this.__OutputImage.Controls.Add(this.__SaveOutput);
              #endif
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

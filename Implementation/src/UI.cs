//*************************************************************************************************
//* (C) ColorfulSoft corp., 2021 - 2022. All Rights reserved.
//*************************************************************************************************

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Net;

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
        private Button __HelpButton;

        /// <summary>
        // Table layout panel
        /// </summary>
        private TableLayoutPanel __TableLayoutPanel;

        /// <summary>
        /// Header panel
        /// </summary>
        private Panel __HeaderPanel;

        /// <summary>
        /// Source image location ComboBox label
        /// </summary>
        private Label __LabelLocation;

        /// <summary>
        /// Source image location ComboBox.
        /// </summary>
        private ComboBox __InputLocation;

        /// <summary>
        // Tab control
        /// </summary>
        private TabControl __TabControl;

        /// <summary>
        /// Split Container
        /// </summary>
        private SplitContainer __SplitContainer;

        /// <summary>
        /// Contains input controls.
        /// </summary>
        private Panel __InputBox;

        /// <summary>
        /// Drag hint label
        /// </summary>
        private Label __InputLabel;

        /// <summary>
        /// Input image picture box.
        /// </summary>
        private PictureBox __InputPictureBox;

        /// <summary>
        /// Input image.
        /// </summary>
        private Bitmap __InputImage;

        private ContextMenuStrip __InputBoxContextMenuStrip;
        private ToolStripMenuItem __InputBoxMenuItem1;

        /// <summary>
        /// Button to open file dialog.
        /// </summary>
        private Button __ButtonBrowse;

        /// <summary>
        /// Button to load input image file or URL.
        /// </summary>
        private Button __ButtonLoad;

        /// <summary>
        /// Contains output controls.
        /// </summary>
        private Panel __OutputBox;

        /// <summary>
        /// Output image picture box.
        /// </summary>
        private PictureBox __OutputPictureBox;

        /// <summary>
        /// Output image picture box context menu.
        /// </summary>
        private ContextMenuStrip __OutputBoxContextMenuStrip;
        private ToolStripMenuItem __OutputBoxMenuItem1;
        private ToolStripMenuItem __OutputBoxMenuItem2;
        private ToolStripMenuItem __OutputBoxMenuItem3;

        /// <summary>
        /// Button to save output image.
        /// </summary>
        private Button __ButtonSave;

        /// <summary>
        /// Footer panel
        /// </summary>
        private Panel __FooterPanel;

        /// <summary>
        /// Using stock ProgressBar
        /// </summary>
        private ProgressBar __ProgressBar;

        /// <summary>
        /// Label for progress status
        /// </summary>
        private Label __ProgressLabel;

        /// <summary>
        /// Button to start, stop and control colorization.
        /// </summary>
        private Button __StartButton;

        /// <summary>
        /// Button to desaturate an image
        /// </summary>
        private Button __DecolorizeButton;

        /// <summary>
        /// Thread for neural network.
        /// </summary>
        private Thread __ColorizationThread;

        /// <summary>
        /// The name of the open file.
        /// </summary>
        private string __ImageFileName;

        private string __ProgressLabelText = "{0} %";

        private TextBox __DebugMemo;

        /// <summary>
        /// Blurrifies the image.
        /// </summary>
        /// <param name="source">Input image.</param>
        /// <returns>Blurrified image.</returns>
        public static Bitmap __Blurify(Bitmap source)
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
        public static Bitmap __Decolorize(Bitmap source)
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
        /// Workaround for .NET Framework incomplete high DPI support. Scale from 96 to current form DPI.
        /// </summary>
        /// <param name="control_size">Size of control.</param>
        private Size __ScaleDPI(Size control_size)
        {
            SizeF scale_ratio = new SizeF(this.AutoScaleDimensions.Width / 96, this.AutoScaleDimensions.Height / 96);
            control_size = new Size (
                (int) Math.Ceiling (control_size.Width * scale_ratio.Width),
                (int) Math.Ceiling (control_size.Height * scale_ratio.Height)
            );
            return control_size;
        }

        /// <summary>
        /// Workaround for .NET Framework incomplete high DPI support. Scale from 96 to current form DPI.
        /// </summary>
        /// <param name="control_size">Size of control.</param>
        private int __ScaleDPI(int number, Boolean round_up = false)
        {
            float scale_ratio = this.AutoScaleDimensions.Width / 96;
            if (round_up)
                number = (int) Math.Ceiling (number * scale_ratio);
            else
                number = (int) Math.Floor (number * scale_ratio);

            return number;
        }

        /// <summary>
        /// Toggle controls.
        /// </summary>
        /// <param name="state">True to enable images and disable progress. False to disable images and enable progress.</param>
        private void ToggleControls(Boolean state) {
            // review: enumerated - enabled/disabled instead?
            if (state)
                __StartButton.Text = "DeOldify!";
            else
                __StartButton.Text = "Stop";
            __InputPictureBox.Enabled = state;
            __OutputPictureBox.Enabled = state;
            __ProgressBar.Visible = !state;
            __ProgressLabel.Visible = !state;
        }

        /// <summary>
        /// Stops the colorization process and image loading. Event Handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Args.</param>
        private void StopColorizationThread(object sender, EventArgs e)
        {

            __InputPictureBox.CancelAsync();

            if (__ColorizationThread != null) {
                __ColorizationThread.Interrupt();
                __ColorizationThread.Join();
            }

        }

        /// <summary>
        /// Starts the colorization process. Event handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Args.</param>
        private void StartColorizationThread(object sender, EventArgs e)
        {

            StopColorizationThread(sender, e);
            if (__InputImage == null)
                return;
            __ProgressBar.Maximum = 1000;
            __ProgressBar.Value = 0;
            ToggleControls(false);
            __StartButton.Click -= StartColorizationThread;
            __StartButton.Click += StopColorizationThread;

            __ColorizationThread = new Thread(() =>
            {
                try {
                    Bitmap OutputImage = DeOldify.Colorize(this.__InputImage);
                    if (this.InvokeRequired) {
                        this.BeginInvoke((MethodInvoker) delegate() {
                            if (OutputImage != null) {
                                this.__OutputPictureBox.Image = OutputImage;
                                this.__TabControl.SelectedIndex = 1;
                            }
                        });
                    }
                }
                catch (System.Threading.ThreadInterruptedException) {}
                catch (System.Threading.Tasks.TaskSchedulerException) {}
                catch (Exception ex) {
                    MessageBox.Show(String.Format("{0} ({1})", ex.Message, ex.HResult.ToString()) ,
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally {
                    //  GC.Collect(); // bugs. will collect by its own eventually.
                    if (this.InvokeRequired) {
                        this.BeginInvoke((MethodInvoker) delegate() {
                            __StartButton.Click -= StopColorizationThread;
                            __StartButton.Click += StartColorizationThread;
                            ToggleControls(true);
                        });
                    }
                }
            });
            __ColorizationThread.Start();

        }

        /// <summary>
        /// Starting image downloading.
        /// </summary>
        /// <param name="url">Image URL.</param>
        private Image DownloadImage (String url)
        {

            StopColorizationThread(this, null);

            __StartButton.Click -= StartColorizationThread;
            __StartButton.Click += StopColorizationThread;

            __ProgressBar.Maximum = 100;
            __ProgressBar.Minimum = 0;
            UpdateProgress(0);

            ToggleControls(false);

            __InputPictureBox.LoadAsync(url);
            return null;

        }

        /// <summary>
        /// Opening and image from location.
        /// </summary>
        /// <param name="sourceLocation">File or network location of an image.</param>
        private Boolean OpenImage (String sourceLocation)
        {

            __ImageFileName = "";
            __InputPictureBox.Image = null;
            __InputLabel.Visible = false;
            __InputPictureBox.SuspendLayout();

            if (Uri.IsWellFormedUriString(sourceLocation, UriKind.Absolute)) {
                DownloadImage(sourceLocation);
            }
            else {
                try
                {
                    __InputImage = new Bitmap(sourceLocation);
                    __ImageFileName = sourceLocation;
                    OpenImage(__InputImage, false);
                }
                catch (Exception ex) {
                    MessageBox.Show(String.Format("{0} ({1})" , ex.Message, ex.HResult),
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    __InputLabel.Visible = true;
                    __InputPictureBox.ResumeLayout();
                    return false;
                }
            }

            AddLocationHistory(sourceLocation);

            return true;
        }

        /// <summary>
        /// Opening and image from Bitmap.
        /// </summary>
        /// <param name="sourceImage">Source Bitmap.</param>
        /// <param name="clear">Clear load. I.e. not been called during another opening process.</param>
        private Boolean OpenImage (Bitmap sourceImage, Boolean clear = true)
        {

            if (clear)
                __ImageFileName = "";

            try
            {
                // __InputImage = __PrepareInputImage(sourceImage); // too slow, unnecessary for most images
                __InputImage = sourceImage;
                __InputPictureBox.Image = __InputImage;
                __InputLabel.Visible = false;
                if (__TabControl.SelectedIndex == 1)
                    __TabControl.SelectedIndex = 0;
                __OutputPictureBox.Image = null;
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format("{0} ({1})", ex.Message, ex.HResult.ToString()) ,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                __InputPictureBox.ResumeLayout();
            }

            return true;

        }

        /// <summary>
        /// Open/Paste an image from clipboard
        /// </summary>
        private Boolean OpenImageFromClipboard ()
        {

            try
            {
                if (Clipboard.ContainsImage()) {
                    OpenImage ( (Bitmap) Clipboard.GetImage() );
                    return true;
                }
            }
            // todo: show error message
            catch { }

            return false;

        }

        /// <summary>
        /// Saving an image
        /// </summary>
        /// <param name="saveas">True to show Save As dialog.</param>
        private Boolean SaveImage (Boolean saveas = false)
        {

            if (__OutputPictureBox.Image == null)
                return false;

            String destPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            String destFileName = "Deoldified";
            ImageFormat format = ImageFormat.Jpeg;

            if (!String.IsNullOrEmpty(__ImageFileName)) {
                destPath = Path.GetFullPath(__ImageFileName);
                destFileName = Path.GetFileNameWithoutExtension(__ImageFileName) + "-deoldified";
            }

            if (saveas) {
                var SFD = new SaveFileDialog();
                SFD.Title = "Save colorized";
                SFD.Filter = "BMP images (*.bmp)|*.bmp|EMF images (*.emf)|*.emf|EXIF images (*.exif)|*.exif|GIF images (*.gif)|*.gif|ICO images (*.ico)|*.ico|JPG images (*.jpg)|*.jpg|PNG images (*.png)|*.png|TIFF images (*.tiff)|*.tiff|WMF images (*.wmf)|*.wmf"; // For future use "WebP images (*.webp)|*.webp"
                SFD.FilterIndex = 7;
                SFD.FileName = destFileName;
                SFD.InitialDirectory = destPath;
                SFD.OverwritePrompt = true;
                if(SFD.ShowDialog() == DialogResult.OK)
                {
                    destPath = SFD.FileName;
                    if (Path.HasExtension(destPath)) {
                        format = GetImageFormat(destPath);
                        if (format == null)
                            format = GetImageFormat(SFD.FilterIndex);
                    }
                    else {
                        format = GetImageFormat(SFD.FilterIndex);
                    }
                }
                else
                {
                    return false;
                }
            }
            else {
                String[] joins = {destFileName, DateTime.Now.ToString("yyyyMMdd-HHmmss")};
                destFileName = String.Join("-", joins);
                destFileName += GetImageExtension(format);
                destPath = Path.Combine(destPath, destFileName);
                if (File.Exists(destPath)) {
                    DialogResult response =
                        MessageBox.Show (String.Format ("File {0} already exist. Do you wish to overwrite it?", destFileName), Application.ProductName,
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                    if (response == DialogResult.No)
                        return false;
                }
            }

            __OutputPictureBox.Image.Save(destPath, format);

            return true;

        }

        /// <summary>
        /// Copy an output image to clipboard.
        /// </summary>
        private void CopyImageToClipboard()
        {
            if (__OutputPictureBox.Image != null)
                Clipboard.SetDataObject((Bitmap)__OutputPictureBox.Image);
        }

        /// <summary>
        /// Returning ImageFormat based on file extension.
        /// </summary>
        /// <param name="fileName">Filename.</param>
        private ImageFormat GetImageFormat (String fileName)
        {
            String ext = Path.GetExtension(fileName).ToLower();
            switch(ext) {
                case ".bmp": return ImageFormat.Bmp;
                case ".emf": return ImageFormat.Emf;
                case ".exif": return ImageFormat.Exif;
                case ".gif": return ImageFormat.Gif;
                case ".ico": return ImageFormat.Icon;
                case ".jpg":
                case ".jpeg": return ImageFormat.Jpeg;
                case ".png": return ImageFormat.Png;
                case ".tif":
                case ".tiff": return ImageFormat.Tiff;
                case ".wmf": return ImageFormat.Wmf;
                // case ".webp": return ImageFormat.Webp; //WebP support is still broken as of .NET 7.0
                default: return null;
            }
        }

        /// <summary>
        /// Returning ImageFormat based on Save As dialog filter.
        /// </summary>
        /// <param name="filterIndex">Save As dialog filter index.</param>
        private ImageFormat GetImageFormat (int filterIndex)
        {
            switch(filterIndex)
            {
                case 1: return ImageFormat.Bmp;
                case 2: return ImageFormat.Emf;
                case 3: return ImageFormat.Exif;
                case 4: return ImageFormat.Gif;
                case 5: return ImageFormat.Icon;
                case 6: return ImageFormat.Jpeg;
                case 7: return ImageFormat.Png;
                case 8: return ImageFormat.Tiff;
                case 9: return ImageFormat.Wmf;
                // case 10: return ImageFormat.Webp; //WebP support is still broken as of .NET 7.0
                default: return ImageFormat.Jpeg;
            }
        }

        /// <summary>
        /// Returning file extension based on ImageFormat.
        /// </summary>
        /// <param name="format">ImageFormat.</param>
        private String GetImageExtension (ImageFormat format)
        {
            if (format == ImageFormat.Bmp)
                return ".bmp";
            if (format == ImageFormat.Emf)
                return ".emf";
            if (format == ImageFormat.Exif)
                return ".exif";
            if (format == ImageFormat.Icon)
                return ".ico";
            if (format == ImageFormat.Jpeg)
                return ".jpg";
            if (format == ImageFormat.Png)
                return ".png";
            if (format == ImageFormat.Tiff)
                return ".tif";
            if (format == ImageFormat.Wmf)
                return ".wmf";
            // if (format == ImageFormat.Webp)
            //     return ".webp";
            // if (format == ImageFormat.Heif)
            //     return ".heif";
            return ".jpeg";
        }

        /// <summary>
        /// Add a source location to Location ComboBox history
        /// </summary>
        /// <param name="sourceLocation">Source of an image.</param>
        private void AddLocationHistory (String sourceLocation)
        {
            __InputLocation.Items.Insert(0, sourceLocation);
            for (int i = 1; i < __InputLocation.Items.Count-1; i++) {
                if (__InputLocation.Items[i].ToString() == sourceLocation)
                    __InputLocation.Items.RemoveAt(i);
            }
            __InputLocation.Text = sourceLocation; // in .NET Text property will reset, when inserting new items.
        }

        /// <summary>
        /// Update progressBar and Label
        /// </summary>
        /// <param name="progressValue">Current progress position.</param>
        private void UpdateProgress (int progressValue)
        {
            __ProgressLabel.Text = String.Format(__ProgressLabelText, progressValue.ToString());
            __ProgressBar.Value = progressValue;
        }

        /// <summary>
        /// Update progressBar and Label
        /// </summary>
        /// <param name="progressValue">Current progress position.</param>
        private void UpdateProgress (float progressValue)
        {
            __ProgressBar.Value = (int)progressValue*10;
            __ProgressLabel.Text = String.Format(__ProgressLabelText, ((int)progressValue).ToString());
        }

        /// <summary>
        /// Initializes the main form.
        /// </summary>
        public MainForm() : base()
        {

            try
            {

            this.Text =
            #if stable
                "Stable " +
            #else
                "Artistic " +
            #endif
                "DeOldify.NET v2.1" +
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
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.MinimumSize = new Size(800, 800);
            this.Height = (int)(Screen.FromControl(this).Bounds.Height * 0.7);
            this.Width = (int)(this.Height * 1.2); //proportions closer to 3:2 and 4:3
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                this.Font = new Font("Segue UI", this.Font.Size);
            this.AllowDrop = true;
            this.KeyPreview = true;

            // HelpForm
            this.__HelpForm = new HelpForm();

            ////////////////////////////////////////////////////////////////////////////////
            // FORM LAYOUT
            this.__TableLayoutPanel = new TableLayoutPanel();
            this.__TableLayoutPanel.Location = new Point(0, 0);
            this.__TableLayoutPanel.Dock = DockStyle.Fill;
            this.__TableLayoutPanel.ColumnCount = 1;
            this.__TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
            this.__TableLayoutPanel.RowCount = 3;
            this.__TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, __ScaleDPI(42)));
            this.__TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50));
            this.__TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, __ScaleDPI(64)));
            this.__TableLayoutPanel.TabIndex = 0;
            this.__TableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            this.Controls.Add(this.__TableLayoutPanel);

            ////////////////////////////////////////////////////////////////////////////////
            // Header row

            this.__HeaderPanel = new Panel();
            this.__HeaderPanel.Dock = DockStyle.Fill;
            this.__TableLayoutPanel.Controls.Add(this.__HeaderPanel, 0, 0);

            this.__LabelLocation = new Label();
            this.__InputLocation = new ComboBox();
            this.__ButtonBrowse = new Button();
            this.__ButtonLoad = new Button();

            this.__LabelLocation.Text = "Image file or URL:";
            this.__LabelLocation.TextAlign = ContentAlignment.BottomLeft;
            this.__LabelLocation.AutoSize = true;
            this.__LabelLocation.Left = __ScaleDPI(18);
            this.__HeaderPanel.Controls.Add(this.__LabelLocation);

            this.__ButtonLoad.Text = "Load image";
            this.__ButtonLoad.AutoSize = true;
            this.__ButtonLoad.Left = this.__HeaderPanel.Width - this.__ButtonLoad.Width - __ScaleDPI(20);
            this.__ButtonLoad.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.__HeaderPanel.Controls.Add(this.__ButtonLoad);

            this.__ButtonBrowse.Text = "Browse";
            this.__ButtonBrowse.AutoSize = true;
            this.__ButtonBrowse.Left = this.__ButtonLoad.Left - this.__ButtonBrowse.Width - __ScaleDPI(20);
            this.__ButtonBrowse.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.__HeaderPanel.Controls.Add(this.__ButtonBrowse);

            this.__InputLocation.Location = new Point(
                this.__LabelLocation.Right + __ScaleDPI(10),
                (this.__HeaderPanel.ClientSize.Height - this.__InputLocation.Height) / 2
            );
            this.__InputLocation.Width = this.__ButtonBrowse.Left - this.__InputLocation.Left - __ScaleDPI(10);
            this.__InputLocation.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
            this.__HeaderPanel.Controls.Add(this.__InputLocation);

            this.__ButtonBrowse.MinimumSize = new Size(0, this.__InputLocation.Height);
            this.__LabelLocation.Top = this.__InputLocation.Top;
            this.__ButtonBrowse.Top = this.__InputLocation.Top - (this.__ButtonBrowse.Height - this.__InputLocation.Height) + __ScaleDPI(1, true);
            this.__ButtonLoad.Top = this.__InputLocation.Top - (this.__ButtonBrowse.Height - this.__InputLocation.Height) + __ScaleDPI(1, true);

            ////////////////////////////////////////////////////////////////////////////////
            // Main row

            this.__TabControl = new TabControl();
            this.__TabControl.Dock = DockStyle.Fill;
            this.__TabControl.TabPages.Add("Source");
            this.__TabControl.TabPages.Add("DeOldified");
            this.__TabControl.TabPages.Add("Side-by-side");
            this.__TableLayoutPanel.Controls.Add(__TabControl, 0, 1);

            this.__InputBox = new Panel();
            this.__InputBox.Dock = DockStyle.Fill;
            this.__InputBox.BackColor = Color.Gray;
            this.__TabControl.TabPages[0].Controls.Add(this.__InputBox);
              this.__InputPictureBox = new PictureBox();
              this.__InputPictureBox.BorderStyle = BorderStyle.None;
              this.__InputPictureBox.Dock = DockStyle.Fill;
              this.__InputPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
              this.__InputBox.Controls.Add(this.__InputPictureBox);
              this.__InputLabel = new Label();
              this.__InputLabel.Dock = DockStyle.Fill;
              this.__InputLabel.FlatStyle = FlatStyle.Flat;
              this.__InputLabel.TextAlign = ContentAlignment.MiddleCenter;
              this.__InputLabel.ForeColor = Color.LightGray;
              this.__InputLabel.BackColor = Color.Transparent;
              this.__InputLabel.Text = "(Drop or paste an image here)";
              this.__InputPictureBox.Controls.Add(this.__InputLabel);

              this.__InputBoxContextMenuStrip = new ContextMenuStrip();
              this.__InputBoxMenuItem1 = new ToolStripMenuItem();
              this.__InputBoxMenuItem1.Text = "Paste";
              this.__InputBoxContextMenuStrip.Items.Add( this.__InputBoxMenuItem1);
              this.__InputPictureBox.ContextMenuStrip =  this.__InputBoxContextMenuStrip;

            this.__OutputBox = new Panel();
            this.__OutputBox.Dock = DockStyle.Fill;
            this.__OutputBox.BackColor = Color.Gray;
            this.__TabControl.TabPages[1].Controls.Add(this.__OutputBox);
              this.__OutputPictureBox = new PictureBox();
              this.__OutputPictureBox.BorderStyle = BorderStyle.None;
              this.__OutputPictureBox.Dock = DockStyle.Fill;
              this.__OutputPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
              this.__OutputBox.Controls.Add(this.__OutputPictureBox);

              this.__OutputBoxContextMenuStrip = new ContextMenuStrip();
              this.__OutputBoxMenuItem1 = new ToolStripMenuItem();
              this.__OutputBoxMenuItem2 = new ToolStripMenuItem();
              this.__OutputBoxMenuItem3 = new ToolStripMenuItem();
              this.__OutputBoxMenuItem1.Text = "Copy";
              this.__OutputBoxMenuItem2.Text = "Save";
              this.__OutputBoxMenuItem3.Text = "Save As...";
              this.__OutputBoxContextMenuStrip.Items.Add( this.__OutputBoxMenuItem1);
              this.__OutputBoxContextMenuStrip.Items.Add( this.__OutputBoxMenuItem2);
              this.__OutputBoxContextMenuStrip.Items.Add( this.__OutputBoxMenuItem3);
              this.__OutputPictureBox.ContextMenuStrip =  this.__OutputBoxContextMenuStrip;

            this.__SplitContainer = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.__SplitContainer)).BeginInit();
            this.__SplitContainer.BorderStyle = BorderStyle.None;
            this.__SplitContainer.Dock = DockStyle.Fill;
            this.__SplitContainer.SplitterDistance = this.__SplitContainer.Width/2;
            this.__SplitContainer.BackColor = Color.Gray;
            this.__SplitContainer.Panel1.Padding = new Padding(4);
            this.__SplitContainer.Panel2.Padding = new Padding(4);
            this.__TabControl.TabPages[2].Controls.Add(this.__SplitContainer);
            this.__SplitContainer.ResumeLayout(false);

            ////////////////////////////////////////////////////////////////////////////////
            // Footer row row

            this.__FooterPanel = new Panel();
            this.__FooterPanel.Dock = DockStyle.Fill;
            this.__TableLayoutPanel.Controls.Add(this.__FooterPanel, 0, 2);

            this.__ProgressBar = new ProgressBar();
            this.__ProgressBar.Dock = DockStyle.Top;
            this.__ProgressBar.Height = __ScaleDPI(8);
            this.__ProgressBar.Step = 1;
            this.__FooterPanel.Controls.Add(this.__ProgressBar);

            this.__ProgressLabel = new Label();
            this.__ProgressLabel.Text = "0 %";
            this.__ProgressLabel.AutoSize = true;
            this.__ProgressLabel.Left = __ScaleDPI(10);
            this.__ProgressLabel.Top = (__FooterPanel.ClientSize.Height + __ProgressBar.Height - __ProgressLabel.Height) / 2;
            this.__ProgressLabel.Visible = false;
            this.__FooterPanel.Controls.Add(this.__ProgressLabel);

            this.__StartButton = new Button();
            this.__StartButton.Text = "DeOldify!";
            this.__StartButton.NotifyDefault(true);
            this.__StartButton.Size = __ScaleDPI(new Size(120, 25));
            this.__StartButton.Top = (__FooterPanel.ClientSize.Height + __ProgressBar.Height - __StartButton.Height) / 2;
            this.__StartButton.Left = this.__FooterPanel.ClientSize.Width - this.__StartButton.Width - __ScaleDPI(10);
            this.__StartButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.__StartButton.Click += StartColorizationThread;
            this.__FooterPanel.Controls.Add(this.__StartButton);

            this.__DecolorizeButton = new Button();
            this.__DecolorizeButton.Text = "Decolorize";
            this.__DecolorizeButton.Size = __ScaleDPI(new Size(120, 25));
            this.__DecolorizeButton.Location = new Point(this.__StartButton.Left - __ScaleDPI(10) - this.__DecolorizeButton.Width, this.__StartButton.Top);
            this.__DecolorizeButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.__FooterPanel.Controls.Add(this.__DecolorizeButton);

            this.__ButtonSave = new Button();
            this.__ButtonSave.Text = "Save";
            this.__ButtonSave.Size = __ScaleDPI(new Size(120, 25));
            this.__ButtonSave.Location = new Point(this.__DecolorizeButton.Left - __ScaleDPI(10) - this.__ButtonSave.Width, this.__StartButton.Top);
            this.__ButtonSave.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.__FooterPanel.Controls.Add(this.__ButtonSave);

            this.__HelpButton = new Button();
            this.__HelpButton.Text = "About...";
            this.__HelpButton.Size = __ScaleDPI(new Size(100, 25));
            this.__HelpButton.Location = new Point(this.__ButtonSave.Left - __ScaleDPI(10) - this.__HelpButton.Width, this.__StartButton.Top);
            this.__HelpButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.__FooterPanel.Controls.Add(this.__HelpButton);

            ToggleControls(true);

            // debug:
            this.__DebugMemo = new TextBox();
            __DebugMemo.Multiline = true;
            __DebugMemo.ScrollBars = ScrollBars.Both;
            __DebugMemo.Location = new Point(0, 0);
            __DebugMemo.Size = new Size(400, 600);
            __InputPictureBox.Controls.Add(__DebugMemo);

            __DebugMemo.BringToFront();
            __DebugMemo.Visible = false;
            // __DebugMemo.AppendText ( Environment.NewLine );

            ////////////////////////////////////////////////////////////////////////////////
            // Events

            this.__HelpButton.Click += delegate
            {
                __HelpForm.ShowDialog();
            };

            this.Closing += delegate
            {
                StopColorizationThread(this, null);
            };

            this.KeyDown += (s, e) =>
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
                    OpenImageFromClipboard();

                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
                    CopyImageToClipboard();
            };

            this.__InputLocation.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    OpenImage(__InputLocation.Text);
            };

            this.DragDrop += (object sender, DragEventArgs e) =>
            {

                if (e.Data == null)
                    return;

                if(e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    OpenImage(files[0]);
                }
                if(e.Data.GetDataPresent(DataFormats.Bitmap))
                {
                    OpenImage((Bitmap)e.Data.GetData(DataFormats.Bitmap));
                }

                StartColorizationThread(this, null);

            };

            this.DragEnter += (object sender, DragEventArgs e) =>
            {
                if(e.Data.GetDataPresent(DataFormats.Bitmap) ||
                   e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            };

            this.__TabControl.SelectedIndexChanged += delegate {

                if (__TabControl.SelectedIndex == 2) {
                    __InputBox.Parent = __SplitContainer.Panel1;
                    __OutputBox.Parent = __SplitContainer.Panel2;
                }
                else {
                    __InputBox.Parent = __TabControl.TabPages[0];
                    __OutputBox.Parent = __TabControl.TabPages[1];
                }

            };

            this.__InputPictureBox.MouseEnter += delegate
            {
                // this.__InputImage.Image = this.__BlurryInput;
                // this.__InputImage.Controls.Add(this.__ButtonOpen);
            };
            this.__InputPictureBox.MouseLeave += delegate
            {
                // this.__InputImage.Image = this.__NormalInput;
                // var p = Control.MousePosition;
                // if(this.__ButtonOpen.ClientRectangle.Contains(this.__ButtonOpen.PointToClient(p)))
                // {
                //     this.__InputImage.Image = this.__BlurryInput;
                //     return;
                // }
                // this.__InputImage.Controls.Remove(this.__ButtonOpen);
            };
            this.__ButtonBrowse.Click += delegate
            {
                var OFD = new OpenFileDialog();
                OFD.Title = "Open";
                OFD.Filter = "Images (*.bmp; *.emf; *.exif; *.gif; *.ico; *.jpg; *.png; *.tiff; *.wmf)|*.bmp; *.emf; *.exif; *.gif; *.ico; *.jpg; *.png; *.tiff; *.wmf|All files|*.*";
                if(OFD.ShowDialog() == DialogResult.OK) {
                    __InputLocation.Text = OFD.FileName;
                    OpenImage(OFD.FileName);
                }
            };
            this.__ButtonLoad.Click += delegate
            {
                OpenImage(__InputLocation.Text);
            };
            this.__OutputPictureBox.MouseEnter += delegate
            {
                // this.__OutputImage.Image = this.__BlurryOutput;
                // this.__OutputImage.Controls.Add(this.__ButtonSave);
            };
            this.__OutputPictureBox.MouseLeave += delegate
            {
                // this.__OutputImage.Image = this.__NormalOutput;
                // var p = Control.MousePosition;
                // if(this.__ButtonSave.ClientRectangle.Contains(this.__ButtonSave.PointToClient(p)))
                // {
                //     this.__OutputImage.Image = this.__BlurryOutput;
                //     return;
                // }
                // this.__OutputImage.Controls.Remove(this.__ButtonSave);
            };

            this.__InputBoxContextMenuStrip.ItemClicked += (object sender, ToolStripItemClickedEventArgs e) =>
            {
                __InputBoxContextMenuStrip.Hide(); // Workaround menu won't disappear until ItemClick event is finished.
                if (e.ClickedItem == __InputBoxMenuItem1) {
                    OpenImageFromClipboard();
                }
            };

            this.__OutputBoxContextMenuStrip.ItemClicked += (object sender, ToolStripItemClickedEventArgs e) =>
            {
                __OutputBoxContextMenuStrip.Hide(); // Workaround menu won't disappear until ItemClick event is finished.
                if (e.ClickedItem == __OutputBoxMenuItem1)
                    CopyImageToClipboard();
                if (e.ClickedItem == __OutputBoxMenuItem2)
                    SaveImage(false);
                if (e.ClickedItem == __OutputBoxMenuItem3)
                    SaveImage(true);
            };

            this.__DecolorizeButton.Click += delegate
            {
                if (__InputPictureBox.Image != null) {
                    __InputPictureBox.Image = __Decolorize((Bitmap)__InputPictureBox.Image);
                }
            };

            this.__ButtonSave.Click += delegate
            {
                SaveImage(true);
            };

            this.__InputPictureBox.LoadProgressChanged += (s, e) =>
            {
                __ProgressLabelText = "Downloading: {0} %";
                UpdateProgress (e.ProgressPercentage);
            };

            this.__InputPictureBox.LoadCompleted += (s, e) =>
            {

                try {

                    if (e.Error != null) {
                        if (e.Error is WebException) {
                            WebException ex = (WebException) e.Error;
                            if (ex.Response != null) {
                                HttpStatusCode statusCode = ((HttpWebResponse)(ex.Response)).StatusCode;
                                MessageBox.Show(String.Format("{0}: {1} ({2})" , statusCode, ex.Message, ex.HResult),
                                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else {
                                // debug: SSL error on many sites. Move to WebClient and its security options to resolve?
                                MessageBox.Show(String.Format("{0} ({1})", ex.Message, ex.HResult),
                                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else if (e.Error is System.ArgumentException) {
                            System.ArgumentException ex = (System.ArgumentException) e.Error;
                            MessageBox.Show(
                                String.Format("{0} ({1}).{2}Server is likely using unknown to .NET format, like WebP or AVIF.",
                                    ex.Message, ex.HResult.ToString(), Environment.NewLine + Environment.NewLine) ,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else {
                            Exception ex = e.Error;
                            MessageBox.Show(String.Format("{0} ({1})" , ex.Message, ex.HResult),
                                            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (e.Cancelled || e.Error != null) {
                        __InputPictureBox.Image = null;
                        __InputLabel.Visible = true;
                        __InputPictureBox.ResumeLayout();
                        return;
                    };

                    OpenImage ((Bitmap) __InputPictureBox.Image); //reopen to perform all tasks on image // review: load without picturebox?

                }
                catch (Exception ex) {
                    MessageBox.Show(String.Format("{0} ({1})", ex.Message, ex.HResult.ToString()) ,
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally {
                    __StartButton.Click -= StopColorizationThread;
                    __StartButton.Click += StartColorizationThread;
                    ToggleControls (true);
                }

            };

            DeOldify.Progress += (float Percent) =>
            {
                __ProgressLabelText = "Colorizing: {0} %";
                UpdateProgress(Percent);
            };

            ((System.ComponentModel.ISupportInitialize)(this.__SplitContainer)).EndInit();

            }
            catch (Exception ex){
                MessageBox.Show(String.Format("{0} ({1})", ex.Message, ex.HResult.ToString()) ,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

    }

}

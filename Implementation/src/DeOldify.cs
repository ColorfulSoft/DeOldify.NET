//*************************************************************************************************
//* (C) ColorfulSoft corp., 2021. All Rights reserved.
//*************************************************************************************************

using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;

namespace ColorfulSoft.DeOldify
{

    internal static unsafe class DeOldify
    {

        ///<summary>
        /// Loads data from the stream using BinaryReader.
        ///</summary>
        ///<param name="Reader">Source.</param>
        ///<param name="Shape">Shape of the tensor.</param>
        public unsafe static Tensor LoadTensor(BinaryReader Reader, params int[] Shape)
        {
            var t = new Tensor(Shape);
            var n = t.Numel;
            var Container = t.Data;
            for(int i = 0; i < n; ++i)
            {
                Container[i] = HalfHelper.HalfToSingle(Reader.ReadUInt16());
            }
            return t;
        }

        ///<summary>
        /// Parameters.
        ///</summary>
        public static Dictionary<string, Tensor> Parameters
        {

            get;

            private set;

        }

        ///<summary>
        /// Converts Bitmap to Tensor.
        ///</summary>
        ///<param name="bmp">Source.</param>
        public static Tensor Image2Tensor(Bitmap bmp)
        {
            var t = new Tensor(3, bmp.Height, bmp.Width);
            var pt = t.Data;
            for(int y = 0; y < bmp.Height; ++y)
            {
                for(int x = 0; x < bmp.Width; ++x)
                {
                    var C = bmp.GetPixel(x, y);
                    var L = (C.R + C.G + C.B) / 765f;
                    pt[y * bmp.Width + x] = (L - 0.485f) / 0.229f;
                    pt[(bmp.Height + y) * bmp.Width + x] = (L - 0.456f) / 0.224f;
                    pt[(2 * bmp.Height + y) * bmp.Width + x] = (L - 0.406f) / 0.225f;
                }
            }
            return t;
        }

        ///<summary>
        /// Converts Tensor to Bitmap.
        ///</summary>
        ///<param name="t">Source.</param>
        public static Bitmap Tensor2Image(Tensor t)
        {
            var bmp = new Bitmap(t.Shape[2], t.Shape[1]);
            for(int y = 0; y < t.Shape[1]; ++y)
            {
                for(int x = 0; x < t.Shape[2]; ++x)
                {
                    bmp.SetPixel(x, y, Color.FromArgb((byte)Math.Min(Math.Max(((t.Data[y * t.Shape[2] + x] * 6f - 3f) * 0.229f + 0.485f) * 255f, 0f), 255f),
                                                      (byte)Math.Min(Math.Max(((t.Data[(t.Shape[1] + y) * t.Shape[2] + x] * 6f - 3f) * 0.224f + 0.456f) * 255f, 0f), 255f),
                                                      (byte)Math.Min(Math.Max(((t.Data[(2 * t.Shape[1] + y) * t.Shape[2] + x] * 6f - 3f) * 0.225f + 0.406f) * 255f, 0f), 255f)));
                }
            }
            return bmp;
        }

        ///<summary>
        /// Transfers colors from colorized image to original B&W image.
        ///</summary>
        ///<param name="full_size">Original B&W image.</param>
        ///<param name="colorized">Colorized image.</param>
        public static Bitmap Mux(Bitmap full_size, Bitmap colorized)
        {
            var colorized_ = new Bitmap(colorized, full_size.Width, full_size.Height);
            for(int y = 0; y < colorized_.Height; ++y)
            {
                for(int x = 0; x < colorized_.Width; ++x)
                {
                    var bwc = full_size.GetPixel(x, y);
                    var rc = colorized_.GetPixel(x, y);
                    var bwy = 0.299f * bwc.R + 0.587f * bwc.G + 0.114f * bwc.B;
                    var ru = -0.14713f * rc.R - 0.28886f * rc.G + 0.436f * rc.B;
                    var rv = 0.615f * rc.R - 0.51499f * rc.G - 0.10001f * rc.B;
                    colorized_.SetPixel(x, y, Color.FromArgb((byte)Math.Min(Math.Max(bwy + 1.139837398373983740f * rv, 0f), 255f),
                                                             (byte)Math.Min(Math.Max(bwy - 0.3946517043589703515f * ru - 0.5805986066674976801f * rv, 0f), 255f),
                                                             (byte)Math.Min(Math.Max(bwy + 2.032110091743119266f * ru, 0f), 255f)));
                }
            }
            return colorized_;
        }

        ///<summary>
        /// Changes the image size using render scale table.
        ///</summary>
        ///<param name="bmp">Source.</param>
        public static Bitmap Resize(Bitmap bmp)
        {
            int factor = 320;
            var size = Math.Min(bmp.Width, bmp.Height);
            if(size <= 256)
            {
                factor = 192;
            }
            if((size > 256) && (size <= 768))
            {
                factor = 256;
            }
            if(bmp.Width > bmp.Height)
            {
                return new Bitmap(bmp, (int)((float)factor / bmp.Height * bmp.Width), factor);
            }
            else
            {
                return new Bitmap(bmp, factor, (int)((float)factor / bmp.Width * bmp.Height));
            }
        }

        ///<summary>
        /// Executes Functional.Conv2d with parameters from ckpt.
        ///</summary>
        ///<param name="x">Source.</param>
        ///<param name="layer">Layer name.</param>
        ///<param name="ckpt">State dict.</param>
        ///<param name="stride">Stride.</param>
        ///<param name="padding">Padding.</param>
        public static Tensor Conv2d(Tensor x, string layer, Dictionary<string, Tensor> ckpt, int stride = 1, int padding = 1)
        {
            var y = Functional.Conv2d(x, ckpt[layer + ".weight"], ckpt.ContainsKey(layer + ".bias") ? ckpt[layer + ".bias"] : null, padding, padding, padding, padding, stride, stride, 1, 1, 1);
            __Progress += Step;
            if(Progress != null)
            {
                Progress(Math.Min(Math.Max(__Progress, 0), 100));
            }
            return y;
        }

        ///<summary>
        /// Executes Functional.BatchNorm2d_ with parameters from ckpt.
        ///</summary>
        ///<param name="x">Source.</param>
        ///<param name="layer">Layer name.</param>
        ///<param name="ckpt">State dict.</param>
        public static Tensor BatchNorm2d(Tensor x, string layer, Dictionary<string, Tensor> ckpt)
        {
            return Functional.BatchNorm2d_(x, ckpt[layer + ".running_mean"], ckpt[layer + ".running_var"], ckpt[layer + ".weight"], ckpt[layer + ".bias"]);
        }

        ///<summary>
        /// Executes BasicBlock of DeOldify backbone with parameters from ckpt.
        ///</summary>
        ///<param name="x">Source.</param>
        ///<param name="layer">Layer name.</param>
        ///<param name="ckpt">State dict.</param>
        ///<param name="strided">Is stride == 2?</param>
        public static Tensor BasicBlock(Tensor x, string layer, Dictionary<string, Tensor> ckpt, bool strided = false)
        {
            var @out = Conv2d(x, layer + ".conv1", ckpt, strided ? 2 : 1, 1);
            @out = BatchNorm2d(@out, layer + ".bn1", ckpt);
            @out = Functional.ReLU_(@out);
            @out = Conv2d(@out, layer + ".conv2", ckpt, 1, 1);
            @out = BatchNorm2d(@out, layer + ".bn2", ckpt);
            if((@out.Shape[0] != x.Shape[0]) || strided)
            {
                x = BatchNorm2d(Conv2d(x, layer + ".downsample.0", ckpt, strided ? 2 : 1, 0), layer + ".downsample.1", ckpt);
            }
            return Functional.ReLU_(Functional.Plus_(@out, x));
        }

        ///<summary>
        /// Executes Middle of DeOldify with parameters from ckpt.
        ///</summary>
        ///<param name="x">Source.</param>
        ///<param name="layer">Layer name.</param>
        ///<param name="ckpt">State dict.</param>
        public static Tensor MiddleBlock(Tensor x, string layer, Dictionary<string, Tensor> ckpt)
        {
            return BatchNorm2d(Functional.ReLU_(Conv2d(BatchNorm2d(Functional.ReLU_(Conv2d(x, layer + ".0.0", ckpt)), layer + ".0.2", ckpt), layer + ".1.0", ckpt)), layer + ".1.2", ckpt);
        }

        ///<summary>
        /// Executes CustomPixelShuffle of DeOldify with parameters from ckpt.
        ///</summary>
        ///<param name="x">Source.</param>
        ///<param name="layer">Layer name.</param>
        ///<param name="ckpt">State dict.</param>
        public static Tensor CustomPixelShuffle(Tensor x, string layer, Dictionary<string, Tensor> ckpt)
        {
            return Functional.AvgPool2d(Functional.PixelShuffle(Functional.ReLU_(BatchNorm2d(Conv2d(x, layer + ".conv.0", ckpt, padding: 0), layer + ".conv.1", ckpt))), 2, 2, 1, 1, 1, 1, 1, 1);
        }

        ///<summary>
        /// Executes UnetBlockDeep of DeOldify with parameters from ckpt.
        ///</summary>
        ///<param name="x">Source.</param>
        ///<param name="layer">Layer name.</param>
        ///<param name="ckpt">State dict.</param>
        public static Tensor UnetBlockDeep(Tensor up_in, Tensor s, string layer, Dictionary<string, Tensor> ckpt, bool self_attentional = false)
        {
            var up_out = CustomPixelShuffle(up_in, layer + ".shuf", ckpt);
            var cat_x = Functional.ReLU_(Functional.RestrictedCat2d(up_out, BatchNorm2d(s, layer + ".bn", ckpt)));
            if(self_attentional)
            {
                return SelfAttention(BatchNorm2d(Functional.ReLU_(Conv2d(BatchNorm2d(Functional.ReLU_(Conv2d(cat_x, layer + ".conv1.0", ckpt)), layer + ".conv1.2", ckpt), layer + ".conv2.0", ckpt)), layer + ".conv2.2", ckpt), layer + ".conv2.3", ckpt);
            }
            else
            {
                return Functional.AvgPool2d(BatchNorm2d(Functional.ReLU_(Conv2d(BatchNorm2d(Functional.ReLU_(Conv2d(cat_x, layer + ".conv1.0", ckpt)), layer + ".conv1.2", ckpt), layer + ".conv2.0", ckpt)), layer + ".conv2.2", ckpt), 2, 2, 1, 1, 0, 0, 1, 1);
            }
        }

        ///<summary>
        /// Executes SelfAttention of DeOldify with parameters from ckpt.
        ///</summary>
        ///<param name="x">Source.</param>
        ///<param name="layer">Layer name.</param>
        ///<param name="ckpt">State dict.</param>
        public static Tensor SelfAttention(Tensor x, string layer, Dictionary<string, Tensor> ckpt)
        {
            var gamma = ckpt[layer + ".gamma"].Data[0];
            var f = Conv2d(x, layer + ".query", ckpt, padding: 0).Flat3d();
            var g = Conv2d(x, layer + ".key", ckpt, padding: 0).Flat3d();
            var h = Conv2d(x, layer + ".value", ckpt, padding: 0).Flat3d();
            var beta = Functional.Softmax2d(Functional.MatMul(f.Transpose2d(), g));
            return Functional.Plus_(Functional.EltwiseMulScalar_(Functional.MatMul(h, beta), gamma).Unflat3d(x.Shape[1], x.Shape[2]), x);
        }

        ///<summary>
        /// Executes PixelShuffle of DeOldify with parameters from ckpt.
        ///</summary>
        ///<param name="x">Source.</param>
        ///<param name="layer">Layer name.</param>
        ///<param name="ckpt">State dict.</param>
        public static Tensor PixelShuffle(Tensor x, string layer, Dictionary<string, Tensor> ckpt)
        {
            return Functional.AvgPool2d(Functional.ReLU_(Functional.PixelShuffle(Conv2d(x, layer + ".conv.0", ckpt, padding: 0))), 2, 2, 1, 1, 1, 1, 1, 1);
        }

        ///<summary>
        /// Executes ResBlock of DeOldify with parameters from ckpt.
        ///</summary>
        ///<param name="x">Source.</param>
        ///<param name="layer">Layer name.</param>
        ///<param name="ckpt">State dict.</param>
        public static Tensor ResBlock(Tensor x, string layer, Dictionary<string, Tensor> ckpt)
        {
            var @out = Conv2d(x, layer + ".layers.0.0", ckpt);
            @out = Functional.ReLU_(@out);
            @out = Conv2d(@out, layer + ".layers.1.0", ckpt);
            @out = Functional.ReLU_(@out);
            return Functional.Plus_(@out, x);
        }

        ///<summary>
        /// Progress value.
        ///</summary>
        private static float __Progress;

        ///<summary>
        /// Progress step.
        ///</summary>
        private const float Step = 100f / 57f;

        ///<summary>
        /// Progress bar changer.
        ///</summary>
        public delegate void ProgressDelegate(float Percent);

        ///<summary>
        /// Occurs when the coloring progress changes.
        ///</summary>
        public static event ProgressDelegate Progress;

        ///<summary>
        /// Colorizes the image.
        ///</summary>
        ///<param name="BW">Original B&W image.</param>
        public static Bitmap Colorize(Bitmap BW)
        {
            __Progress = 0f;
            var x = Image2Tensor(Resize(BW));
            // Net
            var x1 = Functional.ReLU_(
                BatchNorm2d(
                    Conv2d(x, "layers.0.0", Parameters, 2, 3), "layers.0.1", Parameters));
            var x2 = BasicBlock(
                BasicBlock(
                    BasicBlock(
                        Functional.MaxPool2d(x1, 3, 3, 2, 2, 1, 1, 1, 1), "layers.0.4.0", Parameters), "layers.0.4.1", Parameters), "layers.0.4.2", Parameters);
            var x3 = BasicBlock(
                BasicBlock(
                    BasicBlock(
                        BasicBlock(x2, "layers.0.5.0", Parameters, true), "layers.0.5.1", Parameters), "layers.0.5.2", Parameters), "layers.0.5.3", Parameters);
            var x4 = BasicBlock(
                BasicBlock(
                    BasicBlock(
                        BasicBlock(
                            BasicBlock(
                                BasicBlock(x3, "layers.0.6.0", Parameters, true), "layers.0.6.1", Parameters), "layers.0.6.2", Parameters), "layers.0.6.3", Parameters), "layers.0.6.4", Parameters), "layers.0.6.5", Parameters);
            var x5 = BasicBlock(
                BasicBlock(
                    BasicBlock(x4, "layers.0.7.0", Parameters, true), "layers.0.7.1", Parameters), "layers.0.7.2", Parameters);
            var y = Functional.ReLU_(BatchNorm2d(x5, "layers.1", Parameters));
            y = MiddleBlock(y, "layers.3", Parameters);
            y = UnetBlockDeep(y, x4, "layers.4", Parameters);
            y = UnetBlockDeep(y, x3, "layers.5", Parameters, true);
            y = UnetBlockDeep(y, x2, "layers.6", Parameters);
            y = UnetBlockDeep(y, x1, "layers.7", Parameters);
            y = PixelShuffle(y, "layers.8", Parameters);
            y = Functional.RestrictedCat2d(y, x);
            y = ResBlock(y, "layers.10", Parameters);
            y = Conv2d(y, "layers.11.0", Parameters, padding: 1);
            y = Functional.Sigmoid_(y);
            __Progress = 0f;
            return Mux(BW, Tensor2Image(y));
        }

        ///<summary>
        /// Initializes DeOldify parameters.
        ///</summary>
        public static void Initialize()
        {
            var br = new BinaryReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("DeOldify.hmodel"));
            Parameters = new Dictionary<string, Tensor>();
            Parameters.Add("layers.0.0.weight", LoadTensor(br, 64, 3, 7, 7));
            Parameters.Add("layers.0.1.weight", LoadTensor(br, 64));
            Parameters.Add("layers.0.1.bias", LoadTensor(br, 64));
            Parameters.Add("layers.0.1.running_mean", LoadTensor(br, 64));
            Parameters.Add("layers.0.1.running_var", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.0.conv1.weight", LoadTensor(br, 64, 64, 3, 3));
            Parameters.Add("layers.0.4.0.bn1.weight", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.0.bn1.bias", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.0.bn1.running_mean", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.0.bn1.running_var", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.0.conv2.weight", LoadTensor(br, 64, 64, 3, 3));
            Parameters.Add("layers.0.4.0.bn2.weight", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.0.bn2.bias", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.0.bn2.running_mean", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.0.bn2.running_var", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.1.conv1.weight", LoadTensor(br, 64, 64, 3, 3));
            Parameters.Add("layers.0.4.1.bn1.weight", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.1.bn1.bias", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.1.bn1.running_mean", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.1.bn1.running_var", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.1.conv2.weight", LoadTensor(br, 64, 64, 3, 3));
            Parameters.Add("layers.0.4.1.bn2.weight", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.1.bn2.bias", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.1.bn2.running_mean", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.1.bn2.running_var", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.2.conv1.weight", LoadTensor(br, 64, 64, 3, 3));
            Parameters.Add("layers.0.4.2.bn1.weight", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.2.bn1.bias", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.2.bn1.running_mean", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.2.bn1.running_var", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.2.conv2.weight", LoadTensor(br, 64, 64, 3, 3));
            Parameters.Add("layers.0.4.2.bn2.weight", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.2.bn2.bias", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.2.bn2.running_mean", LoadTensor(br, 64));
            Parameters.Add("layers.0.4.2.bn2.running_var", LoadTensor(br, 64));
            Parameters.Add("layers.0.5.0.conv1.weight", LoadTensor(br, 128, 64, 3, 3));
            Parameters.Add("layers.0.5.0.bn1.weight", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.0.bn1.bias", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.0.bn1.running_mean", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.0.bn1.running_var", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.0.conv2.weight", LoadTensor(br, 128, 128, 3, 3));
            Parameters.Add("layers.0.5.0.bn2.weight", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.0.bn2.bias", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.0.bn2.running_mean", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.0.bn2.running_var", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.0.downsample.0.weight", LoadTensor(br, 128, 64, 1, 1));
            Parameters.Add("layers.0.5.0.downsample.1.weight", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.0.downsample.1.bias", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.0.downsample.1.running_mean", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.0.downsample.1.running_var", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.1.conv1.weight", LoadTensor(br, 128, 128, 3, 3));
            Parameters.Add("layers.0.5.1.bn1.weight", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.1.bn1.bias", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.1.bn1.running_mean", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.1.bn1.running_var", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.1.conv2.weight", LoadTensor(br, 128, 128, 3, 3));
            Parameters.Add("layers.0.5.1.bn2.weight", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.1.bn2.bias", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.1.bn2.running_mean", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.1.bn2.running_var", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.2.conv1.weight", LoadTensor(br, 128, 128, 3, 3));
            Parameters.Add("layers.0.5.2.bn1.weight", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.2.bn1.bias", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.2.bn1.running_mean", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.2.bn1.running_var", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.2.conv2.weight", LoadTensor(br, 128, 128, 3, 3));
            Parameters.Add("layers.0.5.2.bn2.weight", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.2.bn2.bias", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.2.bn2.running_mean", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.2.bn2.running_var", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.3.conv1.weight", LoadTensor(br, 128, 128, 3, 3));
            Parameters.Add("layers.0.5.3.bn1.weight", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.3.bn1.bias", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.3.bn1.running_mean", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.3.bn1.running_var", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.3.conv2.weight", LoadTensor(br, 128, 128, 3, 3));
            Parameters.Add("layers.0.5.3.bn2.weight", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.3.bn2.bias", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.3.bn2.running_mean", LoadTensor(br, 128));
            Parameters.Add("layers.0.5.3.bn2.running_var", LoadTensor(br, 128));
            Parameters.Add("layers.0.6.0.conv1.weight", LoadTensor(br, 256, 128, 3, 3));
            Parameters.Add("layers.0.6.0.bn1.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.0.bn1.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.0.bn1.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.0.bn1.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.0.conv2.weight", LoadTensor(br, 256, 256, 3, 3));
            Parameters.Add("layers.0.6.0.bn2.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.0.bn2.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.0.bn2.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.0.bn2.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.0.downsample.0.weight", LoadTensor(br, 256, 128, 1, 1));
            Parameters.Add("layers.0.6.0.downsample.1.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.0.downsample.1.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.0.downsample.1.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.0.downsample.1.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.1.conv1.weight", LoadTensor(br, 256, 256, 3, 3));
            Parameters.Add("layers.0.6.1.bn1.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.1.bn1.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.1.bn1.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.1.bn1.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.1.conv2.weight", LoadTensor(br, 256, 256, 3, 3));
            Parameters.Add("layers.0.6.1.bn2.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.1.bn2.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.1.bn2.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.1.bn2.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.2.conv1.weight", LoadTensor(br, 256, 256, 3, 3));
            Parameters.Add("layers.0.6.2.bn1.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.2.bn1.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.2.bn1.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.2.bn1.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.2.conv2.weight", LoadTensor(br, 256, 256, 3, 3));
            Parameters.Add("layers.0.6.2.bn2.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.2.bn2.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.2.bn2.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.2.bn2.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.3.conv1.weight", LoadTensor(br, 256, 256, 3, 3));
            Parameters.Add("layers.0.6.3.bn1.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.3.bn1.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.3.bn1.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.3.bn1.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.3.conv2.weight", LoadTensor(br, 256, 256, 3, 3));
            Parameters.Add("layers.0.6.3.bn2.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.3.bn2.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.3.bn2.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.3.bn2.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.4.conv1.weight", LoadTensor(br, 256, 256, 3, 3));
            Parameters.Add("layers.0.6.4.bn1.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.4.bn1.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.4.bn1.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.4.bn1.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.4.conv2.weight", LoadTensor(br, 256, 256, 3, 3));
            Parameters.Add("layers.0.6.4.bn2.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.4.bn2.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.4.bn2.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.4.bn2.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.5.conv1.weight", LoadTensor(br, 256, 256, 3, 3));
            Parameters.Add("layers.0.6.5.bn1.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.5.bn1.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.5.bn1.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.5.bn1.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.5.conv2.weight", LoadTensor(br, 256, 256, 3, 3));
            Parameters.Add("layers.0.6.5.bn2.weight", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.5.bn2.bias", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.5.bn2.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.0.6.5.bn2.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.0.7.0.conv1.weight", LoadTensor(br, 512, 256, 3, 3));
            Parameters.Add("layers.0.7.0.bn1.weight", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.0.bn1.bias", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.0.bn1.running_mean", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.0.bn1.running_var", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.0.conv2.weight", LoadTensor(br, 512, 512, 3, 3));
            Parameters.Add("layers.0.7.0.bn2.weight", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.0.bn2.bias", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.0.bn2.running_mean", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.0.bn2.running_var", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.0.downsample.0.weight", LoadTensor(br, 512, 256, 1, 1));
            Parameters.Add("layers.0.7.0.downsample.1.weight", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.0.downsample.1.bias", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.0.downsample.1.running_mean", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.0.downsample.1.running_var", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.1.conv1.weight", LoadTensor(br, 512, 512, 3, 3));
            Parameters.Add("layers.0.7.1.bn1.weight", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.1.bn1.bias", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.1.bn1.running_mean", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.1.bn1.running_var", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.1.conv2.weight", LoadTensor(br, 512, 512, 3, 3));
            Parameters.Add("layers.0.7.1.bn2.weight", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.1.bn2.bias", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.1.bn2.running_mean", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.1.bn2.running_var", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.2.conv1.weight", LoadTensor(br, 512, 512, 3, 3));
            Parameters.Add("layers.0.7.2.bn1.weight", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.2.bn1.bias", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.2.bn1.running_mean", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.2.bn1.running_var", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.2.conv2.weight", LoadTensor(br, 512, 512, 3, 3));
            Parameters.Add("layers.0.7.2.bn2.weight", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.2.bn2.bias", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.2.bn2.running_mean", LoadTensor(br, 512));
            Parameters.Add("layers.0.7.2.bn2.running_var", LoadTensor(br, 512));
            Parameters.Add("layers.1.weight", LoadTensor(br, 512));
            Parameters.Add("layers.1.bias", LoadTensor(br, 512));
            Parameters.Add("layers.1.running_mean", LoadTensor(br, 512));
            Parameters.Add("layers.1.running_var", LoadTensor(br, 512));
            Parameters.Add("layers.3.0.0.weight", LoadTensor(br, 1024, 512, 3, 3));
            Parameters.Add("layers.3.0.2.weight", LoadTensor(br, 1024));
            Parameters.Add("layers.3.0.2.bias", LoadTensor(br, 1024));
            Parameters.Add("layers.3.0.2.running_mean", LoadTensor(br, 1024));
            Parameters.Add("layers.3.0.2.running_var", LoadTensor(br, 1024));
            Parameters.Add("layers.3.1.0.weight", LoadTensor(br, 512, 1024, 3, 3));
            Parameters.Add("layers.3.1.2.weight", LoadTensor(br, 512));
            Parameters.Add("layers.3.1.2.bias", LoadTensor(br, 512));
            Parameters.Add("layers.3.1.2.running_mean", LoadTensor(br, 512));
            Parameters.Add("layers.3.1.2.running_var", LoadTensor(br, 512));
            Parameters.Add("layers.4.shuf.conv.0.weight", LoadTensor(br, 1024, 512, 1, 1));
            Parameters.Add("layers.4.shuf.conv.1.weight", LoadTensor(br, 1024));
            Parameters.Add("layers.4.shuf.conv.1.bias", LoadTensor(br, 1024));
            Parameters.Add("layers.4.shuf.conv.1.running_mean", LoadTensor(br, 1024));
            Parameters.Add("layers.4.shuf.conv.1.running_var", LoadTensor(br, 1024));
            Parameters.Add("layers.4.bn.weight", LoadTensor(br, 256));
            Parameters.Add("layers.4.bn.bias", LoadTensor(br, 256));
            Parameters.Add("layers.4.bn.running_mean", LoadTensor(br, 256));
            Parameters.Add("layers.4.bn.running_var", LoadTensor(br, 256));
            Parameters.Add("layers.4.conv1.0.weight", LoadTensor(br, 768, 512, 3, 3));
            Parameters.Add("layers.4.conv1.2.weight", LoadTensor(br, 768));
            Parameters.Add("layers.4.conv1.2.bias", LoadTensor(br, 768));
            Parameters.Add("layers.4.conv1.2.running_mean", LoadTensor(br, 768));
            Parameters.Add("layers.4.conv1.2.running_var", LoadTensor(br, 768));
            Parameters.Add("layers.4.conv2.0.weight", LoadTensor(br, 768, 768, 3, 3));
            Parameters.Add("layers.4.conv2.2.weight", LoadTensor(br, 768));
            Parameters.Add("layers.4.conv2.2.bias", LoadTensor(br, 768));
            Parameters.Add("layers.4.conv2.2.running_mean", LoadTensor(br, 768));
            Parameters.Add("layers.4.conv2.2.running_var", LoadTensor(br, 768));
            Parameters.Add("layers.5.shuf.conv.0.weight", LoadTensor(br, 1536, 768, 1, 1));
            Parameters.Add("layers.5.shuf.conv.1.weight", LoadTensor(br, 1536));
            Parameters.Add("layers.5.shuf.conv.1.bias", LoadTensor(br, 1536));
            Parameters.Add("layers.5.shuf.conv.1.running_mean", LoadTensor(br, 1536));
            Parameters.Add("layers.5.shuf.conv.1.running_var", LoadTensor(br, 1536));
            Parameters.Add("layers.5.bn.weight", LoadTensor(br, 128));
            Parameters.Add("layers.5.bn.bias", LoadTensor(br, 128));
            Parameters.Add("layers.5.bn.running_mean", LoadTensor(br, 128));
            Parameters.Add("layers.5.bn.running_var", LoadTensor(br, 128));
            Parameters.Add("layers.5.conv1.0.weight", LoadTensor(br, 768, 512, 3, 3));
            Parameters.Add("layers.5.conv1.2.weight", LoadTensor(br, 768));
            Parameters.Add("layers.5.conv1.2.bias", LoadTensor(br, 768));
            Parameters.Add("layers.5.conv1.2.running_mean", LoadTensor(br, 768));
            Parameters.Add("layers.5.conv1.2.running_var", LoadTensor(br, 768));
            Parameters.Add("layers.5.conv2.0.weight", LoadTensor(br, 768, 768, 3, 3));
            Parameters.Add("layers.5.conv2.2.weight", LoadTensor(br, 768));
            Parameters.Add("layers.5.conv2.2.bias", LoadTensor(br, 768));
            Parameters.Add("layers.5.conv2.2.running_mean", LoadTensor(br, 768));
            Parameters.Add("layers.5.conv2.2.running_var", LoadTensor(br, 768));
            Parameters.Add("layers.5.conv2.3.gamma", LoadTensor(br, 1));
            Parameters.Add("layers.5.conv2.3.query.weight", LoadTensor(br, 96, 768, 1, 1));
            Parameters.Add("layers.5.conv2.3.key.weight", LoadTensor(br, 96, 768, 1, 1));
            Parameters.Add("layers.5.conv2.3.value.weight", LoadTensor(br, 768, 768, 1, 1));
            Parameters.Add("layers.6.shuf.conv.0.weight", LoadTensor(br, 1536, 768, 1, 1));
            Parameters.Add("layers.6.shuf.conv.1.weight", LoadTensor(br, 1536));
            Parameters.Add("layers.6.shuf.conv.1.bias", LoadTensor(br, 1536));
            Parameters.Add("layers.6.shuf.conv.1.running_mean", LoadTensor(br, 1536));
            Parameters.Add("layers.6.shuf.conv.1.running_var", LoadTensor(br, 1536));
            Parameters.Add("layers.6.bn.weight", LoadTensor(br, 64));
            Parameters.Add("layers.6.bn.bias", LoadTensor(br, 64));
            Parameters.Add("layers.6.bn.running_mean", LoadTensor(br, 64));
            Parameters.Add("layers.6.bn.running_var", LoadTensor(br, 64));
            Parameters.Add("layers.6.conv1.0.weight", LoadTensor(br, 672, 448, 3, 3));
            Parameters.Add("layers.6.conv1.2.weight", LoadTensor(br, 672));
            Parameters.Add("layers.6.conv1.2.bias", LoadTensor(br, 672));
            Parameters.Add("layers.6.conv1.2.running_mean", LoadTensor(br, 672));
            Parameters.Add("layers.6.conv1.2.running_var", LoadTensor(br, 672));
            Parameters.Add("layers.6.conv2.0.weight", LoadTensor(br, 672, 672, 3, 3));
            Parameters.Add("layers.6.conv2.2.weight", LoadTensor(br, 672));
            Parameters.Add("layers.6.conv2.2.bias", LoadTensor(br, 672));
            Parameters.Add("layers.6.conv2.2.running_mean", LoadTensor(br, 672));
            Parameters.Add("layers.6.conv2.2.running_var", LoadTensor(br, 672));
            Parameters.Add("layers.7.shuf.conv.0.weight", LoadTensor(br, 1344, 672, 1, 1));
            Parameters.Add("layers.7.shuf.conv.1.weight", LoadTensor(br, 1344));
            Parameters.Add("layers.7.shuf.conv.1.bias", LoadTensor(br, 1344));
            Parameters.Add("layers.7.shuf.conv.1.running_mean", LoadTensor(br, 1344));
            Parameters.Add("layers.7.shuf.conv.1.running_var", LoadTensor(br, 1344));
            Parameters.Add("layers.7.bn.weight", LoadTensor(br, 64));
            Parameters.Add("layers.7.bn.bias", LoadTensor(br, 64));
            Parameters.Add("layers.7.bn.running_mean", LoadTensor(br, 64));
            Parameters.Add("layers.7.bn.running_var", LoadTensor(br, 64));
            Parameters.Add("layers.7.conv1.0.weight", LoadTensor(br, 300, 400, 3, 3));
            Parameters.Add("layers.7.conv1.2.weight", LoadTensor(br, 300));
            Parameters.Add("layers.7.conv1.2.bias", LoadTensor(br, 300));
            Parameters.Add("layers.7.conv1.2.running_mean", LoadTensor(br, 300));
            Parameters.Add("layers.7.conv1.2.running_var", LoadTensor(br, 300));
            Parameters.Add("layers.7.conv2.0.weight", LoadTensor(br, 300, 300, 3, 3));
            Parameters.Add("layers.7.conv2.2.weight", LoadTensor(br, 300));
            Parameters.Add("layers.7.conv2.2.bias", LoadTensor(br, 300));
            Parameters.Add("layers.7.conv2.2.running_mean", LoadTensor(br, 300));
            Parameters.Add("layers.7.conv2.2.running_var", LoadTensor(br, 300));
            Parameters.Add("layers.8.conv.0.bias", LoadTensor(br, 1200));
            Parameters.Add("layers.8.conv.0.weight", LoadTensor(br, 1200, 300, 1, 1));
            Parameters.Add("layers.10.layers.0.0.bias", LoadTensor(br, 303));
            Parameters.Add("layers.10.layers.0.0.weight", LoadTensor(br, 303, 303, 3, 3));
            Parameters.Add("layers.10.layers.1.0.bias", LoadTensor(br, 303));
            Parameters.Add("layers.10.layers.1.0.weight", LoadTensor(br, 303, 303, 3, 3));
            Parameters.Add("layers.11.0.bias", LoadTensor(br, 3));
            Parameters.Add("layers.11.0.weight", LoadTensor(br, 3, 303, 1, 1));
        }

    }

}
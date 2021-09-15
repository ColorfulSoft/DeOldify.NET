//*************************************************************************************************
//* (C) ColorfulSoft corp., 2021. All Rights reserved.
//*************************************************************************************************

using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ColorfulSoft.DeOldify
{

    internal static unsafe class Functional
    {

        public static Tensor AvgPool2d(Tensor x,
                                       int kernelH,
                                       int kernelW,
                                       int strideY,
                                       int strideX,
                                       int paddingY,
                                       int paddingX,
                                       int dilationY,
                                       int dilationX)
        {
            int x_width = x.Shape[2];
            int x_height = x.Shape[1];
            int x_channel = x.Shape[0];
            int y_width = (x_width + 2 * paddingX - dilationX * (kernelW - 1) - 1) / strideX + 1;
            int y_height = (x_height + 2 * paddingY - dilationY * (kernelH - 1) - 1) / strideY + 1;
            int y_channel = x_channel;
            var y = new Tensor(y_channel, y_height, y_width);
            var px = x.Data;
            var py = y.Data;
            var winsize = (float)(kernelW * kernelH);
            Parallel.For(0, x_channel, (int c) =>
            {
                for(int ox = 0; ox < y_width; ++ox)
                {
                    var ix_ = ox * strideX - paddingX;
                    for(int oy = 0; oy < y_height; ++oy)
                    {
                        var iy_ = oy * strideY - paddingY;
                        var mean = 0f;
                        for(int fx = 0; fx < kernelW; ++fx)
                        {
                            var ix = ix_ + fx * dilationX;
                            if((ix >= x_width) || (ix < 0))
                            {
                                continue;
                            }
                            for(int fy = 0; fy < kernelH; ++fy)
                            {
                                var iy = iy_ + fy * dilationY;
                                if((iy >= x_height) || (iy < 0))
                                {
                                    continue;
                                }
                                mean += px[(c * x_height + iy) * x_width + ix];
                            }
                        }
                        py[(c * y_height + oy) * y_width + ox] = mean / winsize;
                    }
                }
            });
            return y;
        }

        public static Tensor BatchNorm2d_(Tensor x, Tensor mean, Tensor @var, Tensor weight, Tensor bias)
        {
            var hw = x.Shape[1] * x.Shape[2];
            float* pmean = mean.Data;
            float* pvar = @var.Data;
            float* pw = weight.Data;
            float* pb = bias.Data;
            for(float* px_ = x.Data; px_ < (x.Data + x.Numel); px_ += hw, ++pmean, ++pvar, ++pw, ++pb)
            {
                for(float* px = px_; px < (px_ + hw); ++px)
                {
                    *px = (*px - *pmean) / *pvar * *pw + *pb;
                }
            }
            return x;
        }

        // Based on this: https://habr.com/ru/post/448436/
        public static void im2col(float* src,
                                  int srcC,
                                  int srcH,
                                  int srcW,
                                  int kernelY,
                                  int kernelX,
                                  int dilationY,
                                  int dilationX,
                                  int strideY,
                                  int strideX,
                                  int padY,
                                  int padX,
                                  int padH,
                                  int padW,
                                  float* buf)
        {
            int dstH = (srcH + padY + padH - (dilationY * (kernelY - 1) + 1)) / strideY + 1;
            int dstW = (srcW + padX + padW - (dilationX * (kernelX - 1) + 1)) / strideX + 1;
            for(int sc = 0; sc < srcC; ++sc)
            {
                for(int ky = 0; ky < kernelY; ky++)
                {
                    for(int kx = 0; kx < kernelX; kx++)
                    {
                        for(int dy = 0; dy < dstH; ++dy)
                        {
                            int sy = dy * strideY + ky * dilationY - padY;
                            if((sy < 0) || (sy >= srcH))
                            {
                                for(int dx = 0; dx < dstW; ++dx)
                                {
                                    *buf++ = 0;
                                }
                                continue;
                            }
                            for(int dx = 0; dx < dstW; ++dx)
                            {
                                int sx = dx * strideX + kx * dilationX - padX;
                                if((sx >= 0) && (sx < srcW))
                                {
                                    *buf++ = src[(sc * srcH + sy) * srcW + sx];
                                }
                                else
                                {
                                    *buf++ = 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Based on this: https://habr.com/ru/post/448436/
        public static void gemm_nn(int M,
                                   int N,
                                   int K,
                                   float* A,
                                   int lda,
                                   float* B,
                                   int ldb,
                                   float* C,
                                   int ldc)
        {
            var Bt = (float*)Marshal.AllocHGlobal(K * N * sizeof(float)).ToPointer();
        		for(int j = 0; j < N; ++j)
            {
                for(int k = 0; k < K; ++k)
                {
                    Bt[j * K + k] = B[k * N + j];
                }
        		}
            Parallel.For(0, M, (int i) =>
            {
                for(int j = 0; j < N; ++j)
                {
                    var sum = 0f;
                    for(int k = 0; k < K; ++k)
                    {
                        sum += A[i * lda + k] * Bt[j * K + k];
                    }
                    C[i * ldc + j] = sum;
                }
            });
            Marshal.FreeHGlobal((IntPtr)Bt);
        }

        // Based on this: https://habr.com/ru/post/448436/
        public static Tensor Conv2d(Tensor x,
                                    Tensor weight,
                                    Tensor bias,
                                    int padY,
                                    int padX,
                                    int padH,
                                    int padW,
                                    int strideY,
                                    int strideX,
                                    int dilationY,
                                    int dilationX,
                                    int group)
        {
            int srcC = x.Shape[0];
            int srcH = x.Shape[1];
            int srcW = x.Shape[2];
            int kernelY = weight.Shape[2];
            int kernelX = weight.Shape[3];
            int dstC = weight.Shape[0];
            int dstH = (srcH + padY + padH - (dilationY * (kernelY - 1) + 1)) / strideY + 1;
            int dstW = (srcW + padX + padW - (dilationX * (kernelX - 1) + 1)) / strideX + 1;
            var y = new Tensor(dstC, dstH, dstW);
            var buf = (float*)Marshal.AllocHGlobal(srcC * dstH * dstW * kernelY * kernelX * sizeof(float)).ToPointer();
            // Pointers
            var pdst = y.Data;
            var pweight = weight.Data;
            var psrc = x.Data;
            int M = dstC / group;
            int N = dstH * dstW;
            int K = srcC * kernelY * kernelX / group;
            im2col(psrc, srcC, srcH, srcW, kernelY, kernelX, dilationY, dilationX, strideY, strideX, padY, padX, padH, padW, buf);
            for(int g = 0; g < group; ++g)
            {
                gemm_nn(M, N, K, pweight + M * K * g, K, buf + N * K * g, N, pdst + M * N * g, N);
            }
            if(bias != null)
            {
                var pbias = bias.Data;
                for(int i = 0; i < dstC; ++i)
                {
                    for(int j = 0; j < N; ++j)
                    {
                        pdst[i * N + j] += pbias[i];
                    }
                }
            }
            Marshal.FreeHGlobal((IntPtr)buf);
            return y;
        }

        public static Tensor EltwiseMulScalar_(Tensor x, float s)
        {
            var px = x.Data;
            for(int i = 0; i < x.Numel; ++i)
            {
                px[i] *= s;
            }
            return x;
        }

        public static Tensor MatMul(Tensor a, Tensor b)
        {
            var aw = a.Shape[1];
            var ah = a.Shape[0];
            var bw = b.Shape[1];
            var bh = b.Shape[0];
            b = b.Transpose2d();
            var c = new Tensor(ah, bw);
            var pa = a.Data;
            var pb = b.Data;
            var pc = c.Data;
            Parallel.For(0, ah, (int i) =>
            {
                var pa_ = pa + aw * i;
                var pc_ = pc + i * bw;
                for(int j = 0; j < bw; j++)
                {
                    var pb_ = pb + bh * j;
                    var v = 0f;
                    for(int k = 0; k < bh; k++)
                    {
                        v += pa_[k] * pb_[k];
                    }
                    pc_[j] = v;
                }
            });
            return c;
        }

        public static Tensor MaxPool2d(Tensor x,
                                       int kernelH,
                                       int kernelW,
                                       int strideY,
                                       int strideX,
                                       int paddingY,
                                       int paddingX,
                                       int dilationY,
                                       int dilationX)
        {
            int x_width = x.Shape[2];
            int x_height = x.Shape[1];
            int x_channel = x.Shape[0];
            int y_width = (x_width + 2 * paddingX - dilationX * (kernelW - 1) - 1) / strideX + 1;
            int y_height = (x_height + 2 * paddingY - dilationY * (kernelH - 1) - 1) / strideY + 1;
            int y_channel = x_channel;
            var y = new Tensor(y_channel, y_height, y_width);
            var px = x.Data;
            var py = y.Data;
            Parallel.For(0, x_channel, (int c) =>
            {
                for(int ox = 0; ox < y_width; ++ox)
                {
                    var ix_ = ox * strideX - paddingX;
                    for(int oy = 0; oy < y_height; ++oy)
                    {
                        var iy_ = oy * strideY - paddingY;
                        var max = float.MinValue;
                        for(int fx = 0; fx < kernelW; ++fx)
                        {
                            var ix = ix_ + fx * dilationX;
                            if((ix >= x_width) || (ix < 0))
                            {
                                continue;
                            }
                            for(int fy = 0; fy < kernelH; ++fy)
                            {
                                var iy = iy_ + fy * dilationY;
                                if((iy >= x_height) || (iy < 0))
                                {
                                    continue;
                                }
                                var v = px[(c * x_height + iy) * x_width + ix];
                                if(v > max)
                                {
                                    max = v;
                                }
                            }
                        }
                        py[(c * y_height + oy) * y_width + ox] = max;
                    }
                }
            });
            return y;
        }

        public static Tensor PixelShuffle(Tensor x)
        {
            var x_depth = x.Shape[0];
            var x_height = x.Shape[1];
            var x_width = x.Shape[2];
            var y = new Tensor(x.Shape[0] / 4, x.Shape[1] * 2, x.Shape[2] * 2);
            var y_depth = y.Shape[0];
            var y_height = y.Shape[1];
            var y_width = y.Shape[2];
            var py = y.Data;
            var px = x.Data;
            for(int od = 0; od < y_depth; ++od)
            {
                var id = od * 4;
                for(int oy = 0; oy < y_height; oy += 2)
                {
                    var iy = oy / 2;
                    for(int ox = 0; ox < y_width; ox += 2)
                    {
                        var ix = ox / 2;
                        py[(od * y_height + oy) * y_width + ox] = px[(id * x_height + iy) * x_width + ix];
                        py[(od * y_height + oy) * y_width + ox + 1] = px[((id + 1) * x_height + iy) * x_width + ix];
                        py[(od * y_height + oy + 1) * y_width + ox] = px[((id + 2) * x_height + iy) * x_width + ix];
                        py[(od * y_height + oy + 1) * y_width + ox + 1] = px[((id + 3) * x_height + iy) * x_width + ix];
                    }
                }
            }
            return y;
        }

        public static Tensor Plus_(Tensor a, Tensor b)
        {
            var pa = a.Data;
            for(float* pb = b.Data; pb < (b.Data + b.Numel); ++pa, ++pb)
            {
                *pa += *pb;
            }
            return a;
        }

        public static Tensor ReLU_(Tensor x)
        {
            for(float* px = x.Data; px < (x.Data + x.Numel); ++px)
            {
                if(*px < 0)
                {
                    *px = 0;
                }
            }
            return x;
        }

        public static Tensor RestrictedCat2d(Tensor a, Tensor b)
        {
            var height = Math.Min(a.Shape[1], b.Shape[1]);
            var width = Math.Min(a.Shape[2], b.Shape[2]);
            var adepth = a.Shape[0];
            var bdepth = b.Shape[0];
            var aheight = a.Shape[1];
            var bheight = b.Shape[1];
            var awidth = a.Shape[2];
            var bwidth = b.Shape[2];
            var depth = adepth + bdepth;
            var c = new Tensor(depth, height, width);
            var pa = a.Data;
            var pb = b.Data;
            var pc = c.Data;
            for(int y = 0; y < height; ++y)
            {
                for(int x = 0; x < width; ++x)
                {
                    int gd = 0;
                    for(int d = 0; d < adepth; ++d, ++gd)
                    {
                        pc[(gd * height + y) * width + x] = pa[(d * aheight + y) * awidth + x];
                    }
                    for(int d = 0; d < bdepth; ++d, ++gd)
                    {
                        pc[(gd * height + y) * width + x] = pb[(d * bheight + y) * bwidth + x];
                    }
                }
            }
            return c;
        }

        public static Tensor Sigmoid_(Tensor x)
        {
            for(float* px = x.Data; px < x.Data + x.Numel; ++px)
            {
                *px = 1f / (1f + (float)Math.Exp(-*px));
            }
            return x;
        }

        public static Tensor Softmax2d(Tensor input)
        {
            var Result = new Tensor(input.Shape[0], input.Shape[1]);
            var px = input.Data;
            var py = Result.Data;
            var height = input.Shape[0];
            var width = input.Shape[1];
            for(int y = 0; y < height; ++y)
            {
                var amax = float.MinValue;
                for(int x = 0; x < width; ++x)
                {
                    var v = px[y * width + x];
                    if(amax < v)
                    {
                        amax = v;
                    }
                }
                var sum = 0f;
                for(int x = 0; x < width; ++x)
                {
                    var v = px[y * width + x];
                    sum += (float)Math.Exp(v - amax);
                }
                for(int x = 0; x < width; ++x)
                {
                    var v = px[y * width + x];
                    py[y * width + x] = (float)(Math.Exp(v - amax) / sum);
                }
            }
            return Result;
        }

    }

}

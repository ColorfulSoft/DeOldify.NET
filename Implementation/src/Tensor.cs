//*************************************************************************************************
//* (C) ColorfulSoft corp., 2021. All Rights reserved.
//*************************************************************************************************

using System;
using System.Runtime.InteropServices;

namespace ColorfulSoft.DeOldify
{

    internal sealed unsafe class Tensor : IDisposable
    {

        public float* Data;

        private bool __DisposeData = true;

        public int* Shape;

        public int Numel;

        public int Ndim;

        public Tensor()
        {
        }

        public Tensor(params int[] Shape)
        {
            this.Ndim = Shape.Length;
            this.Numel = 1;
            this.Shape = (int*)Marshal.AllocHGlobal(sizeof(int) * this.Ndim).ToPointer();
            var Pshape = this.Shape;
            foreach(var Dim in Shape)
            {
                this.Numel *= Dim;
                *Pshape++ = Dim;
            }
            this.Data = (float*)Marshal.AllocHGlobal(sizeof(float) * this.Numel).ToPointer();
        }

        void IDisposable.Dispose()
        {
            if((this.Data != null) && this.__DisposeData)
            {
                Marshal.FreeHGlobal((IntPtr)this.Data);
                this.Data = null;
            }
            if(this.Shape != null)
            {
                Marshal.FreeHGlobal((IntPtr)this.Shape);
                this.Shape = null;
            }
        }

        public Tensor Flat3d()
        {
            var t = new Tensor();
            t.Data = this.Data;
            t.Ndim = 2;
            t.Numel = this.Numel;
            t.Shape = (int*)Marshal.AllocHGlobal(sizeof(int) * 2).ToPointer();
            t.Shape[0] = this.Shape[0];
            t.Shape[1] = this.Shape[1] * this.Shape[2];
            t.__DisposeData = false;
            return t;
        }

        public Tensor Unflat3d(int h, int w)
        {
            var t = new Tensor();
            t.Data = this.Data;
            t.Ndim = 3;
            t.Numel = this.Numel;
            t.Shape = (int*)Marshal.AllocHGlobal(sizeof(int) * 3).ToPointer();
            t.Shape[0] = this.Shape[0];
            t.Shape[1] = h;
            t.Shape[2] = w;
            t.__DisposeData = false;
            return t;
        }

        public Tensor Transpose2d()
        {
            var t = new Tensor(this.Shape[1], this.Shape[0]);
            var px = this.Data;
            var py = t.Data;
            var width = this.Shape[1];
            var height = this.Shape[0];
            var n = 0;
            for(int i = 0; i < height; ++i)
            {
                for(int j = 0; j < width; ++j)
                {
                    py[j * height + i] = px[i * width + j];
                    ++n;
                }
            }
            return t;
        }

    }

}
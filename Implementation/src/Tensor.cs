//*************************************************************************************************
//* (C) ColorfulSoft corp., 2021. All Rights reserved.
//*************************************************************************************************

using System;
using System.Runtime.InteropServices;

namespace ColorfulSoft.DeOldify
{

    /// <summary>
    /// Multidimentional array of floating point data type.
    /// </summary>
    internal sealed unsafe class Tensor : IDisposable
    {

        /// <summary>
        /// Data.
        /// </summary>
        public float* Data;

        /// <summary>
        /// Should destructor free Data?
        /// </summary>
        private bool __DisposeData = true;

        /// <summary>
        /// Shape.
        /// </summary>
        public int* Shape;

        /// <summary>
        /// Number of elements.
        /// </summary>
        public int Numel;

        /// <summary>
        /// Number of dimentions.
        /// </summary>
        public int Ndim;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Tensor()
        {
        }

        /// <summary>
        /// Initializes the tensor with specified shape.
        /// </summary>
        /// <param name="shape">Shape.</param>
        public Tensor(params int[] shape)
        {
            this.Ndim = shape.Length;
            this.Numel = 1;
            this.Shape = (int*)Marshal.AllocHGlobal(sizeof(int) * this.Ndim).ToPointer();
            var Pshape = this.Shape;
            foreach(var Dim in shape)
            {
                this.Numel *= Dim;
                *Pshape++ = Dim;
            }
            this.Data = (float*)Marshal.AllocHGlobal(sizeof(float) * this.Numel).ToPointer();
        }

        /// <summary>
        /// Disposes unmanaged resources of the tensor.
        /// </summary>
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

        /// <summary>
        /// Disposes unmanaged resources of the tensor.
        /// </summary>
        ~Tensor()
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

        /// <summary>
        /// Flattens 3d tensor to 2d.
        /// </summary>
        /// <returns>Tensor.</returns>
        public Tensor Flat3d()
        {
            var t = new Tensor();
            t.Data = this.Data;
            t.Ndim = 2;
            t.Numel = this.Numel;
            t.Shape = (int*)Marshal.AllocHGlobal(sizeof(int) * 2).ToPointer();
            t.Shape[0] = this.Shape[0];
            t.Shape[1] = this.Shape[1] * this.Shape[2];
            this.__DisposeData = false;
            return t;
        }

        /// <summary>
        /// Unflats the 2d tensor to 3d using specified size.
        /// </summary>
        /// <param name="h">Height.</param>
        /// <param name="w">Width.</param>
        /// <returns>Tensor.</returns>
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
            this.__DisposeData = false;
            return t;
        }

        /// <summary>
        /// Returns transposed version of this tensor.
        /// </summary>
        /// <returns>Tensor.</returns>
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

# DeOldify.NET
*C# implementation of Jason Antic's DeOldify(https://github.com/jantic/DeOldify)* **Only for photos for now!**

Paper "__DeOldify.NET: cross-platform application for coloring black and white photos__" was accepted to poster session of _Neuroinformatics - 2022_ conference. The paper describes technical details of managed C# implementation of the original DeOldify and contains some comparisons with different other image colorization products.

# How to run

## On Windows 7, 8, 8.1, 10, 11
* Make sure that .NET Framework 4.5+ (4.6+ for SIMD-accelerated version) or higher is installed on your computer.

* You can use any bit depth (x32 or x64), but on a 32-bit system you will not be able to process large images due to the limited amount of memory.

* **SIMD and Stable model are supported only in 64-bit mode. On a 32-bit machine, you should use the regular artistic version.**

* At least ~~3 GB~~ **1.5 GB with new convolution algorithm** of free RAM is required to run Artistic model. About 3 GB is required for Stable model.

* Select a version of DeOldify.NET the one you want to build. Versions with and without simd are available, with float32 typed weights (higher accuracy) and float16 typed weights (lower accuracy and smaller file size), with a stable model and an artistic model, as in the original DeOldify. Further actions depend on your choice.

* Download and unpack the repository, then download model from the releases (https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights) and place it in Implementation\src\Resources.

|Model|Details|File|
|:---:|:-----:|:--:|
|float32 Artistic|Artistic model with single-precision floating point weights. More accurate than compressed float16 model.|https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/Artistic.model|
|float16 Artistic|Artistic model with half-precision floating point weights. Less accurate than original float32 model, but requires 2 times less disk space.|https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/Artistic.hmodel|
|float32 Stable|Stable model with single-precision floating point weights. More accurate than compressed float16 model.|https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/Stable.model|
|float16 Stable|Stable model with single-precision floating point weights. Less accurate than original float32 model, but requires 2 times less disk space.|https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/Stable.hmodel|

* Select compilation script and run it.

|Build|Details|Script|
|:---:|:-----:|:----:|
|Artistic|Basic version of Artistic colorizer with float16 weights|Compile.artistic.bat|
|Artistic.w32|Artistic colorizer with float32 weights|Compile.artistic.float.bat|
|Artistic.simd|Artistic colorizer with SIMD acceleration and float16 weights|Compile.artistic.simd.bat|
|Artistic.simd.w32|Artistic colorizer with SIMD acceleration and float32 weights|Compile.artistic.simd.float.bat|
|Stable|Basic version of Stable colorizer with float16 weights|Compile.stable.bat|
|Stable.w32|Stable colorizer with float32 weights|Compile.stable.float.bat|
|Stable.simd|Stable colorizer with SIMD acceleration and float16 weights|Compile.stable.simd.bat|
|Stable.simd.w32|Stable colorizer with SIMD acceleration and float32 weights|Compile.stable.simd.float.bat|

* The executable file will appear in the `Implementation\Release` folder. The application is ready to work!

* **Use!**

![Windows GUI](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Sample.jpg)

## On Linux (Tested on Mint)
* We recommend that the first step is to update everything. It may take time, but it's worth it:
```
sudo apt-get update
sudo apt-get upgrade
```
* Install Mono:
```
sudo apt-get install mono-complete
```

* Get sources; select and download model

|Model|Details|File|
|:---:|:-----:|:--:|
|float32 Artistic|Artistic model with single-precision floating point weights. More accurate than compressed float16 model.|https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/Artistic.model|
|float16 Artistic|Artistic model with half-precision floating point weights. Less accurate than original float32 model, but requires 2 times less disk space.|https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/Artistic.hmodel|
|float32 Stable|Stable model with single-precision floating point weights. More accurate than compressed float16 model.|https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/Stable.model|
|float16 Stable|Stable model with single-precision floating point weights. Less accurate than original float32 model, but requires 2 times less disk space.|https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/Stable.hmodel|

<details>
<summary>Using git and terminal</summary>

```
git clone https://github.com/ColorfulSoft/DeOldify.NET.git
cd DeOldify.NET
wget <model url> -O Implementation/src/Resources/<model name>
```

</details>
<details>
<summary>Using GUI</summary>

* Download and unpack the repository.
  
* Download model from the releases(https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/) and place it in `Implementation/src/Resources`.
</details>

* Select and run compilation script

|Build|Details|Script|
|:---:|:-----:|:----:|
|Artistic|Basic version of Artistic colorizer with float16 weights|Compile.artistic.sh|
|Artistic.w32|Artistic colorizer with float32 weights|Compile.artistic.float.sh|
|Artistic.simd|Artistic colorizer with SIMD acceleration and float16 weights|Compile.artistic.simd.sh|
|Artistic.simd.w32|Artistic colorizer with SIMD acceleration and float32 weights|Compile.artistic.simd.float.sh|
|Stable|Basic version of Stable colorizer with float16 weights|Compile.stable.sh|
|Stable.w32|Stable colorizer with float32 weights|Compile.stable.float.sh|
|Stable.simd|Stable colorizer with SIMD acceleration and float16 weights|Compile.stable.simd.sh|
|Stable.simd.w32|Stable colorizer with SIMD acceleration and float32 weights|Compile.stable.simd.float.sh|

* The executable file will appear in the `Implementation/Release` folder. The application is ready to work!

* Run application using `mono <build name>.exe` command in terminal or double click as in Windows.

* **Use!**

![Linux GUI](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Sample.Linux.jpg)

**_Please note, that DeOldify.NET using Mono is a bit slower, than using .NET Framework_**

# Examples

![Example1](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Example1.jpg)

![Example2](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Example2.jpg)

|Original|Artistic|Stable|
|:------:|:------:|:----:|
|![Original](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Original.jpg)|![Artistic](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Artistic.jpg)|![Stable](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Stable.jpg)|

# New algorithms

DeOldify.NET has become a platform for testing the latest highly optimized algorithms, which will then be applied in the System.AI project. In this section you can find some information about the results of the experiments.

## Patch2Vec Conv2d

> The meaning of most fast convolution algorithms, such as im2col or im2row, involves bringing the convolution to matrix multiplication, which allows optimizing memory access operations by using the processor cache. However, such methods either require a buffer for `srcC * kernelY * kernelH * dstH * dstW` elements, which is extremely irrational. The proposed **patch2vec** method unwraps each patch of the input image on the fly, and then applies all convolution filters to it. This implementation is not inferior in efficiency to classical algorithms like im2col, and in practice even surpasses them. The buffer for this algorithm will have the size of `srcC * kernelY * kernelX`, which is much smaller than in the case of similar methods. Moreover, patch2vec does not impose restrictions on the convolution parameters, unlike, for example, the Shmuel Vinograd method. The proposed algorithm is difficult to fit into classical machine learning frameworks due to the fact that they are focused on using GEMM as the core. Pure C#-based implementations make it easy to do this.

__For more detailed information, please see official Patch2Vec repository: https://github.com/GlebSBrykin/Patch2Vec__

In DeOldify.NET two versions of the patch2vec conv2d algorithm are implemented - with and without SIMD support. You can choose, which version to use by executing the corresponding compilation command file. Vectorization is implemented through the `Vector4` structure of the `System.Numerics` namespace. Vectorization is only available for x86-64 processors at version .NET Framework 4.6 and higher, or when using Mono newer than 2008.

|Method               |Time (ms)|
|:-------------------:|:-------:|
|im2col               |123902   |
|patch2vec            |114970   |
|**patch2vec + simd** |**33270**|

___All tests was done in Windows 7 x64 laptop with Intel(R) Core(TM) i5-6300HQ CPU and 32 GB of RAM___

# Updates

* [10.09.2022] - **Version 2.1** has been released. The ability to drag and drop images into the application has been added, error handling has been improved, and the process of saving the result has been simplified.
* [20.07.2022] - The Windows and Linux versions are combined.
* **[11.07.2022] - DeOldify.NET now supports Stable model from original DeOldify. Added the ability to build DeOldify.NET with uncompressed original weights to obtain the maximum quality of coloring.**
* **[27.04.2022] - DeOldify.NET has become a testing ground for the latest optimized algorithms. The Conv2d layer has been optimized and now requires significantly less memory. Support for SIMD vectorization will allow you to get about a fourfold increase in performance.**
* [29.10.2021] - **Big refactoring and code clean up**
* [29.10.2021] - **Linux support**
* [16.09.2021] - **Fixed a memory leak issue**

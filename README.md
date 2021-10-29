# DeOldify.NET
*C# implementation of Jason Antic's DeOldify(https://github.com/jantic/DeOldify)* **Only for photos for now!**

# How to run

## On Windows 7, 8, 8.1, 10, 11
* Make sure that .NET Framework 4.5 or higher is installed on your computer.
* You can use any bit depth(x32 or x64), but on a 32-bit system you will not be able to process large images due to the limited amount of memory.
* At least 3 GB of free RAM is required to run.
* Download and unpack the repository, then download DeOldify.hmodel from the releases(https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/DeOldify.hmodel) and place it in Implementation\src\Resources.
* Run Compile.bat
* The DeOldify.NET.exe file will appear in the Implementation\Release folder. The application is ready to work!
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
* Download sources.
<details>
<summary>Using git and terminal</summary>

```
git clone https://github.com/ColorfulSoft/DeOldify.NET.git
cd DeOldify.NET-main
wget https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/DeOldify.hmodel -O Implementation/src/Resources/DeOldify.hmodel
```

</details>

# Examples

![Example1](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Example1.jpg)

![Example2](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Example2.jpg)

# Updates
* [16.09.2021] - **Fixed a memory leak issue**

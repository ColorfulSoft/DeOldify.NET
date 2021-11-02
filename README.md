# DeOldify.NET
*C# implementation of Jason Antic's DeOldify(https://github.com/jantic/DeOldify)* **Only for photos for now!**

# How to run

## On Windows 7, 8, 8.1, 10, 11
* Make sure that .NET Framework 4.5 or higher is installed on your computer.

* You can use any bit depth(x32 or x64), but on a 32-bit system you will not be able to process large images due to the limited amount of memory.

* At least 3 GB of free RAM is required to run.

* Download and unpack the repository, then download DeOldify.hmodel from the releases(https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/DeOldify.hmodel) and place it in Implementation\src\Resources.

* Run Compile.bat

* The DeOldify.NET.win.exe file will appear in the Implementation\Release folder. The application is ready to work!

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
cd DeOldify.NET
wget https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/DeOldify.hmodel -O Implementation/src/Resources/DeOldify.hmodel
```

</details>
<details>
<summary>Using GUI</summary>

* Download and unpack the repository.
  
* Download DeOldify.hmodel from the releases(https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/DeOldify.hmodel) and place it in Implementation/src/Resources.
</details>

* Run Compile.sh `bash Compile.sh`

* The DeOldify.NET.linux.exe file will appear in the Implementation/Release folder. The application is ready to work!

* Run application using "mono DeOldify.NET.linux.exe" command in terminal or double click as in Windows.

* **Use!**

![Linux GUI](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Sample.Linux.jpg)

**_Please note, that DeOldify.NET using Mono is a bit slower, than using .NET Framework_**

# Examples

![Example1](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Example1.jpg)

![Example2](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Example2.jpg)

# Updates

* [29.10.2021] - **Big refactoring and code clean up**
* [29.10.2021] - **Linux support**
* [16.09.2021] - **Fixed a memory leak issue**

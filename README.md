# DeOldify.NET

C# implementation of [https://github.com/jantic/DeOldify](Jason Antic's DeOldify).

## How to run

* Make sure that `.NET Framework 4.5` or higher is installed on your computer.
* You can use any bit depth (x32 or x64), but on a 32-bit system you will not be able to process large images due to the limited amount of memory.
* At least 3 GB of free RAM is required to run.
* Download and unpack the repository, then download `DeOldify.hmodel` from the [https://github.com/ColorfulSoft/DeOldify.NET/releases/download/Weights/DeOldify.hmodel](releases) and place it in Implementation\src\Resources.
* Run `Compile.bat` on Windows or `unix-compile.sh` on Unix-based systems (they are placed in `Implementation` folder).
* The `DeOldify.NET.exe` file will appear in the `Implementation\Release` folder. The application is ready to work!
* **Use!**

![GUI](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Sample.jpg)

## Examples

![Example1](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Example1.jpg)

![Example2](https://github.com/ColorfulSoft/DeOldify.NET/blob/main/Examples/Example2.jpg)

## Updates

* [16.09.2021] - **Fixed a memory leak issue**

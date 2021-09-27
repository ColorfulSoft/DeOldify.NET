@echo off

set "CONFIG=Windows.rsp"
set "SRC=src\"
set "RESOURCES=%SRC%Resources\"

(
  echo #*************************************************************************************************
  echo #* ^(C^) ColorfulSoft corp., 2021. All Rights reserved.
  echo #*************************************************************************************************
  echo.
  echo /unsafe
  echo /optimize
  echo /platform:anycpu
  echo /target:winexe
  echo /out:"Release\DeOldify.NET.exe"
  echo /r:System.Drawing.dll
  echo /r:System.Windows.Forms.dll
  echo.
) > %CONFIG%


for /r %%F in (%RESOURCES%\*) do echo /resource:"%RESOURCES%\%%F" > %CONFIG%
echo /resource:"src\Resources\DeOldify.hmodel" > %CONFIG%

for /r %%F in (%SRC*) do echo "%SRC%F" > %CONFIG%

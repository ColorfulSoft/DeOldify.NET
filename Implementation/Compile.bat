@echo off

md "Release"
set "csc=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"
"%csc%" @"Windows.rsp"
pause > nul

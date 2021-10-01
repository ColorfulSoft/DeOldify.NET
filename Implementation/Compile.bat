@echo off
md "Release"
csc.exe @"Windows.rsp"
pause > nul

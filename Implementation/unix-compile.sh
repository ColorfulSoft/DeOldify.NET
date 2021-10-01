#!/usr/bin/env bash

targetDir="Release"

[[ -d $targetDir ]] && rm -rf "$targetDir"

mkdir "$targetDir"
csc @"unix-config.rsp"
read -rn 1

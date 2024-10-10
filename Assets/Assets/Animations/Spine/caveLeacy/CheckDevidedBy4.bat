@echo off
setlocal enabledelayedexpansion

REM Replace "%~dp0" with the directory containing your PNG files
set "directory=%~dp0"

REM Loop through each PNG file in the directory
for %%F in ("%directory%\*.png") do (
    REM Get width and height of the image using ImageMagick
    for /F %%A in ('magick identify -format "%%w" "%%F"') do set width=%%A
    for /F %%B in ('magick identify -format "%%h" "%%F"') do set height=%%B

    REM Check if width and height are divisible by 4
    set /a mod_width=width %% 4
    set /a mod_height=height %% 4

    REM Output text indicating if divisible by 4 or not
    if !mod_width! equ 0 (
        if !mod_height! equ 0 (
            echo %%F: Width and height are divisible by 4
        ) else (
            echo %%F: ---------------->Height is not divisible by 4<--------------
        )
    ) else (
        echo %%F: ---------------->Width is not divisible by 4<--------------
    )
)

pause

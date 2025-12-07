@echo off
REM ============================================================================
REM .NET 10.0 Upgrade Presentation Converter
REM ============================================================================
REM 
REM This script converts PRESENTATION.md to PowerPoint using Marp
REM 
REM Requirements:
REM   - Node.js installed (https://nodejs.org/)
REM   - Marp CLI will be installed automatically if not present
REM
REM Usage:
REM   Double-click this file or run: convert-presentation.bat
REM
REM Output:
REM   - KeyLogger-NET10-Upgrade.pptx (PowerPoint format)
REM   - KeyLogger-NET10-Upgrade.pdf (PDF format for preview)
REM
REM ============================================================================

echo.
echo ============================================================================
echo  .NET 10.0 Upgrade Presentation Converter
echo ============================================================================
echo.
echo  This script will convert PRESENTATION.md to PowerPoint format
echo.

REM Change to the upgrades directory
cd /d "%~dp0"
echo  Working directory: %CD%
echo.

REM Check if Node.js is installed
echo  [1/5] Checking Node.js installation...
where node >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo  ERROR: Node.js is not installed!
    echo.
    echo  Please install Node.js from: https://nodejs.org/
    echo  Download the LTS version and restart this script.
    echo.
    pause
    exit /b 1
)

node --version
echo  SUCCESS: Node.js is installed
echo.

REM Check if npm is available
echo  [2/5] Checking npm installation...
where npm >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo  ERROR: npm is not installed!
    echo.
    echo  npm should be included with Node.js. Please reinstall Node.js.
    echo.
    pause
    exit /b 1
)

npm --version
echo  SUCCESS: npm is available
echo.

REM Check if Marp CLI is installed, install if not
echo  [3/5] Checking Marp CLI installation...
where marp >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo  Marp CLI not found. Installing...
    echo.
    call npm install -g @marp-team/marp-cli
    if %ERRORLEVEL% NEQ 0 (
        echo  ERROR: Failed to install Marp CLI
        echo.
        echo  Try running this script as Administrator or install manually:
        echo  npm install -g @marp-team/marp-cli
        echo.
        pause
        exit /b 1
    )
    echo  SUCCESS: Marp CLI installed
) else (
    marp --version
    echo  SUCCESS: Marp CLI is already installed
)
echo.

REM Check if PRESENTATION.md exists
echo  [4/5] Checking for PRESENTATION.md...
if not exist "PRESENTATION.md" (
    echo  ERROR: PRESENTATION.md not found in current directory!
    echo.
    echo  Expected location: %CD%\PRESENTATION.md
    echo.
    echo  Please make sure you are running this script from the .github\upgrades folder.
    echo.
    pause
    exit /b 1
)
echo  SUCCESS: PRESENTATION.md found
echo.

REM Convert to PowerPoint and PDF
echo  [5/5] Converting presentation...
echo.
echo  Creating PowerPoint version...

call marp PRESENTATION.md --pptx --allow-local-files -o KeyLogger-NET10-Upgrade.pptx
if %ERRORLEVEL% NEQ 0 (
    echo  ERROR: Failed to create PowerPoint file
    echo.
    echo  Marp error occurred. Check the error message above.
    echo.
    pause
    exit /b 1
)
echo  SUCCESS: PowerPoint file created: KeyLogger-NET10-Upgrade.pptx
echo.

echo  Creating PDF version for preview...
call marp PRESENTATION.md --pdf --allow-local-files -o KeyLogger-NET10-Upgrade.pdf
if %ERRORLEVEL% NEQ 0 (
    echo  WARNING: Failed to create PDF file
    echo  PowerPoint file was created successfully, continuing...
    echo.
) else (
    echo  SUCCESS: PDF file created: KeyLogger-NET10-Upgrade.pdf
    echo.
)

REM Summary
echo.
echo ============================================================================
echo  CONVERSION COMPLETE!
echo ============================================================================
echo.
echo  Output files created:
echo    - KeyLogger-NET10-Upgrade.pptx (PowerPoint)
if exist "KeyLogger-NET10-Upgrade.pdf" (
    echo    - KeyLogger-NET10-Upgrade.pdf (PDF preview)
)
echo.
echo  Location: %CD%
echo.
echo  Next steps:
echo    1. Open KeyLogger-NET10-Upgrade.pptx in PowerPoint
echo    2. Apply your corporate theme (Design -^> Themes)
echo    3. Add company logo to title slide
echo    4. Review and customize as needed
echo.

REM Ask if user wants to open the PowerPoint file
echo  Would you like to open the PowerPoint file now? (Y/N)
set /p OPEN_FILE=  Your choice: 

if /i "%OPEN_FILE%"=="Y" (
    echo.
    echo  Opening PowerPoint file...
    start "" "KeyLogger-NET10-Upgrade.pptx"
)

echo.
echo  Press any key to exit...
pause >nul

exit /b 0

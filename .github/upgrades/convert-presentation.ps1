# ============================================================================
# .NET 10.0 Upgrade Presentation Converter (PowerShell Version)
# ============================================================================
# 
# This script converts PRESENTATION.md to PowerPoint using Marp
# 
# Requirements:
#   - Node.js installed (https://nodejs.org/)
#   - Marp CLI will be installed automatically if not present
#
# Usage:
#   .\convert-presentation.ps1
#   or right-click and select "Run with PowerShell"
#
# Parameters:
#   -Format     : Output format (pptx, pdf, html, all) - Default: pptx
#   -Output     : Output filename (without extension) - Default: KeyLogger-NET10-Upgrade
#   -Theme      : Marp theme (default, gaia, uncover) - Default: default
#   -OpenAfter  : Open file after conversion - Default: $true
#
# Examples:
#   .\convert-presentation.ps1
#   .\convert-presentation.ps1 -Format all
#   .\convert-presentation.ps1 -Format pdf -OpenAfter $false
#   .\convert-presentation.ps1 -Output "CustomName" -Theme gaia
#
# ============================================================================

param(
    [Parameter(HelpMessage="Output format: pptx, pdf, html, or all")]
    [ValidateSet("pptx", "pdf", "html", "all")]
    [string]$Format = "pptx",
    
    [Parameter(HelpMessage="Output filename without extension")]
    [string]$Output = "KeyLogger-NET10-Upgrade",
    
    [Parameter(HelpMessage="Marp theme: default, gaia, or uncover")]
    [ValidateSet("default", "gaia", "uncover")]
    [string]$Theme = "default",
    
    [Parameter(HelpMessage="Open file after conversion")]
    [bool]$OpenAfter = $true
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Script banner
Write-Host ""
Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host " .NET 10.0 Upgrade Presentation Converter (PowerShell)" -ForegroundColor Cyan
Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host " Converting PRESENTATION.md to: $Format" -ForegroundColor White
Write-Host " Output name: $Output" -ForegroundColor White
Write-Host " Theme: $Theme" -ForegroundColor White
Write-Host ""

# Change to script directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir
Write-Host " Working directory: $ScriptDir" -ForegroundColor Gray
Write-Host ""

# Function to check if a command exists
function Test-CommandExists {
    param($Command)
    $null -ne (Get-Command $Command -ErrorAction SilentlyContinue)
}

# Function to display progress
function Write-Progress-Step {
    param($Step, $Total, $Message)
    Write-Host " [$Step/$Total] $Message" -ForegroundColor Yellow
}

# Function to display success
function Write-Success {
    param($Message)
    Write-Host " SUCCESS: $Message" -ForegroundColor Green
}

# Function to display error
function Write-Error-Message {
    param($Message)
    Write-Host " ERROR: $Message" -ForegroundColor Red
}

# Function to display warning
function Write-Warning-Message {
    param($Message)
    Write-Host " WARNING: $Message" -ForegroundColor Yellow
}

try {
    # Step 1: Check Node.js
    Write-Progress-Step -Step 1 -Total 5 -Message "Checking Node.js installation..."
    if (-not (Test-CommandExists "node")) {
        Write-Error-Message "Node.js is not installed!"
        Write-Host ""
        Write-Host " Please install Node.js from: https://nodejs.org/" -ForegroundColor White
        Write-Host " Download the LTS version and restart this script." -ForegroundColor White
        Write-Host ""
        Read-Host " Press Enter to exit"
        exit 1
    }
    $nodeVersion = node --version
    Write-Success "Node.js is installed (Version: $nodeVersion)"
    Write-Host ""

    # Step 2: Check npm
    Write-Progress-Step -Step 2 -Total 5 -Message "Checking npm installation..."
    if (-not (Test-CommandExists "npm")) {
        Write-Error-Message "npm is not installed!"
        Write-Host ""
        Write-Host " npm should be included with Node.js. Please reinstall Node.js." -ForegroundColor White
        Write-Host ""
        Read-Host " Press Enter to exit"
        exit 1
    }
    $npmVersion = npm --version
    Write-Success "npm is available (Version: $npmVersion)"
    Write-Host ""

    # Step 3: Check/Install Marp CLI
    Write-Progress-Step -Step 3 -Total 5 -Message "Checking Marp CLI installation..."
    if (-not (Test-CommandExists "marp")) {
        Write-Host " Marp CLI not found. Installing..." -ForegroundColor Yellow
        Write-Host ""
        npm install -g @marp-team/marp-cli
        if ($LASTEXITCODE -ne 0) {
            Write-Error-Message "Failed to install Marp CLI"
            Write-Host ""
            Write-Host " Try running PowerShell as Administrator or install manually:" -ForegroundColor White
            Write-Host " npm install -g @marp-team/marp-cli" -ForegroundColor White
            Write-Host ""
            Read-Host " Press Enter to exit"
            exit 1
        }
        Write-Success "Marp CLI installed successfully"
    } else {
        $marpVersion = marp --version
        Write-Success "Marp CLI is already installed (Version: $marpVersion)"
    }
    Write-Host ""

    # Step 4: Check if PRESENTATION.md exists
    Write-Progress-Step -Step 4 -Total 5 -Message "Checking for PRESENTATION.md..."
    if (-not (Test-Path "PRESENTATION.md")) {
        Write-Error-Message "PRESENTATION.md not found in current directory!"
        Write-Host ""
        Write-Host " Expected location: $ScriptDir\PRESENTATION.md" -ForegroundColor White
        Write-Host ""
        Write-Host " Please make sure you are running this script from the .github\upgrades folder." -ForegroundColor White
        Write-Host ""
        Read-Host " Press Enter to exit"
        exit 1
    }
    Write-Success "PRESENTATION.md found"
    Write-Host ""

    # Step 5: Convert presentation
    Write-Progress-Step -Step 5 -Total 5 -Message "Converting presentation..."
    Write-Host ""

    $ConvertedFiles = @()

    # Convert based on format parameter
    if ($Format -eq "pptx" -or $Format -eq "all") {
        Write-Host " Creating PowerPoint version..." -ForegroundColor Cyan
        $PptxFile = "$Output.pptx"
        marp PRESENTATION.md --pptx --allow-local-files --theme $Theme -o $PptxFile
        if ($LASTEXITCODE -ne 0) {
            Write-Error-Message "Failed to create PowerPoint file"
            exit 1
        }
        Write-Success "PowerPoint created: $PptxFile"
        $ConvertedFiles += $PptxFile
        Write-Host ""
    }

    if ($Format -eq "pdf" -or $Format -eq "all") {
        Write-Host " Creating PDF version..." -ForegroundColor Cyan
        $PdfFile = "$Output.pdf"
        marp PRESENTATION.md --pdf --allow-local-files --theme $Theme -o $PdfFile
        if ($LASTEXITCODE -eq 0) {
            Write-Success "PDF created: $PdfFile"
            $ConvertedFiles += $PdfFile
        } else {
            Write-Warning-Message "Failed to create PDF file"
        }
        Write-Host ""
    }

    if ($Format -eq "html" -or $Format -eq "all") {
        Write-Host " Creating HTML version..." -ForegroundColor Cyan
        $HtmlFile = "$Output.html"
        marp PRESENTATION.md --html --allow-local-files --theme $Theme -o $HtmlFile
        if ($LASTEXITCODE -eq 0) {
            Write-Success "HTML created: $HtmlFile"
            $ConvertedFiles += $HtmlFile
        } else {
            Write-Warning-Message "Failed to create HTML file"
        }
        Write-Host ""
    }

    # Display summary
    Write-Host ""
    Write-Host "============================================================================" -ForegroundColor Green
    Write-Host " CONVERSION COMPLETE!" -ForegroundColor Green
    Write-Host "============================================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host " Output files created:" -ForegroundColor White
    foreach ($file in $ConvertedFiles) {
        Write-Host "   - $file" -ForegroundColor Cyan
    }
    Write-Host ""
    Write-Host " Location: $ScriptDir" -ForegroundColor Gray
    Write-Host ""
    Write-Host " Next steps:" -ForegroundColor White
    Write-Host "   1. Open the PowerPoint file" -ForegroundColor Gray
    Write-Host "   2. Apply your corporate theme (Design -> Themes)" -ForegroundColor Gray
    Write-Host "   3. Add company logo to title slide" -ForegroundColor Gray
    Write-Host "   4. Review and customize as needed" -ForegroundColor Gray
    Write-Host ""

    # Open file if requested
    if ($OpenAfter -and $ConvertedFiles.Count -gt 0) {
        $FileToOpen = $ConvertedFiles[0]
        Write-Host " Opening $FileToOpen..." -ForegroundColor Yellow
        Start-Process $FileToOpen
    }

    Write-Host ""
    Write-Host " Press Enter to exit..." -ForegroundColor Gray
    Read-Host

} catch {
    Write-Host ""
    Write-Error-Message "An error occurred:"
    Write-Host " $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host " Stack trace:" -ForegroundColor Gray
    Write-Host " $($_.ScriptStackTrace)" -ForegroundColor Gray
    Write-Host ""
    Read-Host " Press Enter to exit"
    exit 1
}

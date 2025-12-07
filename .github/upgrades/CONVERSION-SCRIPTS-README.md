# Presentation Conversion Scripts

This folder contains automated scripts to convert `PRESENTATION.md` to PowerPoint and other formats.

---

## ?? Quick Start

### Windows Batch Script (Easiest)

**Double-click**: `convert-presentation.bat`

or from Command Prompt:
```cmd
cd .github\upgrades
convert-presentation.bat
```

### PowerShell Script (Advanced)

**Right-click** `convert-presentation.ps1` ? **Run with PowerShell**

or from PowerShell:
```powershell
cd .github\upgrades
.\convert-presentation.ps1
```

---

## ?? What These Scripts Do

1. ? Check if Node.js is installed
2. ? Check if npm is available  
3. ? Install Marp CLI automatically (if needed)
4. ? Convert PRESENTATION.md to PowerPoint
5. ? Create PDF preview (optional)
6. ? Open the file automatically

---

## ?? Script Comparison

| Feature | Batch (.bat) | PowerShell (.ps1) |
|---------|--------------|-------------------|
| **Easy to use** | ? Double-click | ? Right-click |
| **Output formats** | PPTX + PDF | PPTX, PDF, HTML |
| **Custom themes** | ? No | ? Yes |
| **Custom filenames** | ? No | ? Yes |
| **Parameters** | ? No | ? Yes |
| **Color output** | ? Basic | ? Colorized |
| **Error handling** | ? Basic | ? Advanced |

---

## ?? Usage Examples

### Batch Script (Simple)

```cmd
REM Basic conversion (PowerPoint + PDF)
convert-presentation.bat
```

**Output**:
- `KeyLogger-NET10-Upgrade.pptx`
- `KeyLogger-NET10-Upgrade.pdf`

---

### PowerShell Script (Advanced)

#### Default (PowerPoint only)
```powershell
.\convert-presentation.ps1
```

#### All formats (PowerPoint + PDF + HTML)
```powershell
.\convert-presentation.ps1 -Format all
```

#### PDF only, don't open after
```powershell
.\convert-presentation.ps1 -Format pdf -OpenAfter $false
```

#### Custom filename with Gaia theme
```powershell
.\convert-presentation.ps1 -Output "MyPresentation" -Theme gaia
```

#### HTML for web hosting
```powershell
.\convert-presentation.ps1 -Format html -Output "index"
```

---

## ?? Available Themes

### Default Theme
Clean and simple, professional look
```powershell
.\convert-presentation.ps1 -Theme default
```

### Gaia Theme
Corporate and elegant
```powershell
.\convert-presentation.ps1 -Theme gaia
```

### Uncover Theme
Minimalist and modern
```powershell
.\convert-presentation.ps1 -Theme uncover
```

---

## ?? PowerShell Parameters Reference

### `-Format`
**Type**: String  
**Values**: `pptx`, `pdf`, `html`, `all`  
**Default**: `pptx`  
**Description**: Output format(s) to generate

**Examples**:
```powershell
# PowerPoint only
.\convert-presentation.ps1 -Format pptx

# PDF only
.\convert-presentation.ps1 -Format pdf

# HTML only
.\convert-presentation.ps1 -Format html

# All formats
.\convert-presentation.ps1 -Format all
```

---

### `-Output`
**Type**: String  
**Default**: `KeyLogger-NET10-Upgrade`  
**Description**: Output filename (without extension)

**Examples**:
```powershell
# Custom filename
.\convert-presentation.ps1 -Output "NET10-Presentation"

# Date-stamped filename
$date = Get-Date -Format "yyyy-MM-dd"
.\convert-presentation.ps1 -Output "Presentation-$date"
```

---

### `-Theme`
**Type**: String  
**Values**: `default`, `gaia`, `uncover`  
**Default**: `default`  
**Description**: Marp theme to apply

**Examples**:
```powershell
# Default theme
.\convert-presentation.ps1 -Theme default

# Gaia theme (corporate)
.\convert-presentation.ps1 -Theme gaia

# Uncover theme (minimalist)
.\convert-presentation.ps1 -Theme uncover
```

---

### `-OpenAfter`
**Type**: Boolean  
**Default**: `$true`  
**Description**: Open file after conversion

**Examples**:
```powershell
# Open file (default)
.\convert-presentation.ps1 -OpenAfter $true

# Don't open file
.\convert-presentation.ps1 -OpenAfter $false
```

---

## ??? Prerequisites

### Required

**Node.js** (LTS version recommended)
- Download: https://nodejs.org/
- Install and restart your terminal
- Verify: `node --version`

### Auto-Installed

**Marp CLI** (installed automatically by scripts)
- Package: `@marp-team/marp-cli`
- Installed via: `npm install -g @marp-team/marp-cli`

---

## ?? Troubleshooting

### "Node.js is not installed"

**Solution**: Install Node.js
1. Visit https://nodejs.org/
2. Download LTS version
3. Run installer
4. Restart terminal
5. Re-run script

---

### "Failed to install Marp CLI"

**Solution 1**: Run as Administrator
```powershell
# Right-click PowerShell ? Run as Administrator
.\convert-presentation.ps1
```

**Solution 2**: Manual installation
```powershell
npm install -g @marp-team/marp-cli
```

**Solution 3**: Check npm permissions
```powershell
# Check current permissions
npm config get prefix

# Set user-level prefix (no admin needed)
npm config set prefix $env:APPDATA\npm
```

---

### "PRESENTATION.md not found"

**Solution**: Run script from correct directory
```powershell
# Navigate to upgrades folder
cd C:\GitHub\MarkHazleton\KeyPressCounter\.github\upgrades

# Then run script
.\convert-presentation.ps1
```

---

### PowerShell execution policy error

**Error**: "cannot be loaded because running scripts is disabled"

**Solution**: Allow script execution
```powershell
# Check current policy
Get-ExecutionPolicy

# Set policy for current user
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Or run with bypass
powershell -ExecutionPolicy Bypass -File .\convert-presentation.ps1
```

---

### Conversion fails with Marp error

**Solution 1**: Update Marp CLI
```powershell
npm update -g @marp-team/marp-cli
```

**Solution 2**: Check Marp version
```powershell
marp --version
# Should be 3.0.0 or higher
```

**Solution 3**: Reinstall Marp CLI
```powershell
npm uninstall -g @marp-team/marp-cli
npm install -g @marp-team/marp-cli
```

---

## ?? Output Files

### PowerPoint (.pptx)
- **Use for**: Editing, presenting, sharing
- **Opens in**: Microsoft PowerPoint, LibreOffice Impress
- **Editable**: Yes
- **File size**: Medium (~2-5 MB)

### PDF (.pdf)
- **Use for**: Preview, distribution, archiving
- **Opens in**: Any PDF reader
- **Editable**: No (read-only)
- **File size**: Small (~1-3 MB)

### HTML (.html)
- **Use for**: Web hosting, browser viewing
- **Opens in**: Any web browser
- **Editable**: Yes (source code)
- **File size**: Small (~100-500 KB)

---

## ?? Recommended Workflows

### For Quick Review
```powershell
# Create PDF for quick preview
.\convert-presentation.ps1 -Format pdf
```

### For Presentation
```powershell
# Create PowerPoint for editing
.\convert-presentation.ps1 -Format pptx

# Then customize in PowerPoint:
# 1. Apply corporate theme
# 2. Add company logo
# 3. Adjust colors
# 4. Review slide content
```

### For Distribution
```powershell
# Create all formats
.\convert-presentation.ps1 -Format all

# Distribute:
# - PPTX for editable version
# - PDF for read-only sharing
# - HTML for web hosting
```

### For Archiving
```powershell
# Create with date stamp
$date = Get-Date -Format "yyyy-MM-dd"
.\convert-presentation.ps1 -Format all -Output "KeyLogger-Upgrade-$date"
```

---

## ?? Advanced Usage

### Batch Processing Multiple Files

**PowerShell script**: `convert-all.ps1`
```powershell
# Convert all markdown files in folder
Get-ChildItem *.md | ForEach-Object {
    $name = $_.BaseName
    .\convert-presentation.ps1 -Output $name -Format pptx
}
```

### Custom Theme Configuration

**Create custom theme**: `custom-theme.css`
```css
/* custom-theme.css */
section {
    background-color: #f0f0f0;
    color: #333;
}
h1 {
    color: #0066cc;
}
```

**Apply custom theme**:
```powershell
marp PRESENTATION.md --theme-set custom-theme.css --pptx -o output.pptx
```

---

## ?? Script Maintenance

### Update Scripts

Scripts are located in:
```
.github/upgrades/
??? convert-presentation.bat      ? Batch script
??? convert-presentation.ps1      ? PowerShell script
??? CONVERSION-SCRIPTS-README.md  ? This file
```

### Version History

**v1.0** (December 7, 2025)
- Initial release
- Batch and PowerShell versions
- Auto-install Marp CLI
- Multiple output formats
- Theme support

---

## ?? Tips & Best Practices

### 1. Preview First
Always create PDF first to preview content:
```powershell
.\convert-presentation.ps1 -Format pdf
```

### 2. Use Themes Appropriately
- **default**: General purpose, clean
- **gaia**: Corporate presentations
- **uncover**: Minimalist, modern

### 3. Customize in PowerPoint
Don't spend time making PRESENTATION.md perfect:
1. Convert with script
2. Customize in PowerPoint
3. Apply corporate theme
4. Add branding

### 4. Keep Original
Always keep `PRESENTATION.md`:
- Easy to regenerate PowerPoint
- Version control friendly
- Can update and reconvert

### 5. Automate Regular Conversions
Add to your workflow:
```powershell
# After updating PRESENTATION.md
.\convert-presentation.ps1 -Format all
```

---

## ?? Support

### For Script Issues

**Check**:
1. Node.js installed? `node --version`
2. npm available? `npm --version`
3. Marp installed? `marp --version`
4. In correct directory? `pwd` or `cd`

**Get Help**:
- Review troubleshooting section above
- Check Marp documentation: https://marp.app/
- Check Node.js documentation: https://nodejs.org/

### For Content Issues

**Modify**:
1. Edit `PRESENTATION.md` directly
2. Re-run conversion script
3. Review output

**Resources**:
- Marp Markdown syntax: https://marpit.marp.app/markdown
- Marp CLI documentation: https://github.com/marp-team/marp-cli

---

## ?? Success Checklist

After running scripts:

- [ ] PowerPoint file created successfully
- [ ] File opens in PowerPoint without errors
- [ ] All slides present (42 slides)
- [ ] Code blocks formatted correctly
- [ ] Tables render properly
- [ ] No missing content
- [ ] Ready for customization

---

## ?? Quick Reference Card

**Most Common Commands**:

```powershell
# Default conversion (PPTX only)
.\convert-presentation.ps1

# All formats
.\convert-presentation.ps1 -Format all

# PDF preview
.\convert-presentation.ps1 -Format pdf

# Custom name with Gaia theme
.\convert-presentation.ps1 -Output "MyPresentation" -Theme gaia

# Don't open after
.\convert-presentation.ps1 -OpenAfter $false
```

---

*Scripts Version 1.0*  
*Last Updated: December 7, 2025*  
*For: KeyLogger .NET 10.0 Upgrade Project*

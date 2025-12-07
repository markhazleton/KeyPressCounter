# ?? Quick Start: Convert Presentation to PowerPoint

**Goal**: Convert `PRESENTATION.md` to PowerPoint in 2 minutes

---

## ? Fastest Method (Windows)

### Option 1: Double-Click Batch File

```
1. Navigate to: C:\GitHub\MarkHazleton\KeyPressCounter\.github\upgrades
2. Double-click: convert-presentation.bat
3. Wait for conversion (30-60 seconds)
4. PowerPoint opens automatically!
```

**That's it!** ??

---

### Option 2: PowerShell (One Command)

```powershell
cd C:\GitHub\MarkHazleton\KeyPressCounter\.github\upgrades
.\convert-presentation.ps1
```

**Done!** ??

---

## ?? What You'll Get

```
KeyLogger-NET10-Upgrade.pptx
??? 42 Professional Slides
??? All Content from PRESENTATION.md
??? Ready for PowerPoint Editing
??? Ready to Present
```

---

## ?? Customization (Optional)

### Change Theme

```powershell
# Corporate look
.\convert-presentation.ps1 -Theme gaia

# Minimalist look
.\convert-presentation.ps1 -Theme uncover
```

### Get All Formats

```powershell
# Creates PPTX + PDF + HTML
.\convert-presentation.ps1 -Format all
```

### Custom Filename

```powershell
.\convert-presentation.ps1 -Output "MyPresentation"
```

---

## ?? First Time Setup (One-Time)

### Install Node.js

**Only if you don't have Node.js:**

1. Visit: https://nodejs.org/
2. Download: LTS version (left button)
3. Install: Run installer (accept defaults)
4. Restart: Close and reopen terminal

**Verify Installation**:
```powershell
node --version
# Should show: v20.x.x or similar
```

### Install Marp CLI

**Automatic**: Scripts install Marp automatically!

**Or install manually**:
```powershell
npm install -g @marp-team/marp-cli
```

---

## ?? Complete Workflow

```
???????????????????????????????????????
? 1. Open Terminal                    ?
?    (PowerShell or Command Prompt)   ?
???????????????????????????????????????
           ?
???????????????????????????????????????
? 2. Navigate to upgrades folder      ?
?    cd .github\upgrades              ?
???????????????????????????????????????
           ?
???????????????????????????????????????
? 3. Run conversion script            ?
?    .\convert-presentation.ps1       ?
?    or                               ?
?    convert-presentation.bat         ?
???????????????????????????????????????
           ?
???????????????????????????????????????
? 4. Wait 30-60 seconds               ?
?    (Script installs Marp if needed) ?
???????????????????????????????????????
           ?
???????????????????????????????????????
? 5. PowerPoint opens automatically!  ?
?    File: KeyLogger-NET10-Upgrade    ?
???????????????????????????????????????
           ?
???????????????????????????????????????
? 6. Customize in PowerPoint          ?
?    - Apply corporate theme          ?
?    - Add company logo               ?
?    - Review slides                  ?
???????????????????????????????????????
           ?
???????????????????????????????????????
? 7. Save and Present! ??             ?
???????????????????????????????????????
```

---

## ?? Troubleshooting (Quick Fixes)

### Error: "Node.js is not installed"

**Fix**: Install Node.js from https://nodejs.org/

---

### Error: "Failed to install Marp"

**Fix 1**: Run as Administrator
```powershell
Right-click PowerShell ? Run as Administrator
.\convert-presentation.ps1
```

**Fix 2**: Install manually
```powershell
npm install -g @marp-team/marp-cli
```

---

### Error: "PRESENTATION.md not found"

**Fix**: Make sure you're in the right folder
```powershell
cd C:\GitHub\MarkHazleton\KeyPressCounter\.github\upgrades
pwd  # Verify location
```

---

### Error: "Cannot run scripts"

**Fix**: Allow PowerShell scripts
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

---

## ?? What The Scripts Do

```
Step 1: Check Node.js installed     [?]
Step 2: Check npm available         [?]
Step 3: Install Marp CLI (if needed)[?]
Step 4: Find PRESENTATION.md        [?]
Step 5: Convert to PowerPoint       [?]
Step 6: Open file                   [?]
```

**All automatic!** No manual work required.

---

## ?? Pro Tips

### 1. Preview Before PowerPoint

Create PDF first to preview:
```powershell
.\convert-presentation.ps1 -Format pdf
# Opens: KeyLogger-NET10-Upgrade.pdf
```

### 2. Create All Formats at Once

```powershell
.\convert-presentation.ps1 -Format all
# Creates:
# - KeyLogger-NET10-Upgrade.pptx
# - KeyLogger-NET10-Upgrade.pdf
# - KeyLogger-NET10-Upgrade.html
```

### 3. Custom Name with Date

```powershell
$date = Get-Date -Format "yyyy-MM-dd"
.\convert-presentation.ps1 -Output "Presentation-$date"
# Creates: Presentation-2025-12-07.pptx
```

---

## ?? Expected Output

### Console Output (Success)

```
============================================================================
 .NET 10.0 Upgrade Presentation Converter
============================================================================

 [1/5] Checking Node.js installation...
 SUCCESS: Node.js is installed (Version: v20.10.0)

 [2/5] Checking npm installation...
 SUCCESS: npm is available (Version: 10.2.3)

 [3/5] Checking Marp CLI installation...
 SUCCESS: Marp CLI is already installed (Version: 3.4.0)

 [4/5] Checking for PRESENTATION.md...
 SUCCESS: PRESENTATION.md found

 [5/5] Converting presentation...

 Creating PowerPoint version...
 SUCCESS: PowerPoint created: KeyLogger-NET10-Upgrade.pptx

============================================================================
 CONVERSION COMPLETE!
============================================================================

 Output files created:
   - KeyLogger-NET10-Upgrade.pptx

 Opening PowerPoint file...
```

---

## ? Success Checklist

After conversion:

- [ ] File created: `KeyLogger-NET10-Upgrade.pptx`
- [ ] File opens in PowerPoint
- [ ] 42 slides present
- [ ] Title slide shows correctly
- [ ] Code blocks formatted
- [ ] Tables render properly
- [ ] All content present

**If all checked: Success!** ??

---

## ?? Full Documentation

For advanced usage and troubleshooting:
- **Scripts README**: `CONVERSION-SCRIPTS-README.md`
- **Conversion Guide**: `CONVERSION-GUIDE.md`
- **Marp Documentation**: https://marp.app/

---

## ?? Summary

**Simplest Method**:
```
Double-click: convert-presentation.bat
Wait: 30-60 seconds
Done! PowerPoint opens automatically
```

**Total Time**: 1-2 minutes (first time includes Marp install)

**Result**: Professional 42-slide PowerPoint presentation ready to customize!

---

*Quick Start Guide Version 1.0*  
*Last Updated: December 7, 2025*

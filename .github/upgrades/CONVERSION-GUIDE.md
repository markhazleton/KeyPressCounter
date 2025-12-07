# Converting PRESENTATION.md to PowerPoint/Slides

This guide explains how to convert the `PRESENTATION.md` file into various presentation formats.

---

## ?? Conversion Options

### Option 1: Marp (Recommended for PowerPoint)

**What is Marp?**  
Marp is a Markdown-to-slide converter that creates beautiful presentations from Markdown files.

**Installation**:
```bash
# Using npm
npm install -g @marp-team/marp-cli

# Using VS Code Extension
# Install "Marp for VS Code" extension
```

**Convert to PowerPoint**:
```bash
cd .github/upgrades
marp PRESENTATION.md --pptx -o KeyLogger-NET10-Upgrade.pptx
```

**Convert to PDF**:
```bash
marp PRESENTATION.md --pdf -o KeyLogger-NET10-Upgrade.pdf
```

**Preview in Browser**:
```bash
marp PRESENTATION.md --preview
```

**Resources**:
- Website: https://marp.app/
- Documentation: https://marpit.marp.app/
- VS Code Extension: https://marketplace.visualstudio.com/items?itemName=marp-team.marp-vscode

---

### Option 2: Slidev (Interactive Web Presentations)

**What is Slidev?**  
Slidev creates interactive, web-based presentations with animations and code highlighting.

**Installation**:
```bash
npm install -g @slidev/cli
```

**Create Presentation**:
```bash
cd .github/upgrades
slidev PRESENTATION.md
```

**Export to PDF**:
```bash
slidev export PRESENTATION.md --output KeyLogger-NET10-Upgrade.pdf
```

**Resources**:
- Website: https://sli.dev/
- Documentation: https://sli.dev/guide/
- Examples: https://sli.dev/showcases

---

### Option 3: Pandoc (Universal Converter)

**What is Pandoc?**  
Pandoc is a universal document converter supporting many formats.

**Installation**:
- Windows: Download from https://pandoc.org/installing.html
- Or via Chocolatey: `choco install pandoc`

**Convert to PowerPoint**:
```bash
cd .github/upgrades
pandoc PRESENTATION.md -o KeyLogger-NET10-Upgrade.pptx
```

**Convert to PDF (requires LaTeX)**:
```bash
pandoc PRESENTATION.md -o KeyLogger-NET10-Upgrade.pdf
```

**Convert to HTML**:
```bash
pandoc PRESENTATION.md -s -o KeyLogger-NET10-Upgrade.html
```

**Resources**:
- Website: https://pandoc.org/
- Documentation: https://pandoc.org/MANUAL.html
- Try Online: https://pandoc.org/try/

---

### Option 4: reveal.js (Web Presentations)

**What is reveal.js?**  
Create stunning web-based presentations with animations and transitions.

**Using Pandoc + reveal.js**:
```bash
pandoc PRESENTATION.md -t revealjs -s -o presentation.html
```

**Open in Browser**:
```bash
# Open presentation.html in your browser
start presentation.html
```

**Resources**:
- Website: https://revealjs.com/
- Documentation: https://revealjs.com/
- Examples: https://revealjs.com/demo/

---

### Option 5: Google Slides (Manual Import)

**Steps**:
1. Convert to PPTX using Marp or Pandoc (see above)
2. Upload PPTX to Google Drive
3. Right-click ? Open with ? Google Slides
4. Edit as needed in Google Slides

---

### Option 6: Microsoft PowerPoint (Direct Import)

**Method 1: Using Marp**
1. Install Marp (see Option 1)
2. Convert: `marp PRESENTATION.md --pptx -o output.pptx`
3. Open in PowerPoint

**Method 2: Copy-Paste**
1. Open PRESENTATION.md in VS Code with Markdown Preview
2. Copy rendered content
3. Paste into PowerPoint slides
4. Format as needed

---

## ?? Styling Options

### Marp Themes

**Add theme directive to PRESENTATION.md**:
```markdown
---
marp: true
theme: default
---
# Your Presentation
```

**Available themes**:
- `default` - Clean, simple design
- `gaia` - Professional, corporate look
- `uncover` - Minimalist, centered

**Custom CSS**:
```markdown
---
marp: true
style: |
  section {
    background-color: #f0f0f0;
    color: #333;
  }
---
```

---

### Slidev Themes

**Using built-in themes**:
```bash
slidev PRESENTATION.md --theme apple-basic
```

**Popular themes**:
- `default` - Slidev default
- `seriph` - Elegant serif
- `apple-basic` - Apple Keynote style
- `bricks` - Masonry layout

**Theme Gallery**: https://sli.dev/themes/gallery

---

## ?? Layout Tips

### Slide Separators

The PRESENTATION.md file uses `---` as slide separators:
```markdown
# Slide 1

---

# Slide 2
```

### Two-Column Layouts

**Marp**:
```markdown
<div class="columns">
<div>

Left column content

</div>
<div>

Right column content

</div>
</div>
```

**Slidev**:
```markdown
:::: columns
::: column
Left content
:::
::: column
Right content
:::
::::
```

---

## ?? Troubleshooting

### Issue: Marp not converting properly

**Solution**:
```bash
# Check Marp version
marp --version

# Update Marp
npm update -g @marp-team/marp-cli

# Try with verbose output
marp PRESENTATION.md --pptx -o output.pptx --allow-local-files
```

---

### Issue: Pandoc missing features

**Solution**:
```bash
# Install with extended features
choco install pandoc-extended

# Or download full installer from pandoc.org
```

---

### Issue: Images not displaying

**Solution**:
- Use absolute paths or URLs for images
- Or use `--allow-local-files` flag with Marp
```bash
marp PRESENTATION.md --pptx --allow-local-files
```

---

### Issue: Code blocks not formatting

**Solution**:
Add language identifier to code blocks:
```markdown
```csharp
var hook = new TaskPoolGlobalHook();
` ` `
```

---

## ?? Best Practices

### 1. Use Consistent Formatting
- Keep slide titles (H1/H2) consistent
- Use `---` for slide breaks
- Maintain consistent spacing

### 2. Optimize for Readability
- Limit text per slide (max 6-7 bullet points)
- Use larger fonts (will be auto-adjusted by converters)
- Include white space

### 3. Test Your Conversion
```bash
# Quick preview before full conversion
marp PRESENTATION.md --preview

# Check PDF before PPTX
marp PRESENTATION.md --pdf -o test.pdf
```

### 4. Keep Backups
```bash
# Create backup before modifying
cp PRESENTATION.md PRESENTATION.md.backup
```

---

## ?? Recommended Workflow

### For Corporate Presentations (PowerPoint)

1. **Convert with Marp**:
   ```bash
   marp PRESENTATION.md --pptx -o KeyLogger-Upgrade.pptx
   ```

2. **Customize in PowerPoint**:
   - Apply corporate theme
   - Add company logo
   - Adjust colors/fonts

3. **Export final version**:
   - Save as PPTX for editing
   - Export as PDF for distribution

---

### For Web Presentations (Interactive)

1. **Use Slidev**:
   ```bash
   slidev PRESENTATION.md
   ```

2. **Customize with theme**:
   - Choose from theme gallery
   - Add custom CSS if needed

3. **Host online**:
   - Export static site
   - Deploy to GitHub Pages or Netlify

---

### For PDF Distribution

1. **Convert with Marp**:
   ```bash
   marp PRESENTATION.md --pdf -o KeyLogger-Upgrade.pdf
   ```

2. **Optimize PDF**:
   - Compress if needed
   - Add metadata (title, author)

3. **Distribute**:
   - Email to stakeholders
   - Upload to shared drive

---

## ?? Quick Commands Cheat Sheet

```bash
# Marp: Markdown ? PowerPoint
marp PRESENTATION.md --pptx -o output.pptx

# Marp: Markdown ? PDF
marp PRESENTATION.md --pdf -o output.pdf

# Marp: Preview in browser
marp PRESENTATION.md --preview

# Slidev: Interactive web presentation
slidev PRESENTATION.md

# Pandoc: Markdown ? PowerPoint
pandoc PRESENTATION.md -o output.pptx

# Pandoc: Markdown ? HTML (reveal.js)
pandoc PRESENTATION.md -t revealjs -s -o output.html
```

---

## ?? Additional Resources

### Learning Resources
- **Marp Tutorial**: https://marp.app/
- **Slidev Guide**: https://sli.dev/guide/
- **Pandoc Manual**: https://pandoc.org/MANUAL.html
- **Markdown Guide**: https://www.markdownguide.org/

### Community
- **Marp GitHub**: https://github.com/marp-team/marp
- **Slidev Discord**: https://chat.sli.dev/
- **Pandoc Discussions**: https://github.com/jgm/pandoc/discussions

### Tools
- **VS Code Marp Extension**: Instant preview in VS Code
- **Obsidian Marp Plugin**: For Obsidian users
- **Markdown Viewer**: https://markdownlivepreview.com/

---

## ? Recommended Setup

### For First-Time Users

1. **Install Marp CLI**:
   ```bash
   npm install -g @marp-team/marp-cli
   ```

2. **Test conversion**:
   ```bash
   cd .github/upgrades
   marp PRESENTATION.md --pdf -o test.pdf
   ```

3. **Review output**:
   - Open test.pdf
   - Check formatting
   - Verify all slides present

4. **Create final version**:
   ```bash
   marp PRESENTATION.md --pptx -o KeyLogger-NET10-Upgrade.pptx
   ```

5. **Customize in PowerPoint**:
   - Open .pptx file
   - Apply theme/branding
   - Review and adjust

---

*Conversion Guide Version 1.0*  
*Last Updated: December 7, 2025*

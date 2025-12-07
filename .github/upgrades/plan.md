# .NET 10.0 Upgrade Migration Plan

## Executive Summary

### Scenario
Upgrade KeyLogger solution from .NET 9.0 to .NET 10.0 (Preview). This is a single-project Windows Forms application that captures and logs keyboard and mouse activity.

### Scope
- **Total Projects**: 1
- **Project Name**: MWH.KeyPressCounter
- **Current State**: net9.0-windows (Windows Forms application)
- **Lines of Code**: 2,028
- **NuGet Packages**: 3

### Target State
- **Target Framework**: net10.0-windows
- **Package Updates**: 2 upgrades required
- **Critical Issue**: 1 package incompatibility (MouseKeyHook)

### Selected Strategy
**Big Bang Strategy** - Single project upgrade in one atomic operation.

**Rationale**: 
- Single project solution (simplest case)
- Small codebase (~2,000 LOC)
- No project dependencies
- Clear upgrade path for Microsoft packages
- Big Bang is the only logical approach for single-project solutions

### Complexity Assessment
**Medium Risk** - Due to MouseKeyHook package incompatibility

**Risk Factors**:
- ⚠️ **MouseKeyHook 5.7.1** has no compatible version for .NET 10.0
- This is a critical dependency for core functionality (keyboard/mouse hooks)
- May require finding alternative package or manual compatibility verification
- Two Microsoft packages have straightforward upgrade paths

### Critical Issues
1. **MouseKeyHook Incompatibility**: The analysis indicates no supported version for .NET 10.0. This requires investigation:
   - Verify if the package actually works with .NET 10.0 despite the warning
   - Explore alternative packages (gma.System.MouseKeyHook, SharpHook, etc.)
   - Consider implementing native P/Invoke hooks if necessary

### Recommended Approach
**Big Bang Strategy** - Update all components simultaneously in a single coordinated operation.

---

## Migration Strategy

### 2.1 Approach Selection

**Chosen Strategy**: Big Bang Strategy

**Justification**:
- Single project solution eliminates any dependency ordering concerns
- Small codebase makes atomic upgrade practical and low-risk
- No intermediate states needed
- Testing surface is contained to one project
- Faster completion with single validation cycle

### 2.2 Dependency-Based Ordering

**Not Applicable** - Single project has no internal dependencies.

The project has zero dependencies on other projects and zero dependents, making it a standalone unit.

### 2.3 Parallel vs Sequential Execution

**Not Applicable** - Single project means all changes happen in one operation.

---

## Detailed Dependency Analysis

### 3.1 Dependency Graph Summary

```
MWH.KeyPressCounter.csproj (net9.0-windows → net10.0-windows)
├── No project dependencies
└── No project dependents
```

**Migration Order**: Single phase, atomic upgrade.

### 3.2 Project Groupings

**Phase 0: Prerequisites**
- Verify .NET 10.0 SDK installation
- Check global.json compatibility (if exists)

**Phase 1: Atomic Upgrade**
- MWH.KeyPressCounter.csproj (all changes in single operation)

**Phase 2: Validation**
- Build verification
- Functional testing

---

## Project-by-Project Migration Plans

### Project: MWH.KeyPressCounter.csproj

**Current State**
- **Target Framework**: net9.0-windows
- **Project Type**: Windows Forms (WinExe)
- **Dependencies**: 0 projects
- **Dependants**: 0 projects
- **Package Count**: 3
- **LOC**: 2,028
- **Files**: 8

**Current Packages**:
- MouseKeyHook 5.7.1
- System.Management 8.0.0
- System.Diagnostics.PerformanceCounter 8.0.0

**Target State**
- **Target Framework**: net10.0-windows
- **Updated Packages**: 2 confirmed, 1 requires investigation

**Migration Steps**

#### 1. Prerequisites

**Verify .NET 10.0 SDK Installation**
- Confirm .NET 10.0 SDK is installed on the development machine
- If not installed, download from: https://dotnet.microsoft.com/download/dotnet/10.0

**Check global.json**
- Verify if global.json exists in solution directory
- If exists, ensure SDK version is compatible with .NET 10.0
- Update if necessary to allow .NET 10.0 SDK

#### 2. Framework Update

**Update Project File**: `MWH.KeyPressCounter.csproj`

Change:
```xml
<TargetFramework>net9.0-windows</TargetFramework>
```

To:
```xml
<TargetFramework>net10.0-windows</TargetFramework>
```

#### 3. Package Updates

| Package | Current Version | Target Version | Action | Reason |
|---------|----------------|----------------|--------|---------|
| System.Management | 8.0.0 | 10.0.0 | Update | Framework compatibility - Standard Microsoft package upgrade |
| System.Diagnostics.PerformanceCounter | 8.0.0 | 10.0.0 | Update | Framework compatibility - Standard Microsoft package upgrade |
| MouseKeyHook | 5.7.1 | **Investigate** | Special Handling | No compatible version detected - requires verification |

**Standard Package Updates** (Straightforward):
- System.Management: Update to 10.0.0 - Standard upgrade for .NET 10.0 compatibility
- System.Diagnostics.PerformanceCounter: Update to 10.0.0 - Standard upgrade for .NET 10.0 compatibility

**Critical Package - MouseKeyHook** (Requires Special Attention):

The MouseKeyHook package (5.7.1) is flagged as incompatible with .NET 10.0. This requires a multi-step investigation:

**Option A: Verify Actual Compatibility** (Try First)
1. Attempt to keep MouseKeyHook 5.7.1 unchanged initially
2. Restore packages and build to see if it actually works
3. .NET 10.0 may be binary compatible even if metadata doesn't indicate support
4. If builds and runs correctly, document as working despite warning

**Option B: Upgrade to Latest MouseKeyHook** (If Available)
1. Check NuGet for newer versions of MouseKeyHook
2. Check GitHub repository (https://github.com/gmamaladze/globalmousekeyhook) for updates
3. If newer version available, update to latest

**Option C: Alternative Package** (If Options A/B Fail)
Consider alternative packages:
- **SharpHook** - Cross-platform global keyboard and mouse hook
- **gma.System.MouseKeyHook** (alternate NuGet package name)
- **H.Hooks** - Modern .NET global hooks library

**Option D: Custom Implementation** (Last Resort)
- Implement P/Invoke-based hooks using Windows API
- Use SetWindowsHookEx/UnhookWindowsHookEx
- More complex but full control

**Recommended Approach**: Try Option A first, then B, then C, then D.

#### 4. Expected Breaking Changes

**Framework Breaking Changes** (.NET 9.0 → .NET 10.0):

Based on typical .NET upgrades, potential breaking changes include:

1. **Windows Forms Changes**:
   - Control rendering updates
   - DPI scaling improvements
   - Event handler changes
   - Font handling updates

2. **Runtime Changes**:
   - Performance counter API refinements
   - System.Management API updates
   - Threading model improvements

3. **MouseKeyHook Specific**:
   - If alternative package needed, API surface will differ
   - Hook registration patterns may change
   - Event argument types may differ

**Specific Areas to Review** (Based on Windows Forms + System Hooks):

- **Form1.cs**: Main form implementation
  - Check DPI scaling behavior
  - Verify control layouts
  - Test icon loading (`favicon.ico`)

- **Hook Registration Code**:
  - Keyboard hook setup
  - Mouse hook setup
  - Event handler signatures
  - Disposal patterns

- **Performance Counter Usage**:
  - Counter instantiation
  - Permission requirements
  - Counter categories

- **System.Management Usage**:
  - WMI query execution
  - Security permissions
  - Object disposal

#### 5. Code Modifications

**Phase 1: Immediate Changes**

1. **Project File** (`MWH.KeyPressCounter.csproj`):
   - Update TargetFramework to net10.0-windows
   - Update System.Management to 10.0.0
   - Update System.Diagnostics.PerformanceCounter to 10.0.0
   - Handle MouseKeyHook per investigation results (Options A-D above)

2. **Restore and Build**:
   - Run `dotnet restore`
   - Run `dotnet build`
   - Capture all compilation errors

**Phase 2: Compilation Error Fixes**

After initial build, address errors in priority order:

1. **Critical**: API removed or changed
2. **High**: Namespace changes
3. **Medium**: Method signature changes
4. **Low**: Warning-level issues

**Expected Code Changes** (Specific to Common Scenarios):

- **If MouseKeyHook replaced**: Refactor all hook registration and event handling
- **Performance counters**: Update counter initialization if API changed
- **WMI queries**: Update query syntax if System.Management API changed
- **Windows Forms**: Address DPI or rendering changes if present

**Files Likely Requiring Changes**:
- Any file using `MouseKeyHook` namespace
- Any file using `System.Diagnostics.PerformanceCounter`
- Any file using `System.Management`
- Form initialization code

#### 6. Testing Strategy

**Unit Testing**:
- Not applicable - no test projects in solution

**Integration Testing**:
- Build succeeds without errors
- Application starts successfully
- Main window renders correctly
- Icon displays properly

**Functional Testing** (Manual - Critical Scenarios):

1. **Hook Registration**:
   - [ ] Application starts without exceptions
   - [ ] Keyboard hook registers successfully
   - [ ] Mouse hook registers successfully

2. **Event Capture**:
   - [ ] Keyboard events are captured
   - [ ] Mouse events are captured
   - [ ] Events are logged correctly
   - [ ] No missed events under normal use

3. **Performance Monitoring**:
   - [ ] Performance counters initialize correctly
   - [ ] Counter values update as expected
   - [ ] No permission errors

4. **System Management**:
   - [ ] WMI queries execute successfully
   - [ ] System information retrieved correctly

5. **UI Functionality**:
   - [ ] Window opens at correct size/position
   - [ ] Application icon displays in taskbar
   - [ ] All controls render correctly
   - [ ] Application responds to user input

6. **Stability**:
   - [ ] Application runs for extended period without crashes
   - [ ] Memory usage remains stable
   - [ ] CPU usage is acceptable
   - [ ] Clean shutdown releases all hooks

#### 7. Validation Checklist

- [ ] .NET 10.0 SDK verified installed
- [ ] global.json compatible (if exists)
- [ ] Dependencies resolve correctly (dotnet restore succeeds)
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] Application executable launches
- [ ] Keyboard hooks functional
- [ ] Mouse hooks functional
- [ ] Performance counters accessible
- [ ] System management queries work
- [ ] UI renders correctly
- [ ] No runtime exceptions during basic operations
- [ ] MouseKeyHook compatibility resolved (Option A, B, C, or D successful)

---

## Implementation Timeline

### Phase 0: Preparation

**Operations**:
- Verify .NET 10.0 SDK installation
- Check and update global.json (if present)

**Deliverables**: Development environment ready for .NET 10.0

**Estimated Time**: 15-30 minutes

---

### Phase 1: Atomic Upgrade

**Operations** (performed as single coordinated batch):

1. **Update project file** to target net10.0-windows
2. **Update Microsoft packages** (System.Management, System.Diagnostics.PerformanceCounter) to 10.0.0
3. **Address MouseKeyHook** per investigation strategy (Options A-D)
4. **Restore dependencies** (`dotnet restore`)
5. **Build solution** (`dotnet build`)
6. **Fix compilation errors** identified during build
7. **Rebuild to verify** all errors resolved

**Deliverables**: Solution builds with 0 errors

**Estimated Time**: 2-4 hours
- 30 minutes: Project file and Microsoft package updates
- 1-2 hours: MouseKeyHook investigation and resolution
- 30 minutes-1 hour: Compilation error fixes
- 30 minutes: Build verification

**MouseKeyHook Time Breakdown**:
- Option A (verify compatibility): 30 minutes
- Option B (upgrade): 1 hour
- Option C (alternative package): 2 hours
- Option D (custom implementation): 4-8 hours (if needed)

---

### Phase 2: Test Validation

**Operations**:
- Launch application
- Execute functional test scenarios (per Testing Strategy section)
- Verify keyboard and mouse hook functionality
- Confirm performance counters and system management features
- Validate UI rendering and stability
- Extended stability testing (30-60 minutes runtime)

**Deliverables**: All functional tests pass, application stable

**Estimated Time**: 1-2 hours
- 30 minutes: Basic functional testing
- 30 minutes: Hook functionality verification
- 30-60 minutes: Extended stability testing

---

### Total Estimated Timeline

**Optimistic** (MouseKeyHook Option A succeeds): 3.5-6.5 hours
**Realistic** (MouseKeyHook requires Option B or C): 4-8 hours
**Pessimistic** (MouseKeyHook requires Option D): 8-14 hours

---

## Package Update Reference

### Microsoft Package Updates (Straightforward)

| Package | Current | Target | Projects Affected | Update Reason | Priority |
|---------|---------|--------|-------------------|---------------|----------|
| System.Management | 8.0.0 | 10.0.0 | 1 (MWH.KeyPressCounter) | Framework compatibility | High |
| System.Diagnostics.PerformanceCounter | 8.0.0 | 10.0.0 | 1 (MWH.KeyPressCounter) | Framework compatibility | High |

### Third-Party Package (Requires Investigation)

| Package | Current | Target | Projects Affected | Update Reason | Priority |
|---------|---------|--------|-------------------|---------------|----------|
| MouseKeyHook | 5.7.1 | **TBD** | 1 (MWH.KeyPressCounter) | No .NET 10.0 compatible version detected | **CRITICAL** |

**MouseKeyHook Investigation Strategy**:
1. **First**: Test with existing version (binary compatibility)
2. **Second**: Check for updated version on NuGet
3. **Third**: Evaluate alternative packages (SharpHook, H.Hooks)
4. **Fourth**: Custom P/Invoke implementation

---

## Breaking Changes Catalog

### .NET 9.0 → .NET 10.0 Framework Changes

**Note**: .NET 10.0 is currently in Preview. Breaking changes are subject to change before final release. Monitor:
- https://learn.microsoft.com/en-us/dotnet/core/compatibility/10.0
- https://github.com/dotnet/core/releases

**Common Breaking Change Categories**:

#### 1. Windows Forms Changes
- **DPI Scaling**: Improvements may affect control positioning
- **Font Rendering**: Changes to default font handling
- **Control Lifecycle**: Event ordering may change
- **Designer Support**: May require regenerating designer files

#### 2. System.Management Changes
- **WMI Query Execution**: Performance improvements may change behavior
- **Security**: Enhanced permission requirements
- **Object Disposal**: Stricter dispose patterns

#### 3. System.Diagnostics.PerformanceCounter Changes
- **Counter Access**: May require updated permissions
- **Counter Categories**: Some legacy categories may be deprecated
- **Multi-instance Counters**: API refinements

#### 4. MouseKeyHook (If Alternative Needed)
- **API Surface**: Complete API change if switching packages
- **Event Arguments**: Different event argument types
- **Hook Lifetime**: Different hook management patterns
- **Thread Affinity**: May require different threading approach

### Specific Code Patterns to Review

**Pattern 1: Hook Registration**
```csharp
// Current (MouseKeyHook 5.7.1)
var hook = Hook.GlobalEvents();
hook.KeyPress += OnKeyPress;

// May need update depending on investigation outcome
```

**Pattern 2: Performance Counter Instantiation**
```csharp
// Review for API changes
var counter = new PerformanceCounter(categoryName, counterName, instanceName);
```

**Pattern 3: WMI Queries**
```csharp
// Review for API changes
var searcher = new ManagementObjectSearcher(query);
```

**Pattern 4: Windows Forms Initialization**
```csharp
// Review DPI scaling changes
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
```

---

## Testing and Validation Strategy

### 6.1 Phase-by-Phase Testing

**Phase 0: Prerequisites**
- [ ] .NET 10.0 SDK installed and detected
- [ ] SDK version compatible with global.json (if exists)

**Phase 1: Atomic Upgrade**
- [ ] `dotnet restore` succeeds
- [ ] `dotnet build` succeeds
- [ ] Zero compilation errors
- [ ] Zero build warnings (stretch goal)
- [ ] Executable file generated

**Phase 2: Functional Validation**
- [ ] Application launches without exceptions
- [ ] Main window appears
- [ ] Application icon displays correctly

### 6.2 Smoke Tests

Quick validation after atomic upgrade completes:

**Build Smoke Test** (5 minutes):
```powershell
cd C:\GitHub\MarkHazleton\KeyPressCounter
dotnet clean
dotnet restore
dotnet build --configuration Release
# Expected: Build succeeded with 0 errors
```

**Launch Smoke Test** (2 minutes):
```powershell
# Start application
.\bin\Release\net10.0-windows\MWH.KeyPressCounter.exe
# Expected: Window opens, no exceptions in console
```

**Hook Smoke Test** (3 minutes):
1. Launch application
2. Press several keys
3. Move mouse
4. Click mouse buttons
5. Expected: Events captured and logged

### 6.3 Comprehensive Validation

Before marking upgrade complete:

**Functional Test Suite** (30-45 minutes):

1. **Startup and Initialization** (5 minutes)
   - [ ] Application starts within 5 seconds
   - [ ] No exception dialogs appear
   - [ ] Main window renders correctly
   - [ ] Taskbar icon displays
   - [ ] Window is responsive

2. **Keyboard Hook Functionality** (10 minutes)
   - [ ] Standard keys captured (A-Z, 0-9)
   - [ ] Special keys captured (Enter, Tab, Escape)
   - [ ] Modifier keys captured (Ctrl, Alt, Shift)
   - [ ] Function keys captured (F1-F12)
   - [ ] Rapid key presses handled correctly
   - [ ] No missed key events

3. **Mouse Hook Functionality** (10 minutes)
   - [ ] Mouse movement tracked
   - [ ] Left click captured
   - [ ] Right click captured
   - [ ] Middle click captured
   - [ ] Mouse wheel events captured
   - [ ] Rapid clicks handled correctly
   - [ ] No missed mouse events

4. **Performance Monitoring** (5 minutes)
   - [ ] Performance counters initialize
   - [ ] Counter values update correctly
   - [ ] No permission errors
   - [ ] No access denied exceptions

5. **System Management Queries** (5 minutes)
   - [ ] WMI queries execute successfully
   - [ ] System information retrieved
   - [ ] No permission errors
   - [ ] No timeout errors

6. **UI and Controls** (5 minutes)
   - [ ] All controls visible and positioned correctly
   - [ ] Buttons respond to clicks
   - [ ] Text fields accept input
   - [ ] Lists/grids display data correctly
   - [ ] Menus work (if present)
   - [ ] Tooltips appear (if present)

**Stability Test** (30-60 minutes):
- [ ] Application runs continuously for 30-60 minutes
- [ ] Memory usage remains stable (no memory leaks)
- [ ] CPU usage is acceptable (<5% at idle)
- [ ] No exceptions thrown during extended run
- [ ] Hook continue to function correctly
- [ ] Clean shutdown releases all hooks properly

**Performance Benchmarks**:
| Metric | Target | Notes |
|--------|--------|-------|
| Startup Time | < 5 seconds | Time to main window displayed |
| Memory Usage (Idle) | < 50 MB | After initial hooks registered |
| Memory Usage (Active) | < 100 MB | During active key/mouse capture |
| CPU Usage (Idle) | < 1% | Background monitoring |
| CPU Usage (Active) | < 5% | During event capture |
| Event Capture Latency | < 50ms | Time from physical event to log |
| Hook Registration Time | < 1 second | Time to register all hooks |

---

## Risk Management

### 7.1 High-Risk Changes

| Project | Risk | Impact | Probability | Mitigation |
|---------|------|--------|-------------|------------|
| MWH.KeyPressCounter | MouseKeyHook incompatibility | **HIGH** - Core functionality broken | Medium | 4-option investigation strategy (A→B→C→D) |
| MWH.KeyPressCounter | Windows Forms breaking changes | Medium - UI/rendering issues | Low | Comprehensive UI testing, DPI validation |
| MWH.KeyPressCounter | Performance counter API changes | Medium - Monitoring features broken | Low | Thorough testing, API documentation review |
| MWH.KeyPressCounter | .NET 10.0 Preview instability | Medium - Runtime issues | Medium | Extended stability testing, monitoring release notes |

### 7.2 Risk Details

#### Risk 1: MouseKeyHook Package Incompatibility
**Description**: Analysis indicates no .NET 10.0 compatible version exists

**Impact**: 
- Core keyboard/mouse hooking functionality may be broken
- Application cannot fulfill its primary purpose
- May require significant refactoring

**Mitigation Strategy**:
1. **Option A** (Low effort): Test with existing version - binary compatibility may work
2. **Option B** (Low effort): Update to newer MouseKeyHook version if available
3. **Option C** (Medium effort): Migrate to alternative package (SharpHook, H.Hooks)
4. **Option D** (High effort): Implement custom P/Invoke hooks

**Rollback**: Keep Option A code in separate branch before attempting Options C or D

**Time Buffer**: Add 2-4 hours for Options C-D scenarios

#### Risk 2: .NET 10.0 Preview Stability
**Description**: .NET 10.0 is currently in Preview, not RTM

**Impact**:
- Potential runtime bugs or crashes
- APIs may change before final release
- Limited community support and documentation
- Upgrade may need rework when RTM released

**Mitigation Strategy**:
- Monitor .NET 10.0 release notes and breaking changes
- Test extensively with extended stability runs
- Keep .NET 9.0 version in main branch as fallback
- Plan for re-testing when .NET 10.0 reaches RTM
- Subscribe to .NET announcements for breaking changes

**Recommendation**: Consider waiting for .NET 10.0 RTM unless Preview features required

#### Risk 3: Windows Forms DPI/Rendering Changes
**Description**: Windows Forms improvements in .NET 10.0 may affect UI

**Impact**:
- Controls may be mispositioned
- Fonts may render differently
- Icons may scale incorrectly
- User experience degraded

**Mitigation Strategy**:
- Comprehensive UI testing on multiple DPI settings (96, 120, 144 DPI)
- Visual comparison screenshots (before/after)
- Test on multiple Windows versions (10, 11)
- Review Windows Forms breaking changes documentation

**Rollback**: UI issues can often be fixed with minor adjustments to control properties

### 7.3 Contingency Plans

#### Contingency 1: MouseKeyHook Cannot Be Resolved
**Trigger**: All options (A-D) fail or take excessive time

**Actions**:
1. Rollback to .NET 9.0 (main branch)
2. Document findings and blockers
3. Contact MouseKeyHook maintainers for .NET 10.0 support timeline
4. Wait for .NET 10.0 RTM and re-evaluate
5. Consider alternative project architecture (e.g., Windows Service with different hooking approach)

**Decision Point**: If Option C or D will take >8 hours, pause and reassess

#### Contingency 2: Critical Runtime Issues in .NET 10.0 Preview
**Trigger**: Application crashes, data corruption, or unacceptable performance

**Actions**:
1. Document specific issues with repro steps
2. Check .NET GitHub issues for similar reports
3. Rollback to .NET 9.0 immediately
4. Report issues to .NET team
5. Wait for .NET 10.0 RTM or hotfix
6. Re-attempt upgrade after fix available

**Decision Point**: Any data corruption or security issue = immediate rollback

#### Contingency 3: Microsoft Package Breaking Changes
**Trigger**: System.Management or System.Diagnostics.PerformanceCounter APIs changed significantly

**Actions**:
1. Review .NET 10.0 API documentation for alternatives
2. Implement API changes following official guidance
3. Add compatibility layer if needed
4. If no alternative exists, consider removing feature temporarily
5. Escalate to Microsoft support if critical and undocumented

**Decision Point**: If workaround takes >4 hours, escalate for support

#### Contingency 4: Unacceptable Performance Degradation
**Trigger**: Performance benchmarks not met (see 6.3 Comprehensive Validation)

**Actions**:
1. Profile application to identify bottlenecks
2. Check .NET 10.0 performance improvements/regressions
3. Review application event handler efficiency
4. Consider async patterns for hook processing
5. If unresolvable, rollback and wait for performance fixes

**Decision Point**: If >20% performance regression with no fix, rollback

### 7.4 Rollback Procedures

**Rollback Trigger Conditions**:
- Critical functionality broken with no 4-hour fix available
- Data corruption or security issues
- Unacceptable performance with no resolution
- .NET 10.0 Preview bugs blocking progress

**Rollback Steps**:

1. **Immediate Rollback** (5 minutes):
   ```powershell
   cd C:\GitHub\MarkHazleton\KeyPressCounter
   git checkout main
   git branch -D upgrade-to-NET10  # Optional: delete upgrade branch
   ```

2. **Verify Rollback** (5 minutes):
   ```powershell
   dotnet restore
   dotnet build
   # Launch and verify functionality
   ```

3. **Document Issues** (30 minutes):
   - Create detailed issue report
   - Include error messages, stack traces
   - Document reproduction steps
   - Save diagnostic logs

4. **Plan Next Steps**:
   - Wait for .NET 10.0 RTM
   - Wait for package updates
   - Investigate alternative approaches
   - Set reminder to retry in 1-3 months

**Data Preservation**:
- Source code on main branch unchanged
- User data unaffected (application doesn't store state)
- Configuration files remain compatible

---

## Source Control Strategy

### 8.1 Strategy-Specific Guidance

**Big Bang Single-Commit Approach** (Recommended for this upgrade):

Since this is a single-project solution with atomic upgrade, prefer a single comprehensive commit:

**Commit Strategy**:
1. Complete all changes in Phase 1 (Atomic Upgrade)
2. Verify build succeeds with 0 errors
3. Create single commit with complete upgrade

**Commit Message Template**:
```
Upgrade to .NET 10.0

- Update target framework: net9.0-windows → net10.0-windows
- Update System.Management: 8.0.0 → 10.0.0
- Update System.Diagnostics.PerformanceCounter: 8.0.0 → 10.0.0
- [MouseKeyHook resolution: describe outcome of Options A-D]
- Fix compilation errors: [list key fixes if any]

Resolves: [Issue number if tracked]
```

**Alternative: Checkpoint Commits** (If MouseKeyHook investigation is lengthy):

If MouseKeyHook resolution (Options C or D) takes significant time:

**Commit 1 - Framework and Microsoft Packages**:
```
chore: upgrade framework and Microsoft packages to .NET 10.0

- Update target framework: net9.0-windows → net10.0-windows
- Update System.Management: 8.0.0 → 10.0.0
- Update System.Diagnostics.PerformanceCounter: 8.0.0 → 10.0.0
- MouseKeyHook: investigation in progress
```

**Commit 2 - MouseKeyHook Resolution**:
```
fix: resolve MouseKeyHook compatibility for .NET 10.0

- [Describe specific resolution: Option A, B, C, or D]
- [List code changes made]
- [Note any API changes]
```

**Commit 3 - Final Fixes** (if needed):
```
fix: address remaining compilation errors

- [List specific fixes]
```

### 8.2 Branching Strategy

**Primary Branch**: `upgrade-to-NET10` (already created)

**Branch Lifecycle**:
1. **Development**: All work happens on `upgrade-to-NET10`
2. **Validation**: Testing happens on `upgrade-to-NET10`
3. **Integration**: Merge to `main` after all validation passes

**Investigation Branches** (Optional, if MouseKeyHook requires extensive R&D):

If testing multiple MouseKeyHook options:
- `upgrade-to-NET10-hook-optionA` - Test existing version
- `upgrade-to-NET10-hook-optionB` - Test upgraded version
- `upgrade-to-NET10-hook-optionC` - Test alternative package
- `upgrade-to-NET10-hook-optionD` - Custom implementation

Delete investigation branches once solution chosen, merge into `upgrade-to-NET10`.

### 8.3 Commit Strategy

**Default Approach**: Single comprehensive commit after Phase 1 complete

**Commit Frequency**:
- **Minimal commits** preferred for clarity
- **Checkpoint commits** only if investigation is lengthy or risky
- **No WIP commits** - only commit working states

**Commit Content Standards**:
- All commits must build successfully (no broken states)
- Include relevant file changes together
- Clear commit messages following template above

**What to Commit**:
- Project file changes (.csproj)
- Code changes (if any compilation fixes needed)
- Configuration changes (if any)
- Documentation updates (if any)

**What NOT to Commit**:
- `bin/` and `obj/` directories (already in .gitignore)
- User-specific files (.suo, .user)
- Temporary or backup files

### 8.4 Review and Merge Process

**Pre-Merge Checklist**:
- [ ] All validation tests pass (Section 6.3)
- [ ] Build succeeds with 0 errors
- [ ] Build succeeds with 0 warnings (stretch)
- [ ] Functional testing complete
- [ ] Stability testing complete (30-60 minutes runtime)
- [ ] No known regressions
- [ ] Commit messages clear and descriptive

**Merge Steps**:

1. **Final Validation** (1 hour):
   ```powershell
   cd C:\GitHub\MarkHazleton\KeyPressCounter
   git checkout upgrade-to-NET10
   dotnet clean
   dotnet restore
   dotnet build --configuration Release
   # Run full functional test suite
   # Run 30-60 minute stability test
   ```

2. **Prepare for Merge**:
   ```powershell
   git checkout main
   git pull origin main  # Ensure main is up-to-date
   git checkout upgrade-to-NET10
   git rebase main  # Rebase if needed
   ```

3. **Merge to Main**:
   ```powershell
   git checkout main
   git merge --no-ff upgrade-to-NET10
   # Or create Pull Request if using PR workflow
   ```

4. **Post-Merge Validation**:
   ```powershell
   git checkout main
   dotnet clean
   dotnet build --configuration Release
   # Quick smoke test
   ```

5. **Push and Tag**:
   ```powershell
   git push origin main
   git tag -a v2.0.0-net10 -m "Upgrade to .NET 10.0"
   git push origin v2.0.0-net10
   ```

**Pull Request Template** (if using PR workflow):

```markdown
## .NET 10.0 Upgrade

### Summary
Upgrades KeyLogger solution from .NET 9.0 to .NET 10.0 (Preview).

### Changes
- Target framework: net9.0-windows → net10.0-windows
- System.Management: 8.0.0 → 10.0.0
- System.Diagnostics.PerformanceCounter: 8.0.0 → 10.0.0
- MouseKeyHook: [describe resolution]

### Testing
- [x] Build succeeds with 0 errors
- [x] Functional tests pass
- [x] Stability test (60 minutes) pass
- [x] Performance benchmarks met
- [x] No regressions identified

### Breaking Changes
[List any breaking changes or note "None"]

### Rollback Plan
Rollback to main branch if critical issues discovered.

### Related Issues
Resolves #[issue number]
```

**Review Criteria**:
- Code builds successfully
- No compiler warnings (ideal)
- Functional tests documented and passed
- Commit messages are clear
- No debug code or commented-out blocks
- Proper error handling maintained

---

## Success Criteria

### 9.1 Strategy-Specific Success Criteria

**Big Bang Strategy Success**:
- [x] Single atomic upgrade completed without intermediate states
- [x] All changes applied simultaneously (framework + packages)
- [x] Solution transitions cleanly from .NET 9.0 to .NET 10.0
- [x] No multi-targeting required (clean single-target upgrade)

### 9.2 Technical Success Criteria

**Framework Migration**:
- [x] Project migrated from net9.0-windows to net10.0-windows
- [x] .NET 10.0 SDK verified and functioning

**Package Updates**:
- [x] System.Management updated to 10.0.0
- [x] System.Diagnostics.PerformanceCounter updated to 10.0.0
- [x] MouseKeyHook compatibility resolved (via Option A, B, C, or D)
- [x] Zero security vulnerabilities in dependencies

**Build Quality**:
- [x] Solution builds without errors (`dotnet build` succeeds)
- [x] Solution builds without warnings (ideal, not blocking)
- [x] All package dependencies resolve correctly
- [x] Executable file generated in bin/Release/net10.0-windows

**Runtime Quality**:
- [x] Application launches successfully
- [x] No unhandled exceptions during startup
- [x] All hooks register correctly (keyboard + mouse)
- [x] Performance counters initialize and function
- [x] System management queries execute successfully

### 9.3 Quality Criteria

**Functional Quality**:
- [x] All core features working (keyboard logging, mouse logging)
- [x] UI renders correctly (main window, icon, controls)
- [x] No regressions in functionality
- [x] Performance meets or exceeds .NET 9.0 baseline

**Code Quality**:
- [x] Code quality maintained (no quick hacks)
- [x] Proper error handling preserved
- [x] Resource disposal patterns maintained
- [x] No obsolete API warnings

**Stability**:
- [x] Application runs for 30-60 minutes without crashes
- [x] Memory usage stable (no leaks)
- [x] CPU usage acceptable
- [x] Clean shutdown releases all hooks

**Documentation**:
- [x] README updated to note .NET 10.0 requirement (if exists)
- [x] Any MouseKeyHook changes documented
- [x] Known issues documented (if any)
- [x] Build instructions updated (if needed)

### 9.4 Process Criteria

**Source Control**:
- [x] Big Bang single-commit approach followed (or minimal checkpoint commits)
- [x] Clear, descriptive commit messages
- [x] No broken intermediate commits
- [x] Upgrade branch merged to main cleanly

**Validation**:
- [x] All smoke tests passed
- [x] All functional tests passed
- [x] Stability testing completed (30-60 minutes)
- [x] Performance benchmarks met or exceeded

**Risk Management**:
- [x] MouseKeyHook incompatibility resolved
- [x] All contingency plans documented
- [x] Rollback procedure tested (if needed)
- [x] No blocking issues remain

### 9.5 Scenario-Specific Criteria

**.NET Version Upgrade Specific**:
- [x] Target framework correctly set to net10.0-windows
- [x] No multi-targeting needed (clean single-target)
- [x] Runtime version detected correctly (verify with `dotnet --version`)
- [x] No .NET 9.0 assemblies remain in output folder

**Windows Forms Specific**:
- [x] Windows Forms application launches and displays main window
- [x] DPI scaling works correctly (test at 96, 120, 144 DPI)
- [x] Application icon displays in taskbar and window
- [x] All controls render at correct positions

**Global Hooks Specific**:
- [x] Keyboard hook registers without administrator privileges (if designed that way)
- [x] Mouse hook registers without administrator privileges (if designed that way)
- [x] Hook events fire reliably with low latency (<50ms)
- [x] Hooks unregister cleanly on application exit
- [x] No hook-related memory leaks

---

## Definition of Done

The .NET 10.0 upgrade is **COMPLETE** when:

1. ✅ All technical success criteria met
2. ✅ All quality criteria met
3. ✅ All process criteria met
4. ✅ All scenario-specific criteria met
5. ✅ No critical or high-priority issues remain open
6. ✅ Upgrade branch merged to main
7. ✅ Version tagged (e.g., v2.0.0-net10)
8. ✅ Build artifacts generated and verified

**Final Sign-Off Requirements**:
- [ ] Developer testing complete
- [ ] Peer review passed (if applicable)
- [ ] Documentation updated
- [ ] Release notes prepared
- [ ] Rollback procedure documented and understood
- [ ] Stakeholders informed of .NET 10.0 Preview status

---

## Additional Notes

### .NET 10.0 Preview Considerations

**Important**: .NET 10.0 is currently in **Preview** status (not RTM/GA). This means:

1. **Not Production-Ready** (yet):
   - APIs may change before final release
   - Performance characteristics may change
   - Bugs may exist that will be fixed in RTM
   - Not recommended for production deployments

2. **Re-Testing Required**:
   - Plan to re-test when .NET 10.0 reaches RTM
   - Breaking changes may occur between Preview and RTM
   - Budget time for re-validation

3. **Documentation Limitations**:
   - Some APIs may lack complete documentation
   - Community support limited during Preview
   - Fewer Stack Overflow answers available

4. **Recommendation**: Unless .NET 10.0 Preview features are required, consider waiting for .NET 10.0 RTM (November 2025 expected) before upgrading production systems.

### MouseKeyHook Package - Deep Dive

The MouseKeyHook package is critical to application functionality. Here's additional context:

**Package Details**:
- **NuGet**: MouseKeyHook 5.7.1
- **GitHub**: https://github.com/gmamaladze/globalmousekeyhook
- **Last Updated**: Check NuGet for latest update date
- **Target Frameworks**: Check package metadata

**Investigation Checklist**:
1. Check NuGet for versions newer than 5.7.1
2. Check GitHub repository for commits after 5.7.1 release
3. Check GitHub issues for .NET 10.0 compatibility reports
4. Test binary compatibility with .NET 10.0
5. Review alternative package ecosystems (SharpHook, H.Hooks)

**Alternative Packages** (if replacement needed):

| Package | NuGet | GitHub | Target Frameworks | Notes |
|---------|-------|--------|-------------------|-------|
| SharpHook | [SharpHook](https://www.nuget.org/packages/SharpHook/) | [SharpHook GitHub](https://github.com/TolikPylypchuk/SharpHook) | .NET 6+ | Cross-platform, actively maintained |
| H.Hooks | [H.Hooks](https://www.nuget.org/packages/H.Hooks/) | [H.Hooks GitHub](https://github.com/HavenDV/H.Hooks) | .NET 6+ | Modern API, good documentation |
| gma.System.MouseKeyHook | [gma variant](https://www.nuget.org/packages/gma.System.MouseKeyHook/) | N/A | .NET 4.5+ | Alternative NuGet package name |

### Performance Counter Permissions

If performance counter access issues occur:

**Windows Permissions Required**:
- Performance Monitor Users group membership, OR
- Administrator privileges

**Common Issues**:
```
UnauthorizedAccessException: Cannot access performance counter
```

**Resolution**:
1. Add user to "Performance Monitor Users" group
2. Or run application elevated (not ideal)
3. Or disable performance counter features

### Testing Environments

Recommended testing configurations:

| Environment | OS | DPI | Resolution | Purpose |
|-------------|----|----|------------|---------|
| Dev Primary | Windows 11 | 96 DPI | 1920x1080 | Primary development |
| Dev High-DPI | Windows 11 | 144 DPI | 3840x2160 | DPI scaling testing |
| Legacy | Windows 10 | 96 DPI | 1920x1080 | Backwards compatibility |

### Known .NET 10.0 Features

Features that may be relevant to this application:

1. **Performance Improvements**: Potential startup time improvements
2. **Windows Forms Enhancements**: Improved DPI scaling and accessibility
3. **Diagnostics Improvements**: Better performance counter APIs
4. **Trimming**: Improved self-contained deployment size (if using)

Monitor: https://devblogs.microsoft.com/dotnet/ for .NET 10.0 announcements

---

## Plan Metadata

- **Plan Created**: [Current Date]
- **Target Framework**: .NET 10.0 (Preview)
- **Solution**: KeyLogger
- **Project Count**: 1
- **Strategy**: Big Bang
- **Estimated Duration**: 3.5-14 hours (depending on MouseKeyHook resolution)
- **Risk Level**: Medium
- **Plan Version**: 1.0
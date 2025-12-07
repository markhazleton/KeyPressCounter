# .NET 10.0 Upgrade Tasks

**Strategy**: Big Bang - Single atomic upgrade
**Target Framework**: net10.0-windows
**MouseKeyHook Decision**: Use SharpHook (pre-approved by user)

---

### [?] TASK-001: Verify .NET 10.0 SDK Installation *(Completed: 2025-12-07 16:36)*
**Progress**: 1/15 tasks complete (7%) ![7%](https://progress-bar.xyz/7)
### [?] TASK-001: Verify .NET 10.0 SDK Installation *(Completed: 2025-12-07 16:36)*
### [ ] TASK-001: Verify .NET 10.0 SDK Installation
**Objective**: Confirm .NET 10.0 SDK is installed and ready

- [?] (1) Run `dotnet --list-sdks` to check installed SDKs
- [?] (2) Verify .NET 10.0 SDK is present
- [?] (3) If not present, download from https://dotnet.microsoft.com/download/dotnet/10.0
- [?] (4) Verify installation with `dotnet --version`
- [ ] (4) Verify installation with `dotnet --version`

**Success Criteria**: .NET 10.0 SDK detected and functional

---

### [ ] TASK-002: Check global.json Compatibility
**Objective**: Ensure global.json (if exists) is compatible with .NET 10.0

**Actions**:
- [ ] (1) Check if global.json exists in solution root
- [ ] (2) If exists, read SDK version requirement
- [ ] (3) If incompatible, update to allow .NET 10.0
- [ ] (4) If doesn't exist, no action needed

**Success Criteria**: No global.json blockers for .NET 10.0

---

## Phase 1: Atomic Upgrade

### [ ] TASK-003: Update Project File Target Framework
**Objective**: Change MWH.KeyPressCounter.csproj from net9.0-windows to net10.0-windows
**Reference**: Plan §4.2

**Actions**:
- [ ] (1) Open MWH.KeyPressCounter.csproj
- [ ] (2) Locate `<TargetFramework>net9.0-windows</TargetFramework>`
- [ ] (3) Change to `<TargetFramework>net10.0-windows</TargetFramework>`
- [ ] (4) Save file

**Success Criteria**: Project file shows net10.0-windows

---

### [ ] TASK-004: Update Microsoft Packages
**Objective**: Update System.Management and System.Diagnostics.PerformanceCounter to 10.0.0
**Reference**: Plan §4.3

**Actions**:
- [ ] (1) Open MWH.KeyPressCounter.csproj
- [ ] (2) Update System.Management from 8.0.0 to 10.0.0
- [ ] (3) Update System.Diagnostics.PerformanceCounter from 8.0.0 to 10.0.0
- [ ] (4) Save file

**Success Criteria**: Both packages show version 10.0.0

---

### [ ] TASK-005: Replace MouseKeyHook with SharpHook
**Objective**: Replace incompatible MouseKeyHook 5.7.1 with SharpHook
**Reference**: Plan §4.3 (Option C - User Pre-Approved)
**Critical**: This is core functionality - requires code changes

**Actions**:
- [ ] (1) Open MWH.KeyPressCounter.csproj
- [ ] (2) Remove MouseKeyHook 5.7.1 package reference
- [ ] (3) Add SharpHook package (latest stable version)
- [ ] (4) Save project file
- [ ] (5) Search codebase for `using Gma.System.MouseKeyHook` or `MouseKeyHook` references
- [ ] (6) Document all files that use MouseKeyHook (for next task)

**Success Criteria**: Project file has SharpHook, MouseKeyHook removed

---

### [ ] TASK-006: Refactor Code for SharpHook API
**Objective**: Update all hook registration code to use SharpHook API
**Reference**: Plan §4.5 (Code Modifications)
**Critical**: Core functionality changes

**Context**:
MouseKeyHook ? SharpHook API differences:
- Hook creation: `Hook.GlobalEvents()` ? `new TaskPoolGlobalHook()` or `new SimpleGlobalHook()`
- Event registration: Different event names and signatures
- Lifecycle: Requires `RunAsync()` or `Run()` to start
- Disposal: Proper async disposal patterns

**Actions**:
- [ ] (1) Identify all files using MouseKeyHook (from TASK-005)
- [ ] (2) For each file, update `using` statements:
        - Remove: `using Gma.System.MouseKeyHook;`
        - Add: `using SharpHook;` and `using SharpHook.Native;`
- [ ] (3) Update hook initialization code:
        - OLD: `var hook = Hook.GlobalEvents();`
        - NEW: `var hook = new TaskPoolGlobalHook();` (or SimpleGlobalHook)
- [ ] (4) Update event subscriptions:
        - Review SharpHook event names (KeyPressed, KeyReleased, MousePressed, MouseReleased, etc.)
        - Update event handler signatures to match SharpHook's EventArgs types
- [ ] (5) Update hook start logic:
        - Add `await hook.RunAsync();` or `hook.Run();`
- [ ] (6) Update disposal logic:
        - Ensure proper `Dispose()` or `DisposeAsync()` calls
- [ ] (7) Review threading model (SharpHook uses different threading)

**Success Criteria**: All MouseKeyHook references replaced with SharpHook equivalents

---

### [ ] TASK-007: Restore and Build Solution
**Objective**: Restore packages and attempt first build
**Reference**: Plan §4.5

**Actions**:
- [ ] (1) Run `dotnet clean` to remove old build artifacts
- [ ] (2) Run `dotnet restore` to restore all packages
- [ ] (3) Verify restore succeeded for all packages (including SharpHook)
- [ ] (4) Run `dotnet build` 
- [ ] (5) Capture all compilation errors (if any)
- [ ] (6) Capture all warnings

**Success Criteria**: Restore succeeds; Document build results

---

### [ ] TASK-008: Fix Compilation Errors
**Objective**: Resolve any compilation errors from build
**Reference**: Plan §4.5 (Phase 2)

**Actions**:
- [ ] (1) Review compilation errors from TASK-007
- [ ] (2) Fix Critical errors (API removed/changed)
- [ ] (3) Fix High priority (namespace changes)
- [ ] (4) Fix Medium priority (method signature changes)
- [ ] (5) Fix Low priority (warnings)
- [ ] (6) Rebuild after each batch of fixes
- [ ] (7) Verify 0 errors

**Common Issues to Check**:
- SharpHook event handler signatures
- Async/await patterns for hook lifecycle
- Performance counter API changes (if any)
- System.Management API changes (if any)
- Windows Forms DPI changes (if any)

**Success Criteria**: Solution builds with 0 errors

---

## Phase 2: Validation

### [ ] TASK-009: Build Smoke Test
**Objective**: Verify clean build in Release configuration
**Reference**: Plan §6.2

**Actions**:
- [ ] (1) Run `dotnet clean`
- [ ] (2) Run `dotnet restore`
- [ ] (3) Run `dotnet build --configuration Release`
- [ ] (4) Verify: Build succeeded with 0 errors
- [ ] (5) Verify: Executable generated at bin/Release/net10.0-windows/MWH.KeyPressCounter.exe
- [ ] (6) Check warnings (document if any)

**Success Criteria**: Clean Release build, executable present

---

### [ ] TASK-010: Launch Smoke Test
**Objective**: Verify application launches without errors
**Reference**: Plan §6.2

**Actions**:
- [ ] (1) Navigate to bin/Release/net10.0-windows/
- [ ] (2) Run MWH.KeyPressCounter.exe
- [ ] (3) Verify application starts within 5 seconds
- [ ] (4) Verify main window appears
- [ ] (5) Verify taskbar icon displays
- [ ] (6) Verify no exception dialogs
- [ ] (7) Close application cleanly

**Success Criteria**: Application launches and displays UI without errors

---

### [ ] TASK-011: Hook Functionality Smoke Test
**Objective**: Verify SharpHook integration works
**Reference**: Plan §6.2

**Actions**:
- [ ] (1) Launch application
- [ ] (2) Press several keyboard keys (A-Z, 0-9, Enter, Tab)
- [ ] (3) Verify keyboard events are captured and logged
- [ ] (4) Move mouse around screen
- [ ] (5) Click left, right, middle mouse buttons
- [ ] (6) Verify mouse events are captured and logged
- [ ] (7) Verify no missed events
- [ ] (8) Close application cleanly

**Success Criteria**: All keyboard and mouse events captured correctly

---

### [ ] TASK-012: Comprehensive Functional Testing
**Objective**: Execute full functional test suite
**Reference**: Plan §6.3

**Test Areas**:

**Startup and Initialization** (5 minutes):
- [ ] (1) Application starts within 5 seconds
- [ ] (2) No exception dialogs appear
- [ ] (3) Main window renders correctly
- [ ] (4) Taskbar icon displays
- [ ] (5) Window is responsive

**Keyboard Hook Functionality** (10 minutes):
- [ ] (6) Standard keys captured (A-Z, 0-9)
- [ ] (7) Special keys captured (Enter, Tab, Escape)
- [ ] (8) Modifier keys captured (Ctrl, Alt, Shift)
- [ ] (9) Function keys captured (F1-F12)
- [ ] (10) Rapid key presses handled correctly
- [ ] (11) No missed key events

**Mouse Hook Functionality** (10 minutes):
- [ ] (12) Mouse movement tracked
- [ ] (13) Left click captured
- [ ] (14) Right click captured
- [ ] (15) Middle click captured
- [ ] (16) Mouse wheel events captured
- [ ] (17) Rapid clicks handled correctly
- [ ] (18) No missed mouse events

**Performance Monitoring** (5 minutes):
- [ ] (19) Performance counters initialize
- [ ] (20) Counter values update correctly
- [ ] (21) No permission errors
- [ ] (22) No access denied exceptions

**System Management Queries** (5 minutes):
- [ ] (23) WMI queries execute successfully
- [ ] (24) System information retrieved
- [ ] (25) No permission errors
- [ ] (26) No timeout errors

**UI and Controls** (5 minutes):
- [ ] (27) All controls visible and positioned correctly
- [ ] (28) Buttons respond to clicks
- [ ] (29) Text fields accept input
- [ ] (30) Lists/grids display data correctly

**Success Criteria**: All functional tests pass

---

### [ ] TASK-013: Stability Testing
**Objective**: Verify application stability over extended runtime
**Reference**: Plan §6.3

**Actions**:
- [ ] (1) Launch application
- [ ] (2) Let application run for 30-60 minutes
- [ ] (3) Periodically trigger keyboard and mouse events
- [ ] (4) Monitor CPU usage (should be <5% idle, <10% active)
- [ ] (5) Monitor memory usage (should be <50MB idle, <100MB active)
- [ ] (6) Verify no exceptions thrown
- [ ] (7) Verify hooks continue to function
- [ ] (8) Close application cleanly
- [ ] (9) Verify hooks are released (no lingering processes)

**Performance Benchmarks**:
- Startup Time: < 5 seconds ?
- Memory (Idle): < 50 MB ?
- Memory (Active): < 100 MB ?
- CPU (Idle): < 1% ?
- CPU (Active): < 5% ?
- Event Latency: < 50ms ?
- Hook Registration: < 1 second ?

**Success Criteria**: Application stable for 30-60 minutes, no memory leaks, benchmarks met

---

## Phase 3: Finalization

### [ ] TASK-014: Commit Changes
**Objective**: Commit all upgrade changes with descriptive message
**Reference**: Plan §8.1 (Single-Commit Strategy)

**Commit Message**:
```
Upgrade to .NET 10.0

- Update target framework: net9.0-windows ? net10.0-windows
- Update System.Management: 8.0.0 ? 10.0.0
- Update System.Diagnostics.PerformanceCounter: 8.0.0 ? 10.0.0
- Replace MouseKeyHook 5.7.1 with SharpHook (latest)
- Refactor hook registration code for SharpHook API
- All functional tests passing
- Stability verified over 60-minute runtime
```

**Actions**:
- [ ] (1) Review all changed files with `git status`
- [ ] (2) Stage all changes with `git add .`
- [ ] (3) Commit with message above
- [ ] (4) Verify commit succeeded
- [ ] (5) Note commit hash

**Success Criteria**: All changes committed with clear message

---

### [ ] TASK-015: Final Validation
**Objective**: Final check before marking upgrade complete
**Reference**: Plan §9 (Success Criteria)

**Validation Checklist**:

**Technical Success**:
- [ ] (1) Project targets net10.0-windows
- [ ] (2) .NET 10.0 SDK verified functioning
- [ ] (3) System.Management 10.0.0 installed
- [ ] (4) System.Diagnostics.PerformanceCounter 10.0.0 installed
- [ ] (5) SharpHook installed and working
- [ ] (6) Zero security vulnerabilities

**Build Quality**:
- [ ] (7) Solution builds without errors
- [ ] (8) Solution builds without warnings (ideal)
- [ ] (9) All dependencies resolve correctly
- [ ] (10) Executable generated in correct path

**Runtime Quality**:
- [ ] (11) Application launches successfully
- [ ] (12) No unhandled exceptions during startup
- [ ] (13) Keyboard hooks working
- [ ] (14) Mouse hooks working
- [ ] (15) Performance counters working
- [ ] (16) System management queries working

**Functional Quality**:
- [ ] (17) All core features working
- [ ] (18) UI renders correctly
- [ ] (19) No regressions detected
- [ ] (20) Performance acceptable

**Stability**:
- [ ] (21) 30-60 minute stability test passed
- [ ] (22) Memory usage stable
- [ ] (23) CPU usage acceptable
- [ ] (24) Clean shutdown releases hooks

**Success Criteria**: All validation checks pass

---

## Rollback Procedure (If Needed)

**If critical issues discovered:**

1. **Immediate Rollback**:
   ```powershell
   cd C:\GitHub\MarkHazleton\KeyPressCounter
   git checkout main
   ```

2. **Verify Rollback**:
   ```powershell
   dotnet restore
   dotnet build
   # Test application
   ```

3. **Document Issues**:
   - Create detailed issue report
   - Include error messages and stack traces
   - Document reproduction steps

---

## Progress Dashboard
**Phase 0: Prerequisites**
- TASK-001: .NET 10.0 SDK .................. [?]
- TASK-001: .NET 10.0 SDK .................. [ ]
- TASK-002: global.json Check .............. [ ]

**Phase 1: Atomic Upgrade**
- TASK-003: Update Target Framework ........ [ ]
- TASK-004: Update Microsoft Packages ...... [ ]
- TASK-005: Replace with SharpHook ......... [ ]
- TASK-006: Refactor Code .................. [ ]
- TASK-007: Restore and Build .............. [ ]
- TASK-008: Fix Compilation Errors ......... [ ]

**Phase 2: Validation**
- TASK-009: Build Smoke Test ............... [ ]
- TASK-010: Launch Smoke Test .............. [ ]
- TASK-011: Hook Smoke Test ................ [ ]
- TASK-012: Functional Testing ............. [ ]
- TASK-013: Stability Testing .............. [ ]

**Phase 3: Finalization**
- TASK-014: Commit Changes ................. [ ]
- TASK-015: Final Validation ............... [ ]

---

## Notes

- **MouseKeyHook Decision**: User pre-approved SharpHook replacement
- **Strategy**: Big Bang single atomic upgrade
- **Branch**: upgrade-to-NET10
- **Estimated Duration**: 2-4 hours (SharpHook migration path)
- **Risk Level**: Medium (SharpHook API differences require code changes)
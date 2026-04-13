# Coffee Checkup Fixes - Deployment Scripts

This folder contains PowerShell scripts to deploy the coffee checkup bug fixes to another similar project.

## ?? Files

- **`Deploy-CoffeeCheckupFixes.ps1`** - Full-featured deployment script with backups and validation
- **`Quick-Deploy.ps1`** - Fast deployment of core files only

---

## ?? Quick Start

### No Parameters? No Problem!
Both scripts now show helpful information if run without parameters:

```powershell
# From parent folder (e.g., C:\SRC\ASP.net\)
.\Quick-Deploy.ps1

# Output:
# ?? Quick Deploy: Coffee Checkup Fixes
# No target project specified. Here's what's available:
# 
# ?? Available projects:
#   • OtherProject1
#   • OtherProject2
# 
# ?? Usage:
#   .\Quick-Deploy.ps1 -TargetProject "OtherProject1"
```

### Option 1: Quick Deploy (Fastest)
Copies only the 3 core code files + CHANGES_LOG.md

```powershell
# From parent folder (e.g., C:\SRC\ASP.net\)
.\Quick-Deploy.ps1 -TargetProject "OtherProjectName"
```

### Option 2: Full Deploy (Recommended)
Copies everything with backups and validation

```powershell
# From parent folder (e.g., C:\SRC\ASP.net\)
.\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "OtherProjectName"
```

---

## ?? Getting Help

Both scripts now include comprehensive help documentation:

```powershell
# Show detailed help for full script
Get-Help .\Deploy-CoffeeCheckupFixes.ps1 -Detailed

# Show examples
Get-Help .\Deploy-CoffeeCheckupFixes.ps1 -Examples

# Show quick help
Get-Help .\Quick-Deploy.ps1
```

---

## ?? Full Script Options

### Interactive Mode (New!)
Run without parameters to see available projects:

```powershell
.\Deploy-CoffeeCheckupFixes.ps1

# Shows:
# === Coffee Checkup Fixes Deployment Script ===
# No target project specified. Here's what's available:
# 
# ?? Available projects in current directory:
#   • Project1
#   • Project2
#   • Project3
# 
# ?? Usage Examples:
#   .\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "Project1"
#   .\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "Project1" -WhatIf
#   .\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "Project1" -SkipDocumentation
# 
# ?? For detailed help, run:
#   Get-Help .\Deploy-CoffeeCheckupFixes.ps1 -Detailed
```

### Basic Usage
```powershell
# Deploy everything
.\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "OtherProject"

# Preview what will be copied (WhatIf mode)
.\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "OtherProject" -WhatIf

# Prompt for confirmation before each file
.\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "OtherProject" -Confirm
```

### Advanced Options
```powershell
# Skip documentation files
.\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "OtherProject" -SkipDocumentation

# Skip diagnostic SQL queries
.\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "OtherProject" -SkipDiagnostics

# Skip both
.\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "OtherProject" -SkipDocumentation -SkipDiagnostics

# Don't create backups
.\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "OtherProject" -CreateBackup:$false

# Custom source project name
.\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "NewProject" -SourceProject "CustomTrackerDotNet"
```

---

## ?? Folder Structure Expected

```
C:\SRC\ASP.net\                          ? Run scripts from here
??? TrackerDotNet\                       ? Source project (your fixes)
?   ??? Classes\
?   ?   ??? DateCalculator.cs
?   ??? Controls\
?   ?   ??? ContactsThatMayNeedNextWeek.cs
?   ?   ??? SentRemindersLogTbl.cs
?   ??? App_Data\
?   ?   ??? SQLCommands-*.xml
?   ??? CHANGES_LOG.md
?   ??? ...
?
??? OtherProject\                        ? Target project (receives fixes)
?   ??? Classes\
?   ??? Controls\
?   ??? App_Data\
?   ??? ...
?
??? Deploy-CoffeeCheckupFixes.ps1        ? Full script
??? Quick-Deploy.ps1                     ? Quick script
```

---

## ?? What Gets Copied

### Core Code Files (Always)
? `Classes\DateCalculator.cs` - Monthly recurring fix + end-of-month edge case
? `Controls\ContactsThatMayNeedNextWeek.cs` - Delivery date filtering fix
? `Controls\SentRemindersLogTbl.cs` - New GetLastSuccessfulCheckupDate() method

### Documentation (Optional)
?? `CHANGES_LOG.md` - Complete change log
?? `DEVELOPMENT_SESSION_NOTES.md` - Quick reference
?? `DELIVERY_DATE_FIX_SUMMARY.md` - Delivery date fix details
?? `MONTHLY_RECURRING_END_OF_MONTH_BUG.md` - Bug analysis
?? `SESSION_SUMMARY.md` - Session overview
?? `WHY_RIDWAAN_RYAN_NOT_IN_CHECKUP.md` - Customer investigation
?? `COMMIT_MESSAGE.md` - Git commit template
?? `CHAT_RESTART_PROMPT.txt` - Restart instructions

### Diagnostic SQL Queries (Optional)
?? 10+ SQL diagnostic queries in `App_Data\`

---

## ?? Configuration Required

The target project needs these Web.config settings:

```xml
<appSettings>
    <add key="CoffeeCheckupMinMonthlyRecurringDays" value="20" />
    <add key="CoffeeCheckupReminderWindowDays" value="9" />
</appSettings>
```

The script will check for these and warn if missing.

---

## ?? Dependencies to Verify

After deployment, verify the target project has these:

### Classes/Constants:
- `SystemConstants.CheckupConstants.DefaultMinimumMonthlyRecurringDays`
- `SystemConstants.LogTypes.Orders`
- `SystemConstants.LogTypes.SendCheckup`

### Utility Classes:
- `AppLogger.WriteLog()`
- `TimeZoneUtils.Now()`
- `TrackerDb` class

### Database Tables:
- `SentRemindersLogTbl` (DateSentReminder, ReminderSent columns)
- `SysDataTbl` (MinReminderDate column)
- `ClientUsageTbl` (NextCoffeeBy column)
- `NextRoastDateByCityTbl` (NextDeliveryDate column)

---

## ?? Testing After Deployment

1. **Build the project:**
   ```powershell
   cd OtherProject
   MSBuild /t:Build /p:Configuration=Debug
   ```

2. **Run diagnostic queries:**
   - Open `App_Data\SQLCommands-VerifyDeliveryDateFix.xml`
   - Execute queries to verify fix

3. **Test with real data:**
   - Find a customer with monthly recurring order on day 1
   - Verify NextDateRequired calculates correctly
   - Check coffee checkup returns customers (not 0)

---

## ?? Backups

The full script creates automatic backups in:
```
OtherProject\backup_YYYYMMDD_HHMMSS\
```

To restore from backup:
```powershell
# Copy files back from backup folder
Copy-Item "OtherProject\backup_*\Classes\*.cs" "OtherProject\Classes\" -Force
```

---

## ?? Troubleshooting

### Script won't run
```powershell
# Enable script execution (one-time)
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
```

### Target project not found
```powershell
# List available folders
Get-ChildItem -Directory

# Make sure you're in the parent folder
cd C:\SRC\ASP.net\
```

### Files not copied
Check script output for errors. Common issues:
- Source files don't exist
- Target directories need creation (script handles this)
- Permission issues

---

## ?? Example Session

```powershell
PS C:\SRC\ASP.net> .\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "MyOtherProject"

=== Coffee Checkup Fixes Deployment Script ===
? Source: C:\SRC\ASP.net\TrackerDotNet
? Target: C:\SRC\ASP.net\MyOtherProject

=== Creating Backup ===
? Backup directory created: MyOtherProject\backup_20250121_143022

=== Copying Core Code Files (CRITICAL) ===
? Copied: Classes\DateCalculator.cs
         Monthly recurring date calculation fix + end-of-month edge case
? Copied: Controls\ContactsThatMayNeedNextWeek.cs
         Delivery date filtering using last checkup date
? Copied: Controls\SentRemindersLogTbl.cs
         Added GetLastSuccessfulCheckupDate() method

Core files: 3/3 copied

=== Copying Documentation ===
? Copied: CHANGES_LOG.md
...

=== Configuration Verification ===
? Checking Web.config for required settings...
? CoffeeCheckupMinMonthlyRecurringDays - Found
? CoffeeCheckupReminderWindowDays - Found

=== Deployment Summary ===

?? Files Copied:
  Core Code:       3/3
  Documentation:   7/7
  Diagnostics:     10/10

?? Backup Location:
  MyOtherProject\backup_20250121_143022

=== Next Steps ===
1. ? Files have been copied to target project
2. ? IMPORTANT - Verify dependencies exist
3. ?? Review Web.config settings
4. ?? Build the target project
5. ?? Test the fixes
...

=== Deployment Complete ===
? All requested files have been processed!
```

---

## ?? License & Credits

These scripts deploy fixes documented in CHANGES_LOG.md.
See that file for complete technical details.

---

## ?? Related Files

- `CHANGES_LOG.md` - Complete change documentation
- `DEVELOPMENT_SESSION_NOTES.md` - Quick reference guide
- `SESSION_SUMMARY.md` - Session overview

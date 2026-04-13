<#
.SYNOPSIS
    Deploys coffee checkup bug fixes from TrackerDotNet to another project

.DESCRIPTION
    Copies critical bug fixes for coffee checkup delivery date filtering and monthly recurring 
    orders from TrackerDotNet to another similar project. Includes automatic backups, 
    configuration validation, and comprehensive error checking.

.PARAMETER TargetProject
    Name of the target project folder (relative to current directory).
    If not specified, the script will list available projects.

.PARAMETER SourceProject
    Name of source project folder (default: TrackerDotNet)

.PARAMETER SkipDocumentation
    Skip copying documentation markdown files

.PARAMETER SkipDiagnostics
    Skip copying diagnostic SQL query files

.PARAMETER CreateBackup
    Create backup of existing files before copying (default: true)

.EXAMPLE
    .\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "OtherProject"
    
    Deploys all fixes, documentation, and diagnostics to OtherProject with backups

.EXAMPLE
    .\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "OtherProject" -WhatIf
    
    Preview what will be copied without making any changes

.EXAMPLE
    .\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "OtherProject" -SkipDocumentation -SkipDiagnostics
    
    Deploy only core code files, skip documentation and diagnostics

.EXAMPLE
    .\Deploy-CoffeeCheckupFixes.ps1 -TargetProject "OtherProject" -CreateBackup:$false
    
    Deploy without creating backups (not recommended)

.NOTES
    Author: TrackerDotNet Development Team
    Date: 2025-01-21
    Version: 1.0
    
    Run from PARENT folder of both projects:
    Example: If TrackerDotNet is at C:\SRC\ASP.net\TrackerDotNet
             Run from: C:\SRC\ASP.net\

.LINK
    See DEPLOYMENT_README.md for complete documentation
    See CHANGES_LOG.md for details on fixes
#>

[CmdletBinding(SupportsShouldProcess=$true)]
param(
    [Parameter(Mandatory=$false, HelpMessage="Name of target project folder (relative to current directory)")]
    [string]$TargetProject,
    
    [Parameter(Mandatory=$false, HelpMessage="Source project folder name (default: TrackerDotNet)")]
    [string]$SourceProject = "TrackerDotNet",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipDocumentation,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipDiagnostics,
    
    [Parameter(Mandatory=$false)]
    [switch]$CreateBackup = $true
)

# Color output helpers
function Write-Success { param($msg) Write-Host "✓ $msg" -ForegroundColor Green }
function Write-Info { param($msg) Write-Host "ℹ $msg" -ForegroundColor Cyan }
function Write-Warning { param($msg) Write-Host "⚠ $msg" -ForegroundColor Yellow }
function Write-Error { param($msg) Write-Host "✗ $msg" -ForegroundColor Red }
function Write-Header { param($msg) Write-Host "`n=== $msg ===" -ForegroundColor Magenta }

# If no target specified, show available options and help
if ([string]::IsNullOrEmpty($TargetProject)) {
    Write-Header "Coffee Checkup Fixes Deployment Script"
    Write-Host "`nNo target project specified. Here's what's available:" -ForegroundColor Yellow
    Write-Host "`n📁 Available projects in current directory:" -ForegroundColor Cyan
    
    $folders = Get-ChildItem $PSScriptRoot -Directory | Where-Object { $_.Name -ne $SourceProject }
    
    if ($folders.Count -eq 0) {
        Write-Error "No other project folders found in: $PSScriptRoot"
        Write-Info "`nMake sure you're running this from the parent folder of both projects."
        exit 1
    }
    
    $folders | ForEach-Object { Write-Host "  • $($_.Name)" -ForegroundColor White }
    
    Write-Host "`n📖 Usage Examples:" -ForegroundColor Cyan
    Write-Host "  .\Deploy-CoffeeCheckupFixes.ps1 -TargetProject `"$($folders[0].Name)`"" -ForegroundColor Gray
    Write-Host "  .\Deploy-CoffeeCheckupFixes.ps1 -TargetProject `"$($folders[0].Name)`" -WhatIf" -ForegroundColor Gray
    Write-Host "  .\Deploy-CoffeeCheckupFixes.ps1 -TargetProject `"$($folders[0].Name)`" -SkipDocumentation" -ForegroundColor Gray
    
    Write-Host "`n💡 For detailed help, run:" -ForegroundColor Cyan
    Write-Host "  Get-Help .\Deploy-CoffeeCheckupFixes.ps1 -Detailed" -ForegroundColor Gray
    
    exit 0
}

# Validate paths
$sourcePath = Join-Path $PSScriptRoot $SourceProject
$targetPath = Join-Path $PSScriptRoot $TargetProject

Write-Header "Coffee Checkup Fixes Deployment Script"
Write-Info "Source: $sourcePath"
Write-Info "Target: $targetPath"

if (-not (Test-Path $sourcePath)) {
    Write-Error "Source project not found at: $sourcePath"
    exit 1
}

if (-not (Test-Path $targetPath)) {
    Write-Error "Target project not found at: $targetPath"
    Write-Info "Available folders:"
    Get-ChildItem $PSScriptRoot -Directory | ForEach-Object { Write-Host "  - $($_.Name)" }
    exit 1
}

# Create backup if requested
if ($CreateBackup) {
    Write-Header "Creating Backup"
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $backupPath = Join-Path $targetPath "backup_$timestamp"
    
    if ($PSCmdlet.ShouldProcess($backupPath, "Create backup directory")) {
        New-Item -ItemType Directory -Path $backupPath -Force | Out-Null
        Write-Success "Backup directory created: $backupPath"
    }
}

# Define file mappings
$coreFiles = @(
    @{
        Source = "Classes\DateCalculator.cs"
        Target = "Classes\DateCalculator.cs"
        Critical = $true
        Description = "Monthly recurring date calculation fix + end-of-month edge case"
    },
    @{
        Source = "Controls\ContactsThatMayNeedNextWeek.cs"
        Target = "Controls\ContactsThatMayNeedNextWeek.cs"
        Critical = $true
        Description = "Delivery date filtering using last checkup date"
    },
    @{
        Source = "Controls\SentRemindersLogTbl.cs"
        Target = "Controls\SentRemindersLogTbl.cs"
        Critical = $true
        Description = "Added GetLastSuccessfulCheckupDate() method"
    }
)

$documentationFiles = @(
    "CHANGES_LOG.md",
    "DEVELOPMENT_SESSION_NOTES.md",
    "DELIVERY_DATE_FIX_SUMMARY.md",
    "MONTHLY_RECURRING_END_OF_MONTH_BUG.md",
    "SESSION_SUMMARY.md",
    "WHY_RIDWAAN_RYAN_NOT_IN_CHECKUP.md",
    "COMMIT_MESSAGE.md",
    "CHAT_RESTART_PROMPT.txt"
)

$diagnosticFiles = @(
    "App_Data\SQLCommands-VerifyDeliveryDateFix.xml",
    "App_Data\SQLCommands-DiagnoseCheckup.xml",
    "App_Data\SQLCommands-DiagnoseMonthlyRecurring.xml",
    "App_Data\SQLCommands-DiagnoseRidwaanRyan.xml",
    "App_Data\SQLCommands-WhyNoCheckup-RidwaanRyan.xml",
    "App_Data\SQLCommands-CheckNextRoastDates.xml",
    "App_Data\SQLCommands-DiagnoseNextRoastDates.xml",
    "App_Data\SQLCommands-FindMissing32.xml",
    "App_Data\SQLCommands-DiagnoseMissing6.xml",
    "App_Data\SQLCommands-NextCoffeeByReset.xml",
    "App_Data\TEST-RidwaanRyan.sql"
)

# Function to copy file with backup
function Copy-FileWithBackup {
    param(
        [string]$SourceFile,
        [string]$TargetFile,
        [string]$BackupDir,
        [string]$Description
    )
    
    $sourceFullPath = Join-Path $sourcePath $SourceFile
    $targetFullPath = Join-Path $targetPath $TargetFile
    
    if (-not (Test-Path $sourceFullPath)) {
        Write-Warning "Source file not found: $SourceFile (skipping)"
        return $false
    }
    
    # Create target directory if needed
    $targetDir = Split-Path $targetFullPath -Parent
    if (-not (Test-Path $targetDir)) {
        if ($PSCmdlet.ShouldProcess($targetDir, "Create directory")) {
            New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
        }
    }
    
    # Backup existing file
    if ((Test-Path $targetFullPath) -and $CreateBackup -and $BackupDir) {
        $backupFile = Join-Path $BackupDir $TargetFile
        $backupFileDir = Split-Path $backupFile -Parent
        
        if (-not (Test-Path $backupFileDir)) {
            New-Item -ItemType Directory -Path $backupFileDir -Force | Out-Null
        }
        
        if ($PSCmdlet.ShouldProcess($targetFullPath, "Backup existing file")) {
            Copy-Item $targetFullPath $backupFile -Force
            Write-Info "  Backed up: $TargetFile"
        }
    }
    
    # Copy new file
    if ($PSCmdlet.ShouldProcess($targetFullPath, "Copy file")) {
        Copy-Item $sourceFullPath $targetFullPath -Force
        Write-Success "Copied: $TargetFile"
        if ($Description) {
            Write-Host "         $Description" -ForegroundColor Gray
        }
        return $true
    }
    
    return $false
}

# Copy core files
Write-Header "Copying Core Code Files (CRITICAL)"
$coreSuccess = 0
$coreTotal = $coreFiles.Count

foreach ($file in $coreFiles) {
    if (Copy-FileWithBackup -SourceFile $file.Source -TargetFile $file.Target -BackupDir $backupPath -Description $file.Description) {
        $coreSuccess++
    }
}

Write-Host "`nCore files: $coreSuccess/$coreTotal copied" -ForegroundColor $(if ($coreSuccess -eq $coreTotal) { "Green" } else { "Yellow" })

# Copy documentation
if (-not $SkipDocumentation) {
    Write-Header "Copying Documentation"
    $docSuccess = 0
    $docTotal = $documentationFiles.Count
    
    foreach ($file in $documentationFiles) {
        if (Copy-FileWithBackup -SourceFile $file -TargetFile $file -BackupDir $backupPath -Description "") {
            $docSuccess++
        }
    }
    
    Write-Host "`nDocumentation files: $docSuccess/$docTotal copied" -ForegroundColor $(if ($docSuccess -eq $docTotal) { "Green" } else { "Yellow" })
} else {
    Write-Warning "Documentation files skipped (use without -SkipDocumentation to include)"
}

# Copy diagnostic tools
if (-not $SkipDiagnostics) {
    Write-Header "Copying Diagnostic SQL Queries"
    $diagSuccess = 0
    $diagTotal = $diagnosticFiles.Count
    
    foreach ($file in $diagnosticFiles) {
        if (Copy-FileWithBackup -SourceFile $file -TargetFile $file -BackupDir $backupPath -Description "") {
            $diagSuccess++
        }
    }
    
    Write-Host "`nDiagnostic files: $diagSuccess/$diagTotal copied" -ForegroundColor $(if ($diagSuccess -eq $diagTotal) { "Green" } else { "Yellow" })
} else {
    Write-Warning "Diagnostic files skipped (use without -SkipDiagnostics to include)"
}

# Configuration check
Write-Header "Configuration Verification"
$targetWebConfig = Join-Path $targetPath "Web.config"

if (Test-Path $targetWebConfig) {
    Write-Info "Checking Web.config for required settings..."
    $webConfigContent = Get-Content $targetWebConfig -Raw
    
    $requiredSettings = @(
        @{ Key = "CoffeeCheckupMinMonthlyRecurringDays"; DefaultValue = "20"; Found = $false },
        @{ Key = "CoffeeCheckupReminderWindowDays"; DefaultValue = "9"; Found = $false }
    )
    
    foreach ($setting in $requiredSettings) {
        if ($webConfigContent -match "key=`"$($setting.Key)`"") {
            Write-Success "$($setting.Key) - Found"
            $setting.Found = $true
        } else {
            Write-Warning "$($setting.Key) - NOT FOUND (default: $($setting.DefaultValue))"
        }
    }
    
    # Generate config additions if needed
    $missingSettings = $requiredSettings | Where-Object { -not $_.Found }
    if ($missingSettings.Count -gt 0) {
        Write-Warning "`nMissing configuration settings. Add these to Web.config <appSettings>:"
        foreach ($setting in $missingSettings) {
            Write-Host "  <add key=`"$($setting.Key)`" value=`"$($setting.DefaultValue)`" />" -ForegroundColor Yellow
        }
    }
} else {
    Write-Warning "Web.config not found in target project - manual configuration required"
}

# Summary Report
Write-Header "Deployment Summary"

Write-Host "`n📊 Files Copied:" -ForegroundColor Cyan
Write-Host "  Core Code:       $coreSuccess/$coreTotal" -ForegroundColor $(if ($coreSuccess -eq $coreTotal) { "Green" } else { "Red" })
if (-not $SkipDocumentation) {
    Write-Host "  Documentation:   $docSuccess/$docTotal" -ForegroundColor Green
}
if (-not $SkipDiagnostics) {
    Write-Host "  Diagnostics:     $diagSuccess/$diagTotal" -ForegroundColor Green
}

if ($CreateBackup -and $backupPath -and (Test-Path $backupPath)) {
    Write-Host "`n💾 Backup Location:" -ForegroundColor Cyan
    Write-Host "  $backupPath" -ForegroundColor Gray
}

# Next steps
Write-Header "Next Steps"
Write-Host @"
1. ✓ Files have been copied to target project

2. ⚠ IMPORTANT - Verify these dependencies exist in target project:
   - SystemConstants.CheckupConstants.DefaultMinimumMonthlyRecurringDays
   - AppLogger.WriteLog()
   - TimeZoneUtils.Now()
   - SysDataTbl.GetMinReminderDate()
   
3. 📋 Review Web.config settings (see warnings above)

4. 🔨 Build the target project:
   cd "$targetPath"
   MSBuild /t:Build /p:Configuration=Debug

5. 🧪 Test the fixes:
   - Run diagnostic queries from App_Data folder
   - Test with a customer like Ridwaan (recurring order on day 1)
   - Verify checkup returns customers (not 0)

6. 📝 Update version/release notes in target project

7. 🚀 Deploy to test environment before production

"@ -ForegroundColor White

Write-Header "Deployment Complete"
Write-Success "All requested files have been processed!"

# Return status
if ($coreSuccess -eq $coreTotal) {
    exit 0
} else {
    Write-Error "Not all core files were copied successfully"
    exit 1
}

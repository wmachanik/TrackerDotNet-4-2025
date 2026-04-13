<#
.SYNOPSIS
    Quick deployment of coffee checkup bug fixes

.DESCRIPTION
    Fast deployment script that copies only the 3 critical code files plus CHANGES_LOG.md
    from TrackerDotNet to another project. Minimal options for maximum speed.

.PARAMETER TargetProject
    Name of the target project folder (relative to current directory).
    If not specified, the script will list available projects.

.PARAMETER SourceProject
    Name of source project folder (default: TrackerDotNet)

.EXAMPLE
    .\Quick-Deploy.ps1 -TargetProject "OtherProject"
    
    Deploys core fixes to OtherProject

.EXAMPLE
    .\Quick-Deploy.ps1
    
    Shows available projects and usage examples

.NOTES
    Author: TrackerDotNet Development Team
    Date: 2025-01-21
    Version: 1.0
    
    Copies only:
    - Classes\DateCalculator.cs
    - Controls\ContactsThatMayNeedNextWeek.cs
    - Controls\SentRemindersLogTbl.cs
    - CHANGES_LOG.md
    
    For full deployment with backups and diagnostics, use Deploy-CoffeeCheckupFixes.ps1

.LINK
    See DEPLOYMENT_README.md for complete documentation
#>

param(
    [Parameter(Mandatory=$false, HelpMessage="Name of target project folder")]
    [string]$TargetProject,
    
    [Parameter(Mandatory=$false, HelpMessage="Source project folder name (default: TrackerDotNet)")]
    [string]$SourceProject = "TrackerDotNet"
)

# If no target specified, show available options
if ([string]::IsNullOrEmpty($TargetProject)) {
    Write-Host "`n🚀 Quick Deploy: Coffee Checkup Fixes" -ForegroundColor Cyan
    Write-Host "No target project specified. Here's what's available:`n" -ForegroundColor Yellow
    
    $folders = Get-ChildItem $PSScriptRoot -Directory | Where-Object { $_.Name -ne $SourceProject }
    
    if ($folders.Count -eq 0) {
        Write-Host "✗ No other project folders found in: $PSScriptRoot" -ForegroundColor Red
        Write-Host "  Make sure you're running this from the parent folder of both projects.`n" -ForegroundColor Gray
        exit 1
    }
    
    Write-Host "📁 Available projects:" -ForegroundColor Cyan
    $folders | ForEach-Object { Write-Host "  • $($_.Name)" -ForegroundColor White }
    
    Write-Host "`n📖 Usage:" -ForegroundColor Cyan
    Write-Host "  .\Quick-Deploy.ps1 -TargetProject `"$($folders[0].Name)`"`n" -ForegroundColor Gray
    
    Write-Host "💡 Tip: For full deployment with backups, use Deploy-CoffeeCheckupFixes.ps1`n" -ForegroundColor Yellow
    
    exit 0
}

$source = $SourceProject

# Validate source exists
if (-not (Test-Path $source)) {
    Write-Host "`n✗ Source project not found: $source" -ForegroundColor Red
    Write-Host "  Current directory: $PSScriptRoot`n" -ForegroundColor Gray
    exit 1
}

# Validate target exists
if (-not (Test-Path $TargetProject)) {
    Write-Host "`n✗ Target project not found: $TargetProject" -ForegroundColor Red
    Write-Host "`n📁 Available projects:" -ForegroundColor Cyan
    Get-ChildItem $PSScriptRoot -Directory | Where-Object { $_.Name -ne $source } | ForEach-Object { 
        Write-Host "  • $($_.Name)" -ForegroundColor White 
    }
    Write-Host ""
    exit 1
}

$files = @(
    "Classes\DateCalculator.cs",
    "Controls\ContactsThatMayNeedNextWeek.cs",
    "Controls\SentRemindersLogTbl.cs",
    "CHANGES_LOG.md"
)

Write-Host "`n🚀 Quick Deploy: Coffee Checkup Fixes" -ForegroundColor Cyan
Write-Host "Source: $source → Target: $TargetProject`n" -ForegroundColor Gray

$successCount = 0
$totalCount = $files.Count

foreach ($file in $files) {
    $src = Join-Path $source $file
    $dst = Join-Path $TargetProject $file
    
    if (Test-Path $src) {
        $dstDir = Split-Path $dst -Parent
        if (-not (Test-Path $dstDir)) {
            New-Item -ItemType Directory -Path $dstDir -Force | Out-Null
        }
        
        Copy-Item $src $dst -Force
        Write-Host "✓ $file" -ForegroundColor Green
        $successCount++
    } else {
        Write-Host "✗ $file (not found)" -ForegroundColor Red
    }
}

if ($successCount -eq $totalCount) {
    Write-Host "`n✓ All core files deployed successfully! ($successCount/$totalCount)" -ForegroundColor Green
} else {
    Write-Host "`n⚠ Partial deployment: $successCount/$totalCount files copied" -ForegroundColor Yellow
}

Write-Host "`n📋 Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Build: cd $TargetProject && MSBuild /t:Build" -ForegroundColor Gray
Write-Host "  2. Test the fixes (see CHANGES_LOG.md)" -ForegroundColor Gray
Write-Host "  3. Review Web.config for required settings`n" -ForegroundColor Gray

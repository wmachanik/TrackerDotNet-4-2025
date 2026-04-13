# Fix-SmartAppControl.ps1
# Adds TrackerDotNet to Windows Defender exclusions
# Run as Administrator

<#
.SYNOPSIS
    Fixes Smart App Control blocking TrackerDotNet during development

.DESCRIPTION
    Adds the TrackerDotNet bin folder to Windows Defender exclusions so Smart App Control
    doesn't block unsigned development builds.

.NOTES
    - Must run as Administrator
    - For development use only
    - For production, use proper code signing
#>

# Check if running as admin
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "? This script must be run as Administrator" -ForegroundColor Red
    Write-Host "`nTo run as admin:" -ForegroundColor Yellow
    Write-Host "  1. Right-click PowerShell" -ForegroundColor Gray
    Write-Host "  2. Select 'Run as Administrator'" -ForegroundColor Gray
    Write-Host "  3. Run this script again`n" -ForegroundColor Gray
    exit 1
}

Write-Host "`n?? TrackerDotNet - Smart App Control Fix" -ForegroundColor Cyan
Write-Host "=" * 50 -ForegroundColor Gray

# Get project root
$projectRoot = $PSScriptRoot
$binPath = Join-Path $projectRoot "bin"

Write-Host "`nProject: $projectRoot" -ForegroundColor White
Write-Host "Bin folder: $binPath`n" -ForegroundColor White

# Add exclusions
try {
    Write-Host "Adding Windows Defender exclusions..." -ForegroundColor Cyan
    
    # Add bin folder
    Add-MpPreference -ExclusionPath $binPath
    Write-Host "? Added: $binPath" -ForegroundColor Green
    
    # Add project root (optional but recommended for development)
    Add-MpPreference -ExclusionPath $projectRoot
    Write-Host "? Added: $projectRoot" -ForegroundColor Green
    
    Write-Host "`n? Success! TrackerDotNet is now excluded from Smart App Control`n" -ForegroundColor Green
    
} catch {
    Write-Host "`n? Error adding exclusions: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "`nTry manually:" -ForegroundColor Yellow
    Write-Host "  1. Open Windows Security" -ForegroundColor Gray
    Write-Host "  2. Go to Virus & threat protection > Manage settings" -ForegroundColor Gray
    Write-Host "  3. Scroll to Exclusions > Add or remove exclusions" -ForegroundColor Gray
    Write-Host "  4. Add folder: $binPath`n" -ForegroundColor Gray
    exit 1
}

# Check current exclusions
Write-Host "Current exclusions:" -ForegroundColor Cyan
$exclusions = Get-MpPreference | Select-Object -ExpandProperty ExclusionPath
$exclusions | Where-Object { $_ -like "*TrackerDotNet*" } | ForEach-Object {
    Write-Host "  • $_" -ForegroundColor Gray
}

Write-Host "`n??  Note: This is for development only!" -ForegroundColor Yellow
Write-Host "   For production, use proper code signing.`n" -ForegroundColor Gray

Write-Host "?? Next steps:" -ForegroundColor Cyan
Write-Host "  1. Rebuild your project" -ForegroundColor White
Write-Host "  2. Run the application" -ForegroundColor White
Write-Host "  3. Smart App Control should no longer block it`n" -ForegroundColor White

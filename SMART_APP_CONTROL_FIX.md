# Smart App Control Fix - TrackerDotNet

## ?? Problem

Windows Smart App Control blocks part of TrackerDotNet because some assemblies/DLLs are not digitally signed.

**Error Message:**
```
Smart App Control has blocked part of this app
```

## ?? Quick Fix (Development)

### Run the Fix Script (Recommended)

1. **Open PowerShell as Administrator:**
   - Right-click PowerShell
   - Select "Run as Administrator"

2. **Navigate to project and run:**
   ```powershell
   cd C:\SRC\ASP.net\TrackerDotNet
   .\Fix-SmartAppControl.ps1
   ```

3. **Rebuild and run** - Smart App Control should no longer block

---

## ?? Alternative Solutions

### Option 1: Manual Windows Defender Exclusion

1. Open **Windows Security**
2. Go to **Virus & threat protection** > **Manage settings**
3. Scroll to **Exclusions** > **Add or remove exclusions**
4. Click **Add an exclusion** > **Folder**
5. Add: `C:\SRC\ASP.net\TrackerDotNet\bin`

### Option 2: Disable Smart App Control Temporarily

?? **Warning:** Cannot be re-enabled without clean Windows reinstall!

1. Open **Windows Security**
2. Go to **App & browser control**
3. Click **Smart App Control settings**
4. Turn off Smart App Control

### Option 3: Check Which File is Blocked

1. Open **Windows Security**
2. Go to **Virus & threat protection** > **Protection history**
3. Look for "Smart App Control" or "Blocked app" entries
4. Note the specific file being blocked

Common blocked files in TrackerDotNet:
- `Microsoft.ACE.OLEDB.*.dll` (Access database driver)
- `MailKit.dll` or related
- `BouncyCastle.Cryptography.dll`

---

## ?? Production Solution (Proper Code Signing)

For production deployment, you should **sign your assemblies**.

### Step 1: Get a Code Signing Certificate

**Option A: Commercial Certificate** (recommended)
- Purchase from DigiCert, Sectigo, etc.
- Cost: ~$200-500/year
- Trusted by Windows by default

**Option B: Self-Signed** (development only)
```powershell
# Run as Administrator
$cert = New-SelfSignedCertificate `
    -Subject "CN=TrackerDotNet" `
    -Type CodeSigningCert `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -NotAfter (Get-Date).AddYears(5)

# Export certificate
$pwd = ConvertTo-SecureString -String "YourPassword" -Force -AsPlainText
Export-PfxCertificate -Cert $cert `
    -FilePath "C:\Certs\TrackerDotNet.pfx" `
    -Password $pwd

# Install to Trusted Root
Import-PfxCertificate `
    -FilePath "C:\Certs\TrackerDotNet.pfx" `
    -CertStoreLocation "Cert:\LocalMachine\Root" `
    -Password $pwd
```

### Step 2: Sign Your Assembly

Add post-build event to `TrackerDotNet.csproj`:

```xml
<Target Name="SignAssembly" AfterTargets="Build">
  <Exec Condition="Exists('C:\Certs\TrackerDotNet.pfx')" 
        Command="signtool sign /f &quot;C:\Certs\TrackerDotNet.pfx&quot; /p YourPassword /t http://timestamp.digicert.com &quot;$(TargetPath)&quot;" />
</Target>
```

Or use strong name signing:

```xml
<PropertyGroup>
  <SignAssembly>true</SignAssembly>
  <AssemblyOriginatorKeyFile>TrackerDotNet.snk</AssemblyOriginatorKeyFile>
</PropertyGroup>
```

Generate SNK file:
```powershell
sn -k TrackerDotNet.snk
```

---

## ?? Testing the Fix

After applying the fix:

1. **Clean and rebuild:**
   ```powershell
   MSBuild TrackerDotNet.csproj /t:Clean
   MSBuild TrackerDotNet.csproj /t:Build /p:Configuration=Debug
   ```

2. **Run the application**

3. **If still blocked:**
   - Check Protection History (Windows Security > Virus & threat protection > Protection history)
   - Find the specific blocked file
   - Add that file/folder to exclusions

---

## ?? Troubleshooting

### Issue: Script won't run
```powershell
# Enable script execution
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
```

### Issue: "Access Denied" when adding exclusion
- Make sure PowerShell is running **as Administrator**
- Check Windows Security isn't managed by group policy

### Issue: Still getting blocked after exclusion
- Restart Visual Studio
- Clean and rebuild solution
- Restart Windows

### Issue: Need to find which DLL is blocked
```powershell
# Check Windows Event Log
Get-WinEvent -FilterHashtable @{
    LogName='Microsoft-Windows-Security-SPP/Operational'
    Id=903
} -MaxEvents 10 | Format-List
```

---

## ?? Security Implications

### Development Exclusions
- ? Safe for development machines
- ?? Only excludes your project folder
- ?? Other apps still protected

### Disabling Smart App Control
- ? Not recommended
- ? Cannot be re-enabled without clean Windows install
- ? Reduces overall system security

### Proper Code Signing
- ? Best for production
- ? Maintains system security
- ? Users trust signed apps

---

## ?? Related Files

- `Fix-SmartAppControl.ps1` - Auto-fix script
- `TrackerDotNet.csproj` - Project file
- `Web.config` - Application configuration

---

## ?? References

- [Microsoft Docs: Smart App Control](https://support.microsoft.com/en-us/topic/what-is-smart-app-control-285ea03d-fa88-4d56-882e-6698afdb7003)
- [Code Signing Best Practices](https://docs.microsoft.com/en-us/windows/win32/seccrypto/cryptography-tools)
- [Windows Defender Exclusions](https://support.microsoft.com/en-us/windows/add-an-exclusion-to-windows-security-811816c0-4dfd-af4a-47e4-c301afe13b26)

---

## ? Summary

**For Development (Quick Fix):**
```powershell
# Run as Administrator
.\Fix-SmartAppControl.ps1
```

**For Production (Proper Fix):**
- Get code signing certificate
- Sign assemblies in build process
- Deploy signed application

**Emergency (Not Recommended):**
- Disable Smart App Control in Windows Security

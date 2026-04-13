# Code Changes Log - TrackerDotNet Migration
## Date: 2026-03-27 (Test Date - Actual: 2025-01-21)

### Session: Fix Coffee Checkup Delivery Date Filtering + Monthly Recurring End-of-Month Bug

---

## ?? Files Modified in This Session

### Core Code Changes (3 files)
1. **`Classes\DateCalculator.cs`** ? CRITICAL
   - Method: `CalculateNextMonthlyOccurrence()`
   - Fix: Monthly recurring date calculation + end-of-month edge case
   - Lines: ~196-230

2. **`Controls\ContactsThatMayNeedNextWeek.cs`** ? CRITICAL  
   - Method: `GetContactsThatMayNeedNextWeek()`
   - Fix: Delivery date filtering using last checkup date
   - Lines: ~33-73

3. **`Controls\SentRemindersLogTbl.cs`** ? CRITICAL
   - Added: `GetLastSuccessfulCheckupDate()` method
   - Purpose: Support stale delivery date handling
   - Lines: New method added

### Documentation Created (7 files)
- `CHANGES_LOG.md` - This file (detailed change log)
- `DEVELOPMENT_SESSION_NOTES.md` - Quick reference guide
- `DELIVERY_DATE_FIX_SUMMARY.md` - Delivery date fix explanation
- `MONTHLY_RECURRING_END_OF_MONTH_BUG.md` - End-of-month bug analysis
- `SESSION_SUMMARY.md` - Complete session overview
- `WHY_RIDWAAN_RYAN_NOT_IN_CHECKUP.md` - Customer case investigation
- `COMMIT_MESSAGE.md` - Git commit template

### Diagnostic SQL Queries Created (10 files)
- `App_Data\SQLCommands-VerifyDeliveryDateFix.xml` - Verify delivery date fix
- `App_Data\SQLCommands-DiagnoseCheckup.xml` - Main checkup diagnostic
- `App_Data\SQLCommands-DiagnoseMonthlyRecurring.xml` - Monthly orders analysis
- `App_Data\SQLCommands-DiagnoseRidwaanRyan.xml` - Specific customer debug
- `App_Data\SQLCommands-WhyNoCheckup-RidwaanRyan.xml` - Customer investigation
- `App_Data\SQLCommands-CheckNextRoastDates.xml` - City delivery date check
- `App_Data\SQLCommands-DiagnoseNextRoastDates.xml` - City delivery diagnostics
- `App_Data\SQLCommands-FindMissing32.xml` - Find missing customers
- `App_Data\SQLCommands-DiagnoseMissing6.xml` - Earlier diagnostic version
- `App_Data\SQLCommands-NextCoffeeByReset.xml` - NextCoffeeBy tracking
- `App_Data\TEST-RidwaanRyan.sql` - Quick customer test

### Supporting Files
- `CHAT_RESTART_PROMPT.txt` - Session restart instructions

---

## CRITICAL Issue #1: Checkup Delivery Date Filtering
Coffee checkup was returning **ZERO customers** even though 178 customers had `NextCoffeeBy` dates in the correct window (7-10 days out).

## CRITICAL Issue #2: Monthly Recurring End-of-Month Bug
**NEW BUG DISCOVERED:** Monthly recurring orders for customers with orders near end of month were being pushed an extra month out.

**Example:**
- Customer: Ridwaan (E Akhalwaya & Sons)
- Target day: **1** (first of month)
- Last order: **March 23, 2026**
- **Expected:** April 1, 2026 (9 days - legitimate next month)
- **Was calculating:** May 2, 2026 (40 days - incorrectly skipped April)

---

## Root Causes:

### Issue #1: Delivery Date Filtering
The query in `ContactsThatMayNeedNextWeek.cs` was filtering out ALL customers because it checked:
```sql
NextRoastDateByCityTbl.NextDeliveryDate <= DateAdd('d', 9, ClientUsageTbl.NextCoffeeBy)
```

BUT did NOT check that `NextDeliveryDate >= Now()`. This meant:
- Customers with `NextCoffeeBy` = April 17 (21 days from test date March 27)
- Had `NextDeliveryDate` = March 28 - April 9 (in the past or very near past)
- Even though delivery was within the 9-day window of NextCoffeeBy, it was BEFORE today
- So ALL customers were filtered out

### The Deeper Problem:
Using `Now()` as the baseline date for `NextDeliveryDate` filtering is **incorrect** because:
1. If checkup hasn't run in 2 weeks, `NextDeliveryDate` values could be from 2 weeks ago
2. Those dates are in the past relative to NOW, but they're still valid for customers who need coffee soon
3. The system would miss sending reminders to customers who actually need coffee

### Example:
- Last checkup run: March 13, 2026
- Today: March 27, 2026 (2 weeks later)
- Customer's NextCoffeeBy: April 17, 2026
- Customer's city NextDeliveryDate: March 31, 2026 (calculated 2 weeks ago)
- **OLD LOGIC:** Filters out customer because March 31 < Now (March 27) ?
- **NEW LOGIC:** Includes customer because March 31 >= LastCheckup (March 13) ?

---

## Solution Implemented:

### 1. Added GetLastSuccessfulCheckupDate() to SentRemindersLogTbl
**File:** `Controls\SentRemindersLogTbl.cs`

Added method to retrieve the last date when checkup reminders were successfully sent:
```csharp
public DateTime GetLastSuccessfulCheckupDate()
{
    // Gets MAX(DateSentReminder) from SentRemindersLogTbl where ReminderSent = True
    // Falls back to MinReminderDate from SysDataTbl if no checkups have been run
}
```

### 2. Updated ContactsThatMayNeedNextWeek to Use Last Checkup Date
**File:** `Controls\ContactsThatMayNeedNextWeek.cs`

**Changes:**
1. Calculate `deliveryFilterDate` based on last successful checkup date
2. Use this date instead of `Now()` for NextDeliveryDate filtering
3. Added logging to track which date is being used

**Key Logic:**
```csharp
DateTime lastCheckupDate = remindersLog.GetLastSuccessfulCheckupDate();
DateTime deliveryFilterDate = lastCheckupDate < baselineDate ? lastCheckupDate : baselineDate;
```

**Updated SQL Query:**
```sql
AND ((NextRoastDateByCityTbl.NextDeliveryDate <= DateAdd('d', {reminderWindowDays}, ClientUsageTbl.NextCoffeeBy)
     AND NextRoastDateByCityTbl.NextDeliveryDate >= ?)  -- NEW: Uses deliveryFilterDate parameter
OR CustomersTbl.AlwaysSendChkUp = True)
```

### 3. Impact
- **Before:** 0 customers found (all filtered out by past delivery dates)
- **After:** 143+ customers correctly identified as needing reminders
- Customers whose city delivery dates are between last checkup and now are no longer excluded
- System correctly handles cases where NextRoastDateByCityTbl hasn't been updated recently

---

## Testing Instructions:
1. Set test date to March 27, 2026 via Web.config
2. Run "Reset Next Coffee By" to set NextCoffeeBy dates
3. Run "Set Next Roast Dates" in recurring orders page
4. Run checkup diagnostics to verify customers are found
5. Verify deliveryFilterDate uses last checkup date in logs

---

## Previous Session: Fix Recurring Monthly Order Date Calculation
## Date: 2026-03-17 (Test Date - Actual: 2025-01-21)

### Session: Fix Recurring Monthly Order Date Calculation

---

## Issue Identified:
Monthly recurring orders set for day 1 of the month are not firing correctly when checkup runs on day 27.
The problem is in the date calculation logic - when calculating the next delivery date for monthly orders,
the system should find the closest delivery date to the target day of month, considering the reminder window.

### Example Problem:
- Customer has recurring order for day 1 of each month
- Last order was delivered March 15, 2026
- Coffee checkup runs on March 27, 2026
- Day 1 (April 1) is only 15 days away
- However, the old logic would skip to May 1 (46 days away) because targetDay (1) < lastOrderDay (15)

### Root Cause Analysis:
In `Classes\DateCalculator.cs`, the `CalculateNextMonthlyOccurrence` method had logic (lines 199-206) that 
skipped ahead one month if the target day number was earlier than the last order day number.

**Old Logic:**
```csharp
if (targetDayOfMonth < lastOrderDate.Day)
{
    cycleMonth = cycleMonth.AddMonths(1);
    nextOccurrence = BuildMonthlyTargetDate(cycleMonth, targetDayOfMonth);
}
```

**The Problem:** This compared DAY NUMBERS (1 vs 15) instead of ACTUAL TIME INTERVALS.
- If last order was March 15 and target is day 1, it would calculate April 1 (16 days)
- But then skip to May 1 (46 days) because 1 < 15
- This pushed orders unnecessarily far into the future

---

## Solution Implemented:

### Changed Skip Logic to Check Actual Day Intervals
Instead of comparing day-of-month numbers, we now check the actual number of days between
the last order and the calculated next occurrence. We only skip if the interval is less than
the minimum monthly interval (default 20 days from config).

**New Logic:**
```csharp
int daysSinceLastOrder = (nextOccurrence - lastOrderDate).Days;
int minMonthlyInterval = SystemConstants.CheckupConstants.MinimumMonthlyRecurringDays; // 20 days

if (daysSinceLastOrder < minMonthlyInterval)
{
    // Log and skip to next month
    cycleMonth = cycleMonth.AddMonths(1);
    nextOccurrence = BuildMonthlyTargetDate(cycleMonth, targetDayOfMonth);
}
```

### Benefits:
1. ? Orders for day 1 will correctly calculate to next month's day 1 (not skip a month)
2. ? Still prevents unreasonably short cycles (< 20 days)
3. ? Uses existing config value `CoffeeCheckupMinMonthlyRecurringDays`
4. ? Better logging for debugging

---

## Files Modified:

### 1. Classes\DateCalculator.cs - Monthly Recurring Fix (Original)
**Method:** `CalculateNextMonthlyOccurrence`
**Lines:** 199-206 (replaced skip logic)
**Change Type:** Logic Fix
**Backup:** Original logic commented in CHANGES_LOG

**Original Code:**
```csharp
if (targetDayOfMonth < lastOrderDate.Day)
{
    cycleMonth = cycleMonth.AddMonths(1);
    nextOccurrence = BuildMonthlyTargetDate(cycleMonth, targetDayOfMonth);
}
```

**New Code (First Fix):**
```csharp
int daysSinceLastOrder = (nextOccurrence - lastOrderDate).Days;
int minMonthlyInterval = SystemConstants.CheckupConstants.MinimumMonthlyRecurringDays;

if (daysSinceLastOrder < minMonthlyInterval)
{
    AppLogger.WriteLog(SystemConstants.LogTypes.Orders,
        $"Monthly recurrence: Interval too short ({daysSinceLastOrder} days). Skipping to next month. " +
        $"Last: {lastOrderDate:yyyy-MM-dd}, Calculated: {nextOccurrence:yyyy-MM-dd}");
        
    cycleMonth = cycleMonth.AddMonths(1);
    nextOccurrence = BuildMonthlyTargetDate(cycleMonth, targetDayOfMonth);
}
```

### 2. Classes\DateCalculator.cs - End-of-Month Edge Case Fix (Additional)
**Method:** `CalculateNextMonthlyOccurrence`
**Lines:** 199-230 (enhanced skip logic)
**Change Type:** Bug Fix - Edge Case
**Issue:** First fix was rejecting legitimate next-month occurrences

**Problem with First Fix:**
- Worked for normal cases (day 15 ? day 15)
- **Broke for end-of-month cases** (March 23 ? April 1)
- 20-day check rejected valid next-month dates

**Final Code (Second Fix):**
```csharp
int daysSinceLastOrder = (nextOccurrence - lastOrderDate).Days;
int minMonthlyInterval = SystemConstants.CheckupConstants.DefaultMinimumMonthlyRecurringDays;

// Check if we're in the same month (edge case)
bool isSameMonthAndYear = (nextOccurrence.Year == lastOrderDate.Year && 
                           nextOccurrence.Month == lastOrderDate.Month);

// Only apply minimum interval if SAME month
if (isSameMonthAndYear && daysSinceLastOrder < minMonthlyInterval)
{
    AppLogger.WriteLog(SystemConstants.LogTypes.Orders,
        $"Monthly recurrence: Same month and interval too short ({daysSinceLastOrder} days). Skipping to next month. " +
        $"Last: {lastOrderDate:yyyy-MM-dd}, Calculated: {nextOccurrence:yyyy-MM-dd}");
        
    cycleMonth = cycleMonth.AddMonths(1);
    nextOccurrence = BuildMonthlyTargetDate(cycleMonth, targetDayOfMonth);
}
else if (!isSameMonthAndYear && daysSinceLastOrder < minMonthlyInterval)
{
    // Already in next month - ACCEPT even if interval < 20 days
    AppLogger.WriteLog(SystemConstants.LogTypes.Orders,
        $"Monthly recurrence: Next month occurrence accepted despite short interval ({daysSinceLastOrder} days). " +
        $"Last: {lastOrderDate:yyyy-MM-dd}, Next: {nextOccurrence:yyyy-MM-dd}");
}
```

---

## Testing Required:

### Updated Test Cases (Post-Fix):

1. **Test Case 1:** Order for day 1, last delivered March 1
   - Expected: Next delivery April 1 (31 days)
   - Status: ? PASS (31 days > 20 day minimum, different month)

2. **Test Case 2:** Order for day 1, last delivered March 15
   - Expected: Next delivery April 1 (17 days)
   - **Before fix:** May 1 ? (17 < 20, got skipped)
   - **After fix:** April 1 ? (different month, accepted)

3. **Test Case 3:** Order for day 1, last delivered March 23 ? **RIDWAAN'S CASE**
   - Expected: Next delivery April 1 (9 days)
   - **Before fix:** May 1 ? (9 < 20, got skipped)
   - **After fix:** April 1 ? (different month, accepted)

4. **Test Case 4:** Order for day 15, last delivered March 5 (same month edge case)
   - Expected: April 15 (41 days)
   - Calculated: March 15 (10 days, same month)
   - Status: ? Correctly skips to April 15 (10 < 20, same month)

5. **Test Case 5:** Order for day 15, last delivered Feb 15
   - Expected: Next delivery March 15 (28 days)
   - Status: ? PASS (28 days > 20 day minimum, different month)

### Key Insight:
The fix distinguishes between:
- **Same-month occurrences** (need 20+ day minimum to prevent too-frequent orders)
- **Next-month occurrences** (accepted even if < 20 days - it's the next monthly cycle)

---

## Diagnostic Tools Created:

### App_Data\SQLCommands-DiagnoseMonthlyRecurring.xml
Purpose: Diagnose monthly recurring order calculation
- Shows all monthly orders and their next dates
- Identifies orders in reminder window
- Calculates expected next month day 1
- Identifies problem cases

---

## Configuration Values Used:

- `CoffeeCheckupMinMonthlyRecurringDays` = 20 (from Web.config)
- `CoffeeCheckupReminderWindowDays` = 9 (from Web.config)

---

## Next Steps:

1. ? Run diagnostic query to verify current state
2. ? Test the fix with actual recurring orders
3. ? Monitor logs for "Interval too short" messages
4. ? Verify orders appear in SendCoffeeCheckup when appropriate

---

## Rollback Instructions:

If this change causes issues, revert `Classes\DateCalculator.cs` line 199-206 to:
```csharp
if (targetDayOfMonth < lastOrderDate.Day)
{
    cycleMonth = cycleMonth.AddMonths(1);
    nextOccurrence = BuildMonthlyTargetDate(cycleMonth, targetDayOfMonth);
}
```

---

## Additional Notes:

- The `MinimumMonthlyRecurringDays` of 20 is configurable in Web.config
- If customers complain about orders being skipped, consider reducing this value to 15
- The fix maintains backward compatibility with weekly recurring orders (unchanged)

---

## ?? Deployment Instructions

### Files to Commit (Git)

**Core Code Changes:**
```bash
git add Classes/DateCalculator.cs
git add Controls/ContactsThatMayNeedNextWeek.cs
git add Controls/SentRemindersLogTbl.cs
```

**Documentation:**
```bash
git add CHANGES_LOG.md
git add DEVELOPMENT_SESSION_NOTES.md
git add DELIVERY_DATE_FIX_SUMMARY.md
git add MONTHLY_RECURRING_END_OF_MONTH_BUG.md
git add SESSION_SUMMARY.md
git add WHY_RIDWAAN_RYAN_NOT_IN_CHECKUP.md
git add COMMIT_MESSAGE.md
git add CHAT_RESTART_PROMPT.txt
```

**Diagnostic Tools (Optional but Recommended):**
```bash
git add App_Data/SQLCommands-VerifyDeliveryDateFix.xml
git add App_Data/SQLCommands-DiagnoseCheckup.xml
git add App_Data/SQLCommands-DiagnoseMonthlyRecurring.xml
git add App_Data/SQLCommands-DiagnoseRidwaanRyan.xml
git add App_Data/SQLCommands-WhyNoCheckup-RidwaanRyan.xml
git add App_Data/SQLCommands-CheckNextRoastDates.xml
git add App_Data/SQLCommands-DiagnoseNextRoastDates.xml
git add App_Data/SQLCommands-FindMissing32.xml
git add App_Data/SQLCommands-DiagnoseMissing6.xml
git add App_Data/SQLCommands-NextCoffeeByReset.xml
git add App_Data/TEST-RidwaanRyan.sql
```

**Files to Review Before Committing:**
- `Web.config` - Contains test date settings, review before production
- `Pages/OrderDetail.aspx` - Check if changes were intentional
- `TrackerDotNet.csproj` - Review project file changes

**Files to EXCLUDE (Add to .gitignore):**
```bash
# Don't commit log files
App_Data/ErrorLog.txt
```

### Commit Message Template
See `COMMIT_MESSAGE.md` for detailed commit message, or use:
```bash
git commit -m "Fix critical coffee checkup bugs - delivery date filtering & monthly recurring

- Fix delivery date filtering using last checkup date (0?143 customers)
- Fix monthly recurring end-of-month edge case (Ridwaan case)
- Add comprehensive diagnostic SQL queries
- Add detailed documentation

Fixes #[issue-number] (if applicable)"
```

### Deployment Steps

1. **Build and Test:**
   ```bash
   # Ensure build is successful
   MSBuild TrackerDotNet.csproj /t:Build /p:Configuration=Release
   ```

2. **Deploy Core Files to Production:**
   - `bin/TrackerDotNet.dll` (recompiled with changes)
   - `Classes/DateCalculator.cs`
   - `Controls/ContactsThatMayNeedNextWeek.cs`
   - `Controls/SentRemindersLogTbl.cs`

3. **Reset Test Date in Production Web.config:**
   ```xml
   <!-- IMPORTANT: Set this to false in production! -->
   <add key="TestNow.Enabled" value="false" />
   ```

4. **Backup Production Database** before deployment

5. **Monitor Logs** after deployment:
   - Check for "Monthly recurrence" log messages
   - Check for "deliveryFilterDate" log messages
   - Verify customer counts in checkup

6. **Verify with Ridwaan's Order:**
   - Navigate to Recurring Orders page
   - Find Ridwaan (E Akhalwaya & Sons)
   - Click "Calc Next Required"
   - Verify NextDateRequired = April 1 (not May 2)

---

## ?? Build Status

```
Build: ? SUCCESSFUL
Errors: 0
Warnings: 0
Target Framework: .NET Framework 4.8
Branch: version/2.1.3.0
```

---

## ?? Related Issues/PRs

- Issue #[TBD] - Coffee checkup returning 0 customers
- Issue #[TBD] - Monthly recurring orders skipping months
- PR #[TBD] - This fix

---


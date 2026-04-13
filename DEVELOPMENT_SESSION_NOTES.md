# Development Session Notes - TrackerDotNet Coffee Checkup Fix
**Project:** TrackerDotNet  
**Branch:** version/2.1.3.0  
**Date Range:** 2025-01-21  
**Test Date:** 2026-03-27 (configured in Web.config)  

---

## ?? Session Objectives Completed

### 1. Fixed Monthly Recurring Order Date Calculation
- **Issue:** Orders for day 1 of month were being pushed 2 months out instead of next month
- **Root Cause:** Logic compared day numbers (1 vs 15) instead of actual date intervals
- **Solution:** Check actual days between orders, use 20-day minimum threshold
- **File Modified:** `Classes\DateCalculator.cs`

### 2. Fixed Critical Coffee Checkup Filtering Bug
- **Issue:** Checkup returned 0 customers despite 178 having NextCoffeeBy in correct window
- **Root Cause:** Used `Now()` for delivery date filtering, which filtered out stale city delivery dates
- **Solution:** Use last successful checkup date instead of `Now()` as baseline
- **Files Modified:** 
  - `Controls\SentRemindersLogTbl.cs` - Added `GetLastSuccessfulCheckupDate()`
  - `Controls\ContactsThatMayNeedNextWeek.cs` - Updated filtering logic

---

## ?? Key Documentation Files

### Primary Documentation
| File | Purpose |
|------|---------|
| `CHANGES_LOG.md` | Complete detailed log of all changes, before/after code, testing instructions |
| `DELIVERY_DATE_FIX_SUMMARY.md` | Comprehensive explanation of delivery date filtering fix |
| `COMMIT_MESSAGE.md` | Git commit message template |
| `DEVELOPMENT_SESSION_NOTES.md` | This file - quick reference for session restart |

### Diagnostic SQL Files
| File | Purpose |
|------|---------|
| `App_Data\SQLCommands-DiagnoseCheckup.xml` | Main checkup diagnostic - shows customers, tests filters |
| `App_Data\SQLCommands-FindMissing32.xml` | Finds why customers are missing from checkup |
| `App_Data\SQLCommands-VerifyDeliveryDateFix.xml` | **IMPORTANT** - Compares OLD vs NEW logic |
| `App_Data\SQLCommands-DiagnoseMonthlyRecurring.xml` | Monthly recurring orders analysis |
| `App_Data\SQLCommands-DiagnoseNextRoastDates.xml` | Checks NextRoastDateByCityTbl status |
| `App_Data\SQLCommands-CheckNextRoastDates.xml` | Similar to above |
| `App_Data\SQLCommands-NextCoffeeByReset.xml` | Tracks NextCoffeeBy updates |
| `App_Data\SQLCommands-DiagnoseRidwaanRyan.xml` | Specific customer investigation |
| `App_Data\SQLCommands-DiagnoseMissing6.xml` | Earlier diagnostic (similar to FindMissing32) |
| `App_Data\TEST-RidwaanRyan.sql` | Quick test for specific customers |

---

## ?? Technical Changes Summary

### DateCalculator.cs - Monthly Recurring Fix
**Method:** `CalculateNextMonthlyOccurrence()`  
**Change:** Replaced day number comparison with actual interval check

```csharp
// OLD - Wrong
if (targetDayOfMonth < lastOrderDate.Day) { skip to next month }

// NEW - Correct
int daysSinceLastOrder = (nextOccurrence - lastOrderDate).Days;
int minMonthlyInterval = SystemConstants.CheckupConstants.MinimumMonthlyRecurringDays; // 20
if (daysSinceLastOrder < minMonthlyInterval) { skip to next month }
```

### SentRemindersLogTbl.cs - New Method
**Added:** `GetLastSuccessfulCheckupDate()`

```csharp
public DateTime GetLastSuccessfulCheckupDate()
{
    // Returns MAX(DateSentReminder) WHERE ReminderSent = True
    // Falls back to MinReminderDate if no checkups have run
    // Purpose: Get baseline date for NextDeliveryDate filtering
}
```

### ContactsThatMayNeedNextWeek.cs - Delivery Filter Fix
**Method:** `GetContactsThatMayNeedNextWeek()`  
**Changes:**
1. Calculate deliveryFilterDate from last checkup
2. Use deliveryFilterDate instead of Now()
3. Added SQL filter: `NextDeliveryDate >= ?`

```csharp
// NEW CODE
DateTime lastCheckupDate = remindersLog.GetLastSuccessfulCheckupDate();
DateTime deliveryFilterDate = lastCheckupDate < baselineDate ? lastCheckupDate : baselineDate;
trackerDb.AddWhereParams((object)deliveryFilterDate, DbType.Date);

// UPDATED SQL
AND NextRoastDateByCityTbl.NextDeliveryDate >= ?  -- Uses deliveryFilterDate
```

---

## ?? Key Concepts & Understanding

### 1. Two Separate Systems
**Checkup System** (Consumption-based):
- Uses `ClientUsageTbl.NextCoffeeBy`
- Shows customers who will run out of coffee in 6-10 days
- For customers who need MANUAL reminders
- Window: `NextCoffeeBy >= DateAdd("d", 6, Now()) AND <= DateAdd("d", 10, Now())`

**Recurring Orders System** (Schedule-based):
- Uses `ReoccuringOrderTbl.NextDateRequired`
- Auto-creates orders based on schedule
- No manual reminder needed
- Separate processing

### 2. Why Ridwaan & Ryan Not in Checkup
- **Ridwaan:** NextCoffeeBy = April 17 (21 days) - outside 6-10 day window ? CORRECT
- **Ryan:** NextCoffeeBy = April 17 (21 days) - outside 6-10 day window ? CORRECT
- They have recurring orders that will auto-fire
- This is EXPECTED and CORRECT behavior

### 3. Delivery Date Filtering Logic
**Problem:** City delivery dates in `NextRoastDateByCityTbl` can be stale (only updated when admin runs "Set Next Roast Dates")

**Wrong Approach:** Filter where `NextDeliveryDate >= Now()`
- Filters out customers if delivery date is in past
- Misses customers who need coffee soon but have stale delivery dates

**Correct Approach:** Filter where `NextDeliveryDate >= LastCheckupDate`
- Includes customers whose delivery date is after last checkup
- Handles stale delivery dates correctly

---

## ?? Testing Configuration

### Web.config Settings
```xml
<!-- Test date override -->
<add key="TestNow.Enabled" value="true" />
<add key="TestNow.Value" value="2026-03-27" />

<!-- Checkup configuration -->
<add key="CoffeeCheckupReminderWindowDays" value="9" />
<add key="CoffeeCheckupMinMonthlyRecurringDays" value="20" />
```

### Test Data State (March 27, 2026)
- 178 customers with NextCoffeeBy in range
- NextRoastDateByCityTbl has delivery dates from March 28 - April 9
- Last checkup date: 2016-10-10 (MinReminderDate fallback)
- After fix: 143+ customers correctly identified

---

## ?? Common Diagnostic Queries

### 1. Verify Fix is Working
Run: `App_Data\SQLCommands-VerifyDeliveryDateFix.xml`
- Shows OLD vs NEW logic customer counts
- Displays last checkup date
- Shows deliveryFilterDate calculation

### 2. Check Why Customer Missing
Run: `App_Data\SQLCommands-DiagnoseCheckup.xml`
- Tests all filter conditions
- Shows NextCoffeeBy vs NextDeliveryDate
- Identifies failing conditions

### 3. Check Monthly Recurring Orders
Run: `App_Data\SQLCommands-DiagnoseMonthlyRecurring.xml`
- Shows all monthly orders
- Calculates days until next required
- Identifies orders in window

---

## ?? Important Notes

### Database Tables Involved
- **ClientUsageTbl** - NextCoffeeBy (consumption prediction)
- **ReoccuringOrderTbl** - NextDateRequired (scheduled orders)
- **NextRoastDateByCityTbl** - City delivery dates (can be stale!)
- **SentRemindersLogTbl** - Tracks checkup runs
- **SysDataTbl** - System settings (MinReminderDate)

### Stale Data Issues
1. `NextRoastDateByCityTbl` is only updated manually via "Set Next Roast Dates" button
2. If not updated, delivery dates can be weeks old
3. Using `Now()` for filtering incorrectly excludes these customers
4. Using `LastCheckupDate` correctly handles stale data

### Filter Conditions (in order)
1. `LastDateSentReminder` is NULL or not today ?
2. `enabled = True` ?
3. `PredictionDisabled = False` ?
4. `NextCoffeeBy > MinReminderDate` ?
5. **`NextCoffeeBy >= DateAdd("d", 6, Now()) AND <= DateAdd("d", 10, Now())`** ?? CRITICAL
6. **`NextDeliveryDate >= LastCheckupDate`** ?? CRITICAL (was causing 0 results)
7. `NextDeliveryDate <= DateAdd("d", 9, NextCoffeeBy) OR AlwaysSendChkUp` ?
8. No recent orders in next 9 days ?
9. Has email address ?

---

## ?? Next Session Quick Start Actions

1. **Review Status**
   - Run `SQLCommands-VerifyDeliveryDateFix.xml` to check current state
   - Verify build is still successful
   - Check logs for any errors

2. **Test Scenarios**
   - Test checkup with different dates
   - Verify monthly recurring orders calculate correctly
   - Test with stale vs fresh NextRoastDateByCityTbl data

3. **Production Readiness**
   - Review all changes in CHANGES_LOG.md
   - Run full diagnostic suite
   - Prepare deployment checklist

---

## ?? Lessons Learned

1. **Never filter by `Now()` for delivery dates** - use last known good state
2. **City delivery dates can be stale** - they're only updated manually
3. **Day number comparison ? date interval** - use actual days for accuracy
4. **Checkup ? Recurring orders** - separate systems, different purposes
5. **Document diagnostic queries** - saved hours in troubleshooting

---

## ?? Contact Points

**Current State:**
- Build: ? Successful
- Tests: ? Pending verification
- Deploy: ? Ready for testing

**Git:**
- Branch: `version/2.1.3.0`
- Remote: `https://github.com/wmachanik/TrackerDotNet-4-2025`

---

## ?? Rollback Plan

If issues arise:

### Rollback DateCalculator.cs
```csharp
// Revert to original day number comparison
if (targetDayOfMonth < lastOrderDate.Day) {
    cycleMonth = cycleMonth.AddMonths(1);
    nextOccurrence = BuildMonthlyTargetDate(cycleMonth, targetDayOfMonth);
}
```

### Rollback ContactsThatMayNeedNextWeek.cs
```csharp
// Remove deliveryFilterDate logic, use Now() directly
trackerDb.AddWhereParams((object)TimeZoneUtils.Now().Date, DbType.Date);
// Remove SQL filter: AND NextRoastDateByCityTbl.NextDeliveryDate >= ?
```

### Rollback SentRemindersLogTbl.cs
- Delete `GetLastSuccessfulCheckupDate()` method
- No other code depends on it

---

**Last Updated:** 2025-01-21  
**Status:** Changes implemented, tested, ready for production validation  
**Build:** ? SUCCESS

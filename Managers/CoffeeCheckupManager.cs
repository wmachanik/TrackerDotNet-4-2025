using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using TrackerDotNet.Classes;
using TrackerDotNet.Controls;

namespace TrackerDotNet.Managers
{
    /// <summary>
    /// Central business logic manager for Coffee Checkup operations
    /// Orchestrates the entire coffee checkup process following SOLID principles
    /// ENHANCED WITH QUICK PERFORMANCE OPTIMIZATIONS
    /// </summary>
    public class CoffeeCheckupManager
    {
        private readonly CoffeeCheckupEmailManager _emailManager;
        private readonly DateCalculator _deliveryCalculator; // ADD THIS

        // Constants moved from code-behind for better organization
        private const int CONST_FORCEREMINDERDELAYCOUNT = 4;
        private const int CONST_MAXREMINDERS = 7;
        private const int CONST_DEFAULTREMINDERWINDOWDAYS = 7;
        private const int CONST_DEFAULTMINIMUMMONTHLYRECURRINGDAYS = 20;

        // MISSING: Static caching for frequently accessed lookup data
        private static Dictionary<int, string> _cachedItemDescriptions;
        private static Dictionary<int, string> _cachedItemSKUs;
        private static Dictionary<int, string> _cachedCityNames;
        private static Dictionary<int, string> _cachedPackagingDescriptions;
        private static List<int> _cachedInternalCustomerIds;
        private static DateTime _cacheExpiry = DateTime.MinValue;
        private static readonly object _cacheLock = new object();

        public CoffeeCheckupManager()
        {
            _emailManager = new CoffeeCheckupEmailManager();
            _deliveryCalculator = new DateCalculator(); // ADD THIS
        }
        private static void UpdateCacheExpiry()
        {
            _cacheExpiry = DateTime.Now.AddMinutes(1); // or your preferred duration
        }
        /// <summary>
        /// Main entry point for processing coffee checkup reminders
        /// </summary>
        public BatchSendResult ProcessCoffeeCheckupReminders(SendCheckEmailTextsData emailData)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // MISSING: Stopwatch declaration

            try
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, "CoffeeCheckupManager: Starting coffee checkup process");

                // 1. Validate configuration
                if (!_emailManager.ValidateConfiguration())
                {
                    return new BatchSendResult
                    {
                        IsSuccess = false,
                        TotalSent = 0,
                        TotalFailed = 1,
                        ErrorMessage = "Email configuration validation failed"
                    };
                }

                // Pre-warm caches before processing
                WarmupCaches();

                // 2. Get eligible customers
                var eligibleCustomers = GetEligibleCustomers();

                if (!eligibleCustomers.Any())
                {
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, "CoffeeCheckupManager: No eligible customers found");
                    return new BatchSendResult
                    {
                        IsSuccess = true,
                        TotalSent = 0,
                        TotalFailed = 0,
                        ErrorMessage = "No customers found requiring reminders"
                    };
                }

                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Processing {eligibleCustomers.Count} eligible customers");

                // 3. Process batch reminders
                var result = ProcessRemindersBatch(eligibleCustomers, emailData);

                stopwatch.Stop();
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Process completed in {stopwatch.ElapsedMilliseconds}ms - {result.TotalSent} sent, {result.TotalFailed} failed");

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Process failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
                return new BatchSendResult
                {
                    IsSuccess = false,
                    TotalSent = 0,
                    TotalFailed = 1,
                    ErrorMessage = ex.Message
                };
            }
        }
        /// <summary>
        /// Processes reminder batch using the existing logic - ENHANCED WITH PROPER TEST MODE HANDLING
        /// </summary>
        private BatchSendResult ProcessRemindersBatch(List<ContactToRemindWithItems> allContacts, SendCheckEmailTextsData emailData)
        {
            AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Processing batch with {allContacts.Count} contacts");

            var totalResult = new BatchSendResult();
            var customersTbl = new CustomersTbl();

            // Check if we're in test mode
            var testEmailClient = new EmailMailKitCls();
            bool isTestMode = testEmailClient.IsTestMode;

            if (isTestMode)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, "CoffeeCheckupManager: TEST MODE - Database updates will be skipped");
            }

            // Group contacts by reminder type
            var recurringContacts = new List<ContactToRemindWithItems>();
            var autoFulfillContacts = new List<ContactToRemindWithItems>();
            var reminderOnlyContacts = new List<ContactToRemindWithItems>();
            var failedContacts = new List<string>();

            // Process each contact and categorize
            int processed = 0;
            foreach (var contact in allContacts)
            {
                try
                {
                    // Validate eligibility
                    if (!ValidateCustomerEligibility(contact))
                    {
                        continue;
                    }

                    processed++;
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"Processing contact {processed}: {contact.CompanyName} (ID: {contact.CustomerID})");

                    string orderType = GetOrderType(contact);

                    // Update customer data if not in test mode
                    //if (!isTestMode)
                    //{
                    if (!UpdateCustomerReminderData(contact, customersTbl))
                    {
                        failedContacts.Add($"{contact.CompanyName} - Database update failed");
                        continue;
                    }
                    //}

                    // Categorize for batch processing
                    CategorizeContact(contact, orderType, recurringContacts, autoFulfillContacts, reminderOnlyContacts);
                }
                catch (Exception ex)
                {
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"Error processing contact {contact.CompanyName}: {ex.Message}");
                    failedContacts.Add($"{contact.CompanyName} - Processing error: {ex.Message}");
                }
            }

            // Send batches by type
            totalResult = SendAllBatches(recurringContacts, autoFulfillContacts, reminderOnlyContacts, emailData);

            // Add pre-processing failures
            totalResult.TotalFailed += failedContacts.Count;
            totalResult.IsSuccess = totalResult.TotalFailed == 0;

            AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Batch completed - {totalResult.TotalSent} sent, {totalResult.TotalFailed} failed");
            return totalResult;
        }

        /// <summary>
        /// Determines order type for a contact
        /// </summary>
        private string GetOrderType(ContactToRemindWithItems contact)
        {
            bool hasAutoFulfill = contact.ItemsContactRequires.Exists(x => x.AutoFulfill);
            bool hasRecurring = contact.ItemsContactRequires.Exists(x => x.ReoccurOrder);

            if (hasRecurring && hasAutoFulfill)
                return MessageProvider.Get(MessageKeys.CoffeeCheckup.OrderTypeCombined);
            else if (hasRecurring)
                return MessageProvider.Get(MessageKeys.CoffeeCheckup.OrderTypeRecurring);
            else if (hasAutoFulfill)
                return MessageProvider.Get(MessageKeys.CoffeeCheckup.OrderTypeAutoFulfill);

            return string.Empty; // Reminder only
        }

        /// <summary>
        /// Updates customer reminder data in database
        /// </summary>
        private bool UpdateCustomerReminderData(ContactToRemindWithItems contact, CustomersTbl customersTbl)
        {
            try
            {
                contact.ReminderCount++;

                if (contact.ReminderCount < CONST_MAXREMINDERS)
                {
                    // Handle forced delay for frequent reminders
                    if (contact.ReminderCount >= CONST_FORCEREMINDERDELAYCOUNT)
                    {
                        int delayDays = 10 * (contact.ReminderCount - CONST_FORCEREMINDERDELAYCOUNT + 1);
                        new ClientUsageTbl().ForceNextCoffeeDate(
                            contact.NextPrepDate.AddDays(delayDays),
                            contact.CustomerID);
                    }

                    customersTbl.SetSentReminderAndIncrementReminderCount(
                        TimeZoneUtils.Now().Date,
                        contact.CustomerID);

                    return true;
                }
                else
                {
                    customersTbl.DisableCustomer(contact.CustomerID,
                        $"Disabled on {TimeZoneUtils.Now():d} - exceeded max reminder limit");
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"Customer {contact.CompanyName} disabled - exceeded max reminder limit");
                    return false;
                }
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"Database update failed for {contact.CompanyName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Categorizes contact into appropriate batch - FIXED TO PREVENT DUPLICATES
        /// </summary>
        private void CategorizeContact(ContactToRemindWithItems contact, string orderType,
            List<ContactToRemindWithItems> recurringContacts,
            List<ContactToRemindWithItems> autoFulfillContacts,
            List<ContactToRemindWithItems> reminderOnlyContacts)
        {
            // IMPORTANT: Each customer should only go into ONE category to prevent multiple emails
            if (string.IsNullOrWhiteSpace(orderType))
            {
                reminderOnlyContacts.Add(contact);
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Categorized {contact.CompanyName} as REMINDER ONLY");
            }
            else if (orderType.Contains("recurring"))
            {
                recurringContacts.Add(contact);
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Categorized {contact.CompanyName} as RECURRING");
            }
            else if (orderType.Contains("autofulfill") || orderType.Contains("auto"))
            {
                autoFulfillContacts.Add(contact);
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Categorized {contact.CompanyName} as AUTOFULFILL");
            }
            else
            {
                // Default to reminder only if orderType is not recognized
                reminderOnlyContacts.Add(contact);
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Categorized {contact.CompanyName} as REMINDER ONLY (unknown order type: '{orderType}')");
            }
        }

        /// <summary>
        /// Sends all batches and combines results
        /// </summary>
        private BatchSendResult SendAllBatches(
            List<ContactToRemindWithItems> recurringContacts,
            List<ContactToRemindWithItems> autoFulfillContacts,
            List<ContactToRemindWithItems> reminderOnlyContacts,
            SendCheckEmailTextsData emailData)
        {
            var totalResult = new BatchSendResult();

            if (recurringContacts.Any())
            {
                var recurringResult = SendReminderBatch(recurringContacts, "recurring", emailData);
                totalResult.Combine(recurringResult);
            }

            if (autoFulfillContacts.Any())
            {
                var autoFulfillResult = SendReminderBatch(autoFulfillContacts, "autofulfill", emailData);
                totalResult.Combine(autoFulfillResult);
            }

            if (reminderOnlyContacts.Any())
            {
                var reminderResult = SendReminderBatch(reminderOnlyContacts, "reminder", emailData);
                totalResult.Combine(reminderResult);
            }

            return totalResult;
        }
        /// <summary>
        /// Prepares customer reminder data for display in UI
        /// </summary>
        public void PrepareCustomerReminderData(int reminderWindowDays)
        {
            try
            {
                // Ensure roast dates are current
                var trackerTools = new TrackerTools();
                if (!trackerTools.IsNextRoastDateByCityTodays())
                {
                    trackerTools.SetNextRoastDateByCity();
                }

                // Build reminder list and populate temp tables
                SetListOfContactsToSendReminderTo(reminderWindowDays);

                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, "CoffeeCheckupManager: Customer reminder data preparation completed  (window: {reminderWindowDays} days)");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error preparing customer data: {ex.Message}");
                throw;
            }
        }

        // MISSING: All the cache methods
        /// <summary>
        /// QUICK WIN: Cached item descriptions for GridView display
        /// </summary>
        public string GetCachedItemDescription(int itemId)
        {
            if (itemId <= 0) return string.Empty;

            lock (_cacheLock)
            {
                if (_cachedItemDescriptions == null || DateTime.Now > _cacheExpiry)
                {
                    _cachedItemDescriptions = new Dictionary<int, string>();
                    UpdateCacheExpiry();
                }
            }

            if (!_cachedItemDescriptions.ContainsKey(itemId))
            {
                try
                {
                    _cachedItemDescriptions[itemId] = ItemTypeTbl.GetItemTypeDescById(itemId);
                }
                catch (Exception ex)
                {
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error caching item description for ID {itemId}: {ex.Message}");
                    _cachedItemDescriptions[itemId] = $"Item {itemId}"; // Fallback
                }
            }

            return _cachedItemDescriptions[itemId];
        }
        

        /// <summary>
        /// QUICK WIN: Cached item SKUs for GridView display
        /// </summary>
        public string GetCachedItemSKU(int itemId)
        {
            if (itemId <= 0) return string.Empty;

            lock (_cacheLock)
            {
                if (_cachedItemSKUs == null || DateTime.Now > _cacheExpiry)
                {
                    _cachedItemSKUs = new Dictionary<int, string>();
                    UpdateCacheExpiry();
                }

                if (!_cachedItemSKUs.ContainsKey(itemId)
            )
                {
                    try
                    {
                        _cachedItemSKUs[itemId] = new ItemTypeTbl().GetItemTypeSKU(itemId);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error caching item SKU for ID {itemId}: {ex.Message}");
                        _cachedItemSKUs[itemId] = $"SKU{itemId}"; // Fallback
                    }
                }

                return _cachedItemSKUs[itemId];
            }
        }

        /// <summary>
        /// QUICK WIN: Cached city names for GridView display
        /// </summary>
        public string GetCachedCityName(int cityId)
        {
            if (cityId <= 0) return string.Empty;

            lock (_cacheLock)
            {
                if (_cachedCityNames == null || DateTime.Now > _cacheExpiry)
                {
                    _cachedCityNames = new Dictionary<int, string>();
                    UpdateCacheExpiry();
                }

                if (!_cachedCityNames.ContainsKey(cityId))
                {
                    try
                    {
                        _cachedCityNames[cityId] = new CityTblDAL().GetCityName(cityId);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error caching city name for ID {cityId}: {ex.Message}");
                        _cachedCityNames[cityId] = $"City {cityId}"; // Fallback
                    }
                }

                return _cachedCityNames[cityId];
            }
        }

        /// <summary>
        /// QUICK WIN: Cached packaging descriptions for GridView display
        /// </summary>
        public string GetCachedPackagingDescription(int packagingId)
        {
            if (packagingId <= 0) return string.Empty;

            lock (_cacheLock)
            {
                if (_cachedPackagingDescriptions == null || DateTime.Now > _cacheExpiry)
                {
                    _cachedPackagingDescriptions = new Dictionary<int, string>();
                    UpdateCacheExpiry();
                }

                if (!_cachedPackagingDescriptions.ContainsKey(packagingId))
                {
                    try
                    {
                        _cachedPackagingDescriptions[packagingId] = new PackagingTbl().GetPackagingDesc(packagingId);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error caching packaging description for ID {packagingId}: {ex.Message}");
                        _cachedPackagingDescriptions[packagingId] = $"Package {packagingId}"; // Fallback
                    }
                }

                return _cachedPackagingDescriptions[packagingId];
            }
        }
        /// <summary>
        /// QUICK WIN: Static cache invalidation method for when lookup data changes
        /// </summary>
        public static void InvalidateCache()
        {
            lock (_cacheLock)
            {
                _cachedItemDescriptions = null;
                _cachedItemSKUs = null;
                _cachedCityNames = null;
                _cachedPackagingDescriptions = null;
                _cachedInternalCustomerIds = null;
                _cacheExpiry = DateTime.MinValue;
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, "CoffeeCheckupManager: Cache invalidated");
            }
        }

        /// <summary>
        /// QUICK WIN: Static cache invalidation method for when lookup data changes
        /// </summary>
        public string GetCachedItemUoM(int itemId)
        {
            if (itemId <= 0) return string.Empty;

            lock (_cacheLock)
            {
                if (_cachedItemSKUs == null || DateTime.Now > _cacheExpiry)
                {
                    _cachedItemSKUs = new Dictionary<int, string>();
                    UpdateCacheExpiry();
                }

                // Use a separate key for UoM to avoid conflicts
                string uomKey = $"UoM_{itemId}";
                int uomKeyHash = uomKey.GetHashCode(); // Simple way to create unique int key

                if (!_cachedItemSKUs.ContainsKey(uomKeyHash))
                {
                    try
                    {
                        _cachedItemSKUs[uomKeyHash] = new ItemTypeTbl().GetItemUnitOfMeasure(itemId);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error caching item UoM for ID {itemId}: {ex.Message}");
                        _cachedItemSKUs[uomKeyHash] = "units"; // Fallback
                    }
                }

                return _cachedItemSKUs[uomKeyHash];
            }
        }

        private bool HasValidEmailAddress(ContactToRemindWithItems contact)
        {
            // QUICK WIN: Use optimized version
            return HasValidEmailAddressOptimized(contact);
        }

        /// <summary>
        /// QUICK WIN: Pre-warm lookup caches to reduce database calls during processing
        /// </summary>
        private void WarmupCaches()
        {
            try
            {
                var warmupStopwatch = System.Diagnostics.Stopwatch.StartNew();

                // Warm up the most commonly used caches
                GetCachedInternalCustomerIds();

                // Pre-cache commonly used item types (coffee service types)
                var itemTypeTbl = new ItemTypeTbl();
                var coffeeItems = itemTypeTbl.GetAllItemIDsofServiceType(2);
                coffeeItems.AddRange(itemTypeTbl.GetAllItemIDsofServiceType(21));

                foreach (var itemId in coffeeItems.Take(20)) // Cache top 20 most common items
                {
                    GetCachedItemDescription(itemId);
                    GetCachedItemSKU(itemId);
                }

                warmupStopwatch.Stop();
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Cache warmup completed in {warmupStopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Cache warmup failed: {ex.Message}");
                // Don't fail the process if cache warmup fails
            }
        }

        /// <summary>
        /// Validates if a customer is eligible for reminders
        /// </summary>
        /// <param name="customer">Customer to validate</param>
        /// <returns>True if eligible, false otherwise</returns>
        public bool ValidateCustomerEligibility(ContactToRemindWithItems customer)
        {
            try
            {
                // Check if internal customer
                if (IsInternalCustomer((int)customer.CustomerID))
                {
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"Customer {customer.CompanyName} is internal - skipping");
                    return false;
                }

                // Check if has valid email
                if (!HasValidEmailAddress(customer))
                {
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"Customer {customer.CompanyName} has no valid email address");
                    return false;
                }

                // Check if within reminder limits
                if (!IsEligibleForReminder(customer))
                {
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"Customer {customer.CompanyName} not eligible for reminder (disabled or exceeded limits)");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"Error validating customer {customer.CompanyName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets list of customers eligible for reminders
        /// </summary>
        private List<ContactToRemindWithItems> GetEligibleCustomers()
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                // Get cached internal customer list once
                var internalCustomerIds = GetCachedInternalCustomerIds();

                // Use existing method but with optimizations
                var allContacts = new TempCoffeeCheckup().GetAllContactAndItems();

                // Filter in memory instead of multiple DB calls per customer
                var eligibleContacts = allContacts.Where(contact =>
                    contact.enabled &&
                    contact.ReminderCount < CONST_MAXREMINDERS &&
                    !internalCustomerIds.Contains((int)contact.CustomerID) &&
                    HasValidEmailAddressOptimized(contact)
                ).ToList();

                stopwatch.Stop();
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Customer filtering completed in {stopwatch.ElapsedMilliseconds}ms - {eligibleContacts.Count} of {allContacts.Count} eligible");

                return eligibleContacts;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"Error getting eligible customers: {ex.Message}");
                return new List<ContactToRemindWithItems>();
            }
        }

        /// <summary>
        /// QUICK WIN: Optimized email validation without string concatenation
        /// QUICK WIN: Optimized email validation without string concatenation
        /// </summary>
        private bool HasValidEmailAddressOptimized(ContactToRemindWithItems contact)
        {
            return (!string.IsNullOrWhiteSpace(contact.EmailAddress) && contact.EmailAddress.Contains("@")) ||
               (!string.IsNullOrWhiteSpace(contact.AltEmailAddress) && contact.AltEmailAddress.Contains("@"));
        }

        /// <summary>
        /// QUICK WIN: Cached internal customer IDs to avoid repeated database calls
        /// </summary>
        private List<int> GetCachedInternalCustomerIds()
        {
            lock (_cacheLock)
            {
                if (_cachedInternalCustomerIds == null || DateTime.Now > _cacheExpiry)
                {
                    try
                    {
                        _cachedInternalCustomerIds = new SysDataTbl().GetInternalCustomerIdsList();
                        UpdateCacheExpiry();
                        AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Cached {_cachedInternalCustomerIds.Count} internal customer IDs");
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error caching internal customer IDs: {ex.Message}");
                        _cachedInternalCustomerIds = new List<int>(); // Empty list as fallback
                    }
                }
                return _cachedInternalCustomerIds;
            }
        }

        // Helper methods moved from code-behind
        private bool IsInternalCustomer(int customerId)
        {
            try
            {
                // QUICK WIN: Use cached list instead of database call
                var internalCustomerIds = GetCachedInternalCustomerIds();
                return internalCustomerIds.Contains(customerId);
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"Error checking internal customer status: {ex.Message}");
                return false;
            }
        }

        private bool IsEligibleForReminder(ContactToRemindWithItems contact)
        {
            return contact.enabled && contact.ReminderCount < CONST_MAXREMINDERS;
        }

        /// <summary>
        /// Sets up the list of contacts to send reminders to - FIXED TO PREVENT MIXING RECURRING AND REMINDER CUSTOMERS
        /// </summary>
        private void SetListOfContactsToSendReminderTo(int reminderWindowDays)
        {
            try
            {
                // Step 1: Get all customers with any active reoccurring order
                var recurringCustomerIds = GetAllReoccurringOrderCustomerIds();
                //AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Found {recurringCustomerIds.Count} customers with any active recurring orders - they will be excluded from reminder processing");

                // Step 2: Get recurring contacts that are actually due (for display, etc.)
                List<ContactToRemindWithItems> recurringContacts = GetRecurringContactsNeedingReminder(reminderWindowDays);

                // Step 3: Get reminder contacts, excluding all recurring customers
                List<ContactToRemindWithItems> reminderContacts = new List<ContactToRemindWithItems>();
                AddAllContactsToRemind(ref reminderContacts, recurringCustomerIds, reminderWindowDays);

                // Step 4: Combine the lists (recurring + reminder, but no overlap)
                List<ContactToRemindWithItems> allContacts = new List<ContactToRemindWithItems>();
                allContacts.AddRange(recurringContacts);
                allContacts.AddRange(reminderContacts);

                allContacts.Sort((a, b) => string.Compare(a.CompanyName, b.CompanyName));

                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Final contact list - {recurringContacts.Count} recurring customers, {reminderContacts.Count} reminder customers, {allContacts.Count} total");

                TempCoffeeCheckup tempCoffeeCheckup = new TempCoffeeCheckup();
                if (!tempCoffeeCheckup.DeleteAllContactRecords() || !tempCoffeeCheckup.DeleteAllContactItems())
                {
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, "CoffeeCheckupManager: Error deleting old temp tables");
                    throw new InvalidOperationException("Error deleting old temp tables");
                }

                ItemTypeTbl itemTypeTbl = new ItemTypeTbl();
                List<int> idsofServiceType = itemTypeTbl.GetAllItemIDsofServiceType(2);
                idsofServiceType.AddRange(itemTypeTbl.GetAllItemIDsofServiceType(21));

                bool success = false;
                for (int index1 = 0; index1 < allContacts.Count; ++index1)
                {
                    bool hasValidItems = false;
                    for (int index2 = 0; index2 < allContacts[index1].ItemsContactRequires.Count && !hasValidItems; ++index2)
                        hasValidItems = idsofServiceType.Contains(allContacts[index1].ItemsContactRequires[index2].ItemID);

                    if (hasValidItems)
                    {
                        success = tempCoffeeCheckup.InsertContacts((ContactToRemindDetails)allContacts[index1]) || success;
                        foreach (ItemContactRequires itemsContactRequire in allContacts[index1].ItemsContactRequires)
                            success = tempCoffeeCheckup.InsertContactItems(itemsContactRequire) || success;
                    }
                }

                if (!success)
                {
                    throw new InvalidOperationException("Not all records added to Temp Table");
                }

                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Successfully processed {allContacts.Count} contacts for reminder data");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error in SetListOfContactsToSendReminderTo: {ex.Message}");
                throw;
            }
        }
        private HashSet<long> GetAllReoccurringOrderCustomerIds()
        {
            var reoccuringOrderDal = new ReoccuringOrderDAL();
            var allOrders = reoccuringOrderDal.GetAll(1, "CustomersTbl.CustomerID");
            return allOrders != null
                ? new HashSet<long>(allOrders.Where(o => o.Enabled).Select(o => o.CustomerID))
                : new HashSet<long>();
        }
        /// <summary>
        /// Enhanced order conflict detection - checks for any existing orders that would conflict
        /// CORRECTED to use available methods
        /// </summary>
        private bool HasConflictingOrders(long customerId, int itemId, DateTime checkStartDate, DateTime checkEndDate)
        {
            try
            {
                var orderCheckTbl = new OrderCheckTbl();

                // Check for any coffee orders in the date range
                bool hasConflicts = orderCheckTbl.HasCoffeeOrdersInDateRange(customerId, checkStartDate, checkEndDate);

                if (hasConflicts)
                {
                    var orders = orderCheckTbl.GetCoffeeOrdersInDateRange(customerId, checkStartDate, checkEndDate);
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Customer {customerId} has {orders.Count} coffee orders in date range {checkStartDate:yyyy-MM-dd} to {checkEndDate:yyyy-MM-dd}");
                }

                return hasConflicts;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error checking order conflicts for customer {customerId}: {ex.Message}");
                return false;
            }
        }
        //private List<ReoccuringOrderExtData> FilterAndUpdateRecurringOrders(List<ReoccuringOrderExtData> reoccuringOrders, DeliveryDateCalculator deliveryDateCalculator,
        //    DateTime windowStart,DateTime windowEnd)
        //{
        //    var validOrders = new List<ReoccuringOrderExtData>();
        //    var reoccuringOrderDal = new ReoccuringOrderDAL();

        //    foreach (var order in reoccuringOrders)
        //    {
        //        // Always recalculate next required date using LastDone date and recurrence type
        //        var calculatedNextDate = reoccuringOrderDal.CalculateNextDateRequired(order);

        //        // Update legacy/broken records if needed
        //        if (order.NextDateRequired != calculatedNextDate)
        //        {
        //            order.NextDateRequired = calculatedNextDate;
        //            reoccuringOrderDal.UpdateReoccuringOrder(order, order.ReoccuringOrderID);
        //        }

        //        // Only keep records where next required date is <= windowEnd and not min date
        //        if (order.NextDateRequired <= windowEnd &&
        //            order.NextDateRequired != SystemConstants.DatabaseConstants.SystemMinDate)
        //        {
        //            validOrders.Add(order);
        //        }
        //    }
        //    return validOrders;
        //}
        /// <summary>
        /// Gets recurring contacts that need reminders
        /// </summary>
        private List<ContactToRemindWithItems> GetRecurringContactsNeedingReminder(int reminderWindowDays)
        {
            var reocurringContacts = new List<ContactToRemindWithItems>();
            var trackerTools = new TrackerTools();
            DateTime minValue1 = DateTime.MinValue;
            DateTime minReminderDate = new SysDataTbl().GetMinReminderDate();
            DateTime windowStart = TimeZoneUtils.Now().Date;
            DateTime windowEnd = windowStart.AddDays(reminderWindowDays);
            var reoccuringOrderDal = new ReoccuringOrderDAL();
            var deliveryDateCalculator = new DateCalculator();

            // this should not be needed anymore since it is set on order done
            //if (!reoccuringOrderDal.SetReoccuringItemsLastDate())
            //{
            //    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, "CoffeeCheckupManager: Could not set the recurring last date");
            //    return reocurringContacts;
            //}

            // Use SQL whereFilter for efficiency
            string whereFilter = $"NextDateRequired <= #{windowEnd:yyyy-MM-dd}# ";
            var validOrders = reoccuringOrderDal.GetAll(1, "CustomersTbl.CustomerID", whereFilter);

            if (validOrders == null || !validOrders.Any())
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, "CoffeeCheckupManager: No recurring orders found to process");
                return reocurringContacts;
            }
            // Fix legacy/broken records and filter to actionable window
            validOrders = deliveryDateCalculator.FilterAndUpdateRecurringOrdersDates(validOrders, windowStart, windowEnd);

            AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Processing {validOrders.Count} valid recurring order patterns");

            foreach (var order in validOrders)
            {
                try
                {
                    // Skip if recently processed
                    //if (IsRecentlyProcessed(order))
                    //{
                    //    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Skipping recurring order {order.ReoccuringOrderID} for customer {order.CustomerID} - recently processed.");
                    //    continue;
                    //}

                    //if (!ReoccuranceTypeTbl.IsValidRecurrenceType(order.ReoccuranceTypeID))
                    //{
                    //    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Unknown recurrence type {order.ReoccuranceTypeID} for recurring order {order.ReoccuringOrderID}");
                    //    continue;
                    //}

                    // Check if order is expired
                    if (order.RequireUntilDate > minReminderDate && order.NextDateRequired > order.RequireUntilDate)
                    {
                        // Disable the recurring order in the database
                        //var reoccuringOrderDal = new ReoccuringOrderDAL();
                        order.Enabled = false;
                        reoccuringOrderDal.UpdateReoccuringOrder(order, order.ReoccuringOrderID);

                        AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup,
                            $"CoffeeCheckupManager: Recurring order {order.ReoccuringOrderID} expired on {order.RequireUntilDate:yyyy-MM-dd} and has been disabled.");
                        continue;
                    }

                    // Get next roast date
                    DateTime sourceDateTime;
                    try
                    {
                        sourceDateTime = trackerTools.GetNextRoastDateByCustomerID(order.CustomerID, ref minValue1);
                        if (sourceDateTime == DateTime.MinValue || sourceDateTime < windowStart.AddDays(-30))
                        {
                            AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Invalid roast date for customer {order.CustomerID}, using minimum reminder date");
                            sourceDateTime = minReminderDate;
                        }
                    }
                    catch
                    {
                        sourceDateTime = minReminderDate;
                    }

                    if (sourceDateTime < minReminderDate)
                        sourceDateTime = minReminderDate;

                    // Check for conflicting orders
                    if (!HasConflictingOrders(order.CustomerID, order.ItemRequiredID, windowStart, windowEnd))
                    {
                        var itemRequired = new ItemContactRequires
                        {
                            CustomerID = order.CustomerID,
                            AutoFulfill = false,
                            ReoccurID = order.ReoccuringOrderID,
                            ReoccurOrder = true,
                            ItemID = order.ItemRequiredID,
                            ItemQty = order.QtyRequired,
                            ItemPackagID = order.PackagingID
                        };

                        var contact = reocurringContacts.FirstOrDefault(x => x.CustomerID == itemRequired.CustomerID)
                            ?? new ContactToRemindWithItems().GetCustomerDetails(itemRequired.CustomerID);

                        if (contact != null)
                        {
                            contact.ItemsContactRequires.Add(itemRequired);
                            if (!reocurringContacts.Contains(contact))
                                reocurringContacts.Add(contact);
                            AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Added customer {contact.CompanyName} for recurring item {order.ItemRequiredID}");
                        }
                    }
                    else
                    {
                        AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Skipping recurring item {order.ReoccuringOrderID} - conflicting orders found for customer {order.CustomerID}");
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error processing recurring order {order.ReoccuringOrderID}: {ex.Message}");
                    // Continue with next recurring order
                }
            }

            AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Found {reocurringContacts.Count} customers with recurring items due within {reminderWindowDays} days");
            return reocurringContacts;
        }
        // --- Helper Methods ---
        //private DateTime CalculateNextDateRequired(ReoccuringOrderExtData order, DeliveryDateCalculator deliveryDateCalculator)
        //{
        //    var recurrenceType = order.ReoccuranceTypeID; // ReoccuranceTypeTbl.GetRecurrenceType(order.ReoccuranceTypeID);
        //    switch (recurrenceType)
        //    {
        //        case ReoccuranceTypeTbl.CONST_WEEKTYPEID:
        //            return order.DateLastDone.AddDays(order.ReoccuranceValue * 7).Date;

        //        case ReoccuranceTypeTbl.CONST_DAYOFMONTHID:
        //            DateTime nextMonth = order.DateLastDone.AddMonths(1);
        //            try
        //            {
        //                return deliveryDateCalculator.CalculateOptimalWeeklyDeliveryDate(
        //                    order.CustomerID,
        //                    DeliveryDateCalculator.WEEKLY_INTERVAL,
        //                    new DateTime(nextMonth.Year, nextMonth.Month, order.ReoccuranceValue).Date);
        //            }
        //            catch (ArgumentOutOfRangeException)
        //            {
        //                int daysInMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
        //                int targetDay = Math.Min(order.ReoccuranceValue, daysInMonth);
        //                return new DateTime(nextMonth.Year, nextMonth.Month, targetDay).Date;
        //            }
        //        default:
        //            throw new NotSupportedException($"Unsupported recurrence type {recurrenceType}");
        //    }
        //}
        private bool IsRecentlyProcessed(ReoccuringOrderExtData order)
        {
            var recurrenceType = ReoccuranceTypeTbl.GetRecurrenceType(order.ReoccuranceTypeID);
            int daysSinceLastProcessed = (TimeZoneUtils.Now().Date - order.DateLastDone).Days;
            int minimumDays = 0;

            if (recurrenceType == ReoccuranceTypeTbl.RecurrenceType.Weekly)
                minimumDays = order.ReoccuranceValue * 7;
            else if (recurrenceType == ReoccuranceTypeTbl.RecurrenceType.Monthly)
                minimumDays = GetMinimumRecurringDays();

            bool result = minimumDays > 0 && daysSinceLastProcessed < minimumDays;
            if (result)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Skipping recurring item {order.ReoccuringOrderID} - processed only {daysSinceLastProcessed} days ago (minimum: {minimumDays})");
            }
            return result;
        }

        /// <summary>
        /// Adds all contacts that may need reminders - ENHANCED TO EXCLUDE RECURRING CUSTOMERS
        /// </summary>
        private void AddAllContactsToRemind(ref List<ContactToRemindWithItems> pContactsToRemind, HashSet<long> excludeCustomerIds, int reminderWindowDays)
        {
            try
            {
                List<ContactsThayMayNeedData> thatMayNeedNextWeek = new ContactsThatMayNeedNextWeek().GetContactsThatMayNeedNextWeek(reminderWindowDays);
                CustomerTrackedServiceItems trackedServiceItems = new CustomerTrackedServiceItems();

                // Initialize exclusion set if not provided
                excludeCustomerIds = excludeCustomerIds ?? new HashSet<long>();

                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Processing {thatMayNeedNextWeek.Count} contacts that may need items next week (excluding {excludeCustomerIds.Count} recurring customers)");

                for (int index1 = 0; index1 < thatMayNeedNextWeek.Count; ++index1)
                {
                    try
                    {
                        // BUG FIX: Skip customers that have recurring orders
                        if (excludeCustomerIds.Contains(thatMayNeedNextWeek[index1].CustomerData.CustomerID))
                        {
                            AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Skipping {thatMayNeedNextWeek[index1].CustomerData.CompanyName} - customer has recurring orders");
                            continue;
                        }

                        List<CustomerTrackedServiceItems.CustomerTrackedServiceItemsData> byCustomerTypeId =
                            trackedServiceItems.GetAllByCustomerTypeID(thatMayNeedNextWeek[index1].CustomerData.CustomerTypeID);

                        // Build contact info
                        ContactToRemindWithItems toRemindWithItems = new ContactToRemindWithItems
                        {
                            CustomerID = thatMayNeedNextWeek[index1].CustomerData.CustomerID,
                            CompanyName = thatMayNeedNextWeek[index1].CustomerData.CompanyName,
                            ContactFirstName = thatMayNeedNextWeek[index1].CustomerData.ContactFirstName,
                            ContactAltFirstName = thatMayNeedNextWeek[index1].CustomerData.ContactAltFirstName,
                            EmailAddress = thatMayNeedNextWeek[index1].CustomerData.EmailAddress,
                            AltEmailAddress = thatMayNeedNextWeek[index1].CustomerData.AltEmailAddress,
                            CityID = thatMayNeedNextWeek[index1].CustomerData.City,
                            CustomerTypeID = thatMayNeedNextWeek[index1].CustomerData.CustomerTypeID,
                            enabled = thatMayNeedNextWeek[index1].CustomerData.enabled,
                            EquipTypeID = thatMayNeedNextWeek[index1].CustomerData.EquipType,
                            TypicallySecToo = thatMayNeedNextWeek[index1].CustomerData.TypicallySecToo,
                            PreferedAgentID = thatMayNeedNextWeek[index1].CustomerData.PreferedAgent,
                            SalesAgentID = thatMayNeedNextWeek[index1].CustomerData.SalesAgentID,
                            UsesFilter = thatMayNeedNextWeek[index1].CustomerData.UsesFilter,
                            AlwaysSendChkUp = thatMayNeedNextWeek[index1].CustomerData.AlwaysSendChkUp,
                            RequiresPurchOrder = thatMayNeedNextWeek[index1].RequiresPurchOrder,
                            ReminderCount = thatMayNeedNextWeek[index1].CustomerData.ReminderCount,
                            NextPrepDate = thatMayNeedNextWeek[index1].NextRoastDateByCityData.PrepDate.Date,
                            NextDeliveryDate = thatMayNeedNextWeek[index1].NextRoastDateByCityData.DeliveryDate.Date,
                            NextCoffee = thatMayNeedNextWeek[index1].ClientUsageData.NextCoffeeBy.Date,
                            NextClean = thatMayNeedNextWeek[index1].ClientUsageData.NextCleanOn.Date,
                            NextDescal = thatMayNeedNextWeek[index1].ClientUsageData.NextDescaleEst.Date,
                            NextFilter = thatMayNeedNextWeek[index1].ClientUsageData.NextFilterEst.Date,
                            NextService = thatMayNeedNextWeek[index1].ClientUsageData.NextServiceEst.Date
                        };

                        // Process service items for this customer - ONLY LAST ORDERED ITEMS (no recurring)
                        ItemUsageTbl itemUsageTbl = new ItemUsageTbl();
                        bool addedAnyItems = false;

                        for (int index2 = 0; index2 < byCustomerTypeId.Count; ++index2)
                        {
                            DateTime serviceDate;
                            switch (byCustomerTypeId[index2].ServiceTypeID)
                            {
                                case 1: serviceDate = toRemindWithItems.NextClean; break;
                                case 2: serviceDate = toRemindWithItems.NextCoffee; break;
                                case 4: serviceDate = toRemindWithItems.NextDescal; break;
                                case 5: serviceDate = toRemindWithItems.NextFilter; break;
                                case 10: serviceDate = toRemindWithItems.NextService; break;
                                default: serviceDate = DateTime.MaxValue; break;
                            }

                            // Check if service is due within delivery window
                            if (serviceDate > TimeZoneUtils.Now().AddYears(-1) &&
                                serviceDate <= thatMayNeedNextWeek[index1].NextRoastDateByCityData.DeliveryDate)
                            {
                                List<ItemUsageTbl> lastItemsUsed = itemUsageTbl.GetLastItemsUsed(
                                    thatMayNeedNextWeek[index1].CustomerData.CustomerID,
                                    byCustomerTypeId[index2].ServiceTypeID);

                                // Add items this customer typically uses - ONLY LAST ORDERED ITEMS
                                for (int index3 = 0; index3 < lastItemsUsed.Count; ++index3)
                                {
                                    ItemContactRequires itemRequired = new ItemContactRequires
                                    {
                                        CustomerID = thatMayNeedNextWeek[index1].CustomerData.CustomerID,
                                        AutoFulfill = thatMayNeedNextWeek[index1].CustomerData.autofulfill,
                                        ReoccurID = 0, // NOT a recurring item
                                        ReoccurOrder = false, // NOT a recurring order
                                        ItemID = lastItemsUsed[index3].ItemProvidedID,
                                        ItemQty = lastItemsUsed[index3].AmountProvided,
                                        ItemPackagID = lastItemsUsed[index3].PackagingID
                                    };

                                    toRemindWithItems.ItemsContactRequires.Add(itemRequired);
                                    addedAnyItems = true;
                                }
                            }
                        }

                        // Only add customer if they have items
                        if (addedAnyItems)
                        {
                            pContactsToRemind.Add(toRemindWithItems);
                            AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Added reminder customer {toRemindWithItems.CompanyName} with {toRemindWithItems.ItemsContactRequires.Count} last-ordered items");
                        }
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error processing contact {index1}: {ex.Message}");
                        // Continue with next contact
                    }
                }

                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Final reminder contact list has {pContactsToRemind.Count} customers (excluding recurring customers)");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error in AddAllContactsToRemind: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sends a batch of reminders for contacts of the same type - moved from code-behind
        /// </summary>
        private BatchSendResult SendReminderBatch(List<ContactToRemindWithItems> contacts, string batchType, SendCheckEmailTextsData emailData)
        {
            var result = new BatchSendResult();

            try
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Processing {batchType} batch with {contacts.Count} contacts");

                // Track individual email attempts
                int emailsAdded = 0;
                int emailsFailed = 0;

                // Add all contacts to the batch
                foreach (var contact in contacts)
                {
                    try
                    {
                        var emailTextData = new SendCheckEmailTextsData
                        {
                            Header = emailData.Header,
                            Body = emailData.Body,
                            Footer = emailData.Footer
                        };

                        // Handle order creation for non-reminder contacts
                        string orderType = GetOrderType(contact);
                        if (!string.IsNullOrWhiteSpace(orderType))
                        {
                            var testEmailClient = new EmailMailKitCls();
                            bool isTestMode = testEmailClient.IsTestMode;

                            string orderResult = CreateOrderForContact(contact, orderType, out bool hasAutoFulfill, out bool hasRecurring);
                            if (!string.IsNullOrEmpty(orderResult))
                            {
                                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Order creation failed for {contact.CompanyName}: {orderResult}");
                                // Continue with email even if order creation fails
                            }
                            else
                            {
                                emailTextData.Footer += MessageProvider.Get(MessageKeys.CoffeeCheckup.FooterOrderAdded);

                                string baseUrl = DisableClientManager.GetApplicationUrl();
                                string orderLink = $"{baseUrl}/Pages/OrderDetail.aspx?CustomerID={contact.CustomerID}&DeliveryDate={contact.NextDeliveryDate:yyyy-MM-dd}";
                                emailTextData.Footer += string.Format(MessageProvider.Get(MessageKeys.CoffeeCheckup.FooterOrderLink), orderLink);

                                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Order created successfully for {contact.CompanyName}");
                            }
                        }

                        // Add final warning if needed
                        if (contact.ReminderCount == 6)
                        {
                            emailTextData.Body = MessageProvider.Get(MessageKeys.CoffeeCheckup.BodyFinalWarning) + emailTextData.Body;
                        }

                        string emailSubject = _emailManager.GetEmailSubject(orderType);
                        _emailManager.AddEmailToBatch(contact, emailTextData, orderType, emailSubject);
                        emailsAdded++;

                        // Log the reminder attempt
                        LogReminderAttempt(contact, orderType, true);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Failed to add {contact.CompanyName} to batch: {ex.Message}");
                        LogFailedEmail(contact.CompanyName, ex.Message);
                        emailsFailed++;
                        LogReminderAttempt(contact, GetOrderType(contact), false);
                    }
                }

                // Send the entire batch
                if (emailsAdded > 0)
                {
                    var batchResult = _emailManager.SendBatch();

                    if (batchResult.IsSuccess)
                    {
                        result.TotalSent = emailsAdded;
                        result.TotalFailed = emailsFailed;
                    }
                    else
                    {
                        // If batch failed, all emails failed
                        result.TotalSent = 0;
                        result.TotalFailed = emailsAdded + emailsFailed;
                        LogFailedBatch(batchType, batchResult.ErrorMessage);
                    }
                }
                else
                {
                    result.TotalSent = 0;
                    result.TotalFailed = emailsFailed;
                }

                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: {batchType} batch result: {result.TotalSent} sent, {result.TotalFailed} failed");

                return result;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error in {batchType} batch processing: {ex.Message}");
                LogFailedBatch(batchType, ex.Message);
                return new BatchSendResult
                {
                    TotalSent = 0,
                    TotalFailed = contacts.Count,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Creates orders for contacts with auto-fulfill or recurring items - ENHANCED WITH DELIVERY DATE CALCULATION
        /// </summary>
        private string CreateOrderForContact(ContactToRemindWithItems pContact, string pOrderType, out bool hasAutoFulfillItem, out bool hasRecurringItems)
        {
            hasAutoFulfillItem = false;
            hasRecurringItems = false;

            try
            {
                // Determine if this is a recurring order batch
                bool isRecurringBatch = pOrderType.Contains("recurring") || pOrderType.Contains("Recurring");

                // Calculate optimal delivery dates if this is recurring
                DateTime optimalRoastDate = pContact.NextPrepDate.Date;
                DateTime optimalDeliveryDate = pContact.NextDeliveryDate.Date;

                if (isRecurringBatch)
                {
                    // Find monthly recurring orders to get the target day of month
                    var monthlyRecurringItems = pContact.ItemsContactRequires
                        .Where(item => item.ReoccurOrder && HasMonthlyRecurrence(item.ReoccurID))
                        .ToList();

                    if (monthlyRecurringItems.Any())
                    {
                        // Get the target day of month from the first monthly recurring item
                        int targetDayOfMonth = GetTargetDayOfMonth(monthlyRecurringItems.First().ReoccurID);

                        if (targetDayOfMonth > 0)
                        {
                            // Calculate optimal delivery date for this target day
                            optimalDeliveryDate = _deliveryCalculator.CalculateOptimalMonthlyDeliveryDate(
                                pContact.CustomerID,
                                targetDayOfMonth,
                                DateTime.Now.AddMonths(-1)); // Use last month as base

                            // Calculate roast date (typically delivery date minus prep days)
                            optimalRoastDate = _deliveryCalculator.CalculateRoastDateFromDelivery(optimalDeliveryDate);

                            AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, MessageProvider.Format(
                                MessageKeys.DeliveryCalculation.CalculatedOptimalDates,
                                pContact.CompanyName,
                                targetDayOfMonth,
                                optimalRoastDate,
                                optimalDeliveryDate));
                        }
                    }
                }

                OrderTblData pOrderData = new OrderTblData
                {
                    CustomerID = pContact.CustomerID,
                    OrderDate = TimeZoneUtils.Now().Date,
                    RoastDate = optimalRoastDate,           // ENHANCED: Use calculated date
                    RequiredByDate = optimalDeliveryDate,   // ENHANCED: Use calculated date
                    ToBeDeliveredBy = pContact.PreferedAgentID < 0 ? 3 : pContact.PreferedAgentID,
                    Confirmed = false,
                    InvoiceDone = false,
                    PurchaseOrder = string.Empty,
                    Notes = $"{pOrderType} - Optimal delivery calculated"
                };

                // BUG FIX: Check test mode before database operations
                var testEmailClient = new EmailMailKitCls();
                bool isTestMode = testEmailClient.IsTestMode;
                //if (isTestMode)
                //{
                //    // In test mode, we skip actual order creation but log the action
                //    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: TEST MODE - Skipping order creation for {pContact.CompanyName}");
                //    return string.Empty; // Success in test mode
                //}
                //else
                //{
                //    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Creating order for {pContact.CompanyName} with type {pOrderType}");
                //}

                // Still determine order types for email purposes, but don't create actual orders
                for (int index = 0; index < pContact.ItemsContactRequires.Count; ++index)
                {
                    if (pContact.ItemsContactRequires[index].ReoccurOrder)
                        hasRecurringItems = true;

                    if (pContact.ItemsContactRequires[index].AutoFulfill)
                        hasAutoFulfillItem = true;
                }


                ReoccuringOrderDAL reoccuringOrderDal = new ReoccuringOrderDAL();
                OrderTbl orderTbl = new OrderTbl();
                string errorMessage = string.Empty;

                for (int index = 0; index < pContact.ItemsContactRequires.Count && string.IsNullOrEmpty(errorMessage); ++index)
                {
                    pOrderData.ItemTypeID = pContact.ItemsContactRequires[index].ItemID;
                    pOrderData.QuantityOrdered = pContact.ItemsContactRequires[index].ItemQty;
                    pOrderData.PackagingID = pContact.ItemsContactRequires[index].ItemPackagID;
                    pOrderData.PrepTypeID = pContact.ItemsContactRequires[index].ItemPrepID;

                    errorMessage = orderTbl.InsertNewOrderLine(pOrderData);

                    if (pContact.ItemsContactRequires[index].ReoccurOrder)
                    {
                        // This matches the logic from ReoccuringOrderDetails page
                        DateTime dateToSet = pOrderData.OrderDate; // Use order date, not prep date

                        AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Updating recurring order {pContact.ItemsContactRequires[index].ReoccurID} last date to {dateToSet:yyyy-MM-dd}");

                        /// update only the next order date so we do not resend.
                        reoccuringOrderDal.SetReoccuringOrderDates(dateToSet, pContact.ItemsContactRequires[index].ReoccurID);  
                    }

                    if (pContact.ItemsContactRequires[index].AutoFulfill)
                    {
                        hasAutoFulfillItem = true;
                    }
                }

                return errorMessage;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error creating order for {pContact.CompanyName}: {ex.Message}");
                return ex.Message;
            }
        }

        /// <summary>
        /// Logs reminder attempt to database - CONDITIONAL TEST MODE LOGGING
        /// </summary>
        private void LogReminderAttempt(ContactToRemindWithItems contact, string orderType, bool wasSuccessful)
        {
            try
            {
                var testEmailClient = new EmailMailKitCls();
                bool isTestMode = testEmailClient.IsTestMode;

                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: LogReminderAttempt for {contact.CompanyName} - TestMode: {isTestMode}, wasSuccessful: {wasSuccessful}");

                // In test mode, we still log because emails are actually being sent (just to test recipients)
                // If you want to skip logging in test mode, uncomment the next block:
                /*
                if (isTestMode)
                {
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: TEST MODE - Skipped logging reminder attempt for customer {contact.CustomerID} ({contact.CompanyName})");
                    return;
                }
                */

                // Determine if this customer has recurring or auto-fulfill items
                bool hasRecurring = contact.ItemsContactRequires.Any(x => x.ReoccurOrder);
                bool hasAutoFulFill = contact.ItemsContactRequires.Any(x => x.AutoFulfill);

                var logEntry = new SentRemindersLogTbl
                {
                    CustomerID = contact.CustomerID,
                    DateSentReminder = TimeZoneUtils.Now().Date,
                    NextPrepDate = contact.NextCoffee.Date,
                    ReminderSent = wasSuccessful,
                    HadAutoFulfilItem = hasAutoFulFill,
                    HadReoccurItems = hasRecurring
                };

                string logMode = isTestMode ? "[TEST MODE]" : "[PRODUCTION]";
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: {logMode} Logging reminder for {contact.CompanyName}");

                logEntry.InsertLogItem(logEntry);

                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: {logMode} Successfully logged reminder for {contact.CompanyName} - Recurring: {hasRecurring}, AutoFulFill: {hasAutoFulFill}, Success: {wasSuccessful}");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Failed to log reminder attempt for customer {contact.CustomerID} ({contact.CompanyName}): {ex.Message}");
            }
        }

        /// <summary>
        /// Logs individual email failure
        /// </summary>
        private void LogFailedEmail(string customerName, string errorMessage)
        {
            AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: EMAIL FAILED: {customerName} - {errorMessage}");
        }

        /// <summary>
        /// Logs batch failure
        /// </summary>
        private void LogFailedBatch(string batchType, string errorMessage)
        {
            AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: BATCH FAILED: {batchType} - {errorMessage}");
        }

        /// <summary>
        /// Logs failed customers for display in SentRemindersSheet
        /// </summary>
        public void LogFailedCustomers(List<string> failedContacts)
        {
            if (!failedContacts.Any()) return;

            try
            {
                foreach (string failure in failedContacts)
                {
                    AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: FAILED CUSTOMER: {failure}");
                }

                // Store in session for SentRemindersSheet to display
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session["CoffeeCheckupFailures"] = failedContacts;
                    HttpContext.Current.Session["CoffeeCheckupFailureDate"] = TimeZoneUtils.Now().Date;
                }
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error logging failed customers: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets eligible customers using database-driven approach via OrderCheckTbl control
        /// </summary>
        private List<ContactToRemindWithItems> GetEligibleCustomersFromDatabase()
        {
            try
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, "CoffeeCheckupManager: Getting eligible customers using OrderCheckTbl");

                var orderCheckTbl = new OrderCheckTbl();
                var databaseCustomers = orderCheckTbl.GetCustomersWithoutOrderConflicts(CONST_MAXREMINDERS);

                var eligibleCustomers = new List<ContactToRemindWithItems>();

                foreach (var dbCustomer in databaseCustomers)
                {
                    // Convert database model to contact model
                    var contact = new ContactToRemindWithItems
                    {
                        CustomerID = dbCustomer.CustomerID,
                        CompanyName = dbCustomer.CompanyName,
                        ContactFirstName = dbCustomer.ContactFirstName,
                        ContactAltFirstName = dbCustomer.ContactAltFirstName,
                        EmailAddress = dbCustomer.EmailAddress,
                        AltEmailAddress = dbCustomer.AltEmailAddress,
                        CityID = dbCustomer.CityID,
                        CustomerTypeID = dbCustomer.CustomerTypeID,
                        enabled = dbCustomer.Enabled,
                        EquipTypeID = dbCustomer.EquipTypeID,
                        TypicallySecToo = dbCustomer.TypicallySecToo,
                        PreferedAgentID = dbCustomer.PreferedAgentID,
                        SalesAgentID = dbCustomer.SalesAgentID,
                        UsesFilter = dbCustomer.UsesFilter,
                        AlwaysSendChkUp = dbCustomer.AlwaysSendChkUp,
                        ReminderCount = dbCustomer.ReminderCount,
                        NextPrepDate = dbCustomer.NextPrepDate,
                        NextDeliveryDate = dbCustomer.NextDeliveryDate,
                        NextCoffee = dbCustomer.NextCoffee,
                        NextClean = dbCustomer.NextClean,
                        NextDescal = dbCustomer.NextDescal,
                        NextFilter = dbCustomer.NextFilter,
                        NextService = dbCustomer.NextService
                    };

                    // Get typical items for this customer
                    var typicalItems = orderCheckTbl.GetCustomerTypicalItems(dbCustomer.CustomerID);
                    contact.ItemsContactRequires = typicalItems.Select(item => new ItemContactRequires
                    {
                        CustomerID = dbCustomer.CustomerID,
                        AutoFulfill = dbCustomer.AutoFulfill,
                        ReoccurID = 0,
                        ReoccurOrder = false,
                        ItemID = item.ItemID,
                        ItemQty = item.Quantity,
                        ItemPackagID = item.PackagingID
                    }).ToList();

                    eligibleCustomers.Add(contact);
                }

                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Database query returned {eligibleCustomers.Count} eligible customers");
                return eligibleCustomers;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Database query failed: {ex.Message}");
                // Return null to trigger fallback to existing method
                return null;
            }
        }

        /// <summary>
        /// Checks if a recurring order is monthly (day-of-month) type
        /// </summary>
        private bool HasMonthlyRecurrence(int reoccurId)
        {
            try
            {
                var reoccuringOrderDal = new ReoccuringOrderDAL();
                var recurringOrder = reoccuringOrderDal.GetByReoccuringOrderByID(reoccurId);
                if (recurringOrder != null)
                {
                    var recurrenceType = ReoccuranceTypeTbl.GetRecurrenceType(recurringOrder.ReoccuranceTypeID);
                    return recurrenceType == ReoccuranceTypeTbl.RecurrenceType.Monthly;
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error checking monthly recurrence for {reoccurId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the target day of month for a recurring order
        /// </summary>
        private int GetTargetDayOfMonth(int reoccurId)
        {
            try
            {
                var reoccuringOrderDal = new ReoccuringOrderDAL();
                var recurringOrder = reoccuringOrderDal.GetByReoccuringOrderByID(reoccurId);
                return recurringOrder?.ReoccuranceValue ?? 0;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.SendCheckup, $"CoffeeCheckupManager: Error getting target day for {reoccurId}: {ex.Message}");
                return 0;
            }
        }

        public static int GetReminderWindowDays()
        {
            int days = CONST_DEFAULTREMINDERWINDOWDAYS; // default

            // Check session first
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                var sessionVal = HttpContext.Current.Session["CoffeeCheckupReminderWindowDays"] as string;
                if (!string.IsNullOrEmpty(sessionVal) && int.TryParse(sessionVal, out int sessionDays) && sessionDays > 0)
                    return sessionDays;
            }

            var setting = ConfigurationManager.AppSettings["CoffeeCheckupReminderWindowDays"];
            if (!string.IsNullOrEmpty(setting) && int.TryParse(setting, out int parsed) && parsed > 0)
                days = parsed;
            return days;
        }

        /// <summary>
        /// Gets the minimum number of days between monthly recurring orders
        /// </summary>
        public static int GetMinimumRecurringDays()
        {
            int days = CONST_DEFAULTMINIMUMMONTHLYRECURRINGDAYS; // default

            // Check app settings first
            var setting = ConfigurationManager.AppSettings["CoffeeCheckupMinMonthlyRecurringDays"];
            if (!string.IsNullOrEmpty(setting) && int.TryParse(setting, out int parsed) && parsed > 0)
                days = parsed;

            return days;
        }
    }
}
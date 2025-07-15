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
        private readonly DeliveryDateCalculator _deliveryCalculator; // ADD THIS

        // Constants moved from code-behind for better organization
        private const int CONST_FORCEREMINDERDELAYCOUNT = 4;
        private const int CONST_MAXREMINDERS = 7;

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
            _deliveryCalculator = new DeliveryDateCalculator(); // ADD THIS
        }

        /// <summary>
        /// Main entry point for processing coffee checkup reminders
        /// ENHANCED WITH PERFORMANCE MONITORING
        /// </summary>
        public BatchSendResult ProcessCoffeeCheckupReminders(SendCheckEmailTextsData emailData)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // MISSING: Stopwatch declaration

            try
            {
                AppLogger.WriteLog("email", "CoffeeCheckupManager: Starting coffee checkup process");

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

                // MISSING: Pre-warm caches before processing
                WarmupCaches();

                // 2. Get eligible customers
                var eligibleCustomers = GetEligibleCustomers();

                if (!eligibleCustomers.Any())
                {
                    AppLogger.WriteLog("email", "CoffeeCheckupManager: No eligible customers found");
                    return new BatchSendResult
                    {
                        IsSuccess = true,
                        TotalSent = 0,
                        TotalFailed = 0,
                        ErrorMessage = "No customers found requiring reminders"
                    };
                }

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Processing {eligibleCustomers.Count} eligible customers");

                // 3. Process batch reminders
                var result = ProcessRemindersBatch(eligibleCustomers, emailData);

                stopwatch.Stop();
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Process completed in {stopwatch.ElapsedMilliseconds}ms - {result.TotalSent} sent, {result.TotalFailed} failed");

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Process failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
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
            AppLogger.WriteLog("email", $"CoffeeCheckupManager: Processing batch with {allContacts.Count} contacts");

            var totalResult = new BatchSendResult();
            var customersTbl = new CustomersTbl();

            // Check if we're in test mode
            var testEmailClient = new EmailMailKitCls();
            bool isTestMode = testEmailClient.IsTestMode;

            if (isTestMode)
            {
                AppLogger.WriteLog("email", "CoffeeCheckupManager: TEST MODE - Database updates will be skipped");
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
                    AppLogger.WriteLog("email", $"Processing contact {processed}: {contact.CompanyName} (ID: {contact.CustomerID})");

                    string orderType = GetOrderType(contact);

                    // Update customer data if not in test mode
                    if (!isTestMode)
                    {
                        if (!UpdateCustomerReminderData(contact, customersTbl))
                        {
                            failedContacts.Add($"{contact.CompanyName} - Database update failed");
                            continue;
                        }
                    }

                    // Categorize for batch processing
                    CategorizeContact(contact, orderType, recurringContacts, autoFulfillContacts, reminderOnlyContacts);
                }
                catch (Exception ex)
                {
                    AppLogger.WriteLog("email", $"Error processing contact {contact.CompanyName}: {ex.Message}");
                    failedContacts.Add($"{contact.CompanyName} - Processing error: {ex.Message}");
                }
            }

            // Send batches by type
            totalResult = SendAllBatches(recurringContacts, autoFulfillContacts, reminderOnlyContacts, emailData);

            // Add pre-processing failures
            totalResult.TotalFailed += failedContacts.Count;
            totalResult.IsSuccess = totalResult.TotalFailed == 0;

            AppLogger.WriteLog("email", $"CoffeeCheckupManager: Batch completed - {totalResult.TotalSent} sent, {totalResult.TotalFailed} failed");
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
                    AppLogger.WriteLog("email", $"Customer {contact.CompanyName} disabled - exceeded max reminder limit");
                    return false;
                }
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"Database update failed for {contact.CompanyName}: {ex.Message}");
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
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Categorized {contact.CompanyName} as REMINDER ONLY");
            }
            else if (orderType.Contains("recurring"))
            {
                recurringContacts.Add(contact);
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Categorized {contact.CompanyName} as RECURRING");
            }
            else if (orderType.Contains("autofulfill") || orderType.Contains("auto"))
            {
                autoFulfillContacts.Add(contact);
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Categorized {contact.CompanyName} as AUTOFULFILL");
            }
            else
            {
                // Default to reminder only if orderType is not recognized
                reminderOnlyContacts.Add(contact);
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Categorized {contact.CompanyName} as REMINDER ONLY (unknown order type: '{orderType}')");
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
        public void PrepareCustomerReminderData()
        {
            try
            {
                AppLogger.WriteLog("email", "CoffeeCheckupManager: Preparing customer reminder data");

                // Ensure roast dates are current
                var trackerTools = new TrackerTools();
                if (!trackerTools.IsNextRoastDateByCityTodays())
                {
                    trackerTools.SetNextRoastDateByCity();
                }

                // Build reminder list and populate temp tables
                SetListOfContactsToSendReminderTo();

                AppLogger.WriteLog("email", "CoffeeCheckupManager: Customer reminder data preparation completed");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error preparing customer data: {ex.Message}");
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
                }

                if (!_cachedItemDescriptions.ContainsKey(itemId))
                {
                    try
                    {
                        _cachedItemDescriptions[itemId] = new ItemTypeTbl().GetItemTypeDesc(itemId);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error caching item description for ID {itemId}: {ex.Message}");
                        _cachedItemDescriptions[itemId] = $"Item {itemId}"; // Fallback
                    }
                }

                return _cachedItemDescriptions[itemId];
            }
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
                }

                if (!_cachedItemSKUs.ContainsKey(itemId))
                {
                    try
                    {
                        _cachedItemSKUs[itemId] = new ItemTypeTbl().GetItemTypeSKU(itemId);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error caching item SKU for ID {itemId}: {ex.Message}");
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
                }

                if (!_cachedCityNames.ContainsKey(cityId))
                {
                    try
                    {
                        _cachedCityNames[cityId] = new CityTblDAL().GetCityName(cityId);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error caching city name for ID {cityId}: {ex.Message}");
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
                }

                if (!_cachedPackagingDescriptions.ContainsKey(packagingId))
                {
                    try
                    {
                        _cachedPackagingDescriptions[packagingId] = new PackagingTbl().GetPackagingDesc(packagingId);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error caching packaging description for ID {packagingId}: {ex.Message}");
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
                AppLogger.WriteLog("email", "CoffeeCheckupManager: Cache invalidated");
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
                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error caching item UoM for ID {itemId}: {ex.Message}");
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
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Cache warmup completed in {warmupStopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Cache warmup failed: {ex.Message}");
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
                    AppLogger.WriteLog("email", $"Customer {customer.CompanyName} is internal - skipping");
                    return false;
                }

                // Check if has valid email
                if (!HasValidEmailAddress(customer))
                {
                    AppLogger.WriteLog("email", $"Customer {customer.CompanyName} has no valid email address");
                    return false;
                }

                // Check if within reminder limits
                if (!IsEligibleForReminder(customer))
                {
                    AppLogger.WriteLog("email", $"Customer {customer.CompanyName} not eligible for reminder (disabled or exceeded limits)");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"Error validating customer {customer.CompanyName}: {ex.Message}");
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

                // QUICK WIN: Get cached internal customer list once
                var internalCustomerIds = GetCachedInternalCustomerIds();

                // Use existing method but with optimizations
                var allContacts = new TempCoffeeCheckup().GetAllContactAndItems();

                // QUICK WIN: Filter in memory instead of multiple DB calls per customer
                var eligibleContacts = allContacts.Where(contact =>
                    contact.enabled &&
                    contact.ReminderCount < CONST_MAXREMINDERS &&
                    !internalCustomerIds.Contains((int)contact.CustomerID) &&
                    HasValidEmailAddressOptimized(contact)
                ).ToList();

                stopwatch.Stop();
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Customer filtering completed in {stopwatch.ElapsedMilliseconds}ms - {eligibleContacts.Count} of {allContacts.Count} eligible");

                return eligibleContacts;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"Error getting eligible customers: {ex.Message}");
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
                        _cacheExpiry = DateTime.Now.AddHours(1); // Cache for 1 hour
                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Cached {_cachedInternalCustomerIds.Count} internal customer IDs");
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error caching internal customer IDs: {ex.Message}");
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
                AppLogger.WriteLog("email", $"Error checking internal customer status: {ex.Message}");
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
        private void SetListOfContactsToSendReminderTo()
        {
            try
            {
                // Step 1: Get recurring contacts (with ONLY recurring items)
                List<ContactToRemindWithItems> recurringContacts = GetRecurringContacts();

                // Step 2: Create a hashset of customer IDs that have recurring orders to exclude them from reminder processing
                var recurringCustomerIds = new HashSet<long>(recurringContacts.Select(x => x.CustomerID));

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Found {recurringCustomerIds.Count} customers with recurring orders - they will be excluded from reminder processing");

                // Step 3: Get reminder contacts (excluding customers with recurring orders)
                List<ContactToRemindWithItems> reminderContacts = new List<ContactToRemindWithItems>();
                AddAllContactsToRemind(ref reminderContacts, recurringCustomerIds);

                // Step 4: Combine the lists (recurring + reminder, but no overlap)
                List<ContactToRemindWithItems> allContacts = new List<ContactToRemindWithItems>();
                allContacts.AddRange(recurringContacts);
                allContacts.AddRange(reminderContacts);

                allContacts.Sort((a, b) => string.Compare(a.CompanyName, b.CompanyName));

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Final contact list - {recurringContacts.Count} recurring customers, {reminderContacts.Count} reminder customers, {allContacts.Count} total");

                TempCoffeeCheckup tempCoffeeCheckup = new TempCoffeeCheckup();
                if (!tempCoffeeCheckup.DeleteAllContactRecords() || !tempCoffeeCheckup.DeleteAllContactItems())
                {
                    AppLogger.WriteLog("email", "CoffeeCheckupManager: Error deleting old temp tables");
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

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Successfully processed {allContacts.Count} contacts for reminder data");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error in SetListOfContactsToSendReminderTo: {ex.Message}");
                throw;
            }
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
                    AppLogger.WriteLog("email", $"CoffeeCheckupManager: Customer {customerId} has {orders.Count} coffee orders in date range {checkStartDate:yyyy-MM-dd} to {checkEndDate:yyyy-MM-dd}");
                }

                return hasConflicts;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error checking order conflicts for customer {customerId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets recurring contacts that need reminders - ENHANCED WITH BETTER ORDER CONFLICT DETECTION
        /// </summary>
        private List<ContactToRemindWithItems> GetRecurringContacts()
        {
            List<ContactToRemindWithItems> reocurringContacts = new List<ContactToRemindWithItems>();

            try
            {
                TrackerTools trackerTools = new TrackerTools();
                DateTime minValue1 = DateTime.MinValue;
                DateTime minReminderDate = new SysDataTbl().GetMinReminderDate();
                DateTime sevenDaysFromNow = TimeZoneUtils.Now().Date.AddDays(7);
                ReoccuringOrderDAL reoccuringOrderDal = new ReoccuringOrderDAL();

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Checking recurring items with 7-day lookahead until {sevenDaysFromNow:yyyy-MM-dd}");

                if (!reoccuringOrderDal.SetReoccuringItemsLastDate())
                {
                    AppLogger.WriteLog("email", "CoffeeCheckupManager: Could not set the recurring last date");
                    return reocurringContacts;
                }

                List<ReoccuringOrderExtData> all = reoccuringOrderDal.GetAll(1, "CustomersTbl.CustomerID");

                if (all == null || !all.Any())
                {
                    AppLogger.WriteLog("email", "CoffeeCheckupManager: No recurring orders found to process");
                    return reocurringContacts;
                }

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Processing {all.Count} recurring order patterns");

                for (int index1 = 0; index1 < all.Count; ++index1)
                {
                    try
                    {
                        // Validate recurrence type first
                        if (!ReoccuranceTypeTbl.IsValidRecurrenceType(all[index1].ReoccuranceTypeID))
                        {
                            AppLogger.WriteLog("email", $"CoffeeCheckupManager: Unknown recurrence type {all[index1].ReoccuranceTypeID} for recurring order {all[index1].ReoccuringOrderID}");
                            continue;
                        }

                        // Calculate next date required using centralized enum
                        ReoccuranceTypeTbl.RecurrenceType recurrenceType = ReoccuranceTypeTbl.GetRecurrenceType(all[index1].ReoccuranceTypeID);

                        switch (recurrenceType)
                        {
                            case ReoccuranceTypeTbl.RecurrenceType.Weekly:
                                all[index1].NextDateRequired = all[index1].DateLastDone.AddDays((double)(all[index1].ReoccuranceValue * 7)).Date;
                                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Weekly recurring item {all[index1].ReoccuringOrderID} - every {all[index1].ReoccuranceValue} weeks, next due: {all[index1].NextDateRequired:yyyy-MM-dd}");
                                break;

                            case ReoccuranceTypeTbl.RecurrenceType.Monthly:
                                // Calculate next monthly occurrence - ENHANCED for specific day-of-month
                                DateTime nextMonth = all[index1].DateLastDone.AddMonths(1);
                                try
                                {
                                    // Try to set to specific day of month (ReoccuranceValue)
                                    all[index1].NextDateRequired = new DateTime(nextMonth.Year, nextMonth.Month, all[index1].ReoccuranceValue).Date;
                                    AppLogger.WriteLog("email", $"CoffeeCheckupManager: Monthly recurring item {all[index1].ReoccuringOrderID} - day {all[index1].ReoccuranceValue} of month, next due: {all[index1].NextDateRequired:yyyy-MM-dd}");
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    // Handle months with fewer days (e.g., Feb 31st → Feb 28th)
                                    int daysInMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
                                    int targetDay = Math.Min(all[index1].ReoccuranceValue, daysInMonth);
                                    all[index1].NextDateRequired = new DateTime(nextMonth.Year, nextMonth.Month, targetDay).Date;
                                    AppLogger.WriteLog("email", $"CoffeeCheckupManager: Adjusted monthly recurring item {all[index1].ReoccuringOrderID} from day {all[index1].ReoccuranceValue} to {targetDay} for {nextMonth:yyyy-MM}, next due: {all[index1].NextDateRequired:yyyy-MM-dd}");
                                }
                                break;

                            default:
                                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Unsupported recurrence type {recurrenceType} for recurring order {all[index1].ReoccuringOrderID}");
                                continue;
                        }

                        // Check if recurring order is still valid
                        if (all[index1].RequireUntilDate > TrackerTools.STATIC_TrackerMinDate && all[index1].NextDateRequired > all[index1].RequireUntilDate)
                        {
                            all[index1].Enabled = false;
                            AppLogger.WriteLog("email", $"CoffeeCheckupManager: Recurring order {all[index1].ReoccuringOrderID} expired on {all[index1].RequireUntilDate:yyyy-MM-dd}");
                            continue;
                        }

                        // Get next roast date for this customer
                        DateTime sourceDateTime;
                        try
                        {
                            sourceDateTime = trackerTools.GetNextRoastDateByCustomerID(all[index1].CustomerID, ref minValue1);

                            if (sourceDateTime == DateTime.MinValue || sourceDateTime < TimeZoneUtils.Now().Date.AddDays(-30))
                            {
                                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Invalid roast date for customer {all[index1].CustomerID}, using minimum reminder date");
                                sourceDateTime = minReminderDate;
                            }
                        }
                        catch (Exception ex)
                        {
                            AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error getting roast date for customer {all[index1].CustomerID}: {ex.Message}");
                            sourceDateTime = minReminderDate;
                        }

                        if (sourceDateTime < minReminderDate)
                            sourceDateTime = minReminderDate;

                        // BUG FIX: Enhanced 7-day window check
                        bool isDueNow = all[index1].NextDateRequired <= sourceDateTime;
                        bool isDueWithin7Days = all[index1].NextDateRequired <= sevenDaysFromNow;

                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Recurring item {all[index1].ReoccuringOrderID} - Next due: {all[index1].NextDateRequired:yyyy-MM-dd}, Roast date: {sourceDateTime:yyyy-MM-dd}, Due now: {isDueNow}, Due within 7 days: {isDueWithin7Days}");

                        // Check if this recurring item is due (either now or within 7 days)
                        if (isDueNow || isDueWithin7Days)
                        {
                            // ENHANCED: Better order conflict detection
                            DateTime checkStartDate = TimeZoneUtils.Now().Date;
                            DateTime checkEndDate = sevenDaysFromNow;

                            if (!HasConflictingOrders(all[index1].CustomerID, all[index1].ItemRequiredID, checkStartDate, checkEndDate))
                            {
                                // No conflicting orders, add to recurring contacts
                                ItemContactRequires itemRequired = new ItemContactRequires
                                {
                                    CustomerID = all[index1].CustomerID,
                                    AutoFulfill = false,
                                    ReoccurID = all[index1].ReoccuringOrderID,
                                    ReoccurOrder = true,
                                    ItemID = all[index1].ItemRequiredID,
                                    ItemQty = all[index1].QtyRequired,
                                    ItemPackagID = all[index1].PackagingID
                                };

                                // Find or create customer contact
                                if (!reocurringContacts.Exists(x => x.CustomerID == itemRequired.CustomerID))
                                {
                                    ContactToRemindWithItems customerDetails = new ContactToRemindWithItems().GetCustomerDetails(itemRequired.CustomerID);
                                    if (customerDetails != null)
                                    {
                                        customerDetails.ItemsContactRequires.Add(itemRequired);
                                        reocurringContacts.Add(customerDetails);
                                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Added customer {customerDetails.CompanyName} for recurring item {all[index1].ItemRequiredID}");
                                    }
                                }
                                else
                                {
                                    int index2 = reocurringContacts.FindIndex(x => x.CustomerID == itemRequired.CustomerID);
                                    if (index2 >= 0)
                                    {
                                        reocurringContacts[index2].ItemsContactRequires.Add(itemRequired);
                                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Added item {all[index1].ItemRequiredID} to existing customer {all[index1].CustomerID}");
                                    }
                                }
                            }
                            else
                            {
                                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Skipping recurring item {all[index1].ReoccuringOrderID} - conflicting orders found for customer {all[index1].CustomerID}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error processing recurring order {all[index1].ReoccuringOrderID}: {ex.Message}");
                        // Continue with next recurring order
                    }
                }

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Found {reocurringContacts.Count} customers with recurring items due within 7 days");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error in GetRecurringContacts: {ex.Message}");
            }

            return reocurringContacts;
        }

        /// <summary>
        /// Adds all contacts that may need reminders - ENHANCED TO EXCLUDE RECURRING CUSTOMERS
        /// </summary>
        private void AddAllContactsToRemind(ref List<ContactToRemindWithItems> pContactsToRemind, HashSet<long> excludeCustomerIds = null)
        {
            try
            {
                List<ContactsThayMayNeedData> thatMayNeedNextWeek = new ContactsThatMayNeedNextWeek().GetContactsThatMayNeedNextWeek();
                CustomerTrackedServiceItems trackedServiceItems = new CustomerTrackedServiceItems();

                // Initialize exclusion set if not provided
                excludeCustomerIds = excludeCustomerIds ?? new HashSet<long>();

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Processing {thatMayNeedNextWeek.Count} contacts that may need items next week (excluding {excludeCustomerIds.Count} recurring customers)");

                for (int index1 = 0; index1 < thatMayNeedNextWeek.Count; ++index1)
                {
                    try
                    {
                        // BUG FIX: Skip customers that have recurring orders
                        if (excludeCustomerIds.Contains(thatMayNeedNextWeek[index1].CustomerData.CustomerID))
                        {
                            AppLogger.WriteLog("email", $"CoffeeCheckupManager: Skipping {thatMayNeedNextWeek[index1].CustomerData.CompanyName} - customer has recurring orders");
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
                            AppLogger.WriteLog("email", $"CoffeeCheckupManager: Added reminder customer {toRemindWithItems.CompanyName} with {toRemindWithItems.ItemsContactRequires.Count} last-ordered items");
                        }
                    }
                    catch (Exception ex)
                    {
                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error processing contact {index1}: {ex.Message}");
                        // Continue with next contact
                    }
                }

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Final reminder contact list has {pContactsToRemind.Count} customers (excluding recurring customers)");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error in AddAllContactsToRemind: {ex.Message}");
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
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Processing {batchType} batch with {contacts.Count} contacts");

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

                            if (!isTestMode)
                            {
                                string orderResult = CreateOrderForContact(contact, orderType, out bool hasAutoFulfill, out bool hasRecurring);
                                if (!string.IsNullOrEmpty(orderResult))
                                {
                                    AppLogger.WriteLog("email", $"CoffeeCheckupManager: Order creation failed for {contact.CompanyName}: {orderResult}");
                                    // Continue with email even if order creation fails
                                }
                                else
                                {
                                    emailTextData.Footer += MessageProvider.Get(MessageKeys.CoffeeCheckup.FooterOrderAdded);

                                    string baseUrl = DisableClientManager.GetApplicationUrl();
                                    string orderLink = $"{baseUrl}/Pages/OrderDetail.aspx?CustomerID={contact.CustomerID}&DeliveryDate={contact.NextDeliveryDate:yyyy-MM-dd}";
                                    emailTextData.Footer += string.Format(MessageProvider.Get(MessageKeys.CoffeeCheckup.FooterOrderLink), orderLink);

                                    AppLogger.WriteLog("email", $"CoffeeCheckupManager: Order created successfully for {contact.CompanyName}");
                                }
                            }
                            else
                            {
                                AppLogger.WriteLog("email", $"CoffeeCheckupManager: TEST MODE - Skipped order creation for {contact.CompanyName}");
                                emailTextData.Footer += "<br />" + MessageProvider.Get(MessageKeys.CoffeeCheckup.FooterOrderAdded);
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
                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Failed to add {contact.CompanyName} to batch: {ex.Message}");
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

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: {batchType} batch result: {result.TotalSent} sent, {result.TotalFailed} failed");

                return result;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error in {batchType} batch processing: {ex.Message}");
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
                            
                            AppLogger.WriteLog("email", MessageProvider.Format(
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
                
                if (isTestMode)
                {
                    AppLogger.WriteLog("email", $"CoffeeCheckupManager: TEST MODE - Skipping order creation for {pContact.CompanyName}");
                    
                    // Still determine order types for email purposes, but don't create actual orders
                    for (int index = 0; index < pContact.ItemsContactRequires.Count; ++index)
                    {
                        if (pContact.ItemsContactRequires[index].ReoccurOrder)
                            hasRecurringItems = true;
            
                        if (pContact.ItemsContactRequires[index].AutoFulfill)
                            hasAutoFulfillItem = true;
                    }
                    
                    return string.Empty; // Success in test mode
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
                        // BUG FIX: Use proper date logic - order date, not prep date
                        // This matches the logic from ReoccuringOrderDetails page
                        DateTime dateToSet = pOrderData.OrderDate; // Use order date, not prep date
                        
                        AppLogger.WriteLog("email", $"CoffeeCheckupManager: Updating recurring order {pContact.ItemsContactRequires[index].ReoccurID} last date to {dateToSet:yyyy-MM-dd}");
                        
                        reoccuringOrderDal.SetReoccuringOrdersLastDate(dateToSet, pContact.ItemsContactRequires[index].ReoccurID);
                        hasRecurringItems = true;
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
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error creating order for {pContact.CompanyName}: {ex.Message}");
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

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: LogReminderAttempt for {contact.CompanyName} - TestMode: {isTestMode}, wasSuccessful: {wasSuccessful}");

                // In test mode, we still log because emails are actually being sent (just to test recipients)
                // If you want to skip logging in test mode, uncomment the next block:
                /*
                if (isTestMode)
                {
                    AppLogger.WriteLog("email", $"CoffeeCheckupManager: TEST MODE - Skipped logging reminder attempt for customer {contact.CustomerID} ({contact.CompanyName})");
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
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: {logMode} Logging reminder for {contact.CompanyName}");

                logEntry.InsertLogItem(logEntry);

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: {logMode} Successfully logged reminder for {contact.CompanyName} - Recurring: {hasRecurring}, AutoFulFill: {hasAutoFulFill}, Success: {wasSuccessful}");
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Failed to log reminder attempt for customer {contact.CustomerID} ({contact.CompanyName}): {ex.Message}");
            }
        }

        /// <summary>
        /// Logs individual email failure
        /// </summary>
        private void LogFailedEmail(string customerName, string errorMessage)
        {
            AppLogger.WriteLog("email", $"CoffeeCheckupManager: EMAIL FAILED: {customerName} - {errorMessage}");
        }

        /// <summary>
        /// Logs batch failure
        /// </summary>
        private void LogFailedBatch(string batchType, string errorMessage)
        {
            AppLogger.WriteLog("email", $"CoffeeCheckupManager: BATCH FAILED: {batchType} - {errorMessage}");
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
                    AppLogger.WriteLog("email", $"CoffeeCheckupManager: FAILED CUSTOMER: {failure}");
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
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error logging failed customers: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets eligible customers using database-driven approach via OrderCheckTbl control
        /// </summary>
        private List<ContactToRemindWithItems> GetEligibleCustomersFromDatabase()
        {
            try
            {
                AppLogger.WriteLog("email", "CoffeeCheckupManager: Getting eligible customers using OrderCheckTbl");

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

                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Database query returned {eligibleCustomers.Count} eligible customers");
                return eligibleCustomers;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Database query failed: {ex.Message}");
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
                var allRecurring = reoccuringOrderDal.GetAll(1, "ReoccuringOrderID");
                var recurringOrder = allRecurring.FirstOrDefault(r => r.ReoccuringOrderID == reoccurId);

                if (recurringOrder != null)
                {
                    var recurrenceType = ReoccuranceTypeTbl.GetRecurrenceType(recurringOrder.ReoccuranceTypeID);
                    return recurrenceType == ReoccuranceTypeTbl.RecurrenceType.Monthly;
                }

                return false;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error checking monthly recurrence for {reoccurId}: {ex.Message}");
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
                var allRecurring = reoccuringOrderDal.GetAll(1, "ReoccuringOrderID");
                var recurringOrder = allRecurring.FirstOrDefault(r => r.ReoccuringOrderID == reoccurId);

                return recurringOrder?.ReoccuranceValue ?? 0;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", $"CoffeeCheckupManager: Error getting target day for {reoccurId}: {ex.Message}");
                return 0;
            }
        }
    }
}
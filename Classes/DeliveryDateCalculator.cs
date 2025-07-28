using System;
using System.Collections.Generic;
using TrackerDotNet.Controls;

namespace TrackerDotNet.Classes
{
    /// <summary>
    /// Calculates optimal delivery dates based on city delivery schedules and recurring order requirements
    /// Follows project standards with MessageProvider logging and proper error handling
    /// </summary>
    public class DeliveryDateCalculator
    {
        private readonly TrackerTools _trackerTools;
        private readonly CityTblDAL _cityTblDAL;

        public const int WEEKLY_INTERVAL = 1;
        public const int BIWEEKLY_INTERVAL = 2;
        public const int TRI_WEEKLY_INTERVAL = 3;
        // Add more as needed
        public DeliveryDateCalculator()
        {
            _trackerTools = new TrackerTools();
            _cityTblDAL = new CityTblDAL();
        }

        /// <summary>
        /// Calculates the optimal delivery date for a monthly recurring order
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="targetDayOfMonth">Target day of month (1-31)</param>
        /// <param name="lastOrderDate">Date of last order</param>
        /// <returns>Optimal delivery date considering city delivery schedule</returns>
        public DateTime CalculateOptimalMonthlyDeliveryDate(long customerId, int targetDayOfMonth, DateTime lastOrderDate)
        {
            try
            {
                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.CalculatingMonthlyDelivery,
                    customerId, targetDayOfMonth));

                // Step 1: Calculate the next occurrence of target day
                DateTime nextTargetDate = CalculateNextMonthlyOccurrence(targetDayOfMonth, lastOrderDate);

                // Step 2: Get customer's optimal delivery date based on city schedule
                DateTime optimalDeliveryDate = FindClosestCityDeliveryDate(customerId, nextTargetDate, TimeZoneUtils.Now());  //use todays date as a minimum

                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.OptimalDateCalculated,
                    nextTargetDate, optimalDeliveryDate));

                return optimalDeliveryDate;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.ErrorCalculatingDelivery,
                    customerId, ex.Message));

                // Fallback: return next month occurrence without optimization
                return CalculateNextMonthlyOccurrence(targetDayOfMonth, lastOrderDate);
            }
        }

        /// <summary>
        /// Calculates the optimal delivery date for a weekly recurring order
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="weekInterval">Week interval (1 = weekly, 2 = bi-weekly, etc.)</param>
        /// <param name="lastOrderDate">Date of last order</param>
        /// <returns>Optimal delivery date</returns>
        public DateTime CalculateOptimalWeeklyDeliveryDate(long customerId, int weekInterval, DateTime lastOrderDate)
        {
            try
            {
                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.CalculatingWeeklyDelivery,
                    customerId, weekInterval));

                DateTime nextWeeklyDate = lastOrderDate.AddDays(weekInterval * 7);
                DateTime optimalDate = FindClosestCityDeliveryDate(customerId, nextWeeklyDate, TimeZoneUtils.Now());

                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.WeeklyDateCalculated,
                    nextWeeklyDate, optimalDate));

                return optimalDate;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.ErrorCalculatingWeekly,
                    customerId, ex.Message));

                return lastOrderDate.AddDays(weekInterval * 7);
            }
        }

        /// <summary>
        /// Calculates the roast date from a delivery date (typically 1-2 days before)
        /// </summary>
        public DateTime CalculateRoastDateFromDelivery(DateTime deliveryDate)
        {
            try
            {
                // Standard business rule: roast 1 day before delivery
                DateTime roastDate = deliveryDate.AddDays(-1);

                // Ensure roast date is not in the past
                DateTime today = TimeZoneUtils.Now().Date;

                if (roastDate < today)
                {
                    roastDate = today;
                }

                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.RoastDateCalculated,
                    deliveryDate, roastDate));

                return roastDate;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.ErrorCalculatingRoastDate,
                    deliveryDate, ex.Message));

                return deliveryDate.AddDays(-1); // Simple fallback
            }
        }

        /// <summary>
        /// Calculates the next occurrence of a specific day of month
        /// </summary>
        private DateTime CalculateNextMonthlyOccurrence(int targetDayOfMonth, DateTime lastOrderDate)
        {
            try
            {
                DateTime currentDate = TimeZoneUtils.Now().Date;
                DateTime nextMonth = lastOrderDate.AddMonths(1);
                
                // Handle months with fewer days (e.g., Feb 31st → Feb 28th)
                int daysInMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
                int actualDay = Math.Min(targetDayOfMonth, daysInMonth);
                
                DateTime nextOccurrence = new DateTime(nextMonth.Year, nextMonth.Month, actualDay);
                
                // If the calculated date is in the past, move to next month
                if (nextOccurrence <= currentDate)
                {
                    nextMonth = nextOccurrence.AddMonths(1);
                    daysInMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
                    actualDay = Math.Min(targetDayOfMonth, daysInMonth);
                    nextOccurrence = new DateTime(nextMonth.Year, nextMonth.Month, actualDay);
                }
                
                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.NextOccurrenceCalculated,
                    targetDayOfMonth, nextOccurrence));
                
                return nextOccurrence;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.ErrorCalculatingOccurrence,
                    targetDayOfMonth, ex.Message));
                throw;
            }
        }
        
        /// <summary>
        /// Finds the optimal delivery date based on customer's city delivery schedule
        /// </summary>
        private DateTime FindOptimalCityDeliveryDate(long customerId, DateTime targetDate)
        {
            try
            {
                // Get customer's city delivery schedule using existing infrastructure
                DateTime dummyDate = DateTime.MinValue;
                DateTime customerRoastDate = _trackerTools.GetNextRoastDateByCustomerID(customerId, ref dummyDate);
                
                if (customerRoastDate == DateTime.MinValue || customerRoastDate < TimeZoneUtils.Now().Date.AddDays(-30))
                {
                    AppLogger.WriteLog("email", MessageProvider.Format(
                        MessageKeys.DeliveryCalculation.InvalidRoastDate,
                        customerId));
                    
                    return targetDate; // Fallback to target date
                }
                
                // Find the delivery date closest to our target
                DateTime optimalDate = FindClosestAvailableDeliveryDate(customerRoastDate, targetDate);
                
                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.OptimalCityDateFound,
                    customerRoastDate, optimalDate));
                
                return optimalDate;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.ErrorFindingCityDate,
                    customerId, ex.Message));
                
                return targetDate; // Fallback to target date
            }
        }
        
        /// <summary>
        /// Finds the closest available delivery date to the target date
        /// </summary>
        private DateTime FindClosestAvailableDeliveryDate(DateTime cityDeliveryDate, DateTime targetDate)
        {
            try
            {
                // If city delivery date is within 7 days of target, use it
                double daysDifference = Math.Abs((cityDeliveryDate - targetDate).TotalDays);
                
                if (daysDifference <= 7)
                {
                    AppLogger.WriteLog("email", MessageProvider.Format(
                        MessageKeys.DeliveryCalculation.UsingCityDeliveryDate,
                        cityDeliveryDate, daysDifference));
                    
                    return cityDeliveryDate;
                }
                
                // Otherwise, use GetClosestNextRoastDate to find next available date
                DateTime searchDate = targetDate;
                int maxSearchDays = 14; // Search up to 2 weeks ahead
                
                for (int i = 0; i < maxSearchDays; i++)
                {
                    try
                    {
                        // Use existing TrackerTools method
                        DateTime testRoastDate = _trackerTools.GetClosestNextRoastDate(searchDate);
                        
                        if (testRoastDate > searchDate && testRoastDate != DateTime.MinValue)
                        {
                            AppLogger.WriteLog("email", MessageProvider.Format(
                                MessageKeys.DeliveryCalculation.FoundNextDeliveryCycle,
                                testRoastDate, i));
                            
                            return testRoastDate;
                        }
                    }
                    catch (Exception)
                    {
                        // Continue searching if this date fails
                    }
                    
                    searchDate = searchDate.AddDays(1);
                }
                
                // Final fallback: use target date
                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.UsingFallbackDate,
                    targetDate));
                
                return targetDate;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.ErrorFindingClosestDate,
                    ex.Message));
                
                return targetDate;
            }
        }
        
        /// <summary>
        /// Finds the closest delivery date for a customer based on their city's prep/delivery schedule and a target date.
        /// </summary>
            private DateTime FindClosestCityDeliveryDate(long customerId, DateTime targetDate, DateTime minDate)
            {
                int cityId = _cityTblDAL.GetCityIdByCustomerId(customerId);
                if (cityId == 0)
                    return targetDate;

                List<CityPrepDaysTbl> prepDays = _cityTblDAL.GetPrepRulesForCity(cityId);
                if (prepDays == null || prepDays.Count == 0)
                    return targetDate;

                DateTime closestDelivery = DateTime.MinValue;
                double minDiff = double.MaxValue;

                foreach (var prep in prepDays)
                {
                    // --- Check NEXT delivery date (forward) ---
                    int daysToAdd = ((int)prep.PrepDayOfWeekID - (int)targetDate.DayOfWeek + 7) % 7;
                    DateTime nextPrep = targetDate.AddDays(daysToAdd);
                    DateTime nextDelivery = nextPrep.AddDays(prep.DeliveryDelayDays);

                    if (nextDelivery >= minDate)
                    {
                        double diff = Math.Abs((nextDelivery - targetDate).TotalDays);
                        if (diff < minDiff)
                        {
                            minDiff = diff;
                            closestDelivery = nextDelivery;
                        }
                    }

                    // --- Check PREVIOUS delivery date (backward) ---
                    int daysToSubtract = ((int)targetDate.DayOfWeek - (int)prep.PrepDayOfWeekID + 7) % 7;
                    DateTime prevPrep = targetDate.AddDays(-daysToSubtract);
                    DateTime prevDelivery = prevPrep.AddDays(prep.DeliveryDelayDays);

                    if (prevDelivery >= minDate)
                    {
                        double diff = Math.Abs((prevDelivery - targetDate).TotalDays);
                        if (diff < minDiff)
                        {
                            minDiff = diff;
                            closestDelivery = prevDelivery;
                        }
                    }
                }

                return closestDelivery == DateTime.MinValue ? targetDate : closestDelivery;
            }
    }
}
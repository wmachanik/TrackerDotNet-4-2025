using System;
using System.Collections.Generic;
using TrackerDotNet.Controls;

namespace TrackerDotNet.Classes
{
    /// <summary>
    /// Calculates optimal delivery dates based on city delivery schedules and recurring order requirements
    /// Follows project standards with MessageProvider logging and proper error handling
    /// </summary>
    public class DateCalculator
    {
        private readonly TrackerTools _trackerTools;
        private readonly CityTblDAL _cityTblDAL;

        public const int WEEKLY_INTERVAL = 1;
        public const int BIWEEKLY_INTERVAL = 2;
        public const int TRI_WEEKLY_INTERVAL = 3;
        // Add more as needed
        public DateCalculator()
        {
            _trackerTools = new TrackerTools();
            _cityTblDAL = new CityTblDAL();
        }
        //public static class DateTimeExtensions
        //{
        //    public static DateTime GetFirstDayOfWeek(this DateTime sourceDateTime)
        //    {
        //        int diff = -(int)sourceDateTime.DayOfWeek;
        //        return sourceDateTime.AddDays(diff).Date;
        //    }


        //    public static DateTime GetLastDayOfWeek(this DateTime sourceDateTime)
        //    {
        //        DayOfWeek dayOfWeek = 6 - sourceDateTime.DayOfWeek;
        //        sourceDateTime = sourceDateTime.AddDays((double)dayOfWeek);
        //        return sourceDateTime;
        //    }
        //}
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
                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.CalculatingMonthlyDelivery,
                    customerId, targetDayOfMonth));

                // Step 1: Calculate the next occurrence of target day
                DateTime nextTargetDate = CalculateNextMonthlyOccurrence(targetDayOfMonth, lastOrderDate);

                // Step 2: Get customer's optimal delivery date based on city schedule
                DateTime optimalDeliveryDate = FindClosestCityDeliveryDate(customerId, nextTargetDate, TimeZoneUtils.Now());  //use todays date as a minimum

                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.OptimalDateCalculated,
                    nextTargetDate, optimalDeliveryDate));

                return optimalDeliveryDate;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
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
        public DateTime CalculateOptimalWeeklyDeliveryDate(long customerId, int weekInterval, DateTime calculatedDeliveryDate)
        {
            try
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.CalculatingWeeklyDelivery,
                    customerId, weekInterval));

                DateTime optimalDate = FindClosestCityDeliveryDate(customerId, calculatedDeliveryDate, TimeZoneUtils.Now());

                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.WeeklyDateCalculated,
                    calculatedDeliveryDate, optimalDate));

                return optimalDate;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.ErrorCalculatingWeekly,
                    customerId, ex.Message));

                return calculatedDeliveryDate;
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

                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.RoastDateCalculated,
                    deliveryDate, roastDate));

                return roastDate;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
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

                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.NextOccurrenceCalculated,
                    targetDayOfMonth, nextOccurrence));

                return nextOccurrence;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
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
                    AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
                        MessageKeys.DeliveryCalculation.InvalidRoastDate,
                        customerId));

                    return targetDate; // Fallback to target date
                }

                // Find the delivery date closest to our target
                DateTime optimalDate = FindClosestAvailableDeliveryDate(customerRoastDate, targetDate);

                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.OptimalCityDateFound,
                    customerRoastDate, optimalDate));

                return optimalDate;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
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
                    AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
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
                            AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
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
                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
                    MessageKeys.DeliveryCalculation.UsingFallbackDate,
                    targetDate));

                return targetDate;
            }
            catch (Exception ex)
            {
                AppLogger.WriteLog(SystemConstants.LogTypes.Orders, MessageProvider.Format(
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

            DateTime earliestDelivery = DateTime.MaxValue;

            // Search a window of prep dates around the target date (e.g., ±7 days)
            for (int offset = -7; offset <= 7; offset++)
            {
                DateTime candidatePrep = targetDate.AddDays(offset);

                foreach (var prep in prepDays)
                {
                    if ((int)candidatePrep.DayOfWeek != prep.PrepDayOfWeekID)
                        continue;

                    DateTime candidateDelivery = candidatePrep.AddDays(prep.DeliveryDelayDays);

                    // Only consider delivery dates on or after targetDate and minDate
                    if (candidateDelivery >= targetDate && candidateDelivery >= minDate && candidateDelivery < earliestDelivery)
                    {
                        earliestDelivery = candidateDelivery;
                    }
                }
            }

            return earliestDelivery == DateTime.MaxValue ? targetDate : earliestDelivery;
        }
        public List<ReoccuringOrderExtData> FilterAndUpdateRecurringOrdersDates(List<ReoccuringOrderExtData> reoccuringOrders,
            DateTime? windowStart = null, DateTime? windowEnd = null)
        {
            var validOrders = new List<ReoccuringOrderExtData>();
            var reoccuringOrderDal = new ReoccuringOrderDAL();

            foreach (var order in reoccuringOrders)
            {
                // Always recalculate next required date using LastDone date and recurrence type
                var calculatedNextDate = reoccuringOrderDal.CalculateNextDateRequired(order);

                // Update legacy/broken records if needed
                if (order.NextDateRequired != calculatedNextDate)
                {
                    order.NextDateRequired = calculatedNextDate;
                    reoccuringOrderDal.UpdateReoccuringOrder(order, order.ReoccuringOrderID, false);
                }

                // Only keep records where next required date is <= windowEnd and not min date
                bool isValid = order.NextDateRequired != SystemConstants.DatabaseConstants.SystemMinDate;
                if (windowEnd.HasValue)
                    isValid &= order.NextDateRequired <= windowEnd.Value;
                if (windowStart.HasValue)
                    isValid &= order.NextDateRequired >= windowStart.Value;

                if (isValid)
                    validOrders.Add(order);
            }
            return validOrders;
        }
    }
}
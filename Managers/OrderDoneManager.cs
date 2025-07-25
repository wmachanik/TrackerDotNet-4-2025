﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackerDotNet.Classes;
using TrackerDotNet.Controls;
using TrackerDotNet.Managers;

namespace TrackerDotNet.BusinessLogic
{
    public class OrderDoneResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ClientUsageTbl OriginalUsage { get; set; }
        public ClientUsageTbl UpdatedUsage { get; set; }
    }

    public class OrderDoneManager
    {
        public OrderDoneManager() { }

        public static OrderDoneResult CompleteOrder(
            int customerId,
            DateTime deliveryDate,
            string stockText,
            string cupCountText,
            string statusKey)
        {
            var result = new OrderDoneResult();
            var trackerTools = new TrackerTools();
            trackerTools.SetTrackerSessionErrorString(string.Empty);

            var clientUsageTbl = new ClientUsageTbl();
            var originalUsage = clientUsageTbl.GetUsageData(customerId);

            var tempOrdersDal = new TempOrdersDAL();
            bool hasCoffee = tempOrdersDal.HasCoffeeInTempOrder();

            if (!string.IsNullOrEmpty(trackerTools.GetTrackerSessionErrorString()))
            {
                result.Success = false;
                result.Message = trackerTools.GetTrackerSessionErrorString();
                return result;
            }

            if (!string.IsNullOrEmpty(stockText) && double.TryParse(stockText, out double stock) && stock > 50.0)
            {
                result.Success = false;
                result.Message = "The stock quantity appears very high. Please check that you have entered the correct value in kilograms.";
                return result;
            }

            double pStock = string.IsNullOrEmpty(stockText) ? 0.0 : Math.Round(Convert.ToDouble(stockText), 2);
            var generalTrackerDbTools = new GeneralTrackerDbTools();
            var latestUsageData = generalTrackerDbTools.GetLatestUsageData(customerId, 2);

            bool pIsActual = !string.IsNullOrEmpty(cupCountText);
            int pCupCount = 0;
            if (pIsActual)
                pCupCount = Convert.ToInt32(cupCountText);

            if (pCupCount < 1 || pCupCount < latestUsageData.LastCount)
            {
                pCupCount = generalTrackerDbTools.CalcEstCupCount(customerId, latestUsageData, hasCoffee);
                pIsActual = false;
            }

            // this is suppose to close a repair order if it exists, but does nto seem to work
            //var repairManager = new RepairManager();
            //repairManager.SetStatusDoneByTempOrder();
            int updatedCupCount = AddItemsToClientUsageTbl(customerId, pIsActual, pCupCount, pStock, deliveryDate);

            if (!clientUsageTbl.UpdateUsageCupCount(customerId, updatedCupCount))
            {
                result.Success = false;
                result.Message = "Error updating last count";
                return result;
            }

            generalTrackerDbTools.UpdatePredictions(customerId, updatedCupCount);
            tempOrdersDal.MarkTempOrdersItemsAsDone();
            generalTrackerDbTools.ResetCustomerReminderCount(customerId, hasCoffee);

            string sentStatus = null;
            if (!string.IsNullOrEmpty(statusKey))
            {
                sentStatus = SendOrderStatusEmail(customerId, statusKey);
            }

            tempOrdersDal.KillTempOrdersData();

            result.Success = sentStatus == null;
            result.Message = sentStatus ?? "Order done email sent successfully.";
            result.OriginalUsage = originalUsage;
            result.UpdatedUsage = clientUsageTbl.GetUsageData(customerId);

            return result;
        }

        public static int AddItemsToClientUsageTbl(long pCustomerID, bool pIsActual, int pCupCount, double pStock, DateTime pDeliveryDate)
        {
            List<ClientUsageFromTempOrder> all = new ClientUsageFromTempOrder().GetAll(pCustomerID);
            List<ItemUsageTbl> itemUsageTblList = new List<ItemUsageTbl>();
            List<ClientUsageLinesTbl> clientUsageLinesTblList = new List<ClientUsageLinesTbl>();
            int index1 = 0;
            string str = pIsActual ? "actual count" : "estimate count";
            if (pStock > 0.0)
            {
                pCupCount -= Convert.ToInt32(Math.Round(pStock * 100.0, 0));
                str = $"{str}; Stock of: {pCupCount.ToString()}";
            }
            while (all.Count > index1)
            {
                ClientUsageLinesTbl clientUsageLinesTbl = new ClientUsageLinesTbl();
                clientUsageLinesTbl.CustomerID = all[index1].CustomerID;
                clientUsageLinesTbl.LineDate = pDeliveryDate;
                clientUsageLinesTbl.ServiceTypeID = all[index1].ServiceTypeID;
                clientUsageLinesTbl.Qty = 0.0;
                clientUsageLinesTbl.CupCount = pCupCount;
                clientUsageLinesTbl.Notes = str;
                do
                {
                    clientUsageLinesTbl.Qty += all[index1].Qty * all[index1].UnitsPerQty;
                    itemUsageTblList.Add(new ItemUsageTbl()
                    {
                        CustomerID = all[index1].CustomerID,
                        ItemDate = pDeliveryDate,
                        ItemProvidedID = all[index1].ItemID,
                        AmountProvided = all[index1].Qty,
                        PackagingID = all[index1].PackagingID,
                        Notes = str
                    });
                    ++index1;
                }
                while (all.Count > index1 && clientUsageLinesTbl.ServiceTypeID == all[index1].ServiceTypeID);
                clientUsageLinesTblList.Add(clientUsageLinesTbl);
            }
            for (int index2 = 0; index2 < clientUsageLinesTblList.Count; ++index2)
                clientUsageLinesTblList[index2].InsertItemsUsed(clientUsageLinesTblList[index2]);
            for (int index3 = 0; index3 < itemUsageTblList.Count; ++index3)
                itemUsageTblList[index3].InsertItemsUsed(itemUsageTblList[index3]);
            return pCupCount;
        }

        public static string SendOrderStatusEmail(long customerId, string statusKey)
        {
            if (statusKey== null)
            {
                return "❌ Status key is null.";
            }
            var customer = new CustomersTbl().GetCustomerByCustomerID(customerId);
            string recipient = !string.IsNullOrWhiteSpace(customer.EmailAddress) ? customer.EmailAddress : customer.AltEmailAddress;
            if (string.IsNullOrWhiteSpace(recipient))
                return "❌ No recipient email address found.";

            var emailSettings = new EmailSettings();
            emailSettings.SetRecipient(recipient);

            var email = new EmailMailKitCls(emailSettings);
            email.AddCCFromAddress();
            email.SetEmailSubject(MessageProvider.Get(MessageKeys.Order.StatusSubject));

            string contactName = !string.IsNullOrWhiteSpace(customer.ContactFirstName)
                ? customer.ContactFirstName
                : MessageProvider.Get(MessageKeys.Order.StatusDefaultContact);

            string statusMessage = MessageProvider.Get(statusKey);
            string body = MessageProvider.Format(MessageKeys.Order.StatusBody, contactName, statusMessage);
            email.AddToBody(body);
            email.AddToBody(MessageProvider.Get(MessageKeys.Order.StatusFooter));
            email.AddToBody(MessageProvider.Get(MessageProvider.GetEmailSignature()));

            bool success = email.SendEmail();
            if (success)
            {
                AppLogger.WriteLog("email", $"✅ Order done message sent to {recipient}");

            }
            else { 
                // Log the error
                AppLogger.WriteLog("email", $"❌ Failed to send email to {recipient}: {email.myResults.sResult}");
            }
            string message = success
                ? null
                : $"❌ Failed to send email to {recipient}: {email.myResults.sResult}";

            return message;
        }
    }
}
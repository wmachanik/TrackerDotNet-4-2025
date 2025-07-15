using System;
using TrackerDotNet.Controls;
using TrackerDotNet.Classes;

namespace TrackerDotNet.Managers
{
    public class RepairManager
    {
        private readonly RepairsTbl _repairsTbl;
        private readonly OrderTbl _orderTbl;
        private readonly CustomersTbl _customersTbl;

        public RepairManager()
        {
            _repairsTbl = new RepairsTbl();
            _orderTbl = new OrderTbl();
            _customersTbl = new CustomersTbl();
        }

        public string HandleStatusChange(RepairsTbl repair)
        {
            bool orderUpdated = true;
            
            switch (repair.RepairStatusID)
            {
                case 1: // LOGGED
                    orderUpdated = LogNewRepair(repair, true);
                    break;

                case 2: // COLLECTED
                    if (repair.RelatedOrderID == 0)
                    {
                        orderUpdated = LogNewRepair(repair, true);
                    }
                    else
                    {
                        _orderTbl.UpdateIncDeliveryDateBy7(repair.RelatedOrderID);
                    }
                    break;

                case 3: // WORKSHOP
                    HandleWorkshopStatus(repair);
                    break;

                case 6: // READY
                    if (repair.RelatedOrderID > 0)
                    {
                        var nextDeliveryDate = new NextRoastDateByCityTbl()
                            .GetNextDeliveryDate(repair.CustomerID);
                        _orderTbl.UpdateOrderDeliveryDate(nextDeliveryDate, repair.RelatedOrderID);
                    }
                    break;

                case 7: // DONE
                    if (repair.RelatedOrderID > 0)
                    {
                        _orderTbl.UpdateSetDoneByID(true, repair.RelatedOrderID);
                    }
                    break;
            }

            bool updateSuccess = string.IsNullOrEmpty(_repairsTbl.UpdateRepair(repair));
            string emailError = updateSuccess ? SendStatusNotification(repair) : null;

            if (!updateSuccess)
                return MessageProvider.Get(MessageKeys.Repairs.ErrorUpdating);

            if (!string.IsNullOrEmpty(emailError))
                return emailError;

            return null; // Success
        }

        private string SendStatusNotification(RepairsTbl repair)
        {
            var equipTypeTbl = new EquipTypeTbl();
            var repairStatusesTbl = new RepairStatusesTbl();

            var emailSettings = new EmailSettings();
            emailSettings.SetRecipient(repair.ContactEmail);

            var email = new EmailMailKitCls(emailSettings);
            email.AddCCFromAddress();

            email.SetEmailSubject(MessageProvider.Get(MessageKeys.Repairs.StatusEmailSubject));

            string body = MessageProvider.Format(
                MessageKeys.Repairs.StatusEmailBody,
                repair.ContactName,
                equipTypeTbl.GetEquipName(repair.MachineTypeID),
                repair.MachineSerialNumber,
                repairStatusesTbl.GetRepairStatusDesc(repair.RepairStatusID)
            );

            email.AddToBody(body);
            email.AddToBody(MessageProvider.Get(MessageProvider.GetEmailSignature()));

            bool success = email.SendEmail();

            if (!success)
            {
                AppLogger.WriteLog("email", MessageProvider.Format(
                    MessageKeys.Email.SendError,
                    repair.ContactEmail,
                    email.LastErrorSummary));
            }

            return success ? null : email.LastErrorSummary;
        }

        private bool LogNewRepair(RepairsTbl repair, bool calculateDelivery)
        {
            var orderData = new OrderTblData
            {
                CustomerID = repair.CustomerID,
                ItemTypeID = 36,
                QuantityOrdered = 1.0,
                Notes = MessageProvider.Get(MessageKeys.Repairs.CollectSwopOutNote)
            };

            DateTime delivery = TimeZoneUtils.Now().Date.AddDays(7.0);

            if (calculateDelivery)
            {
                var tools = new TrackerTools();
                orderData.RoastDate = tools.GetNextRoastDateByCustomerID(repair.CustomerID, ref delivery);
                var prefs = tools.RetrieveCustomerPrefs(repair.CustomerID);
                
                orderData.OrderDate = TimeZoneUtils.Now().Date;
                orderData.RequiredByDate = delivery;
                orderData.ToBeDeliveredBy = prefs.PreferredDeliveryByID;
                
                if (prefs.RequiresPurchOrder)
                    orderData.PurchaseOrder = TrackerTools.CONST_POREQUIRED;
            }
            else
            {
                DateTime today = TimeZoneUtils.Now().Date;
                orderData.OrderDate = today;
                orderData.RoastDate = today;
                orderData.RequiredByDate = delivery;
            }

            _orderTbl.InsertNewOrderLine(orderData);
            repair.RelatedOrderID = _orderTbl.GetLastOrderAdded(
                orderData.CustomerID, 
                orderData.OrderDate, 
                36);

            return true;
        }

        private void HandleWorkshopStatus(RepairsTbl repair)
        {
            if (repair.RelatedOrderID == 0)
            {
                LogNewRepair(repair, false);
            }
            else
            {
                _orderTbl.UpdateIncDeliveryDateBy7(repair.RelatedOrderID);
            }

            if (!string.IsNullOrEmpty(repair.MachineSerialNumber))
            {
                _customersTbl.SetEquipDetailsIfEmpty(
                    repair.MachineTypeID,
                    repair.MachineSerialNumber,
                    repair.CustomerID);
            }
        }

        public void SetStatusDoneByTempOrder()
        {
            var repairsTbl = new RepairsTbl();
            var tempOrders = repairsTbl.GetListOfRelatedTempOrders();
            
            if (tempOrders.Count > 0)
            {
                var tempOrdersLinesTbl = new TempOrdersLinesTbl();
                var orderTbl = new OrderTbl();

                foreach (var repair in tempOrders)
                {
                    if (repair.RepairStatusID <= 3)
                    {
                        // For repairs in early stages, delete temp order and extend delivery date
                        tempOrdersLinesTbl.DeleteByOriginalID(repair.RelatedOrderID);
                        orderTbl.UpdateIncDeliveryDateBy7(repair.RelatedOrderID);
                    }
                    else
                    {
                        // For repairs in later stages, mark as done
                        repair.RepairStatusID = 7; // Done status
                        repairsTbl.UpdateRepair(repair);
                    }
                }
            }
        }

        // Other business logic methods...
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackerDotNet.Classes;
using TrackerDotNet.Controls;

namespace TrackerDotNet.BusinessLogic
{
    public class OrderManager
    {
        public string AddOrderLine(OrderHeaderData headerData, OrderTblData orderData)
        {
            TrackerTools trackerTools = new TrackerTools();
            orderData.ItemTypeID = trackerTools.ChangeItemIfGroupToNextItemInGroup(orderData.CustomerID, orderData.ItemTypeID, orderData.RequiredByDate);
            OrderTbl orderTbl = new OrderTbl();
            return orderTbl.InsertNewOrderLine(orderData);
        }
        public string DeleteOrderItem(int orderId)
        {
            return new OrderTbl().DeleteOrderById(orderId);
        }

        public string MarkItemAsInvoiced(long customerId, DateTime deliveryDate, string notes)
        {
            return new OrderTbl().UpdateSetInvoiced(true, customerId, deliveryDate, notes);
        }

        public string UnDoOrderItem(int orderId)
        {
            return new OrderTbl().UpdateSetDoneByID(false, orderId);
        }

        public void MoveOrderDeliveryDate(DateTime newDate, int orderId)
        {
            new OrderTbl().UpdateOrderDeliveryDate(newDate, orderId);
        }

        public bool CompleteOrderDelivery(OrderHeaderData headerData, List<TempOrderLineData> orderLines)
        {
            TempOrdersDAL tempOrdersDal = new TempOrdersDAL();
            if (!tempOrdersDal.KillTempOrdersData())
                return false;

            TempOrdersData tempOrder = new TempOrdersData();
            tempOrder.HeaderData = new TempOrdersHeaderTbl
            {
                CustomerID = headerData.CustomerID,
                OrderDate = headerData.OrderDate,
                RoastDate = headerData.RoastDate,
                RequiredByDate = headerData.RequiredByDate,
                ToBeDeliveredByID = headerData.ToBeDeliveredBy,
                Confirmed = headerData.Confirmed,
                Done = headerData.Done,
                Notes = headerData.Notes
            };
            tempOrder.OrdersLines = orderLines.Select(line =>
            {
                var tbl = line.ToTempOrdersLinesTbl();
                tbl.ServiceTypeID = new ItemTypeTbl().GetServiceID(tbl.ItemID);
                return tbl;
            }).ToList();

            return tempOrdersDal.Insert(tempOrder);
        }

        // Helper DTO for order lines (optional, for clarity)
        public class TempOrderLineData
        {
            public int ItemID { get; set; }
            public double Qty { get; set; }
            public int PackagingID { get; set; }
            public int ServiceTypeID { get; set; }
            public int OriginalOrderID { get; set; }

            public TempOrdersLinesTbl ToTempOrdersLinesTbl()
            {
                return new TempOrdersLinesTbl
                {
                    ItemID = this.ItemID,
                    Qty = this.Qty,
                    PackagingID = this.PackagingID,
                    ServiceTypeID = this.ServiceTypeID,
                    OriginalOrderID = this.OriginalOrderID
                };
            }
        }
    }
}
// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.control.ReoccuringOrderDAL
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.Data;
using TrackerDotNet.Classes;

//- only form later versions #nullable disable
namespace TrackerDotNet.Controls
{
    public class ReoccuringOrderDAL
    {
        private const string CONST_SQL_SELECT = "SELECT ReoccuringOrderTbl.ID, CustomersTbl.CustomerID, ReoccuranceType, [Value], ItemRequiredID, QtyRequired, DateLastDone, NextDateRequired, RequireUntilDate, PackagingID, ReoccuringOrderTbl.Enabled, ReoccuringOrderTbl.Notes, CustomersTbl.CompanyName, ItemTypeTbl.ItemDesc AS ItemTypeDesc, ReoccuranceTypeTbl.Type AS ReoccuranceTypeDesc  FROM (((ReoccuringOrderTbl LEFT OUTER JOIN ItemTypeTbl ON ReoccuringOrderTbl.ItemRequiredID = ItemTypeTbl.ItemTypeID)   LEFT OUTER JOIN CustomersTbl ON ReoccuringOrderTbl.CustomerID = CustomersTbl.CustomerID)   LEFT OUTER JOIN ReoccuranceTypeTbl ON ReoccuringOrderTbl.ReoccuranceType = ReoccuranceTypeTbl.ID)";
        private const string CONST_SQL_SELECTBYREOCCURINGID = "SELECT CustomerID, ReoccuranceType, [Value], ItemRequiredID, QtyRequired, DateLastDone, NextDateRequired, RequireUntilDate, PackagingID, DeliveryByID, Enabled, Notes  FROM ReoccuringOrderTbl WHERE ID = ?";
        private const string CONST_SQL_UPDATE = "UPDATE ReoccuringOrderTbl SET CustomerID = ?, ReoccuranceType = ?, [Value]= ?,  ItemRequiredID = ?, QtyRequired= ?, DateLastDone= ?, NextDateRequired= ?, RequireUntilDate = ?, PackagingID = ?, Enabled = ?, Notes = ? WHERE ReoccuringOrderTbl.ID = ?";
        private const string CONST_SQL_INSERT = "INSERT INTO ReoccuringOrderTbl (CustomerID, ReoccuranceType, [Value], ItemRequiredID, QtyRequired,  DateLastDone, NextDateRequired, RequireUntilDate, PackagingID, Enabled, Notes)  VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private const string CONST_SQL_DELETE = "DELETE FROM ReoccuringOrderTbl WHERE ReoccuringOrderTbl.ID = ?";
        private const string CONST_SQL_GETITEMSLASTDATE = " SELECT CustomerID, ItemRequiredID, MAX(LastDate) AS LastDatePerItem FROM (SELECT ReoccuringOrderTbl.CustomerID, ReoccuringOrderTbl.ItemRequiredID, ClientUsageLinesTbl.[Date] AS LastDate FROM  ((ClientUsageLinesTbl INNER JOIN ReoccuringOrderTbl ON ClientUsageLinesTbl.CustomerID = ReoccuringOrderTbl.CustomerID AND ClientUsageLinesTbl.[Date] > ReoccuringOrderTbl.DateLastDone) INNER JOIN ItemTypeTbl ON ReoccuringOrderTbl.ItemRequiredID = ItemTypeTbl.ItemTypeID)) ListOfOrdersRequired GROUP BY CustomerID, ItemRequiredID";
        private const string CONST_UPDATE_ITEMSLASTDATE = "UPDATE ReoccuringOrderTbl SET DateLastDone = ? WHERE (CustomerID = ?) AND (ItemRequiredID = ?)";
        private const string CONST_UPDATE_SETREOCCURLASTDATE = "UPDATE ReoccuringOrderTbl SET DateLastDone = ? WHERE (ID = ?)";
        public const int CONST_EITHERENABLEDORDISABLED = -1;
        public const int CONST_DISABLEDONLY = 0;
        public const int CONST_ENABLEDONLY = 1;

        public List<ReoccuringOrderExtData> GetAll() => this.GetAll(-1, string.Empty);

        public List<ReoccuringOrderExtData> GetAll(string SortBy)
        {
            return this.GetAll(-1, SortBy, string.Empty);
        }

        public List<ReoccuringOrderExtData> GetAll(int IsEnabled)
        {
            return this.GetAll(IsEnabled, string.Empty, string.Empty);
        }

        public List<ReoccuringOrderExtData> GetAll(int IsEnabled, string SortBy)
        {
            return this.GetAll(IsEnabled, SortBy, string.Empty);
        }

        public List<ReoccuringOrderExtData> GetAll(int IsEnabled, string SortBy, string WhereFilter)
        {
            List<ReoccuringOrderExtData> all = new List<ReoccuringOrderExtData>();
            TrackerDb trackerDb = new TrackerDb();
            string strSQL = "SELECT ReoccuringOrderTbl.ID, CustomersTbl.CustomerID, ReoccuranceType, [Value], ItemRequiredID, QtyRequired, DateLastDone, NextDateRequired, RequireUntilDate, PackagingID, ReoccuringOrderTbl.Enabled, ReoccuringOrderTbl.Notes, CustomersTbl.CompanyName, ItemTypeTbl.ItemDesc AS ItemTypeDesc, ReoccuranceTypeTbl.Type AS ReoccuranceTypeDesc  FROM (((ReoccuringOrderTbl LEFT OUTER JOIN ItemTypeTbl ON ReoccuringOrderTbl.ItemRequiredID = ItemTypeTbl.ItemTypeID)   LEFT OUTER JOIN CustomersTbl ON ReoccuringOrderTbl.CustomerID = CustomersTbl.CustomerID)   LEFT OUTER JOIN ReoccuranceTypeTbl ON ReoccuringOrderTbl.ReoccuranceType = ReoccuranceTypeTbl.ID)";
            string str = "";
            switch (IsEnabled)
            {
                case 0:
                    str = " WHERE ReoccuringOrderTbl.enabled = false";
                    break;
                case 1:
                    str += " WHERE ReoccuringOrderTbl.enabled = true";
                    break;
            }
            if (!string.IsNullOrWhiteSpace(WhereFilter))
                str = (!string.IsNullOrWhiteSpace(str) ? str + " AND " : " WHERE ") + WhereFilter;
            if (!string.IsNullOrWhiteSpace(str))
                strSQL += str;
            if (!string.IsNullOrEmpty(SortBy))
                strSQL = $"{strSQL} ORDER BY {SortBy}";
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
            if (dataReader != null)
            {
                while (dataReader.Read())
                {
                    ReoccuringOrderExtData reoccuringOrderExtData = new ReoccuringOrderExtData();
                    reoccuringOrderExtData.ReoccuringOrderID = dataReader["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["ID"]);
                    reoccuringOrderExtData.CustomerID = dataReader["CustomerID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["CustomerID"]);
                    reoccuringOrderExtData.ReoccuranceTypeID = dataReader["ReoccuranceType"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["ReoccuranceType"]);
                    reoccuringOrderExtData.ReoccuranceValue = dataReader["Value"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["Value"]);
                    reoccuringOrderExtData.ItemRequiredID = dataReader["ItemRequiredID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["ItemRequiredID"]);
                    reoccuringOrderExtData.QtyRequired = dataReader["QtyRequired"] == DBNull.Value ? 0.0 : Convert.ToDouble(dataReader["QtyRequired"]);
                    reoccuringOrderExtData.DateLastDone = dataReader["DateLastDone"] == DBNull.Value ? TrackerTools.STATIC_TrackerMinDate : Convert.ToDateTime(dataReader["DateLastDone"]).Date;
                    reoccuringOrderExtData.NextDateRequired = dataReader["NextDateRequired"] == DBNull.Value ? TimeZoneUtils.Now().Date : Convert.ToDateTime(dataReader["NextDateRequired"]).Date;
                    reoccuringOrderExtData.RequireUntilDate = dataReader["RequireUntilDate"] == DBNull.Value ? TrackerTools.STATIC_TrackerMinDate : Convert.ToDateTime(dataReader["RequireUntilDate"]).Date;
                    reoccuringOrderExtData.PackagingID = dataReader["PackagingID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["PackagingID"]);
                    reoccuringOrderExtData.Enabled = dataReader["Enabled"] != DBNull.Value && Convert.ToBoolean(dataReader["Enabled"]);
                    reoccuringOrderExtData.Notes = dataReader["Notes"] == DBNull.Value ? string.Empty : dataReader["Notes"].ToString();
                    reoccuringOrderExtData.CompanyName = dataReader["CompanyName"] == DBNull.Value ? string.Empty : dataReader["CompanyName"].ToString();
                    reoccuringOrderExtData.ItemTypeDesc = dataReader["ItemTypeDesc"] == DBNull.Value ? string.Empty : dataReader["ItemTypeDesc"].ToString();
                    reoccuringOrderExtData.ReoccuranceTypeDesc = dataReader["ReoccuranceTypeDesc"] == DBNull.Value ? string.Empty : dataReader["ReoccuranceTypeDesc"].ToString();
                    all.Add(reoccuringOrderExtData);
                }
                dataReader.Close();
            }
            trackerDb.Close();
            return all;
        }

        public ReoccuringOrderTbl GetByReoccuringOrderByID(int pReoccuringID)
        {
            ReoccuringOrderTbl reoccuringOrderById = (ReoccuringOrderTbl)null;
            string strSQL = "SELECT CustomerID, ReoccuranceType, [Value], ItemRequiredID, QtyRequired, DateLastDone, NextDateRequired, RequireUntilDate, PackagingID, DeliveryByID, Enabled, Notes  FROM ReoccuringOrderTbl WHERE ID = ?";
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddWhereParams((object)pReoccuringID, DbType.Int32, "@RecoccuringID");
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
            if (dataReader != null)
            {
                if (dataReader.Read())
                {
                    reoccuringOrderById = new ReoccuringOrderTbl();
                    reoccuringOrderById.ReoccuringOrderID = pReoccuringID;
                    reoccuringOrderById.CustomerID = dataReader["CustomerID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["CustomerID"]);
                    reoccuringOrderById.ReoccuranceTypeID = dataReader["ReoccuranceType"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["ReoccuranceType"]);
                    reoccuringOrderById.ReoccuranceValue = dataReader["Value"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["Value"]);
                    reoccuringOrderById.ItemRequiredID = dataReader["ItemRequiredID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["ItemRequiredID"]);
                    reoccuringOrderById.QtyRequired = dataReader["QtyRequired"] == DBNull.Value ? 0.0 : Convert.ToDouble(dataReader["QtyRequired"]);
                    reoccuringOrderById.DateLastDone = dataReader["DateLastDone"] == DBNull.Value ? TrackerTools.STATIC_TrackerMinDate : Convert.ToDateTime(dataReader["DateLastDone"]).Date;
                    reoccuringOrderById.NextDateRequired = dataReader["NextDateRequired"] == DBNull.Value ? TimeZoneUtils.Now().Date : Convert.ToDateTime(dataReader["NextDateRequired"]).Date;
                    reoccuringOrderById.RequireUntilDate = dataReader["RequireUntilDate"] == DBNull.Value ? TrackerTools.STATIC_TrackerMinDate : Convert.ToDateTime(dataReader["RequireUntilDate"]).Date;
                    reoccuringOrderById.PackagingID = dataReader["PackagingID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["PackagingID"]);
                    reoccuringOrderById.Enabled = dataReader["Enabled"] != DBNull.Value && Convert.ToBoolean(dataReader["Enabled"]);
                    reoccuringOrderById.Notes = dataReader["Notes"] == DBNull.Value ? string.Empty : dataReader["Notes"].ToString();
                }
                dataReader.Close();
            }
            trackerDb.Close();
            return reoccuringOrderById;
        }

        private DateTime GetMonday(DateTime pDate)
        {
            int num = (int)(1 - pDate.DayOfWeek);
            return pDate.AddDays((double)num);
        }

        public string UpdateReoccuringOrder(
          ReoccuringOrderTbl pReoccuranceTypeTbl,
          long pOrig_ReoccuringIDToUpdate)
        {
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddParams((object)pReoccuranceTypeTbl.CustomerID, DbType.Int64, "@CustomerID");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.ReoccuranceTypeID, DbType.Int32, "@ReoccuranceTypeID");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.ReoccuranceValue, DbType.Int32, "@ReoccuranceValue");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.ItemRequiredID, DbType.Int32, "@ItemRequiredID");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.QtyRequired, DbType.Double, "@QtyRequired");
            trackerDb.AddParams((object)this.GetMonday(pReoccuranceTypeTbl.DateLastDone), DbType.DateTime, "@DateLastDone");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.NextDateRequired, DbType.DateTime, "@NextDateRequired");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.RequireUntilDate, DbType.DateTime, "@RequireUntilDate");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.PackagingID, DbType.Int32, "@PackagingID");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.Enabled, DbType.Boolean, "@Enabled");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.Notes, DbType.String, "@Notes");
            trackerDb.AddWhereParams((object)pOrig_ReoccuringIDToUpdate, DbType.Int32, "@ReoccuringID");
            string str = trackerDb.ExecuteNonQuerySQL("UPDATE ReoccuringOrderTbl SET CustomerID = ?, ReoccuranceType = ?, [Value]= ?,  ItemRequiredID = ?, QtyRequired= ?, DateLastDone= ?, NextDateRequired= ?, RequireUntilDate = ?, PackagingID = ?, Enabled = ?, Notes = ? WHERE ReoccuringOrderTbl.ID = ?");
            trackerDb.Close();
            return str;
        }

        public string InsertReoccuringOrder(ReoccuringOrderTbl pReoccuranceTypeTbl)
        {
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddParams((object)pReoccuranceTypeTbl.CustomerID, DbType.Int64, "@CustomerID");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.ReoccuranceTypeID, DbType.Int32, "@ReoccuranceTypeID");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.ReoccuranceValue, DbType.Int32, "@ReoccuranceValue");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.ItemRequiredID, DbType.Int32, "@ItemRequiredID");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.QtyRequired, DbType.Double, "@QtyRequired");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.DateLastDone, DbType.DateTime, "@DateLastDone");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.NextDateRequired, DbType.DateTime, "@NextDateRequired");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.RequireUntilDate, DbType.DateTime, "@RequireUntilDate");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.PackagingID, DbType.Int32, "@PackagingID");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.Enabled, DbType.Boolean, "@Enabled");
            trackerDb.AddParams((object)pReoccuranceTypeTbl.Notes, DbType.String, "@Notes");
            string str = trackerDb.ExecuteNonQuerySQL("INSERT INTO ReoccuringOrderTbl (CustomerID, ReoccuranceType, [Value], ItemRequiredID, QtyRequired,  DateLastDone, NextDateRequired, RequireUntilDate, PackagingID, Enabled, Notes)  VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)");
            trackerDb.Close();
            return str;
        }

        public string DeleteReoccuringOrder(long pReoccuringIDToDelete)
        {
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddWhereParams((object)pReoccuringIDToDelete, DbType.Int64, "@ReoccuringID");
            string str = trackerDb.ExecuteNonQuerySQL("DELETE FROM ReoccuringOrderTbl WHERE ReoccuringOrderTbl.ID = ?");
            trackerDb.Close();
            return str;
        }

        public bool SetReoccuringItemsLastDate()
        {
            List<ReoccuringOrderTbl> reoccuringOrderTblList = new List<ReoccuringOrderTbl>();
            TrackerDb trackerDb = new TrackerDb();
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(" SELECT CustomerID, ItemRequiredID, MAX(LastDate) AS LastDatePerItem FROM (SELECT ReoccuringOrderTbl.CustomerID, ReoccuringOrderTbl.ItemRequiredID, ClientUsageLinesTbl.[Date] AS LastDate FROM  ((ClientUsageLinesTbl INNER JOIN ReoccuringOrderTbl ON ClientUsageLinesTbl.CustomerID = ReoccuringOrderTbl.CustomerID AND ClientUsageLinesTbl.[Date] > ReoccuringOrderTbl.DateLastDone) INNER JOIN ItemTypeTbl ON ReoccuringOrderTbl.ItemRequiredID = ItemTypeTbl.ItemTypeID)) ListOfOrdersRequired GROUP BY CustomerID, ItemRequiredID");
            bool flag = dataReader != null;
            if (flag)
            {
                while (dataReader.Read())
                    reoccuringOrderTblList.Add(new ReoccuringOrderTbl()
                    {
                        CustomerID = dataReader["CustomerID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["CustomerID"]),
                        ItemRequiredID = dataReader["ItemRequiredID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["ItemRequiredID"]),
                        DateLastDone = dataReader["LastDatePerItem"] == DBNull.Value ? TimeZoneUtils.Now().Date : Convert.ToDateTime(dataReader["LastDatePerItem"]).Date
                    });
                dataReader.Close();
            }
            trackerDb.Close();
            for (int index = 0; index < reoccuringOrderTblList.Count; ++index)
            {
                trackerDb.Open();
                trackerDb.AddParams((object)this.GetMonday(reoccuringOrderTblList[index].DateLastDone), DbType.Date, "@DateLastDone");
                trackerDb.AddWhereParams((object)reoccuringOrderTblList[index].CustomerID, DbType.Int64, "@CustomerID");
                trackerDb.AddWhereParams((object)reoccuringOrderTblList[index].ItemRequiredID, DbType.Int32, "@ItemRequiredID");
                flag = flag || string.IsNullOrEmpty(trackerDb.ExecuteNonQuerySQL("UPDATE ReoccuringOrderTbl SET DateLastDone = ? WHERE (CustomerID = ?) AND (ItemRequiredID = ?)"));
                trackerDb.Close();
            }
            return flag;
        }

        public string SetReoccuringOrdersLastDate(DateTime pDate, long pReoccuringOrderId)
        {
            string empty = string.Empty;
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddParams((object)pDate, DbType.Date, "@DateLastDone");
            trackerDb.AddWhereParams((object)pReoccuringOrderId, DbType.Int64, "@ID");
            string str = trackerDb.ExecuteNonQuerySQL("UPDATE ReoccuringOrderTbl SET DateLastDone = ? WHERE (ID = ?)");
            trackerDb.Close();
            return str;
        }
    }
}
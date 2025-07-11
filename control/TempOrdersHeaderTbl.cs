﻿// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.control.TempOrdersHeaderTbl
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.Data;
using TrackerDotNet.classes;

//- only form later versions #nullable disable
namespace TrackerDotNet.control
{
    public class TempOrdersHeaderTbl
    {
        private const string CONST_SQL_SELECT = "SELECT TOHeaderID, CustomerID, OrderDate, RoastDate, RequiredByDate, ToBeDeliveredByID, Confirmed, Done, Notes FROM TempOrdersHeaderTbl";
        private const string CONST_SQL_GETLASTHEADERID = "SELECT TOP 1 TOHeaderID FROM TempOrdersHeaderTbl ORDER By TOHeaderID DESC";
        private const string CONST_SQL_INSERT = "INSERT INTO TempOrdersHeaderTbl (CustomerId, OrderDate, RoastDate, RequiredByDate, ToBeDeliveredByID, Confirmed, Done, Notes)  VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
        private const string CONST_SQL_MARKTEMPORDERSASDONE = "UPDATE OrdersTbl SET OrdersTbl.Done = True WHERE CustomderId = ? AND EXISTS (SELECT RequiredByDate FROM TempOrdersHeaderTbl  WHERE (RequiredByDate = OrdersTbl.RequiredByDate))";
        private const string CONST_SQL_DELETEALL = "DELETE * FROM TempOrdersHeaderTbl";
        private int _TOHeaderID;
        private int _CustomerID;
        private DateTime _OrderDate;
        private DateTime _RoastDate;
        private DateTime _RequiredByDate;
        private int _ToBeDeliveredByID;
        private bool _Confirmed;
        private bool _Done;
        private string _Notes;

        public TempOrdersHeaderTbl()
        {
            this._TOHeaderID = 0L;
            this._CustomerID = 0L;
            this._OrderDate = DateTime.Now.Date;
            this._RoastDate = DateTime.Now.Date;
            this._RequiredByDate = DateTime.Now.Date;
            this._ToBeDeliveredByID = 0;
            this._Confirmed = false;
            this._Done = false;
            this._Notes = string.Empty;
        }

        public int TOHeaderID
        {
            get => this._TOHeaderID;
            set => this._TOHeaderID = value;
        }

        public int CustomerID
        {
            get => this._CustomerID;
            set => this._CustomerID = value;
        }

        public DateTime OrderDate
        {
            get => this._OrderDate;
            set => this._OrderDate = value;
        }

        public DateTime RoastDate
        {
            get => this._RoastDate;
            set => this._RoastDate = value;
        }

        public DateTime RequiredByDate
        {
            get => this._RequiredByDate;
            set => this._RequiredByDate = value;
        }

        public int ToBeDeliveredByID
        {
            get => this._ToBeDeliveredByID;
            set => this._ToBeDeliveredByID = value;
        }

        public bool Confirmed
        {
            get => this._Confirmed;
            set => this._Confirmed = value;
        }

        public bool Done
        {
            get => this._Done;
            set => this._Done = value;
        }

        public string Notes
        {
            get => this._Notes;
            set => this._Notes = value;
        }

        public List<TempOrdersHeaderTbl> GetAll(string SortBy)
        {
            List<TempOrdersHeaderTbl> all = new List<TempOrdersHeaderTbl>();
            string strSQL = "SELECT TOHeaderID, CustomerID, OrderDate, RoastDate, RequiredByDate, ToBeDeliveredByID, Confirmed, Done, Notes FROM TempOrdersHeaderTbl";
            if (!string.IsNullOrEmpty(SortBy))
                strSQL = $"{strSQL} ORDER BY {SortBy}";
            TrackerDb trackerDb = new TrackerDb();
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
            if (dataReader != null)
            {
                while (dataReader.Read())
                    all.Add(new TempOrdersHeaderTbl()
                    {
                        TOHeaderID = dataReader["TOHeaderID"] == DBNull.Value ? 0L : Convert.ToInt32(dataReader["TOHeaderID"]),
                        CustomerID = dataReader["CustomerID"] == DBNull.Value ? 0L : Convert.ToInt32(dataReader["CustomerID"]),
                        OrderDate = dataReader["OrderDate"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["OrderDate"]).Date,
                        RoastDate = dataReader["RoastDate"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["RoastDate"]).Date,
                        RequiredByDate = dataReader["RequiredByDate"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["RequiredByDate"]).Date,
                        ToBeDeliveredByID = dataReader["ToBeDeliveredByID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["ToBeDeliveredByID"]),
                        Confirmed = dataReader["Confirmed"] != DBNull.Value && Convert.ToBoolean(dataReader["Confirmed"]),
                        Done = dataReader["Done"] != DBNull.Value && Convert.ToBoolean(dataReader["Done"]),
                        Notes = dataReader["Notes"] == DBNull.Value ? string.Empty : dataReader["Notes"].ToString()
                    });
                dataReader.Close();
            }
            trackerDb.Close();
            return all;
        }

        public bool Insert(TempOrdersHeaderTbl pHeaderData)
        {
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddParams((object)pHeaderData.CustomerID, DbType.Int64, "@CustomerID");
            trackerDb.AddParams((object)pHeaderData.OrderDate, DbType.Date, "@OrderDate");
            trackerDb.AddParams((object)pHeaderData.RoastDate, DbType.Date, "@RoastDate");
            trackerDb.AddParams((object)pHeaderData.RequiredByDate, DbType.Date, "@RequiredByDate");
            trackerDb.AddParams((object)pHeaderData.ToBeDeliveredByID, DbType.Int32, "@ToBeDeliveredByID");
            trackerDb.AddParams((object)pHeaderData.Confirmed, DbType.Boolean, "@Confirmed");
            trackerDb.AddParams((object)pHeaderData.Done, DbType.Boolean, "@Done");
            trackerDb.AddParams((object)pHeaderData.Notes, DbType.String, "@Notes");
            bool flag = string.IsNullOrEmpty(trackerDb.ExecuteNonQuerySQL("INSERT INTO TempOrdersHeaderTbl (CustomerId, OrderDate, RoastDate, RequiredByDate, ToBeDeliveredByID, Confirmed, Done, Notes)  VALUES (?, ?, ?, ?, ?, ?, ?, ?)"));
            trackerDb.Close();
            return flag;
        }

        public int GetCurrentTOHeaderID()
        {
            long currentToHeaderId = 0;
            TrackerDb trackerDb = new TrackerDb();
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader("SELECT TOP 1 TOHeaderID FROM TempOrdersHeaderTbl ORDER By TOHeaderID DESC");
            if (dataReader != null)
            {
                if (dataReader.Read())
                    currentToHeaderId = dataReader["TOHeaderID"] == DBNull.Value ? 0L : Convert.ToInt32(dataReader["TOHeaderID"]);
                dataReader.Close();
            }
            trackerDb.Close();
            return currentToHeaderId;
        }

        public bool DeleteAllRecords()
        {
            TrackerDb trackerDb = new TrackerDb();
            bool flag = string.IsNullOrEmpty(trackerDb.ExecuteNonQuerySQL("DELETE * FROM TempOrdersHeaderTbl"));
            trackerDb.Close();
            return flag;
        }
    }
}
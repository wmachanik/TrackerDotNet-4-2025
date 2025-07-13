﻿// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.control.NextRoastDateByCityTbl
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.Data;
using TrackerDotNet.classes;

// #nullable disable --- not for this version of C#
namespace TrackerDotNet.control
{

    public class NextRoastDateByCityTbl
    {
        private const string CONST_SELECTBYCUSTOMERID = "SELECT NextRoastDateByCityTbl.CityID,  NextRoastDateByCityTbl.PreperationDate, NextRoastDateByCityTbl.DeliveryDate, NextRoastDateByCityTbl.DeliveryOrder,  NextRoastDateByCityTbl.NextPreperationDate,  NextRoastDateByCityTbl.NextDeliveryDate  FROM  (NextRoastDateByCityTbl RIGHT OUTER JOIN CustomersTbl ON NextRoastDateByCityTbl.CityID = CustomersTbl.City)  WHERE (CustomersTbl.CustomerID = ?) ";
        private const string CONST_SELECTALL = "SELECT NextRoastDateByCityTbl.CityID, NextRoastDateByCityTbl.PreperationDate, NextRoastDateByCityTbl.DeliveryDate,  NextRoastDateByCityTbl.DeliveryOrder, NextRoastDateByCityTbl.NextPreperationDate, NextRoastDateByCityTbl.NextDeliveryDate  FROM NextRoastDateByCityTbl";
        private const string CONST_SELECTALLDELIVERYDATES = "SELECT DISTINCT DeliveryDate FROM NextRoastDateByCityTbl ORDER BY DeliveryDate ";
        private const string CONST_SELECTIDBYDELIVERYDATES = "SELECT NextRoastDayID FROM NextRoastDateByCityTbl WHERE (DeliveryDate = ?)";
        private const string CONST_UPDATE = "UPDATE NextRoastDateByCityTbl SET PreperationDate = ?, DeliveryDate = ?, DeliveryOrder = ?, NextDeliveryDate = ?, NextPreperationDate = ? WHERE CityID = ?";
        private const string CONST_INSERT = "INSERT INTO NextRoastDateByCityTbl (CityID, PreperationDate, DeliveryDate, DeliveryOrder, NextDeliveryDate, NextPreperationDate) VALUES (?,?,?,?,?,?)";
        private const string CONST_UPDATE_MOVEDELIVERYDATE = "UPDATE NextRoastDateByCityTbl SET DeliveryDate = ? WHERE (NextRoastDateByCityTbl.DeliveryDate = ?)";
        private const string CONST_UPDATEDELIVERYDATEBYID = "UPDATE NextRoastDateByCityTbl SET DeliveryDate = ? WHERE (NextRoastDateByCityTbl.NextRoastDayID = ?)";
        private const string CONST_SQL_CUSTOMERSNEXTDELIVERYDATE = "SELECT NextRoastDateByCityTbl.DeliveryDate FROM  (CustomersTbl INNER JOIN  NextRoastDateByCityTbl ON CustomersTbl.City = NextRoastDateByCityTbl.CityID) WHERE (CustomersTbl.CustomerID = ?)";
        private long _CityID;
        private DateTime _DeliveryDate;
        private DateTime _PrepDate;
        private int _DeliveryOrder;
        private DateTime _NextDeliveryDate;
        private DateTime _NextPrepDate;

        public NextRoastDateByCityTbl()
        {
            this._CityID = 0L;
            this._DeliveryOrder = 0;
            this._DeliveryDate = this._PrepDate = DateTime.MinValue;
            this._NextDeliveryDate = this._NextPrepDate = DateTime.MinValue;
        }

        public long CityID
        {
            get => this._CityID;
            set => this._CityID = value;
        }

        public DateTime DeliveryDate
        {
            get => this._DeliveryDate.Date;
            set => this._DeliveryDate = value;
        }

        public DateTime PrepDate
        {
            get => this._PrepDate.Date;
            set => this._PrepDate = value;
        }

        public int DeliveryOrder
        {
            get => this._DeliveryOrder;
            set => this._DeliveryOrder = value;
        }

        public DateTime NextDeliveryDate
        {
            get => this._NextDeliveryDate.Date;
            set => this._NextDeliveryDate = value;
        }

        public DateTime NextPrepDate
        {
            get => this._NextPrepDate.Date;
            set => this._NextPrepDate = value;
        }

        public NextRoastDateByCityTbl GetPrepDataForCustomer(long pCustomerID)
        {
            NextRoastDateByCityTbl prepDataForCustomer = new NextRoastDateByCityTbl();
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddWhereParams((object)pCustomerID, DbType.Int64, "@CustomerID");
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader("SELECT NextRoastDateByCityTbl.CityID,  NextRoastDateByCityTbl.PreperationDate, NextRoastDateByCityTbl.DeliveryDate, NextRoastDateByCityTbl.DeliveryOrder,  NextRoastDateByCityTbl.NextPreperationDate,  NextRoastDateByCityTbl.NextDeliveryDate  FROM  (NextRoastDateByCityTbl RIGHT OUTER JOIN CustomersTbl ON NextRoastDateByCityTbl.CityID = CustomersTbl.City)  WHERE (CustomersTbl.CustomerID = ?) ");
            if (dataReader != null)
            {
                if (dataReader.Read())
                {
                    prepDataForCustomer.CityID = dataReader["CityID"] == DBNull.Value ? 0L : (long)Convert.ToInt32(dataReader["CityID"]);
                    prepDataForCustomer.PrepDate = dataReader["PreperationDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataReader["PreperationDate"]).Date;
                    prepDataForCustomer.DeliveryDate = dataReader["DeliveryDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataReader["DeliveryDate"]).Date;
                    prepDataForCustomer.DeliveryOrder = dataReader["DeliveryOrder"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["DeliveryOrder"]);
                    prepDataForCustomer.NextPrepDate = dataReader["NextPreperationDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataReader["NextPreperationDate"]).Date;
                    prepDataForCustomer.NextDeliveryDate = dataReader["NextDeliveryDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataReader["NextDeliveryDate"]).Date;
                }
                dataReader.Dispose();
            }
            trackerDb.Close();
            return prepDataForCustomer;
        }

        public List<NextRoastDateByCityTbl> GetAll(string SortBy)
        {
            List<NextRoastDateByCityTbl> all = new List<NextRoastDateByCityTbl>();
            TrackerDb trackerDb = new TrackerDb();
            string strSQL = "SELECT NextRoastDateByCityTbl.CityID, NextRoastDateByCityTbl.PreperationDate, NextRoastDateByCityTbl.DeliveryDate,  NextRoastDateByCityTbl.DeliveryOrder, NextRoastDateByCityTbl.NextPreperationDate, NextRoastDateByCityTbl.NextDeliveryDate  FROM NextRoastDateByCityTbl";
            if (!string.IsNullOrEmpty(SortBy))
                strSQL = $"{strSQL} ORDER BY {SortBy}";
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
            if (dataReader != null)
            {
                while (dataReader.Read())
                    all.Add(new NextRoastDateByCityTbl()
                    {
                        CityID = dataReader["CityID"] == DBNull.Value ? 0L : (long)Convert.ToInt32(dataReader["CityID"]),
                        PrepDate = dataReader["PreperationDate"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["PreperationDate"]).Date,
                        DeliveryDate = dataReader["DeliveryDate"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["DeliveryDate"]).Date,
                        DeliveryOrder = dataReader["DeliveryOrder"] == DBNull.Value ? 100 : Convert.ToInt32(dataReader["DeliveryOrder"]),
                        NextPrepDate = dataReader["NextPreperationDate"] == DBNull.Value ? DateTime.Now.Date.AddDays(7.0).Date : Convert.ToDateTime(dataReader["NextPreperationDate"]).Date,
                        NextDeliveryDate = dataReader["NextDeliveryDate"] == DBNull.Value ? DateTime.Now.Date.AddDays(7.0).Date : Convert.ToDateTime(dataReader["NextDeliveryDate"]).Date
                    });
                dataReader.Dispose();
            }
            trackerDb.Close();
            return all;
        }

        public List<DateTime> GetAllDeliveryDates()
        {
            List<DateTime> allDeliveryDates = new List<DateTime>();
            TrackerDb trackerDb = new TrackerDb();
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader("SELECT DISTINCT DeliveryDate FROM NextRoastDateByCityTbl ORDER BY DeliveryDate ");
            if (dataReader != null)
            {
                while (dataReader.Read())
                {
                    DateTime dateTime = dataReader["DeliveryDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataReader["DeliveryDate"]).Date;
                    allDeliveryDates.Add(dateTime);
                }
                dataReader.Dispose();
            }
            trackerDb.Close();
            return allDeliveryDates;
        }

        public List<int> GetAllIDsByDate(DateTime pDeliveryDate)
        {
            List<int> allIdsByDate = new List<int>();
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddWhereParams((object)pDeliveryDate, DbType.Date);
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader("SELECT NextRoastDayID FROM NextRoastDateByCityTbl WHERE (DeliveryDate = ?)");
            if (dataReader != null)
            {
                while (dataReader.Read())
                {
                    int int32 = dataReader["NextRoastDayID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["NextRoastDayID"]);
                    allIdsByDate.Add(int32);
                }
                dataReader.Dispose();
            }
            trackerDb.Close();
            return allIdsByDate;
        }

        public string UpdatePrepDataForCity(int pCityID, NextRoastDateByCityTbl pNextRoastCityTbl)
        {
            string empty = string.Empty;
            TrackerDb trackerDb = new TrackerDb();
            string str;
            if (pNextRoastCityTbl != null)
            {
                trackerDb.AddParams((object)pNextRoastCityTbl.PrepDate, DbType.Date, "@PreperationDate");
                trackerDb.AddParams((object)pNextRoastCityTbl.DeliveryDate, DbType.Date, "@DeliveryDate");
                trackerDb.AddParams((object)pNextRoastCityTbl.DeliveryOrder, DbType.Int16, "@DeliveryOrder");
                trackerDb.AddParams((object)pNextRoastCityTbl.NextDeliveryDate, DbType.Date, "@NextDeliveryDate");
                trackerDb.AddParams((object)pNextRoastCityTbl.NextPrepDate, DbType.Date, "@NextPreperationDate");
                trackerDb.AddWhereParams((object)pNextRoastCityTbl.CityID, DbType.Int64, "@CityID");
                str = trackerDb.ExecuteNonQuerySQL("UPDATE NextRoastDateByCityTbl SET PreperationDate = ?, DeliveryDate = ?, DeliveryOrder = ?, NextDeliveryDate = ?, NextPreperationDate = ? WHERE CityID = ?");
            }
            else
                str = "null data passed-update failed";
            trackerDb.Close();
            return str;
        }

        public string InsertPrepDataForCity(NextRoastDateByCityTbl pNextRoastCityTbl)
        {
            string empty = string.Empty;
            TrackerDb trackerDb = new TrackerDb();
            string str;
            if (pNextRoastCityTbl != null)
            {
                trackerDb.AddParams((object)pNextRoastCityTbl.CityID, DbType.Int64);
                trackerDb.AddParams((object)pNextRoastCityTbl.PrepDate, DbType.Date);
                trackerDb.AddParams((object)pNextRoastCityTbl.DeliveryDate, DbType.Date);
                trackerDb.AddParams((object)pNextRoastCityTbl.DeliveryOrder, DbType.Int16);
                trackerDb.AddParams((object)pNextRoastCityTbl.NextDeliveryDate, DbType.Date);
                trackerDb.AddParams((object)pNextRoastCityTbl.NextPrepDate, DbType.Date);
                str = trackerDb.ExecuteNonQuerySQL("INSERT INTO NextRoastDateByCityTbl (CityID, PreperationDate, DeliveryDate, DeliveryOrder, NextDeliveryDate, NextPreperationDate) VALUES (?,?,?,?,?,?)");
            }
            else
                str = "null data passed-insert failed";
            trackerDb.Close();
            return str;
        }

        public string MoveDeliveryDate(
          DateTime pOldDeliveryDate,
          DateTime pNewDeliveryDate,
          ref int pNumRecs)
        {
            string empty = string.Empty;
            string strSQL = "UPDATE NextRoastDateByCityTbl SET DeliveryDate = ? WHERE (NextRoastDateByCityTbl.DeliveryDate = ?)";
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddParams((object)pNewDeliveryDate.Date, DbType.DateTime);
            trackerDb.AddWhereParams((object)pOldDeliveryDate, DbType.Date);
            string str = trackerDb.ExecuteNonQuerySQL(strSQL);
            pNumRecs = trackerDb.numRecs;
            trackerDb.Close();
            return str;
        }

        public string UpdateDeliveryDateByID(int pNextRoastID, DateTime pDeliveryDate)
        {
            string empty = string.Empty;
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddParams((object)pDeliveryDate.Date, DbType.DateTime);
            trackerDb.AddWhereParams((object)pNextRoastID, DbType.Int32);
            string str = trackerDb.ExecuteNonQuerySQL("UPDATE NextRoastDateByCityTbl SET DeliveryDate = ? WHERE (NextRoastDateByCityTbl.NextRoastDayID = ?)");
            trackerDb.Close();
            return str;
        }

        public DateTime GetNextDeliveryDate(long pCustomerId)
        {
            DateTime nextDeliveryDate = DateTime.MinValue;
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddWhereParams((object)pCustomerId, DbType.Int64);
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader("SELECT NextRoastDateByCityTbl.DeliveryDate FROM  (CustomersTbl INNER JOIN  NextRoastDateByCityTbl ON CustomersTbl.City = NextRoastDateByCityTbl.CityID) WHERE (CustomersTbl.CustomerID = ?)");
            if (dataReader != null)
            {
                if (dataReader.Read())
                    nextDeliveryDate = dataReader["DeliveryDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dataReader["DeliveryDate"]).Date;
                dataReader.Dispose();
            }
            trackerDb.Close();
            return nextDeliveryDate;
        }
    }
}

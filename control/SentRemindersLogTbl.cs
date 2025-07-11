﻿// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.control.SentRemindersLogTbl
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
    public class SentRemindersLogTbl
    {
        private const string CONST_SQL_SELECT = "SELECT ReminderID, CustomerID, DateSentReminder, NextPrepDate, ReminderSent, HadAutoFulfilItem, HadReoccurItems FROM SentRemindersLogTbl";
        private const string CONST_SQL_SELECT_BYDATESENT = "SELECT ReminderID, CustomerID, DateSentReminder, NextPrepDate, ReminderSent, HadAutoFulfilItem, HadReoccurItems  FROM SentRemindersLogTbl WHERE DateSentReminder = ?";
        private const string CONST_SQL_SELECT_REMINDERDATESSENT = "SELECT DISTINCT TOP 20 DateSentReminder FROM SentRemindersLogTbl ORDER BY DateSentReminder DESC";
        private const string CONST_SQL_UPDATE = "UPDATE SentRemindersLogTbl SET CustomerID = ?, DateSentReminder = ?, NextPrepDate = ?, ReminderSent = ?, HadAutoFulfilItem = ?, HadReoccurItems = ?  WHERE (SentRemindersLogTbl.ReminderID = ?)";
        private const string CONST_SQL_INSERT = "INSERT INTO SentRemindersLogTbl (CustomerID, DateSentReminder, NextPrepDate, ReminderSent, HadAutoFulfilItem, HadReoccurItems) VALUES (?, ?, ?, ?, ?, ?)";
        private int _ReminderID;
        private int _CustomerID;
        private DateTime _DateSentReminder;
        private DateTime _NextPrepDate;
        private bool _ReminderSent;
        private bool _HadAutoFulfilItem;
        private bool _HadReoccurItems;

        public SentRemindersLogTbl()
        {
            this._ReminderID = 0;
            this._CustomerID = 0L;
            this._DateSentReminder = DateTime.Now;
            this._NextPrepDate = DateTime.Now.Date;
            this._ReminderSent = false;
            this._HadAutoFulfilItem = false;
            this._HadReoccurItems = false;
        }

        public int ReminderID
        {
            get => this._ReminderID;
            set => this._ReminderID = value;
        }

        public int CustomerID
        {
            get => this._CustomerID;
            set => this._CustomerID = value;
        }

        public DateTime DateSentReminder
        {
            get => this._DateSentReminder;
            set => this._DateSentReminder = value;
        }

        public DateTime NextPrepDate
        {
            get => this._NextPrepDate;
            set => this._NextPrepDate = value;
        }

        public bool ReminderSent
        {
            get => this._ReminderSent;
            set => this._ReminderSent = value;
        }

        public bool HadAutoFulfilItem
        {
            get => this._HadAutoFulfilItem;
            set => this._HadAutoFulfilItem = value;
        }

        public bool HadReoccurItems
        {
            get => this._HadReoccurItems;
            set => this._HadReoccurItems = value;
        }

        public List<SentRemindersLogTbl> GetAll(string SortBy)
        {
            List<SentRemindersLogTbl> all = new List<SentRemindersLogTbl>();
            TrackerDb trackerDb = new TrackerDb();
            string strSQL = "SELECT ReminderID, CustomerID, DateSentReminder, NextPrepDate, ReminderSent, HadAutoFulfilItem, HadReoccurItems FROM SentRemindersLogTbl";
            if (!string.IsNullOrEmpty(SortBy))
                strSQL = $"{strSQL} ORDER BY {SortBy}";
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
            if (dataReader != null)
            {
                while (dataReader.Read())
                    all.Add(new SentRemindersLogTbl()
                    {
                        ReminderID = dataReader["ReminderID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["ReminderID"]),
                        CustomerID = dataReader["CustomerID"] == DBNull.Value ? 0L : Convert.ToInt32(dataReader["CustomerID"]),
                        DateSentReminder = dataReader["DateSentReminder"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["DateSentReminder"]).Date,
                        NextPrepDate = dataReader["NextPrepDate"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["NextPrepDate"]).Date,
                        ReminderSent = dataReader["ReminderSent"] != DBNull.Value && Convert.ToBoolean(dataReader["ReminderSent"]),
                        HadAutoFulfilItem = dataReader["HadAutoFulfilItem"] != DBNull.Value && Convert.ToBoolean(dataReader["HadAutoFulfilItem"]),
                        HadReoccurItems = dataReader["HadReoccurItems"] != DBNull.Value && Convert.ToBoolean(dataReader["HadReoccurItems"])
                    });
                dataReader.Close();
            }
            trackerDb.Close();
            return all;
        }

        public List<SentRemindersLogTbl> GetAllByDate(DateTime pDateSent, string SortBy)
        {
            List<SentRemindersLogTbl> allByDate = new List<SentRemindersLogTbl>();
            TrackerDb trackerDb = new TrackerDb();
            string strSQL = "SELECT ReminderID, CustomerID, DateSentReminder, NextPrepDate, ReminderSent, HadAutoFulfilItem, HadReoccurItems  FROM SentRemindersLogTbl WHERE DateSentReminder = ?";
            if (!string.IsNullOrEmpty(SortBy))
                strSQL = $"{strSQL} ORDER BY {SortBy}";
            trackerDb.AddWhereParams((object)pDateSent.Date, DbType.Date);
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
            if (dataReader != null)
            {
                while (dataReader.Read())
                    allByDate.Add(new SentRemindersLogTbl()
                    {
                        ReminderID = dataReader["ReminderID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["ReminderID"]),
                        CustomerID = dataReader["CustomerID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["CustomerID"]),
                        DateSentReminder = dataReader["DateSentReminder"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["DateSentReminder"]).Date,
                        NextPrepDate = dataReader["NextPrepDate"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["NextPrepDate"]).Date,
                        ReminderSent = dataReader["ReminderSent"] != DBNull.Value && Convert.ToBoolean(dataReader["ReminderSent"]),
                        HadAutoFulfilItem = dataReader["HadAutoFulfilItem"] != DBNull.Value && Convert.ToBoolean(dataReader["HadAutoFulfilItem"]),
                        HadReoccurItems = dataReader["HadReoccurItems"] != DBNull.Value && Convert.ToBoolean(dataReader["HadReoccurItems"])
                    });
                dataReader.Close();
            }
            trackerDb.Close();
            return allByDate;
        }

        public List<DateTime> GetLast20DatesReminderSent()
        {
            List<DateTime> datesReminderSent = new List<DateTime>();
            TrackerDb trackerDb = new TrackerDb();
            string strSQL = "SELECT DISTINCT TOP 20 DateSentReminder FROM SentRemindersLogTbl ORDER BY DateSentReminder DESC";
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
            if (dataReader != null)
            {
                while (dataReader.Read())
                {
                    //DateTime dateTime1 = new DateTime();
                    DateTime dateTime2 = dataReader["DateSentReminder"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["DateSentReminder"]).Date;
                    datesReminderSent.Add(dateTime2);
                }
                dataReader.Close();
            }
            trackerDb.Close();
            return datesReminderSent;
        }

        public string UpdateLogItem(SentRemindersLogTbl pSentRemindersLog)
        {
            string empty = string.Empty;
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddParams((object)pSentRemindersLog.CustomerID, DbType.Int64);
            trackerDb.AddParams((object)pSentRemindersLog.DateSentReminder, DbType.Date);
            trackerDb.AddParams((object)pSentRemindersLog.NextPrepDate, DbType.Date);
            trackerDb.AddParams((object)pSentRemindersLog.ReminderSent, DbType.Boolean);
            trackerDb.AddParams((object)pSentRemindersLog.HadAutoFulfilItem, DbType.Boolean);
            trackerDb.AddParams((object)pSentRemindersLog.HadReoccurItems, DbType.Boolean);
            trackerDb.AddWhereParams((object)pSentRemindersLog.ReminderID, DbType.Int32);
            string str = trackerDb.ExecuteNonQuerySQL("UPDATE SentRemindersLogTbl SET CustomerID = ?, DateSentReminder = ?, NextPrepDate = ?, ReminderSent = ?, HadAutoFulfilItem = ?, HadReoccurItems = ?  WHERE (SentRemindersLogTbl.ReminderID = ?)");
            trackerDb.Close();
            return str;
        }

        public string InsertLogItem(SentRemindersLogTbl pSentRemindersLog)
        {
            string empty = string.Empty;
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddParams((object)pSentRemindersLog.CustomerID, DbType.Int64);
            trackerDb.AddParams((object)pSentRemindersLog.DateSentReminder, DbType.Date);
            trackerDb.AddParams((object)pSentRemindersLog.NextPrepDate, DbType.Date);
            trackerDb.AddParams((object)pSentRemindersLog.ReminderSent, DbType.Boolean);
            trackerDb.AddParams((object)pSentRemindersLog.HadAutoFulfilItem, DbType.Boolean);
            trackerDb.AddParams((object)pSentRemindersLog.HadReoccurItems, DbType.Boolean);
            string str = trackerDb.ExecuteNonQuerySQL("INSERT INTO SentRemindersLogTbl (CustomerID, DateSentReminder, NextPrepDate, ReminderSent, HadAutoFulfilItem, HadReoccurItems) VALUES (?, ?, ?, ?, ?, ?)");
            trackerDb.Close();
            return str;
        }
    }
}
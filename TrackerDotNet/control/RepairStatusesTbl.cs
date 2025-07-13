// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.control.RepairStatusesTbl
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using TrackerDotNet.classes;

// #nullable disable --- not for this version of C#
namespace TrackerDotNet.control
{

    public class RepairStatusesTbl
    {
        private const string CONST_SQL_SELECT = "SELECT RepairStatusID, RepairStatusDesc, EmailClient, SortOrder, Notes FROM RepairStatusesTbl";
        private const string CONST_SQL_SELECTSTATUSDESC = "SELECT RepairStatusDesc FROM RepairStatusesTbl WHERE (RepairStatusID = ?)";
        private int _RepairStatusID;
        private string _RepairStatusDesc;
        private bool _EmailClient;
        private int _SortOrder;
        private string _Notes;

        public RepairStatusesTbl()
        {
            this._RepairStatusID = 0;
            this._RepairStatusDesc = string.Empty;
            this._EmailClient = false;
            this._SortOrder = 0;
            this._Notes = string.Empty;
        }

        public int RepairStatusID
        {
            get => this._RepairStatusID;
            set => this._RepairStatusID = value;
        }

        public string RepairStatusDesc
        {
            get => this._RepairStatusDesc;
            set => this._RepairStatusDesc = value;
        }

        public bool EmailClient
        {
            get => this._EmailClient;
            set => this._EmailClient = value;
        }

        public int SortOrder
        {
            get => this._SortOrder;
            set => this._SortOrder = value;
        }

        public string Notes
        {
            get => this._Notes;
            set => this._Notes = value;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<RepairStatusesTbl> GetAll(string SortBy)
        {
            List<RepairStatusesTbl> all = new List<RepairStatusesTbl>();
            TrackerDb trackerDb = new TrackerDb();
            string strSQL = $"SELECT RepairStatusID, RepairStatusDesc, EmailClient, SortOrder, Notes FROM RepairStatusesTbl ORDER BY {(string.IsNullOrEmpty(SortBy) ? "SortOrder" : SortBy)}";
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
            if (dataReader != null)
            {
                while (dataReader.Read())
                    all.Add(new RepairStatusesTbl()
                    {
                        RepairStatusID = dataReader["RepairStatusID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RepairStatusID"]),
                        RepairStatusDesc = dataReader["RepairStatusDesc"] == DBNull.Value ? string.Empty : dataReader["RepairStatusDesc"].ToString(),
                        EmailClient = dataReader["EmailClient"] != DBNull.Value && Convert.ToBoolean(dataReader["EmailClient"]),
                        SortOrder = dataReader["SortOrder"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["SortOrder"]),
                        Notes = dataReader["Notes"] == DBNull.Value ? string.Empty : dataReader["Notes"].ToString()
                    });
                dataReader.Close();
            }
            trackerDb.Close();
            return all;
        }

        public string GetRepairStatusDesc(int RepairStatusID)
        {
            string repairStatusDesc = string.Empty;
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddWhereParams((object)RepairStatusID, DbType.Int32);
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader("SELECT RepairStatusDesc FROM RepairStatusesTbl WHERE (RepairStatusID = ?)");
            if (dataReader != null)
            {
                if (dataReader.Read())
                    repairStatusDesc = dataReader["RepairStatusDesc"] == DBNull.Value ? string.Empty : dataReader["RepairStatusDesc"].ToString();
                dataReader.Close();
            }
            trackerDb.Close();
            return repairStatusDesc;
        }
    }
}

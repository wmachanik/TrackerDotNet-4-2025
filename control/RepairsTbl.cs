// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.control.RepairsTbl
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using TrackerDotNet.classes;

//- only form later versions #nullable disable
namespace TrackerDotNet.control
{
    public class RepairsTbl
    {
        public const int CONST_REPAIRSTATUS_LOGGED = 1;
        public const int CONST_REPAIRSTATUS_COLLECTED = 2;
        public const int CONST_REPAIRSTATUS_WORKSHOP = 3;
        public const int CONST_REPAIRSTATUS_WAITING = 4;
        public const int CONST_REPAIRSTATUS_ESTIMATE = 5;
        public const int CONST_REPAIRSTATUS_READY = 6;
        public const int CONST_REPAIRSTATUS_DONE = 7;
        private const string CONST_REPAIR_DONESTR = "7";
        private const string CONST_SQL_SELECT = "SELECT RepairID, CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, Notes FROM RepairsTbl";
        private const string CONST_SQL_SELECTNOTDONE = "SELECT RepairID, CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, RepairsTbl.Notes FROM RepairsTbl WHERE (RepairsTbl.RepairStatusID <> 7)";
        private const string CONST_SQL_SELECTONSTATUS = "SELECT RepairID, CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, RepairsTbl.Notes FROM RepairsTbl WHERE (RepairsTbl.RepairStatusID = ?)";
        private const string CONST_SQL_SELECTBYREPAIRID = "SELECT CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, Notes FROM RepairsTbl WHERE (RepairID = ?)";
        private const string CONST_SQL_SELECTLAST = "SELECT TOP 1 RepairID FROM RepairsTbl WHERE (CustomerID = ?) ORDER BY DateLogged DESC";
        private const string CONST_SQL_INSERT = "INSERT INTO RepairsTbl (CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairStatusID, RelatedOrderID, Notes)VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private const string CONST_SQL_UPDATE = "UPDATE RepairsTbl SET CustomerID = ?, ContactName = ?, ContactEmail = ?, JobCardNumber = ?, DateLogged = ?, LastStatusChange = ?, MachineTypeID = ?, MachineSerialNumber = ?, SwopOutMachineID = ?,  MachineConditionID = ?, TakenFrother = ?, TakenBeanLid = ?, TakenWaterLid = ?, BrokenFrother = ?,  BrokenBeanLid = ?, BrokenWaterLid = ?, RepairFaultID = ?, RepairFaultDesc = ?, RepairStatusID = ?,  RelatedOrderID = ?, Notes = ? WHERE (RepairsTbl.RepairID = ?)";
        private const string CONST_SQL_UPDATESTATUS = "UPDATE RepairsTbl SET RepairStatusID = ? WHERE (RepairsTbl.RepairID = ?)";
        private const string CONST_SQL_DELETE = "DELETE FROM RepairsTbl WHERE (RepairsTbl.RepairID = ?)";
        private const string CONST_SQL_SELECTITEMWITHANORDER = "SELECT  RepairID, CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, Notes FROM (RepairsTbl INNER JOIN TempOrdersLinesTbl ON RepairsTbl.RelatedOrderID = TempOrdersLinesTbl.OriginalOrderID)";
        private const string CONST_SQL_SETDONEBYID = "UPDATE RepairsTbl SET RepairStatusID = ? WHERE (RelatedOrderID = ?)";
        private int _RepairID;
        private long _CustomerID;
        private string _ContactName;
        private string _ContactEmail;
        private string _JobCardNumber;
        private DateTime _DateLogged;
        private DateTime _LastStatusChange;
        private int _MachineTypeID;
        private string _MachineSerialNumber;
        private int _SwopOutMachineID;
        private int _MachineConditionID;
        private bool _TakenFrother;
        private bool _TakenBeanLid;
        private bool _TakenWaterLid;
        private bool _BrokenFrother;
        private bool _BrokenBeanLid;
        private bool _BrokenWaterLid;
        private int _RepairFaultID;
        private string _RepairFaultDesc;
        private int _RepairStatusID;
        private int _RelatedOrderID;
        private string _Notes;

        public RepairsTbl()
        {
            this._RepairID = 0;
            this._CustomerID = 0;
            this._ContactName = string.Empty;
            this._ContactEmail = string.Empty;
            this._JobCardNumber = string.Empty;
            this._DateLogged = DateTime.MinValue;
            this._LastStatusChange = DateTime.Now;
            this._MachineTypeID = 0;
            this._MachineSerialNumber = string.Empty;
            this._SwopOutMachineID = 0;
            this._MachineConditionID = 0;
            this._TakenFrother = false;
            this._TakenBeanLid = true;
            this._TakenWaterLid = true;
            this._BrokenFrother = false;
            this._BrokenBeanLid = false;
            this._BrokenWaterLid = false;
            this._RepairFaultID = 0;
            this._RepairFaultDesc = string.Empty;
            this._RepairStatusID = 0;
            this._RelatedOrderID = 0;
            this._Notes = string.Empty;
        }

        public int RepairID
        {
            get => this._RepairID;
            set => this._RepairID = value;
        }

        public long CustomerID
        {
            get => this._CustomerID;
            set => this._CustomerID = value;
        }

        public string ContactName
        {
            get => this._ContactName;
            set => this._ContactName = value;
        }

        public string ContactEmail
        {
            get => this._ContactEmail;
            set => this._ContactEmail = value;
        }

        public string JobCardNumber
        {
            get => this._JobCardNumber;
            set => this._JobCardNumber = value;
        }

        public DateTime DateLogged
        {
            get => this._DateLogged;
            set => this._DateLogged = value;
        }

        public DateTime LastStatusChange
        {
            get => this._LastStatusChange;
            set => this._LastStatusChange = value;
        }

        public int MachineTypeID
        {
            get => this._MachineTypeID;
            set => this._MachineTypeID = value;
        }

        public string MachineSerialNumber
        {
            get => this._MachineSerialNumber;
            set => this._MachineSerialNumber = value;
        }

        public int SwopOutMachineID
        {
            get => this._SwopOutMachineID;
            set => this._SwopOutMachineID = value;
        }

        public int MachineConditionID
        {
            get => this._MachineConditionID;
            set => this._MachineConditionID = value;
        }

        public bool TakenFrother
        {
            get => this._TakenFrother;
            set => this._TakenFrother = value;
        }

        public bool TakenBeanLid
        {
            get => this._TakenBeanLid;
            set => this._TakenBeanLid = value;
        }

        public bool TakenWaterLid
        {
            get => this._TakenWaterLid;
            set => this._TakenWaterLid = value;
        }

        public bool BrokenFrother
        {
            get => this._BrokenFrother;
            set => this._BrokenFrother = value;
        }

        public bool BrokenBeanLid
        {
            get => this._BrokenBeanLid;
            set => this._BrokenBeanLid = value;
        }

        public bool BrokenWaterLid
        {
            get => this._BrokenWaterLid;
            set => this._BrokenWaterLid = value;
        }

        public int RepairFaultID
        {
            get => this._RepairFaultID;
            set => this._RepairFaultID = value;
        }

        public string RepairFaultDesc
        {
            get => this._RepairFaultDesc;
            set => this._RepairFaultDesc = value;
        }

        public int RepairStatusID
        {
            get => this._RepairStatusID;
            set => this._RepairStatusID = value;
        }

        public int RelatedOrderID
        {
            get => this._RelatedOrderID;
            set => this._RelatedOrderID = value;
        }

        public string Notes
        {
            get => this._Notes;
            set => this._Notes = value;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<RepairsTbl> GetAll(string SortBy)
        {
            return this.GetAllRepairs(SortBy, "SELECT RepairID, CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, Notes FROM RepairsTbl", (List<DBParameter>)null);
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RepairsTbl> GetAllNotDone(string SortBy)
        {
            return this.GetAllRepairs(SortBy, "SELECT RepairID, CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, RepairsTbl.Notes FROM RepairsTbl WHERE (RepairsTbl.RepairStatusID <> 7)", (List<DBParameter>)null);
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RepairsTbl> GetAllRepairsOfStatus(string SortBy, string pRepairStatus)
        {
            int result;
            bool flag = !int.TryParse(pRepairStatus, out result);
            if (pRepairStatus.Equals("OPEN") || flag)
                return this.GetAllRepairs(SortBy, "SELECT RepairID, CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, RepairsTbl.Notes FROM RepairsTbl WHERE (RepairsTbl.RepairStatusID <> 7)", (List<DBParameter>)null);
            return this.GetAllRepairs(SortBy, "SELECT RepairID, CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, RepairsTbl.Notes FROM RepairsTbl WHERE (RepairsTbl.RepairStatusID = ?)", new List<DBParameter>()
    {
      new DBParameter()
      {
        DataValue = (object) result,
        DataDbType = DbType.Int32
      }
    });
        }

        private List<RepairsTbl> GetAllRepairs(
          string SortBy,
          string pSQL,
          List<DBParameter> pWhereParams)
        {
            List<RepairsTbl> allRepairs = new List<RepairsTbl>();
            TrackerDb trackerDb = new TrackerDb();
            pSQL = $"{pSQL} ORDER BY {(string.IsNullOrEmpty(SortBy) ? "DateLogged DESC" : SortBy)}";
            if (pWhereParams != null)
                trackerDb.WhereParams = pWhereParams;
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(pSQL);
            if (dataReader != null)
            {
                while (dataReader.Read())
                    allRepairs.Add(new RepairsTbl()
                    {
                        RepairID = dataReader["RepairID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RepairID"]),
                        CustomerID = dataReader["CustomerID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["CustomerID"]),
                        ContactName = dataReader["ContactName"] == DBNull.Value ? string.Empty : dataReader["ContactName"].ToString(),
                        ContactEmail = dataReader["ContactEmail"] == DBNull.Value ? string.Empty : dataReader["ContactEmail"].ToString(),
                        JobCardNumber = dataReader["JobCardNumber"] == DBNull.Value ? string.Empty : dataReader["JobCardNumber"].ToString(),
                        DateLogged = dataReader["DateLogged"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["DateLogged"]).Date,
                        LastStatusChange = dataReader["LastStatusChange"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["LastStatusChange"]).Date,
                        MachineTypeID = dataReader["MachineTypeID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["MachineTypeID"]),
                        MachineSerialNumber = dataReader["MachineSerialNumber"] == DBNull.Value ? string.Empty : dataReader["MachineSerialNumber"].ToString(),
                        SwopOutMachineID = dataReader["SwopOutMachineID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["SwopOutMachineID"]),
                        MachineConditionID = dataReader["MachineConditionID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["MachineConditionID"]),
                        TakenFrother = dataReader["TakenFrother"] != DBNull.Value && Convert.ToBoolean(dataReader["TakenFrother"]),
                        TakenBeanLid = dataReader["TakenBeanLid"] != DBNull.Value && Convert.ToBoolean(dataReader["TakenBeanLid"]),
                        TakenWaterLid = dataReader["TakenWaterLid"] != DBNull.Value && Convert.ToBoolean(dataReader["TakenWaterLid"]),
                        BrokenFrother = dataReader["BrokenFrother"] != DBNull.Value && Convert.ToBoolean(dataReader["BrokenFrother"]),
                        BrokenBeanLid = dataReader["BrokenBeanLid"] != DBNull.Value && Convert.ToBoolean(dataReader["BrokenBeanLid"]),
                        BrokenWaterLid = dataReader["BrokenWaterLid"] != DBNull.Value && Convert.ToBoolean(dataReader["BrokenWaterLid"]),
                        RepairFaultID = dataReader["RepairFaultID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RepairFaultID"]),
                        RepairFaultDesc = dataReader["RepairFaultDesc"] == DBNull.Value ? string.Empty : dataReader["RepairFaultDesc"].ToString(),
                        RepairStatusID = dataReader["RepairStatusID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RepairStatusID"]),
                        RelatedOrderID = dataReader["RelatedOrderID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RelatedOrderID"]),
                        Notes = dataReader["Notes"] == DBNull.Value ? string.Empty : dataReader["Notes"].ToString()
                    });
                dataReader.Close();
            }
            trackerDb.Close();
            return allRepairs;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public RepairsTbl GetRepairById(int pRepairID)
        {
            RepairsTbl repairById = (RepairsTbl)null;
            if (pRepairID > 0)
            {
                TrackerDb trackerDb = new TrackerDb();
                trackerDb.AddWhereParams((object)pRepairID, DbType.Int32);
                IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader("SELECT CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, Notes FROM RepairsTbl WHERE (RepairID = ?)");
                if (dataReader != null)
                {
                    if (dataReader.Read())
                    {
                        repairById = new RepairsTbl();
                        repairById.RepairID = pRepairID;
                        repairById.CustomerID = dataReader["CustomerID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["CustomerID"]);
                        repairById.ContactName = dataReader["ContactName"] == DBNull.Value ? string.Empty : dataReader["ContactName"].ToString();
                        repairById.ContactEmail = dataReader["ContactEmail"] == DBNull.Value ? string.Empty : dataReader["ContactEmail"].ToString();
                        repairById.JobCardNumber = dataReader["JobCardNumber"] == DBNull.Value ? string.Empty : dataReader["JobCardNumber"].ToString();
                        repairById.DateLogged = dataReader["DateLogged"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["DateLogged"]).Date;
                        repairById.LastStatusChange = dataReader["LastStatusChange"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["LastStatusChange"]).Date;
                        repairById.MachineTypeID = dataReader["MachineTypeID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["MachineTypeID"]);
                        repairById.MachineSerialNumber = dataReader["MachineSerialNumber"] == DBNull.Value ? string.Empty : dataReader["MachineSerialNumber"].ToString();
                        repairById.SwopOutMachineID = dataReader["SwopOutMachineID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["SwopOutMachineID"]);
                        repairById.MachineConditionID = dataReader["MachineConditionID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["MachineConditionID"]);
                        repairById.TakenFrother = dataReader["TakenFrother"] != DBNull.Value && Convert.ToBoolean(dataReader["TakenFrother"]);
                        repairById.TakenBeanLid = dataReader["TakenBeanLid"] != DBNull.Value && Convert.ToBoolean(dataReader["TakenBeanLid"]);
                        repairById.TakenWaterLid = dataReader["TakenWaterLid"] != DBNull.Value && Convert.ToBoolean(dataReader["TakenWaterLid"]);
                        repairById.BrokenFrother = dataReader["BrokenFrother"] != DBNull.Value && Convert.ToBoolean(dataReader["BrokenFrother"]);
                        repairById.BrokenBeanLid = dataReader["BrokenBeanLid"] != DBNull.Value && Convert.ToBoolean(dataReader["BrokenBeanLid"]);
                        repairById.BrokenWaterLid = dataReader["BrokenWaterLid"] != DBNull.Value && Convert.ToBoolean(dataReader["BrokenWaterLid"]);
                        repairById.RepairFaultID = dataReader["RepairFaultID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RepairFaultID"]);
                        repairById.RepairFaultDesc = dataReader["RepairFaultDesc"] == DBNull.Value ? string.Empty : dataReader["RepairFaultDesc"].ToString();
                        repairById.RepairStatusID = dataReader["RepairStatusID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RepairStatusID"]);
                        repairById.RelatedOrderID = dataReader["RelatedOrderID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RelatedOrderID"]);
                        repairById.Notes = dataReader["Notes"] == DBNull.Value ? string.Empty : dataReader["Notes"].ToString();
                    }
                    dataReader.Close();
                }
                trackerDb.Close();
            }
            return repairById;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public bool InsertRepair(RepairsTbl DataItem)
        {
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddParams((object)DataItem.CustomerID, DbType.Int64);
            trackerDb.AddParams((object)DataItem.ContactName, DbType.String);
            trackerDb.AddParams((object)DataItem.ContactEmail, DbType.String);
            trackerDb.AddParams((object)DataItem.JobCardNumber, DbType.String);
            trackerDb.AddParams((object)DataItem.DateLogged, DbType.Date);
            trackerDb.AddParams((object)DataItem.LastStatusChange, DbType.Date);
            trackerDb.AddParams((object)DataItem.MachineTypeID, DbType.Int32);
            trackerDb.AddParams((object)DataItem.MachineSerialNumber, DbType.String);
            trackerDb.AddParams((object)DataItem.SwopOutMachineID, DbType.Int32);
            trackerDb.AddParams((object)DataItem.MachineConditionID, DbType.Int32);
            trackerDb.AddParams((object)DataItem.TakenFrother, DbType.Boolean);
            trackerDb.AddParams((object)DataItem.TakenBeanLid, DbType.Boolean);
            trackerDb.AddParams((object)DataItem.TakenWaterLid, DbType.Boolean);
            trackerDb.AddParams((object)DataItem.BrokenFrother, DbType.Boolean);
            trackerDb.AddParams((object)DataItem.BrokenBeanLid, DbType.Boolean);
            trackerDb.AddParams((object)DataItem.BrokenWaterLid, DbType.Boolean);
            trackerDb.AddParams((object)DataItem.RepairFaultID, DbType.Int32);
            trackerDb.AddParams((object)DataItem.RepairFaultDesc, DbType.String);
            trackerDb.AddParams((object)DataItem.RepairStatusID, DbType.Int32);
            trackerDb.AddParams((object)DataItem.RelatedOrderID, DbType.Int64);
            trackerDb.AddParams((object)DataItem.Notes, DbType.String);
            bool flag = string.IsNullOrEmpty(trackerDb.ExecuteNonQuerySQL("INSERT INTO RepairsTbl (CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairStatusID, RelatedOrderID, Notes)VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"));
            trackerDb.Close();
            return flag;
        }

        public int GetLastIDInserted(long pCustomerID)
        {
            int lastIdInserted = 0;
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddWhereParams((object)pCustomerID, DbType.Int64);
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader("SELECT TOP 1 RepairID FROM RepairsTbl WHERE (CustomerID = ?) ORDER BY DateLogged DESC");
            if (dataReader != null)
            {
                if (dataReader.Read())
                    lastIdInserted = dataReader["RepairID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RepairID"]);
                else
                    dataReader.Close();
            }
            trackerDb.Close();
            return lastIdInserted;
        }

        [DataObjectMethod(DataObjectMethodType.Update, true)]
        public string UpdateRepair(RepairsTbl RepairItem)
        {
            return this.UpdateRepair(RepairItem, RepairItem.RepairID);
        }

        public string UpdateRepair(RepairsTbl RepairItem, int orig_RepairID)
        {
            string empty = string.Empty;
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddParams((object)RepairItem.CustomerID, DbType.Int64);
            trackerDb.AddParams((object)RepairItem.ContactName, DbType.String);
            trackerDb.AddParams((object)RepairItem.ContactEmail, DbType.String);
            trackerDb.AddParams((object)RepairItem.JobCardNumber, DbType.String);
            trackerDb.AddParams((object)RepairItem.DateLogged, DbType.Date);
            trackerDb.AddParams((object)RepairItem.LastStatusChange, DbType.Date);
            trackerDb.AddParams((object)RepairItem.MachineTypeID, DbType.Int32);
            trackerDb.AddParams((object)RepairItem.MachineSerialNumber, DbType.String);
            trackerDb.AddParams((object)RepairItem.SwopOutMachineID, DbType.Int32);
            trackerDb.AddParams((object)RepairItem.MachineConditionID, DbType.Int32);
            trackerDb.AddParams((object)RepairItem.TakenFrother, DbType.Boolean);
            trackerDb.AddParams((object)RepairItem.TakenBeanLid, DbType.Boolean);
            trackerDb.AddParams((object)RepairItem.TakenWaterLid, DbType.Boolean);
            trackerDb.AddParams((object)RepairItem.BrokenFrother, DbType.Boolean);
            trackerDb.AddParams((object)RepairItem.BrokenBeanLid, DbType.Boolean);
            trackerDb.AddParams((object)RepairItem.BrokenWaterLid, DbType.Boolean);
            trackerDb.AddParams((object)RepairItem.RepairFaultID, DbType.Int32);
            trackerDb.AddParams((object)RepairItem.RepairFaultDesc, DbType.String);
            trackerDb.AddParams((object)RepairItem.RepairStatusID, DbType.Int32);
            trackerDb.AddParams((object)RepairItem.RelatedOrderID, DbType.Int64);
            trackerDb.AddParams((object)RepairItem.Notes, DbType.String);
            trackerDb.AddWhereParams((object)orig_RepairID, DbType.Int32);
            string str = trackerDb.ExecuteNonQuerySQL("UPDATE RepairsTbl SET CustomerID = ?, ContactName = ?, ContactEmail = ?, JobCardNumber = ?, DateLogged = ?, LastStatusChange = ?, MachineTypeID = ?, MachineSerialNumber = ?, SwopOutMachineID = ?,  MachineConditionID = ?, TakenFrother = ?, TakenBeanLid = ?, TakenWaterLid = ?, BrokenFrother = ?,  BrokenBeanLid = ?, BrokenWaterLid = ?, RepairFaultID = ?, RepairFaultDesc = ?, RepairStatusID = ?,  RelatedOrderID = ?, Notes = ? WHERE (RepairsTbl.RepairID = ?)");
            trackerDb.Close();
            return str;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public string DeleteRepair(int RepairID)
        {
            string empty = string.Empty;
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.AddParams((object)RepairID, DbType.Int32);
            return trackerDb.ExecuteNonQuerySQL("DELETE FROM RepairsTbl WHERE (RepairsTbl.RepairID = ?)");
        }

        private bool SendStatusChangeEmail(RepairsTbl pRepair)
        {
            EmailCls emailCls = new EmailCls();
            EquipTypeTbl equipTypeTbl = new EquipTypeTbl();
            RepairStatusesTbl repairStatusesTbl = new RepairStatusesTbl();
            emailCls.SetEmailTo(pRepair.ContactEmail);
            emailCls.SetEmailSubject("Change to repair status - notification.");
            emailCls.AddStrAndNewLineToBody("<b>Repair Status Change Notification</b>");
            emailCls.AddFormatToBody("Dear {0}, <br /><br />", (object)pRepair.ContactName);
            emailCls.AddFormatToBody("Please note that repair status of the {0}, serial number: {1}, has changed to <b>{2}</b>.<br /><br />", (object)equipTypeTbl.GetEquipName(pRepair._MachineTypeID), (object)pRepair.MachineSerialNumber, (object)repairStatusesTbl.GetRepairStatusDesc(pRepair.RepairStatusID));
            emailCls.AddStrAndNewLineToBody("You are receiving this status update since your email address was assigned to the tracking of this repair.<br />");
            emailCls.AddStrAndNewLineToBody("Please note that Quaffee DOES NOT run a coffee machine repair workshop. As a service to our current coffee purchasing clients we will take the coffee machine to the workshop, give a swop out machine to the client, if available, and charge what we are charged by the workshop plus a small admin fee. Please note: we are not able to quote these repairs, if you would like to contact the workshop directly, please ask us for their details.<br />");
            emailCls.AddStrAndNewLineToBody("For clients that are not currently using our coffee, or have not used our coffee consistently over the last 3 months, we may at our discretion offer a swop out machine, at a fee, and also change a collection and delivery fee.<br />");
            emailCls.AddStrAndNewLineToBody("Any warantee on the repairs is carried by the workshop not by Quaffee.<br />");
            emailCls.AddStrAndNewLineToBody("The Quaffee Team");
            emailCls.AddStrAndNewLineToBody("web: <a href='http://www.quaffee.co.za'>quaffee.co.za</a>");
            return emailCls.SendEmail();
        }

        private bool LogARepair(RepairsTbl pRepair, bool pCalcOrderData)
        {
            bool flag = false;
            OrderTblData pOrderData = new OrderTblData();
            DateTime pDelivery = DateTime.Now.Date.AddDays(7.0);
            pOrderData.CustomerID = pRepair.CustomerID;
            pOrderData.ItemTypeID = 36;
            pOrderData.QuantityOrdered = 1.0;
            pOrderData.Notes = "Collect/Swop out Machine for Service";
            if (pCalcOrderData)
            {
                TrackerTools trackerTools = new TrackerTools();
                pOrderData.RoastDate = trackerTools.GetNextRoastDateByCustomerID(pRepair.CustomerID, ref pDelivery);
                TrackerTools.ContactPreferedItems contactPreferedItems = trackerTools.RetrieveCustomerPrefs(pRepair.CustomerID);
                pOrderData.OrderDate = DateTime.Now.Date;
                pOrderData.RequiredByDate = pDelivery;
                if (contactPreferedItems.RequiresPurchOrder)
                    pOrderData.PurchaseOrder = "!!!PO required!!!";
                pOrderData.ToBeDeliveredBy = contactPreferedItems.PreferredDeliveryByID;
            }
            else
            {
                OrderTblData orderTblData1 = pOrderData;
                OrderTblData orderTblData2 = pOrderData;
                DateTime now = DateTime.Now;
                DateTime date;
                DateTime dateTime1 = date = now.Date;
                orderTblData2.OrderDate = date;
                DateTime dateTime2 = dateTime1;
                orderTblData1.RoastDate = dateTime2;
                pOrderData.RequiredByDate = pDelivery;
            }
            OrderTbl orderTbl = new OrderTbl();
            orderTbl.InsertNewOrderLine(pOrderData);
            pRepair.RelatedOrderID = orderTbl.GetLastOrderAdded(pOrderData.CustomerID, pOrderData.OrderDate, 36);
            return flag;
        }

        public bool HandleAndUpdateRepairStatusChange(RepairsTbl pRepair)
        {
            bool flag = true;
            switch (pRepair.RepairStatusID)
            {
                case 1:
                    flag = this.LogARepair(pRepair, true);
                    break;
                case 2:
                    if (pRepair.RelatedOrderID.Equals(0L))
                    {
                        flag = this.LogARepair(pRepair, true);
                        break;
                    }
                    new OrderTbl().UpdateIncDeliveryDateBy7(pRepair.RelatedOrderID);
                    break;
                case 3:
                    if (pRepair.RelatedOrderID.Equals(0L))
                        flag = this.LogARepair(pRepair, false);
                    else
                        new OrderTbl().UpdateIncDeliveryDateBy7(pRepair.RelatedOrderID);
                    if (!string.IsNullOrEmpty(pRepair.MachineSerialNumber))
                    {
                        new CustomersTbl().SetEquipDetailsIfEmpty(pRepair.MachineTypeID, pRepair.MachineSerialNumber, pRepair.CustomerID);
                        break;
                    }
                    break;
                case 6:
                    if (pRepair.RelatedOrderID > 0L)
                    {
                        new OrderTbl().UpdateOrderDeliveryDate(new NextRoastDateByCityTbl().GetNextDeliveryDate(pRepair.CustomerID), pRepair.RelatedOrderID);
                        break;
                    }
                    break;
                case 7:
                    if (pRepair.RelatedOrderID > 0L)
                    {
                        new OrderTbl().UpdateSetDoneByID(true, pRepair.RelatedOrderID);
                        break;
                    }
                    break;
            }
            return string.IsNullOrEmpty(this.UpdateRepair(pRepair)) && this.SendStatusChangeEmail(pRepair);
        }

        public List<RepairsTbl> GetListOfRelatedTempOrders()
        {
            List<RepairsTbl> relatedTempOrders = new List<RepairsTbl>();
            TrackerDb trackerDb = new TrackerDb();
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader("SELECT  RepairID, CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, Notes FROM (RepairsTbl INNER JOIN TempOrdersLinesTbl ON RepairsTbl.RelatedOrderID = TempOrdersLinesTbl.OriginalOrderID)");
            if (dataReader != null)
            {
                while (dataReader.Read())
                    relatedTempOrders.Add(new RepairsTbl()
                    {
                        RepairID = dataReader["RepairID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RepairID"]),
                        CustomerID = dataReader["CustomerID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["CustomerID"]),
                        ContactName = dataReader["ContactName"] == DBNull.Value ? string.Empty : dataReader["ContactName"].ToString(),
                        ContactEmail = dataReader["ContactEmail"] == DBNull.Value ? string.Empty : dataReader["ContactEmail"].ToString(),
                        JobCardNumber = dataReader["JobCardNumber"] == DBNull.Value ? string.Empty : dataReader["JobCardNumber"].ToString(),
                        DateLogged = dataReader["DateLogged"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["DateLogged"]).Date,
                        LastStatusChange = dataReader["LastStatusChange"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["LastStatusChange"]).Date,
                        MachineTypeID = dataReader["MachineTypeID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["MachineTypeID"]),
                        MachineSerialNumber = dataReader["MachineSerialNumber"] == DBNull.Value ? string.Empty : dataReader["MachineSerialNumber"].ToString(),
                        SwopOutMachineID = dataReader["SwopOutMachineID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["SwopOutMachineID"]),
                        MachineConditionID = dataReader["MachineConditionID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["MachineConditionID"]),
                        TakenFrother = dataReader["TakenFrother"] != DBNull.Value && Convert.ToBoolean(dataReader["TakenFrother"]),
                        TakenBeanLid = dataReader["TakenBeanLid"] != DBNull.Value && Convert.ToBoolean(dataReader["TakenBeanLid"]),
                        TakenWaterLid = dataReader["TakenWaterLid"] != DBNull.Value && Convert.ToBoolean(dataReader["TakenWaterLid"]),
                        BrokenFrother = dataReader["BrokenFrother"] != DBNull.Value && Convert.ToBoolean(dataReader["BrokenFrother"]),
                        BrokenBeanLid = dataReader["BrokenBeanLid"] != DBNull.Value && Convert.ToBoolean(dataReader["BrokenBeanLid"]),
                        BrokenWaterLid = dataReader["BrokenWaterLid"] != DBNull.Value && Convert.ToBoolean(dataReader["BrokenWaterLid"]),
                        RepairFaultID = dataReader["RepairFaultID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RepairFaultID"]),
                        RepairFaultDesc = dataReader["RepairFaultDesc"] == DBNull.Value ? string.Empty : dataReader["RepairFaultDesc"].ToString(),
                        RepairStatusID = dataReader["RepairStatusID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RepairStatusID"]),
                        RelatedOrderID = dataReader["RelatedOrderID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["RelatedOrderID"]),
                        Notes = dataReader["Notes"] == DBNull.Value ? string.Empty : dataReader["Notes"].ToString()
                    });
                dataReader.Close();
            }
            trackerDb.Close();
            return relatedTempOrders;
        }

        public string SetStatusDoneByTempOrder()
        {
            string empty = string.Empty;
            List<RepairsTbl> relatedTempOrders = this.GetListOfRelatedTempOrders();
            if (relatedTempOrders.Count > 0)
            {
                TempOrdersLinesTbl tempOrdersLinesTbl = new TempOrdersLinesTbl();
                for (int index = 0; index < relatedTempOrders.Count; ++index)
                {
                    if (relatedTempOrders[index].RepairStatusID <= 3)
                    {
                        tempOrdersLinesTbl.DeleteByOriginalID(relatedTempOrders[index].RelatedOrderID);
                        new OrderTbl().UpdateIncDeliveryDateBy7(relatedTempOrders[index].RelatedOrderID);
                    }
                    else
                    {
                        TrackerDb trackerDb = new TrackerDb();
                        trackerDb.AddParams((object)7, DbType.Int32);
                        empty += trackerDb.ExecuteNonQuerySQL("UPDATE RepairsTbl SET RepairStatusID = ? WHERE (RelatedOrderID = ?)");
                        trackerDb.Close();
                    }
                }
            }
            return empty;
        }
    }
}
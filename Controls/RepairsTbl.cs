// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.control.RepairsTbl
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using TrackerDotNet.Classes;

namespace TrackerDotNet.Controls
{
    public class RepairsTbl
    {
        // Status constants moved to RepairStatus enum in Classes/Enums
        private const string CONST_REPAIR_DONESTR = "7";

        // SQL queries
        private const string CONST_SQL_SELECT = "SELECT RepairID, CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, Notes FROM RepairsTbl";
        private const string CONST_SQL_SELECTNOTDONE = CONST_SQL_SELECT + " WHERE (RepairsTbl.RepairStatusID <> "+ CONST_REPAIR_DONESTR +")";
        private const string CONST_SQL_SELECTONSTATUS = CONST_SQL_SELECT + "WHERE (RepairsTbl.RepairStatusID = ?)";
        private const string CONST_SQL_SELECTBYREPAIRID = "SELECT CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, Notes FROM RepairsTbl WHERE (RepairID = ?)";
        private const string CONST_SQL_SELECTLAST = "SELECT TOP 1 RepairID FROM RepairsTbl WHERE (CustomerID = ?) ORDER BY DateLogged DESC";
        private const string CONST_SQL_INSERT = "INSERT INTO RepairsTbl (CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairStatusID, RelatedOrderID, Notes)VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private const string CONST_SQL_UPDATE = "UPDATE RepairsTbl SET CustomerID = ?, ContactName = ?, ContactEmail = ?, JobCardNumber = ?, DateLogged = ?, LastStatusChange = ?, MachineTypeID = ?, MachineSerialNumber = ?, SwopOutMachineID = ?,  MachineConditionID = ?, TakenFrother = ?, TakenBeanLid = ?, TakenWaterLid = ?, BrokenFrother = ?,  BrokenBeanLid = ?, BrokenWaterLid = ?, RepairFaultID = ?, RepairFaultDesc = ?, RepairStatusID = ?,  RelatedOrderID = ?, Notes = ? WHERE (RepairsTbl.RepairID = ?)";
        private const string CONST_SQL_DELETE = "DELETE FROM RepairsTbl WHERE (RepairsTbl.RepairID = ?)";
        private const string CONST_SQL_SELECTITEMWITHANORDER = "SELECT RepairID, CustomerID, ContactName, ContactEmail, JobCardNumber, DateLogged, LastStatusChange, MachineTypeID, MachineSerialNumber, SwopOutMachineID, MachineConditionID, TakenFrother, TakenBeanLid, TakenWaterLid, BrokenFrother, BrokenBeanLid, BrokenWaterLid, RepairFaultID, RepairFaultDesc, RepairsTbl.RepairStatusID, RelatedOrderID, Notes FROM RepairsTbl WHERE RelatedOrderID > 0";

        // Properties
        public int RepairID { get; set; }
        public long CustomerID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string JobCardNumber { get; set; } = string.Empty;
        public DateTime DateLogged { get; set; } = DateTime.MinValue;
        public DateTime LastStatusChange { get; set; } = TimeZoneUtils.Now();
        public int MachineTypeID { get; set; }
        public string MachineSerialNumber { get; set; } = string.Empty;
        public int SwopOutMachineID { get; set; }
        public int MachineConditionID { get; set; }
        public bool TakenFrother { get; set; }
        public bool TakenBeanLid { get; set; } = true;
        public bool TakenWaterLid { get; set; } = true;
        public bool BrokenFrother { get; set; }
        public bool BrokenBeanLid { get; set; }
        public bool BrokenWaterLid { get; set; }
        public int RepairFaultID { get; set; }
        public string RepairFaultDesc { get; set; } = string.Empty;
        public int RepairStatusID { get; set; }
        public int RelatedOrderID { get; set; }
        public string Notes { get; set; } = string.Empty;

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<RepairsTbl> GetAll(string sortBy)
        {
            return GetAllRepairs(sortBy, CONST_SQL_SELECT, null);
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RepairsTbl> GetAllNotDone(string sortBy)
        {
            return GetAllRepairs(sortBy, CONST_SQL_SELECTNOTDONE, null);
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<RepairsTbl> GetAllRepairsOfStatus(string repairStatus)
        {
            return GetAllRepairsOfStatus("DateLogged DESC", repairStatus);
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RepairsTbl> GetAllRepairsOfStatus(string sortBy, string repairStatus)
        {
            if (!int.TryParse(repairStatus, out int statusId) || repairStatus.Equals("OPEN"))
                return GetAllNotDone(sortBy);

            var parameters = new List<DBParameter>
    {
        new DBParameter { DataValue = statusId, DataDbType = DbType.Int32 }
    };

            return GetAllRepairs(sortBy, CONST_SQL_SELECTONSTATUS, parameters);
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public RepairsTbl GetRepairById(int repairId)
        {
            RepairsTbl repairById = null;
            if (repairId <= 0) return null;

            TrackerDb trackerDb = new TrackerDb();
            using (trackerDb)
            {
                trackerDb.AddWhereParams(repairId, DbType.Int32);
                IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(CONST_SQL_SELECTBYREPAIRID);
                if (dataReader != null)
                {
                    if (dataReader.Read())
                    {
                        repairById = MapDataToRepair(dataReader, repairId);
                    }
                    dataReader.Close();
                }
                trackerDb.Close();
            }
            return repairById;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public bool InsertRepair(RepairsTbl repair)
        {
            TrackerDb db = new TrackerDb();
            using (db)
            {
                AddRepairParameters(db, repair);
                string result = db.ExecuteNonQuerySQL(CONST_SQL_INSERT);
                return string.IsNullOrEmpty(result);
            }
        }

        [DataObjectMethod(DataObjectMethodType.Update, true)]
        public string UpdateRepair(RepairsTbl repair)
        {
            return UpdateRepair(repair, repair.RepairID);
        }

        public string UpdateRepair(RepairsTbl repair, int origRepairId)
        {
            TrackerDb db = new TrackerDb();
            using (db)
            {
                AddRepairParameters(db, repair);
                db.AddWhereParams(origRepairId, DbType.Int32);
                return db.ExecuteNonQuerySQL(CONST_SQL_UPDATE);
            }
        }

        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public string DeleteRepair(int repairId)
        {
            TrackerDb db = new TrackerDb();
            using (db)
            {
                db.AddParams(repairId, DbType.Int32);
                return db.ExecuteNonQuerySQL(CONST_SQL_DELETE);
            }
        }

        public int GetLastIDInserted(long customerId)
        {
            TrackerDb db = new TrackerDb();
            using (db)
            {
                db.AddWhereParams(customerId, DbType.Int64);
                IDataReader reader = db.ExecuteSQLGetDataReader(CONST_SQL_SELECTLAST);
                if (reader != null)
                {
                    using (reader)
                    {
                        if (reader.Read())
                        {
                            return reader["RepairID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RepairID"]);
                        }
                    }
                }
                return 0;
            }
        }

        private List<RepairsTbl> GetAllRepairs(string sortBy, string sql, List<DBParameter> whereParams)
        {
            List<RepairsTbl> repairs = new List<RepairsTbl>();
            TrackerDb db = new TrackerDb();
            using (db)
            {
                sql = sql + " ORDER BY " + (string.IsNullOrEmpty(sortBy) ? "DateLogged DESC" : sortBy);
                
                if (whereParams != null)
                {
                    db.WhereParams = whereParams;
                }

                IDataReader reader = db.ExecuteSQLGetDataReader(sql);
                if (reader != null)
                {
                    using (reader)
                    {
                        while (reader.Read())
                        {
                            repairs.Add(MapDataToRepair(reader));
                        }
                    }
                }
            }
            return repairs;
        }

        private RepairsTbl MapDataToRepair(IDataReader reader, int? repairId = null)
        {
            return new RepairsTbl
            {
                RepairID = repairId ?? (reader["RepairID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RepairID"])),
                CustomerID = reader["CustomerID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CustomerID"]),
                ContactName = reader["ContactName"]?.ToString() ?? string.Empty,
                ContactEmail = reader["ContactEmail"]?.ToString() ?? string.Empty,
                JobCardNumber = reader["JobCardNumber"]?.ToString() ?? string.Empty,
                DateLogged = reader["DateLogged"] == DBNull.Value ? TimeZoneUtils.Now().Date : Convert.ToDateTime(reader["DateLogged"]).Date,
                LastStatusChange = reader["LastStatusChange"] == DBNull.Value ? TimeZoneUtils.Now().Date : Convert.ToDateTime(reader["LastStatusChange"]).Date,
                MachineTypeID = reader["MachineTypeID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MachineTypeID"]),
                MachineSerialNumber = reader["MachineSerialNumber"]?.ToString() ?? string.Empty,
                SwopOutMachineID = reader["SwopOutMachineID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SwopOutMachineID"]),
                MachineConditionID = reader["MachineConditionID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MachineConditionID"]),
                TakenFrother = reader["TakenFrother"] != DBNull.Value && Convert.ToBoolean(reader["TakenFrother"]),
                TakenBeanLid = reader["TakenBeanLid"] != DBNull.Value && Convert.ToBoolean(reader["TakenBeanLid"]),
                TakenWaterLid = reader["TakenWaterLid"] != DBNull.Value && Convert.ToBoolean(reader["TakenWaterLid"]),
                BrokenFrother = reader["BrokenFrother"] != DBNull.Value && Convert.ToBoolean(reader["BrokenFrother"]),
                BrokenBeanLid = reader["BrokenBeanLid"] != DBNull.Value && Convert.ToBoolean(reader["BrokenBeanLid"]),
                BrokenWaterLid = reader["BrokenWaterLid"] != DBNull.Value && Convert.ToBoolean(reader["BrokenWaterLid"]),
                RepairFaultID = reader["RepairFaultID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RepairFaultID"]),
                RepairFaultDesc = reader["RepairFaultDesc"]?.ToString() ?? string.Empty,
                RepairStatusID = reader["RepairStatusID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RepairStatusID"]),
                RelatedOrderID = reader["RelatedOrderID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RelatedOrderID"]),
                Notes = reader["Notes"]?.ToString() ?? string.Empty
            };
        }

        private void AddRepairParameters(TrackerDb db, RepairsTbl repair)
        {
            db.AddParams(repair.CustomerID, DbType.Int64);
            db.AddParams(repair.ContactName, DbType.String);
            db.AddParams(repair.ContactEmail, DbType.String);
            db.AddParams(repair.JobCardNumber, DbType.String);
            db.AddParams(repair.DateLogged, DbType.Date);
            db.AddParams(repair.LastStatusChange, DbType.Date);
            db.AddParams(repair.MachineTypeID, DbType.Int32);
            db.AddParams(repair.MachineSerialNumber, DbType.String);
            db.AddParams(repair.SwopOutMachineID, DbType.Int32);
            db.AddParams(repair.MachineConditionID, DbType.Int32);
            db.AddParams(repair.TakenFrother, DbType.Boolean);
            db.AddParams(repair.TakenBeanLid, DbType.Boolean);
            db.AddParams(repair.TakenWaterLid, DbType.Boolean);
            db.AddParams(repair.BrokenFrother, DbType.Boolean);
            db.AddParams(repair.BrokenBeanLid, DbType.Boolean);
            db.AddParams(repair.BrokenWaterLid, DbType.Boolean);
            db.AddParams(repair.RepairFaultID, DbType.Int32);
            db.AddParams(repair.RepairFaultDesc, DbType.String);
            db.AddParams(repair.RepairStatusID, DbType.Int32);
            db.AddParams(repair.RelatedOrderID, DbType.Int64);
            db.AddParams(repair.Notes, DbType.String);
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<RepairsTbl> GetListOfRelatedTempOrders()
        {
            return GetAllRepairs(null, CONST_SQL_SELECTITEMWITHANORDER, null);
        }
    }
}
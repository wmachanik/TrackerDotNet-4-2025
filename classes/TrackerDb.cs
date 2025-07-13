// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.classes.TrackerDb
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;

//- only form later versions #nullable disable
namespace TrackerDotNet.classes
{
    public class TrackerDb : IDisposable
    {
        public const string CONST_CONSTRING = "Tracker08ConnectionString";
        public const int CONST_INVALIDID = -1;
        public const string CONST_INVALIDIDSTR = "-1";
        public const string SQLTABLENAME_LOGTBL = "LogTbl";
        public const string SQLTABLENAME_SECTIONTYPESTBL = "SectionTypesTbl";
        public const string SQLTABLENAME_TRANSACTIONTYPESTBL = "TransactionTypesTbl";
        private const string CONST_SQL_CREATE_LOGTBL = "CREATE TABLE LogTbl ( [LogID] AUTOINCREMENT, [DateAdded] DateTime, [UserID] INT, [SectionID] INT, [TranactionTypeID] INT,  [CustomerID] INT, Details VARCHAR(255), [Notes] MEMO,  CONSTRAINT [pk_LogID] PRIMARY KEY (LogID) )";
        private const string CONST_SQL_CREATE_SECTIONTYPETBL = "CREATE TABLE SectionTypesTbl ( [SectionID] INT, [SectionType] VARCHAR(50), [Notes] MEMO,  CONSTRAINT [pk_SectionID] PRIMARY KEY (SectionID) )";
        private const string CONST_SQL_CREATE_TRANSACTIONTYPETBL = "CREATE TABLE TransactionTypesTbl ( [TransactionID] INT, [TransactionType] VARCHAR(50), [Notes] MEMO,  CONSTRAINT [pk_TransactionID] PRIMARY KEY (TransactionID) )";
        private List<DBParameter> _Params;
        private List<DBParameter> _WhereParams;
        private OleDbConnection _TrackerDbConn;
        private int _numRecs;
        private OleDbCommand _command;

        public TrackerDb()
        {
            this._TrackerDbConn = (OleDbConnection)null;
            this._command = (OleDbCommand)null;
            this.Initialize();
            this._Params = new List<DBParameter>();
            this._WhereParams = new List<DBParameter>();
            this._numRecs = 0;
        }

        public List<DBParameter> Params
        {
            get => this._Params;
            set => this._Params = value;
        }

        public List<DBParameter> WhereParams
        {
            get => this._WhereParams;
            set => this._WhereParams = value;
        }

        public OleDbConnection TrackerDbConn
        {
            get => this._TrackerDbConn;
            set => this._TrackerDbConn = value;
        }

        public int numRecs
        {
            get => this._numRecs;
            set => this._numRecs = value;
        }

        public string ErrorResult
        {
            get => new TrackerTools().GetTrackerSessionErrorString();
            set => new TrackerTools().SetTrackerSessionErrorString(value);
        }

        private object PrepareValueForOleDb(DbType dbType, object value)
        {
            if (value == null || value == DBNull.Value)
                return DBNull.Value;

            try
            {
                switch (dbType)
                {
                    case DbType.Date:
                        return Convert.ToDateTime(value).Date;

                    case DbType.DateTime:
                        DateTime dtVal;
                        if (value is DateTime)
                            dtVal = (DateTime)value;
                        else
                            dtVal = Convert.ToDateTime(value);
                        return dtVal.ToString("MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                    case DbType.Int16:
                        return Convert.ToInt16(value);

                    case DbType.Int32:
                        return Convert.ToInt32(value);

                    case DbType.Int64:
                        long longVal = Convert.ToInt64(value);
                        if (longVal <= int.MaxValue && longVal >= int.MinValue)
                        {
                            return (int)longVal;
                        }
                        return DBNull.Value;

                    case DbType.Decimal:
                    case DbType.Currency:
                    case DbType.VarNumeric:
                        return Convert.ToDecimal(value);

                    case DbType.Double:
                        return Convert.ToDouble(value);

                    case DbType.Single:
                        return Convert.ToSingle(value);

                    case DbType.Boolean:
                        return Convert.ToBoolean(value);

                    case DbType.String:
                    case DbType.AnsiString:
                    case DbType.AnsiStringFixedLength:
                    case DbType.StringFixedLength:
                    case DbType.Xml:
                        string strVal = Convert.ToString(value);
                        if (string.IsNullOrWhiteSpace(strVal))
                            return DBNull.Value;
                        return strVal;

                    case DbType.Guid:
                        Guid guidResult;
                        if (Guid.TryParse(value.ToString(), out guidResult))
                            return guidResult;
                        return DBNull.Value;

                    case DbType.Byte:
                        return Convert.ToByte(value);

                    case DbType.Binary:
                        if (value is byte[] bytes)
                            return bytes;
                        else
                            return DBNull.Value;

                    default:
                        return value;
                }
            }
            catch
            {
                return DBNull.Value;
            }
        }
        private OleDbParameter BuildOleDbParameter(DBParameter param)
        {
            OleDbParameter oleParam = new OleDbParameter();

            oleParam.Value = PrepareValueForOleDb(param.DataDbType, param.DataValue);

            if (param.ParamName != null && !param.ParamName.Equals("?"))
            {
                oleParam.ParameterName = param.ParamName;
                oleParam.DbType = param.DataDbType;
            }
            else
            {
                oleParam.OleDbType = ConvertToOleDbType(param.DataDbType);
            }

            return oleParam;
        }
        //private object ConvertToOleDbValue(DbType dbType, object value)
        //{
        //    if (value == null || value == DBNull.Value)
        //        return DBNull.Value;

        //    try
        //    {
        //        switch (dbType)
        //        {
        //            case DbType.Date:
        //            case DbType.DateTime:
        //                if (value is DateTime dt)
        //                    return dt.Date;
        //                return Convert.ToDateTime(value).Date;

        //            case DbType.Int16:
        //                return Convert.ToInt16(value);

        //            case DbType.Int32:
        //                return Convert.ToInt32(value);

        //            case DbType.Int64:
        //                long myValue = Convert.ToInt64(value); // your logic
        //                if (myValue <= int.MaxValue && myValue >= int.MinValue)
        //                {
        //                    return (int)myValue;
        //                }
        //                else
        //                {
        //                    // Log to file, event log, or debugging trace
        //                    System.Diagnostics.Trace.WriteLine($"Conversion failed: long to int for Access database");
        //                    return -1; // DBNull.Value;
        //                }

        //            case DbType.Decimal:
        //            case DbType.Currency:
        //                return Convert.ToDecimal(value);

        //            case DbType.Double:
        //                return Convert.ToDouble(value);

        //            case DbType.Single:
        //                return Convert.ToSingle(value);

        //            case DbType.Boolean:
        //                return Convert.ToBoolean(value);

        //            case DbType.String:
        //            case DbType.AnsiString:
        //            case DbType.AnsiStringFixedLength:
        //            case DbType.StringFixedLength:
        //                string strVal = Convert.ToString(value);
        //                if (string.IsNullOrWhiteSpace(strVal))
        //                    return DBNull.Value;
        //                else
        //                    return strVal;


        //            case DbType.Guid:
        //                if (Guid.TryParse(value.ToString(), out Guid result))
        //                    return result;
        //                else
        //                    return DBNull.Value;

        //            case DbType.Byte:
        //                return Convert.ToByte(value);

        //            case DbType.Binary:
        //                if (value is byte[] bytes)
        //                    return bytes;
        //                else
        //                    return DBNull.Value;


        //            default:
        //                return value;
        //        }
        //    }
        //    catch
        //    {
        //        return DBNull.Value; // fallback to avoid crashing
        //    }
        //}

        private OleDbType ConvertToOleDbType(DbType pDbType)
        {
            switch (pDbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                    return OleDbType.Char;
                case DbType.Binary:
                    return OleDbType.Binary;
                case DbType.Byte:
                    return OleDbType.UnsignedTinyInt;
                case DbType.Boolean:
                    return OleDbType.Boolean;
                case DbType.Currency:
                    return OleDbType.Currency;
                case DbType.Date:
                    return OleDbType.Date;
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return OleDbType.Date;
                case DbType.Decimal:
                    return OleDbType.Decimal;
                case DbType.Double:
                    return OleDbType.Double;
                case DbType.Guid:
                    return OleDbType.Guid;
                case DbType.Int16:
                    return OleDbType.SmallInt;
                case DbType.Int32:
                    return OleDbType.Integer;
                case DbType.Int64:
                    return OleDbType.Integer;  // Access32-bit does nto support BigInt, but this is what it should be;
                case DbType.Object:
                    return OleDbType.IDispatch;
                case DbType.SByte:
                    return OleDbType.TinyInt;
                case DbType.Single:
                    return OleDbType.Single;
                case DbType.String:
                    return OleDbType.VarChar;
                case DbType.Time:
                    return OleDbType.DBTime;
                case DbType.UInt16:
                    return OleDbType.UnsignedSmallInt;
                case DbType.UInt32:
                    return OleDbType.UnsignedInt;
                case DbType.UInt64:
                    return OleDbType.UnsignedBigInt;
                case DbType.VarNumeric:
                    return OleDbType.Decimal;
                case DbType.StringFixedLength:
                    return OleDbType.Char;
                case DbType.Xml:
                    return OleDbType.Char;
                default:
                    return OleDbType.IUnknown;
            }
        }

        private void Initialize() => this.Open();

        public void Open()
        {
            if (this._TrackerDbConn != null)
            {
                if (this._TrackerDbConn.State == ConnectionState.Open)
                    this._TrackerDbConn.Close();
                this._TrackerDbConn.Dispose();
                this._TrackerDbConn = (OleDbConnection)null;
            }
            this._TrackerDbConn = !string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings["Tracker08ConnectionString"].ConnectionString) ? new OleDbConnection(ConfigurationManager.ConnectionStrings["Tracker08ConnectionString"].ConnectionString) : throw new Exception("A connection string named Tracker08ConnectionString with a valid connection string must exist in the <connectionStrings> configuration section for the application.");
        }

        public bool TableExists(string pTableName)
        {
            bool flag = false;
            this._TrackerDbConn.Open();
            try
            {
                flag = this._TrackerDbConn.GetSchema("Tables", new string[3]
                {
        null,
        null,
        pTableName
                }).Rows.Count != 0;
            }
            catch (OleDbException ex)
            {
                this.ErrorResult = ex.Message;
            }
            finally
            {
                this._TrackerDbConn.Close();
            }
            return flag;
        }

        public string ExecuteNonQuerySQL(string strSQL)
        {
            return this.ExecuteNonQuerySQLWithParams(strSQL, this.Params.Count == 0 ? (List<DBParameter>)null : this.Params, this.WhereParams.Count == 0 ? (List<DBParameter>)null : this.WhereParams);
        }

        public string ExecuteNonQuerySQLWithParams(string strSQL, List<DBParameter> pParams)
        {
            return this.ExecuteNonQuerySQLWithParams(strSQL, pParams, (List<DBParameter>)null);
        }

        public string ExecuteNonQuerySQLWithParams(
          string strSQL,
          List<DBParameter> pParams,
          List<DBParameter> pWhereParams)
        {
            string str = string.Empty;

            try
            {
                this._TrackerDbConn.Open();
                OleDbTransaction transaction = this._TrackerDbConn.BeginTransaction();
                this._command = new OleDbCommand(strSQL, this._TrackerDbConn, transaction);

                if (pParams != null)
                {
                    foreach (DBParameter pParam in pParams)
                    {
                        this._command.Parameters.Add(BuildOleDbParameter(pParam));
                    }
                }

                if (pWhereParams != null)
                {
                    foreach (DBParameter pWhereParam in pWhereParams)
                    {
                        this._command.Parameters.Add(BuildOleDbParameter(pWhereParam));
                    }
                }

                for (int i = 0; i < _command.Parameters.Count; i++)
                {
                    OleDbParameter param = _command.Parameters[i];
                    System.Diagnostics.Debug.WriteLine(
                        $"Param[{i}]: Value={param.Value}, DbType={param.DbType}, OleDbType={param.OleDbType}");
                }

                this.numRecs = this._command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (OleDbException ex)
            {
                this.ErrorResult = ex.Message;
            }
            finally
            {
                if (this._command != null)
                    this._command.Dispose();

                this._TrackerDbConn.Close();
            }

            return str;

        }

        public DataSet ReturnDataSet(string strSQL)
        {
            return this.ReturnDataSet(strSQL, this.WhereParams.Count == 0 ? (List<DBParameter>)null : this.WhereParams);
        }

        public DataSet ReturnDataSet(string strSQL, List<DBParameter> pWhereParams)
        {
            DataSet dataSet = null;

            try
            {
                this._TrackerDbConn.Open();
                this._command = new OleDbCommand(strSQL, this._TrackerDbConn);

                if (pWhereParams != null)
                {
                    foreach (DBParameter param in pWhereParams)
                    {
                        this._command.Parameters.Add(BuildOleDbParameter(param));
                    }
                }

                dataSet = new DataSet();
                OleDbDataAdapter adapter = new OleDbDataAdapter(this._command);
                adapter.Fill(dataSet, "objDataSet");
            }
            catch (OleDbException ex)
            {
                this.ErrorResult = ex.Message;
            }
            finally
            {
                if (this._command != null)
                    this._command.Dispose();

                this._TrackerDbConn.Close();
            }

            return dataSet;
        }

        public IDataReader ExecuteSQLGetDataReader(string strSQL)
        {
            return this.ExecuteSQLGetDataReader(strSQL, this.WhereParams.Count == 0 ? (List<DBParameter>)null : this.WhereParams);
        }
        public IDataReader ExecuteSQLGetDataReader(string strSQL, List<DBParameter> pWhereParams)
        {
            IDataReader dataReader = null;

            try
            {
                this._TrackerDbConn.Open();
                this._command = new OleDbCommand(strSQL, this._TrackerDbConn);

                if (pWhereParams != null)
                {
                    foreach (DBParameter pWhereParam in pWhereParams)
                    {
                        this._command.Parameters.Add(BuildOleDbParameter(pWhereParam));
                    }
                }

                dataReader = this._command.ExecuteReader();
            }
            catch (OleDbException ex)
            {
                this.ErrorResult = $"SQL Error: {ex.Message}\nQuery: {strSQL}";

                foreach (OleDbParameter p in this._command.Parameters)
                {
                    this.ErrorResult += $"\nParam: {p.Value} ({p.OleDbType})";
                }
            }

            return dataReader;
        }

        // Add these helper methods to your TrackerDb class
        //private OleDbType GetOleDbType(DbType dbType)
        //{
        //    switch (dbType)
        //    {
        //        case DbType.DateTime: return OleDbType.DBTimeStamp;
        //        case DbType.Int32: return OleDbType.Integer;
        //        case DbType.String: return OleDbType.VarChar;
        //        default: return OleDbType.VarChar;
        //    }
        //}

        //private object ConvertParameterForAccess(object value, DbType dbType)
        //{
        //    if (value == null) return DBNull.Value;

        //    if (dbType == DbType.DateTime)
        //    {
        //        // Force proper Access date format
        //        return ((DateTime)value).ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        //    }
        //    return value;
        //}
        /* old one
          public IDataReader ExecuteSQLGetDataReader(string strSQL, List<DBParameter> pWhereParams)
                {
                    IDataReader dataReader = (IDataReader)null;
                    try
                    {
                        this._TrackerDbConn.Open();
                        this._command = new OleDbCommand(strSQL, this._TrackerDbConn);
                        if (pWhereParams != null)
                        {
                            foreach (DBParameter pWhereParam in pWhereParams)
                            {
                                OleDbParameterCollection parameters = this._command.Parameters;
                                OleDbParameter oleDbParameter1 = new OleDbParameter();
                                oleDbParameter1.Value = pWhereParam.DataValue;
                                oleDbParameter1.DbType = pWhereParam.DataDbType;
                                OleDbParameter oleDbParameter2 = oleDbParameter1;
                                parameters.Add(oleDbParameter2);
                            }
                        }
                        dataReader = (IDataReader)this._command.ExecuteReader();
                    }
                    catch (OleDbException ex)
                    {
                        this.ErrorResult = ex.Message;
                    }
                    return dataReader;
                }
        */

        public Hashtable ReturnHashTable(string strSQL)
        {
            return this.ReturnHashTable(strSQL, (List<DBParameter>)null);
        }

        public Hashtable ReturnHashTable(string strSQL, List<DBParameter> pWhereParams)
        {
            OleDbDataReader oleDbDataReader = null;
            Hashtable hashtable = new Hashtable();

            try
            {
                this._TrackerDbConn.Open();
                this._command = new OleDbCommand(strSQL, this._TrackerDbConn);

                if (pWhereParams != null)
                {
                    foreach (DBParameter pWhereParam in pWhereParams)
                    {
                        this._command.Parameters.Add(BuildOleDbParameter(pWhereParam));
                    }
                }

                oleDbDataReader = this._command.ExecuteReader();
                while (oleDbDataReader.Read())
                {
                    object key = oleDbDataReader.GetValue(0);
                    object value = oleDbDataReader.GetValue(1);
                    hashtable.Add(key, value);
                }
            }
            catch (OleDbException ex)
            {
                this.ErrorResult = ex.Message;
                throw;
            }
            finally
            {
                if (oleDbDataReader != null && !oleDbDataReader.IsClosed)
                    oleDbDataReader.Close();

                this._command.Dispose();
                this._TrackerDbConn.Close();
            }

            return hashtable;
        }


        public void Close()
        {
            if (this.Params.Count > 0)
                this.Params.Clear();
            if (this.WhereParams.Count > 0)
                this.WhereParams.Clear();
            if (this._command != null)
                this._command.Dispose();
            this.TrackerDbConn.Close();
            this.TrackerDbConn.Dispose();
        }

        public void AddParams(object pDataValue)
        {
            this.Params.Add(new DBParameter()
            {
                DataValue = pDataValue,
                DataDbType = DbType.String
            });
        }

        public void AddParams(object pDataValue, DbType pDataDbType)
        {
            this.Params.Add(new DBParameter()
            {
                DataValue = pDataValue,
                DataDbType = pDataDbType
            });
        }

        public void AddParams(object pDataValue, string pParamName)
        {
            this.Params.Add(new DBParameter()
            {
                DataValue = pDataValue,
                DataDbType = DbType.String,
                ParamName = pParamName
            });
        }

        public void AddParams(object pDataValue, DbType pDataDbType, string pParamName)
        {
            if (pDataValue == null)
            {
                pDataValue = (object)string.Empty;
                pDataDbType = DbType.String;
            }
            this.Params.Add(new DBParameter()
            {
                DataValue = pDataValue,
                DataDbType = pDataDbType
            });
        }

        public void AddWhereParams(object pDataValue)
        {
            this.WhereParams.Add(new DBParameter()
            {
                DataValue = pDataValue,
                DataDbType = DbType.String
            });
        }

        public void AddWhereParams(object pDataValue, DbType pDataDbType)
        {
            this.WhereParams.Add(new DBParameter()
            {
                DataValue = pDataValue,
                DataDbType = pDataDbType
            });
        }

        public void AddWhereParams(object pDataValue, string pParamName)
        {
            this.WhereParams.Add(new DBParameter()
            {
                DataValue = pDataValue,
                DataDbType = DbType.String,
                ParamName = pParamName
            });
        }

        public void AddWhereParams(object pDataValue, DbType pDataDbType, string pParamName)
        {
            if (pDataValue == null)
            {
                pDataValue = (object)string.Empty;
                pDataDbType = DbType.String;
            }
            this.WhereParams.Add(new DBParameter()
            {
                DataValue = pDataValue,
                DataDbType = pDataDbType
            });
        }

        public bool CreateIfDoesNotExists(string pTableName)
        {
            bool ifDoesNotExists = false;
            TrackerDb trackerDb = new TrackerDb();
            if (!trackerDb.TableExists(pTableName))
            {
                string strSQL = string.Empty;
                switch (pTableName)
                {
                    case "LogTbl":
                        strSQL = "CREATE TABLE LogTbl ( [LogID] AUTOINCREMENT, [DateAdded] DateTime, [UserID] INT, [SectionID] INT, [TranactionTypeID] INT,  [CustomerID] INT, Details VARCHAR(255), [Notes] MEMO,  CONSTRAINT [pk_LogID] PRIMARY KEY (LogID) )";
                        break;
                    case "SectionTypesTbl":
                        strSQL = "CREATE TABLE SectionTypesTbl ( [SectionID] INT, [SectionType] VARCHAR(50), [Notes] MEMO,  CONSTRAINT [pk_SectionID] PRIMARY KEY (SectionID) )";
                        break;
                    case "TransactionTypesTbl":
                        strSQL = "CREATE TABLE TransactionTypesTbl ( [TransactionID] INT, [TransactionType] VARCHAR(50), [Notes] MEMO,  CONSTRAINT [pk_TransactionID] PRIMARY KEY (TransactionID) )";
                        break;
                }
                if (!string.IsNullOrWhiteSpace(strSQL))
                    ifDoesNotExists = string.IsNullOrWhiteSpace(trackerDb.ExecuteNonQuerySQL(strSQL));
            }
            trackerDb.Close();
            return ifDoesNotExists;
        }
        // Add this to TrackerDb class as now an IDisposable clas
        public void Dispose()
        {
            this.Close();
        }
    }
}
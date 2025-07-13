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

#nullable disable
namespace TrackerDotNet.classes;

public class TrackerDb
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
    this._TrackerDbConn = (OleDbConnection) null;
    this._command = (OleDbCommand) null;
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
        return OleDbType.BigInt;
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
      this._TrackerDbConn = (OleDbConnection) null;
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
    return this.ExecuteNonQuerySQLWithParams(strSQL, this.Params.Count == 0 ? (List<DBParameter>) null : this.Params, this.WhereParams.Count == 0 ? (List<DBParameter>) null : this.WhereParams);
  }

  public string ExecuteNonQuerySQLWithParams(string strSQL, List<DBParameter> pParams)
  {
    return this.ExecuteNonQuerySQLWithParams(strSQL, pParams, (List<DBParameter>) null);
  }

  public string ExecuteNonQuerySQLWithParams(
    string strSQL,
    List<DBParameter> pParams,
    List<DBParameter> pWhereParams)
  {
    string str = string.Empty;
    this._TrackerDbConn.Open();
    OleDbTransaction transaction = this._TrackerDbConn.BeginTransaction();
    this._command = new OleDbCommand(strSQL, this._TrackerDbConn, transaction);
    if (pParams != null)
    {
      foreach (DBParameter pParam in pParams)
      {
        if (pParam.DataDbType == DbType.DateTime)
          pParam.DataValue = (object) ((DateTime) pParam.DataValue).Date;
        if (pParam.ParamName.Equals("?"))
        {
          OleDbParameterCollection parameters = this._command.Parameters;
          OleDbParameter oleDbParameter1 = new OleDbParameter();
          oleDbParameter1.Value = pParam.DataValue;
          oleDbParameter1.OleDbType = this.ConvertToOleDbType(pParam.DataDbType);
          OleDbParameter oleDbParameter2 = oleDbParameter1;
          parameters.Add(oleDbParameter2);
        }
        else
        {
          OleDbParameterCollection parameters = this._command.Parameters;
          OleDbParameter oleDbParameter3 = new OleDbParameter();
          oleDbParameter3.Value = pParam.DataValue;
          oleDbParameter3.DbType = pParam.DataDbType;
          oleDbParameter3.ParameterName = pParam.ParamName;
          OleDbParameter oleDbParameter4 = oleDbParameter3;
          parameters.Add(oleDbParameter4);
        }
      }
    }
    if (pWhereParams != null)
    {
      foreach (DBParameter pWhereParam in pWhereParams)
      {
        if (pWhereParam.ParamName.Equals("?"))
        {
          OleDbParameterCollection parameters = this._command.Parameters;
          OleDbParameter oleDbParameter5 = new OleDbParameter();
          oleDbParameter5.Value = pWhereParam.DataValue;
          oleDbParameter5.OleDbType = this.ConvertToOleDbType(pWhereParam.DataDbType);
          OleDbParameter oleDbParameter6 = oleDbParameter5;
          parameters.Add(oleDbParameter6);
        }
        else
        {
          OleDbParameterCollection parameters = this._command.Parameters;
          OleDbParameter oleDbParameter7 = new OleDbParameter();
          oleDbParameter7.Value = pWhereParam.DataValue;
          oleDbParameter7.DbType = pWhereParam.DataDbType;
          oleDbParameter7.ParameterName = pWhereParam.ParamName;
          OleDbParameter oleDbParameter8 = oleDbParameter7;
          parameters.Add(oleDbParameter8);
        }
      }
    }
    try
    {
      this.numRecs = this._command.ExecuteNonQuery();
      transaction.Commit();
    }
    catch (OleDbException ex)
    {
      transaction.Rollback();
      str = ex.Message;
      this.ErrorResult = str;
    }
    finally
    {
      this._TrackerDbConn.Close();
    }
    return str;
  }

  public DataSet ReturnDataSet(string strSQL)
  {
    return this.ReturnDataSet(strSQL, this.WhereParams.Count == 0 ? (List<DBParameter>) null : this.WhereParams);
  }

  public DataSet ReturnDataSet(string strSQL, List<DBParameter> pWhereParams)
  {
    DataSet dataSet = (DataSet) null;
    try
    {
      this._TrackerDbConn.Open();
      dataSet = new DataSet();
      this._command = new OleDbCommand(strSQL, this._TrackerDbConn);
      if (pWhereParams != null)
      {
        for (int index = 0; index < pWhereParams.Count; ++index)
        {
          OleDbParameterCollection parameters = this._command.Parameters;
          OleDbParameter oleDbParameter1 = new OleDbParameter();
          oleDbParameter1.Value = pWhereParams[index].DataValue;
          oleDbParameter1.DbType = pWhereParams[index].DataDbType;
          OleDbParameter oleDbParameter2 = oleDbParameter1;
          parameters.Add(oleDbParameter2);
        }
      }
      new OleDbDataAdapter(this._command).Fill(dataSet, "objDataSet");
    }
    catch (OleDbException ex)
    {
      this.ErrorResult = ex.Message;
    }
    finally
    {
      this._command.Dispose();
      this._TrackerDbConn.Close();
    }
    return dataSet;
  }

  public IDataReader ExecuteSQLGetDataReader(string strSQL)
  {
    return this.ExecuteSQLGetDataReader(strSQL, this.WhereParams.Count == 0 ? (List<DBParameter>) null : this.WhereParams);
  }

  public IDataReader ExecuteSQLGetDataReader(string strSQL, List<DBParameter> pWhereParams)
  {
    IDataReader dataReader = (IDataReader) null;
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
      dataReader = (IDataReader) this._command.ExecuteReader();
    }
    catch (OleDbException ex)
    {
      this.ErrorResult = ex.Message;
    }
    return dataReader;
  }

  public Hashtable ReturnHashTable(string strSQL)
  {
    return this.ReturnHashTable(strSQL, (List<DBParameter>) null);
  }

  public Hashtable ReturnHashTable(string strSQL, List<DBParameter> pWhereParams)
  {
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
      OleDbDataReader oleDbDataReader = this._command.ExecuteReader();
      Hashtable hashtable = new Hashtable();
      while (oleDbDataReader.Read())
        hashtable.Add((object) oleDbDataReader.GetString(0), (object) oleDbDataReader.GetString(1));
      return hashtable;
    }
    catch (OleDbException ex)
    {
      this.ErrorResult = ex.Message;
      throw;
    }
    finally
    {
      this._command.Dispose();
      this._TrackerDbConn.Close();
    }
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
      pDataValue = (object) string.Empty;
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
      pDataValue = (object) string.Empty;
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
}

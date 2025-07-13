// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.control.ItemUsageTbl
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using TrackerDotNet.classes;

#nullable disable
namespace TrackerDotNet.control;

public class ItemUsageTbl
{
  private const string CONST_SQL_SELECT = "SELECT ClientUsageLineNo, CustomerID, Date AS ItemDate, ItemProvided, AmountProvided, PrepTypeID, PackagingID, Notes FROM ItemUsageTbl";
  private const string CONST_SQL_SELECT_CUSTOMERLASTORDER = "SELECT ItemUsageTbl.Date AS ItemDate, ItemUsageTbl.ItemProvided, ItemUsageTbl.AmountProvided, ItemUsageTbl.PrepTypeID, ItemUsageTbl.PackagingID  FROM ItemTypeTbl INNER JOIN ItemUsageTbl ON ItemTypeTbl.ItemTypeID = ItemUsageTbl.ItemProvided  WHERE (CustomerID=? AND ItemTypeTbl.ServiceTypeID=?) AND (ItemUsageTbl.Date = (SELECT Max(ItemUsageTbl.Date)  FROM ItemTypeTbl INNER JOIN ItemUsageTbl ON ItemTypeTbl.ItemTypeID = ItemUsageTbl.ItemProvided WHERE CustomerID=? AND ItemTypeTbl.ServiceTypeID=?  ))";
  private const string CONST_SQL_SELECT_CUSTOMERLASTMAINITEM = "SELECT ItemUsageTbl.[Date] AS ItemDate, ItemUsageTbl.ItemProvided, ItemUsageTbl.AmountProvided, ItemUsageTbl.PrepTypeID, ItemUsageTbl.PackagingID  FROM ItemTypeTbl INNER JOIN ItemUsageTbl ON ItemTypeTbl.ItemTypeID = ItemUsageTbl.ItemProvided  WHERE (ItemTypeTbl.ServiceTypeID IN (1,4,5)) AND (CustomerID=?) AND (ItemUsageTbl.[Date] =  (SELECT Max(ItemUsageTbl.[Date]) FROM ItemTypeTbl INNER JOIN ItemUsageTbl ON ItemTypeTbl.ItemTypeID = ItemUsageTbl.ItemProvided  WHERE (ItemTypeTbl.ServiceTypeID IN (1,4,5)) AND (CustomerID=?)))";
  private const string CONST_SQL_UPDATE = "UPDATE ItemUsageTbl SET [Date] = ?, ItemProvided = ?, AmountProvided = ? , PrepTypeID = ?, PackagingID = ?, Notes = ? WHERE ClientUsageLineNo = ? ";
  private const string CONST_SQL_INSERT = "INSERT INTO ItemUsageTbl (CustomerID, [Date], ItemProvided, AmountProvided, PrepTypeID, PackagingID, Notes) VALUES (?, ?, ?, ?, ?, ?, ?)";
  private const string CONST_SQL_DELETELINE = "DELETE FROM ItemUsageTbl WHERE ClientUsageLineNo = ? ";
  private long _ClientUsageLineNo;
  private long _CustomerID;
  private DateTime _ItemDate;
  private int _ItemProvidedID;
  private double _AmountProvided;
  private int _PrepTypeID;
  private int _PackagingID;
  private string _Notes;

  public ItemUsageTbl()
  {
    this._ClientUsageLineNo = 0L;
    this._CustomerID = 0L;
    this._ItemDate = DateTime.MinValue;
    this._ItemProvidedID = 0;
    this._AmountProvided = 0.0;
    this._PrepTypeID = 0;
    this._PackagingID = 0;
    this._Notes = string.Empty;
  }

  public long ClientUsageLineNo
  {
    get => this._ClientUsageLineNo;
    set => this._ClientUsageLineNo = value;
  }

  public long CustomerID
  {
    get => this._CustomerID;
    set => this._CustomerID = value;
  }

  public DateTime ItemDate
  {
    get => this._ItemDate;
    set => this._ItemDate = value;
  }

  public int ItemProvidedID
  {
    get => this._ItemProvidedID;
    set => this._ItemProvidedID = value;
  }

  public double AmountProvided
  {
    get => this._AmountProvided;
    set => this._AmountProvided = value;
  }

  public int PrepTypeID
  {
    get => this._PrepTypeID;
    set => this._PrepTypeID = value;
  }

  public int PackagingID
  {
    get => this._PackagingID;
    set => this._PackagingID = value;
  }

  public string Notes
  {
    get => this._Notes;
    set => this._Notes = value;
  }

  [DataObjectMethod(DataObjectMethodType.Select)]
  public List<ItemUsageTbl> GetAllItemsUsed(long pCustomerID, string SortBy)
  {
    List<ItemUsageTbl> allItemsUsed = new List<ItemUsageTbl>();
    TrackerDb trackerDb = new TrackerDb();
    string strSQL = "SELECT ClientUsageLineNo, CustomerID, Date AS ItemDate, ItemProvided, AmountProvided, PrepTypeID, PackagingID, Notes FROM ItemUsageTbl WHERE CustomerID = " + pCustomerID.ToString();
    if (!string.IsNullOrEmpty(SortBy))
      strSQL = $"{strSQL} ORDER BY {SortBy}";
    IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
    if (dataReader != null)
    {
      while (dataReader.Read())
        allItemsUsed.Add(new ItemUsageTbl()
        {
          ClientUsageLineNo = dataReader["ClientUsageLineNo"] == DBNull.Value ? 0L : Convert.ToInt64(dataReader["ClientUsageLineNo"]),
          CustomerID = dataReader["CustomerID"] == DBNull.Value ? 0L : (long) Convert.ToInt32(dataReader["CustomerID"]),
          ItemDate = dataReader["ItemDate"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["ItemDate"]).Date,
          ItemProvidedID = dataReader["ItemProvided"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["ItemProvided"]),
          AmountProvided = dataReader["AmountProvided"] == DBNull.Value ? 0.0 : Convert.ToDouble(dataReader["AmountProvided"]),
          PrepTypeID = dataReader["PrepTypeID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["PrepTypeID"]),
          PackagingID = dataReader["PackagingID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["PackagingID"]),
          Notes = dataReader["Notes"] == DBNull.Value ? string.Empty : dataReader["Notes"].ToString()
        });
      dataReader.Close();
    }
    trackerDb.Close();
    return allItemsUsed;
  }

  [DataObjectMethod(DataObjectMethodType.Select)]
  public List<ItemUsageTbl> GetLastItemsUsed(long pCustomerID, int pServiceTypeID)
  {
    List<ItemUsageTbl> lastItemsUsed = new List<ItemUsageTbl>();
    TrackerDb trackerDb = new TrackerDb();
    trackerDb.AddWhereParams((object) pCustomerID, DbType.Int64);
    trackerDb.AddWhereParams((object) pServiceTypeID, DbType.Int32);
    trackerDb.AddWhereParams((object) pCustomerID, DbType.Int64);
    trackerDb.AddWhereParams((object) pServiceTypeID, DbType.Int32);
    IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader("SELECT ItemUsageTbl.Date AS ItemDate, ItemUsageTbl.ItemProvided, ItemUsageTbl.AmountProvided, ItemUsageTbl.PrepTypeID, ItemUsageTbl.PackagingID  FROM ItemTypeTbl INNER JOIN ItemUsageTbl ON ItemTypeTbl.ItemTypeID = ItemUsageTbl.ItemProvided  WHERE (CustomerID=? AND ItemTypeTbl.ServiceTypeID=?) AND (ItemUsageTbl.Date = (SELECT Max(ItemUsageTbl.Date)  FROM ItemTypeTbl INNER JOIN ItemUsageTbl ON ItemTypeTbl.ItemTypeID = ItemUsageTbl.ItemProvided WHERE CustomerID=? AND ItemTypeTbl.ServiceTypeID=?  ))");
    if (dataReader != null)
    {
      while (dataReader.Read())
      {
        ItemUsageTbl itemUsageTbl = new ItemUsageTbl();
        itemUsageTbl.CustomerID = pCustomerID;
        itemUsageTbl.ItemDate = dataReader["ItemDate"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["ItemDate"]).Date;
        itemUsageTbl.ItemProvidedID = dataReader["ItemProvided"] == DBNull.Value ? -1 : Convert.ToInt32(dataReader["ItemProvided"]);
        itemUsageTbl.AmountProvided = dataReader["AmountProvided"] == DBNull.Value ? 0.0 : Convert.ToDouble(dataReader["AmountProvided"]);
        itemUsageTbl.PrepTypeID = dataReader["PrepTypeID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["PrepTypeID"]);
        itemUsageTbl.PackagingID = dataReader["PackagingID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["PackagingID"]);
        int groupIfItWas = new UsedItemGroupTbl().ChangeItemIDToGroupIfItWas(pCustomerID, itemUsageTbl.ItemProvidedID, itemUsageTbl.ItemDate);
        if (groupIfItWas != itemUsageTbl.ItemProvidedID)
        {
          itemUsageTbl.ItemProvidedID = groupIfItWas;
          itemUsageTbl.Notes = "Last item was a group item";
        }
        lastItemsUsed.Add(itemUsageTbl);
      }
      dataReader.Close();
    }
    trackerDb.Close();
    return lastItemsUsed;
  }

  public ItemUsageTbl GetLastMaintenanceItem(long pCustomerID)
  {
    ItemUsageTbl lastMaintenanceItem = (ItemUsageTbl) null;
    TrackerDb trackerDb = new TrackerDb();
    trackerDb.AddWhereParams((object) pCustomerID, DbType.Int64);
    trackerDb.AddWhereParams((object) pCustomerID, DbType.Int64);
    IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader("SELECT ItemUsageTbl.[Date] AS ItemDate, ItemUsageTbl.ItemProvided, ItemUsageTbl.AmountProvided, ItemUsageTbl.PrepTypeID, ItemUsageTbl.PackagingID  FROM ItemTypeTbl INNER JOIN ItemUsageTbl ON ItemTypeTbl.ItemTypeID = ItemUsageTbl.ItemProvided  WHERE (ItemTypeTbl.ServiceTypeID IN (1,4,5)) AND (CustomerID=?) AND (ItemUsageTbl.[Date] =  (SELECT Max(ItemUsageTbl.[Date]) FROM ItemTypeTbl INNER JOIN ItemUsageTbl ON ItemTypeTbl.ItemTypeID = ItemUsageTbl.ItemProvided  WHERE (ItemTypeTbl.ServiceTypeID IN (1,4,5)) AND (CustomerID=?)))");
    if (dataReader != null)
    {
      if (dataReader.Read())
      {
        lastMaintenanceItem = new ItemUsageTbl();
        lastMaintenanceItem.CustomerID = pCustomerID;
        lastMaintenanceItem.ItemDate = dataReader["ItemDate"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(dataReader["ItemDate"]).Date;
        lastMaintenanceItem.ItemProvidedID = dataReader["ItemProvided"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["ItemProvided"]);
        lastMaintenanceItem.AmountProvided = dataReader["AmountProvided"] == DBNull.Value ? 0.0 : Convert.ToDouble(dataReader["AmountProvided"]);
        lastMaintenanceItem.PrepTypeID = dataReader["PrepTypeID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["PrepTypeID"]);
        lastMaintenanceItem.PackagingID = dataReader["PackagingID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["PackagingID"]);
      }
      dataReader.Close();
    }
    trackerDb.Close();
    return lastMaintenanceItem;
  }

  [DataObjectMethod(DataObjectMethodType.Insert)]
  public string InsertItemsUsed(ItemUsageTbl ItemUsageLine)
  {
    TrackerDb trackerDb = new TrackerDb();
    trackerDb.AddParams((object) ItemUsageLine.CustomerID, DbType.Int64);
    trackerDb.AddParams((object) ItemUsageLine.ItemDate, DbType.Date);
    trackerDb.AddParams((object) ItemUsageLine.ItemProvidedID, DbType.Int32);
    trackerDb.AddParams((object) ItemUsageLine.AmountProvided, DbType.Double);
    trackerDb.AddParams((object) ItemUsageLine.PrepTypeID, DbType.Int32);
    trackerDb.AddParams((object) ItemUsageLine.PackagingID, DbType.Int32);
    trackerDb.AddParams((object) ItemUsageLine.Notes, DbType.String);
    string str = trackerDb.ExecuteNonQuerySQL("INSERT INTO ItemUsageTbl (CustomerID, [Date], ItemProvided, AmountProvided, PrepTypeID, PackagingID, Notes) VALUES (?, ?, ?, ?, ?, ?, ?)");
    trackerDb.Close();
    return str;
  }

  [DataObjectMethod(DataObjectMethodType.Update)]
  public string UpdateItemsUsed(
    long CustomerID,
    DateTime ItemDate,
    int ItemProvidedID,
    double AmountProvided,
    int PrepTypeID,
    int PackagingID,
    string Notes,
    long original_ClientUsageLineNo)
  {
    return this.UpdateItemsUsed(new ItemUsageTbl()
    {
      ClientUsageLineNo = original_ClientUsageLineNo,
      CustomerID = CustomerID,
      ItemDate = ItemDate,
      ItemProvidedID = ItemProvidedID,
      AmountProvided = AmountProvided,
      PrepTypeID = PrepTypeID,
      PackagingID = PackagingID,
      Notes = Notes
    }, original_ClientUsageLineNo);
  }

  [DataObjectMethod(DataObjectMethodType.Update)]
  public string UpdateItemsUsed(ItemUsageTbl ItemUsageLine)
  {
    return this.UpdateItemsUsed(ItemUsageLine, ItemUsageLine.ClientUsageLineNo);
  }

  [DataObjectMethod(DataObjectMethodType.Update)]
  public string UpdateItemsUsed(ItemUsageTbl ItemUsageLine, long original_ClientUsageLineNo)
  {
    TrackerDb trackerDb = new TrackerDb();
    trackerDb.AddParams((object) ItemUsageLine.ItemDate, DbType.Date);
    trackerDb.AddParams((object) ItemUsageLine.ItemProvidedID, DbType.Int32);
    trackerDb.AddParams((object) Math.Round(ItemUsageLine.AmountProvided, 2), DbType.Double);
    trackerDb.AddParams((object) ItemUsageLine.PrepTypeID, DbType.Int32);
    trackerDb.AddParams((object) ItemUsageLine.PackagingID, DbType.Int32);
    trackerDb.AddParams((object) ItemUsageLine.Notes, DbType.String);
    trackerDb.AddWhereParams((object) original_ClientUsageLineNo, DbType.Int64);
    string str = trackerDb.ExecuteNonQuerySQL("UPDATE ItemUsageTbl SET [Date] = ?, ItemProvided = ?, AmountProvided = ? , PrepTypeID = ?, PackagingID = ?, Notes = ? WHERE ClientUsageLineNo = ? ");
    trackerDb.Close();
    return str;
  }

  [DataObjectMethod(DataObjectMethodType.Delete)]
  public string DeleteItemLine(long ClientUsageLineNo)
  {
    TrackerDb trackerDb = new TrackerDb();
    trackerDb.AddWhereParams((object) ClientUsageLineNo, DbType.Int64);
    string str = trackerDb.ExecuteNonQuerySQL("DELETE FROM ItemUsageTbl WHERE ClientUsageLineNo = ? ");
    trackerDb.Close();
    return str;
  }
}

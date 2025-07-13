using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Configuration;
using System.Web;

namespace TrackerDotNet.control
{
  public class ClientsToReceiveReminderQry
  {

#region  InternalVars
    private long _CustomerID;
    private string _CompanyName;
    private string _ContactFirstName;
    private string _ContactLastName;
    private string _EmailAddress;
    private string _ContactAltFirstName;
    private string _ContactAltLastName;
    private string _AltEmailAddress;
    private long _LastCupCount;
    private double _DailyConsumption;
    private long _CityID;
    private long _PrimaryPreferenceID;
    private string _PrimaryPreferenceDesc;
    private double _PrimaryPreferenceQty;
    private long _SecondaryPreferenceID;
    private double _SecondaryPreferenceQty;
    private string _SecondaryPreferenceDesc;
    private long _PrefPackagingID;
    private long _PrefPrepTypeID;
    private int _EquipTypeID;
    private int _PreferedAgent;
    private bool _TypicallySecToo;
    private bool _enabled;
    private bool _AutomaticFulfill;
    private bool _PredictionDisabled;
    private bool _AlwaysSendChkUp;
    private bool _NormallyResponds;
    private bool _UsesFilter;
    private int _ReminderCount;
    private DateTime _NextCoffeeBy;
    private DateTime _NextCleanOn;
    private DateTime _NextFilterEst;
    private DateTime _NextDescaleEst;
    private DateTime _NextServiceEst;
    //-- not needed    private string _Notes;
#endregion

#region ClassDefinition
    public ClientsToReceiveReminderQry()
    {
      _CustomerID = 0;
      _CompanyName = String.Empty;
      _ContactFirstName = _ContactLastName = String.Empty;
      _EmailAddress = _AltEmailAddress = String.Empty;
      _ContactAltFirstName = _ContactAltLastName = String.Empty;
      _LastCupCount = 0;
      _DailyConsumption = 0;
      _CityID = _PrimaryPreferenceID = _SecondaryPreferenceID = 0;
      _PrimaryPreferenceDesc = String.Empty;
      _PrimaryPreferenceQty = 0.0;
      _PrimaryPreferenceDesc = String.Empty;
      _SecondaryPreferenceQty = 0.0;
      _SecondaryPreferenceDesc = String.Empty;
      _PrefPackagingID = _PrefPrepTypeID = 0;
      _EquipTypeID = _PreferedAgent = 0;
      _TypicallySecToo = _enabled = _AutomaticFulfill = _PredictionDisabled = _AlwaysSendChkUp = _NormallyResponds = _UsesFilter = false;
      _ReminderCount = 0;
      _NextCoffeeBy = _NextCleanOn = _NextFilterEst = _NextDescaleEst = _NextServiceEst = DateTime.MinValue;
    //-- not needed    private string _Notes;
    }
#endregion

#region PublicVars
    public long CustomerID { get { return _CustomerID; } set { _CustomerID = value; } }
    public string CompanyName { get { return _CompanyName; } set { _CompanyName = value; } }
    public string ContactFirstName { get { return _ContactFirstName; } set { _ContactFirstName = value; } }
    public string ContactLastName { get { return _ContactLastName; } set { _ContactLastName = value; } }
    public string EmailAddress { get { return _EmailAddress; } set { _EmailAddress = value; } }
    public string ContactAltFirstName { get { return _ContactAltFirstName; } set { _ContactAltFirstName = value; } }
    public string ContactAltLastName { get { return _ContactAltLastName; } set { _ContactAltLastName = value; } }
    public string AltEmailAddress { get { return _AltEmailAddress; } set { _AltEmailAddress = value; } }
    public long LastCupCount { get { return _LastCupCount; } set { _LastCupCount = value; } }
    public double DailyConsumption { get { return _DailyConsumption; } set { _DailyConsumption = value; } }
    public long CityID { get { return _CityID; } set { _CityID = value; } }
    public long PrimaryPreferenceID { get { return _PrimaryPreferenceID; } set { _PrimaryPreferenceID = value; } }
    public string PrimaryPreferenceDesc { get { return _PrimaryPreferenceDesc; } set { _PrimaryPreferenceDesc = value; } }
    public double PrimaryPreferenceQty { get { return _PrimaryPreferenceQty; } set { _PrimaryPreferenceQty = value; } }
    public long SecondaryPreferenceID { get { return _SecondaryPreferenceID; } set { _SecondaryPreferenceID = value; } }
    public double SecondaryPreferenceQty { get { return _SecondaryPreferenceQty; } set { _SecondaryPreferenceQty = value; } }
    public string SecondaryPreferenceDesc { get { return _SecondaryPreferenceDesc; } set { _SecondaryPreferenceDesc = value; } }
    public long PrefPackagingID { get { return _PrefPackagingID; } set { _PrefPackagingID = value; } }
    public long PrefPrepTypeID { get { return _PrefPrepTypeID; } set { _PrefPrepTypeID = value; } }
    public int EquipTypeID { get { return _EquipTypeID; } set { _EquipTypeID = value; } }
    public int PreferedAgent { get { return _PreferedAgent; } set { _PreferedAgent = value; } }
    public bool TypicallySecToo { get { return _TypicallySecToo; } set { _TypicallySecToo = value; } }
    public bool enabled { get { return _enabled; } set { _enabled = value; } }
    public bool AutomaticFulfill { get { return _AutomaticFulfill; } set { _AutomaticFulfill = value; } }
    public bool PredictionDisabled { get { return _PredictionDisabled; } set { _PredictionDisabled = value; } }
    public bool AlwaysSendChkUp { get { return _AlwaysSendChkUp; } set { _AlwaysSendChkUp = value; } }
    public bool NormallyResponds { get { return _NormallyResponds; } set { _NormallyResponds = value; } }
    public bool UsesFilter { get { return _UsesFilter; } set { _UsesFilter = value; } }
    public int ReminderCount { get { return _ReminderCount; } set { _ReminderCount = value; } }
    public DateTime NextCoffeeBy { get { return _NextCoffeeBy; } set { _NextCoffeeBy = value; } }
    public DateTime NextCleanOn { get { return _NextCleanOn; } set { _NextCleanOn = value; } }
    public DateTime NextFilterEst { get { return _NextFilterEst; } set { _NextFilterEst = value; } }
    public DateTime NextDescaleEst { get { return _NextDescaleEst; } set { _NextDescaleEst = value; } }
    public DateTime NextServiceEst { get { return _NextServiceEst; } set { _NextServiceEst = value; } }
#endregion

#region ConstantDeclarations
    const string CONST_CONSTRING = "Tracker08ConnectionString";
/*
SELECT        CustomersTbl.CustomerID, CompanyName, ContactFirstName, ContactLastName, ContactAltFirstName, 
                         ContactAltLastName, City, EmailAddress, AltEmailAddress, EquipType, 
                         CoffeePreference, PriPrefQty, ItemTypeTbl.ItemDesc AS PrimaryPreferenceDesc, ClientUsageTbl.LastCupCount, 
                         ClientUsageTbl.DailyConsumption, SecItemTypeTbl.ItemDesc AS SecondaryPreferenceDesc, SecondaryPreference, SecPrefQty, 
                         PrefPrepTypeID, PrefPackagingID, TypicallySecToo, PreferedAgent, autofulfill, 
                         enabled, UsesFilter, PredictionDisabled, AlwaysSendChkUp, NormallyResponds, 
                         ReminderCount, ClientUsageTbl.NextCoffeeBy, ClientUsageTbl.NextCleanOn, ClientUsageTbl.NextFilterEst, ClientUsageTbl.NextDescaleEst, 
                         ClientUsageTbl.NextServiceEst, NextRoastDateByCityTbl.DeliveryDate
FROM            ((((CustomersTbl INNER JOIN
                         ClientUsageTbl ON CustomersTbl.CustomerID = ClientUsageTbl.CustomerId) LEFT OUTER JOIN
                         ItemTypeTbl ON CustomersTbl.CoffeePreference = ItemTypeTbl.ItemTypeID) LEFT OUTER JOIN
                         NextRoastDateByCityTbl ON CustomersTbl.City = NextRoastDateByCityTbl.CityID) LEFT OUTER JOIN
                         ItemTypeTbl SecItemTypeTbl ON CustomersTbl.SecondaryPreference = SecItemTypeTbl.ItemTypeID)
WHERE        (CustomersTbl.AlwaysSendChkUp = true) AND (CustomersTbl.PredictionDisabled = false) AND (NextRoastDateByCityTbl.DeliveryDate < DateAdd('d', 3, 
                         NextRoastDateByCityTbl.DeliveryDate)) AND (NOT EXISTS
                             (SELECT        OrderID
                               FROM            OrdersTbl
                               WHERE        (CustomerId = CustomersTbl.CustomerID) AND (RoastDate > DateAdd('ww', - 1, NextRoastDateByCityTbl.PreperationDate)) AND 
                                                         (RoastDate <= NextRoastDateByCityTbl.DeliveryDate)))
ORDER BY CustomersTbl.CompanyName

 */
    const string CONST_SQL_CUSTOMERS_SELECT = "SELECT CustomersTbl.CustomerID, CompanyName, ContactFirstName, ContactLastName, ContactAltFirstName, " +
                         "ContactAltLastName, City, EmailAddress, AltEmailAddress, EquipType, CoffeePreference, PriPrefQty, " +
                         "ItemTypeTbl.ItemDesc AS PrimaryPreferenceDesc, ClientUsageTbl.LastCupCount, ClientUsageTbl.DailyConsumption, " +
                         "SecItemTypeTbl.ItemDesc AS SecondaryPreferenceDesc, SecondaryPreference, SecPrefQty, PrefPrepTypeID, " +
                         "PrefPackagingID, TypicallySecToo, PreferedAgent, autofulfill, enabled, UsesFilter, PredictionDisabled, " +
                         "AlwaysSendChkUp, NormallyResponds, ReminderCount, ClientUsageTbl.NextCoffeeBy, ClientUsageTbl.NextCleanOn, " +
                         "ClientUsageTbl.NextFilterEst, ClientUsageTbl.NextDescaleEst, ClientUsageTbl.NextServiceEst, NextRoastDateByCityTbl.DeliveryDate " +
                       "FROM ((((CustomersTbl INNER JOIN ClientUsageTbl ON CustomersTbl.CustomerID = ClientUsageTbl.CustomerId) " +
                         " LEFT OUTER JOIN ItemTypeTbl ON CustomersTbl.CoffeePreference = ItemTypeTbl.ItemTypeID) LEFT OUTER JOIN " +
                         " NextRoastDateByCityTbl ON CustomersTbl.City = NextRoastDateByCityTbl.CityID) LEFT OUTER JOIN " +
                         " ItemTypeTbl SecItemTypeTbl ON CustomersTbl.SecondaryPreference = SecItemTypeTbl.ItemTypeID) " +
                       "WHERE (CustomersTbl.AlwaysSendChkUp = true) AND (CustomersTbl.PredictionDisabled = false) AND " +
                         "  (NextRoastDateByCityTbl.DeliveryDate < DateAdd('d', 3, NextRoastDateByCityTbl.DeliveryDate))" +
                         "   AND (NOT EXISTS (SELECT OrderID FROM OrdersTbl " +
                         "   WHERE  (CustomerId = CustomersTbl.CustomerID) AND (RoastDate > DateAdd('ww', - 1, NextRoastDateByCityTbl.PreperationDate)) " +
                         "          AND (RoastDate <= NextRoastDateByCityTbl.DeliveryDate)))" +
                       "ORDER BY CustomersTbl.CompanyName";
#endregion

    private ClientsToReceiveReminderQry MoveReaderDataToCustomersData(OleDbDataReader pDataReader)
    { 
      ClientsToReceiveReminderQry  _ClientData = new ClientsToReceiveReminderQry();
      _ClientData.CustomerID = Convert.ToInt64(pDataReader["CustomerID"]);
      _ClientData.CompanyName = (pDataReader["CompanyName"] == DBNull.Value) ? "" : pDataReader["CompanyName"].ToString();
      _ClientData.ContactFirstName = (pDataReader["ContactFirstName"] == DBNull.Value) ? "" : pDataReader["ContactFirstName"].ToString();
      _ClientData.ContactLastName = (pDataReader["ContactLastName"] == DBNull.Value) ? "" : pDataReader["ContactLastName"].ToString();
      _ClientData.EmailAddress = (pDataReader["EmailAddress"] == DBNull.Value) ? "" : pDataReader["EmailAddress"].ToString();
      _ClientData.ContactAltFirstName = (pDataReader["ContactAltFirstName"] == DBNull.Value) ? "" : pDataReader["ContactAltFirstName"].ToString();
      _ClientData.ContactAltLastName = (pDataReader["ContactAltLastName"] == DBNull.Value) ? "" : pDataReader["ContactAltLastName"].ToString();
      _ClientData.CityID = (pDataReader["City"] == DBNull.Value) ? 0 : Convert.ToInt32(pDataReader["City"]);
      _ClientData.LastCupCount = (pDataReader["LastCupCount"] == DBNull.Value) ? 0 : Convert.ToInt64(pDataReader["LastCupCount"]);
      _ClientData.DailyConsumption = (pDataReader["DailyConsumption"] == DBNull.Value) ? 0 : Convert.ToDouble(pDataReader["DailyConsumption"]);
      _ClientData.PrimaryPreferenceID = (pDataReader["CoffeePreference"] == DBNull.Value) ? 0 : Convert.ToInt32(pDataReader["CoffeePreference"]);
      _ClientData.PrimaryPreferenceDesc = (pDataReader["PrimaryPreferenceDesc"] == DBNull.Value) ? "" : pDataReader["PrimaryPreferenceDesc"].ToString();
      _ClientData.PrimaryPreferenceQty = (pDataReader["PriPrefQty"] == DBNull.Value) ? 0 : Convert.ToDouble(pDataReader["PriPrefQty"]);
      _ClientData.SecondaryPreferenceID = (pDataReader["SecondaryPreference"] == DBNull.Value) ? 0 : Convert.ToInt32(pDataReader["SecondaryPreference"]);
      _ClientData.SecondaryPreferenceDesc = (pDataReader["SecondaryPreferenceDesc"] == DBNull.Value) ? "" : pDataReader["SecondaryPreferenceDesc"].ToString();
      _ClientData.SecondaryPreferenceQty = (pDataReader["SecPrefQty"] == DBNull.Value) ? 0 : Convert.ToDouble(pDataReader["SecPrefQty"]);
      _ClientData.PrefPackagingID = (pDataReader["PrefPackagingID"] == DBNull.Value) ? 0 : Convert.ToInt32(pDataReader["PrefPackagingID"]);
      _ClientData.PrefPrepTypeID = (pDataReader["PrefPrepTypeID"] == DBNull.Value) ? 0 : Convert.ToInt32(pDataReader["PrefPrepTypeID"]);
      _ClientData.EquipTypeID = (pDataReader["EquipType"] == DBNull.Value) ? 0 : Convert.ToInt32(pDataReader["EquipType"]);
      _ClientData.enabled = (pDataReader["enabled"] == DBNull.Value) ? false : Convert.ToBoolean(pDataReader["enabled"]);
      _ClientData.UsesFilter = (pDataReader["UsesFilter"] == DBNull.Value) ? false : Convert.ToBoolean(pDataReader["UsesFilter"]);
      _ClientData.AutomaticFulfill = (pDataReader["autofulfill"] == DBNull.Value) ? false : Convert.ToBoolean(pDataReader["autofulfill"]);
      _ClientData.PredictionDisabled = (pDataReader["PredictionDisabled"] == DBNull.Value) ? false : Convert.ToBoolean(pDataReader["PredictionDisabled"]);
      _ClientData.TypicallySecToo = (pDataReader["TypicallySecToo"] == DBNull.Value) ? false : Convert.ToBoolean(pDataReader["TypicallySecToo"]);
      _ClientData.AlwaysSendChkUp = (pDataReader["AlwaysSendChkUp"] == DBNull.Value) ? false : Convert.ToBoolean(pDataReader["AlwaysSendChkUp"]);
      _ClientData.NormallyResponds = (pDataReader["NormallyResponds"] == DBNull.Value) ? false : Convert.ToBoolean(pDataReader["NormallyResponds"]);
      _ClientData.ReminderCount = (pDataReader["ReminderCount"] == DBNull.Value) ? 0 : Convert.ToInt32(pDataReader["ReminderCount"]);
      _ClientData.NextCoffeeBy = (pDataReader["NextCoffeeBy"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(pDataReader["NextCoffeeBy"]);
      _ClientData.NextCleanOn = (pDataReader["NextCleanOn"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(pDataReader["NextCleanOn"]);
      _ClientData.NextFilterEst = (pDataReader["NextFilterEst"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(pDataReader["NextFilterEst"]);
      _ClientData.NextDescaleEst= (pDataReader["NextDescaleEst"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(pDataReader["NextDescaleEst"]);
      _ClientData.NextServiceEst= (pDataReader["NextServiceEst"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(pDataReader["NextServiceEst"]);

      return _ClientData;
    }

    public List<ClientsToReceiveReminderQry> GetCustomersToReveiveReminder(string SortBy)
    {
      List<ClientsToReceiveReminderQry> _ListCustomersData = new List<ClientsToReceiveReminderQry>();
      string _connectionStr = ConfigurationManager.ConnectionStrings[CONST_CONSTRING].ConnectionString;

      using (OleDbConnection _conn = new OleDbConnection(_connectionStr))
      {
        string _sqlCmd = CONST_SQL_CUSTOMERS_SELECT;

        if (!String.IsNullOrEmpty(SortBy))
          _sqlCmd += " ORDER BY " + SortBy;
        // run the qurey we have built
        OleDbCommand _cmd = new OleDbCommand(_sqlCmd, _conn);

        _conn.Open();
        OleDbDataReader _DataReader = _cmd.ExecuteReader();
        while (_DataReader.Read())
        {
          _ListCustomersData.Add(MoveReaderDataToCustomersData(_DataReader));  //_CustomersTblData);
        }
      }

      return _ListCustomersData;
    }
  }
}
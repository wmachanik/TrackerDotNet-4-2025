// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.classes.TrackerTools
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using TrackerDotNet.Controls; 

//- only form later versions #nullable disable
namespace TrackerDotNet.Classes
{
    public class TrackerTools
    {
        public const string CONST_STR_NULLDATE = "1980/01/01";
        public const string CONST_SESSION_DATAACCESSERROR = "DataAccessError";
        public const string CONST_POREQUIRED = "!!!PO required!!!";
        public const int CONST_SERVTYPECLEAN = 1;
        public const int CONST_SERVTYPECOFFEE = 2;
        public const int CONST_SERVTYPECOUNT = 3;
        public const int CONST_SERVTYPEDESCALE = 4;
        public const int CONST_SERVTYPEFILTER = 5;
        public const int CONST_SERVTYPESWOPCOLLECT = 6;
        public const int CONST_SERVTYPESWOPSTART = 7;
        public const int CONST_SERVTYPESWOPSTOP = 8;
        public const int CONST_SERVTYPESWOPRETRUN = 9;
        public const int CONST_SERVTYPESERVICE = 10;
        public const int CONST_SERVTYPE1WKHOLI = 11;
        public const int CONST_SERVTYPE2WKHOLI = 12;
        public const int CONST_SERVTYPE3WKHOLI = 13;
        public const int CONST_SERVTYPE1MTHHOLI = 14;
        public const int CONST_SERVTYPE6WKHOLI = 15;
        public const int CONST_SERVTYPE2MTHHOLI = 16 /*0x10*/;
        public const int CONST_SERVTYPENOTAPPLICABLE = 17;
        public const int CONST_SERVTYPEMAINTENANCE = 18;
        public const int CONST_SERVTYPEGREENBEAN = 19;
        public const int CONST_SERVTYPEGROUPITEM = 21;
        public const string CONST_STRING_SERVTYPECLEAN = "1";
        public const string CONST_STRING_SERVTYPECOFFEE = "2";
        public const string CONST_STRING_SERVTYPECOUNT = "3";
        public const string CONST_STRING_SERVTYPEDESCALE = "4";
        public const string CONST_STRING_SERVTYPEFILTER = "5";
        public const string CONST_STRING_SERVTYPESWOPCOLLECT = "6";
        public const string CONST_STRING_SERVTYPESWOPSTART = "7";
        public const string CONST_STRING_SERVTYPESWOPSTOP = "8";
        public const string CONST_STRING_SERVTYPESWOPRETRUN = "9";
        public const string CONST_STRING_SERVTYPESERVICE = "10";
        public const string CONST_STRING_SERVTYPE1WKHOLI = "11";
        public const string CONST_STRING_SERVTYPE2WKHOLI = "12";
        public const string CONST_STRING_SERVTYPE3WKHOLI = "13";
        public const string CONST_STRING_SERVTYPE1MTHHOLI = "14";
        public const string CONST_STRING_SERVTYPE6WKHOLI = "15";
        public const string CONST_STRING_SERVTYPE2MTHHOLI = "16";
        public const string CONST_STRING_SERVTYPENOTAPPLICABLE = "17";
        public const string CONST_STRING_SERVTYPEMAINTENANCE = "18";
        public const string CONST_STRING_SERVTYPEGREENBEAN = "19";
        public const string CONST_DESC_SERVTYPECLEANSTR = "Clean";
        public const string CONST_DESC_SERVTYPECOFFEESTR = "Coffee";
        public const string CONST_DESC_SERVTYPECOUNTSTR = "Count";
        public const string CONST_DESC_SERVTYPEDESCALESTR = "Descale";
        public const string CONST_DESC_SERVTYPEFILTERSTR = "Filter";
        public const string CONST_DESC_SERVTYPESWOPCOLLECTSTR = "SwopCollect";
        public const string CONST_DESC_SERVTYPESWOPSTARTSTR = "SwopStart";
        public const string CONST_DESC_SERVTYPESWOPSTOPSTR = "SwopStop";
        public const string CONST_DESC_SERVTYPESWOPRETURNSTR = "SwopReturn";
        public const string CONST_DESC_SERVTYPESERVICESTR = "Service";
        public const string CONST_DESC_SERVTYPENOTAPPLICABLE = "N/A";
        public const int CONST_TYPICALNUMCUPSPERKG = 100;
        public const double CONST_TYPICALAVECONSUMPTION = 5.0;
        public const double CONST_TYPICALCLEAN_CONSUMPTION = 200.0;
        public const double CONST_TYPICALDECAL_CONSUMPTION = 500.0;
        public const double CONST_TYPICALFILTER_CONSUMPTION = 300.0;
        public const int CONST_DEFAULT_DELIVERYBYID = 3;
        public const string CONST_DEFAULT_DELIVERYBYABBREVIATION = "SQ";
        public const int CONST_DEFAULT_DELIVERYIDOFCOURIER = 7;
        public const string CONST_DEFAULT_DELIVERYBYCOURIERABBREVIATION = "Cour";
        public static DateTime CONST_NULLDATE = DateTime.MinValue;
        public static DateTime STATIC_TrackerMinDate = DateTime.Parse("1980/01/01").Date;

        public int GetDaysToRoastDate(DateTime pThisDate)
        {
            DayOfWeek pRoastDayOfWeek = DayOfWeek.Tuesday;
            if (pThisDate.DayOfWeek == DayOfWeek.Tuesday && pThisDate.Hour >= 10 || ((pThisDate.DayOfWeek == DayOfWeek.Wednesday ? 1 : 0) | (pThisDate.DayOfWeek != DayOfWeek.Thursday ? 0 : (pThisDate.Hour < 10 ? 1 : 0))) != 0)
                pRoastDayOfWeek = DayOfWeek.Thursday;
            return this.GetDaysToRoastDate(pThisDate, pRoastDayOfWeek);
        }

        public int GetDaysToRoastDate(DateTime pThisDate, DayOfWeek pRoastDayOfWeek)
        {
            DayOfWeek dayOfWeek = pThisDate.DayOfWeek;
            if (pRoastDayOfWeek < DayOfWeek.Sunday || pRoastDayOfWeek > DayOfWeek.Saturday)
                pRoastDayOfWeek = DayOfWeek.Tuesday;
            int num = pRoastDayOfWeek - dayOfWeek;
            return dayOfWeek <= pRoastDayOfWeek ? 7 + num : 14 + num;
        }

        public int NumDaysTillNextRoast() => this.GetDaysToRoastDate(TimeZoneUtils.Now().Date);

        public int NumDaysTillNextRoast(DayOfWeek pRoastDayOfWeek)
        {
            return this.GetDaysToRoastDate(TimeZoneUtils.Now().Date, pRoastDayOfWeek);
        }

        public DateTime RemoveTimePortion(DateTime pDate) => pDate.Date;

        public DateTime GetClosestNextRoastDate(DateTime pThisDate)
        {
            return this.RemoveTimePortion(pThisDate.AddDays((double)(this.GetDaysToRoastDate(pThisDate) - 7)));
        }

        public DateTime GetClosestNextRoastDate(DateTime pThisDate, DayOfWeek pRoastDayOfWeek)
        {
            return this.RemoveTimePortion(pThisDate.AddDays((double)(this.GetDaysToRoastDate(pThisDate, pRoastDayOfWeek) - 7)));
        }

        public bool RoastDateIsBtw(DateTime pRoastDate) => this.RoastDateIsBtw(pRoastDate, 1L);

        public bool RoastDateIsBtw(DateTime pRoastDate, long pOrderId)
        {
            DateTime closestNextRoastDate1 = this.GetClosestNextRoastDate(TimeZoneUtils.Now().AddDays(-7.0), DayOfWeek.Monday);
            DateTime closestNextRoastDate2 = this.GetClosestNextRoastDate(TimeZoneUtils.Now().Date, DayOfWeek.Monday);
            return closestNextRoastDate1 <= pRoastDate && pRoastDate < closestNextRoastDate2;
        }

        public bool IsNextRoastDateByCityTodays()
        {
            bool flag = false;
            IDataReader dataReader = new TrackerDb().ExecuteSQLGetDataReader("SELECT DateLastPrepDateCalcd FROM SysDataTbl WHERE (ID = 1)");
            if (dataReader != null && dataReader.Read())
            {
                DateTime dateTime = TimeZoneUtils.Now().Date;
                if (dateTime.Hour >= 14)
                    dateTime = dateTime.AddDays(1.0);
                flag = dateTime.Date == Convert.ToDateTime(dataReader["DateLastPrepDateCalcd"].ToString()).Date;
                dataReader.Close();
            }
            return flag;
        }

        private string UpdateOrInsertCityNextRstDate(
          int pCityID,
          TrackerTools.PrepAndDeliveryData pThisPrepAndDeliveryData,
          TrackerTools.PrepAndDeliveryData pNextPrepAndDeliveryData)
        {
            string empty = string.Empty;
            NextRoastDateByCityTbl pNextRoastCityTbl = new NextRoastDateByCityTbl();
            pNextRoastCityTbl.CityID = (Int32)pCityID;
            pNextRoastCityTbl.PrepDate = pThisPrepAndDeliveryData.PrepDate;
            pNextRoastCityTbl.DeliveryDate = pThisPrepAndDeliveryData.DeliveryDate;
            pNextRoastCityTbl.DeliveryOrder = pThisPrepAndDeliveryData.SortOrder;
            pNextRoastCityTbl.NextPrepDate = pNextPrepAndDeliveryData.PrepDate;
            pNextRoastCityTbl.NextDeliveryDate = pNextPrepAndDeliveryData.DeliveryDate;
            TrackerDb trackerDb = new TrackerDb();
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader("SELECT CityID FROM NextRoastDateByCityTbl WHERE CityID = " + pCityID.ToString());
            string str;
            if (dataReader != null && dataReader.Read())
            {
                str = pNextRoastCityTbl.UpdatePrepDataForCity(pCityID, pNextRoastCityTbl);
                dataReader.Close();
            }
            else
                str = pNextRoastCityTbl.InsertPrepDataForCity(pNextRoastCityTbl);
            trackerDb.Close();
            return str;
        }

        private byte GetCorrectedDOW(byte pDOW) => pDOW == (byte)0 ? (byte)1 : (byte)((int)pDOW - 1);

        private TrackerTools.PrepAndDeliveryData GetPreAndDeliveryDate(
          int pIdx,
          int pCityID,
          List<CityPrepDaysTbl> pCityPrepDays,
          DateTime pForThisDate)
        {
            TrackerTools.PrepAndDeliveryData preAndDeliveryDate = new TrackerTools.PrepAndDeliveryData();
            byte _ThisDatesDOW = (byte)pForThisDate.DayOfWeek;
            int index1 = pCityPrepDays.FindIndex(pIdx, (Predicate<CityPrepDaysTbl>)(x => x.CityID == pCityID));
            if (index1 > -1)
            {
                byte correctedDow = this.GetCorrectedDOW(pCityPrepDays[index1].PrepDayOfWeekID);
                int deliveryDelayDays = pCityPrepDays[index1].DeliveryDelayDays;
                int deliveryOrder = pCityPrepDays[index1].DeliveryOrder;
                int index2 = pCityPrepDays.FindIndex(index1, (Predicate<CityPrepDaysTbl>)(x => x.CityID != pCityID));
                if (index2 > -1)
                {
                    int index3 = pCityPrepDays.FindIndex(index1, index2 - index1, (Predicate<CityPrepDaysTbl>)(x => (int)this.GetCorrectedDOW(x.PrepDayOfWeekID) >= (int)_ThisDatesDOW));
                    if (index3 > -1)
                    {
                        correctedDow = this.GetCorrectedDOW(pCityPrepDays[index3].PrepDayOfWeekID);
                        deliveryDelayDays = pCityPrepDays[index3].DeliveryDelayDays;
                        deliveryOrder = pCityPrepDays[index3].DeliveryOrder;
                    }
                }
                preAndDeliveryDate.PrepDate = (int)correctedDow < (int)_ThisDatesDOW ? pForThisDate.AddDays((double)(7 - (int)_ThisDatesDOW + (int)correctedDow)) : pForThisDate.AddDays((double)((int)correctedDow - (int)_ThisDatesDOW));
                preAndDeliveryDate.DeliveryDate = preAndDeliveryDate.PrepDate.AddDays((double)deliveryDelayDays);
                preAndDeliveryDate.SortOrder = deliveryOrder;
            }
            return preAndDeliveryDate;
        }

        public void SetNextRoastDateByCity()
        {
            List<CityPrepDaysTbl> all = new CityPrepDaysTbl().GetAll("CityID, PrepDayOfWeekID");
            DateTime minValue = DateTime.MinValue;
            TrackerTools.PrepAndDeliveryData prepAndDeliveryData1 = new TrackerTools.PrepAndDeliveryData();
            TrackerTools.PrepAndDeliveryData prepAndDeliveryData2 = new TrackerTools.PrepAndDeliveryData();
            DateTime pForThisDate1 = TimeZoneUtils.Now().Date;
            if (pForThisDate1.Hour >= 14)
                pForThisDate1 = pForThisDate1.AddDays(1.0);
            int num = 0;
        label_6:
            while (num < all.Count)
            {
                int cityId = all[num].CityID;
                TrackerTools.PrepAndDeliveryData preAndDeliveryDate1 = this.GetPreAndDeliveryDate(num, cityId, all, pForThisDate1);
                DateTime pForThisDate2 = preAndDeliveryDate1.PrepDate == preAndDeliveryDate1.DeliveryDate ? preAndDeliveryDate1.PrepDate.AddDays(1.0).Date : preAndDeliveryDate1.DeliveryDate.Date;
                TrackerTools.PrepAndDeliveryData preAndDeliveryDate2 = this.GetPreAndDeliveryDate(num, cityId, all, pForThisDate2);
                this.UpdateOrInsertCityNextRstDate(cityId, preAndDeliveryDate1, preAndDeliveryDate2);
                ++num;
                while (true)
                {
                    if (num < all.Count && cityId == all[num].CityID)
                        ++num;
                    else
                        goto label_6;
                }
            }
            TrackerDb trackerDb = new TrackerDb();
            trackerDb.ExecuteNonQuerySQLWithParams("UPDATE SysDataTbl SET DateLastPrepDateCalcd = ? WHERE ID=1", new List<DBParameter>()
    {
      new DBParameter()
      {
        DataValue = (object) TimeZoneUtils.Now().Date,
        DataDbType = DbType.Date
      }
    });
            trackerDb.Close();
        }

        public DateTime GetNextRoastDateByCustomerID(long pCustID, ref DateTime pDelivery)
        {
            if (!this.IsNextRoastDateByCityTodays())
                this.SetNextRoastDateByCity();
            NextRoastDateByCityTbl prepDataForCustomer = new NextRoastDateByCityTbl().GetPrepDataForCustomer(pCustID);
            pDelivery = prepDataForCustomer.DeliveryDate;
            return prepDataForCustomer.PrepDate;
        }

        public TrackerTools.ContactPreferedItems RetrieveCustomerPrefs(long _CustID)
        {
            TrackerDb trackerDb = new TrackerDb();
            TrackerTools.ContactPreferedItems contactPreferedItems = new TrackerTools.ContactPreferedItems(_CustID);
            string strSQL = "SELECT CustomersTbl.PreferedAgent, CustomersTbl.CoffeePreference, CustomersTbl.PriPrefQty, CustomersTbl.PrefPackagingID, CustomersAccInfoTbl.RequiresPurchOrder  FROM (CustomersTbl LEFT OUTER JOIN CustomersAccInfoTbl ON CustomersTbl.CustomerID = CustomersAccInfoTbl.CustomerID) WHERE CustomersTbl.CustomerID = " + _CustID.ToString();
            IDataReader dataReader = trackerDb.ExecuteSQLGetDataReader(strSQL);
            if (dataReader != null)
            {
                if (dataReader.Read())
                {
                    contactPreferedItems.PreferredDeliveryByID = dataReader["PreferedAgent"] == DBNull.Value ? 3 : (int)dataReader["PreferedAgent"];
                    contactPreferedItems.PreferedItem = dataReader["CoffeePreference"] == DBNull.Value ? 0 : (int)dataReader["CoffeePreference"];
                    contactPreferedItems.PreferedQty = dataReader["PriPrefQty"] == DBNull.Value ? 1.0 : Convert.ToDouble(dataReader["PriPrefQty"]);
                    contactPreferedItems.PrefPackagingID = dataReader["PrefPackagingID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["PrefPackagingID"]);
                    contactPreferedItems.RequiresPurchOrder = dataReader["RequiresPurchOrder"] == DBNull.Value ? false : (bool)dataReader["RequiresPurchOrder"];
                }
                dataReader.Close();
            }
            trackerDb.Close();
            return contactPreferedItems;
        }

        public void SetTrackerSessionErrorString(string pErrorString)
        {
            HttpContext current = HttpContext.Current;
            if (current == null || current.Session == null)
                return;
            current.Session["DataAccessError"] = (object)pErrorString;
        }

        public void ClearTrackerSessionErrorString()
        {
            HttpContext current = HttpContext.Current;
            if (current == null || current.Session == null)
                return;
            current.Session["DataAccessError"] = (object)string.Empty;
        }

        public string GetTrackerSessionErrorString()
        {
            HttpContext current = HttpContext.Current;
            string sessionErrorString = string.Empty;
            if (current != null && current.Session != null)
                sessionErrorString = current.Session["DataAccessError"] != null ? (string)current.Session["DataAccessError"] : string.Empty;
            return sessionErrorString;
        }

        public bool IsTrackerSessionErrorString()
        {
            HttpContext current = HttpContext.Current;
            return !string.IsNullOrWhiteSpace(current.Session["DataAccessError"] != null ? (string)current.Session["DataAccessError"] : string.Empty);
        }

        public int ChangeItemIfGroupToNextItemInGroup(
          long pContactID,
          int pItemTypeID,
          DateTime pDeliveryDate)
        {
            if (pContactID != -1L && pItemTypeID != -1)
            {
                SysDataTbl sysDataTbl = new SysDataTbl();
                if (new ItemTypeTbl().GetItemTypeFromID(pItemTypeID).ServiceTypeID == sysDataTbl.GetGroupItemTypeID())
                    pItemTypeID = new UsedItemGroupTbl().GetNextGroupItem(pContactID, pItemTypeID, pDeliveryDate).ItemTypeID;
            }
            return pItemTypeID;
        }

        public enum ServiceType
        {
            stNone,
            STypeClean,
            STypeCoffee,
            STypeCount,
            STypeDescale,
            STypeFilter,
            STypeSwopCollect,
            STypeSwopStart,
            STypeSwopStop,
            STypeSwopRetrun,
            STypeService,
            SType1WkHoli,
            SType2WkHoli,
            SType3WkHoli,
            SType1MthHoli,
            SType6WkHoli,
            SType2MthHoli,
        }

        public class ContactPreferedItems
        {
            private long _CustID;
            private int _PreferredDeliveryByID;
            private int _PreferedItem;
            private double _PreferedQty;
            private int _PrefPackagingID;
            private bool _RequiresPurchOrder;

            public ContactPreferedItems(long pCustID)
            {
                this._CustID = pCustID;
                this._PreferredDeliveryByID = 3;
                this._PreferedItem = 0;
                this._RequiresPurchOrder = false;
                this._PreferedQty = 1.0;
                this._PrefPackagingID = -1;
            }

            public long CustID
            {
                get => this._CustID;
                set => this._CustID = value;
            }

            public int PreferredDeliveryByID
            {
                get => this._PreferredDeliveryByID;
                set => this._PreferredDeliveryByID = value;
            }

            public int PreferedItem
            {
                get => this._PreferedItem;
                set => this._PreferedItem = value;
            }

            public double PreferedQty
            {
                get => this._PreferedQty;
                set => this._PreferedQty = value;
            }

            public int PrefPackagingID
            {
                get => this._PrefPackagingID;
                set => this._PrefPackagingID = value;
            }

            public bool RequiresPurchOrder
            {
                get => this._RequiresPurchOrder;
                set => this._RequiresPurchOrder = value;
            }
        }

        public class PrepAndDeliveryData
        {
            private DateTime _PrepDate;
            private DateTime _DeliveryDate;
            private int _SortOrder;

            public PrepAndDeliveryData()
            {
                this._PrepDate = this._DeliveryDate = DateTime.MinValue;
                this._SortOrder = 0;
            }

            public DateTime PrepDate
            {
                get => this._PrepDate;
                set => this._PrepDate = value;
            }

            public DateTime DeliveryDate
            {
                get => this._DeliveryDate;
                set => this._DeliveryDate = value;
            }

            public int SortOrder
            {
                get => this._SortOrder;
                set => this._SortOrder = value;
            }
        }
    }
}
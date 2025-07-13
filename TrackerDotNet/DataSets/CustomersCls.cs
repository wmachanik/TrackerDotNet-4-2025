// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.DataSets.CustomersCls
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

// #nullable disable --- not for this version of C#
namespace TrackerDotNet.DataSets
{

    public class CustomersCls
    {
        private int m_CustomerID;
        private string m_CompanyName;
        private string m_ContactTitle;
        private string m_ContactFirstName;
        private string m_ContactLastName;
        private string m_ContactAltFirstName;
        private string m_ContactAltLastName;
        private string m_Department;
        private string m_BillingAddress;
        private string m_StateOrProvince;
        private string m_PostalCode;
        private string m_PhoneNumber;
        private string m_Extension;
        private string m_FaxNumber;
        private string m_CellNumber;
        private string m_EmailAddress;
        private string m_AltEmailAddress;
        private string m_CustomerType;
        private int m_EquipTypeName;
        private int m_CoffeePreference;
        private int m_City;
        private int m_PriPref;
        private int m_SecPref;
        private double m_PriPrefQty;
        private double m_SecPrefQty;
        private int m_Abreviation;
        private string m_MachineSN;
        private bool m_UsesFilter;
        private bool m_Autofulfill;
        private bool m_Enabled;
        private bool m_PredictionDisabled;
        private bool m_AlwaysSendChkUp;
        private bool m_NormallyResponds;
        private string m_Notes;

        public CustomersCls()
        {
            this.m_CustomerID = 0;
            this.m_CompanyName = this.m_ContactTitle = this.m_ContactFirstName = this.m_ContactLastName = this.m_ContactAltFirstName = this.m_ContactAltLastName = this.m_Department = this.m_BillingAddress = this.m_StateOrProvince = this.m_PostalCode = this.m_PhoneNumber = this.m_Extension = this.m_FaxNumber = this.m_CellNumber = this.m_EmailAddress = this.m_AltEmailAddress = this.m_CustomerType = "";
            this.m_EquipTypeName = this.m_CoffeePreference = this.m_City = this.m_PriPref = this.m_SecPref = 0;
            this.m_PriPrefQty = this.m_SecPrefQty = 0.0;
            this.m_Abreviation = 0;
            this.m_MachineSN = "";
            this.m_Enabled = true;
            this.m_UsesFilter = this.m_Autofulfill = this.m_PredictionDisabled = this.m_AlwaysSendChkUp = this.m_NormallyResponds = false;
            this.m_Notes = "";
        }

        public int CustomerID
        {
            get => this.m_CustomerID;
            set => this.m_CustomerID = value;
        }

        public string CompanyName
        {
            get => this.m_CompanyName;
            set => this.m_CompanyName = value;
        }

        public string ContactTitle
        {
            get => this.m_ContactTitle;
            set => this.m_ContactTitle = value;
        }

        public string ContactFirstName
        {
            get => this.m_ContactFirstName;
            set => this.m_ContactFirstName = value;
        }

        public string ContactLastName
        {
            get => this.m_ContactLastName;
            set => this.m_ContactLastName = value;
        }

        public string ContactAltFirstName
        {
            get => this.m_ContactAltFirstName;
            set => this.m_ContactAltFirstName = value;
        }

        public string ContactAltLastName
        {
            get => this.m_ContactAltLastName;
            set => this.m_ContactAltLastName = value;
        }

        public string Department
        {
            get => this.m_Department;
            set => this.m_Department = value;
        }

        public string BillingAddress
        {
            get => this.m_BillingAddress;
            set => this.m_BillingAddress = value;
        }

        public string StateOrProvince
        {
            get => this.m_StateOrProvince;
            set => this.m_StateOrProvince = value;
        }

        public string PostalCode
        {
            get => this.m_PostalCode;
            set => this.m_PostalCode = value;
        }

        public string PhoneNumber
        {
            get => this.m_PhoneNumber;
            set => this.m_PhoneNumber = value;
        }

        public string Extension
        {
            get => this.m_Extension;
            set => this.m_Extension = value;
        }

        public string FaxNumber
        {
            get => this.m_FaxNumber;
            set => this.m_FaxNumber = value;
        }

        public string CellNumber
        {
            get => this.m_CellNumber;
            set => this.m_CellNumber = value;
        }

        public string EmailAddress
        {
            get => this.m_EmailAddress;
            set => this.m_EmailAddress = value;
        }

        public string AltEmailAddress
        {
            get => this.m_AltEmailAddress;
            set => this.m_AltEmailAddress = value;
        }

        public string CustomerType
        {
            get => this.m_CustomerType;
            set => this.m_CustomerType = value;
        }

        public int EquipTypeName
        {
            get => this.m_EquipTypeName;
            set => this.m_EquipTypeName = value;
        }

        public int CoffeePreference
        {
            get => this.m_CoffeePreference;
            set => this.m_CoffeePreference = value;
        }

        public int City
        {
            get => this.m_City;
            set => this.m_City = value;
        }

        public int PriPref
        {
            get => this.m_PriPref;
            set => this.m_PriPref = value;
        }

        public int SecPref
        {
            get => this.m_SecPref;
            set => this.m_SecPref = value;
        }

        public double PriPrefQty
        {
            get => this.m_PriPrefQty;
            set => this.m_PriPrefQty = value;
        }

        public double SecPrefQty
        {
            get => this.m_SecPrefQty;
            set => this.m_SecPrefQty = value;
        }

        public int Abreviation
        {
            get => this.m_Abreviation;
            set => this.m_Abreviation = value;
        }

        public string MachineSN
        {
            get => this.m_MachineSN;
            set => this.m_MachineSN = value;
        }

        public bool UsesFilter
        {
            get => this.m_UsesFilter;
            set => this.m_UsesFilter = value;
        }

        public bool Autofulfill
        {
            get => this.m_Autofulfill;
            set => this.m_Autofulfill = value;
        }

        public bool Enabled
        {
            get => this.m_Enabled;
            set => this.m_Enabled = value;
        }

        public bool PredictionDisabled
        {
            get => this.m_PredictionDisabled;
            set => this.m_PredictionDisabled = value;
        }

        public bool AlwaysSendChkUp
        {
            get => this.m_AlwaysSendChkUp;
            set => this.m_AlwaysSendChkUp = value;
        }

        public bool NormallyResponds
        {
            get => this.m_NormallyResponds;
            set => this.m_NormallyResponds = value;
        }

        public string Notes
        {
            get => this.m_Notes;
            set => this.m_Notes = value;
        }
    }
}

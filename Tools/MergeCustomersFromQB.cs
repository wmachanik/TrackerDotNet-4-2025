// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Tools.MergeCustomersFromQB
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.classes;
using TrackerDotNet.control;

#nullable disable
namespace TrackerDotNet.Tools;

public class MergeCustomersFromQB : Page
{
  private const string CONST_LOGFILENAME = "~/App_Data/MergeCustomers.log";
  private LogFile _LogFile;
  private List<MergeCustomersFromQB.PaymentTermTranslor> _PaymentTermTranslors;
  private List<MergeCustomersFromQB.PriceLevelTranslor> _PriceLevelTranslors;
  private List<MergeCustomersFromQB.AreaToCityMap> _AreaToCityMap;
  protected FileUpload MergeFileUpload;
  protected DropDownList StartDropDownList;
  protected DropDownList FinishDropDownList;
  protected DropDownList MaxRecsDropDownList;
  protected Button SelectImportFileButton;
  protected Label StatusLabel;
  protected GridView gvCustomers;

  protected void Page_Load(object sender, EventArgs e)
  {
    if (this.IsPostBack)
      return;
    char ch = '0';
    ch.ToString();
    for (; ch <= 'Z'; ++ch)
    {
      string str = ch.ToString();
      this.StartDropDownList.Items.Add(new ListItem()
      {
        Text = str,
        Value = str
      });
      this.FinishDropDownList.Items.Add(new ListItem()
      {
        Text = str,
        Value = str
      });
    }
    this.StartDropDownList.SelectedIndex = 0;
    this.FinishDropDownList.SelectedIndex = this.FinishDropDownList.Items.Count - 1;
    for (int index = 0; index < 500; index += 25)
      this.MaxRecsDropDownList.Items.Add(index.ToString());
  }

  private DataTable MergeFileWithData(string pFileName)
  {
    DataTable dataTable = new DataTable();
    string[] strArray1 = File.ReadAllLines(pFileName);
    int num = 0;
    foreach (string str in strArray1)
    {
      if (!str.StartsWith("!CUSTNAMEDICT") && str.StartsWith("!CUST"))
      {
        string[] strArray2 = str.Split('\t');
        num = strArray2.Length;
        foreach (string columnName in strArray2)
          dataTable.Columns.Add(columnName);
      }
      if (!str.StartsWith("CUSTNAMEDICT") && str.StartsWith("CUST"))
      {
        string[] strArray3 = str.Split('\t');
        if (strArray3.Length == num)
          dataTable.Rows.Add((object[]) strArray3);
      }
    }
    return dataTable;
  }

  private DataTable ReadLogFileData(string pFileName)
  {
    DataTable dataTable = new DataTable();
    string[] strArray1 = File.ReadAllLines(pFileName);
    int num = 2;
    foreach (string str in strArray1)
    {
      char[] chArray = new char[1]{ ',' };
      string[] strArray2 = str.Split(chArray);
      if (strArray2.Length > num)
        num = strArray2.Length;
    }
    dataTable.Columns.Add("Date");
    for (int index = 1; index < num; ++index)
      dataTable.Columns.Add("Desc " + index.ToString());
    foreach (string str in strArray1)
      dataTable.Rows.Add((object[]) str.Split(','));
    return dataTable;
  }

  private List<MergeCustomersFromQB.PaymentTermTranslor> GetPaymentTermList()
  {
    List<MergeCustomersFromQB.PaymentTermTranslor> paymentTermList = new List<MergeCustomersFromQB.PaymentTermTranslor>();
    paymentTermList.Add(new MergeCustomersFromQB.PaymentTermTranslor()
    {
      QBPaymentTermDesc = "Due on receipt",
      QonTPaymentTermDesc = "Due on receipt"
    });
    paymentTermList.Add(new MergeCustomersFromQB.PaymentTermTranslor()
    {
      QBPaymentTermDesc = "MidMonth",
      QonTPaymentTermDesc = "MidMonth"
    });
    paymentTermList.Add(new MergeCustomersFromQB.PaymentTermTranslor()
    {
      QBPaymentTermDesc = "Net 15",
      QonTPaymentTermDesc = "Net 15"
    });
    paymentTermList.Add(new MergeCustomersFromQB.PaymentTermTranslor()
    {
      QBPaymentTermDesc = "Net 30",
      QonTPaymentTermDesc = "Net 30"
    });
    paymentTermList.Add(new MergeCustomersFromQB.PaymentTermTranslor()
    {
      QBPaymentTermDesc = "OnStatement",
      QonTPaymentTermDesc = "OnStatement"
    });
    foreach (MergeCustomersFromQB.PaymentTermTranslor paymentTermTranslor in paymentTermList)
      paymentTermTranslor.QonTPaymentTermID = paymentTermTranslor.GetQonTPaymentTermIDByDesc(paymentTermTranslor.QonTPaymentTermDesc);
    paymentTermList.Sort((Comparison<MergeCustomersFromQB.PaymentTermTranslor>) ((x, y) => x.QBPaymentTermDesc.CompareTo(y.QBPaymentTermDesc)));
    return paymentTermList;
  }

  private List<MergeCustomersFromQB.PriceLevelTranslor> GetPriceLevelList()
  {
    List<MergeCustomersFromQB.PriceLevelTranslor> priceLevelList = new List<MergeCustomersFromQB.PriceLevelTranslor>();
    priceLevelList.Add(new MergeCustomersFromQB.PriceLevelTranslor()
    {
      QBPriceLevelDesc = "Standard",
      QonTPriceLevelDesc = "Standard"
    });
    priceLevelList.Add(new MergeCustomersFromQB.PriceLevelTranslor()
    {
      QBPriceLevelDesc = "Family/Charity/Agent",
      QonTPriceLevelDesc = "Family/Charity/Agent"
    });
    priceLevelList.Add(new MergeCustomersFromQB.PriceLevelTranslor()
    {
      QBPriceLevelDesc = "COD Discount",
      QonTPriceLevelDesc = "Discount 5%"
    });
    priceLevelList.Add(new MergeCustomersFromQB.PriceLevelTranslor()
    {
      QBPriceLevelDesc = "RRP",
      QonTPriceLevelDesc = "RRP"
    });
    priceLevelList.Add(new MergeCustomersFromQB.PriceLevelTranslor()
    {
      QBPriceLevelDesc = "CreditCard",
      QonTPriceLevelDesc = "RRP"
    });
    priceLevelList.Add(new MergeCustomersFromQB.PriceLevelTranslor()
    {
      QBPriceLevelDesc = "Dealer",
      QonTPriceLevelDesc = "Dealer"
    });
    priceLevelList.Add(new MergeCustomersFromQB.PriceLevelTranslor()
    {
      QBPriceLevelDesc = "Reseller",
      QonTPriceLevelDesc = "Dealer"
    });
    priceLevelList.Add(new MergeCustomersFromQB.PriceLevelTranslor()
    {
      QBPriceLevelDesc = "Farm",
      QonTPriceLevelDesc = "Host"
    });
    priceLevelList.Add(new MergeCustomersFromQB.PriceLevelTranslor()
    {
      QBPriceLevelDesc = "Pensioner",
      QonTPriceLevelDesc = "Pensioner"
    });
    priceLevelList.Add(new MergeCustomersFromQB.PriceLevelTranslor()
    {
      QBPriceLevelDesc = "Price With Dlvry",
      QonTPriceLevelDesc = "Standard"
    });
    priceLevelList.Add(new MergeCustomersFromQB.PriceLevelTranslor()
    {
      QBPriceLevelDesc = "Remote Agents",
      QonTPriceLevelDesc = "RemoteAgent"
    });
    foreach (MergeCustomersFromQB.PriceLevelTranslor priceLevelTranslor in priceLevelList)
      priceLevelTranslor.QonTPriceLevelID = priceLevelTranslor.GetQonTPriceLevelIDByDesc(priceLevelTranslor.QonTPriceLevelDesc);
    priceLevelList.Sort((Comparison<MergeCustomersFromQB.PriceLevelTranslor>) ((x, y) => x.QBPriceLevelDesc.CompareTo(y.QBPriceLevelDesc)));
    return priceLevelList;
  }

  private List<MergeCustomersFromQB.AreaToCityMap> MapAreasToCityID()
  {
    return new List<MergeCustomersFromQB.AreaToCityMap>()
    {
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Atlantic Seaboard",
        CityID = 9
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Benoni",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Bloem",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Bloemfontein",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town CBD",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: CBD",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: Near CBD",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: Northern Suburbs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: NSuburbs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: Peninsula",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: S.suburbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: Southern",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: Southern Peninsula",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: Southern Penisula",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: Southern Suburbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: SSuburbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape town: SSurbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: Stellenbosch",
        CityID = 3
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town: Woodstock",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Atlantic Seaboard",
        CityID = 9
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Belville",
        CityID = 15
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:CBD",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Constantia",
        CityID = 11
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Hout Bay",
        CityID = 20
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:HoutBay",
        CityID = 20
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Northern",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Northern Sunurbs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Northern Surburbs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:NSubrubs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:NSuburbs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Paardien Eiland",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Parow",
        CityID = 15
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Somerset",
        CityID = 4
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Southern Peninsula",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Southern Suburbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:SSubrbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:SSuburbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cape Town:Town2Milnerton",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CBD: Milnerton",
        CityID = 24
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CBD:Fhk",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Constantia",
        CityID = 11
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: / Tanzaina",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Atlantic Seaboard",
        CityID = 9
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Beliville",
        CityID = 15
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Belville",
        CityID = 15
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Bishops court",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Brackenfell",
        CityID = 15
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: CBD",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: CDB",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Central",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Century Cty",
        CityID = 12
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Claremont",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Const",
        CityID = 11
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Constantia",
        CityID = 11
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cpt: Constnaita",
        CityID = 11
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Durbanville",
        CityID = 16 /*0x10*/
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: E[[ing",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Epping",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Fhk",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Fish hoek",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Fshk",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Gardens",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: HBay",
        CityID = 20
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Hourbay",
        CityID = 20
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cpt: Hout Bay",
        CityID = 20
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: HoutBay",
        CityID = 20
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Kenilworth",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Kuilsriver",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Kuilsrivier",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Melkbos",
        CityID = 26
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: MID",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Milnerton",
        CityID = 24
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Montague Gardens",
        CityID = 12
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Mowbray",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Muizenberg",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Muizenbrg",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: N. Subs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: N/Suburbs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Newlands",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Noordhoek",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cpt: North",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: North Suburbs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cpt: Northern Suburbs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: NSubrbs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: NSurbs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Paarden",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Peninsula",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Pinelands",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "cpt: plumstead",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Rndbsh",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Rondebosch",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Seapoint",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Simonstown",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: SothSurbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: South Suburbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Southern Pen",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Southern Subrubs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Southern Suburbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: SouthernS",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cpt: SouthernSurbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: SoutherS",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: SoutherSubrbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: SSburbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: SSubrbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: SSuburb",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: SSuburbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: SSurbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: SSurubs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Sthrn",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: SthrnSbrs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: SthrnSubrubs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Toaki",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Tokai",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Town",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Walkin",
        CityID = 11
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Cpt: Westake2Muizenberg",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Westlake",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Westlk",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Woodstock",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT: Wynberg",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "cpt:: Belville",
        CityID = 15
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT:CBD",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT:Cllct",
        CityID = 11
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT:Const",
        CityID = 11
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT:Constantia",
        CityID = 11
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT:Noordhoek",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CPT:SSubrbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CT Ssuburbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CT: CBD",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "CTP: SSurbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Durban",
        CityID = 14
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "East London",
        CityID = 13
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "George",
        CityID = 19
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "GordonsBay",
        CityID = 4
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Grahamstown",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Hermanus",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Hout Bay",
        CityID = 20
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Jhb",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Jhb: East",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Jhb: Honeydew",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Jhb: Midrand",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "JHB: Obs",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Jhb: Sandown",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Jhb: Sandton",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Jhb:Edenvale",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Jhb:Randburg",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Johannesberg",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Johannesburg",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Johannesburg: Alberton",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Johannesburg: Central West",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Johannesburg: Kempton",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Johannesburg: Midrand",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Johannesburg: North",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Johannesburg: Rosebank",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Johannesburg:Randburg",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Kakamas",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Kempton Park",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Krugersdorp",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "KZN:Vryheid",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Melkbos",
        CityID = 26
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Midrand",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Mpumalanga",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Northern Suburbs",
        CityID = 10
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "other",
        CityID = 1
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "PE",
        CityID = 28
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Phalaborwa",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "PMB",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Port Alfred",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Potch",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Pretoria",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Pretoria:Centrurion",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "PTA",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Regional",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Regional: Agulus",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "RegionalSA",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Rhodes",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Rustenberg",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Sandton",
        CityID = 2
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Secunda",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Somerset",
        CityID = 4
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Somerset West",
        CityID = 4
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "South Suburtbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Souther Suburbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Southern Suburbs",
        CityID = 8
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Stellenbosch",
        CityID = 3
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Stellenbosch / Paarl",
        CityID = 3
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Tulbach",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Vereeniging",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Vryheid",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "walkin",
        CityID = 11
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Welkom",
        CityID = 5
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Westlake",
        CityID = 6
      },
      new MergeCustomersFromQB.AreaToCityMap()
      {
        Area = "Witbank",
        CityID = 5
      }
    };
  }

  private List<CustomersTbl> GetAllCustomersWithEmail(string pEmailAddress, out string pEmailFound)
  {
    CustomersTbl customersTbl = new CustomersTbl();
    List<CustomersTbl> customersWithEmail = new List<CustomersTbl>();
    pEmailFound = string.Empty;
    if (!string.IsNullOrEmpty(pEmailAddress))
    {
      string[] strArray = pEmailAddress.Split(',');
      for (int index = 0; index < strArray.Length && customersWithEmail.Count == 0; ++index)
      {
        customersWithEmail = customersTbl.GetAllCustomerWithEmailLIKE(strArray[index]);
        if (customersWithEmail.Count > 0)
          pEmailFound = strArray[index];
      }
    }
    return customersWithEmail;
  }

  private bool CustomerExists(ref CustomersTbl pCustomer)
  {
    bool flag = false;
    long num = 0;
    string str = pCustomer.CompanyName.Contains("(") ? pCustomer.CompanyName.Substring(0, pCustomer.CompanyName.IndexOf("(") - 1) : pCustomer.CompanyName;
    List<CustomersTbl> customersTblList = pCustomer.GetAllCustomerWithNameLIKE(str + "%");
    if (customersTblList.Count > 0)
    {
      int index = 0;
      while (index < customersTblList.Count && !flag)
      {
        if (customersTblList[index].CustomerID > 0L)
        {
          if (customersTblList[index].EmailAddress == pCustomer.EmailAddress || customersTblList[index].AltEmailAddress == pCustomer.AltEmailAddress)
            flag = true;
          else if (customersTblList[index].AltEmailAddress == pCustomer.AltEmailAddress)
          {
            flag = true;
            pCustomer.EmailAddress = customersTblList[index].EmailAddress;
            pCustomer.AltEmailAddress = customersTblList[index].AltEmailAddress;
          }
          else if (customersTblList[index].ContactFirstName == pCustomer.ContactFirstName || customersTblList[index].ContactLastName == pCustomer.ContactLastName)
            flag = true;
          else if (customersTblList[index].ContactAltFirstName == pCustomer.ContactFirstName)
          {
            flag = true;
            pCustomer.ContactFirstName = customersTblList[index].ContactFirstName;
            pCustomer.ContactAltFirstName = customersTblList[index].ContactAltFirstName;
            pCustomer.ContactLastName = customersTblList[index].ContactLastName;
            pCustomer.ContactAltLastName = customersTblList[index].ContactAltLastName;
          }
        }
        if (flag)
          num = customersTblList[index].CustomerID;
        else
          ++index;
      }
    }
    if (!flag)
    {
      if (customersTblList.Count > 0)
        customersTblList.Clear();
      string pEmailFound = string.Empty;
      customersTblList = this.GetAllCustomersWithEmail(pCustomer.EmailAddress, out pEmailFound);
      if (customersTblList.Count > 0)
        pCustomer.EmailAddress = pEmailFound;
      else if (!string.IsNullOrEmpty(pCustomer.AltEmailAddress))
      {
        customersTblList = pCustomer.GetAllCustomerWithEmailLIKE(pCustomer.AltEmailAddress);
        if (customersTblList.Count > 0)
          pCustomer.AltEmailAddress = pEmailFound;
      }
      int index = 0;
      while (index < customersTblList.Count && !flag)
      {
        if (!string.IsNullOrEmpty(customersTblList[index].EmailAddress) && customersTblList[index].EmailAddress == pCustomer.EmailAddress)
          flag = true;
        else if (!string.IsNullOrEmpty(customersTblList[index].AltEmailAddress) && customersTblList[index].AltEmailAddress == pCustomer.AltEmailAddress)
          flag = true;
        else if (!string.IsNullOrEmpty(customersTblList[index].ContactFirstName) && customersTblList[index].ContactFirstName == pCustomer.ContactFirstName)
          flag = true;
        else if (!string.IsNullOrEmpty(customersTblList[index].ContactLastName) && customersTblList[index].ContactLastName == pCustomer.ContactLastName)
          flag = true;
        else if (!string.IsNullOrEmpty(customersTblList[index].ContactFirstName) && customersTblList[index].ContactAltFirstName == pCustomer.ContactFirstName)
        {
          flag = true;
          pCustomer.ContactFirstName = customersTblList[index].ContactFirstName;
          pCustomer.ContactAltFirstName = customersTblList[index].ContactAltFirstName;
          pCustomer.ContactLastName = customersTblList[index].ContactLastName;
          pCustomer.ContactAltLastName = customersTblList[index].ContactAltLastName;
        }
        if (flag)
          num = customersTblList[index].CustomerID;
        else
          ++index;
      }
    }
    if (flag)
      pCustomer.CustomerID = num;
    if (customersTblList.Count > 0)
      customersTblList.Clear();
    return flag;
  }

  private int GetPresonIDFromRep(string pAbreviation)
  {
    return new PersonsTbl().PersonsIDFromAbreviation(pAbreviation);
  }

  private int FindCity(string pArea, string pShipLines)
  {
    int city = 1;
    if (pArea.Trim().Length > 0 && this._AreaToCityMap.Exists((Predicate<MergeCustomersFromQB.AreaToCityMap>) (x => x.Area.Contains(pArea))))
    {
      city = this._AreaToCityMap.Find((Predicate<MergeCustomersFromQB.AreaToCityMap>) (x => x.Area.Contains(pArea))).CityID;
    }
    else
    {
      char[] chArray = new char[2]{ ' ', '\t' };
      string[] _AreaFirstName = pShipLines.Split(chArray);
      int i = 0;
      for (bool flag = false; !flag && i < _AreaFirstName.Length; ++i)
      {
        if (this._AreaToCityMap.Exists((Predicate<MergeCustomersFromQB.AreaToCityMap>) (x => x.Area.Contains(_AreaFirstName[i]))))
        {
          city = this._AreaToCityMap.Find((Predicate<MergeCustomersFromQB.AreaToCityMap>) (x => x.Area.Contains(_AreaFirstName[i]))).CityID;
          flag = true;
        }
      }
    }
    return city;
  }

  private double GetDoubleFromString(string pStr)
  {
    double result = 0.0;
    if (!string.IsNullOrEmpty(pStr))
    {
      pStr = pStr.Replace("\"", string.Empty);
      if (!double.TryParse(pStr, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        result = 0.0;
    }
    return result;
  }

  private CustomersAccInfoTbl GetCustAccInfoFromDataRow(DataRow pDataRow)
  {
    CustomersAccInfoTbl accInfoFromDataRow = new CustomersAccInfoTbl();
    accInfoFromDataRow.BillAddr1 = pDataRow["BADDR1"] == null ? string.Empty : pDataRow["BADDR1"].ToString();
    accInfoFromDataRow.BillAddr2 = pDataRow["BADDR2"] == null ? string.Empty : pDataRow["BADDR2"].ToString();
    accInfoFromDataRow.BillAddr3 = pDataRow["BADDR3"] == null ? string.Empty : pDataRow["BADDR3"].ToString();
    accInfoFromDataRow.BillAddr4 = pDataRow["BADDR4"] == null ? string.Empty : pDataRow["BADDR4"].ToString();
    accInfoFromDataRow.BillAddr5 = pDataRow["BADDR5"] == null ? string.Empty : pDataRow["BADDR5"].ToString();
    accInfoFromDataRow.ShipAddr1 = pDataRow["SADDR1"] == null ? string.Empty : pDataRow["SADDR1"].ToString();
    accInfoFromDataRow.ShipAddr2 = pDataRow["SADDR2"] == null ? string.Empty : pDataRow["SADDR2"].ToString();
    accInfoFromDataRow.ShipAddr3 = pDataRow["SADDR3"] == null ? string.Empty : pDataRow["SADDR3"].ToString();
    accInfoFromDataRow.ShipAddr4 = pDataRow["SADDR4"] == null ? string.Empty : pDataRow["SADDR4"].ToString();
    accInfoFromDataRow.ShipAddr5 = pDataRow["SADDR5"] == null ? string.Empty : pDataRow["SADDR5"].ToString();
    accInfoFromDataRow.AccEmail = pDataRow["EMAIL"] == null ? string.Empty : pDataRow["EMAIL"].ToString();
    string _PayTerms = pDataRow["TERMS"] == null ? string.Empty : pDataRow["TERMS"].ToString();
    if (this._PaymentTermTranslors.Exists((Predicate<MergeCustomersFromQB.PaymentTermTranslor>) (x => x.QBPaymentTermDesc.Equals(_PayTerms))))
    {
      MergeCustomersFromQB.PaymentTermTranslor paymentTermTranslor = this._PaymentTermTranslors.Find((Predicate<MergeCustomersFromQB.PaymentTermTranslor>) (x => x.QBPaymentTermDesc.Equals(_PayTerms)));
      accInfoFromDataRow.PaymentTermID = paymentTermTranslor.QonTPaymentTermID;
    }
    accInfoFromDataRow.Limit = pDataRow["LIMIT"] == null ? 0.0 : this.GetDoubleFromString(pDataRow["LIMIT"].ToString());
    accInfoFromDataRow.CustomerVATNo = pDataRow["RESALENUM"] == null ? string.Empty : pDataRow["RESALENUM"].ToString();
    accInfoFromDataRow.Notes = pDataRow["REP"] == null ? string.Empty : $"REP: {pDataRow["REP"].ToString()}{Environment.NewLine}";
    accInfoFromDataRow.FullCoName = pDataRow["COMPANYNAME"] == null ? string.Empty : pDataRow["COMPANYNAME"].ToString();
    accInfoFromDataRow.AccFirstName = pDataRow["FIRSTNAME"] == null ? string.Empty : pDataRow["FIRSTNAME"].ToString();
    accInfoFromDataRow.AccLastName = pDataRow["LASTNAME"] == null ? string.Empty : pDataRow["LASTNAME"].ToString();
    if (pDataRow["MIDINIT"] != null)
    {
      string str = pDataRow["MIDINIT"].ToString();
      if (str.Equals("le") || str.Equals("vd") || str.Equals("van"))
        accInfoFromDataRow.AccLastName = $"{str} {accInfoFromDataRow.AccLastName}";
    }
    if (pDataRow["CUSTFLD1"] != null && !string.IsNullOrEmpty(pDataRow["CUSTFLD1"].ToString()))
      accInfoFromDataRow.Notes += $"Area: {pDataRow["CUSTFLD1"].ToString()}{Environment.NewLine}";
    accInfoFromDataRow.RegNo = pDataRow["CUSTFLD2"] == null ? string.Empty : pDataRow["CUSTFLD2"].ToString();
    accInfoFromDataRow.BankAccNo = pDataRow["CUSTFLD3"] == null ? string.Empty : pDataRow["CUSTFLD3"].ToString();
    accInfoFromDataRow.BankBranch = pDataRow["CUSTFLD4"] == null ? string.Empty : pDataRow["CUSTFLD4"].ToString();
    if (pDataRow["CUSTFLD5"] != null && !string.IsNullOrEmpty(pDataRow["CUSTFLD5"].ToString()))
      accInfoFromDataRow.Notes += $"SN: {pDataRow["CUSTFLD5"].ToString()}{Environment.NewLine}";
    accInfoFromDataRow.Enabled = pDataRow["HIDDEN"] == null || pDataRow["HIDDEN"].ToString() == "Y";
    string _PriceLevel = pDataRow["PRICELEVEL"] == null ? string.Empty : pDataRow["PRICELEVEL"].ToString();
    if (this._PriceLevelTranslors.Exists((Predicate<MergeCustomersFromQB.PriceLevelTranslor>) (x => x.QBPriceLevelDesc.Equals(_PriceLevel))))
    {
      MergeCustomersFromQB.PriceLevelTranslor priceLevelTranslor = this._PriceLevelTranslors.Find((Predicate<MergeCustomersFromQB.PriceLevelTranslor>) (x => x.QBPriceLevelDesc.Equals(_PriceLevel)));
      accInfoFromDataRow.PaymentTermID = priceLevelTranslor.QonTPriceLevelID;
    }
    return accInfoFromDataRow;
  }

  private void AddAccountInfoToCust(CustomersTbl pCustomer, DataRow pDataRow)
  {
    if (pCustomer.CustomerID > 0L)
    {
      CustomersAccInfoTbl accInfoFromDataRow = this.GetCustAccInfoFromDataRow(pDataRow);
      accInfoFromDataRow.CustomerID = pCustomer.CustomerID;
      CustomersAccInfoTbl byCustomerId = accInfoFromDataRow.GetByCustomerID(pCustomer.CustomerID);
      if (byCustomerId.CustomersAccInfoID > 0)
      {
        this._LogFile.AddFormatStringToLog(", Account Info for Customer: {0} exists, adding what is not there", (object) pCustomer.CompanyName);
        accInfoFromDataRow.CustomersAccInfoID = byCustomerId.CustomersAccInfoID;
        accInfoFromDataRow.CustomerVATNo = byCustomerId.CustomerVATNo == string.Empty ? accInfoFromDataRow.CustomerVATNo : string.Empty;
        accInfoFromDataRow.BillAddr1 = byCustomerId.BillAddr1 == string.Empty ? accInfoFromDataRow.BillAddr1 : byCustomerId.BillAddr1;
        accInfoFromDataRow.BillAddr2 = byCustomerId.BillAddr2 == string.Empty ? accInfoFromDataRow.BillAddr2 : byCustomerId.BillAddr2;
        accInfoFromDataRow.BillAddr3 = byCustomerId.BillAddr3 == string.Empty ? accInfoFromDataRow.BillAddr3 : byCustomerId.BillAddr3;
        accInfoFromDataRow.BillAddr4 = byCustomerId.BillAddr4 == string.Empty ? accInfoFromDataRow.BillAddr4 : byCustomerId.BillAddr4;
        accInfoFromDataRow.BillAddr5 = byCustomerId.BillAddr5 == string.Empty ? accInfoFromDataRow.BillAddr5 : byCustomerId.BillAddr5;
        accInfoFromDataRow.ShipAddr1 = byCustomerId.ShipAddr1 == string.Empty ? accInfoFromDataRow.ShipAddr1 : byCustomerId.ShipAddr1;
        accInfoFromDataRow.ShipAddr2 = byCustomerId.ShipAddr2 == string.Empty ? accInfoFromDataRow.ShipAddr2 : byCustomerId.ShipAddr2;
        accInfoFromDataRow.ShipAddr3 = byCustomerId.ShipAddr3 == string.Empty ? accInfoFromDataRow.ShipAddr3 : byCustomerId.ShipAddr3;
        accInfoFromDataRow.ShipAddr4 = byCustomerId.ShipAddr4 == string.Empty ? accInfoFromDataRow.ShipAddr4 : byCustomerId.ShipAddr4;
        accInfoFromDataRow.ShipAddr5 = byCustomerId.ShipAddr5 == string.Empty ? accInfoFromDataRow.ShipAddr5 : byCustomerId.ShipAddr5;
        accInfoFromDataRow.AccEmail = byCustomerId.AccEmail == string.Empty ? accInfoFromDataRow.AccEmail : byCustomerId.AccEmail;
        accInfoFromDataRow.AltAccEmail = byCustomerId.AltAccEmail == string.Empty ? accInfoFromDataRow.AltAccEmail : byCustomerId.AltAccEmail;
        accInfoFromDataRow.PaymentTermID = byCustomerId.PaymentTermID > 0 ? accInfoFromDataRow.PaymentTermID : byCustomerId.PaymentTermID;
        accInfoFromDataRow.PriceLevelID = byCustomerId.PriceLevelID > 0 ? accInfoFromDataRow.PriceLevelID : byCustomerId.PriceLevelID;
        accInfoFromDataRow.Limit = byCustomerId.Limit > 0.0 ? accInfoFromDataRow.Limit : byCustomerId.Limit;
        accInfoFromDataRow.FullCoName = byCustomerId.FullCoName == string.Empty ? accInfoFromDataRow.FullCoName : byCustomerId.FullCoName;
        accInfoFromDataRow.AccFirstName = byCustomerId.AccFirstName == string.Empty ? accInfoFromDataRow.AccFirstName : byCustomerId.AccFirstName;
        accInfoFromDataRow.AccLastName = byCustomerId.AccLastName == string.Empty ? accInfoFromDataRow.AccLastName : byCustomerId.AccLastName;
        accInfoFromDataRow.AltAccFirstName = byCustomerId.AltAccFirstName == string.Empty ? accInfoFromDataRow.AltAccFirstName : byCustomerId.AltAccFirstName;
        accInfoFromDataRow.AltAccLastName = byCustomerId.AltAccLastName == string.Empty ? accInfoFromDataRow.AltAccLastName : byCustomerId.AltAccLastName;
        accInfoFromDataRow.RegNo = byCustomerId.RegNo == string.Empty ? accInfoFromDataRow.RegNo : byCustomerId.RegNo;
        accInfoFromDataRow.BankAccNo = byCustomerId.BankAccNo == string.Empty ? accInfoFromDataRow.BankAccNo : byCustomerId.BankAccNo;
        accInfoFromDataRow.BankBranch = byCustomerId.BankBranch == string.Empty ? accInfoFromDataRow.BankBranch : byCustomerId.BankBranch;
        accInfoFromDataRow.Notes = byCustomerId.Notes == string.Empty ? accInfoFromDataRow.Notes : byCustomerId.Notes;
        if (!accInfoFromDataRow.Equals((object) byCustomerId))
        {
          this._LogFile.AddToLog(", some items where found to be different and have be updated.");
          accInfoFromDataRow.Update(accInfoFromDataRow, (long) byCustomerId.CustomersAccInfoID);
        }
        else
          this._LogFile.AddToLog(", not items updated.");
      }
      else
      {
        this._LogFile.AddFormatStringToLog(", Account Info for Customer: {0} does not exist adding it", (object) pCustomer.CompanyName);
        accInfoFromDataRow.Insert(accInfoFromDataRow);
      }
    }
    else
      this._LogFile.AddFormatStringToLog(", Account Info for Customer: {0} cannot be added customer does not exist", (object) pCustomer.CompanyName);
  }

  private void AddCustomerToNotFound(CustomersTbl pCustomer, DataRow pDataRow)
  {
    if (!pCustomer.enabled)
      return;
    this._LogFile.AddFormatStringToLog(", Customer {0} is enabled so will add to the tracker system.", (object) pCustomer.CompanyName);
    CustomersAccInfoTbl accInfoFromDataRow = this.GetCustAccInfoFromDataRow(pDataRow);
    pCustomer.ContactAltFirstName = accInfoFromDataRow.AltAccFirstName;
    pCustomer.ContactAltLastName = accInfoFromDataRow.AltAccLastName;
    pCustomer.BillingAddress = $"{accInfoFromDataRow.ShipAddr1};{accInfoFromDataRow.ShipAddr2};{accInfoFromDataRow.ShipAddr3};{accInfoFromDataRow.ShipAddr4};{accInfoFromDataRow.ShipAddr5}";
    string pArea = pDataRow["CUSTFLD1"].ToString();
    pCustomer.City = this.FindCity(pArea, $"{accInfoFromDataRow.ShipAddr3} {accInfoFromDataRow.ShipAddr4} {accInfoFromDataRow.ShipAddr5}");
    Match match = Regex.Match(pCustomer.BillingAddress, "\\\\(?<num>\\d{4,5})\\\\");
    if (match.Success)
      pCustomer.PostalCode = match.Groups["num"].Value;
    pCustomer.AltEmailAddress = accInfoFromDataRow.AltAccEmail;
    pCustomer.MachineSN = pDataRow["CUSTFLD5"] == null ? string.Empty : pDataRow["CUSTFLD5"].ToString();
    pCustomer.CustomerTypeID = !string.IsNullOrEmpty(pCustomer.MachineSN) ? 5 : 3;
    if (pDataRow["REP"] != null)
    {
      pCustomer.SalesAgentID = this.GetPresonIDFromRep(pDataRow["REP"].ToString());
      pCustomer.PreferedAgent = pCustomer.SalesAgentID;
    }
    if (pCustomer.SalesAgentID == 0)
    {
      pCustomer.PreferedAgent = pCustomer.PostalCode.StartsWith("8") || pCustomer.PostalCode.StartsWith("7") ? 3 : 7;
      pCustomer.PreferedAgent = pCustomer.SalesAgentID;
    }
    pCustomer.Notes = $"Client added automatically by Account Merge date: {DateTime.Now:D}";
    if (pCustomer.InsertCustomer(pCustomer))
    {
      this._LogFile.AddFormatStringToLog(", Customer: {0} added to tracker system.", (object) pCustomer.CompanyName);
      if (this.CustomerExists(ref pCustomer))
      {
        this._LogFile.AddFormatStringToLog(", adding Account Info for Customer: {0} to tracker system.", (object) pCustomer.CompanyName);
        this.AddAccountInfoToCust(pCustomer, pDataRow);
      }
      else
        this._LogFile.AddFormatStringToLog(", Customer: {0} was not found could not add account info", (object) pCustomer.CompanyName);
    }
    else
      this._LogFile.AddFormatStringToLog(", error adding Customer: {0}", (object) pCustomer.CompanyName);
  }

  private void MergeCustomersTableToQonTData(DataTable pTable)
  {
    CustomersTbl pCustomer = new CustomersTbl();
    this._LogFile = new LogFile(this.Server.MapPath("~/App_Data/MergeCustomers.log"), false);
    this._PaymentTermTranslors = this.GetPaymentTermList();
    this._PriceLevelTranslors = this.GetPriceLevelList();
    this._AreaToCityMap = this.MapAreasToCityID();
    TrackerTools trackerTools = new TrackerTools();
    trackerTools.GetTrackerSessionErrorString();
    char ch1 = this.StartDropDownList.SelectedValue[0];
    char ch2 = this.FinishDropDownList.SelectedValue[0];
    int int32 = Convert.ToInt32(this.MaxRecsDropDownList.SelectedValue);
    long pObj1_1 = 0;
    foreach (DataRow row in (InternalDataCollectionBase) pTable.Rows)
    {
      pCustomer.CompanyName = row["NAME"] == null ? string.Empty : row["NAME"].ToString();
      pCustomer.CompanyName = pCustomer.CompanyName.Replace("\"", string.Empty);
      if (!string.IsNullOrEmpty(pCustomer.CompanyName) && (int) pCustomer.CompanyName[0] >= (int) ch1 && (int) pCustomer.CompanyName[0] <= (int) ch2)
      {
        pCustomer.EmailAddress = row["EMAIL"] == null ? string.Empty : row["EMAIL"].ToString();
        pCustomer.ContactFirstName = row["FIRSTNAME"] == null ? string.Empty : row["FIRSTNAME"].ToString();
        pCustomer.ContactLastName = row["LASTNAME"] == null ? string.Empty : row["LASTNAME"].ToString();
        if (row["MIDINIT"] != null)
        {
          string str = row["MIDINIT"].ToString();
          if (str.Equals("le") || str.Equals("vd") || str.Equals("van"))
            pCustomer.ContactLastName = $"{str} {pCustomer.ContactLastName}";
        }
        pCustomer.PhoneNumber = row["PHONE1"] == null ? string.Empty : row["PHONE1"].ToString();
        pCustomer.CellNumber = row["PHONE2"] == null ? string.Empty : row["PHONE2"].ToString();
        pCustomer.FaxNumber = row["FAXNUM"] == null ? string.Empty : row["FAXNUM"].ToString();
        pCustomer.enabled = row["HIDDEN"] != null && row["HIDDEN"].ToString().Equals("Y");
        this._LogFile.AddFormatStringToLog("Record: {0} of {1}, ", (object) pObj1_1, (object) pTable.Rows.Count);
        if (this.CustomerExists(ref pCustomer))
        {
          this._LogFile.AddFormatStringToLog("Customer: {0} found, adding info to account", (object) pCustomer.CompanyName);
          this.AddAccountInfoToCust(pCustomer, row);
        }
        else if (pCustomer.enabled)
        {
          this._LogFile.AddFormatStringToLog("Customer: {0} NOT found, adding customer to tracker ", (object) pCustomer.CompanyName);
          this.AddCustomerToNotFound(pCustomer, row);
        }
        else
          this._LogFile.AddFormatStringToLog("Customer: {0} NOT found, but is disabled so ignoring", (object) pCustomer.CompanyName);
        string pObj1_2 = trackerTools.GetTrackerSessionErrorString().Replace(",", ";");
        if (!string.IsNullOrEmpty(pObj1_2))
          this._LogFile.AddLineFormatStringToLog(",err: {0}", (object) pObj1_2);
        else
          this._LogFile.AddLineToLog(",done");
        if (pObj1_1 % 15L == 0L)
          this._LogFile.WriteLinesToLogFile();
        ++pObj1_1;
        if (pObj1_1 == (long) int32)
          break;
      }
    }
    this._LogFile.WriteLinesToLogFile();
  }

  private void ConvertTableToObj(DataTable pTable)
  {
    this.MergeCustomersTableToQonTData(pTable);
    this.gvCustomers.DataSource = (object) this.ReadLogFileData(this.Server.MapPath("~/App_Data/MergeCustomers.log"));
    this.gvCustomers.DataBind();
  }

  protected void SelectImportFileButton_Click(object sender, EventArgs e)
  {
    if (!this.MergeFileUpload.HasFile)
      return;
    try
    {
      string str1 = string.Empty;
      string str2 = this.Server.MapPath("~/App_Data/") + Path.GetFileName(this.MergeFileUpload.FileName);
      if (File.Exists(str2))
      {
        File.Delete(str2);
        str1 = "Old version of file existed, it was deleted. ";
      }
      this.MergeFileUpload.SaveAs(str2);
      this.StatusLabel.Text = str1 + "Upload status: File uploaded!";
      this.ConvertTableToObj(this.MergeFileWithData(str2));
    }
    catch (Exception ex)
    {
      this.StatusLabel.Text = "Error saving file to server: " + ex.Message;
      throw;
    }
  }

  protected void gvCustomers_PageIndexChanging(object sender, GridViewPageEventArgs e)
  {
    this.gvCustomers.PageIndex = e.NewPageIndex;
    this.bindGridView();
  }

  private void bindGridView()
  {
    this.gvCustomers.DataSource = (object) this.ReadLogFileData(this.Server.MapPath("~/App_Data/MergeCustomers.log"));
    this.gvCustomers.DataBind();
  }

  private class PaymentTermTranslor
  {
    private string _QBPaymentTermDesc;
    private string _QonTPaymentTermDesc;
    private int _QonTPaymentTermID;

    public PaymentTermTranslor()
    {
      this._QBPaymentTermDesc = string.Empty;
      this._QonTPaymentTermDesc = string.Empty;
      this._QonTPaymentTermID = 0;
    }

    public string QBPaymentTermDesc
    {
      get => this._QBPaymentTermDesc;
      set => this._QBPaymentTermDesc = value;
    }

    public string QonTPaymentTermDesc
    {
      get => this._QonTPaymentTermDesc;
      set => this._QonTPaymentTermDesc = value;
    }

    public int QonTPaymentTermID
    {
      get => this._QonTPaymentTermID;
      set => this._QonTPaymentTermID = value;
    }

    public int GetQonTPaymentTermIDByDesc(string pDesc)
    {
      return new PaymentTermsTbl().GetPaymentTermIDByDesc(pDesc);
    }
  }

  private class PriceLevelTranslor
  {
    private string _QBPriceLevelDesc;
    private string _QonTPriceLevelDesc;
    private int _QonTPriceLevelID;

    public PriceLevelTranslor()
    {
      this._QBPriceLevelDesc = string.Empty;
      this._QonTPriceLevelDesc = string.Empty;
      this._QonTPriceLevelID = 0;
    }

    public string QBPriceLevelDesc
    {
      get => this._QBPriceLevelDesc;
      set => this._QBPriceLevelDesc = value;
    }

    public string QonTPriceLevelDesc
    {
      get => this._QonTPriceLevelDesc;
      set => this._QonTPriceLevelDesc = value;
    }

    public int QonTPriceLevelID
    {
      get => this._QonTPriceLevelID;
      set => this._QonTPriceLevelID = value;
    }

    public int GetQonTPriceLevelIDByDesc(string pDesc)
    {
      return new PriceLevelsTbl().GetPriceLevelIDByDesc(pDesc);
    }
  }

  private class AreaToCityMap
  {
    private string _Area;
    private int _CityID;

    public AreaToCityMap()
    {
      this._Area = string.Empty;
      this._CityID = 1;
    }

    public string Area
    {
      get => this._Area;
      set => this._Area = value;
    }

    public int CityID
    {
      get => this._CityID;
      set => this._CityID = value;
    }
  }
}

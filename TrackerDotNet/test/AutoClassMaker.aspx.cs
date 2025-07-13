// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.test.AutoClassMaker
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

// #nullable disable --- not for this version of C#
namespace TrackerDotNet.test
{

    public class AutoClassMaker : Page
    {
        private const string CONST_CONSTRING = "Tracker08ConnectionString";
        private const string CONST_NAMESPACE = "TrackerDotNet.control";
        private const string SPC = "        ";
        private Dictionary<OleDbType, AutoClassMaker.dbTypesDef> _ColDBTypes;
        private StreamWriter _ColsStream;
        protected HtmlForm frmAutoClassMaker;
        protected System.Web.UI.ScriptManager smgrORMClass;
        protected UpdateProgress uprgORMCLass;
        protected DropDownList ddlTables;
        protected UpdatePanel upnlClassFileName;
        protected TextBox tbxORMClassFileName;
        protected Button btnGo;
        protected Button btnCreateGV;
        protected Button btnCreateDV;

        private OleDbConnection OpenTrackerOleDBConnection()
        {
            return ConfigurationManager.ConnectionStrings["Tracker08ConnectionString"] != null && !(ConfigurationManager.ConnectionStrings["Tracker08ConnectionString"].ConnectionString.Trim() == "") ? new OleDbConnection(ConfigurationManager.ConnectionStrings["Tracker08ConnectionString"].ConnectionString) : throw new Exception("A connection string named Tracker08ConnectionString with a valid connection string must exist in the <connectionStrings> configuration section for the application.");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
                return;
            OleDbConnection oleDbConnection = this.OpenTrackerOleDBConnection();
            if (oleDbConnection == null)
                return;
            try
            {
                oleDbConnection.Open();
                this.ddlTables.DataSource = (object)oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[4]
                {
        null,
        null,
        null,
        (object) "TABLE"
                });
                this.ddlTables.DataTextField = "TABLE_NAME";
                this.ddlTables.DataBind();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
            finally
            {
                oleDbConnection.Close();
            }
        }

        protected void ddlTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.tbxORMClassFileName.Text = this.ddlTables.SelectedValue + ".cs";
        }

        protected OleDbType GetOleDBType(string pRowDBType) => (OleDbType)int.Parse(pRowDBType);

        protected DbType GetDBType(string pRowDBType)
        {
            DbType dbType = (DbType)int.Parse(pRowDBType);
            if (dbType.ToString().Equals("130"))
                dbType = DbType.String;
            return dbType;
        }

        private Dictionary<OleDbType, AutoClassMaker.dbTypesDef> GetColDBTypes()
        {
            return new Dictionary<OleDbType, AutoClassMaker.dbTypesDef>()
    {
      {
        OleDbType.Binary,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "bool",
          typeNil = "false",
          typeConvert = "Convert.ToBoolean"
        }
      },
      {
        OleDbType.Boolean,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "bool",
          typeNil = "false",
          typeConvert = "Convert.ToBoolean"
        }
      },
      {
        OleDbType.BigInt,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "long",
          typeNil = "0",
          typeConvert = "Convert.ToInt64"
        }
      },
      {
        OleDbType.UnsignedBigInt,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "long",
          typeNil = "0",
          typeConvert = "Convert.ToInt64"
        }
      },
      {
        OleDbType.UnsignedTinyInt,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "byte",
          typeNil = "0",
          typeConvert = "Convert.ToByte"
        }
      },
      {
        OleDbType.TinyInt,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "int",
          typeNil = "0",
          typeConvert = "Convert.ToInt16"
        }
      },
      {
        OleDbType.Integer,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "int",
          typeNil = "0",
          typeConvert = "Convert.ToInt32"
        }
      },
      {
        OleDbType.Currency,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "double",
          typeNil = "0.0",
          typeConvert = "Convert.ToDouble"
        }
      },
      {
        OleDbType.Date,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "DateTime",
          typeNil = "System.DateTime.Now",
          typeConvert = "Convert.ToDateTime"
        }
      },
      {
        OleDbType.DBDate,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "DateTime",
          typeNil = "System.DateTime.Now",
          typeConvert = "Convert.ToDateTime"
        }
      },
      {
        OleDbType.DBTime,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "DateTime",
          typeNil = "System.DateTime.Now",
          typeConvert = "Convert.ToDateTime"
        }
      },
      {
        OleDbType.Double,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "double",
          typeNil = "0.0",
          typeConvert = "Convert.ToDouble"
        }
      },
      {
        OleDbType.Guid,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "long",
          typeNil = "0",
          typeConvert = "Convert.ToInt64"
        }
      },
      {
        OleDbType.Char,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "string",
          typeNil = "string.Empty",
          typeConvert = ""
        }
      },
      {
        OleDbType.WChar,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "string",
          typeNil = "string.Empty",
          typeConvert = ""
        }
      },
      {
        OleDbType.VarChar,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "string",
          typeNil = "string.Empty",
          typeConvert = ""
        }
      },
      {
        OleDbType.LongVarChar,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "string",
          typeNil = "string.Empty",
          typeConvert = ""
        }
      },
      {
        OleDbType.LongVarWChar,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "string",
          typeNil = "string.Empty",
          typeConvert = ""
        }
      },
      {
        OleDbType.Single,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "double",
          typeNil = "0.0",
          typeConvert = "Convert.ToDouble"
        }
      },
      {
        OleDbType.SmallInt,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "int16",
          typeNil = "0",
          typeConvert = "Convert.ToInt16"
        }
      },
      {
        OleDbType.Numeric,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "double",
          typeNil = "0.0",
          typeConvert = "Convert.ToDouble"
        }
      },
      {
        OleDbType.Decimal,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "double",
          typeNil = "0.0",
          typeConvert = "Convert.ToDouble"
        }
      },
      {
        OleDbType.IUnknown,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "var",
          typeNil = "null",
          typeConvert = ""
        }
      },
      {
        OleDbType.Empty,
        new AutoClassMaker.dbTypesDef()
        {
          typeName = "var",
          typeNil = "null",
          typeConvert = ""
        }
      }
    };
        }

        private string GetSELECTstring(DataRow[] pRows)
        {
            string str1 = "    const string CONST_SQL_SELECT = \"SELECT ";
            string str2 = "";
            int num = 0;
            foreach (DataRow pRow in pRows)
            {
                str2 = $"{str2}{pRow["COLUMN_NAME"].ToString()}, ";
                ++num;
                if (num == 8)
                {
                    str2 = $"{str2}\" + {Environment.NewLine}                            \" ";
                    num = 0;
                }
            }
            string str3 = str2.Remove(str2.Length - 2, 2);
            return $"{str1}{str3} FROM {this.ddlTables.SelectedValue}\";";
        }

        private string GetINSERTstring(DataRow[] pRows)
        {
            string str1 = $"    const string CONST_SQL_INSERT = \"INSERT INTO {this.ddlTables.SelectedValue} (";
            string str2 = "";
            string str3 = "";
            int num = 0;
            foreach (DataRow pRow in pRows)
            {
                if (num != 0)
                {
                    str2 = $"{str2}{pRow["COLUMN_NAME"].ToString()}, ";
                    str3 += "?, ";
                    if (num % 8 == 0)
                        str2 = $"{str2}\" + {Environment.NewLine}                            \" ";
                }
                ++num;
            }
            string str4 = str2.Remove(str2.Length - 2, 2) + ")";
            string str5 = str3.Remove(str3.Length - 2, 2);
            return $"{str1}{str4}\" + {Environment.NewLine}                          \" VALUES ( {str5})\";   //id field not inserted";
        }

        private string GetUPDATEstring(DataRow[] pRows)
        {
            string str1 = $"    const string CONST_SQL_UPDATE = \"UPDATE {this.ddlTables.SelectedValue} SET ";
            string str2 = "";
            string str3 = "";
            int num = 0;
            foreach (DataRow pRow in pRows)
            {
                if (num == 0)
                {
                    str3 = $" WHERE ({pRow["COLUMN_NAME"].ToString()} = ?)";
                }
                else
                {
                    str2 = $"{str2}{pRow["COLUMN_NAME"].ToString()} = ?, ";
                    if (num % 8 == 0)
                        str2 = $"{str2}\" + {Environment.NewLine}                            \" ";
                }
                ++num;
            }
            string str4 = str2.Remove(str2.Length - 2, 2);
            return $"{str1}{str4}\" + {Environment.NewLine}                           \"{str3}\";";
        }

        private string GetDELETEstring(DataRow[] pRows)
        {
            return $"{"    const string CONST_SQL_DELETE = \"DELETE FROM " + this.ddlTables.SelectedValue} WHERE ({pRows[0]["COLUMN_NAME"].ToString()} = ?)\";";
        }

        private void WriteGetAllProc(DataRow[] pRows)
        {
            this._ColsStream.WriteLine("    [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select, true)]");
            this._ColsStream.WriteLine($"    public List<{this.ddlTables.SelectedValue}> GetAll(string SortBy)");
            this._ColsStream.WriteLine("    {");
            this._ColsStream.WriteLine($"      List<{this.ddlTables.SelectedValue}> _DataItems = new List<{this.ddlTables.SelectedValue}>();");
            this._ColsStream.WriteLine("      TrackerDb _TDB = new TrackerDb();");
            this._ColsStream.WriteLine("      string _sqlCmd = CONST_SQL_SELECT;");
            this._ColsStream.WriteLine("      if (!String.IsNullOrEmpty(SortBy)) _sqlCmd += \" ORDER BY \" + SortBy;     // Add order by string");
            this._ColsStream.WriteLine("      // params would go here if need");
            this._ColsStream.WriteLine("      IDataReader _DataReader = _TDB.ExecuteSQLGetDataReader(_sqlCmd);");
            this._ColsStream.WriteLine("      if (_DataReader != null)");
            this._ColsStream.WriteLine("      {");
            this._ColsStream.WriteLine("        while (_DataReader.Read())");
            this._ColsStream.WriteLine("        {");
            this._ColsStream.WriteLine($"          {this.ddlTables.SelectedValue} _DataItem = new {this.ddlTables.SelectedValue}();");
            this._ColsStream.WriteLine();
            this._ColsStream.WriteLine("           #region StoreThisDataItem");
            foreach (DataRow pRow in pRows)
            {
                OleDbType oleDbType = this.GetOleDBType(pRow["DATA_TYPE"].ToString());
                this._ColsStream.Write($"          _DataItem.{pRow["COLUMN_NAME"].ToString()} = (_DataReader[\"{pRow["COLUMN_NAME"].ToString()}\"");
                this._ColsStream.Write($"] == DBNull.Value) ? {this._ColDBTypes[oleDbType].typeNil} : ");
                if (string.IsNullOrEmpty(this._ColDBTypes[oleDbType].typeConvert))
                    this._ColsStream.WriteLine($"_DataReader[\"{pRow["COLUMN_NAME"].ToString()}\"].ToString();");
                else
                    this._ColsStream.WriteLine($"{this._ColDBTypes[oleDbType].typeConvert}(_DataReader[\"{pRow["COLUMN_NAME"].ToString()}\"]);");
            }
            this._ColsStream.WriteLine("          #endregion");
            this._ColsStream.WriteLine("          _DataItems.Add(_DataItem);");
            this._ColsStream.WriteLine("        }");
            this._ColsStream.WriteLine("        _DataReader.Close();");
            this._ColsStream.WriteLine("      }");
            this._ColsStream.WriteLine("      _TDB.Close();");
            this._ColsStream.WriteLine("      return _DataItems;");
            this._ColsStream.WriteLine("    }");
        }

        private void WriteInsertProc(DataRow[] pRows)
        {
            this._ColsStream.WriteLine("    [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert, true)]");
            this._ColsStream.WriteLine($"    public string Insert({this.ddlTables.SelectedValue} p{this.ddlTables.SelectedValue})");
            this._ColsStream.WriteLine("    {");
            this._ColsStream.WriteLine("      string _result = string.Empty;");
            this._ColsStream.WriteLine("      TrackerDb _TDB = new TrackerDb();");
            this._ColsStream.WriteLine();
            this._ColsStream.WriteLine("      #region InsertParameters");
            int num = 0;
            foreach (DataRow pRow in pRows)
            {
                if (num > 0)
                    this._ColsStream.WriteLine($"      _TDB.AddParams(p{this.ddlTables.SelectedValue}.{pRow["COLUMN_NAME"].ToString()}, DbType.{(object)this.GetDBType(pRow["DATA_TYPE"].ToString())}, \"@{pRow["COLUMN_NAME"].ToString()}\");");
                ++num;
            }
            this._ColsStream.WriteLine("      #endregion");
            this._ColsStream.WriteLine("      // Now we have the parameters excute the SQL");
            this._ColsStream.WriteLine("      _result = _TDB.ExecuteNonQuerySQL(CONST_SQL_INSERT);");
            this._ColsStream.WriteLine("      _TDB.Close();");
            this._ColsStream.WriteLine("      return _result;");
            this._ColsStream.WriteLine("    }");
            this._ColsStream.WriteLine();
        }

        private void WriteUpdateProc(DataRow[] pRows)
        {
            this._ColsStream.WriteLine("    [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update, true)]");
            this._ColsStream.WriteLine($"    public string Update({this.ddlTables.SelectedValue} p{this.ddlTables.SelectedValue})");
            this._ColsStream.WriteLine($"    {{ return Update(p{this.ddlTables.SelectedValue}, 0); }}");
            this._ColsStream.WriteLine("    [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update, false)]");
            this._ColsStream.WriteLine($"    public string Update({this.ddlTables.SelectedValue} p{this.ddlTables.SelectedValue}, int pOrignal_{pRows[0]["COLUMN_NAME"].ToString()})");
            this._ColsStream.WriteLine("    {");
            this._ColsStream.WriteLine("      string _result = string.Empty;");
            this._ColsStream.WriteLine("      TrackerDb _TDB = new TrackerDb();");
            this._ColsStream.WriteLine();
            this._ColsStream.WriteLine("      #region UpdateParameters");
            int num = 0;
            foreach (DataRow pRow in pRows)
            {
                if (num == 0)
                {
                    this._ColsStream.WriteLine($"      if (pOrignal_{pRows[0]["COLUMN_NAME"].ToString()} > 0)");
                    this._ColsStream.WriteLine($"        _TDB.AddWhereParams(pOrignal_{pRows[0]["COLUMN_NAME"].ToString()}, DbType.Int32);  // check this line it assumes id field is int32");
                    this._ColsStream.WriteLine("      else");
                    this._ColsStream.WriteLine($"        _TDB.AddWhereParams(p{this.ddlTables.SelectedValue}.{pRow["COLUMN_NAME"].ToString()}, DbType.{(object)this.GetDBType(pRow["DATA_TYPE"].ToString())}, \"@{pRow["COLUMN_NAME"].ToString()}\");");
                    this._ColsStream.WriteLine("");
                }
                else
                    this._ColsStream.WriteLine($"      _TDB.AddParams(p{this.ddlTables.SelectedValue}.{pRow["COLUMN_NAME"].ToString()}, DbType.{(object)this.GetDBType(pRow["DATA_TYPE"].ToString())}, \"@{pRow["COLUMN_NAME"].ToString()}\" );");
                ++num;
            }
            this._ColsStream.WriteLine("      #endregion");
            this._ColsStream.WriteLine("      // Now we have the parameters excute the SQL");
            this._ColsStream.WriteLine("      _result = _TDB.ExecuteNonQuerySQL(CONST_SQL_UPDATE);");
            this._ColsStream.WriteLine("      _TDB.Close();");
            this._ColsStream.WriteLine("      return _result;");
            this._ColsStream.WriteLine("    }");
            this._ColsStream.WriteLine();
        }

        private void WriteDeleteProc(DataRow[] pRows)
        {
            this._ColsStream.WriteLine("    [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete, true)]");
            this._ColsStream.WriteLine($"    public string Delete({this.ddlTables.SelectedValue} p{this.ddlTables.SelectedValue})");
            this._ColsStream.WriteLine($"    {{ return Delete(p{this.ddlTables.SelectedValue}.{pRows[0]["COLUMN_NAME"].ToString()}); }}");
            this._ColsStream.WriteLine("    [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete, false)]");
            this._ColsStream.WriteLine($"    public string Delete(int p{pRows[0]["COLUMN_NAME"].ToString()})");
            this._ColsStream.WriteLine("    {");
            this._ColsStream.WriteLine("      string _result = string.Empty;");
            this._ColsStream.WriteLine("      TrackerDb _TDB = new TrackerDb();");
            this._ColsStream.WriteLine();
            this._ColsStream.WriteLine($"      _TDB.AddWhereParams(p{pRows[0]["COLUMN_NAME"].ToString()}, DbType.Int32, \"@{pRows[0]["COLUMN_NAME"].ToString()}\");");
            this._ColsStream.WriteLine("      _result = _TDB.ExecuteNonQuerySQL(CONST_SQL_DELETE);");
            this._ColsStream.WriteLine("      _TDB.Close();");
            this._ColsStream.WriteLine("      return _result;");
            this._ColsStream.WriteLine("    }");
            this._ColsStream.WriteLine();
        }

        protected DataRow[] GetDBRowDefinitions()
        {
            OleDbConnection oleDbConnection = this.OpenTrackerOleDBConnection();
            try
            {
                oleDbConnection.Open();
                return oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[4]
                {
        null,
        null,
        (object) this.ddlTables.SelectedValue,
        null
                }).Select((string)null, "ORDINAL_POSITION", DataViewRowState.CurrentRows);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
            finally
            {
                oleDbConnection.Close();
            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            this._ColDBTypes = this.GetColDBTypes();
            this._ColsStream = new StreamWriter("c:\\temp\\" + this.tbxORMClassFileName.Text, false);
            this._ColsStream.WriteLine("/// --- auto generated class for table: " + this.ddlTables.SelectedValue);
            this._ColsStream.WriteLine("using System;   // for DateTime variables");
            this._ColsStream.WriteLine("using System.Collections.Generic;   // for data stuff");
            this._ColsStream.WriteLine("using System.Data;                  // for IDataReader and DbType");
            this._ColsStream.WriteLine("using TrackerDotNet.classes;        // TrackerDot classes used for DB access");
            this._ColsStream.WriteLine();
            this._ColsStream.WriteLine("namespace TrackerDotNet.control");
            this._ColsStream.WriteLine("{");
            this._ColsStream.WriteLine("  public class " + this.ddlTables.SelectedValue);
            this._ColsStream.WriteLine("  {");
            this._ColsStream.WriteLine("    #region InternalVariableDeclarations");
            DataRow[] dbRowDefinitions = this.GetDBRowDefinitions();
            foreach (DataRow dataRow in dbRowDefinitions)
            {
                OleDbType oleDbType = this.GetOleDBType(dataRow["DATA_TYPE"].ToString());
                this._ColsStream.Write("    private ");
                if (this._ColDBTypes.ContainsKey(oleDbType))
                    this._ColsStream.Write(this._ColDBTypes[oleDbType].typeName);
                else
                    this._ColsStream.Write(oleDbType.ToString());
                this._ColsStream.WriteLine($" _{dataRow["COLUMN_NAME"].ToString()};");
            }
            this._ColsStream.WriteLine("    #endregion");
            this._ColsStream.WriteLine();
            this._ColsStream.WriteLine("    // class definition");
            this._ColsStream.WriteLine($"    public {this.ddlTables.SelectedValue}()");
            this._ColsStream.WriteLine("    {");
            foreach (DataRow dataRow in dbRowDefinitions)
            {
                OleDbType oleDbType = this.GetOleDBType(dataRow["DATA_TYPE"].ToString());
                if (this._ColDBTypes.ContainsKey(oleDbType))
                    this._ColsStream.WriteLine($"      _{dataRow["COLUMN_NAME"].ToString()} = {this._ColDBTypes[oleDbType].typeNil};");
                else
                    this._ColsStream.WriteLine($"      _{dataRow["COLUMN_NAME"].ToString()} = {"1"};");
            }
            this._ColsStream.WriteLine("    }");
            this._ColsStream.WriteLine("    #region PublicVariableDeclarations");
            foreach (DataRow dataRow in dbRowDefinitions)
            {
                OleDbType oleDbType = this.GetOleDBType(dataRow["DATA_TYPE"].ToString());
                this._ColsStream.Write("    public ");
                if (this._ColDBTypes.ContainsKey(oleDbType))
                    this._ColsStream.Write(this._ColDBTypes[oleDbType].typeName);
                else
                    this._ColsStream.Write(oleDbType.ToString());
                this._ColsStream.Write(" " + dataRow["COLUMN_NAME"].ToString());
                this._ColsStream.Write(" { get { return _");
                this._ColsStream.Write(dataRow["COLUMN_NAME"].ToString());
                this._ColsStream.Write(";}  set { _");
                this._ColsStream.Write(dataRow["COLUMN_NAME"].ToString());
                this._ColsStream.WriteLine(" = value;} }");
            }
            this._ColsStream.WriteLine("    #endregion");
            this._ColsStream.WriteLine();
            this._ColsStream.WriteLine("    #region ConstantDeclarations");
            this._ColsStream.WriteLine(this.GetSELECTstring(dbRowDefinitions));
            this._ColsStream.WriteLine(this.GetINSERTstring(dbRowDefinitions));
            this._ColsStream.WriteLine(this.GetUPDATEstring(dbRowDefinitions));
            this._ColsStream.WriteLine(this.GetDELETEstring(dbRowDefinitions));
            this._ColsStream.WriteLine("    #endregion");
            this._ColsStream.WriteLine();
            this.WriteGetAllProc(dbRowDefinitions);
            this.WriteInsertProc(dbRowDefinitions);
            this.WriteUpdateProc(dbRowDefinitions);
            this.WriteDeleteProc(dbRowDefinitions);
            this._ColsStream.WriteLine("  }");
            this._ColsStream.WriteLine("}");
            this._ColsStream.Close();
        }

        protected void WriteTextBoxTemplate(string pFieldName, string pIndnt)
        {
            this._ColsStream.WriteLine(pIndnt + "<asp:TemplateField ConvertEmptyStringToNull=\"False\" ");
            this._ColsStream.WriteLine($"{pIndnt}  HeaderText=\"{pFieldName}\" SortExpression=\"{pFieldName}\">");
            this._ColsStream.WriteLine(pIndnt + "  <EditItemTemplate>");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:TextBox ID=\"{pFieldName}TextBox\" runat=\"server\" Text='<%# Bind(\"{pFieldName}\") %>' Width=\"10em\" />");
            this._ColsStream.WriteLine(pIndnt + "  </EditItemTemplate>");
            this._ColsStream.WriteLine(pIndnt + "  <ItemTemplate>");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:Label ID=\"{pFieldName}Label\" runat=\"server\" Text='<%# Bind(\"{pFieldName}\") %>' />");
            this._ColsStream.WriteLine(pIndnt + " </ItemTemplate>");
            this._ColsStream.WriteLine(pIndnt + "  <FooterTemplate>");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:TextBox ID=\"{pFieldName}TextBox\" runat=\"server\"  Width=\"10em\" />");
            this._ColsStream.WriteLine(pIndnt + "  </FooterTemplate>");
            this._ColsStream.WriteLine(pIndnt + "</asp:TemplateField>");
        }

        protected void WriteCheckBoxTemplate(string pFieldName, string pIndnt)
        {
            this._ColsStream.WriteLine(pIndnt + "<asp:TemplateField ConvertEmptyStringToNull=\"False\" ");
            this._ColsStream.WriteLine($"{pIndnt}  HeaderText=\"{pFieldName}\" SortExpression=\"{pFieldName}\">");
            this._ColsStream.WriteLine(pIndnt + "  <EditItemTemplate>");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:CheckBox ID=\"{pFieldName}CheckBox\" runat=\"server\" Text=\"Yes\" Checked='<%# Bind(\"{pFieldName}\") %>' />");
            this._ColsStream.WriteLine(pIndnt + "  </EditItemTemplate>");
            this._ColsStream.WriteLine(pIndnt + "  <ItemTemplate>");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:CheckBox ID=\"{pFieldName}CheckBox\" runat=\"server\" Text=\"Yes\" Checked='<%# Bind(\"{pFieldName}\") %>' />");
            this._ColsStream.WriteLine(pIndnt + " </ItemTemplate>");
            this._ColsStream.WriteLine(pIndnt + "  <FooterTemplate>");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:CheckBox ID=\"{pFieldName}CheckBox\" runat=\"server\" Text=\"Yes\" Checked='<%# Bind(\"{pFieldName}\") %>' />");
            this._ColsStream.WriteLine(pIndnt + "  </FooterTemplate>");
            this._ColsStream.WriteLine(pIndnt + "</asp:TemplateField>");
        }

        protected void WriteTemplateFields(DataRow[] pRows, string pIndnt)
        {
            this._ColsStream.WriteLine(pIndnt + "<asp:TemplateField ShowHeader=\"False\">");
            this._ColsStream.WriteLine(pIndnt + "  <EditItemTemplate>");
            this._ColsStream.WriteLine(pIndnt + "    <asp:Button ID=\"UpdateButton\" runat=\"server\" CausesValidation=\"True\" CommandName=\"Update\" Text=\"Update\" />");
            this._ColsStream.WriteLine(pIndnt + "    &nbsp;<asp:Button ID=\"CancelButton\" runat=\"server\" CausesValidation=\"False\" CommandName=\"Cancel\" Text=\"Cancel\" />");
            this._ColsStream.WriteLine(pIndnt + "  </EditItemTemplate>");
            this._ColsStream.WriteLine(pIndnt + "  <ItemTemplate>");
            this._ColsStream.WriteLine(pIndnt + "    <asp:Button ID=\"EditButton\" runat=\"server\" CausesValidation=\"False\" CommandName=\"Edit\" Text=\"Edit\" />");
            this._ColsStream.WriteLine(pIndnt + "    &nbsp;<asp:Button ID=\"DeleteButton\" runat=\"server\" CausesValidation=\"False\" CommandName=\"Delete\" Text=\"Delete\" />");
            this._ColsStream.WriteLine(pIndnt + "  </ItemTemplate>");
            this._ColsStream.WriteLine(pIndnt + "  <FooterTemplate>");
            this._ColsStream.WriteLine(pIndnt + "    <asp:Button ID=\"AddButton\" runat=\"server\" CausesValidation=\"False\" CommandName=\"Add\" Text=\"Add\" />");
            this._ColsStream.WriteLine(pIndnt + "  </FooterTemplate>");
            this._ColsStream.WriteLine(pIndnt + "</asp:TemplateField>");
            string empty = string.Empty;
            int num = 0;
            foreach (DataRow pRow in pRows)
            {
                if (num != 0)
                {
                    string pFieldName = pRow["COLUMN_NAME"].ToString();
                    if (this.GetOleDBType(pRow["DATA_TYPE"].ToString()).Equals((object)OleDbType.Boolean))
                        this.WriteCheckBoxTemplate(pFieldName, pIndnt);
                    else
                        this.WriteTextBoxTemplate(pFieldName, pIndnt);
                }
                ++num;
            }
            string str = pRows[0]["COLUMN_NAME"].ToString();
            this._ColsStream.WriteLine(pIndnt + "<asp:TemplateField ConvertEmptyStringToNull=\"False\" ");
            this._ColsStream.WriteLine($"{pIndnt}  HeaderText=\"{str}\" SortExpression=\"{str}\">");
            this._ColsStream.WriteLine(pIndnt + "  <EditItemTemplate>");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:Literal ID=\"{str}Literal\" runat=\"server\" Text='<%# Bind(\"{str}\") %>' />");
            this._ColsStream.WriteLine(pIndnt + "  </EditItemTemplate>");
            this._ColsStream.WriteLine(pIndnt + "  <ItemTemplate>");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:Literal ID=\"{str}Literal\" runat=\"server\" Text='<%# Bind(\"{str}\") %>' />");
            this._ColsStream.WriteLine(pIndnt + "  </ItemTemplate>");
            this._ColsStream.WriteLine(pIndnt + "  <FooterTemplate>");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:Literal ID=\"{str}Literal\" runat=\"server\" Text='<%# Bind(\"{str}\") %>' />");
            this._ColsStream.WriteLine(pIndnt + "  </FooterTemplate>");
            this._ColsStream.WriteLine(pIndnt + "</asp:TemplateField>");
        }

        protected void WriteEmptyTemplate(DataRow[] pRows, string pIndnt)
        {
            this._ColsStream.WriteLine(pIndnt + "<EmptyDataTemplate>");
            string empty = string.Empty;
            int num = 0;
            foreach (DataRow pRow in pRows)
            {
                if (num != 0)
                {
                    string str = pRow["COLUMN_NAME"].ToString();
                    OleDbType oleDbType = this.GetOleDBType(pRow["DATA_TYPE"].ToString());
                    this._ColsStream.Write($"{pIndnt}  &nbsp;&nbsp;{str}:&nbsp;");
                    if (oleDbType.Equals((object)OleDbType.Boolean))
                        this._ColsStream.WriteLine($"<asp:CheckBox ID=\"{str}CheckBox\" runat=\"server\" Text=\"Yes\" />");
                    else
                        this._ColsStream.WriteLine($"<asp:TextBox ID=\"{str}TextBox\" runat=\"server\" Width=\"10em\" />");
                }
                ++num;
            }
            this._ColsStream.WriteLine(pIndnt + "</EmptyDataTemplate>");
        }

        protected void WriteObjectDataSource(DataRow[] pRows, string pFormName, string pIndnt)
        {
            this._ColsStream.WriteLine($"{pIndnt}<asp:ObjectDataSource ID=\"ods{pFormName}\" runat=\"server\" TypeName=\"TrackerDotNet.control.{pFormName}\"");
            this._ColsStream.WriteLine($"{pIndnt}  DataObjectTypeName=\"TrackerDotNet.control.{pFormName}\" SelectMethod=\"GetAll\" SortParameterName=\"SortBy\"");
            this._ColsStream.WriteLine(pIndnt + "  UpdateMethod=\"Update\" OldValuesParameterFormatString=\"original_{0}\" InsertMethod=\"Insert\" DeleteMethod=\"Delete\">");
            this._ColsStream.WriteLine(pIndnt + "  <DeleteParameters>");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:Parameter Name=\"p{pRows[0]["COLUMN_NAME"].ToString()}\" Type=\"Int32\" />");
            this._ColsStream.WriteLine(pIndnt + "  </DeleteParameters>");
            this._ColsStream.WriteLine(pIndnt + "  <SelectParameters>");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:Parameter DefaultValue=\"{pRows[1]["COLUMN_NAME"].ToString()}\" Name=\"SortBy\" Type=\"String\" />");
            this._ColsStream.WriteLine(pIndnt + "  </SelectParameters>");
            this._ColsStream.WriteLine(pIndnt + "  <UpdateParameters>");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:Parameter Name=\"{pFormName}\" Type=\"Object\" DbType=\"Object\" />");
            this._ColsStream.WriteLine($"{pIndnt}    <asp:Parameter Name=\"pOrignal_{pRows[0]["COLUMN_NAME"].ToString()}\" Type=\"Int32\" />");
            this._ColsStream.WriteLine(pIndnt + "  </UpdateParameters>");
            this._ColsStream.WriteLine(pIndnt + "</asp:ObjectDataSource>");
        }

        protected void WriteRowLogic(DataRow[] pRows, string pFormName, string pIndnt)
        {
            int length = pIndnt.Length;
            string str1 = pRows[1]["COLUMN_NAME"].ToString();
            this._ColsStream.WriteLine($"{pIndnt}TextBox _{str1}TextBox = (TextBox)_row.FindControl(\"{str1}TextBox\");");
            this._ColsStream.WriteLine($"{pIndnt}if ((_{str1}TextBox != null) && (!String.IsNullOrEmpty(_{str1}TextBox.Text)))");
            this._ColsStream.WriteLine(pIndnt + "{");
            pIndnt += "  ";
            this._ColsStream.WriteLine($"{pIndnt}control.{pFormName}Tbl _{pFormName} = new control.{pFormName}Tbl();");
            string str2 = pRows[0]["COLUMN_NAME"].ToString();
            this._ColsStream.WriteLine($"{pIndnt}Literal _{str2}Literal = (Literal)_row.FindControl(\"{str2}Literal\");");
            this._ColsStream.WriteLine($"{pIndnt}_{pFormName}.{str2} = (_{str2}Literal != null) ? Convert.ToInt32(_{str2}Literal.Text) : 0;");
            this._ColsStream.WriteLine(pIndnt + "if (e.CommandName.Equals(\"Delete\"))");
            this._ColsStream.WriteLine(pIndnt + "{");
            this._ColsStream.WriteLine($"{pIndnt}  _{pFormName}.Delete(_{pFormName}.{str2});");
            this._ColsStream.WriteLine(pIndnt + "}");
            this._ColsStream.WriteLine(pIndnt + "else");
            this._ColsStream.WriteLine(pIndnt + "{");
            pIndnt += "  ";
            for (int index = 2; index < pRows.Length; index = index + 1 + 1)
            {
                string str3 = pRows[index]["COLUMN_NAME"].ToString();
                if (this.GetOleDBType(pRows[index]["DATA_TYPE"].ToString()).Equals((object)OleDbType.Boolean))
                    this._ColsStream.WriteLine($"{pIndnt}CheckBox _{str3} = (CheckBox)_row.FindControl(\"{str3}CheckBox\");");
                else
                    this._ColsStream.WriteLine($"{pIndnt}TextBox _{str3}TextBox = (TextBox)_row.FindControl(\"{str3}TextBox\");");
            }
            int num = 0;
            foreach (DataRow pRow in pRows)
            {
                string str4 = pRow["COLUMN_NAME"].ToString();
                if (num != 0)
                {
                    if (this.GetOleDBType(pRow["DATA_TYPE"].ToString()).Equals((object)OleDbType.Boolean))
                        this._ColsStream.WriteLine($"{pIndnt}_{pFormName}.{str4} = (_{str4}CheckBox != null) ? _{str4}CheckBox.Checked : false;");
                    else
                        this._ColsStream.WriteLine($"{pIndnt}_{pFormName}.{str4} = (_{str4}TextBox != null) ? _{str4}TextBox.Text : string.Empty");
                }
            }
            string str5 = pRows[0]["COLUMN_NAME"].ToString();
            this._ColsStream.WriteLine(pIndnt + "if (e.CommandName.Equals(\"Add\") || e.CommandName.Equals(\"Insert\"))");
            this._ColsStream.WriteLine(pIndnt + "{");
            this._ColsStream.WriteLine($"{pIndnt}  _{pFormName}.Insert(_{pFormName});");
            this._ColsStream.WriteLine(pIndnt + "}");
            this._ColsStream.WriteLine(pIndnt + "else if (e.CommandName.Equals(\"Update\"))");
            this._ColsStream.WriteLine(pIndnt + "{");
            this._ColsStream.WriteLine($"{pIndnt}  _{pFormName}.Update(_{pFormName}, _{pFormName}.{str5});");
            this._ColsStream.WriteLine(pIndnt + "}");
            this._ColsStream.WriteLine(pIndnt.Remove(1, 2) + "}");
            pIndnt = pIndnt.Remove(1, pIndnt.Length - length);
            this._ColsStream.WriteLine(pIndnt + "}");
        }

        protected void WriteView(bool pIsGridView)
        {
            string str1 = pIsGridView ? "GridView" : "DetailsView";
            string str2 = pIsGridView ? "gv" : "dv";
            this._ColDBTypes = this.GetColDBTypes();
            string path = "c:\\temp\\" + this.tbxORMClassFileName.Text;
            if (path.Contains(".cs"))
                path = path.Replace(".cs", ".aspx");
            this._ColsStream = new StreamWriter(path, false);
            string selectedValue = this.ddlTables.SelectedValue;
            string pFormName = selectedValue.Contains("Tbl") ? selectedValue.Remove(selectedValue.IndexOf("Tbl"), 3) : selectedValue;
            DataRow[] dbRowDefinitions = this.GetDBRowDefinitions();
            this._ColsStream.WriteLine("// copy and past the code below into the area you want to place the " + str1);
            this._ColsStream.WriteLine($"        <asp:UpdateProgress runat=\"server\" ID=\"{str2}{pFormName}UpdateProgress AssociatedUpdatePanelID=\"{str2}{selectedValue}Panel\" >");
            this._ColsStream.WriteLine("          <ProgressTemplate>");
            this._ColsStream.WriteLine("             Please Wait&nbsp;<img src=\"../images/animi/QuaffeeProgress.gif\" alt=\"Please Wait...\" />&nbsp;...");
            this._ColsStream.WriteLine("          </ProgressTemplate>");
            this._ColsStream.WriteLine("        </asp:UpdateProgress>");
            this._ColsStream.WriteLine($"        <asp:UpdatePanel ID=\"{str2}{pFormName}UpdatePanel\" runat=\"server\" ChildrenAsTriggers=\"true\">");
            this._ColsStream.WriteLine("          <ContentTemplate>");
            this._ColsStream.WriteLine($"            <asp:{str1} ID=\"{str2}{pFormName}\" runat=\"server\" AllowSorting=\"True\" DataSourceID=\"ods{pFormName}\" DataKeyNames=\"{dbRowDefinitions[0]["COLUMN_NAME"].ToString()}\"");
            this._ColsStream.WriteLine("               CssClass=\"TblWhite\" AutoGenerateColumns=\"False\" ShowFooter=\"True\"");
            this._ColsStream.WriteLine($"               OnRowCommand=\"{str2}{pFormName}_RowCommand\">");
            if (pIsGridView)
                this._ColsStream.WriteLine("            <Columns>");
            else
                this._ColsStream.WriteLine("            <Fields>");
            this.WriteTemplateFields(dbRowDefinitions, "              ");
            if (pIsGridView)
                this._ColsStream.WriteLine("            </Columns>");
            else
                this._ColsStream.WriteLine("            </Fields>");
            this.WriteEmptyTemplate(dbRowDefinitions, "            ");
            this._ColsStream.WriteLine($"            </asp:{str1}>");
            this._ColsStream.WriteLine("          </ContentTemplate>");
            this._ColsStream.WriteLine("        </asp:UpdatePanel>");
            this.WriteObjectDataSource(dbRowDefinitions, pFormName, "        ");
            this._ColsStream.Close();
            if (path.Contains(".aspx"))
                path = path.Replace(".aspx", ".aspx.cs");
            this._ColsStream = new StreamWriter(path, false);
            this._ColsStream.WriteLine($"    protected void {str2}{pFormName}_RowCommand(object sender, {str1}CommandEventArgs e)");
            this._ColsStream.WriteLine("    {");
            this._ColsStream.WriteLine($"      {str1}Row _row = ({str1}Row)((Control)e.CommandSource).NamingContainer;");
            this._ColsStream.WriteLine("      if (_row != null)");
            this._ColsStream.WriteLine("      {");
            this.WriteRowLogic(dbRowDefinitions, pFormName, "        ");
            this._ColsStream.WriteLine($"        {str2}{pFormName}.DataBind();");
            this._ColsStream.WriteLine("      }");
            this._ColsStream.WriteLine("    }");
            this._ColsStream.Close();
        }

        protected void btnCreateGV_Click(object sender, EventArgs e) => this.WriteView(true);

        protected void btnCreateDV_Click(object sender, EventArgs e) => this.WriteView(false);

        public struct dbTypesDef
        {
            public string typeName;
            public string typeNil;
            public string typeConvert;
        }
    }
}
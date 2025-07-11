using System;
using System.Data;
using System.Data.OleDb;
using System.Web.UI.WebControls;
using System.Configuration;


namespace TrackerDotNet.test
{
    public partial class ShowTableStruct : System.Web.UI.Page
    {
        //private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\SRC\ASP.net\TrackerDotNet\App_Data\QuaffeeTracker08.mdb";

        private string connectionString => ConfigurationManager.ConnectionStrings["Tracker08ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTableNames();
            }
        }

        private void LoadTableNames()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                DataTable tables = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                ddlTables.DataSource = tables;
                ddlTables.DataTextField = "TABLE_NAME";
                ddlTables.DataValueField = "TABLE_NAME";
                ddlTables.DataBind();

                ddlTables.Items.Insert(0, new ListItem("-- Select Table --", ""));
            }
        }

        protected void ddlTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = ddlTables.SelectedValue;
            if (!string.IsNullOrEmpty(selectedTable))
            {
                ShowTableStructure(selectedTable);
            }
        }

        private string GetOleDbTypeName(int typeCode)
        {
            switch (typeCode)
            {
                case 2: return "SmallInt";
                case 3: return "Integer";
                case 4: return "Single";
                case 5: return "Double";
                case 6: return "Currency";
                case 7: return "Date";
                case 11: return "Boolean";
                case 17: return "UnsignedTinyInt";
                case 72: return "GUID";
                case 128: return "Binary";
                case 129: return "Char";
                case 130: return "VarWChar";
                case 131: return "Decimal";
                default: return $"Unknown ({typeCode})";
            }
        }

        private void ShowTableStructure(string tableName)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                DataTable fullSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tableName, null });

                if (chkFullView.Checked)
                {
                    // Add friendly type name to full schema
                    if (!fullSchema.Columns.Contains("DATA_TYPE_NAME"))
                        fullSchema.Columns.Add("DATA_TYPE_NAME", typeof(string));

                    foreach (DataRow row in fullSchema.Rows)
                    {
                        row["DATA_TYPE_NAME"] = GetOleDbTypeName(Convert.ToInt32(row["DATA_TYPE"]));
                    }

                    gvStructure.DataSource = fullSchema;
                }
                else
                {
                    // Simplified view
                    DataTable simplifiedSchema = new DataTable();
                    simplifiedSchema.Columns.Add("COLUMN_NAME", typeof(string));
                    simplifiedSchema.Columns.Add("COLUMN_HASDEFAULT", typeof(string));
                    simplifiedSchema.Columns.Add("COLUMN_DEFAULT", typeof(string));
                    simplifiedSchema.Columns.Add("IS_NULLABLE", typeof(string));
                    simplifiedSchema.Columns.Add("DATA_TYPE_NAME", typeof(string));
                    simplifiedSchema.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(int));
                    simplifiedSchema.Columns.Add("NUMERIC_PRECISION", typeof(int));
                    simplifiedSchema.Columns.Add("DESCRIPTION", typeof(string));

                    foreach (DataRow row in fullSchema.Rows)
                    {
                        var newRow = simplifiedSchema.NewRow();
                        newRow["COLUMN_NAME"] = row["COLUMN_NAME"];
                        newRow["COLUMN_HASDEFAULT"] = row["COLUMN_HASDEFAULT"];
                        newRow["COLUMN_DEFAULT"] = row["COLUMN_DEFAULT"];
                        newRow["IS_NULLABLE"] = row["IS_NULLABLE"];
                        newRow["DATA_TYPE_NAME"] = GetOleDbTypeName(Convert.ToInt32(row["DATA_TYPE"]));
                        newRow["CHARACTER_MAXIMUM_LENGTH"] = row["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value ? Convert.ToInt32(row["CHARACTER_MAXIMUM_LENGTH"]) : 0;
                        newRow["NUMERIC_PRECISION"] = row["NUMERIC_PRECISION"] != DBNull.Value ? Convert.ToInt32(row["NUMERIC_PRECISION"]) : 0;
                        newRow["DESCRIPTION"] = row.Table.Columns.Contains("DESCRIPTION") ? row["DESCRIPTION"]?.ToString() : "";

                        simplifiedSchema.Rows.Add(newRow);
                    }

                    gvStructure.DataSource = simplifiedSchema;
                }

                gvStructure.DataBind();
            }
        }

    }


}


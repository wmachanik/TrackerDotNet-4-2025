// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.test.XMLtoSQL
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using TrackerDotNet.classes;

// #nullable disable --- not for this version of C#
namespace TrackerDotNet.test
{

    public partial class XMLtoSQL : Page
    {
        private const string CONST_DEFAULT_PREFIX = "SQLCommands";
        protected TextBox FileNameTextBox;
        protected Button GoButton;
        protected GridView gvSQLResults;
        protected Panel pnlSQLResults;

        private void SetDefaultFileName()
        {
            string folderPath = Server.MapPath("~/App_Data/");
            try
            {
                FileInfo[] files = new DirectoryInfo(folderPath).GetFiles("SQLCommands*.xml");
                int maxSuffix = -1;
                string selectedFile = "";

                foreach (FileInfo file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.Name);
                    string numberPart = fileName.Substring("SQLCommands".Length);
                    if (int.TryParse(numberPart, out int suffix))
                    {
                        if (suffix > maxSuffix)
                        {
                            maxSuffix = suffix;
                            selectedFile = file.FullName;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(selectedFile))
                {
                    this.FileNameTextBox.Text = selectedFile;
                }
                else
                {
                    this.FileNameTextBox.Text = "No matching XML file found in App_Data.";
                }
            }
            catch (Exception ex)
            {
                this.FileNameTextBox.Text = $"Error finding XML file: {ex.Message}";
            }
        }
        
        //private void SetDefaultFileName()
        //{
        //    string path = "~\\Tools";
        //    try
        //    {
        //        FileInfo[] files = new DirectoryInfo(this.Server.MapPath(path)).GetFiles("SQLCommands*.xml", SearchOption.TopDirectoryOnly);
        //        List<int> intList = new List<int>();
        //        if (files.Length <= 0)
        //            return;
        //        string str = files[0].FullName.Substring(0, files[0].FullName.IndexOf("SQLCommands") + "SQLCommands".Length);
        //        foreach (FileInfo fileInfo in files)
        //        {
        //            string s = fileInfo.FullName.Substring(str.Length, fileInfo.FullName.IndexOf(".") - str.Length);
        //            int result = 0;
        //            if (int.TryParse(s, out result))
        //                intList.Add(result);
        //        }
        //        intList.Sort();
        //        this.FileNameTextBox.Text = $"{str}{intList[intList.Count - 1].ToString()}.xml";
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
                return;
            this.SetDefaultFileName();
        }

        private void showMsgBox(string pTitle, string pMessage)
        {
            string script = $"showAppMessage('{pMessage}');";
            System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), pTitle, script, true);
        }

        protected void GoButton_Click(object sender, EventArgs e)
        {
            List<XMLtoSQL.SQLCommand> sqlCommandList = new List<XMLtoSQL.SQLCommand>();
            XmlReader xmlReader = XmlReader.Create(this.FileNameTextBox.Text.Replace("\\", "\\\\"));
            try
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "command")
                    {
                        XMLtoSQL.SQLCommand sqlCommand = new XMLtoSQL.SQLCommand();
                        sqlCommand.type = xmlReader.GetAttribute("type");
                        xmlReader.Read();
                        sqlCommand.sql = xmlReader.Value.Replace("\n", "");
                        sqlCommandList.Add(sqlCommand);
                    }
                }
                xmlReader.Close();
                for (int index = 0; index < sqlCommandList.Count; ++index)
                {
                    if (sqlCommandList[index].type == "select")
                    {
                        GridView child = new GridView();
                        DataSet dataSet = this.RunSelect(sqlCommandList[index].sql);
                        sqlCommandList[index].result = dataSet != null;
                        child.DataSource = (object)dataSet;
                        child.DataBind();
                        this.pnlSQLResults.Controls.Add((Control)child);
                    }
                    else if (sqlCommandList[index].type != "disabled")
                    {
                        sqlCommandList[index].errString = this.RunCommand(sqlCommandList[index].sql);
                        sqlCommandList[index].result = string.IsNullOrWhiteSpace(sqlCommandList[index].errString);
                    }
                    TrackerTools trackerTools = new TrackerTools();
                    string sessionErrorString = trackerTools.GetTrackerSessionErrorString();
                    if (!string.IsNullOrEmpty(sessionErrorString))
                    {
                        showMessageBox showMessageBox = new showMessageBox(this.Page, "err", sessionErrorString);
                        trackerTools.SetTrackerSessionErrorString("");
                    }
                }
                this.gvSQLResults.DataSource = (object)sqlCommandList;
                this.gvSQLResults.DataBind();
            }
            catch (Exception ex)
            {
                showMessageBox showMessageBox = new showMessageBox(this.Page, "Error", "File access error: \n" + ex.Message);
            }
            finally
            {
                xmlReader.Close();
            }
        }



        private DataSet RunSelect(string pSQL) => new TrackerDb().ReturnDataSet(pSQL);

        private string RunCommand(string pSQL) => new TrackerDb().ExecuteNonQuerySQL(pSQL);

        private class SQLCommand
        {
            private string _type;
            private string _sql;
            private string _errString;
            private bool _result;

            public SQLCommand()
            {
                this._type = "";
                this._sql = "";
                this._errString = "";
                this._result = false;
            }

            public string type
            {
                get => this._type;
                set => this._type = value;
            }

            public string sql
            {
                get => this._sql;
                set => this._sql = value;
            }

            public string errString
            {
                get => this._errString;
                set => this._errString = value;
            }

            public bool result
            {
                get => this._result;
                set => this._result = value;
            }
        }
    }
}
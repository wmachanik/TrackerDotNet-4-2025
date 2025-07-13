﻿// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Administration.ManageRoles
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Data;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

// #nullable disable --- not for this version of C#
namespace TrackerDotNet.Administration
{

    public class ManageRoles : Page
    {
        protected GridView gvRolesManagement;
        protected Label MsgLabel;
        protected TextBox RoleTextBox;
        protected Button CreateRoleButton;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Page.IsPostBack)
                return;
            this.BindRoles();
        }

        private void BindRoles()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Role Name");
            dataTable.Columns.Add("User Count");
            foreach (string allRole in Roles.GetAllRoles())
            {
                int length = Roles.GetUsersInRole(allRole).Length;
                string[] strArray = new string[2]
                {
        allRole,
        length.ToString()
                };
                dataTable.Rows.Add((object[])strArray);
            }
            this.gvRolesManagement.DataSource = (object)dataTable;
            this.gvRolesManagement.DataBind();
        }

        public void CreateRole_OnClick(object sender, EventArgs args)
        {
            string text = this.RoleTextBox.Text;
            try
            {
                if (Roles.RoleExists(text))
                {
                    this.MsgLabel.Text = $"Role '{this.Server.HtmlEncode(text)}' already exists. Please specify a different role name.";
                }
                else
                {
                    Roles.CreateRole(text);
                    this.MsgLabel.Text = $"Role '{this.Server.HtmlEncode(text)}' created.";
                    this.BindRoles();
                }
            }
            catch (Exception ex)
            {
                this.MsgLabel.Text = $"Role '{this.Server.HtmlEncode(text)}' <u>not</u> created.";
                this.Response.Write(ex.ToString());
            }
        }

        public void RenameRoleAndUsers(string OldRoleName, string NewRoleName)
        {
            string[] usersInRole = Roles.GetUsersInRole(OldRoleName);
            Roles.CreateRole(NewRoleName);
            Roles.AddUsersToRole(usersInRole, NewRoleName);
            Roles.RemoveUsersFromRole(usersInRole, OldRoleName);
            Roles.DeleteRole(OldRoleName);
        }

        public void UpdateRole_OnClick(object sender, EventArgs args) => this.BindRoles();

        protected virtual void gvRolesManagement_OnSelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
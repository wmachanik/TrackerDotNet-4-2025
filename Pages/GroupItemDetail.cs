﻿// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.Pages.GroupItemDetail
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackerDotNet.classes;
using TrackerDotNet.control;

#nullable disable
namespace TrackerDotNet.Pages;

public class GroupItemDetail : Page
{
  public const string CONST_QRYSTR_GROUPITEMID = "ItemTypeID";
  private const string CONST_SESSION_RETURNURL = "ReturnItemGroupURL";
  protected System.Web.UI.ScriptManager scmGroupDetail;
  protected UpdatePanel upnlGroupDetail;
  protected Label lblAddOrEditItem;
  protected TextBox tbxGroupItem;
  protected Label lblGroupItemID;
  protected TextBox tbxGroupDesc;
  protected TextBox tbxGroupShortName;
  protected Button btnAdd;
  protected Button btnUpdate;
  protected Button btnCancel;

  protected void Page_Load(object sender, EventArgs e)
  {
    if (this.IsPostBack)
      return;
    this.Session["ReturnItemGroupURL"] = this.Request.UrlReferrer == (Uri) null ? (object) "" : (object) this.Request.UrlReferrer.OriginalString.ToString();
    if (this.Request.QueryString["ItemTypeID"] == null)
      return;
    ItemTypeTbl itemTypeFromId = new ItemTypeTbl().GetItemTypeFromID(Convert.ToInt32(this.Request.QueryString["ItemTypeID"].ToString()));
    if (itemTypeFromId.ItemTypeID.Equals(-1))
      return;
    this.lblGroupItemID.Visible = true;
    this.btnAdd.Visible = false;
    this.btnUpdate.Visible = true;
    this.lblGroupItemID.Text = itemTypeFromId.ItemTypeID.ToString();
    this.tbxGroupItem.Text = itemTypeFromId.ItemDesc;
    this.tbxGroupDesc.Text = itemTypeFromId.ItemDetail;
    this.tbxGroupShortName.Text = itemTypeFromId.ItemShortName;
  }

  protected void ReturnToPrevPage()
  {
    string url = this.Session["ReturnItemGroupURL"].ToString();
    if (url.Length <= 0)
      return;
    this.Response.Redirect(url);
  }

  protected void btnAdd_Click(object sender, EventArgs e)
  {
    if (this.tbxGroupItem.Text.Equals(string.Empty))
    {
      showMessageBox showMessageBox1 = new showMessageBox(this.Page, "Error no item", "Please enter a Group Item Name");
    }
    else
    {
      ItemTypeTbl NewItemType = new ItemTypeTbl();
      if (NewItemType.GroupOfThisNameExists(this.tbxGroupItem.Text))
      {
        showMessageBox showMessageBox2 = new showMessageBox(this.Page, "name exists", $"Group Name: {this.tbxGroupItem.Text} Exists. Please enter a different Group Item Name");
      }
      else
      {
        SysDataTbl sysDataTbl = new SysDataTbl();
        NewItemType.ItemDesc = this.tbxGroupItem.Text;
        NewItemType.ItemDetail = this.tbxGroupDesc.Text;
        NewItemType.ItemEnabled = true;
        NewItemType.ItemsCharacteritics = "Group Item";
        NewItemType.ItemShortName = this.tbxGroupShortName.Text;
        NewItemType.ServiceTypeID = sysDataTbl.GetGroupItemTypeID();
        NewItemType.SortOrder = 15;
        showMessageBox showMessageBox3 = new showMessageBox(this.Page, "Status", NewItemType.InsertItem(NewItemType) ? "Group item added" : "Error adding group item");
        this.ReturnToPrevPage();
      }
    }
  }

  protected void btnUpdate_Click(object sender, EventArgs e)
  {
    if (this.tbxGroupItem.Text.Equals(string.Empty))
    {
      showMessageBox showMessageBox1 = new showMessageBox(this.Page, "Error no item", "Please enter a Group Item Name");
    }
    else
    {
      ItemTypeTbl NewItemType = new ItemTypeTbl();
      SysDataTbl sysDataTbl = new SysDataTbl();
      NewItemType.ItemDesc = this.tbxGroupItem.Text;
      NewItemType.ItemDetail = this.tbxGroupDesc.Text;
      NewItemType.ItemEnabled = true;
      NewItemType.ItemsCharacteritics = "Group Item-update";
      NewItemType.ItemShortName = this.tbxGroupShortName.Text;
      NewItemType.ServiceTypeID = sysDataTbl.GetGroupItemTypeID();
      NewItemType.SortOrder = 15;
      NewItemType.ItemTypeID = Convert.ToInt32(this.lblGroupItemID.Text);
      showMessageBox showMessageBox2 = new showMessageBox(this.Page, "Status", NewItemType.UpdateItem(NewItemType) ? "Group item update" : "Error updating group item");
      this.ReturnToPrevPage();
    }
  }

  protected void btnCancel_Click(object sender, EventArgs e) => this.ReturnToPrevPage();
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using TrackerDotNet.Classes;
using TrackerDotNet.Managers;
using System.Text.RegularExpressions;

namespace TrackerDotNet.Tools
{
    public partial class MessagesEditor : System.Web.UI.Page
    {
        private readonly MessagesResourceManager _manager = new MessagesResourceManager();
        private const string VIEWSTATE_DATA_KEY = "MessagesData";
        private const string VIEWSTATE_FILTER = "MessagesFilter";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!IsUserAdmin())
                {
                    DenyAccess();
                    return;
                }

                ResultsTitleLabel.Text = "Messages (Global Resource Editor)";
                pnlEditor.Visible = true;
                LoadAllData();
                BindGrid();
            }
        }

        private bool IsUserAdmin()
        {
            try
            {
                var user = HttpContext.Current?.User;
                if (user == null) return false;
                return user.IsInRole("Administrators") || user.IsInRole("Admin") || user.IsInRole("AgentManager");
            }
            catch
            {
                return false;
            }
        }

        private void DenyAccess()
        {
            pnlAccessDenied.Visible = true;
            pnlEditor.Visible = false;
            AppLogger.WriteLog(SystemConstants.LogTypes.System,
                "MessagesEditor: Access denied for user " + (HttpContext.Current?.User?.Identity?.Name ?? "unknown"));
        }

        private Dictionary<string, string> AllData
        {
            get { return (Dictionary<string, string>)ViewState[VIEWSTATE_DATA_KEY]; }
            set { ViewState[VIEWSTATE_DATA_KEY] = value; }
        }

        private string CurrentFilter
        {
            get { return (string)ViewState[VIEWSTATE_FILTER] ?? string.Empty; }
            set { ViewState[VIEWSTATE_FILTER] = value; }
        }
        private void LoadAllData()
        {
            try
            {
                // Use unified manager effective values
                AllData = _manager.LoadEffective();
                ltrlStatus.Text = "Loaded " + AllData.Count + " messages.";
            }
            catch (Exception ex)
            {
                ltrlStatus.Text = "Error loading messages: " + ex.Message;
                AppLogger.WriteLog(SystemConstants.LogTypes.System,
                    "MessagesEditor LoadAllData error: " + ex.Message);
                AllData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
        }
        private IEnumerable<KeyValuePair<string, string>> GetFiltered()
        {
            if (AllData == null)
                return Enumerable.Empty<KeyValuePair<string, string>>();

            if (string.IsNullOrWhiteSpace(CurrentFilter))
                return AllData.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase);

            string f = CurrentFilter.Trim();
            return AllData
                .Where(kv =>
                    kv.Key.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (kv.Value ?? string.Empty).IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase);
        }

        private void BindGrid()
        {
            // DO NOT reset EditIndex here – preserve whatever RowEditing set.
            gvMessages.DataSource = GetFiltered()
                .Select(kv => new { Key = kv.Key, Value = kv.Value })
                .ToList();

            gvMessages.DataBind();

            int count = GetFiltered().Count();
            ltrlStatus.Text = string.IsNullOrWhiteSpace(CurrentFilter)
                ? "Showing " + count + " messages."
                : "Filter '" + CurrentFilter + "' matched " + count + " messages.";

            upnlMessagesEditor.Update();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CurrentFilter = tbxSearch.Text;
            gvMessages.PageIndex = 0;
            gvMessages.EditIndex = -1;
            BindGrid();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            tbxSearch.Text = string.Empty;
            CurrentFilter = string.Empty;
            gvMessages.PageIndex = 0;
            gvMessages.EditIndex = -1;
            BindGrid();
        }

        protected void gvMessages_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMessages.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvMessages_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvMessages.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvMessages_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvMessages.EditIndex = -1;
            BindGrid();
        }
        private static readonly Regex BrTagRegex = new Regex(@"<\s*br\s*/?\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ScriptStyleBlockRegex = new Regex(@"<(script|style)\b.*?</\1\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex AnyTagRegex = new Regex(@"<[^>]+>", RegexOptions.Compiled);

        private static string SanitizeMessage(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Normalize newlines
            input = input.Replace("\r\n", "\n");

            // Remove script/style blocks entirely
            input = ScriptStyleBlockRegex.Replace(input, string.Empty);

            // Protect allowed <br> tags with tokens
            int brIndex = 0;
            var brMap = new Dictionary<string, string>();
            input = BrTagRegex.Replace(input, m =>
            {
                string token = "___BR" + (brIndex++) + "___";
                brMap[token] = "<br />";
                return token;
            });

            // Strip any remaining tags (do NOT HTML encode them)
            input = AnyTagRegex.Replace(input, string.Empty);

            // Restore <br /> tokens
            foreach (var kv in brMap)
            {
                input = input.Replace(kv.Key, kv.Value);
            }

            // Trim extra whitespace (but keep intentional line break tags)
            return input.Trim();
        }
        protected void gvMessages_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (!IsUserAdmin()) { DenyAccess(); return; }

            string key = gvMessages.DataKeys[e.RowIndex].Value.ToString();
            GridViewRow row = gvMessages.Rows[e.RowIndex];
            var txt = (TextBox)row.FindControl("txtEditValue");

            // Retrieve unvalidated value if you later re-enable validation globally
            string rawVal;
            if (txt != null)
            {
                // Prefer unvalidated collection (works when request validation deferred)
                try
                {
                    rawVal = Request.Unvalidated[txt.UniqueID] ?? txt.Text;
                }
                catch
                {
                    rawVal = txt.Text;
                }
            }
            else
            {
                rawVal = string.Empty;
            }

            string newVal = SanitizeMessage(rawVal);

            try
            {
                _manager.Update(key, newVal);
                LoadAllData();
                gvMessages.EditIndex = -1;
                BindGrid();
                ltrlStatus.Text = "Updated '" + key + "'.";
                new showMessageBox(this, "Status", ltrlStatus.Text);
                AppLogger.WriteLog(SystemConstants.LogTypes.System,
                    "MessagesEditor: Updated key '" + key + "'");
            }
            catch (Exception ex)
            {
                ltrlStatus.Text = "Update failed: " + ex.Message;
                AppLogger.WriteLog(SystemConstants.LogTypes.System,
                    "MessagesEditor update error for key '" + key + "': " + ex.Message);
            }
        }

        protected void gvMessages_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Nothing special now; values are HTML-encoded in markup to prevent breaking the table.
        }

    }
}
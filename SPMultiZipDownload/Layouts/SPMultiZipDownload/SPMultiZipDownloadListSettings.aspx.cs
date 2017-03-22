using System;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace SPMultiZipDownload.Layouts.SPMultiZipDownload
{
    public partial class SPMultiZipDownloadListSettings : LayoutsPageBase
    {
        private string _listId = string.Empty;
        

        protected void Page_Load(object sender, EventArgs e)
        {
            btn_Save.Click += btn_Save_Click;
            btn_Cancel.Click += btn_Cancel_Click;
            
            _listId = Request.QueryString["ListId"];
            if (string.IsNullOrWhiteSpace(_listId)) return;
            
            SPList currentList = SPContext.Current.Web.Lists[new Guid(_listId)];
            lbl_PageTitleInTitleArea.Text = "Enable multi downloads for " + "'" + currentList.Title + "'";
            if (!IsPostBack)
            {
                chbx_Enable.Checked = IsMultiDownloadEnable(currentList);
            }
        }

        void btn_Cancel_Click(object sender, EventArgs e)
        {
            ReturnToSettingPage();
        }

        void btn_Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_listId)) return;

            SPList spList = SPContext.Current.Web.Lists[new Guid(_listId)];
            if (chbx_Enable.Checked && IsMultiDownloadEnable(spList)) return;

            bool status = chbx_Enable.Checked;
            if (status)
            {
                AddCustomAction(spList);
            }
            else
            {
                DeleteCustomAction(spList);
            }
            ReturnToSettingPage();
        }

        private void ReturnToSettingPage()
        {
            string listSetingsUrl = SPContext.Current.Web.Url + string.Format("/_layouts/15/listedit.aspx?List={0}", _listId);
            Response.Redirect(listSetingsUrl);
        }

        private bool IsMultiDownloadEnable(SPList spList)
        {
            SPUserCustomActionCollection customActions = spList.UserCustomActions;
            return customActions.Any(customAction => customAction.Title == Const.DownloadCustomActionTitle && customAction.Location == "CommandUI.Ribbon");
        }

        private void DeleteCustomAction(SPList spList)
        {
            try
            {
                SPUserCustomActionCollection customActions = spList.UserCustomActions;
                foreach (var customAction in customActions.Where(customAction => customAction.Title == Const.DownloadCustomActionTitle))
                {
                    customAction.Delete();
                    spList.Update();
                    break;
                }
            }
            catch (Exception ex)
            {
                WriteExToLog(ex);
            }
        }

        private void AddCustomAction(SPList spList)
        {
            string lableText = "Multi download as ZIP";
            string urlToDownloadPage = SPContext.Current.Web.Url + "/_layouts/SPMultiZipDownload/SPMultiZipDownload.aspx?listId={SelectedListId}&amp;itemId={SelectedItemId}";
            try
            {
                SPUserCustomAction customAction = spList.UserCustomActions.Add();
                customAction.Title = Const.DownloadCustomActionTitle;
                customAction.Location = "CommandUI.Ribbon";
                customAction.Group = "SPMultiZipDownloadActionGroup";
                customAction.Rights = SPBasePermissions.ViewListItems;
                customAction.CommandUIExtension =
                    "<CommandUIExtension xmlns='http://schemas.microsoft.com/sharepoint/'>" +
                    "<CommandUIDefinitions>" +
                    "<CommandUIDefinition Location='Ribbon.Documents.Copies.Controls._children'>" +
                    "<Button Id='SPMultiZipDownloadButton' Command='SPMultiZipDownload.Button' " +
                    "Image32by32='/_layouts/15/images/SPMultiZipDownload/SPMultiZipDownload_32x32.png' " +
                    "Image16by16='/_layouts/15/images/SPMultiZipDownload/SPMultiZipDownload_16x16.png' " +
                    "TemplateAlias='o1' " +
                    "LabelText='"+ lableText +"'/>" +
                    "</CommandUIDefinition>" +
                    "</CommandUIDefinitions>" +
                    "<CommandUIHandlers>" +
                    "<CommandUIHandler " +
                    "Command='SPMultiZipDownload.Button' " +
                    "CommandAction=\"javascript:window.open('" + urlToDownloadPage + "','_self');\" " +
                    @"EnabledScript='javascript:var EnableDisable = function() {
                             this.clientContext = SP.ClientContext.get_current();
                             this.selectedItems = SP.ListOperation.Selection.getSelectedItems(this.clientContext);
                             var ci = CountDictionary(selectedItems);
                             return (ci > 0);
                           };
                           EnableDisable();' />" +
                    "</CommandUIHandlers>" +
                    "</CommandUIExtension>";
                customAction.Update();
            }
            catch (Exception ex)
            {
                WriteExToLog(ex);
            }
        }

        private void WriteExToLog(Exception ex)
        {
            SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(Const.SpMultiZipDownload, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
        }
    }
}

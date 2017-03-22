using System;
using System.Collections.Generic;
using System.Linq;
using Ionic.Zip;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;

namespace SPMultiZipDownload.Layouts.SPMultiZipDownload
{
    public partial class SPMultiZipDownload : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private SPWeb _currentSpWeb;
        protected SPWeb CurrentSpWeb
        {
            get { return _currentSpWeb ?? (_currentSpWeb = SPControl.GetContextWeb(Context)); }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string listId = Request.QueryString["listId"];
            string[] itemsId = Request.QueryString["itemId"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            SPList spList = CurrentSpWeb.Lists[new Guid(listId)];
            if (spList == null || itemsId.Length <= 0) return;
            
            var itemsForDownload = ProcessSelectedItems(itemsId, spList);
            if (itemsForDownload.Count > 0)
                DownloadItems(itemsForDownload);
        }

        private List<SPFile> ProcessSelectedItems(string[] itemsId, SPList spList)
        {
            List<SPFile> itemsForDownload = new List<SPFile>();
            foreach (string id in itemsId)
            {
                int s = Convert.ToInt32(id);
                SPListItem spListItem = spList.GetItemById(s);

                if (spListItem.FileSystemObjectType == SPFileSystemObjectType.Folder)
                {
                    var files = GetAllItemsFromFolder(spListItem);
                    if(files!= null) itemsForDownload.AddRange(files);
                }

                if (spListItem.FileSystemObjectType == SPFileSystemObjectType.File)
                    itemsForDownload.Add(spListItem.File);
            }
            return itemsForDownload;
        }

        private List<SPFile> GetAllItemsFromFolder(SPListItem spListFolder)
        {
            if (spListFolder.FileSystemObjectType != SPFileSystemObjectType.Folder || spListFolder.Folder == null) return null;
            SPQuery query = new SPQuery();
            query.Folder = spListFolder.Folder;
            query.Query =
               @"<Where>
                  <Eq>
                     <FieldRef Name='FSObjType' />
                     <Value Type='Integer'>0</Value>
                  </Eq>
               </Where>";
            query.ViewAttributes = "Scope='RecursiveAll'";
            SPListItemCollection listItems = spListFolder.ParentList.GetItems(query);
            return (from SPListItem item in listItems select item.File).ToList();
        }

        private void DownloadItems(List<SPFile> downloadItems)
        {
            try
            {
                Response.Clear();
                Response.BufferOutput = false;
                string archiveName = String.Format("archive-{0}.zip", DateTime.Now);
                Response.ContentType = "application/zip";
                Response.AddHeader("content-disposition", "inline; filename=\"" + archiveName + "\"");

                var listFolderName = downloadItems[0].Item.ParentList.RootFolder.Name;

                using (var ms = new System.IO.MemoryStream())
                {
                    using (ZipFile zip = new ZipFile())
                    {
                        foreach (var downloadItem in downloadItems)
                        {
                            var folderPath = downloadItem.Url
                                .Replace('/', '\\')
                                .Replace(listFolderName, "")
                                .TrimStart('\\');
                            zip.AddEntry(folderPath, downloadItem.OpenBinaryStream());
                        }
                        zip.Save(ms);
                    }
                    ms.Position = 0;
                    var b = new byte[1024];
                    int n;
                    while ((n = ms.Read(b, 0, b.Length)) > 0)
                        Response.OutputStream.Write(b, 0, n);
                }
                Response.Close();
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(Const.SpMultiZipDownload, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }
    }
}

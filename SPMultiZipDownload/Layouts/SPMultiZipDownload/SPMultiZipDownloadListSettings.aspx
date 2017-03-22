<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SPMultiZipDownloadListSettings.aspx.cs" Inherits="SPMultiZipDownload.Layouts.SPMultiZipDownload.SPMultiZipDownloadListSettings" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    	<table class="ms-propertysheet">
		<tr>
			<td>
				<asp:Label ID="lbl_Text1" runat="server" Text="Enable multi downloads"></asp:Label></td>
			<td style="text-align: right;">
				<asp:CheckBox ID="chbx_Enable" runat="server" />
			</td>
		</tr>
		<td class="ms-spaceBetContentAndButton" colspan="2" style="height: 20px;"></td>
		<tr>
			<td></td>
			<td>
			    <table>
			        <tr>
			            <td>
			                <asp:Button ID="btn_Save" runat="server" Text="Save"/>
			            </td>
                        <td>
			                <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" />
			            </td>
			        </tr>
			    </table>
			</td>
		</tr>
	</table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Enable multi downloads
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
<asp:Label runat="server" ID="lbl_PageTitleInTitleArea"></asp:Label>
</asp:Content>

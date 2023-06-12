<%@ Page Language="C#" AutoEventWireup="true" CodeFile="guest.aspx.cs" Inherits="guest" Title="New guest client" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="padding-left: 10px; margin-bottom: 10px">
        <h3>owner <asp:HyperLink CssClass="h3_name provider_color" ID="HL_Owner" runat="server"/></h3>
        <h3 style="padding-left:10px;">domain <asp:HyperLink CssClass="h3_name domain_color" ID="HL_Domain" runat="server"/></h3>        
    </div>

    <table class="wide_table" style="margin-bottom: 20px;"><tr>
        <td class="wide_table" style="width:150px;"/>
        <td class="wide_table" style="padding-top: 0px; vertical-align:top;">
            <h2 style="text-align: center;">new guest client</h2>
        </td>
        <td class="wide_table" style="width:150px;">
            <table class="wide_table"><tr>
                <td class="wide_table" style="width:100%"></td>
                <td class="wide_table" style="padding-right:10px;">
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Refresh" runat="server" Text="refresh" onclick="Refresh_Click" />
                    </div>
                </td>                 
            </tr></table>    
        </td>
    </tr></table>  

    <asp:PlaceHolder ID="PH_Site" runat="server"/>
</asp:Content>
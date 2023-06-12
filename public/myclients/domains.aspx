<%@ Page Language="C#" AutoEventWireup="true" CodeFile="domains.aspx.cs" Inherits="public_myclients_domains" Title="Owners and Domains - My Guest Clients"  MasterPageFile="~/Site.master" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="padding-left: 10px;">
        <h3>my guest clients</h3>
    </div>

    <table class="wide_table" style="margin-bottom: 15px"><tr>
        <td class="wide_table" style="width: 150px;"/>
        <td class="wide_table">
            <h1 style="text-align:center;">owners and domains</h1>                 
        </td>                            
        <td class="wide_table" style="width: 150px; padding-right: 10px;">
            <table class="wide_table"><tr>
                <td class="wide_table" style="width:100%"></td>
                <td class="wide_table">                        
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Refresh" runat="server" Text="refresh" onclick="Refresh_Click" />
                    </div>
                </td>
            </tr></table>
        </td>
    </tr></table>   
        
    <div style="height: 8px; background-color: #A2C5D3;"></div>
    <div style="border: 1px solid #A2C5D3; padding: 10px">
        <asp:PlaceHolder ID="PH_Domains" runat="server"></asp:PlaceHolder>
    </div>
    <span style="display:block; text-align: left; padding-top: 4px; padding-left: 10px; color: gray; font-size: 0.9em;">
        The list of service owners and their domains containing your guest clients
    </span>
</asp:Content>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="domains_default" MasterPageFile="~/Site.master"  
    MaintainScrollPositionOnPostback="true" EnableViewState="false" Title="My domains" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1 style="text-align:center">my domains</h1>            

    <table class="wide_table" style="margin-bottom: 10px"><tr>
        <td class="wide_table" style="padding-left: 8px">
            <table class="wide_table"><tr>
                <td class="wide_table">
                    <div class="SubmitButton Blue">
                        <asp:Button ID="B_NewDomain" runat="server" Text="new domain" onclick="NewDomain_Click" />
                    </div>
                </td>                 
                <td class="wide_table" style="width:100%"></td>                
            </tr></table>        
        </td>                
        <td class="wide_table" style="width:100%"></td>
        <td class="wide_table" style="padding-right: 8px">
            <table class="wide_table"><tr>
                <td class="wide_table" style="width:100%"></td>
                <td class="wide_table">
                    <asp:Panel ID="P_SwitchModeButton" CssClass="SubmitButton Gray" runat="server">
                        <asp:Button ID="B_SwitchMode" runat="server" Text="edit" style="padding-left:15px; padding-right:15px" onclick="SwitchMode_Click" />
                    </asp:Panel>
                </td>
            </tr></table>
        </td>
        <td class="wide_table"></td>
    </tr></table>    
    
    <div style="width: 794px; margin-top: 5px; border: 1px solid #A2C5D3;">
        <div style="height: 8px; background-color: #A2C5D3;"></div>
        <asp:PlaceHolder ID="PH_DomainList" runat="server"/>                    
    </div>    
</asp:Content>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="contact.aspx.cs" Inherits="contacts_contact" MasterPageFile="~/Site.master"  MaintainScrollPositionOnPostback="true" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    <table class="wide_table"><tr>
        <td class="wide_table" style="width:150px; vertical-align:top;">
            <table class="auto_table"><tr>
                <td class="auto_table" style="padding-left:5px;">
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Back" runat="server" Text="back" onclick="Back_Click" />
                    </div>
                </td>                 
            </tr></table>        
        </td>
        <td class="wide_table" style="padding-top: 0px; vertical-align:top;">
            <h2 style="text-align: center;">contact <asp:Label CssClass="h2_name" ID="L_ContactName" runat="server"></asp:Label></h2>
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

    <table class="wide_table" style="margin-top: 16px"><tr>
        <td class="wide_table_vtop">
            <asp:Panel ID="P_ContactDomains" Visible="true" Style="width:397px; margin-right: 2px;" runat="server">
                <h4 style="text-align:center; padding-bottom: 3px;">
                    contact domains
                </h4>
                <div style="height: 8px; background-color: #A2C5D3;"></div> 
                <div style="border: 1px solid #A2C5D3; padding: 10px">
                    <asp:PlaceHolder ID="PH_ContactDomains" runat="server"></asp:PlaceHolder>
                </div>
                <span style="display:block; text-align: left; padding-top: 4px; padding-left: 10px; color: gray; font-size: 0.9em;">The list of domains your contact shares with you</span>
            </asp:Panel>
        </td>
        <td class="wide_table_vtop">
            <asp:Panel ID="P_MyDomains" Visible="false" Style="width:397px;" runat="server">
                <h4 style="text-align:center; padding-bottom: 3px;">
                    my shared domains
                </h4>
                <div style="height: 8px; background-color: #A2C5D3;"></div> 
                <div style="border: 1px solid #A2C5D3; padding: 10px">
                    <asp:PlaceHolder ID="PH_MyDomains" runat="server"></asp:PlaceHolder>                    
                </div>
                <span style="display:block; text-align: left; padding-top: 4px; padding-left: 10px; color: gray; font-size: 0.9em;">The list of domains you share with your contact</span>
            </asp:Panel>
        </td>
    </tr></table>    
    <asp:PlaceHolder ID="PH_Warning" runat="server"></asp:PlaceHolder>
</asp:Content>
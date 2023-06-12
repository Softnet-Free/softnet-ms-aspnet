<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="contacts_default" MasterPageFile="~/Site.master" Title="My contacts" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1 style="text-align:center">my contacts</h1>

    <table class="wide_table" style="margin-bottom: 10px"><tr>
        <td class="wide_table" style="padding-left: 8px">
            <table class="wide_table"><tr>
                <td class="wide_table">
                    <div class="SubmitButton Blue">
                        <asp:Button ID="B_NewInvitation" runat="server" Text="new invitation" onclick="NewInvitation_Click" />
                    </div>
                </td>                 
                <td class="wide_table" style="width:100%"></td>                
            </tr></table>        
        </td>                
        <td class="wide_table" style="width:100%;"></td>
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
        <td></td>
    </tr></table>    

    <asp:Panel ID="P_ContactListViewer" Visible="false" Style="width: 794px; margin-top: 5px; border: 1px solid #A2C5D3;" runat="server">    
        <div style="height: 8px; background-color: #A2C5D3;"></div>
        <asp:PlaceHolder ID="PH_ContactListViewer" runat="server"/>
    </asp:Panel>
     
    <asp:Table ID="Tb_ContactListEditor" Visible="false" runat="server" CssClass="wide_table" style="border: 1px solid #A2C5D3; width: 794px; margin-top: 5px;">
        <asp:TableHeaderRow style="background-color: #A2C5D3;">
            <asp:TableHeaderCell Style="width:55px; border-bottom: 1px solid #A2C5D3"></asp:TableHeaderCell>
            <asp:TableHeaderCell Style="width:240px; text-align: left; color: White; border-bottom: 1px solid #A2C5D3; padding-left: 15px;">contact</asp:TableHeaderCell>  
            <asp:TableHeaderCell Style="width:180px; text-align: left; color: White; border-bottom: 1px solid #A2C5D3; padding-left: 15px;">user default name</asp:TableHeaderCell>          
            <asp:TableHeaderCell Style="text-align: left; color: White; border-bottom: 1px solid #A2C5D3; padding-left: 15px;">partner</asp:TableHeaderCell>
            <asp:TableHeaderCell Style="width:23px; border-bottom: 1px solid #A2C5D3;"></asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>
    <asp:Panel ID="P_ContactListHints" Visible="false" style="padding-left:10px; padding-top:10px;" runat="server"/>

    <asp:Panel ID="P_InvitationsViewer" Visible="false" runat="server">
        <h2 style="margin: 20px 0px 5px 40px;">sent invitations</h2>
        <div style="width: 794px; border: 1px solid #A2C5D3;"> 
            <div style="height: 8px; background-color: #A2C5D3;"></div>       
            <asp:PlaceHolder ID="PH_InvitationsViewer" runat="server"/>
        </div> 
    </asp:Panel>

    <asp:Panel ID="P_InvitationsEditor" Visible="false" runat="server">
        <h2 style="margin: 30px 0px 5px 40px;">sent invitations</h2>
        <div style="height: 8px; background-color: #A2C5D3;"></div>
        <asp:Table ID="Tb_InvitationsEditor" runat="server" CssClass="wide_table" style="border: 1px solid #A2C5D3;"></asp:Table> 
    </asp:Panel>
</asp:Content>

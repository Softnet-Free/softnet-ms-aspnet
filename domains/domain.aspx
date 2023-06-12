<%@ Page Language="C#" AutoEventWireup="true" CodeFile="domain.aspx.cs" Inherits="domain" MasterPageFile="~/Site.master"  MaintainScrollPositionOnPostback="true" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Panel ID="P_DeleteDomain" Style="background-color: #ffd6c3; text-align:center; padding-top: 10px; padding-bottom: 15px; margin-bottom: 10px;" Visible="false" runat="server">    
        <span class="critical_action" style="font-size: 1.2em;">Do you really want to delete the domain?</span>
        <table class='wide_table' style="margin-top:10px;"><tr>
                <td style="width:45%;"></td>
                <td>
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Cancel" runat="server" Text="Cancel" onclick="CancelDeletingDomain_Click" />
                    </div>
                </td>
                <td style="width:10%;"></td>
                <td>
                    <div class="SubmitButton RedOrange">
                        <asp:Button ID="B_Delete" runat="server" Text="Delete" onclick="DeleteDomain_Click" />
                    </div>
                </td>
                <td style="width:45%;"></td>
            </tr></table>
    </asp:Panel>

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
            <h2 style="text-align: center;">domain <asp:Label CssClass='h2_name domain_color' ID="L_Domain" runat="server"></asp:Label></h2>        
        </td>
        <td class="wide_table" style="width:150px;"></td>
    </tr></table>  
    
    <table class="wide_table" style="margin-top: 10px;"><tr>
        <td style="padding-left:5px;">
            <div class="SubmitButton Blue">
                <asp:Button ID="B_NewSite" runat="server" Text="new site" onclick="NewSite_Click" />
            </div>
        </td>
        <td style="padding-left:5px;">
            <div class="SubmitButton Blue">
                <asp:Button ID="B_NewUser" runat="server" Text="new user" onclick="NewUser_Click" />
            </div>
        </td>
        <td style="width:100%"></td>
        <td style="padding-right:10px;">
            <div class="SubmitButton Gray">
                <asp:Button ID="B_Refresh" runat="server" Text="refresh" onclick="Refresh_Click" />
            </div>
        </td>
    </tr></table>
    <div style="padding-left: 50px; margin-top: 10px;">
        <h1>user list</h1>
    </div> 
    <br />
    <div style="border-top: 1px solid #A2C5D3; border-left: 1px solid #A2C5D3; border-right: 1px solid #A2C5D3">
        <asp:Table ID="Tb_Users" runat="server" CssClass="wide_table">
            <asp:TableHeaderRow style="background-color: #A2C5D3; height: 22px">
                <asp:TableHeaderCell Style="width:50px; border-bottom: 1px solid #A2C5D3"></asp:TableHeaderCell>
                <asp:TableHeaderCell Style="width:200px; text-align: left; color: White; border-bottom: 1px solid #A2C5D3; padding-left: 15px;">user</asp:TableHeaderCell>  
                <asp:TableHeaderCell Style="width:220px; text-align: left; color: White; border-bottom: 1px solid #A2C5D3; padding-left: 15px;">contact</asp:TableHeaderCell>          
                <asp:TableHeaderCell Style="text-align: left; color: White; border-bottom: 1px solid #A2C5D3; padding-left: 15px;">status</asp:TableHeaderCell>
                <asp:TableHeaderCell Style="width:60px; text-align: left; color: White; border-bottom: 1px solid #A2C5D3">clients</asp:TableHeaderCell>                
                <asp:TableHeaderCell Style="width:23px; border-bottom: 1px solid #A2C5D3;"></asp:TableHeaderCell>
            </asp:TableHeaderRow>
        </asp:Table>
    </div>    
    <asp:Panel ID="P_UserListHints" Visible="false" style="padding-left:10px; padding-top:10px;" runat="server"/>
    <asp:Label ID="L_UserListError" Visible="false" style="display:block; color:#FF3300; margin-left:10px; margin-top:20px;" runat="server"/>    
    <asp:Panel ID="P_ViewUserListButton" Visible="false" runat="server">
        <table class="wide_table" style="margin-top: 25px; margin-bottom: 10px"><tr>                                   
            <td style="width:15%"></td>
            <td>
                <div class="SubmitButton Gray">
                    <asp:Button ID="B_ViewUserList" runat="server" Text="cancel" style="padding-left:15px; padding-right:15px" onclick="ViewUserList_Click" />
                </div>
            </td>
            <td style="width:85%"></td>
        </tr></table>    
    </asp:Panel>     
    <asp:Panel ID="P_Sites" Visible="false" style="margin-top:20px;" runat="server">
        <div style="padding-left: 50px;">
            <h1>site list</h1>
        </div>  
        <br />         
        <asp:PlaceHolder ID="PH_SiteList" runat="server"></asp:PlaceHolder>    
    </asp:Panel>
</asp:Content>

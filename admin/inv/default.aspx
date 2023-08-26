<%--
*   Copyright 2023 Robert Koifman
*   
*   Licensed under the Apache License, Version 2.0 (the "License");
*   you may not use this file except in compliance with the License.
*   You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
*   Unless required by applicable law or agreed to in writing, software
*   distributed under the License is distributed on an "AS IS" BASIS,
*   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*   See the License for the specific language governing permissions and
*   limitations under the License.
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="admin_inv_default" Title="Invitations - Admin" MasterPageFile="~/Site.master"  MaintainScrollPositionOnPostback="true" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1 style="text-align: center;">invitations</h1>
    
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
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Refresh" runat="server" Text="refresh" onclick="Refresh_Click" />
                    </div>
                </td>
            </tr></table>
        </td>
    </tr></table>    

    <asp:Panel ID="P_Invitations" Visible="false" runat="server">
        <asp:Table ID="T_Invitations" CssClass="wide_table" style="border: 1px solid #A2C5D3;" runat="server">
            <asp:TableHeaderRow Style="background-color: #A2C5D3; height: 22px;">                
                <asp:TableHeaderCell Style="width:50px; border-bottom: 1px solid #A2C5D3; padding: 0px;"></asp:TableHeaderCell>
                <asp:TableHeaderCell Style="width:420px; text-align: left; color: White; border-bottom: 1px solid #A2C5D3; padding: 0px; padding-left: 25px;">email&nbsp;&nbsp;+&nbsp;&nbsp;description</asp:TableHeaderCell> 
                <asp:TableHeaderCell Style="text-align: left; color: White; border-bottom: 1px solid #A2C5D3; padding: 0px;">assign 'Provider' role on registration</asp:TableHeaderCell>                                             
                <asp:TableHeaderCell Style="width:23px; border-bottom: 1px solid #A2C5D3;"></asp:TableHeaderCell>
            </asp:TableHeaderRow>
        </asp:Table>        
    </asp:Panel>

    <asp:Panel ID="P_AcceptedInvitations" Visible="false" Style="margin-top:25px; margin-bottom:5px;" runat="server">
        <h2 style="margin: 0px 0px 5px 40px;">accepted invitations</h2>
        <asp:Table ID="T_AcceptedInvitations" CssClass="wide_table" style="border: 1px solid #A2C5D3;" runat="server">
            <asp:TableHeaderRow Style="background-color: #A2C5D3; height: 22px;">                
                <asp:TableHeaderCell Style="border-bottom: 1px solid #A2C5D3; padding: 0px;"></asp:TableHeaderCell>
                <asp:TableHeaderCell Style="width:420px; text-align: left; color: White; border-bottom: 1px solid #A2C5D3; padding: 0px; padding-left: 25px;">email&nbsp;&nbsp;+&nbsp;&nbsp;description</asp:TableHeaderCell>                
                <asp:TableHeaderCell Style="text-align: left; color: White; border-bottom: 1px solid #A2C5D3; padding: 0px; padding-left: 15px;">user</asp:TableHeaderCell>  
                <asp:TableHeaderCell Style="width:23px; border-bottom: 1px solid #A2C5D3;"></asp:TableHeaderCell>
            </asp:TableHeaderRow>
        </asp:Table>

        <table class="wide_table" style="margin-top: 10px"><tr>
            <td class="wide_table" style="width:100%;"></td>
            <td class="wide_table" style="padding-right: 40px;">                
                <div class="SubmitButtonMini RedOrange">
                    <asp:Button ID="B_ClearAIL" runat="server" Text="clear list" onclick="ClearAcceptedInvitationList_Click" />
                </div>
            </td>
        </tr></table>    
    </asp:Panel>

    <asp:Panel ID="P_ExpiredInvitations" Visible="false" Style="margin-bottom:30px;" runat="server">
        <h2 style="margin: 0px 0px 5px 40px;">expired invitations</h2>
        <asp:Table ID="T_ExpiredInvitations" CssClass="wide_table" style="border: 1px solid #A2C5D3;" runat="server">
            <asp:TableHeaderRow Style="background-color: #A2C5D3; height: 22px;">
                <asp:TableHeaderCell Style="border-bottom: 1px solid #A2C5D3; padding: 0px;"></asp:TableHeaderCell>
                <asp:TableHeaderCell Style="text-align: left; color: White; border-bottom: 1px solid #A2C5D3; padding: 0px; padding-left: 25px;">email&nbsp;&nbsp;+&nbsp;&nbsp;description</asp:TableHeaderCell>                
                <asp:TableHeaderCell Style="width:23px; border-bottom: 1px solid #A2C5D3;"></asp:TableHeaderCell>
            </asp:TableHeaderRow>
        </asp:Table>

        <table class="wide_table" style="margin-top: 10px"><tr>
            <td class="wide_table" style="width:100%;"></td>
            <td class="wide_table" style="padding-right: 40px">                
                <div class="SubmitButtonMini RedOrange">
                    <asp:Button ID="B_ClearEIL" runat="server" Text="clear list" onclick="ClearExpiredInvitationList_Click" />
                </div>
            </td>
        </tr></table>    
    </asp:Panel>
</asp:Content>

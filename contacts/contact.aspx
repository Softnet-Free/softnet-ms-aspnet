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
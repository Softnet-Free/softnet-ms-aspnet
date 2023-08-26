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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="domain.aspx.cs" Inherits="public_services_domain" MasterPageFile="~/Site.master" EnableViewState="false" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="padding-left: 10px; margin-bottom: 10px">
        <h3>public services</h3>           
        <h3 style="padding-left: 10px;">owner <asp:HyperLink CssClass="h3_name provider_color" ID="HL_Owner" runat="server"/></h3>
    </div>

    <table class="wide_table"><tr>
        <td class="wide_table" style="width:150px; vertical-align:top;">
            <table class="auto_table"><tr>
                <td class="auto_table" style="padding-left:5px;">
                    <asp:Panel ID="P_BackButton" runat="server" CssClass="SubmitButton Gray">
                        <asp:Button ID="B_Back" runat="server" Text="back" onclick="Back_Click" />
                    </asp:Panel>
                </td>                 
            </tr></table>        
        </td>
        <td class="wide_table" style="padding-top: 0px; vertical-align:top;">
            <h3 style="text-align: center">domain <asp:Label CssClass="h3_name domain_color" ID="L_Domain" runat="server"/></h3>     
        </td>
        <td class="wide_table" style="width:150px; padding-right:10px; vertical-align:top;">
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
    <br />       
    <asp:PlaceHolder ID="PH_Sites" runat="server"/>
</asp:Content>
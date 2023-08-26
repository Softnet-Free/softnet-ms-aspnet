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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="guest.aspx.cs" Inherits="public_clients_guest" Title="New Guest Client by Email"  MasterPageFile="~/Site.master" EnableViewState="false" %>

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
            <h2 style="text-align: center;">
                new guest client <br/>
                by <asp:Label CssClass="h2_name turquoise_text" ID="L_ConsumerEMail" runat="server"></asp:Label>
            </h2>
        </td>
        <td class="wide_table" style="width:150px;"/>
    </tr></table>    
    <asp:PlaceHolder ID="PH_Site" runat="server"/>
</asp:Content>
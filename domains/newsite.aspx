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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="newsite.aspx.cs" Inherits="newsite" Title="New Site" MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="padding-left: 10px; padding-bottom: 10px">
        <h3>
            <span>domain </span>
            <asp:HyperLink CssClass="h3_name domain_color" ID="HL_Domain" runat="server"/>
        </h3>
    </div>  

    <table class="auto_table"><tr>
        <td style="padding-left:5px;">
            <div class="SubmitButton Gray">
                <asp:Button ID="B_Back" runat="server" Text="back" onclick="Back_Click" />
            </div>
        </td>
    </tr></table>

    <h1 style="text-align: center;">new site</h1>
    
    <div style="padding: 20px 10px 10px 10px">
        <table class="wide_table"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">
            <div style="width: 560px; padding: 5px; background-color: #A2C5D3">
                <table class="wide_table"><tr>
                    <td class="wide_table" style="padding-bottom: 5px">
                        <span style="color: white">&nbsp;description</span> <span style="color: #3C6C80">(<span style="color: white">optional</span>)</span>
                        <asp:TextBox ID="TB_SiteDescription" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:450px; margin: 0px; margin-top: 3px; padding: 3px;"></asp:TextBox>
                    </td>
                    <td class="wide_table" style="width:100%">
                    </td>
                    <td class="wide_table" style="padding-right: 10px; padding-top: 15px; padding-bottom: 5px">
                        <div class="SubmitButtonEmbodied LightBlue">
                            <asp:Button ID="B_CreateSite" runat="server" Text="Create" onclick="CreateSite_Click" />
                        </div>
                    </td>
                </tr></table>
                <asp:Label ID="L_Error" Visible="false" runat="server" Text="" Style="display:block; margin-top:10px; color:#9F0000"></asp:Label>
            </div>  
        </td>
        <td class="wide_table" style="width: 50%"></td>
        </tr></table>
    </div>    
</asp:Content>
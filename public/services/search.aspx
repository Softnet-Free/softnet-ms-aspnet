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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="search.aspx.cs" Title="Public Services" Inherits="public_services_search"  MasterPageFile="~/Site.master" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="padding-left: 10px;">
        <h3>public services</h3>    
    </div>
    <table class="wide_table"><tr>
    <td class="wide_table" style="width: 50%"></td>
    <td class="wide_table">
        <div style="width: 700px;">
            <h1 style="text-align:center">search for owner</h1>
            <div style="margin-top:15px; background-color: #A2C5D3">                    
                <table class="wide_table"><tr>
                    <td class="wide_table" style="width: 470px; padding: 3px 5px 3px 5px;">
                        <asp:TextBox ID="TB_SearchFilter" AutoComplete="off" runat="server" Style="font-size:1.1em; border-width: 0px; outline:none; width:464px; margin: 0px; padding: 3px;"></asp:TextBox>
                    </td>
                    <td class="wide_table" style="text-align:right; padding: 3px 5px;">
                        <table class="auto_table"><tr>
                            <td class="auto_table">
                                <div class="SubmitButtonEmbodied LightBlue" style="width: 60px">
                                    <asp:Button ID="B_Search" runat="server" Text="Go" onclick="Search_Click" />
                                </div>
                            </td>
                            <td class="auto_table" style="padding-left: 8px; width: 64px">
                                <asp:Panel Visible="false" CssClass="SubmitButtonEmbodied LightBlue" style="width: 60px" ID="P_PrevButton" runat="server">
                                    <asp:Button ID="B_Prev" runat="server" Text="<<" onclick="Prev_Click" />
                                </asp:Panel>
                            </td>
                            <td class="auto_table" style="padding-left: 8px; width: 64px">
                                <asp:Panel Visible="false" CssClass="SubmitButtonEmbodied LightBlue" style="width: 60px" ID="P_NextButton" runat="server">
                                    <asp:Button ID="B_Next" runat="server" Text=">>" onclick="Next_Click" />
                                </asp:Panel>
                            </td>
                        </tr></table>
                    </td>
                </tr></table>
            </div>
            <div style="margin-top: 5px; margin-bottom: 10px;">
                <asp:PlaceHolder ID="PH_FoundUsers" runat="server"></asp:PlaceHolder>
            </div>
        </div>
    </td>
    <td class="wide_table" style="width: 50%"></td>
    </tr></table>    
</asp:Content>
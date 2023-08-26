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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="user.aspx.cs" Inherits="admin_mgt_user" MasterPageFile="~/Site.master"  EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
        
    <table class="wide_table" style="margin-top:10px;"><tr>
        <td class="wide_table" style="width:199px; vertical-align:top;">
            <table class="wide_table"><tr>
                <td class="wide_table" style="padding-left:5px;">
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Back" runat="server" Text="back" onclick="Back_Click" />
                    </div>
                </td>                 
                <td class="wide_table" style="width:100%"></td>                
            </tr></table>        
        </td>
        <td class="wide_table" style="width: 400px; padding-top: 0px; vertical-align:top;">
            <h1 style="text-align: center;">user management</h1>
        </td>
        <td class="wide_table" style="width:199px; vertical-align:top;">
            <table  class="wide_table"><tr>
                <td class="wide_table" style="width:100%"></td>
                <td class="wide_table" style="padding-right:10px;">
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Refresh" runat="server" Text="refresh" onclick="Refresh_Click" />
                    </div>
                </td>
            </tr></table>
        </td>
        <td></td>
    </tr></table>  

    <table class="wide_table"><tr>
    <td class="wide_table" style="width: 50%"></td>
    <td class="wide_table">
        <div style="width: 500px; border: 1px solid #B9B9F9; margin-top: 20px; padding: 10px;">
            <div style="padding-bottom: 15px;">
                <table class="wide_table">
                    <tr>
                        <td class="wide_table" style="width: 100px; text-align: right; padding-right: 10px;">
                            <span style="color: #C5972F; font-size: 1.1em; white-space:nowrap;">User Name:</span>
                        </td>
                        <td class="wide_table">
                            <asp:Label ID="L_UserName" style="color: black; font-size: 1.1em;" runat="server" />
                        </td>
                    </tr>         
                    <tr>
                        <td class="wide_table" style="width: 100px; text-align: right; padding-top: 5px; padding-right: 10px;">
                            <span style="color: #C5972F; font-size: 1.1em; ; white-space:nowrap;">Account Name:</span>
                        </td>
                        <td class="wide_table" style="padding-top: 5px;">
                            <asp:Label ID="L_AccountName" style="color: black; font-size: 1.1em;" runat="server" />
                        </td>
                    </tr>   
                    <tr>
                        <td class="wide_table" style="width: 100px; text-align: right; padding-top: 5px; padding-right: 10px;">
                            <span style="color: #C5972F; font-size: 1.1em; white-space:nowrap;">E-mail:</span>
                        </td>
                        <td class="wide_table" style="padding-top: 5px;">
                            <asp:Label ID="L_EMail" style="color: black; font-size: 1.1em;" runat="server" />
                        </td>
                    </tr>            
                </table>
            </div>

            <div class="site_block">
                <div class="site_block_header"><span class="yellow_text">authority</span></div>
                <div class="site_block_item">
                    <table class="wide_table"><tr>
                        <td class="wide_table" style="width: 150px;">
                            <asp:Label ID="L_Authority" style="font-size: 1.1em; color: #C5972F; white-space:nowrap;" runat="server" />
                        </td>
                        <td class="wide_table" style="width: 20px;">                            
                        </td>
                        <td class="wide_table">
                            <table  class="wide_table"><tr>                                
                                <td class="wide_table" style="padding-right:10px;">
                                    <asp:PlaceHolder ID="PH_AuthButton" runat="server"/>
                                </td>
                                <td class="wide_table" style="width:100%"></td>
                            </tr></table>
                        </td>                        
                    </tr></table>
                </div>       
            </div>

            <div class="site_block">
                <div class="site_block_header"><span class="yellow_text">status</span></div>
                <div class="site_block_item">
                    <table class="wide_table"><tr>
                        <td class="wide_table" style="width: 150px;">
                            <asp:Label ID="L_Status" style="font-size: 1.1em; color: #C5972F; white-space:nowrap;" runat="server" />
                        </td>
                        <td class="wide_table" style="width: 20px;">                            
                        </td>
                        <td class="wide_table">
                            <table  class="wide_table"><tr>                                
                                <td class="wide_table" style="padding-right:10px;">
                                    <asp:PlaceHolder ID="PH_StatusButton" runat="server"/>
                                </td>
                                <td class="wide_table" style="width:100%"></td>
                            </tr></table>
                        </td>                        
                    </tr></table>
                </div>  
            </div>
        </div>
    </td>
    <td class="wide_table" style="width: 50%"></td>
    </tr></table>  
</asp:Content>

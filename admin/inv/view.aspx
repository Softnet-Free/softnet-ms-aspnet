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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="view.aspx.cs" Inherits="admin_inv_view" Title="Invitation - Admin" MasterPageFile="~/Site.master"  EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <table class="wide_table"><tr>
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
            <h1 style="text-align: center;">invitation</h1> 
        </td>
        <td class="wide_table" style="width:199px;"></td>
    </tr></table>  

    <table class="wide_table" style="margin-top: 25px;"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">            
            <div style="width: 700px; text-align: center">
                <asp:Label ID="L_Url" style="font-size: 1.1em; color: #E07020;"   runat="server"/>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>  

    <table class="wide_table" style="margin-top: 15px;"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">            
            <div style="width: 500px; padding: 10px; background-color: #A2C5D3">
                <table class="wide_table">
                <tr>                
                    <td class="wide_table">
                        <table class="wide_table"><tr>
                        <td class="wide_table" style="width:100%">
                            <span class="white_caption">e-mail</span>
                            <div style="background-color: #E7E7E7; padding: 3px;">
                                <asp:Label ID="L_EMail" style="font-size: 1.1em"   runat="server"/>
                            </div>
                        </td>
                        <td class="wide_table" id="TD_SendButton" runat="server" style="padding-left: 10px; vertical-align: bottom">
                            <asp:Panel ID="P_SendButton" CssClass="SubmitButtonEmbodied LightBlue" runat="server">
                                <asp:Button ID="B_Send" runat="server" Text="send" onclick="Send_Click" />
                            </asp:Panel>
                        </td>
                        </tr></table>
                    </td>                                      
                </tr>
                <tr>
                    <td class="wide_table" style="padding-top: 12px">
                        <span class="white_caption">description</span>
                        <div style="background-color: #E7E7E7; padding: 3px;">
                            <asp:Label ID="L_Description" style="font-size: 1.1em;"   runat="server"/>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="wide_table" style="padding-top: 12px">
                        <table class="auto_table"><tr>                        
                            <td class="auto_table" style="font-size: 1.1em;">
                                <span style="color: White">Assign 'Provider' role on registration:</span>
                            </td>
                            <td class="auto_table" style="padding-left: 10px;">
                                <asp:Label ID="L_AssignProviderRole" runat="server"></asp:Label>
                            </td>
                        </tr></table>
                    </td>
                </tr>
                </table>

                <table class="wide_table" style="margin-top: 20px">
                <tr>
                    <td class="wide_table" style="width:30%"></td>
                    <td class="wide_table">
                        <div class="SubmitButtonEmbodied LightBlue">
                            <asp:Button ID="B_Close" runat="server" Text="close" onclick="Close_Click" />
                        </div>
                    </td>
                    <td class="wide_table" style="width:40%"></td>
                    <td class="wide_table">
                        <div class="SubmitButtonEmbodied LightBlue">
                            <asp:Button ID="B_Edit" runat="server" Text="edit" onclick="Edit_Click" />
                        </div>
                    </td>
                    <td class="wide_table" style="width:30%"></td>
                </tr>
                </table>
            </div>

            <asp:Panel ID="P_Success" Visible="false" runat="server">
                <div style="margin-top: 20px; margin-bottom: 15px; text-align: center">
                    <span style=" font-size: 1.1em;">The invitation has been successfully sent!</span>
                </div>
            </asp:Panel>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>             
</asp:Content>
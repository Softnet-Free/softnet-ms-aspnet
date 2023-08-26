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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="admin_inv_edit" Title="Edit Invitation - Admin" MasterPageFile="~/Site.master"  EnableViewState="false" %>

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
                <asp:Label ID="L_Url" style="font-size: 1.1em; color: #E07020"   runat="server"/>
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
                        <span class="white_caption">e-mail</span>
                        <asp:TextBox ID="TB_EMail" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:494px; margin: 0px; padding: 3px;"></asp:TextBox>
                    </td>                                      
                </tr>
                <tr>
                    <td class="wide_table" style="padding-top: 12px">
                        <span class="white_caption">description</span>
                        <asp:TextBox ID="TB_Description" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:494px; margin: 0px; padding: 3px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="wide_table" style="padding-top: 12px">
                        <asp:CheckBox ID="CB_AssignProviderRole" style="color: White; font-size: 1.1em;" Text="Assign 'Provider' role on registration" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="wide_table" style="padding-top: 25px"> 
                        <table class="wide_table"><tr>
                        <td class="wide_table" style="width:30%"></td>
                        <td class="wide_table">
                            <div class="SubmitButtonEmbodied LightBlue">
                                <asp:Button ID="B_Cancel" runat="server" Text="cancel" onclick="Cancel_Click" />
                            </div>
                        </td>
                        <td class="wide_table" style="width:40%"></td>
                        <td class="wide_table">
                            <div class="SubmitButtonEmbodied LightBlue">
                                <asp:Button ID="B_Save" runat="server" Text="save" onclick="Save_Click" />
                            </div>
                        </td>
                        <td class="wide_table" style="width:30%"></td>
                        </tr></table>
                    </td>
                </tr>
                </table>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>
</asp:Content>

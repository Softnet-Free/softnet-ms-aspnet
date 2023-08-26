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

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ssite.aspx.cs" Inherits="ssite" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="text-align: left; padding-left: 10px; padding-bottom: 10px">
        <h3>
            <span>domain </span>
            <asp:HyperLink CssClass="h3_name domain_color" ID="HL_Domain" runat="server"/>
        </h3>
    </div>
    
    <table class="wide_table" style="width:798px"><tr>
        <td style="width:199px; vertical-align:top;">
            <table class="wide_table"><tr>
                <td style="padding-left:5px;">
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Back" runat="server" Text="back" onclick="Back_Click" />
                    </div>
                </td> 
                <td style="width:100%"></td>                
            </tr></table>        
        </td>
        <td style="width: 400px; vertical-align:top;">
            <div style="padding-top: 5px; padding-bottom: 5px; ">
                <h1 style="text-align: center;">site configuration</h1>
            </div>
        </td>
        <td style="width:199px; vertical-align:top;">
            <table class="wide_table"><tr>
                <td style="width:100%"></td>
                <td style="padding-right:10px;">
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Refresh" runat="server" Text="refresh" onclick="Refresh_Click" />
                    </div>
                </td>
            </tr></table>
        </td>
        <td></td>
    </tr></table>        
    
    <div style="padding-top: 15px; padding-bottom: 5px; padding-left: 30px; padding-right: 30px;">
        <h5>
            <asp:Label CssClass="h_name" ID="L_Description" runat="server"/>
        </h5>
    </div>  

    <div class="site_frame" style="padding-bottom: 0px;">
        <div class="site_menu">    
            <table class="wide_table"><tr>                
                <td class="wide_table" style="width: auto; text-align: left; padding-left: 10px;">
                    <asp:Label CssClass="site_type" ID="L_SiteType" EnableViewState="false" runat="server"/>
                </td>                 
                <td class="wide_table" style="text-align:right; padding-left: 4px; padding-right: 4px; white-space: nowrap">
                    <asp:Label CssClass="object_status" ID="L_SiteStatus" Text="site blank" runat="server"></asp:Label>
                </td>
                <td class="wide_table" style="width:23px; padding: 1px;">
                    <div class="SubmitButtonSquareMini RedOrange">
                        <asp:Button ID="B_DeleteSite" ToolTip="Delete Site" runat="server" Text="X" onclick="DeleteSite_Click" />
                    </div>
                </td>
            </tr></table>
        </div>      

        <div class="site_body"> 
            <div class="site_block">
                <div class="site_block_header">site</div>
                <div class="site_block_item">
                    <table class="auto_table"><tr>
                        <td class="auto_table">                            
                            <asp:Panel ID="P_EditSiteDescription" CssClass="SubmitButtonSquare Blue" runat="server">
                                <asp:Button ID="B_EditSiteDescription" runat="server" Text="description" onclick="EditSiteDescription_Click" />
                            </asp:Panel>
                        </td>
                        <td class="auto_table" style="padding-left:15px">
                            <asp:Panel ID="P_EditSiteEnabledStatus" CssClass="SubmitButtonSquare Blue" runat="server">
                                <asp:Button ID="B_EditSiteEnabledStatus" runat="server" Text="enable / disable" onclick="EditSiteEnabledStatus_Click" />
                            </asp:Panel>
                        </td>
                        <td class="auto_table" style="padding-left:15px">
                            <asp:Panel ID="P_EditSiteKind" CssClass="SubmitButtonSquare Blue" runat="server">
                                <asp:Button ID="B_EditSiteKind" runat="server" Text="multi-service hosting" onclick="EditSiteKind_Click" />
                            </asp:Panel>
                        </td>
                    </tr></table>
                    <asp:Panel ID="P_SiteEdit" Visible="false" style="padding-top: 15px;" runat="server">
                    </asp:Panel>
                </div>       
            </div>
                   
            <div class="site_block">
                <div class="site_block_header">service</div>
                <div class="site_block_item">
                    <table class="wide_table" style="margin-top: 0px"><tr>
                        <td class="wide_table" style="text-align:left">
                            <asp:Label CssClass="name" ID="L_Hostname" runat="server"/>&nbsp;&nbsp;&nbsp;
                            <asp:Label CssClass="service_type" Visible="false" ID="L_ServiceType" runat="server"/>
                            <asp:Label CssClass="service_type" Visible="false" ID="L_Version" runat="server"/>
                        </td>
                        <td class="wide_table" style="text-align:right; white-space: nowrap">
                            <asp:Label CssClass="object_status" Visible="false" ID="L_ServiceStatus" runat="server"/>
                        </td></tr>
                    </table>
                    <table class="auto_table" style="margin-top: 8px"><tr>
                        <td class="auto_table">
                            <asp:Panel ID="P_EditServiceAccount" CssClass="SubmitButtonSquare Blue" runat="server">
                                <asp:Button ID="B_EditServiceAccount" runat="server" Text="account" onclick="EditServiceAccount_Click" />
                            </asp:Panel>
                        </td>
                        <td class="auto_table" style="padding-left:15px">
                            <asp:Panel ID="P_EditServiceHostname" CssClass="SubmitButtonSquare Blue" runat="server">
                                <asp:Button ID="B_EditServiceHostname" runat="server" Text="hostname" onclick="EditServiceHostname_Click" />
                            </asp:Panel>
                        </td>
                        <td class="auto_table" style="padding-left:15px">
                            <asp:Panel ID="P_ShowApplyStructureButton" CssClass="SubmitButtonSquare Blue" runat="server">                                
                                <asp:Button ID="B_ShowApplyStructure" runat="server" Text="structure" onclick="ShowApplyStructure_Click" />
                            </asp:Panel>
                        </td>
                    </tr></table>
                    <asp:Panel ID="P_ServiceEdit" Visible="false" style="padding-top: 15px;" runat="server">
                    </asp:Panel>
                </div>                
            </div>  
        </div>
    </div>
</asp:Content>
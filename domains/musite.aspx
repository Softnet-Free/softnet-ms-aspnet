<%@ Page Language="C#" AutoEventWireup="true" CodeFile="musite.aspx.cs" Inherits="musite" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="padding-left: 10px; padding-bottom: 10px">
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
                <td class="wide_table" style="padding-left:10px;">
                    <div class="SubmitButton Blue">
                        <asp:Button ID="B_NewUser" runat="server" Text="new user" onclick="NewUser_Click" />
                    </div>
                </td>       
                <td style="width:100%"></td>                
            </tr></table>        
        </td>
        <td style="width: 400px;">
            <div style="padding-top: 5px; padding-bottom: 5px;">
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
    
    <div style="padding-top: 15px; padding-bottom: 7px; padding-left: 30px; padding-right: 30px;">
        <h5>
            <asp:Label CssClass="h_name" ID="L_Description" runat="server"/>
        </h5>
    </div>       

    <div class="site_frame" style="padding-bottom: 0px;">
        <div class="site_menu">
            <table class="wide_table"><tr>                
                <td class="wide_table" style="width: 60px; padding: 1px; padding-left: 3px">
                    <div class="SubmitButtonMini Blue">
                        <asp:Button ID="B_Clients" runat="server" Text="clients" onclick="Clients_Click" />
                    </div>
                </td>
                <td class="wide_table" style="width: auto; text-align: left; padding-left: 20px;">
                    <asp:Label CssClass="site_type" ID="L_SiteType" EnableViewState="false" runat="server"/>
                </td>
                <td class="wide_table" style="text-align:right; padding-left: 4px; padding-right: 10px; white-space: nowrap">
                    <asp:Label CssClass="object_status" Visible="false" EnableViewState="false" ID="L_SiteStatus" runat="server"/>
                </td>
                <td class="wide_table" style="width:23px; padding: 1px;">
                    <div class="SubmitButtonSquareMini RedOrange">
                        <asp:Button ID="B_DeleteSite" runat="server" Text="X" ToolTip="Delete Site" onclick="DeleteSite_Click" />
                    </div>
                </td></tr>
            </table>
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
                    </tr></table>
                    <asp:Panel ID="P_SiteEdit" Visible="false" EnableViewState="false" style="padding-top: 15px;" runat="server">
                    </asp:Panel>
                </div>       
            </div>

            <div class="site_block">
                <div class="site_block_header">list of services</div>  
                <div class="site_block_item">          
                    <table class="auto_table"><tr>
                        <td class="auto_table">
                            <asp:Panel ID="P_NewServiceButton" class="SubmitButtonSquare Blue" runat="server">
                                <asp:Button ID="B_NewService" runat="server" Text="new service" onclick="NewService_Click" />
                            </asp:Panel>
                        </td>
                    </tr></table>
                    <asp:Panel ID="P_NewService" Visible="false" CssClass="site_block_item" runat="server">
                        <table class="auto_table">
                            <tr>
                                <td class="auto_table" style="padding-left: 2px">
                                    <span class="legend">hostname</span>
                                </td>
                                <td class="auto_table"></td>
                            </tr>
                            <tr>
                                <td class="auto_table">
                                    <asp:TextBox ID="TB_NewServiceHostname" autocomplete = "off" Style="border: 1px solid #7FBA00; outline:none; width:300px; margin: 0px; padding: 3px;" runat="server"/>
                                </td>                            
                                <td class="auto_table" style="padding-left: 15px">
                                    <div id="Div1" class="SubmitButtonMini Green" runat="server">
                                        <asp:Button ID="B_AddService" runat="server" Text="add service" onclick="AddService_Click" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <asp:Label ID="L_HostnameError" Visible="false" runat="server" Text="" Style="display:block; margin-top:10px; color:#9F0000"></asp:Label>
                    </asp:Panel>
                </div>  
                <asp:PlaceHolder ID="PH_Services" runat="server"/>
            </div>

            <div class="site_block">
                <div class="site_block_header">
                    access defaults
                </div>                   
                <div class="site_block_item">
                    <table class="auto_table"><tr>
                        <td class="auto_table">
                            <span class="name">Implicit users</span>
                        </td>
                        <td class="auto_table" style="padding-left: 5px">
                            <asp:Label ID="L_ImplicitUserStatus" runat="server"/> 
                        </td>
                        <td class="auto_table" style="padding-left: 15px">
                            <asp:Panel ID="P_ChangeImplicitUserStatusButton" runat="server"/>
                        </td>
                    </tr></table>
                </div>                                    
            </div>

            <asp:Panel CssClass="site_block" ID="P_GuestStatus" Visible="false" runat="server">
                <div class="site_block_header">
                    guest status
                </div>  
                <div class="site_block_item">
                    <table class="auto_table"><tr>
                        <td class="auto_table">
                            <span class="name">Guest</span>
                        </td>
                        <td class="auto_table" style="padding-left: 5px">
                            <asp:Label ID="L_GuestStatus" runat="server"/> 
                        </td>
                        <td class="auto_table" style="padding-left: 15px">
                            <asp:Panel ID="P_ChangeGuestStatusButton" runat="server"/>
                        </td>
                    </tr></table>
                </div>
            </asp:Panel>        

            <asp:Panel CssClass="site_block" Visible="false" ID="P_GuestPage" runat="server">
                <div class="site_block_header">
                    guest page
                </div>
                <div class="site_block_item">
                    <asp:Label CssClass="guest_page_url" ID="L_GuestPage" runat="server"/>
                </div>
            </asp:Panel>

            <asp:Panel CssClass="site_block" Visible="false" ID="P_SharedClient" runat="server">
                <div class="site_block_header">
                    guest shared uri
                </div>
                <div class="site_block_item">
                    <asp:Label CssClass="guest_shared_uri" ID="L_SharedGuestURI" runat="server"/>                  
                </div>
            </asp:Panel>

            <asp:Panel ID="P_ExplicitUsers" CssClass="site_block" runat="server">
                <div class="site_block_header">
                    explicit users
                </div>
                <div class="site_block_body">
                    <asp:PlaceHolder ID="PH_ExplicitUsers" runat="server"/>
                </div>
            </asp:Panel>

            <asp:Panel ID="P_ImplicitUsers" CssClass="site_block" runat="server">
                <div class="site_block_header">
                    implicit users
                </div>
                <div class="site_block_body">
                    <asp:PlaceHolder ID="PH_ImplicitUsers" runat="server"/>
                </div>
            </asp:Panel>
        </div>
    </div> 

    <asp:Panel ID="P_DeniedUsers" style="margin-top: 20px;" runat="server">
        <div style="text-align: left; padding-left: 30px; padding-bottom: 10px;">
            <h1 style="color: #BF0000">denied users</h1>
        </div>
        <div class="site_body">
            <div class="site_block">
                <asp:PlaceHolder ID="PH_DeniedUsers" runat="server"/>
            </div>
        </div>
    </asp:Panel>
</asp:Content>

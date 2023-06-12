<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mrclients.aspx.cs" Inherits="mrclients" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" EnableViewState="false" %>

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
                <td style="width:100%"></td>                
            </tr></table>        
        </td>
        <td style="width: 400px;">
            <div style="padding-top: 5px; padding-bottom: 5px;">
                <h1 style="text-align: center;">client manager</h1>
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

    <div class="site_frame" style="margin-bottom: 20px;">
        <div class="site_menu">    
            <table class="wide_table"><tr>                
                <td class="wide_table" style="width: 60px; padding: 1px; padding-left: 4px">
                    <div class="SubmitButtonMini Blue">
                        <asp:Button ID="B_Config" runat="server" Text="config" onclick="Config_Click" />
                    </div>
                </td>
                <td class="wide_table" style="width: auto; text-align: left; padding-left: 20px;">
                    <asp:Label CssClass="site_type" ID="L_SiteType" runat="server"/>
                </td>
                <td class="wide_table" style="text-align:right; padding-left: 4px; padding-right: 4px; white-space: nowrap">
                    <asp:Label CssClass="object_status" Visible="false" ID="L_SiteStatus" runat="server"/>
                </td>
                <td class="wide_table" style="width:23px; padding: 1px;">
                    <div class="SubmitButtonSquareMini RedOrange">
                        <asp:Button ID="B_DeleteSite" runat="server" Text="X" ToolTip = "Delete Site" onclick="DeleteSite_Click" />
                    </div>
                </td></tr>
            </table>
        </div>        
        <div class="site_body">
            <div class="site_block">
                <div class="site_block_header">list of services</div>            
                <asp:PlaceHolder ID="PH_Services" runat="server"/>            
            </div>

            <div class="site_block">
                <div class="site_block_header">
                    supported roles
                </div>
                <div class="site_block_item">
                    <asp:Label ID="L_SupportedRoles" runat="server"/>
                    <asp:Panel Style="margin-top: 8px;" Visible="false" ID="P_DefaultRole" runat="server">
                        <table class="auto_table"><tr>
                            <td class="auto_table">
                                <span style="color: gray">user default role:</span>
                            </td>
                            <td class="auto_table" style="padding-left: 5px">
                                <asp:Label CssClass="user_default_role" ID="L_DefaultRole" runat="server"/>
                            </td>                        
                        </tr></table>
                    </asp:Panel>
                </div>
            </div>            
           
            <asp:Panel CssClass="site_block" Visible="false" ID="P_GuestStatus" runat="server">
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
                    shared guest uri
                </div>
                <div class="site_block_item">
                    <asp:Label CssClass="guest_shared_uri" ID="L_SharedGuestURI" runat="server"/>                 
                </div>
            </asp:Panel>

            <asp:Panel ID="P_Clients" CssClass="site_block" runat="server">
                <div class="site_block_header">
                    clients
                </div>                
                <asp:PlaceHolder ID="PH_Clients" runat="server"/>
            </asp:Panel>
        </div>
    </div> 
</asp:Content>

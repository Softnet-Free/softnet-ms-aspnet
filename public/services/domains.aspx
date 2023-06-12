<%@ Page Language="C#" AutoEventWireup="true" CodeFile="domains.aspx.cs" Inherits="public_services_domains" MasterPageFile="~/Site.master" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">    
    <div style="padding-left: 10px; padding-bottom: 10px;">
        <h3>public services</h3>    
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
        <td class="wide_table">
            <h2 id="H_Owner" runat="server" style="text-align: center;">owner <asp:Label CssClass="h2_name provider_color" ID="L_OwnerName" runat="server"></asp:Label></h2>
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

     <asp:Panel ID="P_Body" runat="server">
        <h2 style="color: #4F8DA6; margin-left: 27px; margin-bottom: 2px; margin-top: 15px">domains</h2>

        <div style="height: 8px; background-color: #A2C5D3;"></div> 
        <div style="border: 1px solid #A2C5D3; padding: 10px">
            <asp:PlaceHolder ID="PH_Domains" runat="server"></asp:PlaceHolder>
        </div>
        <span style="display:block; text-align: left; padding-top: 4px; padding-left: 10px; color: gray; font-size: 0.9em;">The list of domains containing services with guest access enabled</span>
    </asp:Panel>
</asp:Content>
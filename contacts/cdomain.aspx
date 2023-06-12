<%@ Page Language="C#" AutoEventWireup="true" CodeFile="cdomain.aspx.cs" Inherits="contacts_cdomain" MasterPageFile="~/Site.master" EnableViewState="false" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="padding-left: 10px; padding-bottom: 10px">
        <h3>
            <span>contact</span>
            <asp:HyperLink CssClass="h3_name provider_color" ID="HL_Contact" runat="server"/>            
        </h3>
    </div>

    <table class="wide_table" style="width:798px"><tr>
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
            <h2 style="text-align: center">the contact domain <asp:Label CssClass='h2_name domain_color' ID="L_Domain" runat="server"/></h2> 
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
    <br /><br />       
    <asp:PlaceHolder ID="PH_Sites" runat="server"/>
</asp:Content>
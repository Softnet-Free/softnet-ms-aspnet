<%@ Page Language="C#" AutoEventWireup="true" CodeFile="domains.aspx.cs" Inherits="public_clients_domains" Title="Guest Clients by Email"  MasterPageFile="~/Site.master" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Panel ID="P_Blank" Visible="false" runat="server" Style="padding-left: 10px; padding-right: 10px;">
        <div style="padding-left: 10px; padding-bottom: 10px;">
            <h1 style="text-align:center;">guest clients by email</h1>    
        </div>

        <table class="wide_table"><tr>
            <td class="wide_table" style="width: 50%"></td>
            <td class="wide_table">    
                <div style="width: 500px;">
                    <span style="font-size: 1.2em; color: #3C6C80">
                        In order to browse your guest clients you need a browsing url. 
                        If you haven't got it yet type your email address in the text box and click <span style="font-weight:bold;">send confirmation mail</span>,
                        then follow the url from the inbox of your email.
                    </span>

                    <div style="width: 480px; height: 90px; background-color: #A2C5D3; padding: 10px; padding-top: 5px;  margin-top: 25px; margin-bottom: 25px">
                        <span class="white_caption">email</span>
                        <asp:TextBox ID="TB_EMail" runat="server" Style="font-size:1.2em; border-width: 0px; outline:none; width:474px; margin: 0px; padding: 3px;"></asp:TextBox>
                
                        <table class="wide_table" style="margin-top: 15px;"><tr>
                            <td class="wide_table" style="width: 50%"></td>
                            <td class="wide_table">  
                                <asp:Panel ID="P_SendMailButton" class="SubmitButtonEmbodied LightBlue" runat="server">
                                    <asp:Button ID="B_Send" runat="server" Text="send confirmation mail" onclick="SendBrowsingUrl_Click" />                                    
                                </asp:Panel>
                            </td>
                            <td class="wide_table" style="width: 50%"></td>
                        </tr></table>
                    </div>

                    <asp:PlaceHolder ID="PH_Messages" runat="server"></asp:PlaceHolder>
                </div>
            </td>
            <td class="wide_table" style="width: 50%"></td>
        </tr></table>
    </asp:Panel>

    <asp:Panel ID="P_PublicDomains" Visible="false" runat="server">      
        <div style="padding-left: 10px; margin-bottom: 5px">
            <h3>guest clients by <asp:Label ID="L_EMail" CssClass="h3_name name" runat="server"/></h3>
        </div>  

        <table class="wide_table" style="margin-bottom: 15px"><tr>
            <td class="wide_table" style="width: 150px;"/>
            <td class="wide_table">
                 <h1 style="text-align:center;">owners and domains</h1>                 
            </td>                            
            <td class="wide_table" style="width: 150px; padding-right: 10px;">
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

        <div style="height: 8px; background-color: #A2C5D3;"></div>
        <div style="border: 1px solid #A2C5D3; padding: 10px">
            <asp:PlaceHolder ID="PH_Domains" runat="server"></asp:PlaceHolder>
        </div>
        <span style="display:block; text-align: left; padding-top: 4px; padding-left: 10px; color: gray; font-size: 0.9em;">
            The list of service owners and their domains containing guest clients by a given email
        </span>
    </asp:Panel>

</asp:Content>
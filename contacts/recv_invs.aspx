<%@ Page Language="C#" AutoEventWireup="true" CodeFile="recv_invs.aspx.cs" Inherits="contacts_recv_invs" MasterPageFile="~/Site.master" Title="Received invitations" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1 style="text-align: center;">received invitations</h1>

    <table class="wide_table"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">
            <div style="width: 600px; padding-top: 20px">
                <div style="height: 8px; background-color: #A2C5D3;"></div>
                <asp:Table ID="Tb_Invitations" runat="server" CssClass="wide_table" style="border: 1px solid #A2C5D3;">
                </asp:Table>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>
</asp:Content>

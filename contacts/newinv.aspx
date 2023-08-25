<%@ Page Language="C#" AutoEventWireup="true" CodeFile="newinv.aspx.cs" Inherits="contacts_newinv" MasterPageFile="~/Site.master" Title="New Invitation"  MaintainScrollPositionOnPostback="true" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <table class="wide_table"><tr>
        <td class="wide_table" style="width:200px; vertical-align:top;">
            <table class="auto_table"><tr>
                <td class="auto_table" style="padding-left:5px;">
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Back" runat="server" Text="back" onclick="Back_Click" />
                    </div>
                </td>                                                 
            </tr></table>        
        </td>
        <td style="vertical-align: top;">
            <h1 style="text-align: center;">new invitation</h1>
        </td>
        <td class="wide_table" style="width:200px;"></td>
    </tr></table>

    <table class="wide_table"><tr>
    <td class="wide_table" style="width: 50%"></td>
    <td class="wide_table">
        <div style="width: 700px; padding-top: 4px">
            <div style="padding-left: 10px; padding-top: 16px; padding-bottom:4px;">
                <h4 style="color: #4F8DA6; padding-left:5px">search for a person or organization</h4>
            </div>
            <div style="background-color: #A2C5D3">                    
                <table class="wide_table"><tr>
                    <td class="wide_table" style="width: 470px; padding: 3px 5px 3px 5px;">
                        <asp:TextBox ID="TB_SearchFilter" AutoComplete="off" runat="server" Style="font-size:1.1em; border-width: 0px; outline:none; width:464px; margin: 0px; padding: 3px;"></asp:TextBox>
                    </td>
                    <td class="wide_table" style="text-align:right; padding: 3px 5px;">
                        <table class="auto_table"><tr>
                            <td class="auto_table">
                                <div class="SubmitButtonEmbodied LightBlue" style="width: 60px">
                                    <asp:Button ID="B_Search" runat="server" Text="Go" onclick="Search_Click" />
                                </div>
                            </td>
                            <td class="auto_table" style="padding-left: 8px; width: 64px">
                                <asp:Panel Visible="false" CssClass="SubmitButtonEmbodied LightBlue" style="width: 60px" ID="P_PrevButton" runat="server">
                                    <asp:Button ID="B_Prev" runat="server" Text="<<" onclick="Prev_Click" />
                                </asp:Panel>
                            </td>
                            <td class="auto_table" style="padding-left: 8px; width: 64px">
                                <asp:Panel Visible="false" CssClass="SubmitButtonEmbodied LightBlue" style="width: 60px" ID="P_NextButton" runat="server">
                                    <asp:Button ID="B_Next" runat="server" Text=">>" onclick="Next_Click" />
                                </asp:Panel>
                            </td>
                        </tr></table>
                    </td>
                </tr></table>
            </div>

            <div style="margin-top: 5px;">
                <asp:PlaceHolder ID="PH_FoundUsers" runat="server"></asp:PlaceHolder>
            </div>

            <div style="margin-top: 20px; margin-bottom: 10px; text-align: center;">
                <asp:Label CssClass="warning" ID="L_Message" runat="server"></asp:Label>
            </div>
        </div>
    </td>
    <td class="wide_table" style="width: 50%"></td>
    </tr></table>
</asp:Content>

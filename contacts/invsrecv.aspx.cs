/*
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
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class contacts_invs_recv : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            List<OwnerData> invitingUsers = new List<OwnerData>();
            SoftnetRegistry.GetReceivedInvitations(this.Context.User.Identity.Name, invitingUsers);

            if (invitingUsers.Count > 0)
            {
                invitingUsers.Sort(delegate(OwnerData x, OwnerData y)
                {
                    return x.fullName.CompareTo(y.fullName);
                });

                foreach (OwnerData invitingUser in invitingUsers)
                {
                    TableRow tableRow = new TableRow();
                    Tb_Invitations.Rows.Add(tableRow);

                    TableCell td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 7px; padding-right: 10px;";

                    HtmlGenericControl divAcceptButton = new HtmlGenericControl("div");
                    td.Controls.Add(divAcceptButton);
                    divAcceptButton.Attributes["class"] = "SubmitButtonMini Blue";

                    TButton buttonAccept = new TButton();
                    divAcceptButton.Controls.Add(buttonAccept);
                    buttonAccept.Args.Add(invitingUser);
                    buttonAccept.Text = "accept";
                    buttonAccept.ID = string.Format("B_Accept_{0}", invitingUser.ownerId);
                    buttonAccept.Click += new EventHandler(AcceptInvitation_Click);

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 7px; text-align: left; width: 100%";

                    Label labelInvitingUser = new Label();
                    td.Controls.Add(labelInvitingUser);
                    labelInvitingUser.Text = invitingUser.fullName;
                    labelInvitingUser.Attributes["style"] = "font-size: 1.1em;";

                    td = new TableCell();
                    tableRow.Cells.Add(td);
                    td.CssClass = "wide_table";
                    td.Attributes["style"] = "border-top: 1px solid #A2C5D3; border-bottom: 1px solid #A2C5D3; padding: 7px;";

                    HtmlGenericControl divDeleteButton = new HtmlGenericControl("div");
                    td.Controls.Add(divDeleteButton);
                    divDeleteButton.Attributes["class"] = "SubmitButtonSquareMini RedOrange";
                    
                    TButton buttonDelete = new TButton();
                    divDeleteButton.Controls.Add(buttonDelete);
                    buttonDelete.Text = "X";
                    buttonDelete.ID = string.Format("B_Delete_{0}", invitingUser.ownerId);
                    buttonDelete.Args.Add(invitingUser);
                    buttonDelete.Click += new EventHandler(DeleteInvitation_Click);
                }
            }
            else
            {
                Response.Redirect("~/contacts/default.aspx");
            }
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void AcceptInvitation_Click(object sender, EventArgs e)
    {        
        TButton tButton = (TButton)sender;
        OwnerData invitingUser = (OwnerData)tButton.Args[0];
        try
        {
            SoftnetRegistry.AcceptInvitation(this.Context.User.Identity.Name, invitingUser.ownerId);
            Response.Redirect("~/contacts/invsrecv.aspx");
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }

    protected void DeleteInvitation_Click(object sender, EventArgs e)
    {        
        TButton tButton = (TButton)sender;
        OwnerData invitingUser = (OwnerData)tButton.Args[0];
        try
        {
            SoftnetRegistry.DeleteReceivedInvitation(this.Context.User.Identity.Name, invitingUser.ownerId);
            Response.Redirect("~/contacts/invsrecv.aspx");
        }
        catch (SoftnetException ex)
        {
            ExceptionHandler.exec(this, ex);
        }
    }
}
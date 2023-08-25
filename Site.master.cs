/*
*	Copyright 2023 Robert Koifman
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
using System.Web.Security;

public partial class SiteMaster : System.Web.UI.MasterPage
{
    public bool m_anyoneCanRegister = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Context.User.Identity.IsAuthenticated)
        {
            MembershipUser mUser = System.Web.Security.Membership.GetUser(true);
            if (mUser != null && mUser.IsApproved == false)
            {
                this.Session.Abandon();
                FormsAuthentication.SignOut();
                this.Response.Redirect("~/default.aspx");
            }

            MenuItem menuItem_Home = new MenuItem();
            MenuItem menuItem_Domains = new MenuItem();
            MenuItem menuItem_Contacts = new MenuItem();
            MenuItem menuItem_Admin = new MenuItem();
            MenuItem menuItem_Public = new MenuItem();

            menuItem_Home.NavigateUrl = "~/default.aspx";
            menuItem_Home.Text = "Home";
            NavigationMenu.Items.Add(menuItem_Home);

            if (this.Context.User.IsInRole("Administrator"))
            {
                menuItem_Domains.NavigateUrl = "~/domains/default.aspx";
                menuItem_Domains.Text = "Domains";
                NavigationMenu.Items.Add(menuItem_Domains);

                menuItem_Contacts.NavigateUrl = "~/contacts/default.aspx";
                menuItem_Contacts.Text = "Contacts";
                NavigationMenu.Items.Add(menuItem_Contacts);

                menuItem_Admin.NavigateUrl = "~/admin/mgt/search.aspx";
                menuItem_Admin.Text = "Admin";
                NavigationMenu.Items.Add(menuItem_Admin);
            }
            else if (this.Context.User.IsInRole("Provider"))
            {
                menuItem_Domains.NavigateUrl = "~/domains/default.aspx";
                menuItem_Domains.Text = "Domains";
                NavigationMenu.Items.Add(menuItem_Domains);

                menuItem_Contacts.NavigateUrl = "~/contacts/default.aspx";
                menuItem_Contacts.Text = "Contacts";
                NavigationMenu.Items.Add(menuItem_Contacts);
            }
            else
            {
                menuItem_Contacts.NavigateUrl = "~/contacts/default.aspx";
                menuItem_Contacts.Text = "Contacts";
                NavigationMenu.Items.Add(menuItem_Contacts);
            }

            menuItem_Public.NavigateUrl = "~/public/services/search.aspx";
            menuItem_Public.Text = "Public";
            NavigationMenu.Items.Add(menuItem_Public);

            string path = Request.AppRelativeCurrentExecutionFilePath;

            if (path.StartsWith("~/default.aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                menuItem_Home.Selected = true;
            }
            else if (path.StartsWith("~/domains/", StringComparison.InvariantCultureIgnoreCase))
            {
                menuItem_Domains.Selected = true;
            }
            else if (path.StartsWith("~/contacts/", StringComparison.InvariantCultureIgnoreCase))
            {
                menuItem_Contacts.Selected = true;
            }
            else if (path.StartsWith("~/admin/", StringComparison.InvariantCultureIgnoreCase))
            {
                menuItem_Admin.Selected = true;

                Menu adminMenu = new Menu();
                P_AdminMenu.Controls.Add(adminMenu);
                P_AdminMenu.Visible = true;
                adminMenu.CssClass = "menu_2";
                adminMenu.IncludeStyleBlock = false;
                adminMenu.Orientation = Orientation.Horizontal;

                MenuItem menuItem1 = new MenuItem();
                menuItem1.NavigateUrl = "~/admin/mgt/search.aspx";
                menuItem1.Text = "User Management";
                adminMenu.Items.Add(menuItem1);

                MenuItem menuItem2 = new MenuItem();
                menuItem2.NavigateUrl = "~/admin/inv/default.aspx";
                menuItem2.Text = "Invitations";
                adminMenu.Items.Add(menuItem2);

                MenuItem menuItem3 = new MenuItem();
                menuItem3.NavigateUrl = "~/admin/settings.aspx";
                menuItem3.Text = "Settings";
                adminMenu.Items.Add(menuItem3);

                if (path.StartsWith("~/admin/mgt/", StringComparison.InvariantCultureIgnoreCase))
                {
                    menuItem1.Selected = true;
                }
                else if (path.StartsWith("~/admin/Inv/", StringComparison.InvariantCultureIgnoreCase))
                {
                    menuItem2.Selected = true;
                }
                else if (path.StartsWith("~/admin/settings.aspx", StringComparison.InvariantCultureIgnoreCase))
                {
                    menuItem3.Selected = true;
                }
            }
            else if (path.StartsWith("~/public/", StringComparison.InvariantCultureIgnoreCase))
            {
                menuItem_Public.Selected = true;

                Menu searchMenu = new Menu();
                P_SearchMenu.Controls.Add(searchMenu);
                P_SearchMenu.Visible = true;
                searchMenu.CssClass = "menu_2";
                searchMenu.IncludeStyleBlock = false;
                searchMenu.Orientation = Orientation.Horizontal;

                MenuItem menuItem1 = new MenuItem();
                menuItem1.NavigateUrl = "~/public/services/search.aspx";
                menuItem1.Text = "Public Services";
                searchMenu.Items.Add(menuItem1);

                MenuItem menuItem2 = new MenuItem();
                menuItem2.NavigateUrl = "~/public/myclients/domains.aspx";
                menuItem2.Text = "My Guest Clients";
                searchMenu.Items.Add(menuItem2);

                MenuItem menuItem3 = new MenuItem();
                menuItem3.NavigateUrl = "~/public/clients/domains.aspx";
                menuItem3.Text = "Guest Clients by EMail";
                searchMenu.Items.Add(menuItem3);

                if (path.StartsWith("~/public/services/", StringComparison.InvariantCultureIgnoreCase))
                {
                    menuItem1.Selected = true;
                }
                else if (path.StartsWith("~/public/myclients/", StringComparison.InvariantCultureIgnoreCase))
                {
                    menuItem2.Selected = true;
                }
                else if (path.StartsWith("~/public/clients/", StringComparison.InvariantCultureIgnoreCase))
                {
                    menuItem3.Selected = true;
                }
            }

            try
            {
                int invitationCount = SoftnetRegistry.GetReceivedInvitationCount(this.Context.User.Identity.Name);
                if (invitationCount > 0)
                {
                    HL_InvitationsReceived.Visible = true;
                    L_InvitationsReceived.Text = string.Format("Invitations: {0}", invitationCount);
                }
            }
            catch (SoftnetException) { }
        }
        else
        {
            MenuItem menuItem_Home = new MenuItem();
            MenuItem menuItem_PublicServices = new MenuItem();
            MenuItem menuItem_GuestClients = new MenuItem();

            menuItem_Home.NavigateUrl = "~/default.aspx";
            menuItem_Home.Text = "Home";
            NavigationMenu.Items.Add(menuItem_Home);

            menuItem_PublicServices.NavigateUrl = "~/public/services/search.aspx";
            menuItem_PublicServices.Text = "Public Services";
            NavigationMenu.Items.Add(menuItem_PublicServices);

            menuItem_GuestClients = new MenuItem();
            menuItem_GuestClients.NavigateUrl = "~/public/clients/domains.aspx";
            menuItem_GuestClients.Text = "Guest Clients by EMail";
            NavigationMenu.Items.Add(menuItem_GuestClients);

            string path = Request.AppRelativeCurrentExecutionFilePath;

            if (path.StartsWith("~/default.aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                menuItem_Home.Selected = true;
            }
            else if (path.StartsWith("~/public/services/", StringComparison.InvariantCultureIgnoreCase))
            {
                menuItem_PublicServices.Selected = true;
            }
            else if (path.StartsWith("~/public/clients/", StringComparison.InvariantCultureIgnoreCase))
            {
                menuItem_GuestClients.Selected = true;
            }

            if (m_anyoneCanRegister)
                P_NewUser.Visible = true;
        }
    }
}

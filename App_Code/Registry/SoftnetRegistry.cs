using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

/// <summary>
/// Summary description for SoftnetRegistry
/// </summary>
public class SoftnetRegistry
{
    public static void usermgt_getUserData(MemberData memberData)
    { 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtUsers_GetUserData";

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Input;
                command.Parameters["@OwnerId"].Value = memberData.ownerId;

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@FullName", SqlDbType.NVarChar, 256);
                command.Parameters["@FullName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@Authority", SqlDbType.Int);
                command.Parameters["@Authority"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@IsEnabled", SqlDbType.Bit);
                command.Parameters["@IsEnabled"].Direction = ParameterDirection.Output;
                
                command.Parameters.Add("@IsLocked", SqlDbType.Bit);
                command.Parameters["@IsLocked"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@AuthMismatch", SqlDbType.Bit);
                command.Parameters["@AuthMismatch"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@StatusMismatch", SqlDbType.Bit);
                command.Parameters["@StatusMismatch"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The user has not been found.");
                    if (resultCode == -4)
                        throw new MembershipSoftnetException();
                    throw new DataDefinitionSoftnetException();
                }

                memberData.accountName = (string)command.Parameters["@AccountName"].Value;
                memberData.fullName = (string)command.Parameters["@FullName"].Value;
                if (command.Parameters["@EMail"].Value != DBNull.Value)
                    memberData.email = (string)command.Parameters["@EMail"].Value;
                memberData.authority = (int)command.Parameters["@Authority"].Value;
                memberData.isEnabled = (bool)command.Parameters["@IsEnabled"].Value;
                memberData.isLocked = (bool)command.Parameters["@IsLocked"].Value;
                memberData.authMismatch = (bool)command.Parameters["@AuthMismatch"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void usermgt_findUsers(string filter, List<OwnerData> users)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtUsers_FindUsers";

                command.Parameters.Add("@Filter", SqlDbType.NVarChar, 256);
                command.Parameters["@Filter"].Direction = ParameterDirection.Input;
                command.Parameters["@Filter"].Value = filter;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            OwnerData foundUser = new OwnerData();
                            foundUser.ownerId = (long)dataReader[0];
                            foundUser.authority = (int)dataReader[1];
                            foundUser.enabled = (bool)dataReader[2];
                            foundUser.fullName = (string)dataReader[3];
                            users.Add(foundUser);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void usermgt_findNextUsers(string filter, long lastSelectedId, List<OwnerData> users)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtUsers_FindNextUsers";

                command.Parameters.Add("@Filter", SqlDbType.NVarChar, 256);
                command.Parameters["@Filter"].Direction = ParameterDirection.Input;
                command.Parameters["@Filter"].Value = filter;

                command.Parameters.Add("@LastSelectedId", SqlDbType.BigInt);
                command.Parameters["@LastSelectedId"].Direction = ParameterDirection.Input;
                command.Parameters["@LastSelectedId"].Value = lastSelectedId;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            OwnerData foundUser = new OwnerData();
                            foundUser.ownerId = (long)dataReader[0];
                            foundUser.authority = (int)dataReader[1];
                            foundUser.enabled = (bool)dataReader[2];
                            foundUser.fullName = (string)dataReader[3];
                            users.Add(foundUser);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void usermgt_findPrevUsers(string filter, long firstSelectedId, List<OwnerData> users)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtUsers_FindPrevUsers";

                command.Parameters.Add("@Filter", SqlDbType.NVarChar, 256);
                command.Parameters["@Filter"].Direction = ParameterDirection.Input;
                command.Parameters["@Filter"].Value = filter;

                command.Parameters.Add("@FirstSelectedId", SqlDbType.BigInt);
                command.Parameters["@FirstSelectedId"].Direction = ParameterDirection.Input;
                command.Parameters["@FirstSelectedId"].Value = firstSelectedId;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            OwnerData foundUser = new OwnerData();
                            foundUser.ownerId = (long)dataReader[0];
                            foundUser.authority = (int)dataReader[1];
                            foundUser.enabled = (bool)dataReader[2];
                            foundUser.fullName = (string)dataReader[3];
                            users.Add(foundUser);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void public_getDomainDatasetForAccount(string accountName, DomainDatasetForAccount dataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_GetDomainDatasetForAccount";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainId"].Value = dataset.domainId;

                command.Parameters.Add("@CreatorId", SqlDbType.BigInt);
                command.Parameters["@CreatorId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@OwnerName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        List<SiteData> sites = new List<SiteData>();
                        dataset.sites = sites;
                        while (dataReader.Read())
                        {
                            SiteData siteData = new SiteData();
                            siteData.siteId = (long)dataReader[0];
                            siteData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                siteData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                siteData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                siteData.ssHash = (string)dataReader[4];
                            siteData.guestSupported = (bool)dataReader[5];
                            siteData.guestAllowed = (bool)dataReader[6];
                            siteData.statelessGuestSupported = (bool)dataReader[7];
                            if (dataReader[8] != DBNull.Value)
                                siteData.siteKey = (string)dataReader[8];
                            siteData.enabled = (bool)dataReader[9];
                            if (dataReader[10] != DBNull.Value)
                                siteData.description = (string)dataReader[10];
                            sites.Add(siteData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        dataset.services = services;
                        while (dataReader.Read())
                        {
                            ServiceData serviceData = new ServiceData();
                            serviceData.serviceId = (long)dataReader[0];
                            serviceData.siteId = (long)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                serviceData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                serviceData.contractAuthor = (string)dataReader[3];
                            serviceData.version = (string)dataReader[4];
                            serviceData.hostname = (string)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                serviceData.ssHash = (string)dataReader[6];
                            serviceData.enabled = (bool)dataReader[7];
                            serviceData.pingPeriod = (int)dataReader[8];
                            services.Add(serviceData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        UserData guestData = new UserData();
                        dataset.guestData = guestData;
                        if (dataReader.Read())
                        {
                            guestData.userId = (long)dataReader[0];
                            guestData.name = (string)dataReader[1];
                            guestData.enabled = (bool)dataReader[2];
                        }
                        else
                            throw new DataDefinitionSoftnetException();

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ClientData> guestClients = new List<ClientData>();
                        dataset.guestClients = guestClients;
                        while (dataReader.Read())
                        {
                            ClientData clientData = new ClientData();
                            clientData.clientId = (long)dataReader[0];
                            clientData.siteId = (long)dataReader[1];
                            clientData.clientKey = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                clientData.serviceType = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                clientData.contractAuthor = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                clientData.clientDescription = (string)dataReader[5];
                            clientData.pingPeriod = (int)dataReader[6];
                            guestClients.Add(clientData);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode == 0)
                {
                    dataset.creatorId = (long)command.Parameters["@CreatorId"].Value;
                    dataset.ownerId = (long)command.Parameters["@OwnerId"].Value;
                    dataset.ownerName = (string)command.Parameters["@OwnerName"].Value;
                    dataset.domainName = (string)command.Parameters["@DomainName"].Value;
                }
                else if (resultCode == 1)
                {
                    throw new InvalidStateSoftnetException();
                }
                else
                {
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    throw new DataDefinitionSoftnetException();
                }                
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }
    
    public static void public_getDomainsForAccount(string accountName, List<OwnerData> owners, List<DomainData> domains)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_GetDomainsForAccount";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            DomainData domainData = new DomainData();
                            domainData.domainId = (long)dataReader[0];
                            domainData.ownerId = (long)dataReader[1];
                            domainData.domainName = (string)dataReader[2];
                            domains.Add(domainData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        while (dataReader.Read())
                        {
                            OwnerData ownerData = new OwnerData();
                            ownerData.ownerId = (long)dataReader[0];
                            ownerData.fullName = (string)dataReader[1];
                            owners.Add(ownerData);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void public_getSiteDataset(string accountName, string siteKey, PublicSiteDataset dataset)
    { 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_GetSiteDataset";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@SiteKey", SqlDbType.VarChar, 32);
                command.Parameters["@SiteKey"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteKey"].Value = siteKey;

                command.Parameters.Add("@CreatorId", SqlDbType.BigInt);
                command.Parameters["@CreatorId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@OwnerName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        SiteData siteData = new SiteData();
                        dataset.siteData = siteData;
                        dataset.siteData.siteKey = siteKey;
                        if (dataReader.Read())
                        {
                            siteData.siteId = (long)dataReader[0];
                            siteData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                siteData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                siteData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                siteData.ssHash = (string)dataReader[4];
                            siteData.guestSupported = (bool)dataReader[5];
                            siteData.guestAllowed = (bool)dataReader[6];
                            siteData.statelessGuestSupported = (bool)dataReader[7];
                            siteData.enabled = (bool)dataReader[8];
                            if (dataReader[9] != DBNull.Value)
                                siteData.description = (string)dataReader[9];
                        }
                        else
                            throw new DataDefinitionSoftnetException();

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        dataset.services = services;
                        while (dataReader.Read())
                        {
                            ServiceData serviceData = new ServiceData();
                            serviceData.serviceId = (long)dataReader[0];
                            if (dataReader[1] != DBNull.Value)
                                serviceData.serviceType = (string)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                serviceData.contractAuthor = (string)dataReader[2];
                            serviceData.version = (string)dataReader[3];
                            serviceData.hostname = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                serviceData.ssHash = (string)dataReader[5];
                            serviceData.enabled = (bool)dataReader[6];
                            serviceData.pingPeriod = (int)dataReader[7];
                            services.Add(serviceData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        UserData guestData = new UserData();
                        dataset.guestData = guestData;
                        if (dataReader.Read())
                        {
                            guestData.userId = (long)dataReader[0];
                            guestData.name = (string)dataReader[1];
                            guestData.enabled = (bool)dataReader[2];
                        }
                        else
                            throw new DataDefinitionSoftnetException();

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ClientData> guestClients = new List<ClientData>();
                        dataset.guestClients = guestClients;
                        while (dataReader.Read())
                        {
                            ClientData clientData = new ClientData();
                            clientData.clientId = (long)dataReader[0];
                            clientData.clientKey = (string)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                clientData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                clientData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                clientData.clientDescription = (string)dataReader[4];
                            clientData.pingPeriod = (int)dataReader[5];
                            guestClients.Add(clientData);
                        }                        
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == 1)
                        throw new InvalidStateSoftnetException("The site is not authorized.");
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The site has not been found.");
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    if (resultCode == -5)
                        throw new DataIntegritySoftnetException();
                    throw new DataDefinitionSoftnetException();
                }

                dataset.siteKey = siteKey;
                dataset.ownerId = (long)command.Parameters["@OwnerId"].Value;
                dataset.ownerName = (string)command.Parameters["@OwnerName"].Value;
                dataset.domainId = (long)command.Parameters["@DomainId"].Value;
                dataset.domainName = (string)command.Parameters["@DomainName"].Value;
                dataset.creatorId = (long)command.Parameters["@CreatorId"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void public_getSiteDataset(string siteKey, PublicSiteDataset dataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_GetSiteDataset2";

                command.Parameters.Add("@SiteKey", SqlDbType.VarChar, 32);
                command.Parameters["@SiteKey"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteKey"].Value = siteKey;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@OwnerName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@GuestEnabled", SqlDbType.Bit);
                command.Parameters["@GuestEnabled"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        SiteData siteData = new SiteData();
                        dataset.siteData = siteData;
                        dataset.siteData.siteKey = siteKey;
                        if (dataReader.Read())
                        {
                            siteData.siteId = (long)dataReader[0];
                            siteData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                siteData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                siteData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                siteData.ssHash = (string)dataReader[4];
                            siteData.guestSupported = (bool)dataReader[5];
                            siteData.guestAllowed = (bool)dataReader[6];
                            siteData.statelessGuestSupported = (bool)dataReader[7];
                            siteData.enabled = (bool)dataReader[8];
                            if (dataReader[9] != DBNull.Value)
                                siteData.description = (string)dataReader[9];
                        }
                        else
                            throw new DataDefinitionSoftnetException();

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        dataset.services = services;
                        while (dataReader.Read())
                        {
                            ServiceData serviceData = new ServiceData();
                            serviceData.serviceId = (long)dataReader[0];
                            if (dataReader[1] != DBNull.Value)
                                serviceData.serviceType = (string)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                serviceData.contractAuthor = (string)dataReader[2];
                            serviceData.version = (string)dataReader[3];
                            serviceData.hostname = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                serviceData.ssHash = (string)dataReader[5];
                            serviceData.enabled = (bool)dataReader[6];
                            serviceData.pingPeriod = (int)dataReader[7];
                            services.Add(serviceData);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == 1)
                        throw new InvalidStateSoftnetException("The site is not authorized.");
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The site has not been found.");
                    if (resultCode == -5)
                        throw new DataIntegritySoftnetException();
                    if (resultCode == 1)
                        throw new InvalidStateSoftnetException("The site is empty.");
                    throw new DataDefinitionSoftnetException();
                }

                dataset.siteKey = siteKey;
                dataset.ownerId = (long)command.Parameters["@OwnerId"].Value;
                dataset.domainId = (long)command.Parameters["@DomainId"].Value;
                dataset.ownerName = (string)command.Parameters["@OwnerName"].Value;
                dataset.domainName = (string)command.Parameters["@DomainName"].Value;
                dataset.guestEnabled = (bool)command.Parameters["@GuestEnabled"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }    
    }

    public static void public_getGuestEditingDataset(string clientKey, string accessKey, GuestEditingDataset dataset)
    { 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_GetGuestEditingDataset";

                command.Parameters.Add("@ClientKey", SqlDbType.VarChar, 32);
                command.Parameters["@ClientKey"].Direction = ParameterDirection.Input;
                command.Parameters["@ClientKey"].Value = clientKey;

                command.Parameters.Add("@AccessKey", SqlDbType.VarChar, 32);
                command.Parameters["@AccessKey"].Direction = ParameterDirection.Input;
                command.Parameters["@AccessKey"].Value = accessKey;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@OwnerName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@GuestEnabled", SqlDbType.Bit);
                command.Parameters["@GuestEnabled"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@SecretKey", SqlDbType.NVarChar, 64);
                command.Parameters["@SecretKey"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        SiteData siteData = new SiteData();
                        if (dataReader.Read())
                        {                            
                            siteData.siteId = (long)dataReader[0];
                            siteData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                siteData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                siteData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                siteData.ssHash = (string)dataReader[4];
                            siteData.guestAllowed = (bool)dataReader[5];
                            siteData.statelessGuestSupported = (bool)dataReader[6];                            
                            siteData.enabled = (bool)dataReader[7];
                            siteData.siteKey = (string)dataReader[8];
                            if (dataReader[9] != DBNull.Value)
                                siteData.description = (string)dataReader[9];
                        }
                        else
                            throw new ArgumentSoftnetException("The site has not been found.");

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        while (dataReader.Read())
                        {
                            ServiceData serviceData = new ServiceData();
                            serviceData.serviceId = (long)dataReader[0];
                            if (dataReader[1] != DBNull.Value)
                                serviceData.serviceType = (string)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                serviceData.contractAuthor = (string)dataReader[2];
                            serviceData.version = (string)dataReader[3];
                            serviceData.hostname = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                serviceData.ssHash = (string)dataReader[5];
                            serviceData.enabled = (bool)dataReader[6];
                            serviceData.pingPeriod = (int)dataReader[7];
                            services.Add(serviceData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        ClientData clientData = new ClientData();
                        if (dataReader.Read())
                        {
                            clientData.clientId = (long)dataReader[0];                            
                            if (dataReader[1] != DBNull.Value)
                                clientData.serviceType = (string)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                clientData.contractAuthor = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                clientData.clientDescription = (string)dataReader[3];
                            clientData.pingPeriod = (int)dataReader[4];
                        }
                        else
                            throw new ArgumentSoftnetException("The client has not been found.");

                        dataset.siteData = siteData;
                        dataset.services = services;
                        dataset.clientData = clientData;
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == 1)
                        throw new ArgumentSoftnetException("The confirmation url is not valid.");
                    if (resultCode == 2)
                        throw new ArgumentSoftnetException("The confirmation url has expired.");
                    if (resultCode == 3)
                        throw new ArgumentSoftnetException("The site is not authorized.");
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The client has not been found.");
                    if (resultCode == -3)
                        throw new GeneralSettingsSoftnetException("The secret key is not specified in the general settings.");
                    if (resultCode == -5)
                        throw new DataIntegritySoftnetException();
                    throw new DataDefinitionSoftnetException();
                }

                dataset.ownerId = (long)command.Parameters["@OwnerId"].Value;
                dataset.domainId = (long)command.Parameters["@DomainId"].Value;
                dataset.ownerName = (string)command.Parameters["@OwnerName"].Value;
                dataset.domainName = (string)command.Parameters["@DomainName"].Value;
                dataset.guestEnabled = (bool)command.Parameters["@GuestEnabled"].Value;
                dataset.email = (string)command.Parameters["@EMail"].Value;
                dataset.secretKey = (string)command.Parameters["@SecretKey"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static string public_SaveAccountAccessKey(long clientId, string accessKey)
    { 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_SaveAccountAccessKey";

                command.Parameters.Add("@ClientId", SqlDbType.BigInt);
                command.Parameters["@ClientId"].Direction = ParameterDirection.Input;
                command.Parameters["@ClientId"].Value = clientId;

                command.Parameters.Add("@AccessKey", SqlDbType.VarChar, 64);
                command.Parameters["@AccessKey"].Direction = ParameterDirection.InputOutput;
                command.Parameters["@AccessKey"].Value = accessKey;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The client has not been found.");
                    throw new DataDefinitionSoftnetException();
                }

                return (string)command.Parameters["@AccessKey"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static int public_getDomainDatasetForEMail(string email, DomainDatasetForEMail dataset)
    { 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_GetDomainDatasetForEMail";

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Input;
                command.Parameters["@EMail"].Value = email;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainId"].Value = dataset.domainId;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@OwnerName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@GuestEnabled", SqlDbType.Bit);
                command.Parameters["@GuestEnabled"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        List<SiteData> sites = new List<SiteData>();
                        while (dataReader.Read())
                        {
                            SiteData siteData = new SiteData();
                            siteData.siteId = (long)dataReader[0];
                            siteData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                siteData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                siteData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                siteData.ssHash = (string)dataReader[4];
                            siteData.guestSupported = (bool)dataReader[5];
                            siteData.guestAllowed = (bool)dataReader[6];
                            siteData.statelessGuestSupported = (bool)dataReader[7];
                            if (dataReader[8] != DBNull.Value)
                                siteData.siteKey = (string)dataReader[8];
                            siteData.enabled = (bool)dataReader[9];
                            if (dataReader[10] != DBNull.Value)
                                siteData.description = (string)dataReader[10];
                            sites.Add(siteData);
                        }
                        if (sites.Count == 0)
                            return 1;

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        while (dataReader.Read())
                        {
                            ServiceData serviceData = new ServiceData();
                            serviceData.serviceId = (long)dataReader[0];
                            serviceData.siteId = (long)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                serviceData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                serviceData.contractAuthor = (string)dataReader[3];
                            serviceData.version = (string)dataReader[4];
                            serviceData.hostname = (string)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                serviceData.ssHash = (string)dataReader[6];
                            serviceData.enabled = (bool)dataReader[7];
                            serviceData.pingPeriod = (int)dataReader[8];
                            services.Add(serviceData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ClientData> clients = new List<ClientData>();
                        while (dataReader.Read())
                        {
                            ClientData clientData = new ClientData();
                            clientData.clientId = (long)dataReader[0];
                            clientData.siteId = (long)dataReader[1];
                            clientData.clientKey = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                clientData.serviceType = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                clientData.contractAuthor = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                clientData.clientDescription = (string)dataReader[5];
                            clientData.pingPeriod = (int)dataReader[6];
                            clients.Add(clientData);
                        }

                        dataset.sites = sites;
                        dataset.services = services;
                        dataset.clients = clients;
                    }                    
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode == 0)
                {
                    dataset.ownerId = (long)command.Parameters["@OwnerId"].Value;
                    dataset.ownerName = (string)command.Parameters["@OwnerName"].Value;
                    dataset.domainName = (string)command.Parameters["@DomainName"].Value;
                    dataset.guestEnabled = (bool)command.Parameters["@GuestEnabled"].Value;
                    return 0;
                }
                else if (resultCode == 1)
                {
                    throw new InvalidStateSoftnetException();
                }
                else
                {
                    if (resultCode == -5)
                        throw new DataIntegritySoftnetException();
                    throw new DataDefinitionSoftnetException();
                }                
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void public_getDomainsForEMail(string email, List<OwnerData> owners, List<DomainData> domains)
    { 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_GetDomainsForEMail";

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Input;
                command.Parameters["@EMail"].Value = email;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            DomainData domainData = new DomainData();
                            domainData.domainId = (long)dataReader[0];
                            domainData.ownerId = (long)dataReader[1];
                            domainData.domainName = (string)dataReader[2];
                            domains.Add(domainData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        while (dataReader.Read())
                        {
                            OwnerData ownerData = new OwnerData();
                            ownerData.ownerId = (long)dataReader[0];
                            ownerData.fullName = (string)dataReader[1];
                            owners.Add(ownerData);
                        }
                    }                    
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static string public_createGuestClient(long siteId, string saltedPassword, string salt, string email, string creationKey)
    {
        int clientKeyLength = SoftnetRegistry.settings_getClientKeyLength(); 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_ClientKeyExists";

                command.Parameters.Add("@ClientKey", SqlDbType.VarChar, 32);
                command.Parameters["@ClientKey"].Direction = ParameterDirection.Input;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                string clientKey = null;
                for (int keyLength = clientKeyLength; keyLength <= 32; keyLength++)
                {
                    clientKey = Randomizer.generateClientKey(keyLength);
                    command.Parameters["@ClientKey"].Value = clientKey;
                    command.ExecuteNonQuery();
                    if (((int)command.Parameters["@ReturnValue"].Value) == 0)
                        break;
                    if (keyLength == 32)
                        throw new OperationFailedSoftnetException("Failed to generate a client key.");
                }

                command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_CreateClient";

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteId"].Value = siteId;

                command.Parameters.Add("@ClientKey", SqlDbType.VarChar, 32);
                command.Parameters["@ClientKey"].Direction = ParameterDirection.Input;
                command.Parameters["@ClientKey"].Value = clientKey;

                command.Parameters.Add("@SaltedPassword", SqlDbType.VarChar, 64);
                command.Parameters["@SaltedPassword"].Direction = ParameterDirection.Input;
                command.Parameters["@SaltedPassword"].Value = saltedPassword;

                command.Parameters.Add("@Salt", SqlDbType.VarChar, 64);
                command.Parameters["@Salt"].Direction = ParameterDirection.Input;
                command.Parameters["@Salt"].Value = salt;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Input;
                command.Parameters["@EMail"].Value = email;

                command.Parameters.Add("@CreationKey", SqlDbType.VarChar, 64);
                command.Parameters["@CreationKey"].Direction = ParameterDirection.Input;
                command.Parameters["@CreationKey"].Value = creationKey;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == 1)
                        throw new ArgumentSoftnetException("Guest access is denied.");
                    if (resultCode == 2)
                        throw new ArgumentSoftnetException("The confirmation url has already been applied.");
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The site not found.");
                    if (resultCode == -5)
                        throw new DataIntegritySoftnetException();
                    throw new DataDefinitionSoftnetException();
                }

                return clientKey;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void public_getGuestCreatingDataset(string siteKey, string transactionKey, GuestCreatingDataset dataset)
    { 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_GetGuestCreatingDataset";

                command.Parameters.Add("@SiteKey", SqlDbType.VarChar, 32);
                command.Parameters["@SiteKey"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteKey"].Value = siteKey;

                command.Parameters.Add("@TranKey", SqlDbType.VarChar, 64);
                command.Parameters["@TranKey"].Direction = ParameterDirection.Input;
                command.Parameters["@TranKey"].Value = transactionKey;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@OwnerName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@SecretKey", SqlDbType.NVarChar, 64);
                command.Parameters["@SecretKey"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@CurrentTime", SqlDbType.BigInt);
                command.Parameters["@CurrentTime"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        SiteData siteData = new SiteData();
                        if (dataReader.Read())
                        {
                            siteData.siteId = (long)dataReader[0];
                            siteData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                siteData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                siteData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                siteData.ssHash = (string)dataReader[4];
                            siteData.statelessGuestSupported = (bool)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                siteData.description = (string)dataReader[6];
                        }
                        else
                            throw new ArgumentSoftnetException("The site not found.");

                        if (dataReader.NextResult() == false)                        
                            throw new DataDefinitionSoftnetException();
                        
                        List<ServiceData> services = new List<ServiceData>();
                        while (dataReader.Read())
                        {
                            ServiceData serviceData = new ServiceData();
                            serviceData.serviceId = (long)dataReader[0];                            
                            if (dataReader[1] != DBNull.Value)
                                serviceData.serviceType = (string)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                serviceData.contractAuthor = (string)dataReader[2];
                            serviceData.version = (string)dataReader[3];
                            serviceData.hostname = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                serviceData.ssHash = (string)dataReader[5];
                            serviceData.enabled = (bool)dataReader[6];
                            serviceData.pingPeriod = (int)dataReader[7];
                            services.Add(serviceData);
                        }

                        dataset.siteData = siteData;
                        dataset.services = services;
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == 1)
                        throw new ArgumentSoftnetException("The confirmation url has already been applied.");
                    if (resultCode == 2)
                        throw new InvalidStateSoftnetException("The site is unauthorized.");
                    if (resultCode == 3)
                        throw new InvalidStateSoftnetException("The site is disabled.");
                    if (resultCode == 4)
                        throw new InvalidStateSoftnetException("The site is empty.");
                    if (resultCode == 5)
                        throw new InvalidStateSoftnetException("The guest access is denied.");
                    if (resultCode == 6)
                        throw new InvalidStateSoftnetException("The guest is disabled.");
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The site not found.");
                    if (resultCode == -3)
                        throw new InvalidStateSoftnetException("The secret key is not specified in the general settings.");
                    if (resultCode == -5)
                        throw new DataIntegritySoftnetException();
                    throw new DataDefinitionSoftnetException();
                }

                string secretKey = (string)command.Parameters["@SecretKey"].Value;
                if (string.IsNullOrWhiteSpace(secretKey))
                    throw new GeneralSettingsSoftnetException("The secret key is not specified in the general settings.");

                dataset.ownerId = (long)command.Parameters["@OwnerId"].Value;
                dataset.domainId = (long)command.Parameters["@DomainId"].Value;
                dataset.ownerName = (string)command.Parameters["@OwnerName"].Value;
                dataset.domainName = (string)command.Parameters["@DomainName"].Value;
                dataset.secretKey = secretKey;
                dataset.currentTime = (long)command.Parameters["@CurrentTime"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void public_getDomainDataset(PublicDomainDataset dataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_GetDomainDataset2";

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainId"].Value = dataset.domainId;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@OwnerName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        List<SiteData> sites = new List<SiteData>();
                        dataset.sites = sites;
                        while (dataReader.Read())
                        {
                            SiteData siteData = new SiteData();
                            siteData.siteId = (long)dataReader[0];
                            siteData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                siteData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                siteData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                siteData.ssHash = (string)dataReader[4];
                            siteData.statelessGuestSupported = (bool)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                siteData.siteKey = (string)dataReader[6];
                            if (dataReader[7] != DBNull.Value)
                                siteData.description = (string)dataReader[7];
                            sites.Add(siteData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        dataset.services = services;
                        while (dataReader.Read())
                        {
                            ServiceData serviceData = new ServiceData();
                            serviceData.serviceId = (long)dataReader[0];
                            serviceData.siteId = (long)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                serviceData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                serviceData.contractAuthor = (string)dataReader[3];
                            serviceData.version = (string)dataReader[4];
                            serviceData.hostname = (string)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                serviceData.ssHash = (string)dataReader[6];
                            serviceData.enabled = (bool)dataReader[7];
                            serviceData.pingPeriod = (int)dataReader[8];
                            services.Add(serviceData);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode == 0)
                {
                    dataset.ownerId = (long)command.Parameters["@OwnerId"].Value;
                    dataset.ownerName = (string)command.Parameters["@OwnerName"].Value;
                    dataset.domainName = (string)command.Parameters["@DomainName"].Value;
                }
                else if (resultCode == 1)
                {
                    throw new InvalidStateSoftnetException();
                }
                else
                {
                    if (resultCode == -5)
                        throw new DataIntegritySoftnetException();
                    throw new DataDefinitionSoftnetException();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void public_getDomainDataset(string accountName, PublicDomainDataset dataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_GetDomainDataset";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainId"].Value = dataset.domainId;

                command.Parameters.Add("@CreatorId", SqlDbType.BigInt);
                command.Parameters["@CreatorId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@GuestId", SqlDbType.BigInt);
                command.Parameters["@GuestId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@OwnerName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        List<SiteData> sites = new List<SiteData>();
                        dataset.sites = sites;
                        while (dataReader.Read())
                        {
                            SiteData siteData = new SiteData();
                            siteData.siteId = (long)dataReader[0];
                            siteData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                siteData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                siteData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                siteData.ssHash = (string)dataReader[4];
                            siteData.statelessGuestSupported = (bool)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                siteData.siteKey = (string)dataReader[6];                            
                            if (dataReader[7] != DBNull.Value)
                                siteData.description = (string)dataReader[7];
                            sites.Add(siteData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        dataset.services = services;
                        while (dataReader.Read())
                        {
                            ServiceData serviceData = new ServiceData();
                            serviceData.serviceId = (long)dataReader[0];
                            serviceData.siteId = (long)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                serviceData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                serviceData.contractAuthor = (string)dataReader[3];
                            serviceData.version = (string)dataReader[4];
                            serviceData.hostname = (string)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                serviceData.ssHash = (string)dataReader[6];
                            serviceData.enabled = (bool)dataReader[7];
                            serviceData.pingPeriod = (int)dataReader[8];
                            services.Add(serviceData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ClientData> guestClients = new List<ClientData>();
                        dataset.guestClients = guestClients;
                        while (dataReader.Read())
                        {
                            ClientData clientData = new ClientData();
                            clientData.clientId = (long)dataReader[0];
                            clientData.siteId = (long)dataReader[1];
                            clientData.clientKey = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                clientData.serviceType = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                clientData.contractAuthor = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                clientData.clientDescription = (string)dataReader[5];
                            clientData.pingPeriod = (int)dataReader[6];
                            guestClients.Add(clientData);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode == 0)
                {
                    dataset.creatorId = (long)command.Parameters["@CreatorId"].Value;
                    dataset.ownerId = (long)command.Parameters["@OwnerId"].Value;
                    dataset.ownerName = (string)command.Parameters["@OwnerName"].Value;
                    dataset.domainName = (string)command.Parameters["@DomainName"].Value;

                    UserData guestData = new UserData();
                    dataset.guestData = guestData;
                    guestData.userId = (long)command.Parameters["@GuestId"].Value;
                    guestData.name = "Guest";
                }
                else if (resultCode == 1)
                {
                    throw new InvalidStateSoftnetException();                
                }
                else
                {                    
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    if (resultCode == -5)
                        throw new DataIntegritySoftnetException();
                    throw new DataDefinitionSoftnetException();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void public_getOwnerDomains(OwnerData ownerData, List<DomainItem> publicDomains)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_GetOwnerDomains";

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Input;
                command.Parameters["@OwnerId"].Value = ownerData.ownerId;

                command.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@OwnerName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            DomainItem domainItem = new DomainItem();
                            domainItem.domainId = (long)dataReader[0];
                            domainItem.domainName = (string)dataReader[1];
                            publicDomains.Add(domainItem);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int statusCode = (int)command.Parameters["@ReturnValue"].Value;
                if (statusCode != 0)
                    throw new InvalidStateSoftnetException();
                ownerData.fullName = (string)command.Parameters["@OwnerName"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void public_findUsers(string filter, List<OwnerData> users)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_FindUsers";

                command.Parameters.Add("@Filter", SqlDbType.NVarChar, 256);
                command.Parameters["@Filter"].Direction = ParameterDirection.Input;
                command.Parameters["@Filter"].Value = filter;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            OwnerData foundUser = new OwnerData();
                            foundUser.ownerId = (long)dataReader[0];
                            foundUser.fullName = (string)dataReader[1];
                            users.Add(foundUser);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void public_findNextUsers(string filter, long lastSelectedId, List<OwnerData> users)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_FindNextUsers";

                command.Parameters.Add("@Filter", SqlDbType.NVarChar, 256);
                command.Parameters["@Filter"].Direction = ParameterDirection.Input;
                command.Parameters["@Filter"].Value = filter;

                command.Parameters.Add("@LastSelectedId", SqlDbType.BigInt);
                command.Parameters["@LastSelectedId"].Direction = ParameterDirection.Input;
                command.Parameters["@LastSelectedId"].Value = lastSelectedId;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            OwnerData foundUser = new OwnerData();
                            foundUser.ownerId = (long)dataReader[0];
                            foundUser.fullName = (string)dataReader[1];
                            users.Add(foundUser);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void public_findPrevUsers(string filter, long firstSelectedId, List<OwnerData> users)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtPublic_FindPrevUsers";

                command.Parameters.Add("@Filter", SqlDbType.NVarChar, 256);
                command.Parameters["@Filter"].Direction = ParameterDirection.Input;
                command.Parameters["@Filter"].Value = filter;

                command.Parameters.Add("@FirstSelectedId", SqlDbType.BigInt);
                command.Parameters["@FirstSelectedId"].Direction = ParameterDirection.Input;
                command.Parameters["@FirstSelectedId"].Value = firstSelectedId;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            OwnerData foundUser = new OwnerData();
                            foundUser.ownerId = (long)dataReader[0];
                            foundUser.fullName = (string)dataReader[1];
                            users.Add(foundUser);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void account_getData(AccountData data)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_GetData";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = data.accountName;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@OwnerName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resutCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resutCode != 0)
                {
                    if (resutCode == -2)
                        throw new AccountNotFoundSoftnetException(data.accountName);
                    throw new DataDefinitionSoftnetException();
                }

                data.ownerId = (long)command.Parameters["@OwnerId"].Value;
                data.ownerName = (string)command.Parameters["@OwnerName"].Value;
                if (command.Parameters["@EMail"].Value != DBNull.Value)
                    data.email = (string)command.Parameters["@EMail"].Value;                
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void account_setOwnerName(long ownerId, string ownerName)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_SetOwnerName";

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Input;
                command.Parameters["@OwnerId"].Value = ownerId;

                command.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@OwnerName"].Direction = ParameterDirection.Input;
                command.Parameters["@OwnerName"].Value = ownerName;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void account_changePassword(long ownerId, string saltedPassword, string salt)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_ChangePassword";

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Input;
                command.Parameters["@OwnerId"].Value = ownerId;

                command.Parameters.Add("@SaltedPassword", SqlDbType.NVarChar, 128);
                command.Parameters["@SaltedPassword"].Direction = ParameterDirection.Input;
                command.Parameters["@SaltedPassword"].Value = saltedPassword;

                command.Parameters.Add("@Salt", SqlDbType.NVarChar, 128);
                command.Parameters["@Salt"].Direction = ParameterDirection.Input;
                command.Parameters["@Salt"].Value = salt;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void account_initRecoveryOnName(AccountTransactionData data)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_InitRecoveryOnName";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = data.accountName;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@TransactionKey", SqlDbType.VarChar, 64);
                command.Parameters["@TransactionKey"].Direction = ParameterDirection.InputOutput;
                command.Parameters["@TransactionKey"].Value = data.transactionKey;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException(string.Format("The account '{0}' does not exist.", data.accountName));
                    if (resultCode == 1)
                        throw new ArgumentSoftnetException(string.Format("Sorry, the account '{0}' has no recovery email.", data.accountName));
                    throw new DataDefinitionSoftnetException();
                }

                data.email = (string)command.Parameters["@EMail"].Value;
                data.transactionKey = (string)command.Parameters["@TransactionKey"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void account_initRecoveryOnEmail(AccountTransactionData data)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_InitRecoveryOnEMail";

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Input;
                command.Parameters["@EMail"].Value = data.email;

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@TransactionKey", SqlDbType.VarChar, 64);
                command.Parameters["@TransactionKey"].Direction = ParameterDirection.InputOutput;
                command.Parameters["@TransactionKey"].Value = data.transactionKey;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException(string.Format("There is no account for the email '{0}'.", data.email));
                    if (resultCode == -5)
                        throw new DataIntegritySoftnetException(string.Format("There are multiple accounts for the email '{0}'.", data.email));
                    throw new DataDefinitionSoftnetException();
                }

                data.accountName = (string)command.Parameters["@AccountName"].Value;
                data.transactionKey = (string)command.Parameters["@TransactionKey"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void account_initEMailConfirmation(AccountTransactionData data)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_InitEMailConfirmation";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = data.accountName;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Input;
                command.Parameters["@EMail"].Value = data.email;

                command.Parameters.Add("@TransactionKey", SqlDbType.VarChar, 64);
                command.Parameters["@TransactionKey"].Direction = ParameterDirection.InputOutput;
                command.Parameters["@TransactionKey"].Value = data.transactionKey;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == 1)
                        throw new ArgumentSoftnetException("Sorry, the email is already in use.");
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(data.accountName);
                    throw new DataDefinitionSoftnetException();
                }

                data.transactionKey = (string)command.Parameters["@TransactionKey"].Value;                
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void account_getTransactionData(AccountTransactionData2 tranData)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_GetTransactionData";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = tranData.accountName;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@SecretKey", SqlDbType.NVarChar, 64);
                command.Parameters["@SecretKey"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@TransactionKey", SqlDbType.VarChar, 64);
                command.Parameters["@TransactionKey"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@CreatedTime", SqlDbType.BigInt);
                command.Parameters["@CreatedTime"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@CurrentTime", SqlDbType.BigInt);
                command.Parameters["@CurrentTime"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException(string.Format("The account '{0}' has not been found.", tranData.accountName));
                    if (resultCode == -3)
                        throw new GeneralSettingsSoftnetException("The secret key is not specified in the general settings.");                    
                    throw new DataDefinitionSoftnetException();
                }

                tranData.ownerId = (long)command.Parameters["@OwnerId"].Value;
                tranData.secretKey = (string)command.Parameters["@SecretKey"].Value;
                if (command.Parameters["@EMail"].Value != DBNull.Value)
                    tranData.email = (string)command.Parameters["@EMail"].Value;
                if (command.Parameters["@TransactionKey"].Value != DBNull.Value)
                    tranData.transactionKey = (string)command.Parameters["@TransactionKey"].Value;
                tranData.createdTime = (long)command.Parameters["@CreatedTime"].Value;
                tranData.currentTime = (long)command.Parameters["@CurrentTime"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void account_setEMail(AccountTransactionData2 tranData, string email)
    { 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_SetEMail";

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Input;
                command.Parameters["@OwnerId"].Value = tranData.ownerId;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Input;
                command.Parameters["@EMail"].Value = email;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == 1)
                        throw new ArgumentSoftnetException("Sorry, the email is already in use.");
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException(string.Format("The account '{0}' not found.", tranData.accountName));
                    throw new DataDefinitionSoftnetException();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void account_resetPassword(AccountTransactionData2 tranData, string saltedPassword, string salt)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_ResetPassword";

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Input;
                command.Parameters["@OwnerId"].Value = tranData.ownerId;

                command.Parameters.Add("@SaltedPassword", SqlDbType.NVarChar, 128);
                command.Parameters["@SaltedPassword"].Direction = ParameterDirection.Input;
                command.Parameters["@SaltedPassword"].Value = saltedPassword;

                command.Parameters.Add("@Salt", SqlDbType.NVarChar, 128);
                command.Parameters["@Salt"].Direction = ParameterDirection.Input;
                command.Parameters["@Salt"].Value = salt;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resutCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resutCode != 0)
                {
                    if (resutCode == -1)
                        throw new ArgumentSoftnetException(string.Format("The account '{0}' not found.", tranData.accountName));
                    throw new DataDefinitionSoftnetException();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void account_unlock(AccountTransactionData2 tranData)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_Unlock";

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Input;
                command.Parameters["@OwnerId"].Value = tranData.ownerId;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resutCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resutCode != 0)
                {
                    if (resutCode == -1)
                        throw new ArgumentSoftnetException(string.Format("The account '{0}' has not been found.", tranData.accountName));
                    throw new DataDefinitionSoftnetException();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static int account_getInvitationStatus(string invKey)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_GetInvitationStatus";

                command.Parameters.Add("@IKey", SqlDbType.VarChar, 64);
                command.Parameters["@IKey"].Direction = ParameterDirection.Input;
                command.Parameters["@IKey"].Value = invKey;

                command.Parameters.Add("@Status", SqlDbType.Int);
                command.Parameters["@Status"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resutCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resutCode != 0)
                {
                    if (resutCode == -1)
                        throw new ArgumentSoftnetException("The invitation key not found.");
                    throw new DataDefinitionSoftnetException();
                }

                return (int)command.Parameters["@Status"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void account_signupUser(string userFullName, string accountName, string email, string saltedPassword, string salt)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_SignupUser";

                command.Parameters.Add("@UserFullName", SqlDbType.NVarChar, 256);
                command.Parameters["@UserFullName"].Direction = ParameterDirection.Input;
                command.Parameters["@UserFullName"].Value = userFullName;

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 1024);
                command.Parameters["@EMail"].Direction = ParameterDirection.Input;
                command.Parameters["@EMail"].Value = email;

                command.Parameters.Add("@SaltedPassword", SqlDbType.NVarChar, 128);
                command.Parameters["@SaltedPassword"].Direction = ParameterDirection.Input;
                command.Parameters["@SaltedPassword"].Value = saltedPassword;

                command.Parameters.Add("@Salt", SqlDbType.NVarChar, 128);
                command.Parameters["@Salt"].Direction = ParameterDirection.Input;
                command.Parameters["@Salt"].Value = salt;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resutCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resutCode != 0)
                {
                    if (resutCode == 2)
                        throw new ArgumentSoftnetException(string.Format("The email '{0}' is already in use.", email));
                    if (resutCode == 3)
                        throw new ArgumentSoftnetException(string.Format("The account name '{0}' is already in use. Try another one.", accountName));
                    if (resutCode == 4)
                        throw new OperationFailedSoftnetException("Failed to create an account.");
                    if (resutCode == -4)
                        throw new MembershipSoftnetException();
                    if (resutCode == -10)
                        throw new DatabaseSoftnetException("Sql error in 'AspNetMembership' on creating user account.");
                    throw new DataDefinitionSoftnetException();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void account_signupByInvitation(string ikey, string userFullName, string accountName, string email, string saltedPassword, string salt)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAccount_SignupByInvitation";

                command.Parameters.Add("@IKey", SqlDbType.VarChar, 64);
                command.Parameters["@IKey"].Direction = ParameterDirection.Input;
                command.Parameters["@IKey"].Value = ikey;

                command.Parameters.Add("@UserFullName", SqlDbType.NVarChar, 256);
                command.Parameters["@UserFullName"].Direction = ParameterDirection.Input;
                command.Parameters["@UserFullName"].Value = userFullName;

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Input;
                command.Parameters["@EMail"].Value = email;

                command.Parameters.Add("@SaltedPassword", SqlDbType.NVarChar, 128);
                command.Parameters["@SaltedPassword"].Direction = ParameterDirection.Input;
                command.Parameters["@SaltedPassword"].Value = saltedPassword;

                command.Parameters.Add("@Salt", SqlDbType.NVarChar, 128);
                command.Parameters["@Salt"].Direction = ParameterDirection.Input;
                command.Parameters["@Salt"].Value = salt;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resutCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resutCode != 0)
                {
                    if (resutCode == 1)
                        throw new ArgumentSoftnetException("The invitation has already been applied.");
                    if (resutCode == 2)
                        throw new ArgumentSoftnetException("Sorry, the invitation has expired.");
                    if (resutCode == 3)
                        throw new ArgumentSoftnetException(string.Format("The email '{0}' is already in use. Try another one.", email));
                    if (resutCode == 4)
                        throw new ArgumentSoftnetException(string.Format("The account name '{0}' is already in use. Try another one.", accountName));
                    if (resutCode == 5)
                        throw new OperationFailedSoftnetException("Failed to create an account.");
                    if (resutCode == -1)
                        throw new ArgumentSoftnetException("The invitation not found.");
                    if (resutCode == -4)
                        throw new MembershipSoftnetException();
                    if (resutCode == -10)
                        throw new DatabaseSoftnetException("Sql error in 'AspNetMembership' on creating user account.");                    
                    throw new DataDefinitionSoftnetException();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_getInvitationData(string accountName, InvitationData data)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_GetInvitationData";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@InvitationId", SqlDbType.BigInt);
                command.Parameters["@InvitationId"].Direction = ParameterDirection.Input;
                command.Parameters["@InvitationId"].Value = data.invitationId;

                command.Parameters.Add("@IKey", SqlDbType.VarChar, 64);
                command.Parameters["@IKey"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@Description", SqlDbType.NVarChar, 1024);
                command.Parameters["@Description"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@AssignProviderRole", SqlDbType.Bit);
                command.Parameters["@AssignProviderRole"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@Status", SqlDbType.Int);
                command.Parameters["@Status"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@NewUserId", SqlDbType.BigInt);
                command.Parameters["@NewUserId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@NewUserName", SqlDbType.NVarChar, 256);
                command.Parameters["@NewUserName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resutCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resutCode != 0)
                {
                    if (resutCode == -1)
                        throw new ArgumentSoftnetException("The invitation not found.");
                    if (resutCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    throw new DataDefinitionSoftnetException();
                }

                data.ikey = (string)command.Parameters["@IKey"].Value;
                data.email = (string)command.Parameters["@EMail"].Value;
                data.description = (string)command.Parameters["@Description"].Value;
                data.assignProviderRole = (bool)command.Parameters["@AssignProviderRole"].Value;
                data.status = (int)command.Parameters["@Status"].Value;
                if (command.Parameters["@NewUserId"].Value != DBNull.Value)
                    data.newUserId = (long)command.Parameters["@NewUserId"].Value;
                if (command.Parameters["@NewUserName"].Value != DBNull.Value)
                    data.newUserName = (string)command.Parameters["@NewUserName"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_updateInvitationData(long invitationId, string email, string description, bool assignProviderRole)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_UpdateInvitationData";

                command.Parameters.Add("@InvitationId", SqlDbType.BigInt);
                command.Parameters["@InvitationId"].Direction = ParameterDirection.Input;
                command.Parameters["@InvitationId"].Value = invitationId;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Input;
                command.Parameters["@EMail"].Value = email;

                command.Parameters.Add("@Description", SqlDbType.NVarChar, 1024);
                command.Parameters["@Description"].Direction = ParameterDirection.Input;
                command.Parameters["@Description"].Value = description;

                command.Parameters.Add("@AssignProviderRole", SqlDbType.Bit);
                command.Parameters["@AssignProviderRole"].Direction = ParameterDirection.Input;
                command.Parameters["@AssignProviderRole"].Value = assignProviderRole;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static long admin_addNewInvitation(string accountName, string ikey, string email, string description, bool assignProviderRole)
    { 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_AddNewInvitation";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@IKey", SqlDbType.VarChar, 64);
                command.Parameters["@IKey"].Direction = ParameterDirection.Input;
                command.Parameters["@IKey"].Value = ikey;

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Input;
                command.Parameters["@EMail"].Value = email;

                command.Parameters.Add("@Description", SqlDbType.NVarChar, 1024);
                command.Parameters["@Description"].Direction = ParameterDirection.Input;
                command.Parameters["@Description"].Value = description;

                command.Parameters.Add("@AssignProviderRole", SqlDbType.Bit);
                command.Parameters["@AssignProviderRole"].Direction = ParameterDirection.Input;
                command.Parameters["@AssignProviderRole"].Value = assignProviderRole;

                command.Parameters.Add("@InvitationId", SqlDbType.BigInt);
                command.Parameters["@InvitationId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resutCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resutCode != 0)
                {
                    if (resutCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    throw new DataDefinitionSoftnetException();
                }
                return (long)command.Parameters["@InvitationId"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_getInvitationList(string accountName, List<InvitationData> invitations)
    { 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_GetInvitationList";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            InvitationData inv = new InvitationData();
                            inv.invitationId = (long)dataReader[0];
                            inv.ikey = (string)dataReader[1];
                            inv.email = (string)dataReader[2];
                            inv.description = (string)dataReader[3];
                            inv.assignProviderRole = (bool)dataReader[4];
                            inv.status = (int)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                            {
                                inv.newUserId = (long)dataReader[6];
                                inv.newUserName = (string)dataReader[7];
                                inv.newUserAuthority = (int)dataReader[8];
                            }
                            else if (inv.status == 1)
                            {
                                inv.status = 3;
                            }
                            invitations.Add(inv);
                        }
                    }                    
                }
                finally
                {
                    dataReader.Close();
                }

                int resutCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resutCode != 0)
                {
                    if (resutCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    throw new DataDefinitionSoftnetException();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_deleteInvitation(string accountName, long invitationId)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_DeleteInvitation";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@InvitationId", SqlDbType.BigInt);
                command.Parameters["@InvitationId"].Direction = ParameterDirection.Input;
                command.Parameters["@InvitationId"].Value = invitationId;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_clearAcceptedInvitationList(string accountName)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_ClearAcceptedInvitationList";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }   
    }

    public static void admin_clearExpiredInvitationList(string accountName)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_ClearExpiredInvitationList";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_getGeneralSettings(SettingsData settingsData)
    { 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_GetGeneralSettings";

                command.Parameters.Add("@ManagementSystemUrl", SqlDbType.NVarChar, 1024);
                command.Parameters["@ManagementSystemUrl"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ServerAddress", SqlDbType.NVarChar, 1024);
                command.Parameters["@ServerAddress"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@SecretKey", SqlDbType.NVarChar, 64);
                command.Parameters["@SecretKey"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@AnyoneCanRegister", SqlDbType.NVarChar, 1);
                command.Parameters["@AnyoneCanRegister"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@AssignProviderRole", SqlDbType.NVarChar, 1);
                command.Parameters["@AssignProviderRole"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@UserPasswordMinLength", SqlDbType.NVarChar, 2);
                command.Parameters["@UserPasswordMinLength"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ServicePasswordLength", SqlDbType.NVarChar, 2);
                command.Parameters["@ServicePasswordLength"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ClientPasswordLength", SqlDbType.NVarChar, 2);
                command.Parameters["@ClientPasswordLength"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ClientKeyLength", SqlDbType.NVarChar, 2);
                command.Parameters["@ClientKeyLength"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@EmailAddress", SqlDbType.NVarChar, 1024);
                command.Parameters["@EmailAddress"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@EmailPassword", SqlDbType.NVarChar, 64);
                command.Parameters["@EmailPassword"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@SmtpServer", SqlDbType.NVarChar, 1024);
                command.Parameters["@SmtpServer"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@SmtpPort", SqlDbType.NVarChar, 5);
                command.Parameters["@SmtpPort"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@SmtpRequiresSSL", SqlDbType.NVarChar, 1);
                command.Parameters["@SmtpRequiresSSL"].Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();

                if (command.Parameters["@ManagementSystemUrl"].Value != DBNull.Value)
                    settingsData.msUrl = (string)command.Parameters["@ManagementSystemUrl"].Value;

                if (command.Parameters["@ServerAddress"].Value != DBNull.Value)
                    settingsData.serverAddress = (string)command.Parameters["@ServerAddress"].Value;

                if (command.Parameters["@SecretKey"].Value != DBNull.Value)
                    settingsData.secretKey = (string)command.Parameters["@SecretKey"].Value;

                if (command.Parameters["@AnyoneCanRegister"].Value != DBNull.Value)
                    settingsData.anyoneCanRegister = (string)command.Parameters["@AnyoneCanRegister"].Value;

                if (command.Parameters["@AssignProviderRole"].Value != DBNull.Value)
                    settingsData.assignProviderRole = (string)command.Parameters["@AssignProviderRole"].Value;

                if (command.Parameters["@UserPasswordMinLength"].Value != DBNull.Value)
                    settingsData.userPasswordMinLength = (string)command.Parameters["@UserPasswordMinLength"].Value;

                if (command.Parameters["@ServicePasswordLength"].Value != DBNull.Value)
                    settingsData.servicePasswordLength = (string)command.Parameters["@ServicePasswordLength"].Value;

                if (command.Parameters["@ClientPasswordLength"].Value != DBNull.Value)
                    settingsData.clientPasswordLength = (string)command.Parameters["@ClientPasswordLength"].Value;

                if (command.Parameters["@ClientKeyLength"].Value != DBNull.Value)
                    settingsData.clientKeyLength = (string)command.Parameters["@ClientKeyLength"].Value;

                if (command.Parameters["@EmailAddress"].Value != DBNull.Value)
                    settingsData.emailAddress = (string)command.Parameters["@EmailAddress"].Value;

                if (command.Parameters["@EmailPassword"].Value != DBNull.Value)
                    settingsData.emailPassword = (string)command.Parameters["@EmailPassword"].Value;

                if (command.Parameters["@SmtpServer"].Value != DBNull.Value)
                    settingsData.smtpServer = (string)command.Parameters["@SmtpServer"].Value;

                if (command.Parameters["@SmtpPort"].Value != DBNull.Value)
                    settingsData.smtpPort = (string)command.Parameters["@SmtpPort"].Value;

                if (command.Parameters["@SmtpRequiresSSL"].Value != DBNull.Value)
                    settingsData.smtpRequiresSSL = (string)command.Parameters["@SmtpRequiresSSL"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_setManagementSystemUrl(string value)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_SetManagementSystemUrl";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 1024);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Input;
                command.Parameters["@ParamValue"].Value = value;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_SetServerAddress(string value)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_SetServerAddress";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 1024);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Input;
                command.Parameters["@ParamValue"].Value = value;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_SetSecretKey(string value)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_SetSecretKey";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 1024);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Input;
                command.Parameters["@ParamValue"].Value = value;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_SetAnyoneCanRegister(string value)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_SetAnyoneCanRegister";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 1);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Input;
                command.Parameters["@ParamValue"].Value = value;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_SetAutoAssignProviderRole(string value)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_SetAutoAssignProviderRole";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 1);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Input;
                command.Parameters["@ParamValue"].Value = value;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_SetUserPasswordMinLength(string value)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_SetUserPasswordMinLength";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 2);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Input;
                command.Parameters["@ParamValue"].Value = value;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_SetServicePasswordLength(string value)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_SetServicePasswordLength";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 2);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Input;
                command.Parameters["@ParamValue"].Value = value;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_SetClientPasswordLength(string value)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_SetClientPasswordLength";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 2);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Input;
                command.Parameters["@ParamValue"].Value = value;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_SetClientKeyLength(string value)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_SetClientKeyLength";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 2);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Input;
                command.Parameters["@ParamValue"].Value = value;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void admin_SetEMail(string emailAddress, string emailPassword, string smtpServer, string smtpPort, string smtpRequiresSSL)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtAdmin_SetEMail";

                command.Parameters.Add("@EMailAddress", SqlDbType.NVarChar, 1024);
                command.Parameters["@EMailAddress"].Direction = ParameterDirection.Input;
                command.Parameters["@EMailAddress"].Value = emailAddress;

                command.Parameters.Add("@EMailPassword", SqlDbType.NVarChar, 64);
                command.Parameters["@EMailPassword"].Direction = ParameterDirection.Input;
                command.Parameters["@EMailPassword"].Value = emailPassword;

                command.Parameters.Add("@SmtpServer", SqlDbType.NVarChar, 64);
                command.Parameters["@SmtpServer"].Direction = ParameterDirection.Input;
                command.Parameters["@SmtpServer"].Value = smtpServer;

                command.Parameters.Add("@SmtpPort", SqlDbType.NVarChar, 5);
                command.Parameters["@SmtpPort"].Direction = ParameterDirection.Input;
                command.Parameters["@SmtpPort"].Value = smtpPort;

                command.Parameters.Add("@SmtpRequiresSSL", SqlDbType.NVarChar, 1);
                command.Parameters["@SmtpRequiresSSL"].Direction = ParameterDirection.Input;
                command.Parameters["@SmtpRequiresSSL"].Value = smtpRequiresSSL;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static string settings_getServerAddress()
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtSettings_GetServerAddress";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 1024);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();
                if (command.Parameters["@ParamValue"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The softnet server address is not specified in the general settings.");
                string paramValue = (string)command.Parameters["@ParamValue"].Value;   
                if(string.IsNullOrWhiteSpace(paramValue))
                    throw new GeneralSettingsSoftnetException("The softnet server address is not specified in the general settings.");
                return paramValue;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static string settings_getManagementSystemUrl()
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtSettings_GetManagementSystemUrl";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 1024);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();
                if (command.Parameters["@ParamValue"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The Management System URL is not specified in the general settings.");
                string paramValue = (string)command.Parameters["@ParamValue"].Value;
                if (string.IsNullOrWhiteSpace(paramValue))
                    throw new GeneralSettingsSoftnetException("The Management System URL is not specified in the general settings.");
                return paramValue;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static string settings_getSecretKey()
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtSettings_GetSecretKey";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 64);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();
                if (command.Parameters["@ParamValue"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The secret key is not specified in the general settings.");
                string paramValue = (string)command.Parameters["@ParamValue"].Value;
                if (string.IsNullOrWhiteSpace(paramValue))
                    throw new GeneralSettingsSoftnetException("The secret key is not specified in the general settings.");
                return paramValue;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static int settings_getUserPasswordMinLength()
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtSettings_GetUserPasswordMinLength";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 2);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();
                if (command.Parameters["@ParamValue"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The user password min length is not specified in the general settings.");
                string paramValue = (string)command.Parameters["@ParamValue"].Value;
                try
                {
                    int passwordLength = int.Parse(paramValue);
                    if (passwordLength < 4 || passwordLength > 32)
                        throw new GeneralSettingsSoftnetException("The value of the user password min length specified in the general settings is illegal.");
                    return passwordLength;
                }
                catch (FormatException)
                {
                    throw new GeneralSettingsSoftnetException("The value of the user password min length specified in the general settings is illegal.");
                }
                catch (OverflowException)
                {
                    throw new GeneralSettingsSoftnetException("The value of the user password min length specified in the general settings is illegal.");
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static int settings_getServicePasswordLength()
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtSettings_GetServicePasswordLength";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 2);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();
                if (command.Parameters["@ParamValue"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The service password length is not specified in the general settings.");
                string paramValue = (string)command.Parameters["@ParamValue"].Value;
                try
                {
                    int passwordLength = int.Parse(paramValue);
                    if (passwordLength < 4 || passwordLength > 32)
                        throw new GeneralSettingsSoftnetException("The value of the service password length specified in the general settings is illegal.");
                    return passwordLength;
                }
                catch (FormatException)
                {
                    throw new GeneralSettingsSoftnetException("The value of the service password length specified in the general settings is illegal.");
                }
                catch (OverflowException)
                {
                    throw new GeneralSettingsSoftnetException("The value of the service password length specified in the general settings is illegal.");
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static int settings_getClientPasswordLength()
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtSettings_GetClientPasswordLength";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 2);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();
                if (command.Parameters["@ParamValue"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The client password length is not specified in the general settings.");
                string paramValue = (string)command.Parameters["@ParamValue"].Value;
                try
                {
                    int passwordLength = int.Parse(paramValue);
                    if (passwordLength < 4 || passwordLength > 32)
                        throw new GeneralSettingsSoftnetException("The value of the client password length specified in the general settings is illegal.");
                    return passwordLength;
                }
                catch (Exception)
                {
                    throw new GeneralSettingsSoftnetException("The value of the client password length specified in the general settings is illegal.");
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static int settings_getClientKeyLength()
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtSettings_GetClientKeyLength";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 2);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();
                if (command.Parameters["@ParamValue"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The client key length is not specified in the general settings.");
                string paramValue = (string)command.Parameters["@ParamValue"].Value;
                try
                {
                    int keyLength = int.Parse(paramValue);
                    if (keyLength < 4 || keyLength > 32)
                        throw new GeneralSettingsSoftnetException("The value of the client key length specified in the general settings is illegal.");
                    return keyLength;
                }
                catch (Exception)
                {
                    throw new GeneralSettingsSoftnetException("The value of the client key length specified in the general settings is illegal.");
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static bool settings_getAnyoneCanRegister()
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtSettings_getAnyoneCanRegister";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 1);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();
                if (command.Parameters["@ParamValue"].Value == DBNull.Value)
                    return false;
                string paramValue = (string)command.Parameters["@ParamValue"].Value;
                if (paramValue.Equals("1"))
                    return true;
                return false;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static bool settings_getAutoAssignProviderRole()
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtSettings_getAutoAssignProviderRole";

                command.Parameters.Add("@ParamValue", SqlDbType.NVarChar, 2);
                command.Parameters["@ParamValue"].Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();
                if (command.Parameters["@ParamValue"].Value == DBNull.Value)
                    return false;
                string paramValue = (string)command.Parameters["@ParamValue"].Value;
                if (paramValue.Equals("1"))
                    return true;
                return false;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void settings_getMailingData(MailingData data)
    {        
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtSettings_GetMailingData";

                command.Parameters.Add("@ManagementSystemUrl", SqlDbType.NVarChar, 1024);
                command.Parameters["@ManagementSystemUrl"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@SecretKey", SqlDbType.NVarChar, 64);
                command.Parameters["@SecretKey"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@CurrentTime", SqlDbType.BigInt);
                command.Parameters["@CurrentTime"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@EMailAddress", SqlDbType.NVarChar, 1024);
                command.Parameters["@EMailAddress"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@EMailPassword", SqlDbType.NVarChar, 64);
                command.Parameters["@EMailPassword"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@SmtpServer", SqlDbType.NVarChar, 64);
                command.Parameters["@SmtpServer"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@SmtpPort", SqlDbType.NVarChar, 5);
                command.Parameters["@SmtpPort"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@SmtpRequiresSSL", SqlDbType.NVarChar, 1);
                command.Parameters["@SmtpRequiresSSL"].Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();

                if (command.Parameters["@ManagementSystemUrl"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The Management System Url is not specified in the general settings.");
                data.msUrl = (string)command.Parameters["@ManagementSystemUrl"].Value;
                Uri siteUri = new Uri(data.msUrl);
                data.siteAddress = siteUri.Host;

                if (command.Parameters["@SecretKey"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The secret key is not specified in the general settings.");
                data.secretKey = (string)command.Parameters["@SecretKey"].Value;

                data.currentTime = (long)command.Parameters["@CurrentTime"].Value;

                if (command.Parameters["@EMailAddress"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The email address is not specified in the general settings.");
                data.emailAddress = (string)command.Parameters["@EMailAddress"].Value;

                if (command.Parameters["@EMailPassword"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The email password is not specified in the general settings.");
                data.emailPassword = (string)command.Parameters["@EMailPassword"].Value;

                if (command.Parameters["@SmtpServer"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The SMTP server address is not specified in the general settings.");
                data.smtpServer = (string)command.Parameters["@SmtpServer"].Value;

                if (command.Parameters["@SmtpPort"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The SMTP port is not specified in the general settings.");
                string paramValue = (string)command.Parameters["@SmtpPort"].Value;
                try
                {
                    data.smtpPort = int.Parse(paramValue);
                }
                catch (Exception)
                {
                    throw new GeneralSettingsSoftnetException("The SMTP port is not specified in the general settings.");
                }

                if (command.Parameters["@SmtpRequiresSSL"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The parameter 'SMTP requires SSL' is not specified in the general settings.");
                paramValue = (string)command.Parameters["@SmtpRequiresSSL"].Value;
                if (paramValue.Equals("1"))
                    data.smtpRequiresSSL = true;
                else if (paramValue.Equals("0"))
                    data.smtpRequiresSSL = false;
                else
                    throw new GeneralSettingsSoftnetException("The parameter 'SMTP requires SSL' is not specified in the general settings.");
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void settings_getSecretKeyAndTime(Pair<string, long> data)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtSettings_GetSecretKeyAndTime";

                command.Parameters.Add("@SecretKey", SqlDbType.NVarChar, 64);
                command.Parameters["@SecretKey"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@CurrentTime", SqlDbType.BigInt);
                command.Parameters["@CurrentTime"].Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();

                if (command.Parameters["@SecretKey"].Value == DBNull.Value)
                    throw new GeneralSettingsSoftnetException("The secret key is not specified in the general settings.");
                data.First = (string)command.Parameters["@SecretKey"].Value;
                if (string.IsNullOrWhiteSpace(data.First))
                    throw new GeneralSettingsSoftnetException("The secret key is not specified in the general settings.");

                data.Second = (long)command.Parameters["@CurrentTime"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static long getTimeTicks()
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand Command = new SqlCommand();
                Command.Connection = Connection;
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "Softnet_Clock_GetTicks";

                Command.Parameters.Add("@Ticks", SqlDbType.BigInt);
                Command.Parameters["@Ticks"].Direction = ParameterDirection.Output;

                Command.ExecuteNonQuery();
                return (long)Command.Parameters["@Ticks"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static bool isEmailInUse(string email)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_IsEmailInUse";

                command.Parameters.Add("@EMail", SqlDbType.NVarChar, 256);
                command.Parameters["@EMail"].Direction = ParameterDirection.Input;
                command.Parameters["@EMail"].Value = email;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resutCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resutCode == 1)
                    return true;
                return false;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static string getOwnerName(string accountName)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetOwnerName";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@OwnerName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode == -1)
                    throw new ArgumentSoftnetException(string.Format("The account '{0}' not found.", accountName));
                return (string)command.Parameters["@OwnerName"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetSharedDomainDataset(string accountName, SharedDomainDataset dataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetSharedDomainDataset";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@ContactId", SqlDbType.BigInt);
                command.Parameters["@ContactId"].Direction = ParameterDirection.Input;
                command.Parameters["@ContactId"].Value = dataset.contactId;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainId"].Value = dataset.domainId;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ContactStatus", SqlDbType.Int);
                command.Parameters["@ContactStatus"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ContactName", SqlDbType.NVarChar, 256);
                command.Parameters["@ContactName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@PartnerAuthority", SqlDbType.Int);
                command.Parameters["@PartnerAuthority"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        List<SiteData> sites = new List<SiteData>();
                        while (dataReader.Read())
                        {
                            SiteData sData = new SiteData();
                            sData.siteId = (long)dataReader[0];
                            sData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                sData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                sData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                sData.ssHash = (string)dataReader[4];
                            sData.guestSupported = (bool)dataReader[5];
                            sData.guestAllowed = (bool)dataReader[6];
                            sData.statelessGuestSupported = (bool)dataReader[7];
                            if (dataReader[8] != DBNull.Value)
                                sData.siteKey = (string)dataReader[8];
                            sData.rolesSupported = (bool)dataReader[9];
                            if (dataReader[10] != DBNull.Value)
                                sData.defaultRoleId = (long)dataReader[10];
                            sData.implicitUsersAllowed = (bool)dataReader[11];
                            sData.structured = (bool)dataReader[12];
                            sData.enabled = (bool)dataReader[13];
                            if (dataReader[14] != DBNull.Value)
                                sData.description = (string)dataReader[14];

                            if (sData.guestSupported == false)
                                sData.guestAllowed = false;
                            sites.Add(sData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        while (dataReader.Read())
                        {
                            ServiceData sData = new ServiceData();
                            sData.serviceId = (long)dataReader[0];
                            sData.siteId = (long)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                sData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                sData.contractAuthor = (string)dataReader[3];
                            sData.version = (string)dataReader[4];
                            sData.hostname = (string)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                sData.ssHash = (string)dataReader[6];
                            sData.enabled = (bool)dataReader[7];
                            sData.pingPeriod = (int)dataReader[8];
                            services.Add(sData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<UserData> users = new List<UserData>();
                        while (dataReader.Read())
                        {
                            UserData uData = new UserData();
                            uData.userId = (long)dataReader[0];
                            uData.name = (string)dataReader[1];
                            uData.kind = (int)dataReader[2];
                            uData.dedicated = (bool)dataReader[3];
                            uData.enabled = (bool)dataReader[4];
                            users.Add(uData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<RoleData> roles = new List<RoleData>();
                        while (dataReader.Read())
                        {
                            RoleData rData = new RoleData();
                            rData.roleId = (long)dataReader[0];
                            rData.name = (string)dataReader[1];
                            rData.siteId = (long)dataReader[2];
                            rData.index = (int)dataReader[3];
                            roles.Add(rData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<UserInRole> usersInRoles = new List<UserInRole>();
                        while (dataReader.Read())
                        {
                            UserInRole ur = new UserInRole();
                            ur.userId = (long)dataReader[0];
                            ur.roleId = (long)dataReader[1];
                            usersInRoles.Add(ur);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<SiteUser> siteUsers = new List<SiteUser>();
                        while (dataReader.Read())
                        {
                            SiteUser su = new SiteUser();
                            su.userId = (long)dataReader[0];
                            su.siteId = (long)dataReader[1];
                            siteUsers.Add(su);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ClientData> clients = new List<ClientData>();
                        while (dataReader.Read())
                        {
                            ClientData clientData = new ClientData();
                            clientData.clientId = (long)dataReader[0];
                            clientData.siteId = (long)dataReader[1];
                            clientData.userId = (long)dataReader[2];
                            clientData.clientKey = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                clientData.serviceType = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                clientData.contractAuthor = (string)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                clientData.clientDescription = (string)dataReader[6];
                            clientData.pingPeriod = (int)dataReader[7];
                            clients.Add(clientData);
                        }

                        List<ClientData> guestClients = new List<ClientData>();
                        if (dataReader.NextResult())
                        {
                            while (dataReader.Read())
                            {
                                ClientData clientData = new ClientData();
                                clientData.clientId = (long)dataReader[0];
                                clientData.siteId = (long)dataReader[1];                                
                                clientData.clientKey = (string)dataReader[2];
                                if (dataReader[3] != DBNull.Value)
                                    clientData.serviceType = (string)dataReader[3];
                                if (dataReader[4] != DBNull.Value)
                                    clientData.contractAuthor = (string)dataReader[4];
                                if (dataReader[5] != DBNull.Value)
                                    clientData.clientDescription = (string)dataReader[5];
                                clientData.pingPeriod = (int)dataReader[6];
                                guestClients.Add(clientData);
                            }
                        }

                        dataset.sites = sites;
                        dataset.services = services;
                        dataset.users = users;
                        dataset.roles = roles;
                        dataset.usersInRoles = usersInRoles;
                        dataset.siteUsers = siteUsers;
                        dataset.clients = clients;
                        dataset.guestClients = guestClients;
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("Domain or contact not found.");
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    if (resultCode == -5)
                        throw new DataIntegritySoftnetException();
                    throw new DataDefinitionSoftnetException();
                }

                dataset.domainName = (string)command.Parameters["@DomainName"].Value;
                dataset.contactName = (string)command.Parameters["@ContactName"].Value;
                dataset.contactStatus = (int)command.Parameters["@ContactStatus"].Value;
                if (command.Parameters["@PartnerAuthority"].Value != DBNull.Value)
                    dataset.partnerAuthority = (int)command.Parameters["@PartnerAuthority"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetContactDomainDataset(string accountName, ContactDomainDataset dataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetContactDomainDataset";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@ContactId", SqlDbType.BigInt);
                command.Parameters["@ContactId"].Direction = ParameterDirection.Input;
                command.Parameters["@ContactId"].Value = dataset.contactId;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainId"].Value = dataset.domainId;

                command.Parameters.Add("@ConsumerId", SqlDbType.BigInt);
                command.Parameters["@ConsumerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ContactName", SqlDbType.NVarChar, 256);
                command.Parameters["@ContactName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {                        
                        List<SiteData> sites = new List<SiteData>();
                        while (dataReader.Read())
                        {
                            SiteData siteData = new SiteData();
                            siteData.siteId = (long)dataReader[0];
                            siteData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                siteData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                siteData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                siteData.ssHash = (string)dataReader[4];
                            siteData.guestSupported = (bool)dataReader[5];
                            siteData.guestAllowed = (bool)dataReader[6];
                            siteData.statelessGuestSupported = (bool)dataReader[7];
                            if (dataReader[8] != DBNull.Value)
                                siteData.siteKey = (string)dataReader[8];
                            siteData.rolesSupported = (bool)dataReader[9];
                            if (dataReader[10] != DBNull.Value)
                                siteData.defaultRoleId = (long)dataReader[10];
                            siteData.implicitUsersAllowed = (bool)dataReader[11];
                            siteData.structured = (bool)dataReader[12];
                            siteData.enabled = (bool)dataReader[13];
                            if (dataReader[14] != DBNull.Value)
                                siteData.description = (string)dataReader[14];

                            if (siteData.guestSupported == false)
                                siteData.guestAllowed = false;
                            sites.Add(siteData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        while (dataReader.Read())
                        {
                            ServiceData serviceData = new ServiceData();
                            serviceData.serviceId = (long)dataReader[0];
                            serviceData.siteId = (long)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                serviceData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                serviceData.contractAuthor = (string)dataReader[3];
                            serviceData.version = (string)dataReader[4];
                            serviceData.hostname = (string)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                serviceData.ssHash = (string)dataReader[6];
                            serviceData.enabled = (bool)dataReader[7];
                            serviceData.pingPeriod = (int)dataReader[8];
                            services.Add(serviceData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<UserData> users = new List<UserData>();
                        while (dataReader.Read())
                        {
                            UserData uData = new UserData();
                            uData.userId = (long)dataReader[0];
                            uData.name = (string)dataReader[1];
                            uData.kind = (int)dataReader[2];
                            uData.dedicated = (bool)dataReader[3];
                            uData.enabled = (bool)dataReader[4];
                            users.Add(uData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<RoleData> roles = new List<RoleData>();
                        while (dataReader.Read())
                        {
                            RoleData rData = new RoleData();
                            rData.roleId = (long)dataReader[0];
                            rData.name = (string)dataReader[1];
                            rData.siteId = (long)dataReader[2];
                            rData.index = (int)dataReader[3];
                            roles.Add(rData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<UserInRole> usersInRoles = new List<UserInRole>();
                        while (dataReader.Read())
                        {
                            UserInRole ur = new UserInRole();
                            ur.userId = (long)dataReader[0];
                            ur.roleId = (long)dataReader[1];
                            usersInRoles.Add(ur);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<SiteUser> siteUsers = new List<SiteUser>();
                        while (dataReader.Read())
                        {
                            SiteUser su = new SiteUser();
                            su.userId = (long)dataReader[0];
                            su.siteId = (long)dataReader[1];
                            siteUsers.Add(su);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ClientData> clients = new List<ClientData>();
                        while (dataReader.Read())
                        {
                            ClientData clientData = new ClientData();
                            clientData.clientId = (long)dataReader[0];
                            clientData.siteId = (long)dataReader[1];
                            clientData.userId = (long)dataReader[2];
                            clientData.clientKey = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                clientData.serviceType = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                clientData.contractAuthor = (string)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                clientData.clientDescription = (string)dataReader[6];
                            clientData.pingPeriod = (int)dataReader[7];
                            clients.Add(clientData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ClientData> guestClients = new List<ClientData>();
                        while (dataReader.Read())
                        {
                            ClientData clientData = new ClientData();
                            clientData.clientId = (long)dataReader[0];
                            clientData.siteId = (long)dataReader[1];
                            clientData.userId = (long)dataReader[2];
                            clientData.clientKey = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                clientData.serviceType = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                clientData.contractAuthor = (string)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                clientData.clientDescription = (string)dataReader[6];
                            clientData.pingPeriod = (int)dataReader[7];
                            guestClients.Add(clientData);
                        }

                        dataset.sites = sites;
                        dataset.services = services;
                        dataset.users = users;
                        dataset.roles = roles;
                        dataset.usersInRoles = usersInRoles;
                        dataset.siteUsers = siteUsers;
                        dataset.clients = clients;
                        dataset.guestClients = guestClients;
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;                
                if (resultCode != 0)
                {
                    if (resultCode == 1)
                        throw new InvalidStateSoftnetException("The contact is not authorized.");
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The contact or domain not found.");
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    throw new DataDefinitionSoftnetException();
                }

                dataset.consumerId = (long)command.Parameters["@ConsumerId"].Value;
                dataset.contactName = (string)command.Parameters["@ContactName"].Value;
                dataset.domainName = (string)command.Parameters["@DomainName"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetSharedDomainList(string accountName, long contactId, bool meProvider, SharedDomainList dataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetSharedDomainList";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@ContactId", SqlDbType.BigInt);
                command.Parameters["@ContactId"].Direction = ParameterDirection.Input;
                command.Parameters["@ContactId"].Value = contactId;

                command.Parameters.Add("@MeProvider", SqlDbType.Bit);
                command.Parameters["@MeProvider"].Direction = ParameterDirection.Input;
                command.Parameters["@MeProvider"].Value = meProvider;

                command.Parameters.Add("@ContactName", SqlDbType.NVarChar, 256);
                command.Parameters["@ContactName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ContactStatus", SqlDbType.Int);
                command.Parameters["@ContactStatus"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@PartnerAuthority", SqlDbType.Int);
                command.Parameters["@PartnerAuthority"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@PartnerEnabled", SqlDbType.Bit);
                command.Parameters["@PartnerEnabled"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                List<DomainItem> domainList1 = null;
                List<DomainItem> domainList2 = null;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        domainList1 = new List<DomainItem>();
                        while (dataReader.Read())
                        {
                            DomainItem domainItem = new DomainItem();
                            domainItem.domainId = (long)dataReader[0];
                            domainItem.domainName = (string)dataReader[1];
                            domainList1.Add(domainItem);
                        }

                        if (dataReader.NextResult())
                        {
                            domainList2 = new List<DomainItem>();
                            while (dataReader.Read())
                            {
                                DomainItem domainItem = new DomainItem();
                                domainItem.domainId = (long)dataReader[0];
                                domainItem.domainName = (string)dataReader[1];
                                domainList2.Add(domainItem);
                            }
                        }                        
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The contact not found.");
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException();
                    if (resultCode == -5)
                        throw new DataIntegritySoftnetException();
                    throw new DataDefinitionSoftnetException();
                }

                dataset.contactId = contactId;
                dataset.contactName = (string)command.Parameters["@ContactName"].Value;
                dataset.status = (int)command.Parameters["@ContactStatus"].Value;
                if (command.Parameters["@PartnerAuthority"].Value != DBNull.Value)
                    dataset.partnerAuthority = (int)command.Parameters["@PartnerAuthority"].Value;
                if (command.Parameters["@PartnerEnabled"].Value != DBNull.Value)
                    dataset.partnerEnabled = (bool)command.Parameters["@PartnerEnabled"].Value;

                if (meProvider)
                {
                    dataset.myDomains = domainList1;
                    dataset.contactDomains = domainList2;
                }
                else
                {
                    dataset.contactDomains = domainList1;
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetContactData(string accountName, long contactId, ContactData contactData, Atomic<string> partnerName)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetContactData";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@ContactId", SqlDbType.BigInt);
                command.Parameters["@ContactId"].Direction = ParameterDirection.Input;
                command.Parameters["@ContactId"].Value = contactId;

                command.Parameters.Add("@ContactName", SqlDbType.NVarChar, 256);
                command.Parameters["@ContactName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@UserDefaultName", SqlDbType.NVarChar, 256);
                command.Parameters["@UserDefaultName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@Status", SqlDbType.Int);
                command.Parameters["@Status"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@PartnerName", SqlDbType.NVarChar, 256);
                command.Parameters["@PartnerName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode < 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The contact has not been found.");
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    if (resultCode == -5)
                        throw new DataIntegritySoftnetException();
                    throw new DataDefinitionSoftnetException();
                }

                contactData.contactId = contactId;
                contactData.contactName = (string)command.Parameters["@ContactName"].Value;
                contactData.userDefaultName = (string)command.Parameters["@UserDefaultName"].Value;
                contactData.status = (int)command.Parameters["@Status"].Value;
                if (contactData.status != 2)
                    partnerName.Value = (string)command.Parameters["@PartnerName"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void UpdateContact(long contactId, string contactName, string userDefaultName)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_UpdateContact";

                command.Parameters.Add("@ContactId", SqlDbType.BigInt);
                command.Parameters["@ContactId"].Direction = ParameterDirection.Input;
                command.Parameters["@ContactId"].Value = contactId;

                command.Parameters.Add("@ContactName", SqlDbType.NVarChar, 256);
                command.Parameters["@ContactName"].Direction = ParameterDirection.Input;
                command.Parameters["@ContactName"].Value = contactName;

                command.Parameters.Add("@UserDefaultName", SqlDbType.NVarChar, 256);
                command.Parameters["@UserDefaultName"].Direction = ParameterDirection.Input;
                command.Parameters["@UserDefaultName"].Value = userDefaultName;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static bool WhetherPeerContactExists(long ownerId, long selectedUserId)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_WhetherPeerContactExists";

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Input;
                command.Parameters["@OwnerId"].Value = ownerId;

                command.Parameters.Add("@SelectedUserId", SqlDbType.BigInt);
                command.Parameters["@SelectedUserId"].Direction = ParameterDirection.Input;
                command.Parameters["@SelectedUserId"].Value = selectedUserId;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int result = (int)command.Parameters["@ReturnValue"].Value;
                if (result == 1)
                    return true;
                return false;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static int SendInvitation(long ownerId, long selectedId)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_SendInvitation";

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Input;
                command.Parameters["@OwnerId"].Value = ownerId;

                command.Parameters.Add("@SelectedId", SqlDbType.BigInt);
                command.Parameters["@SelectedId"].Direction = ParameterDirection.Input;
                command.Parameters["@SelectedId"].Value = selectedId;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int result = (int)command.Parameters["@ReturnValue"].Value;
                if (-1 <= result && result <= 1)
                    return result;
                if (result == -2)
                    throw new ArgumentSoftnetException("The account has not been found.");
                throw new DataDefinitionSoftnetException();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void DeleteSentInvitation(long selectedId)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_DeleteSentInvitation";

                command.Parameters.Add("@SelectedId", SqlDbType.BigInt);
                command.Parameters["@SelectedId"].Direction = ParameterDirection.Input;
                command.Parameters["@SelectedId"].Value = selectedId;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static int GetReceivedInvitationCount(string accountName)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetReceivedInvitationCount";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                return (int)command.Parameters["@ReturnValue"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetReceivedInvitations(string accountName, List<OwnerData> invitingUsers)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetReceivedInvitations";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount == 0)
                        throw new AccountNotFoundSoftnetException(accountName);

                    while (dataReader.Read())
                    {
                        OwnerData invitedUser = new OwnerData();
                        invitedUser.ownerId = (long)dataReader[0];
                        invitedUser.fullName = (string)dataReader[1];
                        invitingUsers.Add(invitedUser);
                    }
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void AcceptInvitation(string accountName, long invitingUserId)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_AcceptInvitation";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@InvitingUserId", SqlDbType.BigInt);
                command.Parameters["@InvitingUserId"].Direction = ParameterDirection.Input;
                command.Parameters["@InvitingUserId"].Value = invitingUserId;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The inviting user not found.");
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    throw new DataDefinitionSoftnetException();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void DeleteReceivedInvitation(string accountName, long invitingId)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_DeleteReceivedInvitation";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@InvitingId", SqlDbType.BigInt);
                command.Parameters["@InvitingId"].Direction = ParameterDirection.Input;
                command.Parameters["@InvitingId"].Value = invitingId;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int result = (int)command.Parameters["@ReturnValue"].Value;
                if (result == -2)
                    throw new AccountNotFoundSoftnetException(accountName);
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static long Contacts_FindUsers(string accountName, string filter, List<OwnerData> users)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtContacts_FindUsers";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@Filter", SqlDbType.NVarChar, 256);
                command.Parameters["@Filter"].Direction = ParameterDirection.Input;
                command.Parameters["@Filter"].Value = filter;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            OwnerData foundUser = new OwnerData();
                            foundUser.ownerId = (long)dataReader[0];
                            foundUser.authority = (int)dataReader[1];
                            foundUser.fullName = (string)dataReader[2];
                            users.Add(foundUser);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode == -2)
                    throw new ArgumentSoftnetException("The account has not been found.");

                return (long)command.Parameters["@OwnerId"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static long Contacts_FindNextUsers(string accountName, string filter, long lastSelectedId, List<OwnerData> users)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtContacts_FindNextUsers";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@Filter", SqlDbType.NVarChar, 256);
                command.Parameters["@Filter"].Direction = ParameterDirection.Input;
                command.Parameters["@Filter"].Value = filter;

                command.Parameters.Add("@LastSelectedId", SqlDbType.BigInt);
                command.Parameters["@LastSelectedId"].Direction = ParameterDirection.Input;
                command.Parameters["@LastSelectedId"].Value = lastSelectedId;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            OwnerData foundUser = new OwnerData();
                            foundUser.ownerId = (long)dataReader[0];
                            foundUser.authority = (int)dataReader[1];
                            foundUser.fullName = (string)dataReader[2];
                            users.Add(foundUser);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode == -2)
                    throw new ArgumentSoftnetException("The account has not been found.");

                return (long)command.Parameters["@OwnerId"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static long Contacts_FindPrevUsers(string accountName, string filter, long firstSelectedId, List<OwnerData> users)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_MgtContacts_FindPrevUsers";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@Filter", SqlDbType.NVarChar, 256);
                command.Parameters["@Filter"].Direction = ParameterDirection.Input;
                command.Parameters["@Filter"].Value = filter;

                command.Parameters.Add("@FirstSelectedId", SqlDbType.BigInt);
                command.Parameters["@FirstSelectedId"].Direction = ParameterDirection.Input;
                command.Parameters["@FirstSelectedId"].Value = firstSelectedId;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            OwnerData foundUser = new OwnerData();
                            foundUser.ownerId = (long)dataReader[0];
                            foundUser.authority = (int)dataReader[1];
                            foundUser.fullName = (string)dataReader[2];
                            users.Add(foundUser);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode == -2)
                    throw new ArgumentSoftnetException("The account has not been found.");

                return (long)command.Parameters["@OwnerId"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetContactListData(string accountName, ContactListData data)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetContactListData";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        List<ContactData> contacts = new List<ContactData>();
                        data.contacts = contacts;
                        while (dataReader.Read())
                        {
                            ContactData contactData = new ContactData();
                            contactData.contactId = (long)dataReader[0];
                            if (dataReader[1] != DBNull.Value)
                                contactData.consumerId = (long)dataReader[1];
                            contactData.contactName = (string)dataReader[2];
                            contactData.status = (int)dataReader[3];
                            contacts.Add(contactData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new SoftnetException(ErrorCodes.DATA_DEFINITION_ERROR);

                        List<OwnerData> partners = new List<OwnerData>();
                        data.partners = partners;
                        while (dataReader.Read())
                        {
                            OwnerData ownerData = new OwnerData();
                            ownerData.ownerId = (long)dataReader[0];
                            ownerData.authority = (int)dataReader[1];
                            ownerData.enabled = (bool)dataReader[2];
                            partners.Add(ownerData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<OwnerData> invitedUsers = new List<OwnerData>();
                        data.invitedUsers = invitedUsers;
                        while (dataReader.Read())
                        {
                            OwnerData invitedUser = new OwnerData();
                            invitedUser.ownerId = (long)dataReader[0];
                            invitedUser.fullName = (string)dataReader[1];
                            invitedUsers.Add(invitedUser);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }
                
                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException();                    
                    throw new DataDefinitionSoftnetException();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetContactListData2(string accountName, ContactListData data)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetContactListData2";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();                
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        List<ContactData> contacts = new List<ContactData>();
                        data.contacts = contacts;
                        while (dataReader.Read())
                        {
                            ContactData contactData = new ContactData();
                            contactData.contactId = (long)dataReader[0];
                            if (dataReader[1] != DBNull.Value)
                                contactData.consumerId = (long)dataReader[1];
                            contactData.contactName = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                contactData.userDefaultName = (string)dataReader[3];
                            contactData.status = (int)dataReader[4];
                            contacts.Add(contactData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new SoftnetException(ErrorCodes.DATA_DEFINITION_ERROR);

                        List<OwnerData> partners = new List<OwnerData>();
                        data.partners = partners;
                        while (dataReader.Read())
                        {
                            OwnerData ownerData = new OwnerData();
                            ownerData.ownerId = (long)dataReader[0];
                            ownerData.authority = (int)dataReader[1];
                            ownerData.enabled = (bool)dataReader[2];
                            ownerData.fullName = (string)dataReader[3];
                            partners.Add(ownerData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new SoftnetException(ErrorCodes.DATA_DEFINITION_ERROR);

                        List<OwnerData> invitedUsers = new List<OwnerData>();
                        data.invitedUsers = invitedUsers;
                        while (dataReader.Read())
                        {
                            OwnerData invitedUser = new OwnerData();
                            invitedUser.ownerId = (long)dataReader[0];
                            invitedUser.fullName = (string)dataReader[1];
                            invitedUsers.Add(invitedUser);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException();
                    throw new DataDefinitionSoftnetException();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetRecentlyUsedContacts(string accountName, List<ContactData> contacts, List<ContactData> invalidContacts, List<OwnerData> partners)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetRecentlyUsedContacts";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            ContactData contactData = new ContactData();
                            contactData.contactId = (long)dataReader[0];
                            if (dataReader[1] != DBNull.Value)
                                contactData.consumerId = (long)dataReader[1];
                            contactData.contactName = (string)dataReader[2];
                            contacts.Add(contactData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        while (dataReader.Read())
                        {
                            ContactData contactData = new ContactData();
                            contactData.contactId = (long)dataReader[0];
                            contactData.contactName = (string)dataReader[1];
                            contactData.status = (int)dataReader[2];
                            invalidContacts.Add(contactData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        while (dataReader.Read())
                        {
                            OwnerData ownerData = new OwnerData();
                            ownerData.ownerId = (long)dataReader[0];
                            ownerData.authority = (int)dataReader[1];
                            partners.Add(ownerData);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetRecentlyUsedDomains(string accountName, List<DomainItem> domains)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetRecentlyUsedDomains";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            DomainItem domainItem = new DomainItem();
                            domainItem.domainId = (long)dataReader[0];
                            domainItem.domainName = (string)dataReader[1];
                            domains.Add(domainItem);
                        }
                    }
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static long CreateDomain(string accountName, string domainName)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_CreateDomain";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainName"].Value = domainName;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                
                int errorCode = (int)command.Parameters["@ReturnValue"].Value;
                if (errorCode != 0)
                {
                    if (errorCode == 5)
                        throw new LimitReachedSoftnetException("The limit for the maximum number of domains has been reached.");
                    throw new OperationFailedSoftnetException();
                }

                return (long)command.Parameters["@DomainId"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetDomainList(string accountName, List<DomainItem> domains)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetDomainList";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        while (dataReader.Read())
                        {
                            DomainItem domainItem = new DomainItem();
                            domainItem.domainId = (long)dataReader[0];
                            domainItem.domainName = (string)dataReader[1];
                            domains.Add(domainItem);
                        }
                    }
                    else
                        throw new AccountNotFoundSoftnetException(accountName);
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetDomainItem(string accountName, long domainId, DomainItem domainItem)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetDomainItem";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainId"].Value = domainId;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The domain not found.");
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    throw new DataDefinitionSoftnetException();
                }

                domainItem.domainId = domainId;
                domainItem.domainName = (string)command.Parameters["@DomainName"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void ChangeDomainName(long domainId, string domainName)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_ChangeDomainName";

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainId"].Value = domainId;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainName"].Value = domainName;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetDomainDataset(string accountName, long domainId, DomainDataset domainDataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetDomainDataset";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainId"].Value = domainId;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        List<SiteData> sites = new List<SiteData>();
                        while (dataReader.Read())
                        {
                            SiteData sData = new SiteData();
                            sData.siteId = (long)dataReader[0];
                            sData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                sData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                sData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                sData.ssHash = (string)dataReader[4];
                            sData.guestSupported = (bool)dataReader[5];
                            sData.guestAllowed = (bool)dataReader[6];
                            sData.statelessGuestSupported = (bool)dataReader[7];
                            if (dataReader[8] != DBNull.Value)
                                sData.siteKey = (string)dataReader[8];
                            sData.rolesSupported = (bool)dataReader[9];
                            if (dataReader[10] != DBNull.Value)
                                sData.defaultRoleId = (long)dataReader[10];
                            sData.implicitUsersAllowed = (bool)dataReader[11];
                            sData.structured = (bool)dataReader[12];
                            sData.enabled = (bool)dataReader[13];
                            if (dataReader[14] != DBNull.Value)
                                sData.description = (string)dataReader[14];

                            if (sData.guestSupported == false)
                                sData.guestAllowed = false;

                            sites.Add(sData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        while (dataReader.Read())
                        {
                            ServiceData sData = new ServiceData();
                            sData.serviceId = (long)dataReader[0];
                            sData.siteId = (long)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                sData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                sData.contractAuthor = (string)dataReader[3];
                            sData.version = (string)dataReader[4];
                            sData.hostname = (string)dataReader[5];
                            if (dataReader[6] != DBNull.Value)
                                sData.ssHash = (string)dataReader[6];
                            sData.enabled = (bool)dataReader[7];
                            sData.pingPeriod = (int)dataReader[8];
                            services.Add(sData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<UserData> users = new List<UserData>();
                        while (dataReader.Read())
                        {
                            UserData uData = new UserData();
                            uData.userId = (long)dataReader[0];
                            uData.kind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                uData.contactId = (long)dataReader[2];
                            uData.name = (string)dataReader[3];
                            uData.dedicated = (bool)dataReader[4];
                            uData.enabled = (bool)dataReader[5];
                            users.Add(uData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ContactData> contacts = new List<ContactData>();
                        while (dataReader.Read())
                        {
                            ContactData cData = new ContactData();
                            cData.contactId = (long)dataReader[0];
                            cData.contactName = (string)dataReader[1];
                            cData.status = (int)dataReader[2];
                            contacts.Add(cData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<RoleData> roles = new List<RoleData>();
                        while (dataReader.Read())
                        {
                            RoleData rData = new RoleData();
                            rData.roleId = (long)dataReader[0];
                            rData.name = (string)dataReader[1];
                            rData.siteId = (long)dataReader[2];
                            rData.index = (int)dataReader[3];
                            roles.Add(rData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<UserInRole> usersInRoles = new List<UserInRole>();
                        while (dataReader.Read())
                        {
                            UserInRole ur = new UserInRole();
                            ur.userId = (long)dataReader[0];
                            ur.roleId = (long)dataReader[1];
                            usersInRoles.Add(ur);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<SiteUser> siteUsers = new List<SiteUser>();
                        while (dataReader.Read())
                        {
                            SiteUser su = new SiteUser();
                            su.userId = (long)dataReader[0];
                            su.siteId = (long)dataReader[1];
                            siteUsers.Add(su);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ClientCount> clientCounts = new List<ClientCount>();
                        while (dataReader.Read())
                        {
                            ClientCount cc = new ClientCount();
                            cc.userId = (long)dataReader[0];
                            cc.count = (int)dataReader[1];
                            clientCounts.Add(cc);
                        }

                        domainDataset.sites = sites;
                        domainDataset.services = services;
                        domainDataset.users = users;
                        domainDataset.contacts = contacts;
                        domainDataset.roles = roles;
                        domainDataset.usersInRoles = usersInRoles;
                        domainDataset.siteUsers = siteUsers;
                        domainDataset.clientCounts = clientCounts;
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The domain not found.");
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    throw new DataDefinitionSoftnetException();
                }

                domainDataset.domainId = domainId;
                domainDataset.domainName = (string)command.Parameters["@DomainName"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetSiteDeletingData(string accountName, long siteId, SiteDeletingData data)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetSiteDeletingData";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteId"].Value = siteId;

                command.Parameters.Add("@Description", SqlDbType.NVarChar, 256);
                command.Parameters["@Description"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ServiceCount", SqlDbType.Int);
                command.Parameters["@ServiceCount"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ClientCount", SqlDbType.Int);
                command.Parameters["@ClientCount"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The site not found.");
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    throw new DataDefinitionSoftnetException();
                }

                data.siteId = siteId;
                if (command.Parameters["@Description"].Value != DBNull.Value)
                    data.description = (string)command.Parameters["@Description"].Value;
                data.domainId = (long)command.Parameters["@DomainId"].Value;
                if (command.Parameters["@DomainName"].Value != DBNull.Value)
                    data.domainName = (string)command.Parameters["@DomainName"].Value;
                data.serviceCount = (int)command.Parameters["@ServiceCount"].Value;
                data.clientCount = (int)command.Parameters["@ClientCount"].Value;                
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetUserCreatingDataset(string accountName, long domainId, UserCreatingDataset dataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetUserCreatingDataset";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainId"].Value = domainId;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        List<UserData> users = new List<UserData>();
                        while (dataReader.Read())
                        {
                            UserData uData = new UserData();
                            uData.userId = (long)dataReader[0];
                            uData.kind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                uData.contactId = (long)dataReader[2];
                            uData.name = (string)dataReader[3];
                            uData.dedicated = (bool)dataReader[4];
                            uData.enabled = (bool)dataReader[5];
                            users.Add(uData);
                        }

                        if (dataReader.NextResult() == false)                        
                            throw new DataDefinitionSoftnetException();                        

                        List<ContactData> contacts = new List<ContactData>();
                        while (dataReader.Read())
                        {
                            ContactData cd = new ContactData();
                            cd.contactId = (long)dataReader[0];
                            cd.contactName = (string)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                cd.userDefaultName = (string)dataReader[2];
                            cd.status = (int)dataReader[3];
                            contacts.Add(cd);
                        }
                        dataset.users = users;
                        dataset.contacts = contacts;
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The domain not found.");
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    throw new DataDefinitionSoftnetException();
                }

                dataset.domainId = domainId;
                dataset.domainName = (string)command.Parameters["@DomainName"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static int GetSiteAccessType(long siteId)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetSiteAccessType";

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteId"].Value = siteId;

                command.Parameters.Add("@AccessType", SqlDbType.Int);
                command.Parameters["@AccessType"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                int errorCode = (int)command.Parameters["@ReturnValue"].Value;
                if (errorCode == -1)
                    throw new ArgumentSoftnetException("The site not found.");

                return (int)command.Parameters["@AccessType"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static long CreateSite(long domainId, string description)
    {
        int clientKeyLength = SoftnetRegistry.settings_getClientKeyLength();
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_SiteKeyExists";

                command.Parameters.Add("@SiteKey", SqlDbType.VarChar, 32);
                command.Parameters["@SiteKey"].Direction = ParameterDirection.Input;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;                

                string siteKey = null;
                for (int keyLength = clientKeyLength; keyLength <= 32; keyLength++)
                {
                    siteKey = Randomizer.generateClientKey(keyLength);
                    command.Parameters["@SiteKey"].Value = siteKey;
                    command.ExecuteNonQuery();
                    if (((int)command.Parameters["@ReturnValue"].Value) == 0)
                        break;
                    if (keyLength == 32)
                        throw new OperationFailedSoftnetException("Failed to create a client key");
                }

                command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_CreateSite";

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Input;
                command.Parameters["@DomainId"].Value = domainId;

                command.Parameters.Add("@SiteKey", SqlDbType.VarChar, 32);
                command.Parameters["@SiteKey"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteKey"].Value = siteKey;

                command.Parameters.Add("@Description", SqlDbType.NVarChar, 256);
                command.Parameters["@Description"].Direction = ParameterDirection.Input;
                command.Parameters["@Description"].Value = description;

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;                

                command.ExecuteNonQuery();

                int returnCode = (int)command.Parameters["@ReturnValue"].Value;
                if (returnCode != 0)
                {
                    if (returnCode == 5)
                        throw new LimitReachedSoftnetException("The limit for the maximum number of sites has been reached.");
                    throw new OperationFailedSoftnetException("Failed to create a site");
                }

                return (long)command.Parameters["@SiteId"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetRSiteConfigDataset(string accountName, long siteId, SiteConfigDataset siteDataset)
    { 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetRSiteConfigDataset";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteId"].Value = siteId;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        if (dataReader.Read())
                        {
                            SiteData sData = new SiteData();
                            sData.siteId = (long)dataReader[0];
                            sData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                sData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                sData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                sData.ssHash = (string)dataReader[4];
                            sData.guestSupported = (bool)dataReader[5];
                            sData.guestAllowed = (bool)dataReader[6];
                            sData.statelessGuestSupported = (bool)dataReader[7];
                            if (dataReader[8] != DBNull.Value)
                                sData.siteKey = (string)dataReader[8];
                            sData.rolesSupported = (bool)dataReader[9];
                            if (dataReader[10] != DBNull.Value)
                                sData.defaultRoleId = (long)dataReader[10];
                            sData.implicitUsersAllowed = (bool)dataReader[11];
                            sData.structured = (bool)dataReader[12];
                            sData.enabled = (bool)dataReader[13];
                            if (dataReader[14] != DBNull.Value)
                                sData.description = (string)dataReader[14];
                            siteDataset.siteData = sData;
                        }
                        else
                            throw new ArgumentSoftnetException("The site not found.");

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        while (dataReader.Read())
                        {
                            ServiceData sData = new ServiceData();
                            sData.serviceUid = (Guid)dataReader[0];
                            sData.serviceId = (long)dataReader[1];
                            sData.siteId = (long)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                sData.serviceType = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                sData.contractAuthor = (string)dataReader[4];
                            sData.version = (string)dataReader[5];
                            sData.hostname = (string)dataReader[6];
                            if (dataReader[7] != DBNull.Value)
                                sData.ssHash = (string)dataReader[7];
                            sData.enabled = (bool)dataReader[8];
                            sData.pingPeriod = (int)dataReader[9];
                            services.Add(sData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<UserData> users = new List<UserData>();
                        while (dataReader.Read())
                        {
                            UserData uData = new UserData();
                            uData.userId = (long)dataReader[0];
                            uData.kind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                uData.contactId = (long)dataReader[2];
                            uData.name = (string)dataReader[3];
                            uData.dedicated = (bool)dataReader[4];
                            uData.enabled = (bool)dataReader[5];
                            users.Add(uData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ContactData> contacts = new List<ContactData>();
                        while (dataReader.Read())
                        {
                            ContactData cData = new ContactData();
                            cData.contactId = (long)dataReader[0];
                            cData.contactName = (string)dataReader[1];
                            cData.status = (int)dataReader[2];
                            contacts.Add(cData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<RoleData> roles = new List<RoleData>();
                        while (dataReader.Read())
                        {
                            RoleData rData = new RoleData();
                            rData.roleId = (long)dataReader[0];
                            rData.name = (string)dataReader[1];
                            rData.siteId = (long)dataReader[2];
                            rData.index = (int)dataReader[3];
                            roles.Add(rData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<UserInRole> usersInRoles = new List<UserInRole>();
                        while (dataReader.Read())
                        {
                            UserInRole ur = new UserInRole();
                            ur.userId = (long)dataReader[0];
                            ur.roleId = (long)dataReader[1];
                            usersInRoles.Add(ur);
                        }

                        siteDataset.services = services;
                        siteDataset.users = users;
                        siteDataset.contacts = contacts;
                        siteDataset.roles = roles;
                        siteDataset.usersInRoles = usersInRoles;
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode == -1)
                    throw new ArgumentSoftnetException("The site not found.");

                siteDataset.siteId = siteId;
                siteDataset.domainId = (long)command.Parameters["@DomainId"].Value;
                siteDataset.domainName = (string)command.Parameters["@DomainName"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetUSiteConfigDataset(string accountName, long siteId, SiteConfigDataset siteDataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetUSiteConfigDataset";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteId"].Value = siteId;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        if (dataReader.Read())
                        {
                            SiteData sData = new SiteData();
                            sData.siteId = (long)dataReader[0];
                            sData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                sData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                sData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                sData.ssHash = (string)dataReader[4];
                            sData.guestSupported = (bool)dataReader[5];
                            sData.guestAllowed = (bool)dataReader[6];
                            sData.statelessGuestSupported = (bool)dataReader[7];
                            if (dataReader[8] != DBNull.Value)
                                sData.siteKey = (string)dataReader[8];
                            sData.rolesSupported = (bool)dataReader[9];
                            if (dataReader[10] != DBNull.Value)
                                sData.defaultRoleId = (long)dataReader[10];
                            sData.implicitUsersAllowed = (bool)dataReader[11];
                            sData.structured = (bool)dataReader[12];
                            sData.enabled = (bool)dataReader[13];
                            if (dataReader[14] != DBNull.Value)
                                sData.description = (string)dataReader[14];
                            siteDataset.siteData = sData;
                        }
                        else
                            throw new ArgumentSoftnetException("The site not found");

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        while (dataReader.Read())
                        {
                            ServiceData sData = new ServiceData();
                            sData.serviceUid = (Guid)dataReader[0];
                            sData.serviceId = (long)dataReader[1];
                            sData.siteId = (long)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                sData.serviceType = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                sData.contractAuthor = (string)dataReader[4];
                            sData.version = (string)dataReader[5];
                            sData.hostname = (string)dataReader[6];
                            if (dataReader[7] != DBNull.Value)
                                sData.ssHash = (string)dataReader[7];
                            sData.enabled = (bool)dataReader[8];
                            sData.pingPeriod = (int)dataReader[9];
                            services.Add(sData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<UserData> users = new List<UserData>();
                        while (dataReader.Read())
                        {
                            UserData uData = new UserData();
                            uData.userId = (long)dataReader[0];
                            uData.kind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                uData.contactId = (long)dataReader[2];
                            uData.name = (string)dataReader[3];
                            uData.dedicated = (bool)dataReader[4];
                            uData.enabled = (bool)dataReader[5];
                            users.Add(uData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ContactData> contacts = new List<ContactData>();
                        while (dataReader.Read())
                        {
                            ContactData cData = new ContactData();
                            cData.contactId = (long)dataReader[0];
                            cData.contactName = (string)dataReader[1];
                            cData.status = (int)dataReader[2];
                            contacts.Add(cData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<long> siteUsers = new List<long>();
                        while (dataReader.Read())
                        {
                            long userId = (long)dataReader[0];
                            siteUsers.Add(userId);
                        }

                        siteDataset.services = services;
                        siteDataset.users = users;
                        siteDataset.contacts = contacts;
                        siteDataset.siteUsers = siteUsers;
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode == -1)
                    throw new ArgumentSoftnetException("The site not found.");

                siteDataset.siteId = siteId;
                siteDataset.domainId = (long)command.Parameters["@DomainId"].Value;
                siteDataset.domainName = (string)command.Parameters["@DomainName"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetRSiteClientsDataset(string accountName, long siteId, SiteClientsDataset siteDataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetRSiteClientsDataset";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteId"].Value = siteId;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@GClientCount", SqlDbType.Int);
                command.Parameters["@GClientCount"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        SiteData siteData = new SiteData();
                        if (dataReader.Read())
                        {
                            siteData.siteId = (long)dataReader[0];
                            siteData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                siteData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                siteData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                siteData.ssHash = (string)dataReader[4];
                            siteData.guestSupported = (bool)dataReader[5];
                            siteData.guestAllowed = (bool)dataReader[6];
                            siteData.statelessGuestSupported = (bool)dataReader[7];
                            if (dataReader[8] != DBNull.Value)
                                siteData.siteKey = (string)dataReader[8];
                            siteData.rolesSupported = (bool)dataReader[9];
                            if (dataReader[10] != DBNull.Value)
                                siteData.defaultRoleId = (long)dataReader[10];
                            siteData.implicitUsersAllowed = (bool)dataReader[11];
                            siteData.structured = (bool)dataReader[12];
                            siteData.enabled = (bool)dataReader[13];
                            if (dataReader[14] != DBNull.Value)
                                siteData.description = (string)dataReader[14];                            
                        }
                        else                        
                            throw new ArgumentSoftnetException("The site not found.");

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        while (dataReader.Read())
                        {
                            ServiceData sData = new ServiceData();
                            sData.serviceUid = (Guid)dataReader[0];
                            sData.serviceId = (long)dataReader[1];
                            sData.siteId = (long)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                sData.serviceType = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                sData.contractAuthor = (string)dataReader[4];
                            sData.version = (string)dataReader[5];
                            sData.hostname = (string)dataReader[6];
                            if (dataReader[7] != DBNull.Value)
                                sData.ssHash = (string)dataReader[7];
                            sData.enabled = (bool)dataReader[8];
                            sData.pingPeriod = (int)dataReader[9];
                            services.Add(sData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<UserData> users = new List<UserData>();
                        while (dataReader.Read())
                        {
                            UserData uData = new UserData();
                            uData.userId = (long)dataReader[0];
                            uData.kind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                uData.contactId = (long)dataReader[2];
                            uData.name = (string)dataReader[3];
                            uData.dedicated = (bool)dataReader[4];
                            uData.enabled = (bool)dataReader[5];
                            users.Add(uData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ContactData> contacts = new List<ContactData>();
                        while (dataReader.Read())
                        {
                            ContactData cData = new ContactData();
                            cData.contactId = (long)dataReader[0];
                            cData.contactName = (string)dataReader[1];
                            cData.status = (int)dataReader[2];
                            contacts.Add(cData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<RoleData> roles = new List<RoleData>();
                        while (dataReader.Read())
                        {
                            RoleData rData = new RoleData();
                            rData.roleId = (long)dataReader[0];
                            rData.name = (string)dataReader[1];
                            rData.siteId = (long)dataReader[2];
                            rData.index = (int)dataReader[3];
                            roles.Add(rData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<UserInRole> usersInRoles = new List<UserInRole>();
                        while (dataReader.Read())
                        {
                            UserInRole ur = new UserInRole();
                            ur.userId = (long)dataReader[0];
                            ur.roleId = (long)dataReader[1];
                            usersInRoles.Add(ur);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ClientData> clients = new List<ClientData>();
                        while (dataReader.Read())
                        {
                            ClientData clientData = new ClientData();
                            clientData.clientId = (long)dataReader[0];
                            clientData.clientKey = (string)dataReader[1];
                            clientData.userId = (long)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                clientData.serviceType = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                clientData.contractAuthor = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                clientData.clientDescription = (string)dataReader[5];
                            clientData.pingPeriod = (int)dataReader[6];
                            clients.Add(clientData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ClientData> guestClients = new List<ClientData>();
                        while (dataReader.Read())
                        {
                            ClientData clientData = new ClientData();
                            clientData.clientId = (long)dataReader[0];
                            clientData.clientKey = (string)dataReader[1];
                            clientData.userId = (long)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                clientData.serviceType = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                clientData.contractAuthor = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                clientData.clientDescription = (string)dataReader[5];
                            clientData.pingPeriod = (int)dataReader[6];
                            guestClients.Add(clientData);
                        }

                        siteDataset.siteData = siteData;
                        siteDataset.services = services;
                        siteDataset.users = users;
                        siteDataset.contacts = contacts;
                        siteDataset.roles = roles;
                        siteDataset.usersInRoles = usersInRoles;
                        siteDataset.clients = clients;
                        siteDataset.guestClients = guestClients;
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode == -1)
                    throw new ArgumentSoftnetException("The site not found.");

                siteDataset.siteId = siteId;
                siteDataset.ownerId = (long)command.Parameters["@OwnerId"].Value;
                siteDataset.domainId = (long)command.Parameters["@DomainId"].Value;
                siteDataset.domainName = (string)command.Parameters["@DomainName"].Value;
                siteDataset.gclientCount = (int)command.Parameters["@GClientCount"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetUSiteClientsDataset(string accountName, long siteId, SiteClientsDataset siteDataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetUSiteClientsDataset";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteId"].Value = siteId;

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@GClientCount", SqlDbType.Int);
                command.Parameters["@GClientCount"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        SiteData siteData = new SiteData();
                        if (dataReader.Read())
                        {
                            siteData.siteId = (long)dataReader[0];
                            siteData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                siteData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                siteData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                siteData.ssHash = (string)dataReader[4];
                            siteData.guestSupported = (bool)dataReader[5];
                            siteData.guestAllowed = (bool)dataReader[6];
                            siteData.statelessGuestSupported = (bool)dataReader[7];
                            if (dataReader[8] != DBNull.Value)
                                siteData.siteKey = (string)dataReader[8];
                            siteData.rolesSupported = (bool)dataReader[9];
                            if (dataReader[10] != DBNull.Value)
                                siteData.defaultRoleId = (long)dataReader[10];
                            siteData.implicitUsersAllowed = (bool)dataReader[11];
                            siteData.structured = (bool)dataReader[12];
                            siteData.enabled = (bool)dataReader[13];
                            if (dataReader[14] != DBNull.Value)
                                siteData.description = (string)dataReader[14];
                        }
                        else
                            throw new ArgumentSoftnetException("The site not found.");

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        while (dataReader.Read())
                        {
                            ServiceData sData = new ServiceData();
                            sData.serviceUid = (Guid)dataReader[0];
                            sData.serviceId = (long)dataReader[1];
                            sData.siteId = (long)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                sData.serviceType = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                sData.contractAuthor = (string)dataReader[4];
                            sData.version = (string)dataReader[5];
                            sData.hostname = (string)dataReader[6];
                            if (dataReader[7] != DBNull.Value)
                                sData.ssHash = (string)dataReader[7];
                            sData.enabled = (bool)dataReader[8];
                            sData.pingPeriod = (int)dataReader[9];
                            services.Add(sData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<UserData> users = new List<UserData>();
                        while (dataReader.Read())
                        {
                            UserData uData = new UserData();
                            uData.userId = (long)dataReader[0];
                            uData.kind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                uData.contactId = (long)dataReader[2];
                            uData.name = (string)dataReader[3];
                            uData.dedicated = (bool)dataReader[4];
                            uData.enabled = (bool)dataReader[5];
                            users.Add(uData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ContactData> contacts = new List<ContactData>();
                        while (dataReader.Read())
                        {
                            ContactData cData = new ContactData();
                            cData.contactId = (long)dataReader[0];
                            cData.contactName = (string)dataReader[1];
                            cData.status = (int)dataReader[2];
                            contacts.Add(cData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<long> siteUsers = new List<long>();
                        while (dataReader.Read())
                        {
                            long userId = (long)dataReader[0];
                            siteUsers.Add(userId);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ClientData> clients = new List<ClientData>();
                        while (dataReader.Read())
                        {
                            ClientData clientData = new ClientData();
                            clientData.clientId = (long)dataReader[0];
                            clientData.clientKey = (string)dataReader[1];
                            clientData.userId = (long)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                clientData.serviceType = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                clientData.contractAuthor = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                clientData.clientDescription = (string)dataReader[5];
                            clientData.pingPeriod = (int)dataReader[6];
                            clients.Add(clientData);
                        }

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ClientData> guestClients = new List<ClientData>();
                        while (dataReader.Read())
                        {
                            ClientData clientData = new ClientData();
                            clientData.clientId = (long)dataReader[0];
                            clientData.clientKey = (string)dataReader[1];
                            clientData.userId = (long)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                clientData.serviceType = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                clientData.contractAuthor = (string)dataReader[4];
                            if (dataReader[5] != DBNull.Value)
                                clientData.clientDescription = (string)dataReader[5];
                            clientData.pingPeriod = (int)dataReader[6];
                            guestClients.Add(clientData);
                        }

                        siteDataset.siteData = siteData;
                        siteDataset.services = services;
                        siteDataset.users = users;
                        siteDataset.contacts = contacts;
                        siteDataset.siteUsers = siteUsers;
                        siteDataset.clients = clients;
                        siteDataset.guestClients = guestClients;
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode == -1)
                    throw new ArgumentSoftnetException("The site not found.");

                siteDataset.siteId = siteId;
                siteDataset.ownerId = (long)command.Parameters["@OwnerId"].Value;
                siteDataset.domainId = (long)command.Parameters["@DomainId"].Value;
                siteDataset.domainName = (string)command.Parameters["@DomainName"].Value;
                siteDataset.gclientCount = (int)command.Parameters["@GClientCount"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void GetSiteConfigDataset(string accountName, long siteId, SiteConfigDataset siteDataset)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_GetSiteConfigDataset";

                command.Parameters.Add("@AccountName", SqlDbType.NVarChar, 256);
                command.Parameters["@AccountName"].Direction = ParameterDirection.Input;
                command.Parameters["@AccountName"].Value = accountName;

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteId"].Value = siteId;

                command.Parameters.Add("@DomainId", SqlDbType.BigInt);
                command.Parameters["@DomainId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@DomainName", SqlDbType.NVarChar, 256);
                command.Parameters["@DomainName"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                SqlDataReader dataReader = command.ExecuteReader();
                try
                {
                    if (dataReader.FieldCount > 0)
                    {
                        SiteData siteData = new SiteData();
                        if (dataReader.Read())
                        {
                            siteData.siteId = (long)dataReader[0];
                            siteData.siteKind = (int)dataReader[1];
                            if (dataReader[2] != DBNull.Value)
                                siteData.serviceType = (string)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                siteData.contractAuthor = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                siteData.ssHash = (string)dataReader[4];
                            siteData.guestSupported = (bool)dataReader[5];
                            siteData.guestAllowed = (bool)dataReader[6];
                            siteData.statelessGuestSupported = (bool)dataReader[7];
                            siteData.rolesSupported = (bool)dataReader[8];
                            if (dataReader[9] != DBNull.Value)
                                siteData.defaultRoleId = (long)dataReader[9];
                            siteData.implicitUsersAllowed = (bool)dataReader[10];
                            siteData.structured = (bool)dataReader[11];
                            siteData.enabled = (bool)dataReader[12];
                            if (dataReader[13] != DBNull.Value)
                                siteData.description = (string)dataReader[13];
                        }
                        else
                            throw new ArgumentSoftnetException("The site not found.");

                        if (dataReader.NextResult() == false)
                            throw new DataDefinitionSoftnetException();

                        List<ServiceData> services = new List<ServiceData>();
                        while (dataReader.Read())
                        {
                            ServiceData sData = new ServiceData();
                            sData.serviceUid = (Guid)dataReader[0];
                            sData.serviceId = (long)dataReader[1];
                            sData.siteId = (long)dataReader[2];
                            if (dataReader[3] != DBNull.Value)
                                sData.serviceType = (string)dataReader[3];
                            if (dataReader[4] != DBNull.Value)
                                sData.contractAuthor = (string)dataReader[4];
                            sData.version = (string)dataReader[5];
                            sData.hostname = (string)dataReader[6];
                            if (dataReader[7] != DBNull.Value)
                                sData.ssHash = (string)dataReader[7];
                            sData.enabled = (bool)dataReader[8];
                            services.Add(sData);
                        }

                        siteDataset.siteData = siteData;
                        siteDataset.services = services;
                    }
                }
                finally
                {
                    dataReader.Close();
                }

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == -1)
                        throw new ArgumentSoftnetException("The site not found.");
                    if (resultCode == -2)
                        throw new AccountNotFoundSoftnetException(accountName);
                    throw new DataDefinitionSoftnetException();
                }

                siteDataset.siteId = siteId;
                siteDataset.domainId = (long)command.Parameters["@DomainId"].Value;
                siteDataset.domainName = (string)command.Parameters["@DomainName"].Value;                
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void ChangeSiteDescription(long siteId, string description)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_ChangeSiteDescription";

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteId"].Value = siteId;

                command.Parameters.Add("@Description", SqlDbType.NVarChar, 256);
                command.Parameters["@Description"].Direction = ParameterDirection.Input;
                command.Parameters["@Description"].Value = description;

                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static long CreateClient(long ownerId, long siteId, long userId)
    {
        int clientKeyLength = SoftnetRegistry.settings_getClientKeyLength(); 
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_ClientKeyExists";

                command.Parameters.Add("@ClientKey", SqlDbType.VarChar, 32);
                command.Parameters["@ClientKey"].Direction = ParameterDirection.Input;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                string clientKey = null;
                for (int keyLength = clientKeyLength; keyLength <= 32; keyLength++)
                {
                    clientKey = Randomizer.generateClientKey(keyLength);
                    command.Parameters["@ClientKey"].Value = clientKey;
                    command.ExecuteNonQuery();
                    if (((int)command.Parameters["@ReturnValue"].Value) == 0)
                        break;
                    if (keyLength == 32)
                        throw new OperationFailedSoftnetException("Failed to generate a client key.");
                }

                command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_CreateClient";

                command.Parameters.Add("@OwnerId", SqlDbType.BigInt);
                command.Parameters["@OwnerId"].Direction = ParameterDirection.Input;
                command.Parameters["@OwnerId"].Value = ownerId;                

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteId"].Value = siteId;

                command.Parameters.Add("@UserId", SqlDbType.BigInt);
                command.Parameters["@UserId"].Direction = ParameterDirection.Input;
                command.Parameters["@UserId"].Value = userId;

                command.Parameters.Add("@ClientKey", SqlDbType.VarChar, 32);
                command.Parameters["@ClientKey"].Direction = ParameterDirection.Input;
                command.Parameters["@ClientKey"].Value = clientKey;

                command.Parameters.Add("@ClientId", SqlDbType.BigInt);
                command.Parameters["@ClientId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                    
                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == 5)
                        throw new LimitReachedSoftnetException("The limit for the maximum number of private clients in all of your domains has been reached.");
                    if (resultCode == 6)
                        throw new LimitReachedSoftnetException("The limit for the maximum number of clients created by you has been reached.");
                    throw new OperationFailedSoftnetException("Failed to create a client.");                    
                }
                
                return (long)command.Parameters["@ClientId"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static long CreateContactClient(long consumerId, long siteId, long userId)
    {
        int clientKeyLength = SoftnetRegistry.settings_getClientKeyLength();
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_ClientKeyExists";

                command.Parameters.Add("@ClientKey", SqlDbType.VarChar, 32);
                command.Parameters["@ClientKey"].Direction = ParameterDirection.Input;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                string clientKey = null;
                for (int keyLength = clientKeyLength; keyLength <= 32; keyLength++)
                {
                    clientKey = Randomizer.generateClientKey(keyLength);
                    command.Parameters["@ClientKey"].Value = clientKey;
                    command.ExecuteNonQuery();
                    if (((int)command.Parameters["@ReturnValue"].Value) == 0)
                        break;
                    if (keyLength == 32)
                        throw new OperationFailedSoftnetException("Failed to generate a client key.");
                }

                command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_CreateContactClient";

                command.Parameters.Add("@ConsumerId", SqlDbType.BigInt);
                command.Parameters["@ConsumerId"].Direction = ParameterDirection.Input;
                command.Parameters["@ConsumerId"].Value = consumerId;

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteId"].Value = siteId;

                command.Parameters.Add("@UserId", SqlDbType.BigInt);
                command.Parameters["@UserId"].Direction = ParameterDirection.Input;
                command.Parameters["@UserId"].Value = userId;

                command.Parameters.Add("@ClientKey", SqlDbType.VarChar, 32);
                command.Parameters["@ClientKey"].Direction = ParameterDirection.Input;
                command.Parameters["@ClientKey"].Value = clientKey;

                command.Parameters.Add("@ClientId", SqlDbType.BigInt);
                command.Parameters["@ClientId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == 5)
                        throw new LimitReachedSoftnetException("The limit for the maximum number of contact clients for the provider has been reached.");
                    if (resultCode == 6)
                        throw new LimitReachedSoftnetException("The limit for the maximum number of clients created by you has been reached.");
                    throw new OperationFailedSoftnetException("Failed to create a client.");
                }

                return (long)command.Parameters["@ClientId"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static long CreateGuestClient(long creatorId, long siteId, long userId)
    {
        int clientKeyLength = SoftnetRegistry.settings_getClientKeyLength();
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Softnet"].ConnectionString;
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_ClientKeyExists";

                command.Parameters.Add("@ClientKey", SqlDbType.VarChar, 32);
                command.Parameters["@ClientKey"].Direction = ParameterDirection.Input;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                string clientKey = null;
                for (int keyLength = clientKeyLength; keyLength <= 32; keyLength++)
                {
                    clientKey = Randomizer.generateClientKey(keyLength);
                    command.Parameters["@ClientKey"].Value = clientKey;
                    command.ExecuteNonQuery();
                    if (((int)command.Parameters["@ReturnValue"].Value) == 0)
                        break;
                    if (keyLength == 32)
                        throw new OperationFailedSoftnetException("Failed to generate a client key.");
                }

                command = new SqlCommand();
                command.Connection = Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Softnet_Mgt_CreateGuestClient";

                command.Parameters.Add("@CreatorId", SqlDbType.BigInt);
                command.Parameters["@CreatorId"].Direction = ParameterDirection.Input;
                command.Parameters["@CreatorId"].Value = creatorId;

                command.Parameters.Add("@SiteId", SqlDbType.BigInt);
                command.Parameters["@SiteId"].Direction = ParameterDirection.Input;
                command.Parameters["@SiteId"].Value = siteId;

                command.Parameters.Add("@UserId", SqlDbType.BigInt);
                command.Parameters["@UserId"].Direction = ParameterDirection.Input;
                command.Parameters["@UserId"].Value = userId;

                command.Parameters.Add("@ClientKey", SqlDbType.VarChar, 32);
                command.Parameters["@ClientKey"].Direction = ParameterDirection.Input;
                command.Parameters["@ClientKey"].Value = clientKey;

                command.Parameters.Add("@ClientId", SqlDbType.BigInt);
                command.Parameters["@ClientId"].Direction = ParameterDirection.Output;

                command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();

                int resultCode = (int)command.Parameters["@ReturnValue"].Value;
                if (resultCode != 0)
                {
                    if (resultCode == 5)
                        throw new LimitReachedSoftnetException("The limit for the maximum number of guest clients for the provider has been reached.");
                    if (resultCode == 6)
                        throw new LimitReachedSoftnetException("The limit for the maximum number of clients created by you has been reached.");
                    throw new OperationFailedSoftnetException("Failed to create a client.");
                }

                return (long)command.Parameters["@ClientId"].Value;
            }
        }
        catch (SqlException ex)
        {
            throw new DatabaseSoftnetException(ex.Message);
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }   
}
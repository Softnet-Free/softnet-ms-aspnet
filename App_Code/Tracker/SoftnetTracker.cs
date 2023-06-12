using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Configuration;

public class SoftnetTracker
{
    public SoftnetTracker() { }

    public static void enableOwner(long ownerId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.enableOwner(ownerId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void disableOwner(long ownerId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.disableOwner(ownerId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void deleteOwner(long ownerId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.deleteOwner(ownerId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }    
    }

    public static void assignRoleProvider(long ownerId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.assignRoleProvider(ownerId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void removeRoleProvider(long ownerId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.removeRoleProvider(ownerId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }    
    }

    public static void deleteDomain(long domainId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.deleteDomain(domainId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void createPrivateUser(long domainId, string userName, bool dedicatedStatus)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.createPrivateUser(domainId, userName, dedicatedStatus);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void createContactUser(long domainId, long contactId, string userName, bool dedicatedStatus, bool enabledStatus)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.createContactUser(domainId, contactId, userName, dedicatedStatus, enabledStatus);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void updateUser(long domainId, int userKind, long userId, bool enabledStatus)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.updateUser(domainId, userKind, userId, enabledStatus);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {                
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void updateUser(long domainId, long userId, string userName, bool enabledStatus, bool dedicatedStatus)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.updateUser2(domainId, userId, userName, enabledStatus, dedicatedStatus);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void deleteUser(long domainId, long userId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.deleteUser(domainId, userId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void allowGuest(long siteId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.allowGuest(siteId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void denyGuest(long siteId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.denyGuest(siteId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void allowImplicitUsers(long siteId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.allowImplicitUsers(siteId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void denyImplicitUsers(long siteId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.denyImplicitUsers(siteId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void addSiteUser(long siteId, long userId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.addSiteUser(siteId, userId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void removeSiteUser(long siteId, long userId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.removeSiteUser(siteId, userId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void setDefaultRole(long siteId, long roleId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.setDefaultRole(siteId, roleId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void removeDefaultRole(long siteId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.removeDefaultRole(siteId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void updateUserRoles(long siteId, long userId, List<long> roles)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.updateUserRoles(siteId, userId, roles);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void addService(long siteId, string hostname)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.addService(siteId, hostname);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void setServicePassword(long siteId, long serviceId, string salt, string saltedPassword)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.setServicePassword(siteId, serviceId, salt, saltedPassword);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void changeHostname(long siteId, long serviceId, string hostname)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.changeHostname(siteId, serviceId, hostname);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void applyStructure(long siteId, long serviceId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.applyStructure(siteId, serviceId);
                channelFactory.Close();
                if (errorCode != 0)
                {
                    if (errorCode == -5)
                        throw new DataIntegritySoftnetException();
                    throw new OperationFailedSoftnetException();
                }
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void setServicePingPeriod(long siteId, long serviceId, int pingPeriod)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.setServicePingPeriod(siteId, serviceId, pingPeriod);
                channelFactory.Close();
                if (errorCode == 1)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void enableService(long siteId, long serviceId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.enableService(siteId, serviceId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void disableService(long siteId, long serviceId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.disableService(siteId, serviceId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void deleteService(long siteId, long serviceId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.deleteService(siteId, serviceId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void deleteSite(long siteId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.deleteSite(siteId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void enableSite(long siteId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.enableSite(siteId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void disableSite(long siteId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.disableSite(siteId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void setSiteAsMultiservice(long siteId)
    {
        try 
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));

            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.setSiteAsMultiservice(siteId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }         
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void setClientPassword(long siteId, long clientId, string salt, string saltedPassword)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.setClientPassword(siteId, clientId, salt, saltedPassword);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void setClientPingPeriod(long siteId, long clientId, int pingPeriod)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.setClientPingPeriod(siteId, clientId, pingPeriod);
                channelFactory.Close();
                if (errorCode == 1)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void deleteClient(long siteId, long clientId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.deleteClient(siteId, clientId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void deleteContact(long contactId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.deleteContact(contactId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }

    public static void restoreContact(long ownerId, long selectedUserId)
    {
        try
        {
            string managementUri = "net.tcp://" + ConfigurationManager.AppSettings["ManagementEndpoint"];
            ChannelFactory<IManagement> channelFactory = new ChannelFactory<IManagement>(
                            new NetTcpBinding(SecurityMode.None),
                            new EndpointAddress(managementUri));
            try
            {
                IManagement managementChannel = channelFactory.CreateChannel();
                int errorCode = managementChannel.restoreContact(ownerId, selectedUserId);
                channelFactory.Close();
                if (errorCode != 0)
                    throw new OperationFailedSoftnetException();
            }
            catch (TimeoutException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
            catch (CommunicationException ex)
            {
                channelFactory.Abort();
                throw new InfrastructureErrorSoftnetException(ex.Message);
            }
        }
        catch (ConfigurationErrorsException ex)
        {
            throw new ConfigSoftnetException(ex.Message);
        }
    }
}
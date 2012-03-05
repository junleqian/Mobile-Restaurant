namespace Gour.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;
    using Gour.Web.Models;

    // Summary:
    //     Sample Entity Framework 4.1 context class intended for connecting with SQL Azure.
    //     Place a DbSet<T> property for each collection of entities that should be queried from the database.

    public class SqlDataContext : DbContext, IUserPrivilegesRepository, IPushUserEndpointsRepository
    {
        public const string PushUserTableName = "PushUserEndpoints";

        private const string PublicUserId = "00000000-0000-0000-0000-000000000000";

        public SqlDataContext()
            : base(ConfigReader.GetConfigValue("SqlSampleDataContextConnectionString"))
        {
        }

        public DbSet<SqlSampleData> SqlSampleData { get; set; }

        public DbSet<UserPrivilege> UserPrivileges { get; set; }

        public DbSet<PushUserEndpoint> PushUserEndpoints { get; set; }

        public DbSet<QueuedPushNotification> QueuedPushNotifications { get; set; }



        public IEnumerable<UserPrivilege> GetUsersWithPrivilege(string privilege)
        {
            return this.UserPrivileges
                .Where(p => p.Privilege.Equals(privilege, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public void AddSampleData(int id, string title, bool isPublic, DateTime date)
        {
            this.SqlSampleData.Add(new SqlSampleData
            {
                Id = id,
                Title = title,
                IsPublic = isPublic,
                Date = date
            });

            this.SaveChanges();
        }

        public void AddPrivilegeToUser(string userId, string privilege)
        {
            if (!this.HasUserPrivilege(userId, privilege))
            {
                this.UserPrivileges.Add(new UserPrivilege { UserId = userId, Privilege = privilege });
                this.SaveChanges();
            }
        }

        public void AddPublicPrivilege(string privilege)
        {
            this.AddPrivilegeToUser(PublicUserId, privilege);
        }

        public void RemovePrivilegeFromUser(string userId, string privilege)
        {
            var userPrivilege = this.GetUserPrivilege(userId, privilege);
            if (userPrivilege != null)
            {
                this.UserPrivileges.Remove(userPrivilege);
                this.SaveChanges();
            }
        }

        public void DeletePublicPrivilege(string privilege)
        {
            this.RemovePrivilegeFromUser(PublicUserId, privilege);
        }

        public void DeletePrivilege(string privilege)
        {
            var userPrivileges = this.GetUsersWithPrivilege(privilege);
            foreach (var userPrivilege in userPrivileges)
            {
                this.UserPrivileges.Remove(userPrivilege);
            }

            this.SaveChanges();
        }

        public bool HasUserPrivilege(string userId, string privilege)
        {
            return this.GetUserPrivilege(userId, privilege) != null;
        }

        public bool PublicPrivilegeExists(string privilege)
        {
            return this.HasUserPrivilege(PublicUserId, privilege);
        }

        public void AddPushUserEndpoint(PushUserEndpoint pushUserEndpoint)
        {
            this.PushUserEndpoints.Add(pushUserEndpoint);
            this.SaveChanges();
        }

        public void UpdatePushUserEndpoint(PushUserEndpoint pushUserEndpoint)
        {
            var storedPushUser = this.GetPushUserByApplicationAndDevice(pushUserEndpoint.ApplicationId, pushUserEndpoint.DeviceId);
            if (storedPushUser == null)
            {
                throw new ArgumentException(@"Push user endpoint not in repository", "pushUserEndpoint");
            }

            storedPushUser.TileCount = pushUserEndpoint.TileCount;
            storedPushUser.UserId = pushUserEndpoint.UserId;
            storedPushUser.ChannelUri = pushUserEndpoint.ChannelUri;
            this.SaveChanges();
        }

        public void RemovePushUserEndpoint(PushUserEndpoint pushUserEndpoint)
        {
            var storedPushUser = this.GetPushUserByApplicationAndDevice(pushUserEndpoint.ApplicationId, pushUserEndpoint.DeviceId);
            if (storedPushUser.QueuedPushNotifications != null)
            {
                foreach (var message in storedPushUser.QueuedPushNotifications)
                {
                    this.QueuedPushNotifications.Remove(message);
                }
            }

            this.PushUserEndpoints.Remove(storedPushUser);
            this.SaveChanges();
        }

        [CLSCompliant(false)]
        public PushUserEndpoint GetPushUserByApplicationAndDevice(string applicationId, string deviceId)
        {
            return this.PushUserEndpoints
                .Where(u => u.ApplicationId.Equals(applicationId, StringComparison.OrdinalIgnoreCase)
                            && u.DeviceId.Equals(deviceId, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault<PushUserEndpoint>();
        }

        public IEnumerable<string> GetAllPushUsers()
        {
            return this.PushUserEndpoints
                .ToList()
                .GroupBy(u => u.UserId)
                .Select(g => g.Key);
        }

        public PushUserEndpoint GetPushUserEndpointByChannel(Uri channelUri)
        {
            var channel = channelUri.ToString();
            return this.PushUserEndpoints.SingleOrDefault(u => u.ChannelUri.Equals(channel, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<PushUserEndpoint> GetPushUsersByName(string userId)
        {
            return this.PushUserEndpoints
                .Where(u => u.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public IEnumerable<QueuedPushNotification> GetQueuedPushNotificationsByChannel(Uri channelUri)
        {
            var channel = channelUri.ToString();
            return this.QueuedPushNotifications.Where(p => p.ChannelUri.Equals(channel, StringComparison.OrdinalIgnoreCase));
        }

        public void DeleteQueuedMessage(QueuedPushNotification message)
        {
            this.QueuedPushNotifications.Remove(message);
            this.SaveChanges();
        }

        public void AddQueuedPushNotification(QueuedPushNotification queuedPushNotification)
        {
            this.QueuedPushNotifications.Add(queuedPushNotification);
            this.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        private UserPrivilege GetUserPrivilege(string userId, string privilege)
        {
            return this.UserPrivileges
                .Where(p => p.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && p.Privilege.Equals(privilege, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .FirstOrDefault();
        }
    }
}
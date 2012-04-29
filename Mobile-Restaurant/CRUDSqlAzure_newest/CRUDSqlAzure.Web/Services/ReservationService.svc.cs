
using System;
using System.Data.Objects;
using System.Data.Services;
using System.Data.Services.Common;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using Microsoft.IdentityModel.Claims;
using Microsoft.Samples.CRUDSqlAzure.Web.Infrastructure;
using Microsoft.Samples.CRUDSqlAzure.Web.Models;

namespace Microsoft.Samples.CRUDSqlAzure.Web.Services
{

        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ReservationService : DataService<NorthwindEntities>
    {
        private readonly IUserPrivilegesRepository userPrivilegesRepository;
        private readonly IClaimsIdentity identity;
        // This method is called only once to initialize service-wide policies.

        public ReservationService()
            : this(HttpContext.Current.User.Identity as IClaimsIdentity, new InfrastructureEntities())
        {
        }

        public ReservationService(IClaimsIdentity identity, IUserPrivilegesRepository userPrivilegesRepository)
        {
            this.identity = identity;
            this.userPrivilegesRepository = userPrivilegesRepository;
        }

        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            // config.SetEntitySetAccessRule("MyEntityset", EntitySetRights.AllRead);
            // config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
            //TODO: change c&s back to read AllRead
            
            config.SetEntitySetAccessRule("Categories", EntitySetRights.All);
            config.SetEntitySetAccessRule("Restaurants", EntitySetRights.All);
            config.SetEntitySetAccessRule("Customers", EntitySetRights.All);
            config.SetEntitySetAccessRule("Orders", EntitySetRights.All);
            config.SetEntitySetAccessRule("Order_Details", EntitySetRights.All);
            config.SetEntitySetAccessRule("Schedules", EntitySetRights.All);
            config.SetEntitySetAccessRule("Tables", EntitySetRights.All);
            config.SetEntitySetAccessRule("Reservations", EntitySetRights.All);
            config.SetEntitySetAccessRule("Ratings", EntitySetRights.All);
            config.SetEntitySetAccessRule("Reviews", EntitySetRights.All);
            config.SetEntitySetAccessRule("DishView", EntitySetRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
            
        }


        public string serviceTest(string param)
        {
            string output = "success: " + param;
            return output;
        }
    }

}

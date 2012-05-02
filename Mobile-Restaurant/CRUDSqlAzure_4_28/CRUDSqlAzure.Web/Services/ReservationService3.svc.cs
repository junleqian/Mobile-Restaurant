
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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ReservationService3" in code, svc and config file together.

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ReservationService3 : IReservationService3
    {
        public string testService3(string param)
        {
            string temp = "hello " + param;
            return temp;
        }
    }
}

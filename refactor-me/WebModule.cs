using Ninject.Modules;
using Ninject.Web.Common;
using System.Data.Entity;
using System.Web;
using System.IO;
using Ninject.Parameters;
using System.Data.Entity.Core.EntityClient;

namespace ProductAPI
{
    public class WebModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ProductDBContext>().ToSelf().InRequestScope();
        }
    }
}
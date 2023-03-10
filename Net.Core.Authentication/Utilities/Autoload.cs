using Autofac;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Reflection;
using Module = Autofac.Module;

namespace Net.Core.Authentication.Utilities
{
    public class Autoload : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()) // find all types in the assembly
                   .Where(t =>t.Name.EndsWith("Service")) // filter the types
                   .AsImplementedInterfaces()  // register the service with all its public interfaces
                   .InstancePerLifetimeScope(); // register the services as singletons

            //builder.RegisterGeneric(typeof(Repository<>))  //generic
            //    .As(typeof(IRepositoryAsync<>))
            //    .InstancePerLifetimeScope();
        }
    }
}

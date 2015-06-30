using A4EPARC.Models;
using A4EPARC.Repositories;
using A4EPARC.Services;
using A4EPARC.ViewModels;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(A4EPARC.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(A4EPARC.App_Start.NinjectWebCommon), "Stop")]

namespace A4EPARC.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IAuthenticationService>().To<AuthenticationService>().InRequestScope();
            kernel.Bind<IClientRepository>().To<ClientRepository>().InRequestScope();
            kernel.Bind<ISecurityService>().To<SecurityService>().InRequestScope();
            kernel.Bind<IRepository<User>>().To<Repository<User>>().InRequestScope();
            kernel.Bind<IQuestionsRepository>().To<QuestionsRepository>().InRequestScope();
            kernel.Bind<IUserRepository>().To<UserRepository>().InRequestScope();
            kernel.Bind<IRepository<ClientViewModel>>().To<Repository<ClientViewModel>>().InRequestScope();
            kernel.Bind<ICompanyRepository>().To<CompanyRepository>().InRequestScope();
            kernel.Bind<IEmailRepository>().To<EmailRepository>().InRequestScope();
            kernel.Bind<IEmailService>().To<EmailService>().InRequestScope();
            kernel.Bind<ISerializeService>().To<SerializeService>().InRequestScope();
            kernel.Bind<IWebServiceResultsRepository>().To<WebServiceResultsRepository>().InRequestScope();
            kernel.Bind<ISiteLabelsRepository>().To<SiteLabelsRepository>().InRequestScope();
            kernel.Bind<IResultService>().To<ResultService>().InRequestScope();
            kernel.Bind<IQuestionsService>().To<QuestionsService>().InRequestScope();
        }        
    }
}

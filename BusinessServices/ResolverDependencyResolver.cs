using Resolver;
using System.ComponentModel.Composition;

namespace BusinessServices
{
    [Export(typeof(IResolverComponent))]
    public class ResolverDependencyResolver: IResolverComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IProductServices, ProductServices>();
            registerComponent.RegisterType<IUserServices,UserServices>();
            registerComponent.RegisterType<ITokenServices,TokenServices>();
        }
    }
}

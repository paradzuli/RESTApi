using System.ComponentModel.Composition;
using Resolver;

namespace DataModel
{
    [Export(typeof(IResolverComponent))]
    public class ResolverDependencyResolver : IResolverComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<UnitOfWork.IUnitOfWork,UnitOfWork.UnitOfWork>();
        }
    }
}

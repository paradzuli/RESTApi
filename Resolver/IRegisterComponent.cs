
namespace Resolver
{
    public interface IRegisterComponent
    {
        void RegisterType<TFrom, TTo>(bool withInterception = false) where TTo : TFrom;

        void RegisterTypeWithControlledLifeTime<TFrom, TTo>(bool withInterception=false) where TTo:TFrom;
    }
}

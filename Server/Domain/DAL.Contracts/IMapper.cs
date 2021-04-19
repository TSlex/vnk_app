using AutoMapper;

namespace DAL.Contracts
{
    public interface IUniversalMapper
    {
        MapperConfiguration Configuration { get; }
        TOutObject Map<TInObject, TOutObject>(TInObject inObject);
    }
}
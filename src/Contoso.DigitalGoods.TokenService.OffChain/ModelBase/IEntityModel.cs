namespace Contoso.DigitalGoods.TokenService.OffChain.ModelBase
{
    public interface IEntityModel<TIdentifier>
    {
        TIdentifier Id { get; set; }
    }
}
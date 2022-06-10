namespace Game.CoreLogic
{
    public struct RequestChangeLink<TLinkComponent>  where TLinkComponent : ILinkComponent
    {
        public int LinkToEntity;
    }
}
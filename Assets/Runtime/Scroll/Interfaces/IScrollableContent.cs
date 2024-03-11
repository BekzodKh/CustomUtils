namespace CustomUtils.Scroll.Interfaces
{
    public interface IScrollableContent
    {
        void AddContent(IContentView contentView);

        void RemoveContent(IContentView contentView);
    }
}
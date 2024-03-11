namespace CustomUtils.Scroll.Interfaces
{
    public interface IContentHolder
    {
        void AddContent(IContentView contentView);

        void RemoveContent(IContentView contentView);
        
        IContentView[] GetContentViews();
    }
}
namespace CustomUtils.Scroll
{
    public class Scroll : BaseScroll
    {
        protected override void OnAwake()
        {
            ScrollBehaviour = new ScrollBehaviour();
        }
        
        protected override void OnInitScrollBehaviour()
        {
            ScrollBehaviour.SetContentRoot(RootTransform, ContentHolder);
        }
    }
}
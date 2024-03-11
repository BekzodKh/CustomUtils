namespace CustomUtils.Scroll.RoundScroll
{
    public class RoundScroll : BaseScroll
    {
        protected override void OnAwake()
        {
            ScrollBehaviour = new RoundScrollBehaviour();
        }

        protected override void OnInitScrollBehaviour()
        {
            ScrollBehaviour.SetContentRoot(RootTransform, ContentHolder);
        }
    }
}
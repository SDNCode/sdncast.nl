namespace SDNCast.Components
{
    public partial class SocialButtonYouTube : SocialButton
    {
        public SocialButtonYouTube()
        {
            Icon = "fab fa-youtube";
        }

        protected override void OnInitialized()
        {
            Url = "https://www.youtube.com/channel/" + AccountId;
        }
    }
}

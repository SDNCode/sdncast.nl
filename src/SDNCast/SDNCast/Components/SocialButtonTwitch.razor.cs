namespace SDNCast.Components
{
    public partial class SocialButtonTwitch : SocialButton
    {
        public SocialButtonTwitch()
        {
            Icon = "fab fa-twitch";
        }

        protected override void OnInitialized()
        {
            Url = "https://twitch.tv/" + AccountId;
        }
    }
}

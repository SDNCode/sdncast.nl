namespace SDNCast.Components
{
    public partial class SocialButtonTwitter : SocialButton
    {
        public SocialButtonTwitter()
        {
            Icon = "fab fa-twitter";
        }

        protected override void OnInitialized()
        {
            Url = "https://twitter.com/" + AccountId;
        }
    }
}

namespace BihuApiCore.Infrastructure.Extensions.UserClientExtensions
{
    public class UserClient
    {
        public UserClient(string userAgent)
        {
            _userAgent = userAgent;
        }
        private readonly string _userAgent;

        private ClientBrowser _browser;
        public ClientBrowser Browser
        {
            get 
            {
                if (_browser == null)
                {
                    _browser = new ClientBrowser(_userAgent);
                }
                return _browser;
            }
        }

        private ClientOS _os;
        public ClientOS Os
        {
            get
            {
                if (_os == null)
                {
                    _os = new ClientOS(_userAgent);
                }
                return _os;
            }
        }
    }
}

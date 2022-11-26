namespace TandC.RunIfYouWantToLive
{
    public class ServerConfiguration
    {
#if DEVELOPMENT
        private const string DOMAIN = "http://geometryastrosurvivaltandc.com/";
#else
        private const string DOMAIN = "http://geometryastrosurvivaltandc.com/";
#endif

        private const float RESPONSE_CHECKTIME = 0.5f;
        private const float REQUEST_TIMEOUT = RESPONSE_CHECKTIME * 15f;

        public static string GetServerDomain()
        {
            return DOMAIN;
        }
        public static float CheckResponseTime()
        {
            return RESPONSE_CHECKTIME;
        }
        public static float GetRequestTimeout()
        {
            return REQUEST_TIMEOUT;
        }
    }
}

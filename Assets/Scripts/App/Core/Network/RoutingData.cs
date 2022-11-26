namespace TandC.RunIfYouWantToLive
{
    public static class RoutingData
    {
        public const string SIGN_IN = "/rest/users/signIn";
        public const string SIGN_UP = "/rest/users/signUp";
        public const string SIGN_OUT = "/rest/users/signOut";
        public const string CHANGE_USER_PASSWORD = "/rest/users/ChangeUserPassword";
        public const string REMIND_PASSWORD = "/rest/users/RemindPassword";
        public const string GET_PERSONAL_DATA = "/rest/users/GetPersonalData";
        public const string UPDATE_PERSONAL_DATA = "/rest/users/UpdatePersonalData";
        public const string SIGN_IN_VIA_FACEBOOK = "/rest/users/SignInFacebook";
        public const string SEND_FRIENDSHIP_REQUEST = "/rest/users/SendFriendshipRequest";
        public const string GET_LIST_FRIENDSHIP_REQUESTS = "/rest/users/ListFriendshipRequest";
        public const string CONFIRM_FRIENDSHIP_REQUEST = "/rest/users/ConfirmedFriendshipRequest";
        public const string REJECT_FRIENDSHIP_REQUEST = "/rest/users/RejectedFriendshipRequest";
        public const string SEND_PUSH_TO_USER = "/rest/notifications/sendDirectlyToUser";
        public const string SEND_PUSH_TO_ALL_USERS = "/rest/notifications/SendAllUsers";
        public const string GET_ACCOUNT_INFO_BY = "/rest/users/GetAccountInfoBy";
        public const string GET_ACHIEVEMENTS = "/rest/users/GetAchievements";
        public const string COMPLETE_ACHIEVEMENT = "/rest/users/CompleteAchievement";
        public const string GET_USER_DATA = "/rest/users/GetUserData";
        public const string SET_USER_DATA = "/rest/users/SetUserData";
    }
}

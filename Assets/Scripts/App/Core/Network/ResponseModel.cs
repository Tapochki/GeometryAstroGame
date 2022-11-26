using System.Collections.Generic;

namespace TandC.RunIfYouWantToLive.Common
{
    public class ResponseModel
    {
        public bool result;
    }

    public class ErrorResponse : ResponseModel
    {
        public string data;
    }

    public class ErrorResponseArray : ResponseModel
    {
        public string[] data;
    }

    public class DetailedErrorResponse : ResponseModel
    {
        public DetailedError data;
    }
    public class DetailedError
    {
        public string err;
        public string sql;
    }


    #region sign in
    public class SignInResponse : ResponseModel
    {
        public SignInData data;
    }

    public class SignInData
    {
        public string token,
                      session_id;
    }
    #endregion

    #region sign out
    public class SignOutResponse : ResponseModel
    {
        public string data;
    }
    #endregion

    #region change password
    public class ChangePasswordResponse : ResponseModel
    {
        public string data;
    }
    #endregion

    #region remind password
    public class RemindPasswordResponse : ResponseModel
    {
        public string data;
    }
    #endregion

    #region sign up
    public class SignUpResponse : ResponseModel
    {
        public string data;
    }

    #endregion

    #region get personal data
    public class GetPersonalDataResponse : ResponseModel
    {
        public PersonalData data;
    }

    public class PersonalData
    {
        public string UserId,
                      Username,
                      Password,
                      Email,
                      FacebookId,
                      PushNotificationUserID,
                      PhoneNumber,
                      Diocese,
                      Country,
                      Name,
                      Surname,
                      AvatarId,
                      AvatarImage,
                      Parish,
                      GroupLeader,
                      GroupLeaderPhoneNumber,
                      IsConfirmed,
                      IsRegistrationViaFacebook,
                      Language,
                      IsWasFirstAppLogin;

    }
    #endregion
}
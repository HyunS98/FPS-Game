using System;

class UserLoginDTO
{
    public string userID;
    public string userPW;

    public UserLoginDTO(string userID, string userPW)
    {
        this.userID = userID;
        this.userPW = userPW;
    }
}


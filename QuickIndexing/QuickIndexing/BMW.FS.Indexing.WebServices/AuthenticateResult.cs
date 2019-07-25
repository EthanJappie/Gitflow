using System;
using System.Collections.Generic;
using System.Web;


[Serializable]
public class AuthenticateResult
{
    public bool AuthenticateSuccess;
    public string FIRST_NAME;
    public string LAST_NAME;
    public string DEALER;
    public string BRAND;
    public string ErrorMessage;
    public Guid Token;
}

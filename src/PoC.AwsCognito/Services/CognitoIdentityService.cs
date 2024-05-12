using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;

namespace PoC.AwsCognito.Services;

public class CognitoIdentityService
{
    private readonly SignInManager<CognitoUser> _signInManager;
    private readonly CognitoUserManager<CognitoUser> _userManager;
    private readonly CognitoUserPool _pool;

    public CognitoIdentityService(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
    {
        _signInManager = signInManager;
        _userManager = userManager as CognitoUserManager<CognitoUser>;
        _pool = pool;
    }

    public async Task SignUpAsync(string userName, string password, string email)
    {
        var user = _pool.GetUser(userName);
        user.Attributes.Add(CognitoAttribute.Email.AttributeName, email);
        
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            Console.WriteLine("User created a new account with password.");

            await _signInManager.SignInAsync(user, isPersistent: false);
        }
    }
}
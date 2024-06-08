using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PoC.AwsCognito.Models;

namespace PoC.AwsCognito.Controllers;

[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AwsCognitoSettings _awsCognitoSettings;
    private readonly RegionEndpoint _region = RegionEndpoint.USEast1;

    public UsersController(IOptions<AwsCognitoSettings> awsCognitoSettings)
    {
        _awsCognitoSettings = awsCognitoSettings.Value;
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<string>> Register(RegisterModel user)
    {
        var cognito = new AmazonCognitoIdentityProviderClient(_region);

        var request = new SignUpRequest
        {
            ClientId = _awsCognitoSettings.ClientId,
            Password = user.Password,
            Username = user.UserName
        };

        var emailAttribute = new AttributeType
        {
            Name = "email",
            Value = user.Email
        };
        request.UserAttributes.Add(emailAttribute);

        await cognito.SignUpAsync(request);

        var req = new AdminConfirmSignUpRequest
        {
            Username = user.UserName,
            UserPoolId = _awsCognitoSettings.UserPoolId
        };

        await cognito.AdminConfirmSignUpAsync(req);

        return Ok();
    }

    [HttpPost]
    [Route("signin")]
    public async Task<ActionResult<string>> SignIn(RegisterModel user)
    {
        var cognito =
            new AmazonCognitoIdentityProviderClient(_awsCognitoSettings.AccessKey, _awsCognitoSettings.SecretKey,
                _region);

        var request = new AdminInitiateAuthRequest
        {
            UserPoolId = _awsCognitoSettings.UserPoolId,
            ClientId = _awsCognitoSettings.ClientId,
            AuthFlow = AuthFlowType.ADMIN_USER_PASSWORD_AUTH
        };

        request.AuthParameters.Add("USERNAME", user.UserName);
        request.AuthParameters.Add("PASSWORD", user.Password);

        var response = await cognito.AdminInitiateAuthAsync(request);

        return Ok(response.AuthenticationResult.IdToken);
    }
}
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;

namespace PoC.AwsCognito.Controllers;

public class UsersController : ControllerBase
{
    private const string ClientId = "6c4s2rv24lkk8b8iai753bq0c4";
    private const string UserPoolId = "us-east-1_ZV8S0Kdr1";
    private const string AccessKey  = "AKIA4MTWM4VVKWMIUVVY";
    private const string SecretKey  = "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";
    private readonly RegionEndpoint _region = RegionEndpoint.USEast1;

    [HttpPost]
    [Route("api/register")]
    public async Task<ActionResult<string>> Register(RegisterModel user)
    {
        var cognito = new AmazonCognitoIdentityProviderClient(_region);

        var request = new SignUpRequest
        {
            ClientId = ClientId,
            Password = user.Password,
            Username = user.UserName
        };

        var emailAttribute = new AttributeType
        {
            Name = "email",
            Value = user.Email
        };
        request.UserAttributes.Add(emailAttribute);

        var response = await cognito.SignUpAsync(request);

        return Ok();
    }

    [HttpPost]
    [Route("api/signin")]
    public async Task<ActionResult<string>> SignIn(RegisterModel user)
    {
        var cognito = new AmazonCognitoIdentityProviderClient(AccessKey, SecretKey, _region);

        var request = new AdminInitiateAuthRequest
        {
            UserPoolId = UserPoolId,
            ClientId = ClientId,
            AuthFlow = AuthFlowType.ADMIN_USER_PASSWORD_AUTH
        };

        request.AuthParameters.Add("USERNAME", user.UserName);
        request.AuthParameters.Add("PASSWORD", user.Password);

        var response = await cognito.AdminInitiateAuthAsync(request);

        return Ok(response.AuthenticationResult.IdToken);
    }
}


public class RegisterModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}
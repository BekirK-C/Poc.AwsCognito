namespace PoC.AwsCognito.Models;

public class AwsCognitoSettings
{
    public string ClientId { get; set; }
    public string UserPoolId { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string Region { get; set; }
}
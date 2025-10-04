using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using RT;
using System;
using System.IO;
using UnityEngine;

public class AwsS3Service : MonoBehaviour {

    private string IdentityPoolId = "us-east-1:0886f982-c62e-410a-8297-145ec7b99d82";
    private RegionEndpoint cognitoIdentityRegion;
    private RegionEndpoint s3Region;
    public string S3BucketName = "richtown";

    public static AwsS3Service Instance;

    #region private members

    private IAmazonS3 s3Client;
    private AWSCredentials credentials;

    #endregion
    private void Awake()
    {
        Instance = this;
        IdentityPoolId = "us-east-1:0886f982-c62e-410a-8297-145ec7b99d82";
        cognitoIdentityRegion = RegionEndpoint.USEast1;
        s3Region = RegionEndpoint.APSoutheast1;
        S3BucketName = "richtown";

    }

	void Start(){
		UnityInitializer.AttachToGameObject(this.gameObject);
#if UNITY_IOS
        Amazon.AWSConfigs.HttpClient = Amazon.AWSConfigs.HttpClientOption.UnityWebRequest;
#endif

        credentials = new CognitoAWSCredentials(IdentityPoolId, cognitoIdentityRegion);
		s3Client = new AmazonS3Client(credentials, s3Region);
	}

    public void AsyncUploadObject(byte[] bytes, string key, Action<HttpResult<string>> action)
    {
        Stream stream = new MemoryStream(bytes);
        
        var request = new PostObjectRequest()
        {
            Bucket = S3BucketName,
            Key = key,
            InputStream = stream,
            CannedACL = S3CannedACL.Private,
            Region = s3Region
        };

        s3Client.PostObjectAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                HttpResult<string> ret = new HttpResult<string>();
                ret.code = 200;
                ret.data = string.Format("http://s3-{0}.amazonaws.com/{1}/{2}", s3Region.SystemName, S3BucketName, key);
                if (action != null)
                {
                    action(ret);
                }
            }
            else
            {
                HttpResult<string> ret = new HttpResult<string>();
                ret.code = 1023;
                ret.errorMsg = LocalizationManager.Instance.GetText("1023");
                if (action != null)
                {
                    action(ret);
                }
                return;
            }
        });
        
    }

    public void AsyncDownloadObject(string httpPath, Action<HttpResult<byte[]>> action)
    {
        Game.Instance.HttpReq.Download(httpPath, action, false);
    }
}

# Generative AI .NET 8 Sample

This sample application is written using the .NET 8 minimal Web API to demonstrate calling the Vertex [Generative AI API](https://cloud.google.com/vertex-ai/docs/generative-ai/model-reference/gemini?_ga=2.228338718.-220341458.1702671073) with the Gemini model.

Using **Vertex AI Studio** in the Google Cloud Consle you can easily design and test prompts.  You can also quickly generate code using the **<> GET CODE** button in the upper right.

![alt text for image](vertex-screenshot.png)

## Prerequisites

* This sample assumes you have a GCP project with the [Vertex AI API enabled](https://cloud.google.com/vertex-ai/docs/start/cloud-environment#enable_vertexai_apis)
* You have installed the [gcloud cli](https://cloud.google.com/sdk/docs/install)

### Update your GCP project ID

You can find your project id in the GCP console under the [project settings](https://support.google.com/googleapi/answer/7014113?hl=en). Update your `appsettings.json`:
```json
{
  "projectId": "YOUR_GCP_PROJECT_ID_HERE"
}
```

## Generating a .NET wrapper for the Vertex AI API

Unfortunately, there is not a Vertex SDK for .NET *yet*.  Thankfully all GCP APIs have a REST API which makes it easy for us to wrap with C# using the .NET [HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-8.0).  

### Authenticating with Google Cloud APIs

We are going to use the GCP [Application Default Credentials](https://cloud.google.com/docs/authentication/application-default-credentials) which provides an easy abstraction to authenticate with GCP APIs.  When running locally it will look for an environment variable named `GOOGLE_APPLICATION_CREDENTIALS` which contains the path to a JSON file with your GCP credentials.  When you deploy to GCP (i.e. Cloud Run, GKE, GCE, etc...) the SDK will automatically authenticate using the metadata server, so you don't have to pass in any credentials.

Use these [instructions](https://cloud.google.com/docs/authentication/application-default-credentials#personal) to generate your credentials file.  For example on linux follow these steps:

```bash
gcloud auth application-default login 

export GOOGLE_APPLICATION_CREDENTIALS=~/.config/gcloud/application_default_credentials.json
```

The code to get the required access token is in the `GetAccessTokenAsync()` method of `VertexAIModelGenerator.cs`:

```C#

        using var client = new HttpClient();

        // ... 

        var accessToken = await GetAccessTokenAsync();
        
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // ...

    private async Task<string> GetAccessTokenAsync()
    {
        // Use Application Default Credentials
        var credential = await GoogleCredential.GetApplicationDefaultAsync();

        // Create credential scoped to GCP APIs
        var scoped = credential.CreateScoped(
            new[] { "https://www.googleapis.com/auth/cloud-platform" });

        var accessToken = await scoped.UnderlyingCredential.GetAccessTokenForRequestAsync();

        return accessToken;
    }
```

### BONUS: Use Duet to help generate the .NET Wrapper

As an added bonus you can use [Duet AI assistance](https://cloud.google.com/code/docs/vscode/write-code-duet-ai) with Google's [Cloud Code Extension](https://cloud.google.com/code/docs/vscode/install#install) for VS Code.



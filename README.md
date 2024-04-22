# Generative AI .NET 8 Sample

This sample application is written using the .NET 8 minimal Web API to demonstrate calling the Vertex [Generative AI API](https://cloud.google.com/vertex-ai/docs/generative-ai/model-reference/gemini?_ga=2.228338718.-220341458.1702671073) with the Gemini model and the latest [Google.Cloud.AIPlatform.V1](https://cloud.google.com/dotnet/docs/reference/Google.Cloud.AIPlatform.V1) SDK.

Using **Vertex AI Studio** in the Google Cloud Consle you can easily design and test prompts.  You can also quickly generate code using the **<> GET CODE** button in the upper right.

![alt text for image](assets/vertex-screenshot.png)

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

## Running

### Deploying to Cloud Run
To deploy to Cloud Run from source, which will build a container for you:
```sh
gcloud run deploy generative-quote --source . --set-env-vars ASPNETCORE_HTTP_PORTS=8080
```

### Locally

To run locally ensure you have the [dotnet cli](https://dotnet.microsoft.com/en-us/download) installed. Configure [appsettings.json](appsettings.json) with your project and then run the app:

```sh
dotnet run
```

You can then use curl to generate quotes:

```sh
curl 'http://localhost:5000/random-quote?prompt=waterfalls'
```

## End to End

Who doesn't love a CLI? Ok, but if you want a nice little Angular application to display these quotes see [this](/assets/e2e.md).


## Google SDK Reference

There are some [samples in C#](https://cloud.google.com/vertex-ai/generative-ai/docs/multimodal/send-chat-prompts-gemini#gemini-chat-samples-csharp) which demonstrate calling the Gemini Vertex API.  This code uses the [PredictionServiceClient](https://cloud.google.com/dotnet/docs/reference/Google.Cloud.AIPlatform.V1/latest/Google.Cloud.AIPlatform.V1.PredictionServiceClient) API.  

Use the SDK in your project by running the following command:

```bash
dotnet add package Google.Cloud.AIPlatform.V1
```

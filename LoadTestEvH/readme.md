# Load Testing for Azure Event Hub Namespace
This is a tool that can be used to publish bulk data to an event hub, this application need json file for source data, the target event hub name, the target event hub connection string.

## How to run the application

1. Open the project folder
2. Build the app `dotnet build`
3. Run the app using `dotnet .\bin\Debug\netcoreapp3.1\LoadTestEvH.dll <jsonFileLocation> <evhName> <evhConnString>`
	- `jsonFileLocation: location of json file that wanted to publish to an event hub, example and accepted structure could be seen at SampleFile1.json`
	- `evhName: event hub name`
	- `evhConnString: connection string of the event hub namespace`

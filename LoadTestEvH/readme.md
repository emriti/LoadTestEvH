# Load Testing for Azure Event Hub Namespace
This is a tool that can be used to publish bulk data to an event hub, this application need json file for source data, the target event hub name, the target event hub connection string.

## Prerequisites
- Azure Event Hubs Namespace
- .Net Core 3.1
- File editor for formatting json file
- Terminal for running `dotnet`

## How to run the application

1. Open the project folder
2. Build the app using: `dotnet build`
3. Run the app using: `dotnet .\bin\Debug\netcoreapp3.1\LoadTestEvH.dll <jsonFileLocation> <evhName> <evhConnString>`
	- `jsonFileLocation: location of a json file that desired to be published to an event hub. Example and accepted structure could be seen at SampleFile1.json`
	- `evhName: event hub name`
	- `evhConnString: connection string of the event hub namespace`

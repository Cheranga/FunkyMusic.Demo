## **Funky Music Demo**
:innocent:
>*This is an API developed using Azure functions (V3) to connect to MusicBrainz and to return artist and their associated recordings.*

:innocent:

### Architecture and Design

The design I have used in here is the clean architecture design principles. Please see below for how the solution and its modules are organized.

![alt text](https://github.com/Cheranga/FunkyMusic.Demo/blob/master/Images/DependenciesGraph.png "Project Structure")

### **API Layer (FunkyMusic.Demo.Api)**

I have used Azure functions to implement the API.

* All the Azure functions are in V3.

* Please create a `local.settings.json` file once you clone and then add the below content to the file

```json
{
  "IsEncrypted": false,
  "Values": {
    "MusicSearchConfig:Url": "https://musicbrainz.org/ws/2",
    "MusicSearchConfig:MinConfidenceForArtistFilter": 85,
    "MusicSearchConfig:ApplicationId": "PostmanRuntime/7.26.10",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  }
}
```

**Available Azure Functions**

There are three Azure functions implemented here.

> GetArtistByNameFunction

This will return a matching list of artists according to the search criteria.

A sample cURL command to call the service locally,
```
curl --location --request GET 'http://localhost:7071/api/music/search/artist?name=eminem' \
--header 'correlationId: 666'
```

> GetRecordsForArtistByIdFunction

This will return the recordings which are associated with a given artist id.

A sample cURL command to call the service locally,
```
curl --location --request GET 'http://localhost:7071/api/music/search/records/artist/b95ce3ff-3d05-4e87-9e01-c97b66af13d4' \
--header 'correlationId: 666'
```

> SearchArtistByNameFunction

This was designed and implemented as a BFF function, because from the description of the task this fit more likely like an orchestration between multiple services.

A sample cURL command to call the service locally,
```
curl --location --request GET 'http://localhost:7071/api/myapp/music/artist?name=eminem' \
--header 'correlationId: 666'
```

**Mediators**

The `Mediator` pattern has been used to design and implement the abstractions and implementations here. Note that defining abstractions explictly have been avoided here but still the same level of abstraction has been achieved through a generic interface. For that have used `mediatr` library. Also all the `mediator instances` implement throughout the solution goes through a `validation pipeline` and a `performance data logging` pipeline. These pipeline setups are done one time and all the mediator implementations can benefit from them (more details in the `Domain` layer).

```CSharp
public class SearchArtistByNameHandler : IRequestHandler<SearchArtistByNameRequestDto, Result<SearchArtistByNameResponseDto>>
{
  // omitted for brevity.  
}

public class SearchRecordsForArtistIdHandler : IRequestHandler<SearchRecordsForArtistByIdRequestDto, Result<SearchRecordsForArtistByIdResponseDto>>
{
    // omitted for brevity.
}
```

**Specific response formatters**

Each API endpoint will return a response which will be unique (somewhat) to its service. This responsilibity of returning an HTTP response has been separated from the main functionality.

```CSharp
public interface IResponseFormatter<T> where T : class
{
  IActionResult GetActionResult(Result<T> result);
}

public class SearchArtistByNameResponseDtoFormatter : IResponseFormatter<SearchArtistByNameResponseDto>
{
}

public class SearchRecordsForArtistByIdResponseDtoFormatter : IResponseFormatter<SearchRecordsForArtistByIdResponseDto>
{
}
```

**Validators**

For each request, specific validators have been implemented. They are executed through the validation pipeline implemented in the `Domain` module (`ValidationBehaviour<TRequest, TResponse>`) which is in the `CORE` layer. Have used `FluentValidation` nuget package here.

**API Documentation**

API documentation for the HTTP triggered Azure functions are implemented. This will be beneficial for us to hookup this under an APIM or to simply refer the API endpoints request and response structures. Both UI and JSON document endpoints are available.

```http
http://localhost:7071/api/swagger/ui
http://localhost:7071/api/openapi/{version}.{extension}
```
----

### **Domain (FunkyMusic.Demo.Domain)**
This is part of the `CORE` layer of the system. This module contains the domain specific abstractions, implementations and models which are not dependent on any other layer.

**Validator Pipeline**

A domain specific validation pipeline has been implemented so that the respective validator will be executed prior to the actual action. In this way all the validator executions can be seamlined as part of a single execution pipeline and only if the validation is successful, the next action in the pipeline will be carried out.

```CSharp
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : IValidatable
    {
        private readonly ILogger<ValidationBehaviour<TRequest, TResponse>> _logger;
        private readonly IValidator<TRequest> _validator;

        public ValidationBehaviour(IValidator<TRequest> validator, ILogger<ValidationBehaviour<TRequest, TResponse>> logger)
        {
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result<TResponse>> next)
        {
            if (_validator == null)
            {
                return await next();
            }

            var requestType = typeof(TRequest).Name;

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid)
            {
                _logger.LogInformation("Validation successful for {correlationId} in {dtoRequest}", request.CorrelationId, requestType);

                var operation = await next();
                return operation;
            }

            _logger.LogWarning("Validation error occured for {correlationId} in {dtoRequest} with errors: {@errors}", request?.CorrelationId,  requestType, validationResult?.Errors);
            return Result<TResponse>.Failure(ErrorCodes.ValidationError, validationResult);
        }
    }
```

**Performance pipeline**

This is more into logging and to check how fast the actions are carried out through the mediator instances. This will be helpful to identify performance bottlenecks in the code.

```CSharp
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : IValidatable
    {
        private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;

        public PerformanceBehaviour(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result<TResponse>> next)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var response = await next();

            stopWatch.Stop();

            _logger.LogInformation("{request} with {correlationId} ended, time taken {timeTaken} ms.", typeof(TRequest).Name, request?.CorrelationId, stopWatch.ElapsedMilliseconds);

            return response;
        }
    }
```

Both of these pipelines will be registered at the startup of the application.

### **Application (FunkyMusic.Demo.Application)**
This contains application level request/responses and implementations.

----
### **Infrastructure Layer**
This contain the actual implementations where the system will be connecting with the external music service to request relevant information.

**Typed HTTP Client**

A typed HTTP client implementation has been done to get artist and recordings data from the `MusicBrainz API`.

```CSharp
    public interface IMusicSearchApiClient
    {
        Task<Result<MusicArtistSearchResponseDto>> SearchArtistsByNameAsync(string artistName);
        Task<Result<MusicArtistRecordSearchResponseDto>> GetRecordsForArtistByIdAsync(string artistId);
    }

    internal class MusicSearchApiClient : IMusicSearchApiClient
    {
      public MusicSearchApiClient(HttpClient httpClient, MusicSearchConfig musicSearchConfig, ILogger<MusicSearchApiClient> logger)
        {
            _httpClient = httpClient;
            _musicSearchConfig = musicSearchConfig;
            _logger = logger;
        }
      // Omitted for brevity...
    }
```
----

### **Tests**

* Followed a BDD style when implementing the test cases. Also for each layer a specific test project is available.

* Have used `Bddfy` package here to organize the test cases and also it'll generate a test result file automatically once the test cases are executed. Here's a part of the report for the `API` project.

![alt text](https://github.com/Cheranga/FunkyMusic.Demo/blob/master/Images/BddfyReport.png "BDD report for API project")

* The current test coverage of the solution is as below,

![alt text](https://github.com/Cheranga/FunkyMusic.Demo/blob/master/Images/CodeCoverage.png "Test coverage")


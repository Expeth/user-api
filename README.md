<div>
    <h2 align=center> User API</h2>
    <p align="center">
        <img align=center src="https://circleci.com/gh/Expeth/user-api/tree/dev.svg?style=shield&circle-token=80c25fde688730c4dbc393ffeab9a12d6e9af00c"/>
        <img align=center src="https://img.shields.io/badge/semantic--release-angular-e10079?logo=semantic-release"/>
    </p>
</div>

## What is this project about

This simple WebAPI provides the registration and authentication functionalities. Was created just for experimental and education purposes. The Authentication API are based on JWT with assymentic RSA encryption. Private/public .pem files you can find in the .keys directory. The project isn't ideal and has a lot of stuff to improve. Periodically, I update it with new features/bugfixes/refactorings and performance improvements.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

Options to build and run the application:

```
1. Docker/Kubernetes
2. .Net 5 SDK
```

### Run with Docker Compose
Assuming, you're located in the root folder, run the next commands:
```
cd docker-compose
docker-compose up --build
```

### Run with Kubernetes
If you use local Kubernetes cluster, you may use already defined manifests from .k8s folder to run the application. To build yaml files, Kustomize is used. To access API endpoints, you need to have NGINX ingress controller to be installed and the hosts file updated to use the DNS name. For more info about NGINX ingress controller visit https://kubernetes.github.io/ingress-nginx/

Add these lines to your hosts file:
```
# For local UserAPI testing
127.0.0.1	local-api.user.com
127.0.0.1	local-api.mongo.com
```
With local-api.user.com you'll be able to access the API itself, and with local-api.mongo.com - mongo-express.

Commands to start the whole infra:
```
cd .k8s
kubectl apply -k .
```

### Run with .Net 5 SDK
If you have .Net 5 SDK installed, your can build and run the project on .Net platform. For this, use VisualStudio/Rider or the next CLI commands:
```
dotnet restore .
dotnet publish src/UserAPI.Host/ -c Release -o out
dotnet out/UserAPI.Host.dll
```
Please NOTE, that in this case you also need to run MongoDB manually and update the configuration file (appsettings.<ENV>.json) with the right connection string.

### E2E testing
This project has E2E tests written with NUnit framework. They are included to CI pipeline. To run them locally you should use docker-compose. Here are the CLI commands you need to run in order to start tests execution:
```
cd docker-compose
docker-compose -f docker-compose.yaml -f docker-compose-e2e-tests.yaml up --build
```

After they finished, clean up the local env with:
```
docker-compose -f docker-compose.yaml -f docker-compose-e2e-tests.yaml down
```

## Built With

* [Swagger](https://swagger.io/) - The web UI for API endpoints calls
* [.Net 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) - Platform
* [Serilog](https://serilog.net/) - Used for logging
* [MediatR](https://github.com/jbogard/MediatR/wiki) - Used for requests handling
* [MongoDB](https://www.mongodb.com/) - DB for storing all data

## Authors

* **Kovetskiy Valeriy** - *Initial work* - [telegram](https://t.me/kovetskiy)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

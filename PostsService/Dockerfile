FROM mcr.microsoft.com/dotnet/sdk:7.0 as builder
WORKDIR /src

COPY PostsService ./PostsService/
COPY PostsContracts ./PostsContracts/

WORKDIR /src/PostsService
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=builder /src/PostsService/out .
ENTRYPOINT ["dotnet", "PostsService.dll"]
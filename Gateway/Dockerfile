FROM mcr.microsoft.com/dotnet/sdk:7.0 as builder
WORKDIR /src

COPY Gateway ./Gateway/
COPY CommentsContracts ./CommentsContracts/
COPY PostsContracts ./PostsContracts/

WORKDIR /src/Gateway
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=builder /src/Gateway/out .
ENTRYPOINT ["dotnet", "Gateway.dll"]
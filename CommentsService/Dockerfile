FROM mcr.microsoft.com/dotnet/sdk:7.0 as builder
WORKDIR /src

COPY CommentsService ./CommentsService/
COPY CommentsContracts ./CommentsContracts/

WORKDIR /src/CommentsService
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=builder /src/CommentsService/out .
ENTRYPOINT ["dotnet", "CommentsService.dll"]
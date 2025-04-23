FROM mcr.microsoft.com/dotnet/sdk:10.0.100-preview.3 AS build
WORKDIR /app
COPY ./packages /app/packages/.
COPY . .
RUN dotnet nuget remove source local
RUN dotnet nuget add source /app/packages -n local
RUN dotnet build -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:10.0.0-preview.3-alpine3.21 AS runtime
WORKDIR /app
COPY --from=build /app/build .

EXPOSE 8080

CMD ["dotnet", "MyApi.dll"]
FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /src
COPY . .
RUN dotnet restore ./src/sln/Notary.sln -v q --no-cache
RUN dotnet build ./src/sln/Notary.sln -v q -c Release --no-restore --nologo
RUN dotnet publish ./src/api/Notary.Api.csproj -o /publish --no-restore --nologo --no-build -c Release -v q

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
ENV AzureAd__Domain ""
ENV AzureAd__ClientSecret ""
ENV AzureAd__ClientId ""
ENV AzureAd__TenantId ""
ENV AzureAd__Domain ""
ENV Notary__ApplicationKey ""
ENV Notary__Database__ConnectionString ""
ENV Notary__Database__DatabaseName "notary"
ENV Notary__Database__Username ""
ENV Notary__Database__Password ""
WORKDIR /publish
COPY --from=build /publish .
EXPOSE 80
ENTRYPOINT [ "dotnet", "Notary.Api.dll" ]
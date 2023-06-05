FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build

#-c|--configuration {Debug|Release}
ARG build_configuration=Release

#-v|--verbosity <LEVEL>
ARG build_verbosity=m

COPY ./src /src

WORKDIR /src/HR.Vision.Gateway.Api

RUN dotnet restore -s https://api.nuget.org/v3/index.json -v m HR.Vision.Gateway.Api.csproj

RUN dotnet publish --no-restore -c $build_configuration -v $build_verbosity -o /out HR.Vision.Gateway.Api.csproj

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS runtime

EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT="" \
    ASPNETCORE_URLS="http://0.0.0.0:8080" \
    DOTNET_RUNNING_IN_CONTAINER="true"

WORKDIR /app
COPY --from=build /out /app

RUN mkdir -p ~/.postgresql && \
    wget "https://storage.yandexcloud.net/cloud-certs/CA.pem" -O ~/.postgresql/root.crt && \
    chmod 0600 ~/.postgresql/root.crt


ENTRYPOINT ["dotnet", "HR.Vision.Gateway.Api.dll"]
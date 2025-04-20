# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG ENV
ARG APP_VERSION

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080



# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS with-node
RUN apt-get update
RUN apt-get install curl -y
RUN curl -sL https://deb.nodesource.com/setup_22.x | bash
RUN apt-get -y install nodejs
RUN npm install -g @angular/cli

FROM with-node AS build
ARG ENV
ARG BUILD_CONFIGURATION=Release
ARG APP_VERSION
WORKDIR /src
COPY ["NS.Quizzy.Server/NS.Quizzy.Server.csproj", "NS.Quizzy.Server/"]
COPY ["ns.quizzy.client/ns.quizzy.client.esproj", "ns.quizzy.client/"]
COPY ["NS.Quizzy.Server.BL/NS.Quizzy.Server.BL.csproj", "NS.Quizzy.Server.BL/"]
COPY ["NS.Quizzy.Server.DAL/NS.Quizzy.Server.DAL.csproj", "NS.Quizzy.Server.DAL/"]
COPY ["NS.Quizzy.Server.Common/NS.Quizzy.Server.Common.csproj", "NS.Quizzy.Server.Common/"]
COPY ["nuget.config", "nuget.config"]
ENV ASPNETCORE_ENVIRONMENT=$ENV
RUN dotnet restore "./NS.Quizzy.Server/NS.Quizzy.Server.csproj"
COPY . .
WORKDIR "/src/NS.Quizzy.Server"
RUN dotnet build "./NS.Quizzy.Server.csproj" -c $BUILD_CONFIGURATION -p:Version=$APP_VERSION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG ENV
ARG BUILD_CONFIGURATION=Release
ARG APP_VERSION
ENV ASPNETCORE_ENVIRONMENT=$ENV
RUN dotnet publish "./NS.Quizzy.Server.csproj" -c $BUILD_CONFIGURATION -p:Version=$APP_VERSION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
ARG ENV
ENV ASPNETCORE_ENVIRONMENT=$ENV
ENV TZ=Asia/Jerusalem
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NS.Quizzy.Server.dll"]

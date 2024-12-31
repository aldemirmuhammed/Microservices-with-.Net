#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Notification/FreeCourse.Notification.API/FreeCourse.Notification.API.csproj", "Services/Notification/FreeCourse.Notification.API/"]
COPY ["Shared/FreeCourse.EventBus.Messages/FreeCourse.EventBus.Messages.csproj", "Shared/FreeCourse.EventBus.Messages/"]
COPY ["Shared/FreeCourse.Shared/FreeCourse.Shared.csproj", "Shared/FreeCourse.Shared/"]
RUN dotnet restore "./Services/Notification/FreeCourse.Notification.API/./FreeCourse.Notification.API.csproj"
COPY . .
WORKDIR "/src/Services/Notification/FreeCourse.Notification.API"
RUN dotnet build "./FreeCourse.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FreeCourse.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FreeCourse.Notification.API.dll"]
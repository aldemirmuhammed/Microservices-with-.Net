#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Notification/FreeCourse.Notification.API/FreeCourse.Notification.API.csproj", "Services/Notification/FreeCourse.Notification.API/"]
COPY ["Services/Order/FreeCourse.Services.Order.API/FreeCourse.Services.Order.API.csproj", "Services/Order/FreeCourse.Services.Order.API/"]
COPY ["Services/Order/FreeCourse.Services.Order.Application/FreeCourse.Services.Order.Application.csproj", "Services/Order/FreeCourse.Services.Order.Application/"]
COPY ["Shared/FreeCourse.Shared/FreeCourse.Shared.csproj", "Shared/FreeCourse.Shared/"]
COPY ["Services/Order/FreeCourse.Services.Order.Domain/FreeCourse.Services.Order.Domain.csproj", "Services/Order/FreeCourse.Services.Order.Domain/"]
COPY ["Services/Order/FreeCourse.Services.Order.Domain.Core/FreeCourse.Services.Order.Domain.Core.csproj", "Services/Order/FreeCourse.Services.Order.Domain.Core/"]
COPY ["Services/Order/FreeCourse.Services.Order.Infrastructure/FreeCourse.Services.Order.Infrastructure.csproj", "Services/Order/FreeCourse.Services.Order.Infrastructure/"]
RUN dotnet restore "./Services/Order/FreeCourse.Services.Order.API/./FreeCourse.Services.Order.API.csproj"
COPY . .
WORKDIR "/src/Services/Order/FreeCourse.Services.Order.API"
RUN dotnet build "./FreeCourse.Services.Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FreeCourse.Services.Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FreeCourse.Services.Order.API.dll"]
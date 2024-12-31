#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Discount/FreeCourse.Services.Discount/FreeCourse.Services.Discount.csproj", "Services/Discount/FreeCourse.Services.Discount/"]
COPY ["Shared/FreeCourse.Shared/FreeCourse.Shared.csproj", "Shared/FreeCourse.Shared/"]
RUN dotnet restore "./Services/Discount/FreeCourse.Services.Discount/./FreeCourse.Services.Discount.csproj"
COPY . .
WORKDIR "/src/Services/Discount/FreeCourse.Services.Discount"
RUN dotnet build "./FreeCourse.Services.Discount.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FreeCourse.Services.Discount.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FreeCourse.Services.Discount.dll"]
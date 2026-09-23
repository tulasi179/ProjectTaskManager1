# ---- Stage 1: Build ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Projecttaskmanager.csproj", "./"]
RUN dotnet restore "Projecttaskmanager.csproj"

COPY . .
RUN dotnet publish "Projecttaskmanager.csproj" -c Release -o /app/publish

# ---- Stage 2: Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "Projecttaskmanager.dll"]
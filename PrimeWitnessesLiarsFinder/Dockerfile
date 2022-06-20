FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PrimeWitnessesLiarsFinder/PrimeWitnessesLiarsFinder.csproj", "PrimeWitnessesLiarsFinder/"]
RUN dotnet restore "PrimeWitnessesLiarsFinder/PrimeWitnessesLiarsFinder.csproj"
COPY . .
WORKDIR "/src/PrimeWitnessesLiarsFinder"
RUN dotnet build "PrimeWitnessesLiarsFinder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PrimeWitnessesLiarsFinder.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PrimeWitnessesLiarsFinder.dll"]

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS runtime
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgdiplus libc6-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
RUN apt-get update && apt-get install -y libgdiplus

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MLMicroservice/MLMicroservice/MLMicroservice.csproj.user", "MLMicroservice/"]
RUN dotnet restore "MLMicroservice/MLMicroservice/MLMicroservice.csproj.user"
COPY . .
WORKDIR "/src/MLMicroservice"
RUN dotnet build "MLMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MLMicroservice.csproj.user" -c Release -o /app/publish
RUN cp ./MLModel.zip /src/publish

FROM base AS final
WORKDIR /app
# ENTRYPOINT ["dotnet", "HandwritingRecognition.dll"]
# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet MLMicroservice.dll
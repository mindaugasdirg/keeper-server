FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster
WORKDIR /app

# copy published app
COPY ./output .

ENV ASPNETCORE_URLS=http://0.0.0.0:PORT
CMD dotnet Keeper.WebApi.dll --urls http://*:$PORT
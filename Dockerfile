FROM mcr.microsoft.com/dotnet/aspnet:6.0

ARG DLL_NAME=*.dll
ENV APPLICATION=$DLL_NAME

WORKDIR /app
COPY out .

RUN echo "dotnet $DLL_NAME" >> entrypoint.sh

ENTRYPOINT ["/bin/sh", "entrypoint.sh"]
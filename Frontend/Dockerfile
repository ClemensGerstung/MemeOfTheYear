FROM fedora:41

RUN set -xe \
    && yum update -y \
    && dnf install dotnet-sdk-9.0 -y \
    && dnf install aspnetcore-runtime-9.0 -y

RUN set -xe \
    mkdir -o /tmp/Frontend

WORKDIR /tmp/Frontend
ADD MemeOfTheYear.proto /tmp/
ADD Frontend/ /tmp/Frontend/

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

ENV DOTNET_ROOT=/root/.dotnet
ENV PATH=$PATH:$DOTNET_ROOT

RUN set -xe \
    && dotnet publish \
    && rm -r ./bin/Debug

CMD ["dotnet", "run"]
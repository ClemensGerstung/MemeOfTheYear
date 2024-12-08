FROM fedora:41

RUN set -xe \
    && mkdir -p /tmp/backend \
    && yum update -y \
    && yum install dotnet-sdk-9.0 aspnetcore-runtime-9.0 -y

WORKDIR /tmp/backend
ADD MemeOfTheYear.proto /tmp/
ADD backend/ /tmp/backend/

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

RUN set -xe \
    && export PATH=$PATH:/root/.dotnet/tools \
    && dotnet tool install --global dotnet-ef \
    && chmod +x entry_point.sh

CMD ["/tmp/backend/entry_point.sh"]
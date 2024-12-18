FROM fedora:41

RUN set -xe \
    && mkdir -p /tmp/Backend.Application \
    && mkdir -p /tmp/Backend.Library \
    && yum update -y \
    && yum install dotnet-sdk-9.0 aspnetcore-runtime-9.0 -y

WORKDIR /tmp/Backend.Application

ADD MemeOfTheYear.proto /tmp/
ADD Backend.Library/ /tmp/Backend.Library/
ADD Backend.Application/ /tmp/Backend.Application/

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

RUN set -xe \
    && export PATH=$PATH:/root/.dotnet/tools \
    && dotnet tool install --global dotnet-ef \
    && dotnet build \
    && chmod +x entry_point.sh

CMD ["/tmp/Backend.Application/entry_point.sh"]
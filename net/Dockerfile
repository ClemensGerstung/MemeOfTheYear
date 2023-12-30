FROM ubuntu:22.04


RUN set -xe \
    && apt update \
    && apt install -y wget \
    && wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh \
    && chmod +x ./dotnet-install.sh \
    && ./dotnet-install.sh --channel 8.0 \
    && ./dotnet-install.sh --channel 8.0 --runtime aspnetcore

WORKDIR /tmp

ADD *.cs /tmp/
ADD *.proto /tmp/
ADD net.csproj /tmp/
ADD net.sln /tmp/
ADD appsettings.json /tmp/
ADD Server/ /tmp/Server/

ENV DOTNET_ROOT=/root/.dotnet
ENV PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV MEME_OF_THE_YEAR_DB=/share/db
ENV MEME_OF_THE_YEAR_IMAGES=/share/images
ENV MEME_OF_THE_YEAR_QUESTIONS=/share/questions.json

RUN set -xe \
    && mkdir -p $MEME_OF_THE_YEAR_DB \
    && dotnet tool install --global dotnet-ef \
    && dotnet ef migrations add InitialCreate \
    && dotnet ef database update \
    && dotnet publish

RUN set -xe \
    && rm -r ./bin/Debug \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 8081

CMD ["./bin/Release/net8.0/net"]
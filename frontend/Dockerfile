FROM node:20 as build

ARG GRPC_HOST
ENV GRPC_HOST=$GRPC_HOST

WORKDIR /tmp/frontend/

ADD memeoftheyear.proto /tmp/
ADD frontend/src/ /tmp/frontend/src/
ADD frontend/package.json /tmp/frontend/
ADD frontend/webpack.config.js /tmp/frontend/

ADD --chmod=700 https://github.com/grpc/grpc-web/releases/download/1.5.0/protoc-gen-grpc-web-1.5.0-linux-x86_64 /tmp/protoc-gen-grpc-web

RUN yarn install

RUN set -xe \
    && npm install -g grpc-tools \
    && export NPM_GLOBAL_ROOT=$(npm root -g) \
    && export PATH=$PATH:$NPM_GLOBAL_ROOT/grpc-tools/bin:/tmp/ \
    && export OUT_DIR=protogen \
    && mkdir -p $OUT_DIR \
    && protoc -I=../ memeoftheyear.proto --js_out=import_style=commonjs:$OUT_DIR \
    && protoc -I=../ memeoftheyear.proto --grpc-web_out=import_style=commonjs,mode=grpcwebtext:$OUT_DIR

# COPY . .

RUN yarn build

FROM nginx:latest

ADD frontend/css/ /usr/share/nginx/html/css/
ADD frontend/js/ /usr/share/nginx/html/js/
ADD frontend/index.html /usr/share/nginx/html/
ADD frontend/robots.txt /usr/share/nginx/html/

COPY --from=build /tmp/frontend/dist /usr/share/nginx/html/dist

EXPOSE 80
FROM node:20 as build

ARG GRPC_HOST
ENV GRPC_HOST=$GRPC_HOST

ADD ./src/ /tmp/src/
COPY ./package.json /tmp/
COPY ./webpack.config.js /tmp/

WORKDIR /tmp

RUN yarn install

COPY . .

RUN yarn build

FROM nginx:latest

ADD css/ /usr/share/nginx/html/css/
ADD js/ /usr/share/nginx/html/js/
ADD index.html /usr/share/nginx/html/
ADD robots.txt /usr/share/nginx/html/

COPY --from=build /tmp/dist /usr/share/nginx/html/dist

EXPOSE 80
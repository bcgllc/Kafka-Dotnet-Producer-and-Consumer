FROM microsoft/dotnet:latest

LABEL maintainer="Jonathan Burris"

WORKDIR /app
RUN pwd

# RUN dotnet publish -c Release -o dist from the terminal before buidling this docker image
COPY ./dist ./
COPY entrypoint.sh ./

VOLUME [ "/data" ]

ENTRYPOINT [ "./entrypoint.sh" ]
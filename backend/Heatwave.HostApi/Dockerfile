#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001
COPY . .
ENV LANG C.UTF-8
# RUN /bin/cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime && echo 'Asia/Shanghai' >/etc/timezone
#RUN apt-get update \
#    && apt-get install -y libfontconfig
ENTRYPOINT ["dotnet", "Heatwave.HostApi.dll"]
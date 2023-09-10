FROM mcr.microsoft.com/dotnet/aspnet:6.0

ARG PROJECT_NAME

WORKDIR /app
COPY publish/${PROJECT_NAME} ./

RUN adduser --disabled-password dmuser
USER dmuser

ENV RUNTIME_PROJECT ${PROJECT_NAME}.dll
CMD dotnet ${RUNTIME_PROJECT}

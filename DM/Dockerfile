FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ARG PROJECT_NAME
ARG PROJECT_DIR

WORKDIR /app

# copy csproj and restore as distinct layers
COPY . ./
RUN dotnet publish ${PROJECT_DIR}/${PROJECT_NAME} -c Release -o out

FROM tkhoa2711/debian-curl AS trace-install

WORKDIR /datadog

ARG TRACER_VERSION=2.21.0
RUN mkdir -p /var/log/datadog
RUN mkdir -p /opt/datadog
RUN curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb
RUN dpkg -i ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

ARG PROJECT_NAME

WORKDIR /app
COPY --from=build /app/out ./
COPY --from=trace-install /opt/datadog ./datadog

RUN adduser --disabled-password dmuser
USER dmuser

ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/app/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_DOTNET_TRACER_HOME=/app/datadog

ENV RUNTIME_PROJECT ${PROJECT_NAME}.dll
CMD dotnet ${RUNTIME_PROJECT}
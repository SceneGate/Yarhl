# TODO: Move to latest image after PleOps.Cake support Ubuntu 20.04
# https://github.com/pleonex/PleOps.Cake/issues/35
FROM mcr.microsoft.com/vscode/devcontainers/dotnetcore:0.148.1-3.1

# Install .NET 5.0
RUN apt-get update \
    && export DEBIAN_FRONTEND=noninteractive \
    && apt-get install -y apt-transport-https \
    && wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-sdk-5.0

# Install Mono (soon won't be necessary (.NET 6?))
RUN apt install gnupg ca-certificates \
    && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF \
    && echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" >> /etc/apt/sources.list.d/mono-official-stable.list \
    && apt update \
    && export DEBIAN_FRONTEND=noninteractive \
    && apt install -y mono-devel

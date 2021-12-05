FROM mcr.microsoft.com/vscode/devcontainers/dotnet:6.0

# Install Mono (for DocFX)
RUN apt install -y apt-transport-https dirmngr gnupg ca-certificates \
    && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF \
    && echo "deb https://download.mono-project.com/repo/debian stable-buster main" >> tee /etc/apt/sources.list.d/mono-official-stable.list \
    && apt update \
    && export DEBIAN_FRONTEND=noninteractive \
    && apt install -y mono-devel

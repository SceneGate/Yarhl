FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env
WORKDIR /app

# Install mono
RUN apt-get update -y \
    # Add mono repo
    && apt-get install -y apt-transport-https dirmngr gnupg ca-certificates \
    && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF \
    && echo "deb https://download.mono-project.com/repo/debian stable-stretch main" | tee /etc/apt/sources.list.d/mono-official-stable.list \
    && apt-get update -y \
    # Install mono
    && apt-get install -y mono-devel \
    # Clean temporal files
    && rm -rf /var/lib/apt/lists/* /tmp/*

# Copy the whole project and build
COPY ./ ./
RUN ./build.sh --target="CI-Linux"

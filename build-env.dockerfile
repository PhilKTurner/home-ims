FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy

# Setup NodeJs
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y build-essential nodejs
# End setup

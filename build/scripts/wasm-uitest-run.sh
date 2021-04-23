#!/bin/bash
set -euo pipefail
IFS=$'\n\t'


cd $BUILD_SOURCESDIRECTORY

msbuild /r /p:Configuration=Release $BUILD_SOURCESDIRECTORY/e2e/Uno/HelloUnoWorld.UITests/HelloUnoWorld.UITests.csproj
dotnet build /p:Configuration=Release /p:DISABLE_GITVERSIONING=true $BUILD_SOURCESDIRECTORY/e2e/Uno/$TARGET_XAML_FLAVOR/HelloUnoWorld.Wasm/HelloUnoWorld.Wasm.csproj

# copy build app to artifacts
mkdir -p $BUILD_ARTIFACTSTAGINGDIRECTORY/e2e/uno/$TARGET_XAML_FLAVOR/site
cp -r $BUILD_SOURCESDIRECTORY/e2e/Uno/$TARGET_XAML_FLAVOR/HelloUnoWorld.Wasm/bin/Release/net5.0/dist/* $BUILD_ARTIFACTSTAGINGDIRECTORY/e2e/uno/$TARGET_XAML_FLAVOR/site

cd $BUILD_SOURCESDIRECTORY/build

npm i chromedriver@86.0.0
npm i puppeteer@5.3.1

# install dotnet serve
dotnet tool install dotnet-serve --version 1.8.15 --tool-path $BUILD_SOURCESDIRECTORY/build/tools
export PATH="$PATH:$BUILD_SOURCESDIRECTORY/build/tools"

wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
mono nuget.exe install NUnit.ConsoleRunner -Version 3.10.0

export UNO_UITEST_TARGETURI=http://localhost:8000
export UNO_UITEST_DRIVERPATH_CHROME=$BUILD_SOURCESDIRECTORY/build/node_modules/chromedriver/lib/chromedriver
export UNO_UITEST_CHROME_BINARY_PATH=$BUILD_SOURCESDIRECTORY/build/node_modules/puppeteer/.local-chromium/linux-800071/chrome-linux/chrome
export UNO_UITEST_SCREENSHOT_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/e2e/uno/$TARGET_XAML_FLAVOR/wasm
export UNO_UITEST_PLATFORM=Browser
export UNO_UITEST_CHROME_CONTAINER_MODE=true

dotnet serve -p 8000 -d "$BUILD_SOURCESDIRECTORY/e2e/Uno/$TARGET_XAML_FLAVOR/HelloUnoWorld.Wasm/bin/Release/net5.0/dist/" &

mkdir -p $UNO_UITEST_SCREENSHOT_PATH

mono $BUILD_SOURCESDIRECTORY/build/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe \
--trace=Verbose \
--inprocess \
--agents=1 \
--workers=1 \
$BUILD_SOURCESDIRECTORY/e2e/Uno/HelloUnoWorld.UITests/bin/Release/net47/HelloUnoWorld.UITests.dll || true

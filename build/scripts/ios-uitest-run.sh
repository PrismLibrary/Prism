#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

echo "Lising iOS simulators"
xcrun simctl list devices --json

/Applications/Xcode.app/Contents/Developer/Applications/Simulator.app/Contents/MacOS/Simulator &

cd $BUILD_SOURCESDIRECTORY
msbuild /r /p:Configuration=Release $BUILD_SOURCESDIRECTORY/e2e/Uno/HelloUnoWorld.UITests/HelloUnoWorld.UITests.csproj
msbuild /r /p:Configuration=Release "/p:Platform=iPhoneSimulator" $BUILD_SOURCESDIRECTORY/e2e/Uno/$TARGET_XAML_FLAVOR/HelloUnoWorld.iOS/HelloUnoWorld.iOS.csproj

cd $BUILD_SOURCESDIRECTORY/build

wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
mono nuget.exe install NUnit.ConsoleRunner -Version 3.10.0

export UNO_UITEST_PLATFORM=iOS
export UNO_UITEST_IOSBUNDLE_PATH=$BUILD_SOURCESDIRECTORY/e2e/Uno/$TARGET_XAML_FLAVOR/HelloUnoWorld.iOS/bin/iPhoneSimulator/Release/HelloUnoWorld.app
export UNO_UITEST_SCREENSHOT_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/e2e/uno/$TARGET_XAML_FLAVOR/ios

mkdir -p $UNO_UITEST_SCREENSHOT_PATH

mono $BUILD_SOURCESDIRECTORY/build/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe \
--inprocess \
--agents=1 \
--workers=1 \
$BUILD_SOURCESDIRECTORY/e2e/Uno/HelloUnoWorld.UITests/bin/Release/net47/HelloUnoWorld.UITests.dll || true

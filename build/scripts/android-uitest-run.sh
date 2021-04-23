#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

# Install AVD files
echo "y" | $ANDROID_HOME/tools/bin/sdkmanager --install 'system-images;android-28;google_apis_playstore;x86'

# Create emulator
echo "no" | $ANDROID_HOME/tools/bin/avdmanager create avd -n xamarin_android_emulator -k 'system-images;android-28;google_apis_playstore;x86' --force

echo $ANDROID_HOME/emulator/emulator -list-avds

echo "Starting emulator"

# Start emulator in background
nohup $ANDROID_HOME/emulator/emulator -avd xamarin_android_emulator -no-snapshot > /dev/null 2>&1 &

# build the sample, while the emulator is starting
msbuild /r /p:Configuration=Release $BUILD_SOURCESDIRECTORY/e2e/Uno/HelloUnoWorld.UITests/HelloUnoWorld.UITests.csproj
msbuild /r /p:Configuration=Release $BUILD_SOURCESDIRECTORY/e2e/Uno/$TARGET_XAML_FLAVOR/HelloUnoWorld.Droid/HelloUnoWorld.Droid.csproj

# Wait for the emulator to finish booting
$ANDROID_HOME/platform-tools/adb wait-for-device shell 'while [[ -z $(getprop sys.boot_completed | tr -d '\r') ]]; do sleep 1; done; input keyevent 82'

$ANDROID_HOME/platform-tools/adb devices

echo "Emulator started"

export UNO_UITEST_SCREENSHOT_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/e2e/uno/android
export UNO_UITEST_PLATFORM=Android
export UNO_UITEST_ANDROIDAPK_PATH=$BUILD_SOURCESDIRECTORY/e2e/Uno/$TARGET_XAML_FLAVOR/HelloUnoWorld.Droid/bin/Release/com.prismlibrary.helloworld-Signed.apk

cd $BUILD_SOURCESDIRECTORY/build

wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
mono nuget.exe install NUnit.ConsoleRunner -Version 3.10.0

mkdir -p $UNO_UITEST_SCREENSHOT_PATH

mono $BUILD_SOURCESDIRECTORY/build/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe \
	$BUILD_SOURCESDIRECTORY/e2e/Uno/HelloUnoWorld.UITests/bin/Release/net47/HelloUnoWorld.UITests.dll || true

#!/bin/bash
xcodeRoot=/Applications/Xcode_$XcodeOverride.app
echo "Setting Xcode Override to: $xcodeRoot"

echo '##vso[task.setvariable variable=MD_APPLE_SDK_ROOT;]'$xcodeRoot
sudo xcode-select --switch $xcodeRoot/Contents/Developer
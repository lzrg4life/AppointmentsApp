# iOS Build & App Store Release Steps

.NET 10 MAUI app. iOS builds require a Mac (or a cloud Mac / macOS CI runner).

## 1. Prerequisites (Mac)
- Xcode installed and launched once (accept the license).
- .NET 10 SDK, then: `dotnet workload install maui ios`
- Apple Developer Program membership ($99/year).

## 2. Update `AppointmentsApp/AppointmentsApp.csproj`
- Change `ApplicationId` from `com.companyname.appointmentsapp` to your own bundle ID (e.g. `com.yourname.appointments`).
- Register the same bundle ID in the Apple Developer portal.
- Increment `ApplicationVersion` for every upload (Apple rejects a repeated build number).
- `ApplicationDisplayVersion` (currently `1.0`) is the user-visible version.
- Check `Platforms/iOS/Resources/PrivacyInfo.xcprivacy` declares every required-reason API you use.

## 3. Verify the build
```
cd AppointmentsApp
dotnet build -f net10.0-ios
dotnet build -f net10.0-ios -t:Run -p:_DeviceName=:v2:udid=<simulator-udid>
```
Test in the simulator, then on a real device.

## 4. Signing
- Create an **Apple Distribution** certificate and an **App Store provisioning profile** for the bundle ID (Xcode or developer.apple.com).
- Install both on the Mac.

## 5. Build the release package
```
dotnet publish -f net10.0-ios -c Release \
  -p:ArchiveOnBuild=true \
  -p:RuntimeIdentifier=ios-arm64 \
  -p:CodesignKey="Apple Distribution: Your Name (TEAMID)" \
  -p:CodesignProvision="Your App Store Profile Name"
```
Output: `.ipa` in `bin/Release/net10.0-ios/ios-arm64/publish/`.

## 6. Upload and submit
1. In **App Store Connect**, create the app record with the same bundle ID.
2. Upload the `.ipa` via the **Transporter** app, or:
   `xcrun altool --upload-app -f app.ipa -t ios --apiKey <KEY> --apiIssuer <ISSUER>`
3. Wait for processing; test the build in **TestFlight**.
4. Complete the listing: screenshots, description, privacy policy URL, App Privacy answers, age rating.
5. Select the build and **Submit for Review** (typically 1–3 days).

## No Mac?
Use macOS CI runners (GitHub Actions `macos-latest`, Azure DevOps) or a cloud Mac (MacinCloud). For CI, store the certificate and profile as secrets.

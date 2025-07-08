#define MyAppName "Emulator for Azure App Configuration"
#define MyAppVersion "1.2.1"
#define MyAppPublisher "Thomas Clark"
#define MyAppURL "https://azureappconfigurationemulator.thomasclark.app"
#define MyAppExeName "AzureAppConfigurationEmulator.exe"

[Setup]
AppId={{7D09D62B-09DF-4C36-8736-96FE3BEE4830}
AppName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
AppVersion={#MyAppVersion}
#if Architecture == "arm64"
  ArchitecturesAllowed=arm64
#elif Architecture == "x64"
  ArchitecturesAllowed=x64
#endif
ArchitecturesInstallIn64BitMode=arm64 x64
DefaultDirName={autopf}\{#MyAppName}
LicenseFile=LICENSE
PrivilegesRequiredOverridesAllowed=dialog
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "src\AzureAppConfigurationEmulator\bin\Release\net8.0\win-{#Architecture}\publish\*"; DestDir: "{app}"; Flags: createallsubdirs ignoreversion recursesubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{tmp}\aspnetcore-runtime.exe"; Parameters: "/install /quiet /norestart"; Check: AspNetCoreRuntimeNeedsInstall
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
var
  DownloadPage: TDownloadWizardPage;
function AspNetCoreRuntimeNeedsInstall: Boolean;
var      
  FileName: String;
  Output: AnsiString;
  ResultCode: Integer;
begin
  Result := True;
  FileName := ExpandConstant('{tmp}\dotnet.txt');
  if Exec(ExpandConstant('{cmd}'), '/C dotnet --list-runtimes > "' + FileName + '" 2>&1', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) and (ResultCode = 0) then
    if LoadStringFromFile(FileName, Output) then
      if Pos('Microsoft.AspNetCore.App 8.0', Output) > 0 then
        Result := False;
end;
procedure InitializeWizard;
begin
  DownloadPage := CreateDownloadPage(SetupMessage(msgWizardPreparing), SetupMessage(msgPreparingDesc), nil);
end;
function NextButtonClick(CurPageID: Integer): Boolean;
begin
  if CurPageID = wpReady then
  begin
    DownloadPage.Clear;
    #if Architecture == "arm64"
      DownloadPage.Add('https://download.visualstudio.microsoft.com/download/pr/ee1b10d3-aca1-4ae0-b74a-97bd30fa6d2d/87e7a62e9f5438342b66e673d422cd57/aspnetcore-runtime-8.0.6-win-arm64.exe', 'aspnetcore-runtime.exe', '');
    #elif Architecture == "x64"
      DownloadPage.Add('https://download.visualstudio.microsoft.com/download/pr/38b32fc8-8070-4f14-bd52-65505fddc5ff/50e6cf3b7505eee02c3b3db8ea46ffe3/aspnetcore-runtime-8.0.6-win-x64.exe', 'aspnetcore-runtime.exe', '');
    #endif
    DownloadPage.Show;
    try
      try
        DownloadPage.Download;
        Result := True;
      except
        if not DownloadPage.AbortedByUser then
          SuppressibleMsgBox(AddPeriod(GetExceptionMessage), mbCriticalError, MB_OK, IDOK);
        Result := False;
      end;
    finally
      DownloadPage.Hide;
    end;
  end else
    Result := True;
end;

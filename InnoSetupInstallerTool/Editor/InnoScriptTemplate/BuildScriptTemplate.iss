#define ApplicationVersion "{VERSION}"
#define FileName "{PRODUCT_NAME}"
#define Executable "{PRODUCT_NAME}.exe"
#define InstallerFileName "{PRODUCT_NAME}-v" + ApplicationVersion + "-" + GetDateTimeString('yyyy-mm-dd', '-', ':')
#define CompanyName "{COMPANY_NAME}"
#define CompanyName "{COMPANY_NAME}"
#define CustomSetupIcon "{SETUP_ICON}"     
#define EULAFile "{EULA_FILE}"         

[Setup]
AppId={#FileName}
AppName={#FileName}
AppVersion={#ApplicationVersion}
AppVerName={#FileName}
AppPublisher={#CompanyName}
SetupIconFile={#CustomSetupIcon}
LicenseFile={#EULAFile}
DefaultDirName="{commonpf}\{#CompanyName}\{#FileName}"
DisableProgramGroupPage=yes
OutputDir=.\OutputInstaller\
OutputBaseFilename={#InstallerFileName}
UninstallDisplayIcon={app}\{#Executable}
Compression=lzma
SolidCompression=yes
DirExistsWarning=no
DisableWelcomePage=no
ChangesAssociations=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkablealone

[Files]
Source: "{#BUILD_PATH}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{commonprograms}\{#FileName}"; Filename: "{app}\{#Executable}"
Name: "{commondesktop}\{#FileName}"; Filename: "{app}\{#Executable}"; Tasks: desktopicon

[Registry]

[Run]
Filename: "{app}\{#Executable}"; Description: "{cm:LaunchProgram, {#FileName}}"; Flags: nowait postinstall skipifsilent

[Messages]
SetupWindowTitle=Install - %1 - {#ApplicationVersion}
WelcomeLabel2=This will install [name/ver] on your computer.%n

[Code]
var
  InstalledPath, RegistryKey: String;

function GetShortcutTargetFolder(const ShortcutPath: String): String;
var
  WshShell, Shortcut: Variant;
begin
  Result := '';
  if FileExists(ShortcutPath) then
  begin
    try
      WshShell := CreateOleObject('WScript.Shell');
      Shortcut := WshShell.CreateShortcut(ShortcutPath);
      if FileExists(Shortcut.TargetPath) then
        Result := ExtractFilePath(Shortcut.TargetPath);
    except
      Result := '';
    end;
  end;
end;

function FindShortcutInDirectory(const Dir: String): String;
var
  FindRec: TFindRec;
  ShortcutPath, CandidateTarget: String;
begin
  Result := '';
  if FindFirst(Dir + '\*.lnk', FindRec) then
  begin
    try
      repeat
        ShortcutPath := Dir + '\' + FindRec.Name;
        CandidateTarget := GetShortcutTargetFolder(ShortcutPath);
        if (CandidateTarget <> '') and FileExists(CandidateTarget + '\{#Executable}') then
        begin
          Result := CandidateTarget;
          Exit;
        end;
      until not FindNext(FindRec);
    finally
      FindClose(FindRec);
    end;
  end;
end;


function DetectInstallPath: String;
var
  CandidatePath: String;
begin
  RegistryKey := 'Software\{#CompanyName}\Installer';

  // Registry
  if RegQueryStringValue(HKLM, RegistryKey, 'InstallPath', CandidatePath) then
  begin
    if FileExists(CandidatePath + '\{#Executable}') then
    begin
      //MsgBox('Detected via Registry: ' + CandidatePath, mbInformation, MB_OK);
      Result := CandidatePath;
      Exit;
    end;
  end;

  // Desktop shortcut
  CandidatePath := FindShortcutInDirectory(ExpandConstant('{commondesktop}'));
  if CandidatePath <> '' then
  begin
    //MsgBox('Detected via Desktop Shortcut: ' + CandidatePath, mbInformation, MB_OK);
    Result := CandidatePath;
    Exit;
  end;

  // Start Menu shortcut
  CandidatePath := FindShortcutInDirectory(ExpandConstant('{commonprograms}'));
  if CandidatePath <> '' then
  begin
    //MsgBox('Detected via Start Menu Shortcut: ' + CandidatePath, mbInformation, MB_OK);
    Result := CandidatePath;
    Exit;
  end;

  Result := ExpandConstant('{commonpf}\{#CompanyName}\{#FileName}');
end;

procedure SaveInstallPath(const Path: String);
begin
  RegWriteStringValue(HKLM, RegistryKey, 'InstallPath', Path);
end;

function InitializeSetup: Boolean;
begin
  InstalledPath := DetectInstallPath;
  //MsgBox('Installing or updating in: ' + InstalledPath, mbInformation, MB_OK);
  Result := True;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
    SaveInstallPath(InstalledPath);
end;
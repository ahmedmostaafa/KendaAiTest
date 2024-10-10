#define AppVersion ReadIni(SourcePath +'./data.ini', 'Version', 'Ver', '1.0.0')
#define AppName ReadIni(SourcePath +'./data.ini', 'AppName', 'Name', 'app')
#define AppPublisher ReadIni(SourcePath +'./data.ini', 'AppPublisher', 'Publisher', 'Publisher')
#define Path ReadIni(SourcePath +'./data.ini', 'OutPath', 'Path', '.\Installer')
#define ReadPath ReadIni(SourcePath +'./data.ini', 'ReadPath', 'readPath', '{#Path}/StandaloneWindows')

[Setup]
AppId = {{67435A40-FD5B-4CBA-93FF-4709C2E7B9EC}
AppName= {#AppName}
AppVersion={#AppVersion}
AppVerName= {#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={pf}\{#AppPublisher}\{#AppName}
OutputBaseFilename= {#AppName}-{#AppVersion}
OutputDir={#Path}Installer
Compression=lzma2
SolidCompression=yes


WizardSmallImageFile = Resources\WizardSmallImage.bmp
WizardImageFile = Resources\WizardImage.bmp
SetupIconFile = Resources\SetupIcon.ico


UninstallDisplayIcon = {app}\{#AppName}.exe
DirExistsWarning = no
DisableWelcomePage = no
Uninstallable = yes
DisableProgramGroupPage = yes

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "ar"; MessagesFile: "compiler:Languages\Arabic.isl"
Name: "fr"; MessagesFile: "compiler:Languages\French.isl"
Name: "gr"; MessagesFile: "compiler:Languages\German.isl"
Name: "es"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "it"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "jp"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "ko"; MessagesFile: "compiler:Languages\Korean.isl"
Name: "po"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "ru"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "tu"; MessagesFile: "compiler:Languages\Turkish.isl"


[Tasks]
Name: "desktopicon"; Description: "Create a desktop icon"; GroupDescription: "Additional tasks:"; Flags: unchecked


[Files]
Source: "{#ReadPath}/*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{commonprograms}\{#AppName}"; Filename: "{app}\{#AppName}.exe"
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#AppName}.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppName}.exe"; Description: "Run {#AppName}"; Flags: postinstall nowait





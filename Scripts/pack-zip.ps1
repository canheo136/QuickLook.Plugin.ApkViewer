Remove-Item ..\QuickLook.Plugin.ApkViewer.qlplugin -ErrorAction SilentlyContinue

$files = Get-ChildItem -Path ..\bin\release\ -Exclude *.pdb,*.xml
Compress-Archive $files ..\QuickLook.Plugin.ApkViewer.zip
Move-Item ..\QuickLook.Plugin.ApkViewer.zip ..\QuickLook.Plugin.ApkViewer.qlplugin
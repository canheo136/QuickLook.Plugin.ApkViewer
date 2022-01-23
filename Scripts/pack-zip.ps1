Remove-Item ..\QuickLook.Plugin.ApkViewer.qlplugin -ErrorAction SilentlyContinue

$files = Get-ChildItem -Path ..\release\ -Exclude *.pdb,*.xml
$outputPlugin = '..\..\QuickLook.Plugin.ApkViewer.qlplugin'

Compress-Archive $files ..\QuickLook.Plugin.ApkViewer.zip -Force
Move-Item ..\QuickLook.Plugin.ApkViewer.zip $outputPlugin -Force

[string]::Concat("Packed plugin -> ", [System.IO.Path]::GetFullPath($outputPlugin))
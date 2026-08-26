$root = 'c:\Work\BlazorSVT'
$files = Get-ChildItem -Path $root -Recurse -Filter *.sql | Sort-Object FullName
foreach ($f in $files) {

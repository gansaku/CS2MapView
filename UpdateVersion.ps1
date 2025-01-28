Param([string]$project, [string]$newVersion)
$projectFullName = "CS2MapView.${project}"
$f = join-path $PSScriptRoot $projectFullName 
$n = "${projectFullName}.csproj"
$f = join-path $f $n

$doc = [xml](Get-Content $f -Encoding UTF8)
$node = $doc.SelectSingleNode("//Version")
$oldValue = $node.InnerText
$node.InnerText = $newVersion
$sw = new-object System.IO.FileStream @($f, [System.IO.FileMode]::Create)
$tw = new-object System.IO.StreamWriter @($sw, [System.Text.Encoding]::UTF8)
$doc.Save($tw)
$sw.Dispose()
echo "${projectFullName} version:${oldValue}->${newVersion}"
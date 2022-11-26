$json = Get-Content "../manifest.json" | ConvertFrom-Json
$version = $json.version_number
(Get-Content MaxDrawDistance.cs) -Replace "$version.0","0.0.0.0"  | Set-Content MaxDrawDistance.cs
(Get-Content MaxDrawDistancePlugin.csproj) -Replace "<Version>$version</Version>","<Version>0.0.0</Version>" | Set-Content MaxDrawDistancePlugin.csproj
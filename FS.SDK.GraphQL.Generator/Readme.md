# GraphQL Class Generator
 - Raymond Andreassen

## Etter gen: 
### 1. Project file 
´´´
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <PropertyGroup>
    <DefineConstants>GRAPHQL_GENERATOR_DISABLE_NEWTONSOFT_JSON</DefineConstants>
  </PropertyGroup>
  <ItemGroup Condition="!$(DefineConstants.Contains(GRAPHQL_GENERATOR_DISABLE_NEWTONSOFT_JSON))">
    <PackageReference Include="Newtonsoft.Json" Version="13.*" />
  </ItemGroup>

</Project>
´´´

### 2. Studieprogram.cs
```
    [System.Text.Json.Serialization.JsonPropertyName("beskrivelsesavsnitt")]
    public List<Studieprogrambeskrivelsesavsnitt> Beskrivelsesavsnitt { get; set; }
    //public StudieprogramBeskrivelsesavsnittConnection Beskrivelsesavsnitt { get; set; }
```
### 3. Gyldighetsperiode, circa 8 stk
```
    // IDatoperiode IPersonrolle.Gyldighetsperiode
```


## Rutine for generering
- appsettings.json må inneholde apikeyname og apikey
- Branch ut "ClassGenerator {dato}" eller lignende
- Kjør koden
- Kommenter ut // IDatoperiode IPersonrolle.Gyldighetsperiode
- Se hvilke filer som ble endret vha git
- Kanskje må også nuget pakke System.Text.Json legges til i GraphQL.Model



# GraphQL Class Generator
 - Raymond Andreassen

## TODO: 
- Lag meny
- Single file
- Multiple files 
- Delete all

## Generering av GraphQL-klasser skaper hvor mange feil: 
- 15.10.2025: 6075 - System.Text.Json vs Newtonsoft
- 15.10.2025: 9, men må legge til System.Text.Json manuelt. Løser compilatorfeil ved å kommentere ut.
- Resultat: 0 error, 0 warnings, 0 messages (nice!)
´´´
// IDatoperiode IPersonrolle.Gyldighetsperiode
´´´

## Mangler 
- IDatoPeriode må undersøkes. Den er kanskje ikke i bruk, eller den brukes feil. 

## Viktig

´´´
  <PropertyGroup>
    <DefineConstants>GRAPHQL_GENERATOR_DISABLE_NEWTONSOFT_JSON</DefineConstants>
  </PropertyGroup>
´´´

## Rutine for generering
- appsettings.json må inneholde apikeyname og apikey
- Branch ut "ClassGenerator {dato}" eller lignende
- Kjør koden
- Kommenter ut // IDatoperiode IPersonrolle.Gyldighetsperiode
- Se hvilke filer som ble endret vha git
- Kanskje må også nuget pakke System.Text.Json legges til i GraphQL.Model



function Println {
    Param([string]$text = "", [string]$fcolor = "White", [string]$bcolor = "Black")
    Write-Host $text -ForeGroundColor $fcolor -BackgroundColor $bcolor
}

function Setup-Secrets {
    Println "Creating secrets..." Green

    $db_password = Get-Content .\.local\db_password.txt
    $connection_string = "mongodb://minitwit:$db_password@localhost:27018"
    $jwt_key = Get-Content .\.local\jwt_key.txt

    dotnet user-secrets init --project MiniTwit.Server
    dotnet user-secrets set "ConnectionStrings:MiniTwit" $connection_string --project MiniTwit.Server
    dotnet user-secrets set "JwtSettings:Key" $jwt_key --project MiniTwit.Server

    Println "Done." Green
}

function Start-Backend {
    Println "Starting MongoDB..." Green

    $db_password = Get-Content .\.local\db_password.txt
    docker run --rm --name mongodb -d -p 27018:27017 -e "MONGO_INITDB_ROOT_USERNAME=minitwit" -e "MONGO_INITDB_ROOT_PASSWORD=$db_password" mongo:latest

    Println "Starting MiniTwit Backend..." Green
    dotnet run --project MiniTwit.Server
}

function Print-Help {
    Println "--- RUN HELP ---" Green
    Println "Available Commands:"
    Println ">> Run the MiniTwit Backend and datase"
    Println ">> secrets - Initialize .NET Secrets"
    Println ">> -h - Print this page"
    Println "----------------" Green
}

try {
    if ($args.Count -eq 0) {
        Start-Backend
    }
    elseif ($args[0] -eq "secrets") {
        Setup-Secrets
    }
    elseif ($args[0] -eq "-h") {
        Print-Help
    }
    else {
        Println "Error: Unknown command" Red
        Println "For a list of available commands type './run.ps1 -h'" Red
    }
}
finally {
    Println "Stopping and removing MongoDB..." Red
    docker stop mongodb
}
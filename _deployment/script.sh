az group create -n vsts123 -l westeurope
az group deployment create -g vsts123 --template-file template.json --parameters @template.parameters.json


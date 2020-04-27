### Getting started

1. install azure CLI
1. install terraform
1. az login
1. git clone this repo
1. navigate to the cloned root
1. CD into Prod or Dev
1. use the terraform commands to build or tear down Azure Resources

### Terraform commnds

```tf
terraform init 

terraform plan --var-file=./terraform.tfvars

```
terraform apply - deploys --var-file=./terraform.tfvars

terraform destroy - tears down --var-file=./terraform.tfvars


### TODO
1. Don't check in the tfvars file.  This has keys in it.  One should download it from a secure location.
2. Contact support about premissions issues (see below)
3. Post setup scripts.  Terraform allow you to run azure cli scripts and other scripts.  We could run those to better set up the environment.


### Premissions issues
1. Cant read Azure Storage Account.  I can create one but cannot read properties so the whole thing fails and stops deploying when that happens.
2. Can write a secrete to Key Vault. I can create a Key Vault I just cant add a secrete to it.
3. Blockchain member 'yyyxcryptokabs' doesn't have permission to join consortium 'cryptokicks'

# How it works
When you use Az login it select the default subscription if you have more than one.  You can choose to change the subscription if needed.  When you run Terraform Init it does a check of the terraform files.  Plan and Deploy creates a local file that keeps track of what has been deployed and what is planned on being deployed.  You can check this file into source to share it or there is a blob option as well.


### Cloud Shell
Instead of installing the Azure CLI, setting up a Service Principal and the rest of the Terraform Variables you can use the Azure Portal Cloud Shell.
We have made the Terraform experience as simple as possible, as all of the environment details are setup based on your default account through the Azure CLI.






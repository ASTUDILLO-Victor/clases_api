Your goal is to update any vulnerable dependencies.

Do the following:

    Run dotnet list package --vulnerable --include-transitive to find vulnerable installed packages in this project

    Run dotnet add package [Package_Name] --version [Secure_Version] to apply updates

    Run dotnet test and verify the updates didn't break anything
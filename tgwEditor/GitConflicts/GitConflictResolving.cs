using System;
using System.Collections;
using System.IO;
using System.Linq;
using LibGit2Sharp.Repository;

// This will be a class for git conflicts resolving

public class GitConflictResolving
{
   private 

        char* RemoteServerPath;

        int CurrentCommitSustain;

        // Used to determine all possible commit statuses
        public readonly int[] SustainID = {1,2,3,4,5,6};

        // For debugging info    
        public readonly string[] SustainDescription = { "Новая: добавлена новая запись в своих изменениях", 
                                                        "Удалена локально: удалена в своих изменениях",
                                                        "Удалена удалённо: запись была раньше в удалённом репозитории, в своих изменениях её не удаляли и не создавали.",
                                                        "Изменена: запись содержится в удалённом репозитории, но её содержание не соответствует содержанию в локальном.",
                                                        "Не изменена: содержимое и запись остались неизменны"};

        // Load remote Repo
        int LoadRemoteRepo(char* ServerPath)
        {
            // if no local repo existed, completely clone from server
            // Url of the remote repository to clone
            // Моя курсовая по БД =)
            string url = "https://github.com/it6mgupi/Cource_work_DB.git";

            // Location on the disk where the local repository should be cloned
            string workingDirectory = "C:\\projects\to_repo";

            // Perform the initial clone
            string repoPath = Repository.Clone(url, workingDirectory);

            using (var repo = new Repository(repoPath))
            {
               //     ... do stuff ...
            }

            // else, pull from server
            try
            {
                using (var repo = new Repository(@"D:\source\LibGit2Sharp"))
                {
                    Commit commit = repo.Lookup<Commit>("73b48894238c3e9c37f9f3a696bbd4bffcf45ce5");
                    Console.WriteLine("Author: {0}", commit.Author.Name);
                    Console.WriteLine("Message: {0}", commit.MessageShort);
                }
            }
            catch (InvalidCastException e) { }
        } 

    
    // Parse XML Files
    int XMLParser (String FileName, String LocalPath) {


    
    }

    // Checking for any Git conflicts
    int CheckForGitConflict(char* ServerPath) 
    { 
        
    }

    public void UpdateSustain(int CCS)
    {
        CurrentCommitSustain = ccs;
    }

	public int GitConflictResolving(int CurrentSustain)
	{
        switch (CurrentSustain){

            // Новая: просто добавляем запись с её локальным содержимым в удалённый репозиторий
            case 1: {
                        using (var repo = new Repository(@"D:\source\LibGit2Sharp")){
                                    //Commit commit = repo.Lookup<Commit>("73b48894238c3e9c37f9f3a696bbd4bffcf45ce5");
                        }
                        break;
                    }

            // Удалена локально: удаляем запись с её содержимым в удалённом репозитории
            case 2:
                    {

                        break;
                    }

            // Удалена удалённо: ничего не делаем
            case 3:
                    {
                        return 1;
                        break;
                    }

            // Изменена: добавляем/удаляем то, что было сделано с эти содержимым в локальных изменениях.
            case 4:
                    {

                        break;
                    }

            // Не изменена: не делаем ничего.
            case 5:
                    {
                        return 1;
                        break;
                    }
            // Error cannnot be described
            case 6:
                    {
                        return 0;
                        break;
                    }
        };
	}
}
